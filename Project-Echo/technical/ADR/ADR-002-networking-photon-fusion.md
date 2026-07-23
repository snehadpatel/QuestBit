# ADR-002: Networking Middleware — Photon Fusion 2

- **Status**: Accepted
- **Date**: 2026-07-01
- **Decision Maker**: Principal Architect

## Context

Project Echo is a real-time asymmetric co-op horror game for 2–4 players. The networking layer must support:
- Authoritative gameplay state resolution (objectives, creature, hazards)
- Local prediction for player movement and immediate feedback
- Session management and reconnection handling
- Low-latency state replication suitable for horror pacing
- Integration with Unity 6

## Decision

**Use Photon Fusion 2** as the authoritative networking middleware.

## Alternatives Considered

| Option | Pros | Cons | Verdict |
|---|---|---|---|
| **Photon Fusion 2** | Server-authoritative model, Unity integration, mature SDK, tick-based simulation, built-in prediction/reconciliation | Vendor lock-in, per-CCU pricing, complexity for small teams | ✅ Selected |
| **Mirror (open source)** | Free, flexible, community-driven | No built-in prediction, manual authority model, less production-hardened | ❌ Rejected |
| **Netcode for GameObjects (Unity)** | First-party Unity support | Less mature than Photon, smaller community, fewer production references | ❌ Rejected |
| **Custom WebSocket layer** | Full control, low cost | Massive engineering burden, no prediction framework, high risk | ❌ Rejected |

## Consequences

- Server-authoritative resolution for all critical gameplay state (objectives, creature, hazards)
- Client prediction is limited to movement and local feedback only
- Session topology uses host-as-authority model for MVP
- Network replication follows Fusion 2's `[Networked]` property model
- Deterministic state transitions are enforced to avoid race conditions
- Future option to migrate to dedicated servers without rewriting gameplay logic

## Related Documents

- [NetworkArchitecture.md](../NetworkArchitecture.md)
- [Architecture.md](../Architecture.md)
