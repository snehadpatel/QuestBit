# QuestBit Architectural Master Index

This document serves as the single source of truth for the QuestBit technical architecture documentation. It contains the Master Checklist for all 25 core systems. Each system is tracked against its specific dependencies, document location, and the project's strict **Definition of Done (DoD)**.

---

## 📋 Definition of Done (DoD) Criteria

Every system architecture document must meet the following criteria before it can be marked as complete:
1. **Design Intent**: Map directly to Vision Document or GDD requirements with specific chapter and section citations.
2. **Constraints**: State the platform, performance, memory, accessibility, and child-safety bounds for the system.
3. **Specification**: Provide concrete code interfaces, data schemas, folder layouts, and Mermaid diagrams (no generic prose-only descriptions).
4. **Concrete Metrics**: Provide explicit performance and/or memory budgets (no vague "should be performant" statements).
5. **Offline-First Integration**: State failure modes, edge cases, offline behavior, and sync patterns.
6. **Child Safety & Privacy**: Document compliance with data minimization, no third-party SDK leakage, local-first storage, and COPPA/GDPR-K standards.
7. **Accessibility (GDD Chapter 15)**: Integrate native support for accessibility features (e.g., switch-scan, text-to-speech) directly into the design.
8. **Testing Approach**: Outline the unit, integration, and manual testing strategy.
9. **Zero Placeholders**: Contain absolutely zero "TODO," "TBD," or placeholder items.

---

## 🚦 System Architecture Checklist

### Step 1: Engine Selection
- [x] **01. Game Engine Selection (ADR)** — status: completed
  - *File*: `docs/architecture/01_engine_decision.md`
  - *DoD Sub-Checklist*:
    - [x] ADR format with clear decision criteria (cross-platform, accessibility, performance, localization, team, licensing, 2.5D rendering)
    - [x] Compare at least 3 real candidates (e.g., Unity, Godot, Unreal/Cocos) in a comparison table
    - [x] Provide a clear recommendation and architectural commitment
    - [x] Zero placeholders or TBDs

### Step 2: System Architecture (Dependency-Ordered)

- [x] **02. Folder Structure** — status: completed
  - *File*: `docs/architecture/02_folder_structure.md`
  - *DoD Sub-Checklist*:
    - [x] Complete repository directory layout down to key subdirectories and system packages
    - [x] Align with selected engine structure (modules, assets, scripts, configs, tests)
    - [x] Map against GDD modularity targets and clean architecture boundaries
    - [x] Zero placeholders or TBDs

- [x] **03. Coding Standards** — status: completed
  - *File*: `docs/architecture/03_coding_standards.md`
  - *DoD Sub-Checklist*:
    - [x] Comprehensive language-specific style guide matching the selected game engine
    - [x] Guidelines for error handling, class design, memory allocation, and performance safety
    - [x] Static analysis, linting rules, and formatting standards (e.g., ClangFormat, editorconfig)
    - [x] Zero placeholders or TBDs

- [x] **04. Naming Conventions** — status: completed
  - *File*: `docs/architecture/04_naming_conventions.md`
  - *DoD Sub-Checklist*:
    - [x] Detailed naming rules for classes, methods, variables, file structures, and assets
    - [x] Prefix/suffix rules for scene objects, textures, audio assets, and UI components
    - [x] Rules for virtual folders, scene naming, and version tags
    - [x] Zero placeholders or TBDs

- [x] **05. Dependency Injection (DI)** — status: completed
  - *File*: `docs/architecture/05_dependency_injection.md`
  - *DoD Sub-Checklist*:
    - [x] Service locator or DI framework configuration schema and interface contracts
    - [x] Decoupling strategy for core engines, scene managers, dialogue, and UI
    - [x] Explicit performance overhead figures and initialization sequence diagram
    - [x] Zero placeholders or TBDs

- [x] **06. Event Bus** — status: completed
  - *File*: `docs/architecture/06_event_bus.md`
  - *DoD Sub-Checklist*:
    - [x] Interface definitions and implementation spec for loose coupling across gameplay systems
    - [x] Strongly-typed event payloads mapping to GDD Ch. 14 mechanics
    - [x] Event dispatch latency constraints and memory pooling/allocation specifications
    - [x] Zero placeholders or TBDs

