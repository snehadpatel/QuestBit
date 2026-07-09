# Architectural Specification: Animation Pipeline

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS
* **Animation System**: **Unity Mecanim (Animator Controller)**

---

## 1. Design Intent & Requirements Traceability

The Animation Pipeline governs how character models, rigging data, and animation clips are processed and blended. It directly implements QuestBit's visual and mechanical design goals:

* **Animation-Carried Emotional Payload (Vision §6 & GDD §2.2.1 & Ch. 12)**: Mistakes do not use red flashes. Instead, characters pause, tilt their head, and try again, modeling curiosity. Successes trigger a joyful spin from the companion Spark (GDD §2.4.1).
* **Low-End Tablet Performance (Vision §2 & GDD §1.2 & §22)**: Dynamic character skinning is CPU-intensive. To prevent frame rate drops on low-end tablets and Chromebooks, skeletal joint counts, vertex weights, and active blend nodes must be strictly capped.
* **Foot-Slide Prevention (Vision §6)**: Locomotion must match world speed dynamically, preventing "foot sliding" that breaks storybook immersion.

---

## 2. Skeletal Rig & Mesh Specifications

To stay within our performance budgets, the technical art team must build all character models (Wayfinder, Spark, NPCs, and Challenge Creatures) to meet these limits:

* **Maximum Bones per Character Rig**: **30 joints** (excluding hair/cloth dynamics which are handled via simple shaders, not bones).
* **Maximum Vertices per Character Mesh**: **5,000 triangles**.
* **Vertex Influence Limit**: Max **2 bones per vertex** (reduces GPU skinning complexity in WebGL).
* **Animation Compression Preset**: Unity's `Keyframe Reduction` is set to:
  * Rotation Error: `0.5` degrees.
  * Position Error: `0.05` units.
  * This strips up to **75% of redundant keyframe data** on import, meeting our strict memory bounds.

---

## 3. Animator Controller & Blend Tree Structure

The Wayfinder character uses a single unified Animator Controller with a 1D Locomotion Blend Tree and an overlay layer for emotional expressions.

### 3.1 Blend Tree Layout (Locomotion)
* **Parameter**: `ForwardSpeed` (float, range: 0.0 to 4.5)
* **States**:
  * `0.0` - `anim_wayfinder_idle` (Calm breathing, visual loop)
  * `1.5` - `anim_wayfinder_walk` (Casual pace, structured steps)
  * `3.5` - `anim_wayfinder_jog` (Auto-jog state on long touch holds, GDD §2.2.1)

```text
Animator Layers
├── Base Layer (Sync: None)
│   └── Locomotion (1D Blend Tree: Idle -> Walk -> Jog)
└── Expression Layer (Sync: Additive, Weight: Dynamic)
    ├── State: Empty (Default)
    ├── State: anim_wayfinder_curious (Trigger: "OnMistake")
    └── State: anim_wayfinder_joy (Trigger: "OnSuccess")
```

---

## 4. Dynamic Speed Matching & Procedural Head-Look

To eliminate foot sliding and increase immersion, character controllers run dynamic math calculations and procedural adjustments inside the animation loop.

### 4.1 Foot-Slide Prevention Implementation

This script adjusts the animator speed parameter based on the physical character velocity.

```csharp
using UnityEngine;

namespace QuestBit.Gameplay.Animation
{
    [RequireComponent(typeof(Animator), typeof(CharacterController))]
    public class LocomotionSpeedMatcher : MonoBehaviour
    {
        private Animator _animator = null!;
        private CharacterController _controller = null!;
        
        private static readonly int SpeedParamHash = Animator.StringToHash("ForwardSpeed");
        private static readonly int PlaybackSpeedParamHash = Animator.StringToHash("FootPlaybackSpeed");

        [SerializeField] private float _walkFootstepStride = 1.2f; // Stride length in meters
        [SerializeField] private float _jogFootstepStride = 2.0f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // Calculate horizontal speed in world space
            Vector3 velocity = _controller.velocity;
            velocity.y = 0; // Ignore vertical fall/climb velocity
            float currentSpeed = velocity.magnitude;

            // 1. Send absolute speed to the blend tree parameter
            _animator.SetFloat(SpeedParamHash, currentSpeed);

            // 2. Adjust playback speed dynamically to match world movement
            float targetStride = currentSpeed > 2.0f ? _jogFootstepStride : _walkFootstepStride;
            float animationFrequency = currentSpeed / targetStride;
            
            // Limit animation playback speed bounds to prevent hyper-speed running
            float clampPlaybackSpeed = Mathf.Clamp(animationFrequency, 0.5f, 1.8f);
            _animator.SetFloat(PlaybackSpeedParamHash, clampPlaybackSpeed);
        }
    }
}
```

### 4.2 Procedural Head-Look (Observe Target Tracking)
* **Implementation**: The character's head bone (joint index 12) is procedurally rotated to look at the currently focused `IScanTarget` (GDD §15.2).
* **Limit**: The rotation is clamped to a maximum offset angle of **45 degrees** left/right and **20 degrees** up/down. This uses a simple `Quaternion.Slerp` rotation on the bone transform in `LateUpdate`, avoiding expensive Inverse Kinematics (IK) solvers that drag down low-end tablet CPUs.

---

## 5. Memory Budgets

Because animation data is cached in RAM during scene playback, strict limits are placed on active animations:

* **Active Memory Budget for Animation Rigs & Clips**: **<30MB RAM** (verified by the Unity Profiler).
* **Maximum Concurrent Rig Instances**: **15 active skinned mesh renderers** visible in the camera viewport at once (including Player, Spark, and active NPCs).

---

## 6. Failure Modes & Edge Cases

### 1. Root Motion Discrepancy (Drifting Character)
* **Symptom**: The character model physically floats or drifts away from their collision box when an animation plays.
* **Prevention**: Disallow "Apply Root Motion" in the Unity Animator component settings for player-controlled rigs. All character translation must be driven by the C# `CharacterController` component, with the animation matched visually.

### 2. State Machine Transition Hangs (Infinite Loops)
* **Symptom**: Character gets stuck in the "curious head tilt" animation and won't return to the locomotion blend tree.
* **Prevention**: All emotional override states in the Expression layer must define a **Fixed Duration (Exit Time)** of exactly **1.5 seconds**, automatically returning to the empty state without depending on manual C# script triggers.

---

## 7. Verification & Profiling Tests

1. **Rig Memory Overhead Test**:
   Build an Editor Unit Test that loads the base Wayfinder character prefab and all biome NPCs.
   * *Pass Criteria*: The combined size of all referenced `AnimationClip` assets in active memory must not exceed **20MB** (monitored via `Profiler.GetRuntimeMemorySizeLong`).

2. **FPS Verification under Load**:
   A QA automation script spawns the maximum active character rig count (15 rigs) in a biome and forces all of them to execute blend tree transitions simultaneously.
   * *Pass Criteria*: Frame rate must remain above **60 FPS** on iPad baselines.
