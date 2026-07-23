# Contributing to Project Echo

> **Last Updated:** 2026-07-15

Welcome to Project Echo! This guide will get you set up and productive as quickly as possible.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Development Environment Setup](#development-environment-setup)
3. [Documentation Map](#documentation-map)
4. [Coding Standards Quick Reference](#coding-standards-quick-reference)
5. [Branch Strategy & PR Workflow](#branch-strategy--pr-workflow)
6. [Build & Run](#build--run)
7. [Testing](#testing)
8. [Key Contacts](#key-contacts)

---

## Project Overview

**Project Echo** is an asymmetric co-op horror game where 2–4 players explore an industrial facility. Each player perceives the environment differently through a layered reality system. Success depends on verbal communication, interpretation, and coordination under pressure.

**Core Tech Stack:**

| Component | Technology | Purpose |
|---|---|---|
| Engine | Unity 6 | Game runtime and editor |
| Language | C# | All game logic |
| Networking | Photon Fusion 2 | Real-time multiplayer (server-authoritative) |
| Voice | Vivox | In-game voice communication |
| Backend | PlayFab | Account management and persistence |
| Distribution | Steam | Primary platform (PC) |

> See [ADR Index](technical/ADR/README.md) for the full reasoning behind each technology choice.

---

## Development Environment Setup

### Prerequisites

| Tool | Version | Notes |
|---|---|---|
| Unity | 6.x (LTS) | Install via Unity Hub |
| IDE | Rider or Visual Studio 2022+ | JetBrains Rider recommended |
| Git | 2.40+ | With Git LFS enabled |
| Photon Fusion 2 SDK | Latest | Import via Unity Package Manager or `.unitypackage` |
| PlayFab SDK | Latest | Import via Unity Package Manager |
| Vivox SDK | Unity integration | Import via Unity Package Manager |

### Setup Steps

1. **Clone the repository**
   ```bash
   git clone <repo-url>
   cd Project-Echo
   ```

2. **Install Git LFS** (if not already)
   ```bash
   git lfs install
   git lfs pull
   ```

3. **Open in Unity Hub**
   - Add the project folder
   - Unity Hub will prompt to install the correct editor version if needed

4. **Configure credentials**
   - Copy `.env.template` to `.env` (if applicable)
   - Add your Photon App ID (Settings → Photon → App Settings)
   - Add your PlayFab Title ID (Settings → PlayFab)
   - Add Vivox credentials (Settings → Vivox)
   - **Never commit credentials to the repository**

5. **Verify the build**
   - Open the Bootstrap scene
   - Press Play in the editor
   - Confirm no console errors

---

## Documentation Map

### Start Here (Read Order)

| Order | Document | What You'll Learn |
|---|---|---|
| 1 | [This guide](CONTRIBUTING.md) | How to get set up and contribute |
| 2 | [Vision](docs/Vision.md) | What the game is about and why |
| 3 | [High Concept](docs/HighConcept.md) | Elevator pitch and core fantasy |
| 4 | [GDD 02 Core Gameplay](docs/GDD/02%20Core%20Gameplay.md) | The core gameplay loop |
| 5 | [GDD 06 Asymmetric Reality](docs/GDD/06%20Asymmetric%20Reality.md) | The game's signature mechanic |
| 6 | [Architecture](technical/Architecture.md) | How the runtime is structured |
| 7 | [Network Architecture](technical/NetworkArchitecture.md) | How multiplayer works |
| 8 | [Coding Standards](technical/CodingStandards.md) | How to write code for this project |

### Full Documentation Index

| Category | Location |
|---|---|
| Game Design (31 chapters) | `docs/GDD/` |
| Technical Architecture | `technical/` |
| Architecture Decisions | `technical/ADR/` |
| Roadmap & Planning | `docs/Roadmap.md`, `docs/Milestones.md` |
| Monetization | `docs/Monetization.md` |
| QA & Testing | `docs/Testing.md` |
| Glossary | `docs/Glossary.md` |

### QuestBit vs Project Echo

This workspace also contains documentation for **QuestBit** (a separate children's educational game). See [DOCUMENTATION_AUTHORITY.md](../DOCUMENTATION_AUTHORITY.md) at the workspace root for which docs apply to which project.

---

## Coding Standards Quick Reference

Detailed standards are in [CodingStandards.md](technical/CodingStandards.md). Key rules:

- **Naming**: PascalCase for public members, `_camelCase` for private fields
- **Architecture**: Systems are loosely coupled via interfaces and events
- **Authority**: Never resolve gameplay state on the client — send requests to the server
- **Comments**: Document *why*, not *what* — code should be self-explanatory
- **No magic numbers**: Use constants or `ScriptableObject` configuration
- **Null safety**: Never pass `null` where an empty collection or default value works

---

## Branch Strategy & PR Workflow

### Branches

| Branch | Purpose | Merge Target |
|---|---|---|
| `main` | Stable, releasable code | — |
| `develop` | Integration branch for current sprint | `main` |
| `feature/<name>` | New features | `develop` |
| `bugfix/<name>` | Bug fixes | `develop` |
| `hotfix/<name>` | Critical production fixes | `main` + `develop` |

### PR Rules

1. **One feature per PR** — keep changes focused
2. **All PRs require at least one review** before merge
3. **PR description must include**: what changed, why, and how to test it
4. **All tests must pass** before merge
5. **No direct commits to `main` or `develop`**

---

## Build & Run

### Editor Play

1. Open the **Bootstrap** scene
2. Press Play
3. For multiplayer testing, use ParrelSync or build a standalone client

### Standalone Build

```bash
# Build from Unity CLI (if configured)
Unity -batchmode -projectPath . -buildTarget StandaloneWindows64 -executeMethod BuildScript.Build
```

### Multiplayer Testing

- Use **ParrelSync** for local multi-instance testing during development
- Use **Photon Dashboard** to monitor active sessions and player counts
- Minimum viable playtest requires 2 connected clients

---

## Testing

See [Testing.md](docs/Testing.md) for the full QA strategy.

### Running Tests

```
# Open Test Runner in Unity
Window → General → Test Runner
# Run Edit Mode and Play Mode tests
```

### What to Test Before Submitting a PR

- [ ] No new console errors or warnings
- [ ] Existing tests still pass
- [ ] New gameplay code has at least basic unit tests
- [ ] Multiplayer changes tested with 2+ clients
- [ ] Edge cases documented in the PR description

---

## Key Contacts

| Role | Responsibility |
|---|---|
| Principal Architect | Architecture decisions, ADRs, technical direction |
| Lead Designer | GDD ownership, gameplay decisions |
| Producer | Milestones, roadmap, sprint planning |

---

*If something in this guide is unclear or outdated, update it and submit a PR. The best onboarding doc is one that's maintained by the people who use it.*