- [x] **07. State Machine Framework** — status: completed
  - *File*: `docs/architecture/07_state_machine.md`
  - *DoD Sub-Checklist*:
    - [x] Hierarchy model (FSM/HFSM) interface contracts and transition schemas
    - [x] Game-level flow states (Boot, Hub, Biome, Minigame, Pause, Shutdown)
    - [x] State serialization and transition/loading boundary rules
    - [x] Zero placeholders or TBDs

- [x] **08. Input Manager** — status: completed
  - *File*: `docs/architecture/08_input_manager.md`
  - *DoD Sub-Checklist*:
    - [x] Input action mapping data schemas and control scheme configuration contracts
    - [x] Native switch-scan/single-switch navigation design per GDD Ch. 15
    - [x] Target latency metrics (<16.6ms scanning window) and physical coordinate mapping
    - [x] Zero placeholders or TBDs

- [x] **09. Scene Structure** — status: completed
  - *File*: `docs/architecture/09_scene_structure.md`
  - *DoD Sub-Checklist*:
    - [x] Scene loading and streaming system model (additive scenes, persistent root, biome streaming)
    - [x] Transition flows, async load budgets, and memory boundaries between biomes
    - [x] Visual transition sequence diagrams (loading screens, fade transitions)
    - [x] Zero placeholders or TBDs

- [x] **10. Asset Pipeline** — status: completed
  - *File*: `docs/architecture/10_asset_pipeline.md`
  - *DoD Sub-Checklist*:
    - [x] Import settings, automated processing, compression, and bundling rules
    - [x] Remote addressables / asset bundles strategy and offline caching model
    - [x] Storage constraints (e.g., total download size < 500MB, individual world bundle < 50MB)
    - [x] Zero placeholders or TBDs

- [x] **11. Rendering Pipeline** — status: completed
  - *File*: `docs/architecture/11_rendering_pipeline.md`
  - *DoD Sub-Checklist*:
    - [x] Render features, pass organization, custom shader pipelines (stylized 2.5D storybook look)
    - [x] Screen resolution targets, draw call budget (<120 batches on lower-end tablets), and fill-rate limits
    - [x] Calm mode visual reductions (disable particles, limit screen shakes) per GDD Ch. 15
    - [x] Zero placeholders or TBDs

- [x] **12. Animation Pipeline** — status: completed
  - *File*: `docs/architecture/12_animation_pipeline.md`
  - *DoD Sub-Checklist*:
    - [x] Skeleton rig specifications, blend tree structures, and procedural animations
    - [x] Animation loading and playback memory budgets (<30MB RAM overhead for rigs)
    - [x] Dynamic speed matching and visual emotional feedback mappings per GDD Ch. 12
    - [x] Zero placeholders or TBDs

- [x] **13. Audio System Architecture** — status: completed
  - *File*: `docs/architecture/13_audio_system.md`
  - *DoD Sub-Checklist*:
    - [x] Adaptive audio design, routing structure, mix busses, and voice allocation logic
    - [x] Calming sound filters, high-frequency reduction, and voiceover (VO) narration sync
    - [x] Memory limit for pre-loaded clips (<25MB) and offline audio decoding pipelines
    - [x] Zero placeholders or TBDs

- [x] **14. Localization System** — status: completed
  - *File*: `docs/architecture/14_localization_system.md`
  - *DoD Sub-Checklist*:
    - [x] Table layout, key formats, dynamic text-replacement tokens, and fallback strategies
    - [x] Dialect/regional adaptation pipeline (gendered nouns, numbers, decimals, cultural references)
    - [x] Dynamic font switching (standard vs. dyslexia-optimized) and layout reflow budgets
    - [x] Zero placeholders or TBDs

- [x] **15. Save System** — status: completed
  - *File*: `docs/architecture/15_save_system.md`
  - *DoD Sub-Checklist*:
    - [x] Save-game schema in JSON/Protocol Buffers, local-first encryption, and migration policy
    - [x] Conflict resolution rules for offline play and async cloud sync (no streak-shaming, no progress loss)
    - [x] Data minimization compliance (zero PII storage, local only by default, parental PIN gates)
    - [x] Zero placeholders or TBDs

- [x] **16. Data Pipeline (Analytics/Telemetry)** — status: completed
  - *File*: `docs/architecture/16_data_pipeline.md`
  - *DoD Sub-Checklist*:
    - [x] Analytics telemetry JSON schema, batching logic, local queue caching, and payload sizes
    - [x] Rigorous compliance rules: COPPA, GDPR-K, no third-party tracking SDKs, zero advertising ID leakage
    - [x] Educational analytics structure (learning milestones, Clue Journal logs, productive struggle time)
    - [x] Zero placeholders or TBDs

