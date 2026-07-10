# ADR-0002: Network Topology — Photon Fusion 2 Host Mode (Client-Hosted, with Host Migration)

## Status

Accepted — 2026-07-09. Resolves a direct contradiction identified in the production audit: [docs/GDD/19 Multiplayer.md](../../docs/GDD/19%20Multiplayer.md) previously assumed peer-hosted authority with migration, while [docs/GDD/21 Backend.md](../../docs/GDD/21%20Backend.md) implied a matchmaker-assigned dedicated server. This ADR fixes the topology for the whole repository; all six affected documents are updated to reference it (see §Related Documents).

## Problem

Photon Fusion 2 supports three topology modes: **Shared Mode** (no single authoritative peer; state authority is distributed per-object), **Host Mode** (one connected client is elected the authoritative simulation owner, with native migration to another client if it leaves), and **Server Mode** (a headless dedicated server process, run and hosted independently of any player, owns the simulation). The repository had never chosen one, and two documents silently assumed different ones.

## Explicit Optimization Target

Per direct product guidance: **do not optimize for scalability. Optimize for shipping a successful Steam indie game.** The decision below is evaluated against implementation complexity and cost for a 2–4 player co-op title built by a small, likely unfunded-at-scale team (see the production audit's finding that no team size or budget exists anywhere in the repository) — not against how well it would hypothetically support hundreds of concurrent sessions or public ranked matchmaking, neither of which the game's design (private 2–4 player co-op) requires.

## Decision

Use **Fusion Host Mode**: one connected player's client is elected Session Authority at match start and runs the authoritative simulation (objectives, puzzles, creature, the Pressure System defined in [11 Stress System.md](../../docs/GDD/11%20Stress%20System.md)); all other clients replicate from it. If the host disconnects, Fusion's native host migration reassigns authority to another connected client without ending the session.

## Rationale

| Criterion | Shared Mode | **Host Mode (chosen)** | Server Mode (dedicated) |
|---|---|---|---|
| Infrastructure to build/run | None beyond Photon's relay | None beyond Photon's relay | Headless server build, cloud hosting, deployment/orchestration, regional scaling |
| Ongoing hosting cost | None | None | Per-session or per-CCU cloud compute cost, indefinitely |
| Single source of truth for Global Pressure / puzzle / objective authority | **No** — authority is per-object and distributable; would require building a custom authority-election layer on top to get one, which is more work than Host Mode provides natively | **Yes**, natively — exactly what 07 Puzzle Framework, 09 Objective System, and 11 Stress System.md already assume ("the session authority") | Yes, natively |
| Matches content already written across the repo | No — would require rewriting the authority assumptions in three already-completed documents | **Yes** — Multiplayer.md's existing host-migration example already describes this model | No — contradicts Multiplayer.md as written |
| DevOps/operational burden on a small team | Low | **Low** | High — this is the "optimize for scalability" answer the product guidance explicitly rejected |
| Implementation complexity | Low, but hidden complexity reappears solving the authority problem above | **Lowest complexity that still satisfies the single-authority requirement** | Low complexity per-session, high complexity in aggregate (deployment pipeline, server fleet management) |

Host Mode is the smallest option that satisfies a hard requirement already committed to elsewhere in the repository — a single authoritative computation of Global Pressure and match state (Decision 1a, 11 Stress System.md) — without adding server infrastructure a small team would need to build, pay for, and operate. Shared Mode looks cheaper at a glance but would force the team to hand-build the exact authority guarantee Host Mode provides for free. Server Mode provides nothing this game needs that Host Mode doesn't, at meaningfully higher cost and complexity.

## Alternatives Considered

- **Shared Mode:** rejected. No single node to own Pressure/Objective/Puzzle authority without custom engineering; also weaker cheat resistance for the Asymmetric Reality hidden-information mechanic, since any peer with object authority could in principle read or alter data meant to be hidden from them.
- **Server Mode (dedicated servers):** rejected for MVP. Solves a scaling problem this game does not have (2–4 players, private sessions, not public matchmaking at volume) at the cost of ongoing hosting spend and DevOps work the production audit already flagged as unbudgeted. Not ruled out permanently — see §Future Improvements.

## Consequences

- **Accepted risk, stated explicitly:** the host player has full simulation authority and could theoretically manipulate match state or read hidden information intended for other players. This is accepted for MVP because Project Echo ships as private, invite-based 2–4 player sessions among people who chose to play together, not public ranked matchmaking — the same threat model already implicit in Multiplayer.md's design. This is a downgrade of the production audit's "no anti-cheat" finding from a blocking risk to an accepted, documented one; it should be revisited if a public-matchmaking or competitive mode is ever added post-launch.
- **Host migration is now a hard requirement, not an optional feature**, because 11 Stress System.md's §Ownership section already requires that a migrating authority seed itself from the last acknowledged `PressureSnapshot` rather than resetting state. Fusion's native host migration satisfies this as long as the incoming host receives the outgoing host's last-known match state before taking over, which must be an explicit implementation requirement (see updated NetworkArchitecture.md).
- Backend.md's "Game Server / Session" node is renamed and re-scoped: PlayFab is used for account identity, matchmaking-ticket/lobby coordination, and persistence — never for running the game simulation itself. See the updated Backend Service Flow diagram in that document.
- No dedicated server budget line item is required for MVP or launch.

## Future Improvements

- If the game later adds public matchmaking at meaningful scale, or if host-authority cheating becomes a measured problem in practice, revisit this ADR and evaluate Server Mode against actual usage data rather than speculatively building for it now.

## Related Documents

- [ADR-0001: Photon Fusion 2 as Middleware](0001-photon-fusion-2-as-networking-middleware.md)
- [technical/NetworkArchitecture.md](../NetworkArchitecture.md)
- [docs/TDD.md](../../docs/TDD.md)
- [docs/GDD/19 Multiplayer.md](../../docs/GDD/19%20Multiplayer.md)
- [docs/GDD/21 Backend.md](../../docs/GDD/21%20Backend.md)
- [docs/GDD/20 Steam Integration.md](../../docs/GDD/20%20Steam%20Integration.md)
- [docs/GDD/11 Stress System.md](../../docs/GDD/11%20Stress%20System.md) — §Data Model, Ownership, and Replication assumes this ADR
