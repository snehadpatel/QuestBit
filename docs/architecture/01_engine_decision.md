# Architectural Decision Record (ADR): Game Engine Selection

* **Status**: DECIDED
* **Date**: 2026-07-09
* **Chosen Option**: **Unity (Universal Render Pipeline - URP)**

---

## 1. Design Intent & Requirements Traceability

The selection of the game engine is the foundation for all technical architecture decisions. It must directly support the core requirements and constraints defined in the **QuestBit — Vision Document** and **Master Game Design Document (GDD)**:

* **Cross-Platform Reach (GDD §1.2)**: Must target Tablets (primary), Phones (secondary), Smart TVs/Consoles (living-room co-play), and WebGL (school/Chromebook builds).
* **Offline-First Playability (GDD §1.2 & Vision §5)**: The engine must support local storage, low-overhead local physics/math validation, and client-side gameplay simulation with zero network dependency.
* **Stylized 2.5D Visuals (Vision §6 & GDD §3.2)**: A warm, tactile, hand-crafted painterly aesthetic ("storybook illustration meets Pixar") blending 3D assets with 2D gameplay planes.
* **Accessibility Integration (Vision §8 & GDD Ch. 15)**: High-quality rendering of dyslexia-friendly fonts, screen-reader API bindings, custom switch-scan layouts, and low-stimulation rendering triggers.
* **Asset Bundling and Streaming (Vision §6 & GDD §16.5)**: Small initial download size (<500MB) with dynamic, on-demand streaming of biomes (e.g., Tidewell Cove, Inkwood) to support school Chromebook configurations.

---

## 2. Decision Criteria

Each candidate engine was evaluated against the following criteria, rated on a 1-5 scale:

1. **WebGL Export Maturity (Weight: 20%)**: Critical for Chromebooks in K-12 school districts. Must compile to WebAssembly and load fast with minimal runtime overhead.
2. **Mobile/Tablet Optimization (Weight: 15%)**: Efficient rendering, low battery drain, and thermal safety on lower-end devices.
3. **Accessibility API Access (Weight: 15%)**: Ease of hookup for OS screen readers, custom single-switch setups, and layout scaling.
4. **Asset Pipeline & Streaming (Weight: 15%)**: Native support for remote asset compilation, on-demand chunk loading (Addressables), and local caching.
5. **Localization Tooling (Weight: 10%)**: Out-of-the-box table matching, dynamic text replacement, and custom font mapping for right-to-left and non-latin character sets.
6. **Stylized 2.5D Capabilities (Weight: 10%)**: Native 2D physics, lights, and composite camera rigs combining 2D and 3D scenes.
7. **Ecosystem & Hiring Pool (Weight: 10%)**: Availability of developers experienced in mobile/tablet gameplay systems.
8. **Licensing & Cost (Weight: 5%)**: Total cost of ownership, royalty structure, and financial sustainability for an indie/B2B2C business model.

---

## 3. Options Evaluated

### Option A: Unity (Universal Render Pipeline - URP)
A mature, commercial cross-platform engine with extensive mobile and WebGL deployment history. Features a dedicated 2.5D rendering pathway within URP.

### Option B: Godot Engine (v4.x)
A free, open-source engine with native 2D and 3D engines, lightweight footprints, and an active community. Highly praised for simplicity but historically less optimized for WebGL.

### Option C: Unreal Engine (v5.x)
A powerhouse AAA engine designed for high-fidelity 3D. Features advanced rendering tools but carries significant binary size and runtime overhead.

---

## 4. Evaluation Matrix

| Criterion | Unity (URP) | Godot Engine | Unreal Engine |
| :--- | :---: | :---: | :---: |
| **1. WebGL Export Maturity (20%)** | **5 / 5** (Extensive production use) | **3 / 5** (Safari WebGL2/WebAssembly issues) | **1 / 5** (Deprecated HTML5 exporter) |
| **2. Mobile Optimization (15%)** | **5 / 5** (Industry standard for mobile) | **4 / 5** (Good, but lower driver compatibility) | **2 / 5** (Extremely heavy footprint) |
| **3. Accessibility API Access (15%)** | **4 / 5** (Robust UI Toolkit + plugins) | **5 / 5** (Built-in Control nodes screen-readers) | **3 / 5** (Complex, console-oriented APIs) |
| **4. Asset Pipeline & Streaming (15%)** | **5 / 5** (Addressables System is mature) | **3 / 5** (Manual export/pack scripting required) | **5 / 5** (Pak file chunking is solid) |
| **5. Localization Tooling (10%)** | **5 / 5** (Official Localization Package) | **3 / 5** (CSV based, requires custom wrappers) | **4 / 5** (Translation Dashboard) |
| **6. Stylized 2.5D rendering (10%)** | **5 / 5** (URP 2D Light/Renderer + 3D) | **5 / 5** (Excellent native 2D + 3D) | **3 / 5** (Overkill; difficult 2D workflow) |
| **7. Ecosystem & Hiring Pool (10%)** | **5 / 5** (Dominant mobile developer pool) | **3 / 5** (Growing, but limited professional pool) | **4 / 5** (Large, console/AAA-focused pool) |
| **8. Licensing & Cost (5%)** | **3 / 5** (Runtime Fee / Subscription costs) | **5 / 5** (FOSS - MIT License, $0 cost) | **3 / 5** (5% royalty above $1M USD) |
| **Weighted Total** | **4.70 / 5.00** | **3.80 / 5.00** | **2.60 / 5.00** |

