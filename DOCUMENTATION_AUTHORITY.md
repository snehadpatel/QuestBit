# Documentation Authority Guide

> **Last Updated:** 2026-07-15

This workspace contains documentation for **two separate projects** under the QuestBit organization. This guide establishes which documentation is authoritative for each project to prevent contradictions.

---

## Project Overview

| Project | Description | Target Audience | Tech Stack |
|---|---|---|---|
| **QuestBit** | Children's educational game | Ages 6+, parents, educators | Unity, offline-first, COPPA/GDPR-K compliant |
| **Project Echo** | Asymmetric co-op horror game | Mature players (Steam) | Unity 6, Photon Fusion 2, PlayFab, Vivox |

---

## QuestBit (Educational Game)

### Authoritative Documents

| Category | Authoritative Source | Location |
|---|---|---|
| **Vision** | QuestBit Vision Document | `Documents/QuestBit_Vision_document.md` |
| **Game Design** | QuestBit Master GDD | `Documents/QuestBit_Master_GDD.md` |
| **Architecture** (all 25 systems) | Architecture docs 01–25 | `docs/architecture/01_*.md` – `25_*.md` |
| **System Checklist & DoD** | Master Index | `docs/architecture/00_MASTER_INDEX.md` |
| **Cross-system Audit** | Consistency Audit | `docs/architecture/99_consistency_audit.md` |
| **Research** | Research notes | `Documents/Research.md` |

### Key Characteristics
- Async-only REST/GraphQL networking (no real-time multiplayer)
- COPPA/GDPR-K child safety compliance throughout
- Offline-first with local cloud sync
- Educational analytics with mastery tracking

---

## Project Echo (Co-op Horror Game)

### Authoritative Documents

| Category | Authoritative Source | Location |
|---|---|---|
| **Project Overview** | README | `Project-Echo/README.md` |
| **Game Design** (31 chapters) | GDD chapters 00–30 | `Project-Echo/docs/GDD/` |
| **Vision** | Vision document | `Project-Echo/docs/Vision.md` |
| **High Concept** | High concept pitch | `Project-Echo/docs/HighConcept.md` |
| **Technical Architecture** | Architecture doc | `Project-Echo/technical/Architecture.md` |
| **Networking** | Network Architecture | `Project-Echo/technical/NetworkArchitecture.md` |
| **Coding Standards** | Coding Standards | `Project-Echo/technical/CodingStandards.md` |
| **State Machines** | State Machines | `Project-Echo/technical/StateMachines.md` |
| **Folder Structure** | Folder Structure | `Project-Echo/technical/FolderStructure.md` |
| **Database** | Database design | `Project-Echo/technical/Database.md` |
| **ADRs** | Architecture Decision Records | `Project-Echo/technical/ADR/` |
| **Roadmap** | Development roadmap | `Project-Echo/docs/Roadmap.md` |
| **Milestones** | Milestone gates | `Project-Echo/docs/Milestones.md` |
| **Monetization** | Commercial strategy | `Project-Echo/docs/Monetization.md` |
| **Analytics** | Analytics approach | `Project-Echo/docs/Analytics.md` |
| **Testing** | QA strategy | `Project-Echo/docs/Testing.md` |
| **TDD** | Technical Design Document | `Project-Echo/docs/TDD.md` |

### Key Characteristics
- Real-time multiplayer via Photon Fusion 2 (2–4 players)
- Server-authoritative gameplay resolution
- Voice communication via Vivox
- Account/persistence via PlayFab
- Steam distribution target

---

## Overlap & Divergence Areas

The following topics exist in **both** doc trees. When editing, always check you're modifying the correct project's document:

| Topic | QuestBit Source | Project Echo Source | ⚠️ Note |
|---|---|---|---|
| Networking | `docs/architecture/21_networking_architecture.md` | `Project-Echo/technical/NetworkArchitecture.md` | **Different architectures** — async REST vs Photon real-time |
| Folder Structure | `docs/architecture/02_folder_structure.md` | `Project-Echo/technical/FolderStructure.md` | Different project structures |
| Coding Standards | `docs/architecture/03_coding_standards.md` | `Project-Echo/technical/CodingStandards.md` | May converge if both use Unity/C# |
| State Machines | `docs/architecture/07_state_machine.md` | `Project-Echo/technical/StateMachines.md` | Different game state models |
| Testing | `docs/architecture/24_testing_strategy.md` | `Project-Echo/docs/Testing.md` | QuestBit version is more detailed |

---

## Rules

1. **Always check which project you're editing** before modifying a doc.
2. **Never copy architecture decisions from one project to the other** without explicit review — they have fundamentally different requirements (child safety vs mature horror, async vs real-time, offline-first vs always-online).
3. **Cross-links between projects** should be explicit about which project the linked doc belongs to.
4. **New shared decisions** (e.g., a Unity version upgrade affecting both) should be documented in both trees with cross-references.
