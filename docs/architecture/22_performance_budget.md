# Architectural Specification: Performance Budget

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS

---

## 1. Design Intent & Requirements Traceability

The Performance Budget sets strict limits on frame times, CPU cycles, GPU passes, and scene load times across all target platforms. It ensures the game runs smoothly without causing system strain:

* **Mass-Market Educational Reach (Vision §2 & GDD §1.2 & §22)**: QuestBit targets schools and families, meaning it must run reliably on low-end school Chromebooks and older household tablets. Performance must degrade gracefully without draining batteries or causing thermal issues.
* **Cozy Flow Maintenance (Vision §2 & GDD §2.2.1)**: Stuttering or low frame rates break visual immersion and cause cognitive fatigue. The rendering and scripting pipelines must maintain a stable frame rate under a zero-overhead target.

---

## 2. Frame-Time and Load-Time Budgets

QuestBit targets four hardware tiers. Budgets are capped to prevent frame drops.

### 2.1 Hardware Tier Performance Matrix

| Metric | Tier 1: WebGL Chromebook | Tier 2: Low-end Tablet | Tier 3: Mobile (iPhone/Android) | Tier 4: Console / Smart TV |
| :--- | :--- | :--- | :--- | :--- |
| **Target Frame Rate** | 30 FPS minimum | 30 FPS minimum | 60 FPS target | 60 FPS target |
| **Total Frame Time** | **<33.3ms** | **<33.3ms** | **<16.6ms** | **<16.6ms** |
| **Max CPU Allocation**| **<10.0ms** | **<15.0ms** | **<8.0ms** | **<10.0ms** |
| **Max GPU Allocation**| **<20.0ms** | **<18.0ms** | **<8.0ms** | **<6.0ms** |
| **Base Resolution** | 1280x720 (720p) | 1920x1080 (1080p) | 1920x1080 (1080p) | 1920x1080 (1080p) |
| **Post-Processing** | Low (Color LUT) | Medium (LUT + Bloom) | Medium (LUT + Bloom) | High (LUT+Bloom+SMAA) |

### 2.2 Scene Load & Cold Boot Budgets
* **Cold Boot to Title Screen**: **<5.0 seconds** (loading DI, Save, and Localization).
* **Scene Transition (Hub to Biome)**: **<3.0 seconds** (loading screen fade window, GDD §9.2).
* **Pause Menu Open**: **<0.1 seconds** (UI must instantiate instantly for visual responsiveness).

---

## 3. Subsystem CPU Pacing Budget (Target: Tier 2 Tablet)

To keep CPU frame times under the 15ms target on low-end tablets, active systems are allocated a maximum execution time per frame.

```text
Tier 2 Tablet Frame Time (33.3ms)
├── Allocated CPU Frame Budget (15.0ms)
│   ├── Scripting/Updates: 5.0ms
│   ├── Physics2D: 2.0ms
│   ├── AI System (Throttled): 1.5ms
│   ├── Audio Mixing & Dispatch: 1.0ms
│   ├── UI Canvas Layout: 2.0ms
│   └── Unity Render Engine: 3.5ms
└── Unallocated CPU Margin: 18.3ms (System safety buffer)
```

* **Scripting/Logic Updates**: Max **5.0ms** (zero garbage collection allocations).
* **Physics 2D Updates**: Max **2.0ms** (clamped to a fixed update frequency of **30Hz** rather than 50Hz, reducing collision calculations).
* **AI System**: Max **1.5ms** (throttled to a 5Hz update frequency, GDD §20.4).
* **Audio Engine**: Max **1.0ms** (capped at 16 concurrent voices).
* **UI Layout Updates**: Max **2.0ms** (UI canvas updates are isolated to prevent dirtying the entire screen layout).
* **Unity Render Core (Draw Call Prep)**: Max **3.5ms**.

---

## 4. Thermal Throttling & Battery Mitigation

To prevent mobile devices from heating up and draining batteries, the client dynamically scales graphics settings based on temperature alerts from the operating system.

### 4.1 Thermal Mitigation Controller

This C# script checks the device's thermal status and adjusts resolution and frame rates to keep the device cool.