- [x] **17. Inventory System** — status: completed
  - *File*: `docs/architecture/17_inventory_system.md`
  - *DoD Sub-Checklist*:
    - [x] Class definitions, inventory database schema, and item definition asset patterns
    - [x] Item replication logic (local-first) and unlimited space for crafting materials
    - [x] UI layout mapping interface (e.g., drag-and-combine workbench integration per GDD Ch. 2.6)
    - [x] Zero placeholders or TBDs

- [x] **18. Dialogue System** — status: completed
  - *File*: `docs/architecture/18_dialogue_system.md`
  - *DoD Sub-Checklist*:
    - [x] Dialogue tree graph structure JSON schema and runtime node-traversal logic
    - [x] Audio sync hooks for voiced lines, text speed management, and dyslexia spacing overrides
    - [x] Integration with Event Bus to fire gameplay state changes based on narrative branches
    - [x] Zero placeholders or TBDs

- [x] **19. Quest System** — status: completed
  - *File*: `docs/architecture/19_quest_system.md`
  - *DoD Sub-Checklist*:
    - [x] Quest state machine model (NotStarted, Active, ConditionsMet, Completed) and database schema
    - [x] Mastery Engine integration: adaptive quest parameters and learning path evaluation
    - [x] Offline state persistence, async co-op Quest syncing (e.g., Community Docks), and reward payout
    - [x] Zero placeholders or TBDs

- [x] **20. AI System (Challenge Creatures)** — status: completed
  - *File*: `docs/architecture/20_ai_system.md`
  - *DoD Sub-Checklist*:
    - [x] Non-combat AI architecture (behavior trees / utility AI) and state contracts
    - [x] Cognitive difficulty limits, adaptive pacing (e.g., Snagglecrab helper gates), and tick-rate constraints
    - [x] Frame time budget (<1.5ms total CPU budget) and NPC pathfinding models
    - [x] Zero placeholders or TBDs

- [x] **21. Networking Architecture** — status: completed
  - *File*: `docs/architecture/21_networking_architecture.md`
  - *DoD Sub-Checklist*:
    - [x] Async-only REST/GraphQL API contracts, sync queue manager, and retry back-off schemas
    - [x] Strict child safety: no real-time player chat, no lobbies, async ghost visuals, and parental sync gates
    - [x] Performance target: zero blocking calls on main thread, local simulation matches server sync
    - [x] Zero placeholders or TBDs

- [x] **22. Performance Budget** — status: completed
  - *File*: `docs/architecture/22_performance_budget.md`
  - *DoD Sub-Checklist*:
    - [x] CPU/GPU/Load-time budgets across target tiers: Low-end Tablet, Mobile, Console, WebGL
    - [x] Maximum frame-time allocation (60 FPS on high, 30 FPS minimum on low-end tablets)
    - [x] Battery consumption bounds and thermal throttling mitigation guidelines
    - [x] Zero placeholders or TBDs

- [x] **23. Memory Budget** — status: completed
  - *File*: `docs/architecture/23_memory_budget.md`
  - *DoD Sub-Checklist*:
    - [x] Heap allocations, textures, audio, physics, and code footprint budgets
    - [x] Target bounds (e.g., total footprint <1.5GB RAM for low-end tablets, <512MB for WebGL Chromebooks)
    - [x] Memory leaks checks, garbage collection (GC) spike limits, and object pooling requirements
    - [x] Zero placeholders or TBDs

- [x] **24. Testing Strategy** — status: completed
  - *File*: `docs/architecture/24_testing_strategy.md`
  - *DoD Sub-Checklist*:
    - [x] Testing framework setup, mocking layers, automated UI testing, and regression pipelines
    - [x] High-frequency input simulation for switch-scan accessibility options
    - [x] Integration validation for offline-to-online state transitions
    - [x] Zero placeholders or TBDs

- [x] **25. CI/CD Pipeline** — status: completed
  - *File*: `docs/architecture/25_cicd_pipeline.md`
  - *DoD Sub-Checklist*:
    - [x] Automated build configurations, package compression, deployment pipelines, and compliance verification
    - [x] Automated asset validation (metadata checks, resolution scales, texture formats)
    - [x] Secure credential storage, child-safety scanners, and release build sign-off checklists
    - [x] Zero placeholders or TBDs

---

## 🪵 Changelog

* **2026-07-09**: Initial document creation. Setup checklist for systems 1-25. Status initialized to "not started".