---

## 5. Recommendation & Justification

**Unity (URP)** is the recommended game engine for QuestBit. 

### Core Justifications

1. **Critical WebGL Compliance**: Unreal Engine is completely disqualified due to its lack of HTML5/WebGL export support. Godot 4.x compiled to WebAssembly faces severe driver compatibility issues with iOS Safari due to its reliance on WebGL2 and SharedArrayBuffer threading patterns. Because QuestBit must run seamlessly on low-spec K-12 school Chromebooks (GDD §1.2), Unity’s WebGL export pipeline—which is highly optimized, single-threaded compatible, and works across older Chrome/Safari browsers—is a non-negotiable requirement.
2. **Asset Streaming via Addressables**: QuestBit must limit its initial build size to less than 500MB for classroom deployments while scaling biomes dynamically (GDD §16.5). Unity's **Addressable Asset System** provides local and remote asset group management, automated compression, and client-side caching out of the box, whereas Godot would require the development of a custom asset delivery and serialization framework.
3. **Optimized 2.5D Universal Render Pipeline**: Unity URP allows us to mix 3D character meshes (enabling fluid animation blend trees and skin variations, GDD Ch. 12) with a orthographic 2D physics and lighting system. We can set up custom Render Features to handle the "storybook glow" effects and screen-space overlays (Observe Loupe, GDD §2.2.3) efficiently.
4. **Localization and Accessibility Integration**: The Unity Localization package natively handles dynamic pluralization, gendered nouns, and dialect variations (Vision §8). Additionally, Unity's UI Toolkit facilitates the structural ordering of UI nodes to guarantee a clean **Switch-Scan accessibility loop** (GDD §15.3), with easy layout modifications for dyslexia-friendly fonts (such as OpenDyslexic).

---

## 6. Consequences & Systemic Mitigations

* **Licensing Costs**: Unity Pro licenses and potential runtime fees impact the operating budget.
  * *Mitigation*: Account for licensing fees in the studio startup capitalization document. Maintain project configuration to stay well within the Pro threshold until school district distributions justify Enterprise agreements.
* **Garbage Collection (GC) Spikes**: Unity’s C# Mono runtime can introduce periodic GC pauses, which violate our smooth performance guidelines on low-end tablets.
  * *Mitigation*: Establish strict coding standards (0 alloc in update loops, object pooling for particle effects, UI frames, and fraction planks) to minimize runtime heap allocation (detailed in `03_coding_standards.md` and `23_memory_budget.md`).
* **WebGL Build Sizes**: Unity WebGL builds can compile to large footprint files.
  * *Mitigation*: Strip unused engine modules via the Player Settings (e.g., disable physics 3D, built-in VR/XR, and multiplayer networking libraries). Set up the Asset Pipeline to compile all textures to ASTC on mobile and WebP/Crunch on WebGL.

---

## 7. Failure Modes & Risks

### WebGL Memory Allocations
* **Description**: WebGL builds run inside a browser sandbox with fixed memory limits. If the engine requests more memory than the browser allocates, the tab crashes with an Out-of-Memory (OOM) error.
* **Mitigation**: Limit the WebGL WebAssembly Heap Size to a fixed **256MB** in Unity build settings. Strictly manage asset lifetimes, unloading unneeded biomes from memory when returning to the Bramble hub.

### Core Engine Updates
* **Description**: Upgrading Unity versions mid-development can break rendering shaders, UI setups, and third-party packages.
* **Mitigation**: Lock the project to the latest **LTS (Long Term Support) version of Unity (specifically Unity 6 LTS)**. Upgrade only during major development cycles under QA supervision.

---

## 8. Verification & Testing

1. **Target Hardware Validation**: Compile a blank 2.5D scene with a basic character and URP 2D renderer. Profile CPU, memory, and draw calls on:
   * iPad 9th Generation (Standard Mobile baseline)
   * Lenovo 300e Chromebook (WebGL/School baseline)
2. **Build Size Verification**: Verify that the blank WebGL build template compiles to under **15MB** zipped (pre-asset streaming).
3. **Screen Reader Integration**: Validate that native screen-reading plugins can read UI text variables from the Unity engine.
