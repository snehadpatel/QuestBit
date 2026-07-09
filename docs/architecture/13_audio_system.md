# Architectural Specification: Audio System Architecture

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS

---

## 1. Design Intent & Requirements Traceability

The Audio System manages soundtrack transitions, sound effects (SFX), and localization voiceovers (VO). It directly supports QuestBit's tonal and accessibility goals:

* **Non-Anxiety Sound Design (Vision §7 & GDD §11.1)**: Avoid harsh buzzer sounds, alarm bells, or countdown ticks. Successes trigger resolving major chords; mistakes trigger open, neutral woodwind/string motifs.
* **Full Voiceover Narration (Vision §7 & §8 & GDD §11.1 & §15.2)**: Direct support for narrated reading assistance. VO playback must run in sync with subtitle display speeds and support custom speed adjustments (Standard, Slow, Fast).
* **Calm Mode Frequency Filtering (Vision §8 & GDD §15.4)**: In Calm Mode, the system must apply a low-pass filter to environmental SFX and music, reducing sudden high-frequency spikes and lowering overall volume.
* **Low-End Memory Budgets (Vision §2 & GDD §22)**: Audio assets can consume significant RAM. The pre-loaded audio buffer is capped at **<25MB RAM**, with all larger files streamed directly from local storage.

---

## 2. Audio Routing & Mixer Architecture

QuestBit uses a hierarchical **Unity AudioMixer** setup. The output signal flows through specialized sub-busses to allow real-time volume overrides and DSP (Digital Signal Processing) effects.

```text
AudioMixer Master Output
├── Music Bus (DSP: Low-Pass Filter, Send: Sidechain)
├── VO/Speech Bus (Priority routing, Ducking trigger)
└── SFX Bus (DSP: Low-Pass Filter)
    ├── Footsteps Sub-Bus
    ├── UI Sub-Bus
    └── Environment Sub-Bus
```

### 2.1 Audio Bus Parameter Specification

* **VO Ducking (Sidechain)**: When the VO Bus registers audio output, the Music Bus volume is dynamically ducked by **-12dB** and environmental SFX by **-10dB** to guarantee speech legibility for pre-readers.
* **Calm Mode DSP**: Triggering Calm Mode enables a **Lowpass Simple** filter on both the Music and SFX Busses, cutting off all frequencies above **1500Hz** and decreasing master volume by **-6dB**.

---

## 3. Audio Manager API & C# Implementation

The Audio Manager handles voice allocation, clip playback, and accessibility state changes.

### 3.1 Interface Contracts

```csharp
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace QuestBit.Systems.Audio
{
    public interface IAudioManager
    {
        void PlayMusic(AudioClip musicClip, float fadeDurationSeconds);
        void StopMusic(float fadeDurationSeconds);
        
        void PlaySFX(AudioClip sfxClip, Vector3 worldPosition, float pitchVar = 0.05f);
        
        UniTask PlayVoAsync(string voAddressableKey, float playbackSpeed);
        void StopVo();
        
        void ToggleCalmFilters(bool isCalmActive);
    }
}
```

### 3.2 Audio Manager Implementation

