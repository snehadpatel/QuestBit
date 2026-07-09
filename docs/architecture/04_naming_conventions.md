# Architectural Specification: Naming Conventions

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS

---

## 1. Design Intent & Requirements Traceability

This document establishes a standard naming convention for all code, asset files, and scene objects. Clean naming is crucial for automated asset validation and reliable runtime searching:

* **WebGL Case-Sensitivity (GDD §1.2)**: WebGL builds are served from web servers (Linux/Unix-based) where paths are strictly case-sensitive. Inconsistent capitalization in paths or asset references will cause runtime load failures in browsers.
* **Biome-as-Curriculum Asset Bundling (Vision §6 & GDD §3)**: Textures, materials, and models must be easily filterable via scripts so the CI/CD pipeline can automatically parse and bundle them into the correct Addressable groups.
* **Localization & Audio Mapping (Vision §7 & GDD §7.3)**: Audio voiceovers and text files must map to identical locale keys to automate the sync between subtitles, spoken dialogue, and dyslexia overrides.

---

## 2. Code Naming Conventions

All C# source files, classes, methods, and variables must follow this system to maintain clean compiler output and IDE autocomplete.

### 2.1 C# Code Symbols

| Symbol Type | Casing Style | Prefix | Suffix | Example |
| :--- | :--- | :---: | :---: | :--- |
| **Namespace** | PascalCase | None | None | `QuestBit.Gameplay.Math` |
| **Class** | PascalCase | None | None | `PlankBuilder` |
| **Interface** | PascalCase | `I` | None | `IInteractable` |
| **Struct** | PascalCase | None | None | `SaveData` |
| **Enum** | PascalCase | None | None | `SubjectDomain` |
| **Enum Values** | PascalCase | None | None | `NumberSense`, `Phonics` |
| **Method** | PascalCase | None | None | `CalculateLength()` |
| **Property** | PascalCase | None | None | `CurrentGap` |
| **Event (Action)** | PascalCase | `On` | None | `OnPlankPlaced` |
| **Public Field** | PascalCase | None | None | `StartingGap` |
| **Private Field** | camelCase | `_` | None | `_spawnedPlanks` |
| **Local Variable** | camelCase | None | None | `remainingSpan` |
| **Constant / Readonly** | ALL_CAPS | None | None | `MAX_PLANK_COUNT` |

### 2.2 Script File Naming
* **Rule**: The script file name must exactly match the name of the primary class or struct declared inside it.
* *Example*: `class FractionBridge` must be stored in a file named `FractionBridge.cs`.

---

## 3. Asset File Naming Conventions

Asset files must use lowercase abbreviations with underscores (snake_case) to prevent WebGL server case-sensitivity failures.

### 3.1 Asset Prefix Table

All files imported into the `Assets/` directory must be prefixed according to their asset class:

| Asset Class | Prefix | Example | Format / Notes |
| :--- | :---: | :--- | :--- |
| **Scene** | `sc_` | `sc_tidewell_cove.unity` | All lowercase. |
| **Prefab** | `pf_` | `pf_wayfinder_player.prefab` | Instantiable actors/objects. |
| **Texture (Base Map)** | `tx_` | `tx_driftwood_plank_d.png` | Suffix indicates map type (see §3.2). |
| **Material** | `mat_` | `mat_driftwood_plank.mat` | Tied to URP Shaders. |
| **Audio (SFX)** | `sfx_` | `sfx_math_bridge_solve.wav` | Short uncompressed effects. |
| **Audio (Music)** | `mus_` | `mus_tidewell_theme_loop.ogg` | Compressed loop files. |
| **Audio (Voice)** | `vo_` | `vo_en_mara_intro_01.ogg` | Configured by locale (see §3.3). |
| **Shader Graph** | `sh_` | `sh_2d_storybook_glow.shadergraph` | Custom rendering graphs. |
| **ScriptableObject** | `so_` | `so_math_level_data_01.asset` | Data container assets. |
| **Animation Clip** | `anim_` | `anim_wayfinder_jog.anim` | Single motion clips. |
| **Animator Controller**| `ac_` | `ac_wayfinder_main.controller` | Character blend tree states. |
| **UI Sprite / Icon** | `ui_` | `ui_btn_interact_normal.png` | UI Atlas parts. |
| **Font Asset** | `font_` | `font_opendyslexic.asset` | Signed Distance Field (SDF) TextMeshPro fonts. |