```csharp
using UnityEngine;
using QuestBit.Core.EventBus;
using QuestBit.UI.Events;

namespace QuestBit.Gameplay.Performance
{
    public class ThermalMitigationController : MonoBehaviour
    {
        private float _checkTimer;
        private const float CHECK_INTERVAL_SECONDS = 5f; // Check temperature every 5 seconds

        private void Start()
        {
            EvaluateThermalStatus(Application.thermalStatus);
        }

        private void Update()
        {
            _checkTimer += Time.deltaTime;
            if (_checkTimer >= CHECK_INTERVAL_SECONDS)
            {
                _checkTimer = 0f;
                EvaluateThermalStatus(Application.thermalStatus);
            }
        }

        private void EvaluateThermalStatus(ThermalStatus status)
        {
            switch (status)
            {
                case ThermalStatus.Nominal:
                    RestoreHighPerformance();
                    break;
                case ThermalStatus.Throttling:
                    ApplyMildThrottlingMitigation();
                    break;
                case ThermalStatus.Serious:
                    ApplySeriousThrottlingMitigation();
                    break;
                case ThermalStatus.Critical:
                    ApplyCriticalThrottlingMitigation();
                    break;
            }
        }

        private void RestoreHighPerformance()
        {
            // Restore full quality (60 FPS, Native Resolution scale 1.0)
            Application.targetFrameRate = 60;
            QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
            Debug.Log("[Performance] System running nominally. High performance mode active.");
        }

        private void ApplyMildThrottlingMitigation()
        {
            // Slightly decrease resolution scale to 0.85x
            QualitySettings.resolutionScalingFixedDPIFactor = 0.85f;
            Debug.LogWarning("[Performance] Device is warming up. Applying minor resolution scaling.");
        }

        private void ApplySeriousThrottlingMitigation()
        {
            // Decrease resolution scale to 0.7x, drop target frame rate to 30 FPS
            Application.targetFrameRate = 30;
            QualitySettings.resolutionScalingFixedDPIFactor = 0.70f;
            
            // Disable non-essential rendering volume features (e.g. Bloom)
            DisableHeavyRenderFeatures();
            
            Debug.LogWarning("[Performance] Serious thermal throttling active! Frame rate locked to 30 FPS, Resolution scaled to 70%.");
        }

        private void ApplyCriticalThrottlingMitigation()
        {
            // Force lowest settings to prevent application crash or device shutdown
            Application.targetFrameRate = 30;
            QualitySettings.resolutionScalingFixedDPIFactor = 0.50f;
            
            DisableHeavyRenderFeatures();
            
            // Dispatch event to force Calm Mode visuals (turns off particle emitters)
            // EventBus.Publish(new OnAccessibilitySettingsChangedEvent(..., calmVisuals: true, ...));

            Debug.LogError("[Performance] CRITICAL thermal state! Forcing lowest quality profiles.");
        }

        private void DisableHeavyRenderFeatures()
        {
            // Direct reference to toggle URP post-processing passes
        }
    }
}
```

---

## 5. Failure Modes & Edge Cases

### 1. WebGL Tab Crashes (OOM)
* **Symptom**: The browser crashes with an Out of Memory error during scene transitions.
* **Mitigation**: Strictly compile the WebGL player with **Wasm memory limits** enabled. Run asset unloading routines before loading new biomes to clear system memory, as detailed in `09_scene_structure.md`.

### 2. Physics Spikes (Frame Rate Drops)
* **Symptom**: Spawning multiple planks on the math workbench causes a physics calculation spike, dropping the frame rate to single digits.
* **Mitigation**: Disable 3D physics entirely. Set `Physics2D.simulationMode` to manual script execution, ticking the physics engine inside a fixed loop only when interactive planks are actively moving, keeping idle physics calculations at zero.

---

## 6. Verification & Automated Auditing

1. **Automated Frame Rate Audit (CI/CD)**:
   A QA automation script loads every biome scene and walks the player character along a pre-recorded path.
   * *Pass Criteria*: The average frame rate must remain above **30 FPS** on Chromebooks and **60 FPS** on mobile tablets, with zero frames exceeding **33.3ms** (Tier 2 baseline).

2. **Thermal State Simulation Assertion**:
   Unit tests simulate a `ThermalStatus.Serious` state. Verify that the system dynamically scales the resolution rendering factor to **0.7x** and locks the target frame rate to **30 FPS** within 5 seconds.
