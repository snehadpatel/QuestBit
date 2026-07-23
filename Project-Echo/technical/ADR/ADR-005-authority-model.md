# ADR-005: Authority Model — Server-Authoritative Gameplay

- **Status**: Accepted
- **Date**: 2026-07-01
- **Decision Maker**: Principal Architect

## Context

Project Echo is a multiplayer game where gameplay fairness depends on consistent state across all clients. The team must decide how much authority the host/server has versus how much clients can predict or resolve locally.

Key concerns:
- Objective completion must be consistent across all clients
- Creature behavior must be deterministic and fair
- Player interactions with shared objects must resolve without race conditions
- Local prediction is desirable for movement responsiveness

## Decision

**Use a server-authoritative model for all critical gameplay state.** Local prediction is permitted only for player movement and immediate visual/audio feedback.

### Authority Boundaries

| System | Authority | Prediction Allowed |
|---|---|---|
| **Objectives** | Server-authoritative | ❌ No |
| **Creature AI & State** | Server-authoritative | ❌ No |
| **Puzzle Resolution** | Server-authoritative | ❌ No |
| **Hazard Activation** | Server-authoritative | ❌ No |
| **Player Interactions** | Server-validated | ❌ No (request → validate → apply) |
| **Player Movement** | Client-predicted, server-reconciled | ✅ Yes |
| **Local Audio/VFX Feedback** | Client-local | ✅ Yes |

## Alternatives Considered

| Option | Pros | Cons | Verdict |
|---|---|---|---|
| **Server-authoritative (selected)** | Consistent state, fair outcomes, no desync on critical paths | Higher latency for interactions, host advantage | ✅ Selected |
| **Full client prediction with rollback** | Lowest perceived latency | Extremely complex for puzzle/objective state, visual rollback artifacts | ❌ Rejected |
| **Peer-to-peer consensus** | No dedicated authority | Race conditions, no single source of truth, cheating risk | ❌ Rejected |

## Consequences

- All gameplay-critical state transitions go through the authoritative host
- Interaction requests follow a request → validate → apply → broadcast pattern
- Deterministic state transitions prevent ambiguity when multiple players act simultaneously
- Host advantage is minimal because the game prioritizes interpretation over twitch reflexes
- MVP uses host-as-server; dedicated server migration is possible without architecture change

## Related Documents

- [NetworkArchitecture.md](../NetworkArchitecture.md)
- [Architecture.md](../Architecture.md)
- [GDD 04 Player Systems](../../docs/GDD/04%20Player%20Systems.md)