### 3.2 Texture Map Suffixes
Textures must define their rendering function inside the URP pipeline via standard suffixes:
* **Diffuse / Albedo**: `_d` (e.g., `tx_barnacle_rock_d.png`)
* **Normal Map**: `_n` (e.g., `tx_barnacle_rock_n.png`)
* **Mask Map (Metallic/Gloss/AO)**: `_m` (e.g., `tx_barnacle_rock_m.png`)
* **Emission Map**: `_e` (e.g., `tx_lantern_glass_e.png`)

### 3.3 Voiceover (VO) Filename Structure
Voiceover files must encode their locale, character identity, and quest stage directly into the filename to automate runtime audio scheduling:
`vo_[locale]_[character]_[quest_id]_[index].ogg`
* *Example*: `vo_en_mara_washedshore_01.ogg` (English, Mara Tidekeeper, "The Washed Shore" quest, step 1 dialogue audio file).

---

## 4. Unity Hierarchy & Scene Naming Conventions

GameObjects in a Unity scene must follow a strict organization pattern to make hierarchy traversal and accessibility switch-scanning reliable.

### 4.1 Organizational Folders (Null Parent GameObjects)
Root-level objects in scenes that act as folders to group assets must use uppercase lettering enclosed in square brackets. They must be set to static, have zero active components (except transform), and have a reset transform (position 0,0,0, scale 1,1,1).

* `[SETUP]` - Dynamic managers, Main Camera, Event System, DI Container, Input Listener.
* `[LIGHTING]` - Directional lights, ambient lighting volumes, and skybox configs.
* `[WORLD_GEOMETRY]` - Static ground colliders, tilesets, background meshes.
* `[INTERACTABLES]` - Interactive doors, dialogue triggers, workbench nodes, NPCs.
* `[UI]` - Screen canvases, accessibility scan groups, overlay HUDs.

### 4.2 Dynamic GameObjects
Dynamic scene elements must use PascalCase with underscores to indicate state or type.
* **NPCs**: `NPC_Mara`, `NPC_OldFerro`, `NPC_Tock`
* **Player**: `Player_Wayfinder`
* **Trigger zones**: `Trigger_BiomeGate_Inkwood`
* **Interactive tools**: `Tool_Tideglass_Target`

---

## 5. Version Control & Virtual Folders

* **Branch Naming**:
  * `feature/issue-number-description` (e.g., `feature/qb-104-switch-scan`)
  * `bugfix/issue-number-description` (e.g., `bugfix/qb-212-webgl-alloc-crash`)
  * `release/version-number` (e.g., `release/v1.0.0`)
* **Addressables Group Naming**: Addressable asset groups must match their biome directory folder structure using forward slashes:
  * `biomes/tidewell_cove/assets`
  * `biomes/inkwood/assets`
  * `global/shared_audio`

---

## 6. Failure Modes & Edge Cases

### 1. WebGL Caching File Invalidation
* **Symptom**: Browser continues to load old visual textures or dialogue audio even after an update is deployed.
* **Cause**: Web servers caching filenames that do not change.
* **Prevention**: Addressable export settings must enable **Hash-Renaming** (`Bundle Naming: Filename Hash`). This appends a unique MD5 hash to the end of every compiled asset bundle file (e.g., `biomes_tidewell_cove_assets_8df9a82c.bundle`), forcing browsers to fetch the new version.

### 2. Case Mismatch Duplications
* **Symptom**: Local Mac/Windows development works, but WebGL build fails to find `tx_wood_plank_D.png`.
* **Cause**: Developer referenced the file with a capital `D`, which exists on case-insensitive Mac filesystems but fails on case-sensitive WebGL host servers.
* **Prevention**: Mandatory asset-checker pre-commit hook runs on the developer's computer.

---

## 7. Verification & Automated Enforcement

1. **Static Linting (C#)**: The `.editorconfig` file enforces C# symbol casing rules on build, causing compilation errors or warnings if naming standards are breached.
2. **Asset Naming Pipeline Checker**: A Python script (`tools/asset_checker.py`) runs in the Git CI/CD workflow (`lint_check.yml`) to scan all files inside `Assets/_Project/`. It rejects commits if:
   * Any file contains uppercase characters in its path (except inside the `Scripts/` or `Scenes/` folder).
   * Any file lacks an approved prefix from the asset prefix table (e.g., a file named `driftwood.png` instead of `tx_driftwood_d.png`).
3. **Hierarchy Validation Test**: A custom Editor Unit Test checks current scenes to confirm all root-level GameObjects match the bracketed uppercase format (e.g., `[SETUP]`) and that no functional scripts are attached directly to folders.
