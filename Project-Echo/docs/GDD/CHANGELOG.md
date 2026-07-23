# GDD Changelog

> **Last Updated:** 2026-07-15

This changelog tracks significant design decisions and modifications to the Game Design Document across all 31 chapters.

---

## Format

Each entry includes:
- **Date**: When the change was made
- **Chapter(s)**: Which GDD chapter(s) were affected
- **Change**: What was decided or modified
- **Rationale**: Why the change was made

---

## Changelog

### 2026-07-15

| Chapter(s) | Change | Rationale |
|---|---|---|
| **02 Core Gameplay** | Resolved: failure is recoverable, not a hard reset. Soft time pressure via creature escalation, no hard objective timers. | Aligns with Ch. 04 Decision 5 ("Failure States Must Be Recoverable") and preserves communication-first design. |
| **05 Communication System** | No changes — documented fallback communication as a minimum viable requirement. | Supports accessibility (Ch. 25) and voice quality degradation scenarios. |
| **10 Monster AI** | Confirmed creature states: Inactive → Probing → Tracking → Hunting → Retreating → Stalled. | Formalized the state machine referenced across multiple docs. |
| **25 Accessibility** | Created Accessibility Compliance Matrix (`docs/AccessibilityMatrix.md`) mapping requirements to systems. | Consolidates scattered accessibility requirements into one auditable document. |

### 2026-07-01 — Initial Documentation

| Chapter(s) | Change | Rationale |
|---|---|---|
| **All (00–30)** | Initial GDD creation. 31 chapters covering executive summary through appendix. | Establishing the full game design foundation before implementation begins. |
| **06 Asymmetric Reality** | Defined reality divergence model with per-player visibility/accessibility rules. | Core differentiating mechanic of the game. |
| **10 Monster AI** | Defined creature as "pressure engine" — non-combat, behavior-driven threat system. | Ensures gameplay emphasizes communication over combat reflexes. |
| **19 Multiplayer** | Defined 2–4 player online co-op with voice communication as primary interaction. | Core multiplayer design constraint. |

---

## How to Update This File

When making a design decision that changes gameplay intent, balance targets, or system behavior:

1. Add a new date section (if one doesn't exist for today)
2. List the affected chapter(s)
3. Describe what changed and why
4. If the change is significant, also create or update the relevant ADR in `technical/ADR/`
