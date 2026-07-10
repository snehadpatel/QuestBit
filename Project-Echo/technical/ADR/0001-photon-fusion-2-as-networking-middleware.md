# ADR-0001: Use Photon Fusion 2 as the Multiplayer Networking Middleware

## Status

Accepted — 2026-07-09 (retroactively recorded; every existing document already assumed this decision without a written record, which is itself the exact failure mode this ADR folder exists to prevent per [technical/ADR/README.md](README.md)'s Edge Cases).

## Problem

Project Echo needs a real-time networking layer for 2–4 player co-op on Unity 6, supporting authoritative state (objectives, puzzles, creature, pressure), player movement, and voice-adjacent session coordination, built and shipped by a small indie team on a constrained budget and timeline.

## Decision

Use **Photon Fusion 2** as the sole multiplayer networking middleware.

## Rationale

- Native Unity 6 integration with first-party support, reducing integration risk for a small team.
- Built-in support for both host-authoritative and dedicated-server topologies under one API, which keeps the topology decision (ADR-0002) reversible later without a middleware rewrite if the game's needs change post-launch.
- Native host migration support (see ADR-0002), which would otherwise be substantial custom engineering.
- Commercial licensing with a free tier suitable for indie-scale concurrent session counts, avoiding upfront infrastructure spend during pre-production and prototyping.

## Alternatives Considered

- **Unity Netcode for GameObjects (NGO):** more manual work to get authoritative replication and host migration to the same maturity level; rejected for higher implementation effort on a constrained timeline.
- **Mirror:** mature and free, but lacks first-party managed relay/matchmaking infrastructure, which would shift that operational burden onto the team; rejected for the same reason dedicated servers are rejected in ADR-0002 — it optimizes for control the team doesn't need at the cost of complexity it can't afford.
- **Custom UDP/TCP layer:** rejected outright; building and hardening a custom transport is not a defensible use of a small team's time for a game whose differentiation is design, not networking technology.

## Consequences

- The team takes on Photon's per-CCU pricing at scale; acceptable, since Decision is explicitly not optimizing for scale (see ADR-0002).
- All subsequent networking, backend, and multiplayer documents assume Photon Fusion 2 APIs and terminology (Session, State Authority, Host Migration) rather than generic descriptions.

## Related Documents

- [technical/NetworkArchitecture.md](../NetworkArchitecture.md)
- [ADR-0002: Network Topology](0002-network-topology-host-mode.md)
- [docs/TDD.md](../../docs/TDD.md)
