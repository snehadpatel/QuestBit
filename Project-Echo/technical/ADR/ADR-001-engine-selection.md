# ADR-001: Game Engine Selection — Unity 6

- **Status**: Accepted
- **Date**: 2026-07-01
- **Decision Maker**: Principal Architect

## Context

Project Echo requires a game engine that supports:
- First-person 3D rendering for asymmetric co-op horror
- Real-time multiplayer networking (2–4 players)
- Cross-platform deployment (Steam PC as primary, potential console later)
- Voice chat integration
- Mature content and horror-appropriate rendering features
- C# as the primary development language

## Decision

**Use Unity 6** as the game engine and runtime.

## Alternatives Considered

| Engine | Pros | Cons | Verdict |
|---|---|---|---|
| **Unity 6** | Mature C# ecosystem, Photon Fusion 2 integration, wide platform support, strong asset pipeline, large hiring pool | Licensing cost, runtime fee concerns (since revised) | ✅ Selected |
| **Unreal Engine 5** | Superior rendering, native multiplayer, Nanite/Lumen | C++ complexity, team has stronger C# skills, heavier deployment size | ❌ Rejected |
| **Godot 4** | Open source, lightweight, GDScript simplicity | Immature 3D pipeline, limited multiplayer middleware, smaller hiring pool | ❌ Rejected |

## Consequences

- All technical architecture is designed around Unity 6 APIs, scene management, and asset pipeline
- Photon Fusion 2 is available as a first-class networking middleware
- PlayFab and Vivox integrate via existing Unity SDKs
- Team development uses C# exclusively
- CI/CD pipeline targets Unity Cloud Build or custom GitHub Actions with Unity build support

## Related Documents

- [Architecture.md](../Architecture.md)
- [NetworkArchitecture.md](../NetworkArchitecture.md)
