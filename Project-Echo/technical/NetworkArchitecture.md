# Network Architecture

> **⚠️ Authority Note:** This is the **authoritative networking document for Project Echo**. The QuestBit workspace also contains `docs/architecture/21_networking_architecture.md`, which describes async REST/GraphQL networking for the separate QuestBit educational game project. **Project Echo uses real-time Photon Fusion 2** — that architecture doc does not apply here.

## Purpose

This document defines the multiplayer networking architecture for Project Echo. It specifies how the game handles state synchronization, replication, authority, and session recovery in Photon Fusion 2.

## Scope

This document covers:

- Session topology
- Authority and ownership rules
- Replication strategy
- Match state synchronization
- Failure handling and recovery

## Dependencies

- Photon Fusion 2 is the authoritative networking middleware.
- The game must support 2–4 players online.
- The architecture must support real-time interaction, voice communication, and state replication.

## Diagrams

### Network Topology

```mermaid
flowchart TD
    A[Host / Session Owner] --> B[Replicated Game State]
    B --> C[Client 1]
    B --> D[Client 2]
    B --> E[Client 3]
    B --> F[Client 4]
```

### Replication Flow

```mermaid
sequenceDiagram
    participant C as Client
    participant S as Server
    participant G as Game State
    C->>S: Input Event
    S->>G: Validate and Apply
    S->>C: State Update
```

## Examples

### Example 1: Interaction Replication

A player interacts with a relay panel. The request is validated by the authoritative state and then broadcast to all clients.

### Example 2: Creature State Sync

The host-controlled creature simulation updates the shared threat state and the clients receive the new state as replicated data.

## Edge Cases

- Network jitter causes delayed interaction confirmation.
- A player sends input twice because of packet loss.
- The host disconnects while a critical objective is resolving.
- A client reconnects during a creature escalation event.

## Design Decisions

### Decision 1: Use Server-Authoritative Resolution for Gameplay State

The server should own the match’s critical gameplay state, including objectives, hazards, and creature behavior. This preserves consistency and fairness.

### Decision 2: Use Local Prediction Only for Player Movement and Feedback

Local prediction is acceptable for movement and immediate feedback, but it should not be used to determine objective completion or creature state.

### Decision 3: Prioritize Deterministic State Transitions

The network architecture should avoid ambiguous or race-prone logic. If multiple players trigger the same state transition, the server should resolve the order deterministically.

## Future Improvements

- Add better session recovery and reconnect handling.
- Improve diagnostic tooling for network desync investigation.
- Add region-aware match selection where practical.

## Risks

- A poor authority model will create visible desync and unfair outcomes.
- Over-predicting state can create player confusion and debugging complexity.
- Network-edge cases can become major gameplay bugs if not planned for beforehand.

## Open Questions — Resolved

- **Should the host be the default authoritative runtime for the MVP or should a dedicated authoritative server be used when available?**
  - ✅ **Answer**: **Host-as-authority for MVP.** The host player runs the authoritative simulation. This reduces infrastructure cost and complexity during development. The architecture (per ADR-005) is designed so that migrating to a dedicated server later requires no gameplay code changes — only the transport layer changes.

- **How much input buffering is appropriate for the game's interaction model?**
  - ✅ **Answer**: **3-tick input buffer** (approximately 100ms at 30Hz tick rate). This is sufficient to absorb typical network jitter without introducing perceptible input delay. The game's interaction model (hold-to-interact, 0.5–3s actions) is tolerant of small latency variations since it prioritizes deliberate actions over twitch reflexes.

- **What reconnect tolerance should be supported before a session is considered unrecoverable?**
  - ✅ **Answer**: **30-second reconnect window.** If a player disconnects, their character enters a frozen/safe state visible to other players. If they reconnect within 30 seconds, they resume with full state. After 30 seconds, the session continues without them and their character is despawned. The match remains completable with fewer players (minimum 2).
