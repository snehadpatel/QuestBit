# Architecture Decision Records

## Purpose

This folder stores architecture decision records for significant technical choices in Project Echo. Each record captures the decision, reasoning, alternatives, and consequences.

## ADR Index

| ID | Title | Status | Date |
|---|---|---|---|
| [ADR-001](ADR-001-engine-selection.md) | Game Engine Selection → Unity 6 | Accepted | 2026-07-01 |
| [ADR-002](ADR-002-networking-photon-fusion.md) | Networking Middleware → Photon Fusion 2 | Accepted | 2026-07-01 |
| [ADR-003](ADR-003-persistence-playfab.md) | Account & Persistence → PlayFab | Accepted | 2026-07-01 |
| [ADR-004](ADR-004-voice-vivox.md) | Voice Communication → Vivox | Accepted | 2026-07-01 |
| [ADR-005](ADR-005-authority-model.md) | Authority Model → Server-Authoritative | Accepted | 2026-07-01 |

## ADR Template

When creating a new ADR, use this structure:

```markdown
# ADR-NNN: [Title]

- **Status**: Proposed | Accepted | Deprecated | Superseded
- **Date**: YYYY-MM-DD
- **Decision Maker**: [Role/Name]

## Context
[What is the problem or need?]

## Decision
[What was decided?]

## Alternatives Considered
[Table of options with pros, cons, and verdict]

## Consequences
[What are the implications of this decision?]

## Related Documents
[Links to relevant docs]
```

## Scope

This folder covers important technical choices that affect architecture, tooling, performance, networking, or long-term maintainability.

## Dependencies

- Each ADR should be linked to the relevant technical or gameplay document.
- ADRs should be written when the team makes a decision that could affect future technical choices or scope.

## Rules

- ADRs are **immutable once accepted**. If a decision changes, create a new ADR that supersedes the old one.
- Every ADR must have a clear status, date, and decision maker.
- Link ADRs to the technical and GDD docs they affect.
