# Glossary

> **Last Updated:** 2026-07-15

This glossary defines project-specific terms used across the Project Echo documentation. If a term appears in a GDD chapter, technical doc, or ADR without obvious meaning, it should be defined here.

---

## Game Concepts

| Term | Definition | Primary Reference |
|---|---|---|
| **Asymmetric Reality** | The core mechanic where each player perceives the facility differently. Objects, hazards, and clues vary per player, forcing communication to reconstruct the truth. | GDD Ch. 06 |
| **Communication Loop** | The cycle of Observe → Express → Interpret → Decide → Act → Outcome that drives all cooperative gameplay. | GDD Ch. 05 |
| **Creature** | The game's pressure engine — a non-combat AI entity that escalates danger based on team behavior, noise, hesitation, and communication failures. Not a traditional enemy. | GDD Ch. 10 |
| **Facility** | A self-contained level/environment where a match takes place. Each facility has its own layout, hazards, objectives, and creature variant. | GDD Ch. 14 |
| **Match** | A single play session where 2–4 players enter a facility, complete objectives, and attempt to extract. A match has a defined start and end. | GDD Ch. 03 |
| **Reality Layer** | The per-player overlay that determines what that player sees, hears, and can interact with. Built on top of the shared facility structure. | GDD Ch. 06 |
| **Stress System** | A player-facing mechanic that tracks accumulated pressure from creature encounters, failed objectives, and communication breakdowns. Affects player capabilities and perception. | GDD Ch. 11 |
| **Vertical Slice** | The first complete, playable prototype — one facility, one creature, the full communication loop, and the asymmetric reality system. The minimum viable playtest target. | Roadmap, Milestones |

## Creature States

| Term | Definition | Primary Reference |
|---|---|---|
| **Inactive** | The creature has not yet been triggered. The facility is quiet. | GDD Ch. 10 |
| **Probing** | The creature is passively scanning the environment. Low threat. | GDD Ch. 10 |
| **Tracking** | The creature has detected team activity and is moving through the facility more aggressively. | GDD Ch. 10 |
| **Hunting** | Maximum threat. The creature is actively pursuing and creating consequences. | GDD Ch. 10 |
| **Retreating** | The creature pulls back after an encounter, resetting to a lower state. | GDD Ch. 10 |
| **Stalled** | The creature has lost tracking and is pausing before returning to Probing. | GDD Ch. 10 |

## Player States

| Term | Definition | Primary Reference |
|---|---|---|
| **Idle** | The player is stationary and not performing any action. | GDD Ch. 04 |
| **Moving** | The player is navigating the facility. | GDD Ch. 04 |
| **Interacting** | The player is performing a timed action on an object (0.5–3 seconds). | GDD Ch. 04 |
| **Stunned** | The player is temporarily incapacitated by a creature encounter or environmental hazard. Recoverable. | GDD Ch. 04 |
| **Disabled** | The player is fully incapacitated and cannot act until the team responds or the state times out. | GDD Ch. 04 |

## Communication States

| Term | Definition | Primary Reference |
|---|---|---|
| **Unshared** | Information observed by a player but not yet communicated to the team. | GDD Ch. 05 |
| **Shared** | Information that has been communicated verbally or via the fallback ping system. | GDD Ch. 05 |
| **Confirmed** | Information that multiple players have independently verified or agreed upon. | GDD Ch. 05 |

## Technical Terms

| Term | Definition | Primary Reference |
|---|---|---|
| **Authoritative State** | Gameplay state that is resolved by the server/host. Clients cannot modify it directly — they send requests that the server validates. | ADR-005, NetworkArchitecture.md |
| **Client Prediction** | Local simulation of movement and feedback before server confirmation. Used only for player movement, never for gameplay state. | ADR-005 |
| **Host-as-Authority** | The MVP networking model where one player's machine acts as both client and authoritative server. | ADR-002, ADR-005 |
| **Photon Fusion 2** | The real-time networking middleware used for multiplayer. Provides tick-based simulation, state replication, and prediction/reconciliation. | ADR-002 |
| **PlayFab** | Microsoft's backend-as-a-service used for account management, player data persistence, and authentication. | ADR-003 |
| **Vivox** | Real-time voice communication middleware. Powers in-game voice chat. | ADR-004 |
| **Threat Score** | An internal AI metric that accumulates based on player actions (noise, panic, delay, objective failure, environmental disturbance). Drives creature state transitions. | GDD Ch. 10 |
| **Event Bus** | A loosely-coupled messaging system that allows game systems to communicate without direct dependencies. Uses strongly-typed event payloads. | Architecture.md |
| **ParrelSync** | A Unity editor tool that enables running multiple editor instances for local multiplayer testing. | CONTRIBUTING.md |

## QuestBit-Specific Terms (Separate Project)

These terms appear in the QuestBit architecture docs (`docs/architecture/`) and do **not** apply to Project Echo:

| Term | Definition |
|---|---|
| **Snagglecrab** | A challenge creature in the QuestBit educational game that serves as an adaptive difficulty gate. |
| **Community Docks** | A QuestBit social feature for async co-op quest syncing. |
| **Mastery Engine** | QuestBit's adaptive learning system that adjusts quest parameters based on skill progress. |
| **Clue Journal** | A QuestBit feature that logs learning milestones and productive struggle time. |
| **Calm Mode** | A QuestBit accessibility feature that reduces visual/audio intensity (disables particles, limits screen shakes, applies low-pass audio filters). |
| **Ferro Knot** | A cosmetic charm item in QuestBit (`charm_ferro_knot`). |
