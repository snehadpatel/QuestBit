# Architecture

## Purpose

This document defines the technical architecture for Project Echo. It establishes the runtime structure, major systems boundaries, and the implementation strategy needed to support asymmetric multiplayer horror gameplay in Unity 6.

## Scope

This document covers:

- Core runtime architecture
- System ownership and responsibilities
- Dependency boundaries between gameplay and infrastructure systems
- Extension strategy for future content

This document does not replace engine-specific implementation files.

## Dependencies

- Unity 6 is the engine runtime.
- C# is the implementation language.
- Photon Fusion 2 provides authoritative networking services, in Host Mode ([ADR-0001](ADR/0001-photon-fusion-2-as-networking-middleware.md), [ADR-0002](ADR/0002-network-topology-host-mode.md)) — `GameStateManager` below runs on whichever connected client is the elected Host, not on separate infrastructure.
- PlayFab provides account and persistence services.
- Vivox provides voice communication.

## Diagrams

### High-Level Runtime Architecture

```mermaid
flowchart TD
    A[Unity Scene Runtime] --> B[Game Bootstrap]
    B --> C[Gameplay Systems]
    C --> D[Network Layer]
    C --> E[Persistence Layer]
    C --> F[UI and Audio Layer]
    D --> G[Photon Fusion]
    E --> H[PlayFab]
    F --> I[Vivox]
```

### System Ownership Model

```mermaid
flowchart LR
    A[GameStateManager - runs on elected Host] --> B[ObjectiveSystem]
    A --> C[CreatureSystem]
    A --> D[PlayerSystem]
    A --> E[PuzzleSystem]
    A --> G[PressureSystem]
    A --> F[UISystem - local to each client]
    B --> G
    E --> G
    G --> C
```

`PressureSystem` (11 Stress System.md) was missing from this diagram in the prior version of this document — a gap, since it's the system Objective, Puzzle, and Creature all now route through as the single authoritative escalation value (see 11 Stress System.md §Consuming Systems). `UISystem` is marked local because, unlike the other four, it runs identically but independently on every client rendering the replicated state — it is never authoritative.

## Examples

### Example 1: Objective Execution

A relay puzzle resolves by sending an event to the ObjectiveManager. The manager updates the match state, triggers creature pressure, and notifies the UI.

### Example 2: Player Interaction

A player presses the interact key. The InputController sends a request to the InteractionService, which validates the action against the authoritative game state before applying it.

## Edge Cases

- The same object is used by multiple players in close succession.
- A client sends an interaction request while the server is still resolving a prior action.
- A network disconnect occurs during creature state transition.
- The host leaves after objective state has already changed — resolved by Fusion's native host migration with state hand-off; see technical/NetworkArchitecture.md §Host Migration.

## Design Decisions

### Decision 1: Use Authoritative Simulation for Critical Systems

The match state must be resolved by authoritative systems rather than purely local predictions. This protects fairness and prevents state divergence.

### Decision 2: Keep Gameplay Systems Modular

Systems such as objectives, puzzles, creature logic, player control, and UI should be loosely coupled. This reduces the risk of creating a monolithic runtime state machine.

### Decision 3: Keep the Runtime Layer Thin

The engine should not contain business logic that belongs in data-driven systems. The architecture should make it easy to author content through data assets and prefabs.

## Future Improvements

- Add a formal event bus for cross-system communication.
- Introduce more robust runtime profiling hooks.
- Expand the architecture to support additional facilities without rewriting core systems.

## Risks

- Over-coupling can create fragile systems and long debugging cycles.
- Too much authority on the client can create desync and cheating concerns.
- Content-driven architectures can become hard to maintain if the schema is not controlled.

## Open Questions

- Is a dedicated event bus necessary for the MVP or can a service locator pattern suffice?
- How much state should be replicated versus computed locally for performance reasons?
- Should the architecture favor a single monolithic GameManager or a distributed service model?
