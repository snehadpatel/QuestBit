# ADR-003: Account & Persistence — PlayFab

- **Status**: Accepted
- **Date**: 2026-07-01
- **Decision Maker**: Principal Architect

## Context

Project Echo requires a backend service for:
- Player account management and authentication
- Persistent data storage (progression, cosmetics, statistics)
- Leaderboard and match history support (potential future use)
- Scalable infrastructure without custom server management

## Decision

**Use PlayFab** for account management and persistence services.

## Alternatives Considered

| Option | Pros | Cons | Verdict |
|---|---|---|---|
| **PlayFab** | Mature Unity SDK, authentication, player data, economy, scalable BaaS, free tier for development | Microsoft vendor lock-in, data residency considerations, per-API-call pricing at scale | ✅ Selected |
| **Firebase** | Strong auth, real-time database, Google ecosystem | Less game-specific, Firestore pricing model, no built-in game economy | ❌ Rejected |
| **Custom Backend (Node.js/Go)** | Full control, no vendor lock-in | High engineering cost, ops burden, distracts from game development | ❌ Rejected |
| **Nakama (open source)** | Self-hosted, game-focused, leaderboards | Ops complexity, smaller ecosystem, less Unity-specific documentation | ❌ Rejected |

## Consequences

- Player accounts use PlayFab authentication (Steam identity for PC release)
- Player progression and save data stored in PlayFab Player Data
- Economy system (if needed) uses PlayFab Economy v2
- All PlayFab calls are async and non-blocking on the game thread
- Data migration path exists via PlayFab export APIs if vendor change is needed

## Related Documents

- [Architecture.md](../Architecture.md)