```csharp
using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;
using Addressables = UnityEngine.AddressableAssets.Addressables;

namespace QuestBit.Systems.Audio
{
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField] private AudioMixer _masterMixer = null!;
        [SerializeField] private AudioSource _musicSource = null!;
        [SerializeField] private AudioSource _voSource = null!;
        
        [Header("Voice Pool Setup")]
        [SerializeField] private AudioSource[] _sfxSourcesPool = null!; // pre-allocated voices (Max 16)
        private int _poolIndex;

        public void PlayMusic(AudioClip musicClip, float fadeDurationSeconds)
        {
            if (musicClip == null) return;
            // Crossfade logic managed via UniTask transitions
            FadeMusicAsync(musicClip, fadeDurationSeconds).Forget();
        }

        public void StopMusic(float fadeDurationSeconds)
        {
            FadeMusicAsync(null, fadeDurationSeconds).Forget();
        }

        public void PlaySFX(AudioClip sfxClip, Vector3 worldPosition, float pitchVar = 0.05f)
        {
            if (sfxClip == null) return;

            // Simple round-robin voice allocation in the pre-allocated pool
            var source = _sfxSourcesPool[_poolIndex];
            _poolIndex = (_poolIndex + 1) % _sfxSourcesPool.Length;

            source.transform.position = worldPosition;
            source.clip = sfxClip;
            source.pitch = Random.Range(1.0f - pitchVar, 1.0f + pitchVar);
            source.Play();
        }

        public async UniTask PlayVoAsync(string voAddressableKey, float playbackSpeed)
        {
            _voSource.Stop();

            // Load and stream voice clip dynamically to conserve memory
            var handle = Addressables.LoadAssetAsync<AudioClip>(voAddressableKey);
            var clip = await handle.ToUniTask();

            if (clip != null)
            {
                _voSource.clip = clip;
                _voSource.pitch = playbackSpeed; // Standard, Slow, or Fast pitch matching
                _voSource.Play();

                // Wait for playback to complete before releasing addressable asset handle
                await UniTask.Delay(System.TimeSpan.FromSeconds(clip.length / playbackSpeed));
                Addressables.Release(handle);
            }
        }

        public void StopVo()
        {
            _voSource.Stop();
        }

        public void ToggleCalmFilters(bool isCalmActive)
        {
            // Transition AudioMixer parameters smoothly to prevent pops
            float targetCutoff = isCalmActive ? 1500f : 22000f; // Cut off high frequencies above 1500Hz
            float targetVolume = isCalmActive ? -6f : 0f;

            _masterMixer.SetFloat("SFXCutoff", targetCutoff);
            _masterMixer.SetFloat("MusicCutoff", targetCutoff);
            _masterMixer.SetFloat("MasterVolume", targetVolume);

            Debug.Log($"[Audio] Calm Filters updated. Active: {isCalmActive}");
        }

        private async UniTaskVoid FadeMusicAsync(AudioClip? newClip, float duration)
        {
            float elapsed = 0f;
            float startVolume = _musicSource.volume;

            // Fade Out
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                await UniTask.Yield();
            }

            _musicSource.Stop();
            _musicSource.clip = newClip;

            if (newClip != null)
            {
                _musicSource.Play();
                elapsed = 0f;
                
                // Fade In
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    _musicSource.volume = Mathf.Lerp(0f, startVolume, elapsed / duration);
                    await UniTask.Yield();
                }
            }
        }
    }
}
```

---

## 4. Voice Allocation & Memory Strategy

To fit within our strict memory parameters, QuestBit implements an **Audio Streaming and Voice Limit Policy**.

### 4.1 Audio Compression Settings

* **Short SFX (<1.5s)**: Imported as uncompressed WAV, loaded directly into memory at biome start.
* **Music & Ambient Loops**: Ogg Vorbis compressed format. Configured with **"Stream"** load type, bypassing the memory heap and streaming directly from the disk cache.
* **VO Narration**: Ogg Vorbis compressed format. Loaded on-demand via **Addressables dynamic handle unloading** (unloaded from RAM as soon as the line completes playback).

### 4.2 Voice Limit Budget
* **Maximum Concurrent SFX Voices**: **16 concurrent voices** (capped via pre-allocated pool). Attempts to trigger a 17th SFX will overwrite the oldest active voice in the pool, avoiding CPU spikes.

---

## 5. Failure Modes & Edge Cases

### 1. Sound "Machine-Gunning" (Overlap Overload)
* **Symptom**: Rapid player action (e.g. clicking fractions quickly) triggers the solve SFX 10 times in a single second, creating a loud, harsh acoustic spike.
* **Mitigation**: Implement a **50ms SFX cooldown** per clip type. If the same SFX type is triggered within 50ms of a previous playback, discard the request.

### 2. VO Asset Missing in Localized Build
* **Symptom**: Dialogue script executes, subtitles display, but no voiceover plays.
* **Mitigation**: The `PlayVoAsync` method validates the addressable key. If key check fails, it immediately logs a warning, falls back to an unvoiced state, and fires a subtitle-advance signal to ensure pre-readers aren't blocked by missing audio files.

### 3. WebGL Audio Context Suspended
* **Symptom**: WebGL builds play with zero sound.
* **Cause**: Browsers suspend audio contexts until the user interacts with the page (security rule).
* **Mitigation**: In the boot scene, detect if `AudioSettings.dspTime` is halted. Bind a listener to the first mouse click or touch event on the page to automatically execute `AudioContext.resume()` via Javascript.

---

## 6. Verification & Automated QA

1. **Memory Budget Verification Test**:
   An automated test loads a biome scene, triggers music playback, and plays 16 concurrent SFX clips.
   * *Pass Criteria*: Total audio asset memory in use must not exceed **25MB** (verified by `Profiler.GetRuntimeMemorySizeLong`).

2. **Ducking & Lowpass Verification**:
   Assert that triggering the VO AudioSource decreases the volume parameters of the Music and SFX mixer groups to their target ducking levels within **200ms**.
