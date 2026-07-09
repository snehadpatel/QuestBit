# Architectural Specification: Memory Budget

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS

---

## 1. Design Intent & Requirements Traceability

The Memory Budget defines the maximum allowable physical RAM allocation and Mono Heap bounds for QuestBit. It prevents Out-of-Memory (OOM) browser crashes and mobile OS terminations:

* **WebGL Chromebook Memory Bounds (GDD §1.2 & §22)**: Chromebooks allocate a limited memory sandbox in the browser process. WebGL Wasm heap allocations must remain strictly **under 256MB** to prevent browser crashes.
* **Low-End Android/iOS Tablets (Vision §2 & GDD §1.2)**: Constrained mobile devices running older operating systems will terminate applications if memory consumption spikes. QuestBit enforces a strict **1.0GB physical RAM allocation target** for mobile.
* **Zero Garbage Collection (GC) Interruptions (Vision §2 & GDD §2.3)**: Memory churn triggers the C# Garbage Collector. Sweeping the heap causes micro-stuttering. QuestBit mandates **Object Pooling** and **Wasm Array Reuse** to keep runtime GC allocations flat.

---

## 2. Memory Allocation Budget Matrix

QuestBit divides memory limits into system layers to ensure target targets are met:

| Memory Segment | WebGL Baseline Target | Tablet Baseline Target | Notes / Strategy |
| :--- | :---: | :---: | :--- |
| **Unity Engine / Code Footprint** | **45MB** | **100MB** | Stripped engine DLLs, IL2CPP compilation. |
| **Textures (VRAM)** | **80MB** | **400MB** | Atlased textures, ASTC compression overrides. |
| **Meshes & Geometry** | **30MB** | **100MB** | Stripped unused vertex data, flat mesh models. |
| **Animation Clips & Rigs** | **15MB** | **30MB** | Keyframe reduction on import, joint limits. |
| **Pre-loaded Audio (SFX)** | **15MB** | **25MB** | Short clips only. Music/VO is streamed. |
| **Mono C# Garbage Collector Heap** | **25MB** | **50MB** | Pre-allocated pools, zero runtime allocations. |
| **UI Buffers & Canvas Meshes** | **20MB** | **50MB** | Dynamic canvas segregation, atlased UI. |
| **Physics2D Collision Data** | **20MB** | **40MB** | 2D Colliders, disabled 3D physics modules. |
| **Total Allocation Target** | **251MB** / **256MB** | **795MB** / **1.0GB** | Safety margins preserved for operating system spikes. |

---

## 3. Object Pooling Requirements

Dynamic instantiations (calling `Object.Instantiate()` at runtime) cause memory fragmentation and trigger GC sweeps. QuestBit mandates object pooling for all high-frequency or short-lived objects.

### 3.1 Pooled Object Categories
* **Math Planks (Tidewell Cove)**: Prefabs of fractional planks are pre-allocated at workbench initialization.
* **Dialogue Panel Elements**: Message nodes and choice buttons are recycled in a scroll list rather than created new.
* **UI Particle Feedback**: Pulses, sparkles, and chimes are pulled from a global particle pool.
* **Telemetry Data Packets**: Struct wrappers are written to recycled byte buffers using `System.Buffers.ArrayPool<byte>`.

### 3.2 Reusable Generic Object Pool Implementation (C#)

This allocation-free object pool operates on static generics, avoiding runtime dictionary lookups.

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace QuestBit.Core.Memory
{
    public interface IPooledObject
    {
        void OnReset();
    }

    public class ObjectPool<T> where T : Component, IPooledObject
    {
        private readonly T _prefab;
        private readonly Queue<T> _pool = new Queue<T>(32);
        private readonly Transform _parentTransform;

        public ObjectPool(T prefab, int initialSize, Transform parent)
        {
            _prefab = prefab;
            _parentTransform = parent;

            for (int i = 0; i < initialSize; i++)
            {
                T obj = Object.Instantiate(_prefab, _parentTransform);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                // Fallback allocation if pool is exhausted under heavy load
                Debug.LogWarning($"[Memory] Pool of type {typeof(T).Name} exhausted. Instantiating fallback instance.");
                obj = Object.Instantiate(_prefab, _parentTransform);
            }

            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            obj.OnReset();
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}
```

---

## 4. GC Spike Limits & Diagnostic Audits

* **GC Maximum Latency**: Under active play, GC runs are restricted to transition phases (loading screens). If a garbage collection sweep is triggered during gameplay, its execution time must not exceed **16.6ms** (1 frame at 60Hz).
* **ArrayPool Usage Rule**: For operations requiring temporary memory buffers (such as dialogue text JSON parsing, or networking payload serialization), developers must lease arrays from `System.Buffers.ArrayPool<char>.Shared` or `ArrayPool<byte>.Shared`, returning them upon operation completion:

```csharp
// Example of C# memory safety standard: zero-alloc parsing
char[] buffer = ArrayPool<char>.Shared.Rent(2048);
try
{
    // Execute string manipulation inside the leased buffer array
    ParseJsonToBuffer(rawJson, buffer);
}
finally
{
    ArrayPool<char>.Shared.Return(buffer);
}
```

---

## 5. Failure Modes & Edge Cases

### 1. WebGL Heap Exhaustion (Out of Memory)
* **Symptom**: WebGL build halts, browser displays error code: `Out of memory. If you are the developer of this content, double check your memory allocation settings.`
* **Cause**: Unity requests memory expansion beyond the browser's maximum allocated 256MB.
* **Mitigation**: Strictly compile WebGL builds with the **WebGL WebAssembly Memory Growth** setting disabled, forcing the engine to compile and run within the hard 256MB limit. This forces code validation checks and assets to remain small during development rather than relying on browser memory expansion.

### 2. Audio Buffer Bloat
* **Symptom**: Memory profiler reports audio assets consumption creeping toward 100MB.
* **Cause**: Narration voiceover files (which can be large) are loaded as pre-loaded assets rather than streamed.
* **Mitigation**: Configure the CI asset pipeline checker to reject imports of voiceover files if their Unity load type is set to anything other than `Streaming` (detailed in `10_asset_pipeline.md`).

---

## 6. Verification & Automated Memory Audits

1. **Mono Heap Allocation Unit Test**:
   A PlayMode integration test profiles the execution of the Tidewell Cove math bridge puzzle.
   * *Pass Criteria*: Solving three fraction gaps must result in **exactly 0 bytes** of GC memory allocation (monitored via `GC.GetTotalAllocatedBytes()`).

2. **Addressable Unload Verification**:
   Verify that unloading a biome scene (e.g. `sc_biome_math_tidewell`) and running the garbage collection routine successfully frees all referenced texture and mesh memory, returning the physical RAM footprint back to the Bramble hub baseline (795MB) within 5 seconds.
