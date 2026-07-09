# Architectural Specification: C# Coding Standards

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS (C# 11 / .NET Standard 2.1)

---

## 1. Design Intent & Requirements Traceability

These coding standards establish code quality, predictability, and memory safety requirements for all engineers on the QuestBit project:

* **Low-End Hardware Pacing (Vision §2 & GDD §1.2)**: Chromebooks and lower-end Android tablets have highly constrained CPU, thermal, and memory parameters. Code must execute with zero heap allocations in hot paths (Update/FixedUpdate loops) to prevent Garbage Collection (GC) latency spikes that disrupt gameplay.
* **Cozy, Crash-Free Gameplay (Vision §2 & GDD §2.3)**: Educational gameplay must not be interrupted by technical failures. The codebase must enforce strict null-safety checks (`#nullable enable`), runtime validation, and robust error recovery so that the game degrades gracefully rather than throwing unhandled exceptions.
* **WebGL Platform Safety (GDD §1.2)**: WebGL builds run in a single-threaded WebAssembly environment. Native C# multi-threading APIs (`System.Threading.Thread`, raw `System.Threading.Tasks.Task` with background threads) are unsupported. Coding standards must require WebGL-safe async execution models.

---

## 2. Formatting & Naming Styles

QuestBit follows the Microsoft C# formatting guidelines with additions tailored for Unity-specific serialization.

### 2.1 Code Layout Rules
* **Indentation**: Use 4 spaces for indentation (no tabs).
* **Line Length Limit**: Maximum 120 characters per line.
* **Braces**: Use Egyptian style or K&R style braces for short methods; always use Allman (newline) braces for class, namespace, and standard method definitions.

```csharp
namespace QuestBit.Gameplay
{
    public class FractionBridge : MonoBehaviour
    {
        private int _plankCount;

        public void AddPlank()
        {
            if (_plankCount >= 5)
            {
                Debug.LogWarning("Bridge capacity reached.");
                return;
            }
            _plankCount++;
        }
    }
}
```

### 2.2 Case and Prefix Rules

| Element | Style | Example | Notes |
| :--- | :--- | :--- | :--- |
| **Classes / Structs / Interfaces** | PascalCase | `FractionBridge`, `ISaveable` | Interfaces must always start with an uppercase `I`. |
| **Methods** | PascalCase | `CalculateSpan()` | Descriptive verbs. |
| **Properties** | PascalCase | `CurrentLength` | No auto-property allocations in hot paths. |
| **Public Variables** | PascalCase | `TargetGap` | Used for Unity-serialized inspector fields. |
| **Private Fields** | `_camelCase` | `_spawnedPlanks` | Must prefix with an underscore. |
| **Local Variables** | camelCase | `newLength` | Keep names descriptive; no single-letter names except in loops. |
| **Constants / Readonly** | ALL_CAPS | `MAX_SPAN_TILES` | Explicitly marked `const` or `static readonly`. |

---

## 3. Memory Management & Zero-Allocation Standards

GC sweeps are the primary source of frame rate stutter on low-end mobile devices and Chromebooks. QuestBit mandates a **Zero-Allocation Hot-Path Policy**.

### 3.1 Hot-Path Restrictions
Hot paths are defined as any code block executed in `Update()`, `LateUpdate()`, `FixedUpdate()`, or inside high-frequency event listeners.
* **No `new` allocations**: Do not allocate objects, arrays, or anonymous classes.
* **No LINQ**: Do not use LINQ queries (e.g., `.Where()`, `.Select()`, `.ToList()`) in hot paths. LINQ generates enumerator objects on the heap.
* **No string concatenations**: Avoid `"String " + val` or `string.Format()`. Use cached strings, custom numeric-to-text converters, or `StringBuilder` pre-allocated at startup.
* **No box allocation**: Do not cast value types (structs, enums) to reference types (object). Use custom generic comparisons where necessary.
* **Zero physics raycast allocations**: Use `Physics2D.RaycastNonAlloc` instead of `Physics2D.RaycastAll`. Pre-allocate the results array.

### 3.2 Unity API Gotchas
Many built-in Unity properties allocate arrays under the hood.
* **Avoid `transform.position` loops**: Cache positions locally if accessed multiple times in a single frame.
* **Avoid raw tag checks**: Never use `gameObject.tag == "Player"`. This allocates a string copy of the tag. Use `gameObject.CompareTag("Player")` which executes inside native C++ memory without heap allocation.
* **Avoid component lookups in loops**: Never call `GetComponent<T>()` in an Update loop. Cache all references during `Awake()` or `Start()`.

---

## 4. Null-Safety & Error Handling

To support the GDD's goal of uninterrupted play, the codebase must enforce compile-time safety and graceful degradation.

### 4.1 Nullable Reference Types (`#nullable enable`)
All new scripts must enable nullable annotations to prevent NullReferenceExceptions (NRE) at runtime.

```csharp
#nullable enable
namespace QuestBit.Gameplay
{
    public class SparkController : MonoBehaviour
    {
        // Explicitly marked as nullable because the Spark might not be hatched yet.
        private SparkModel? _activeSpark; 

        public void TriggerHint(Vector3 targetPosition)
        {
            // Compile-time check forces us to handle the null state.
            if (_activeSpark == null)
            {
                Debug.LogWarning("Cannot trigger hint: Spark is not active.");
                return; 
            }

            _activeSpark.MoveTo(targetPosition);
        }
    }
}
```

### 4.2 Unity Object Null Comparison
Unity overloads the `==` operator for `UnityEngine.Object`. Checking `myMonoBehaviour == null` calls native code to check if the C++ object was destroyed, which has significant CPU overhead.
* **Best Practice**: In hot paths, use `ReferenceEquals(myObject, null)` for pure CPU null-checks, or cache the lifetime state of the object in a boolean.

### 4.3 Error Propagation (The Result Pattern)
Avoid using exceptions (`try-catch` blocks) for standard flow control. Exceptions disrupt WebGL assembly pipelines and are slow. Use a `Result<T>` pattern for parsing data, loading saves, or evaluating math tools.

```csharp
public struct Result<T>
{
    public bool Success { get; }
    public T Value { get; }
    public string ErrorMessage { get; }

    private Result(bool success, T value, string errorMessage)
    {
        Success = success;
        Value = value;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Ok(T value) => new Result<T>(true, value, string.Empty);
    public static Result<T> Fail(string error) => new Result<T>(false, default!, error);
}
```

---

## 5. WebGL Async Standard: UniTask

Because WebGL compiles to a single-threaded WebAssembly binary, standard C# threading classes (`Task.Run`, `async/await` running on ThreadPool) will block or fail.
* **Rule**: All asynchronous operations (save loading, networking, asset streaming, screen transitions) must use **`UniTask`** (an allocation-free Unity-optimized async/await package).
* **Task Allocation mitigation**: `UniTask` uses value-type tasks (`ValueTask` equivalents), which compile without allocating objects on the heap.

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BiomeLoader : MonoBehaviour
{
    public async UniTask<Result<bool>> LoadBiomeAsync(string biomeAddressableKey)
    {
        try
        {
            // Allocation-free async operation linked to Unity's frame pipeline
            await Addressables.LoadSceneAsync(biomeAddressableKey).ToUniTask();
            return Result<bool>.Ok(true);
        }
        catch (System.Exception ex)
        {
            return Result<bool>.Fail($"Failed to load biome: {ex.Message}");
        }
    }
}
```

---

## 6. Failure Modes & Edge Cases

### 1. WebAssembly Stack Overflow
* **Symptom**: WebGL build crashes with `Stack Overflow` error in the browser console.
* **Cause**: Excessive recursive methods or too many nested await callbacks running on a single frame.
* **Prevention**: Do not write recursive functions for layout checks or pathfinding. Use iteration loops with explicit collections pooled from `System.Buffers`.

### 2. Serialized Field Nullability
* **Symptom**: Non-nullable field marked `[SerializeField]` throws compiler warnings or runtime nulls when not assigned in the inspector.
* **Prevention**: Mark serialized fields as nullable if they can be unassigned, or use the `null!` assignment pattern to tell the compiler it is managed by the Unity Inspector.

```csharp
[SerializeField] private Transform _targetAnchor = null!; // Tells compiler it will be populated by Unity
```

---

## 7. Verification & Automated Enforcement

1. **Editorconfig Validation**: An `.editorconfig` file is enforced at the root of the repository to validate spacing, indentations, and naming schemas on compile.
2. **Roslyn Analyzers (Compilation Blockers)**: Add custom static code analysis packages to the project:
   * **Microsoft.CodeAnalysis.NetAnalyzers**: Standard code style checking.
   * **UniTask.Analyzers**: Catches unawaited UniTasks or threads used in WebGL.
   * **BannedApiAnalyzers**: Bans the compile-time usage of `LINQ` or `System.Threading.Tasks.Task` inside game scripts.
3. **Automated Formatting Checks**: The CI/CD pipeline (`lint_check.yml`) runs `dotnet format` on every pull request, blocking builds that fail to conform.
