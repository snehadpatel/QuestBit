# Roadmap

## Purpose

This document defines the development sequence for Project Echo from concept validation through live operations.

## Scope

Includes milestone planning, release sequencing, and the prioritization logic for major features.

## Dependencies

- The vision and high-concept documents must remain stable.
- The vertical slice must validate the communication-first gameplay loop.
- Production milestones must reflect the team’s staffing and tooling limits.

## Diagrams

```mermaid
gantt
    title Project Echo Roadmap
    dateFormat  YYYY-MM-DD
    section Foundation
    Vision and Scope :a1, 2026-07-01, 30d
    Architecture and Tools :a2, 2026-07-15, 30d
    section Vertical Slice
    Core Co-op Loop :b1, 2026-08-15, 45d
    One Facility Prototype :b2, 2026-09-01, 30d
    section Full Release
    Content Expansion :c1, 2026-10-15, 90d
    Live Ops Prep :c2, 2026-12-01, 45d
```

## Examples

- A prototype can validate the asymmetric information loop before full-level content is built.
- The roadmap should delay advanced creature systems until core communication survives playtests.

## Edge Cases

- Scope growth can cause the vertical slice to slip.
- Feature work may be blocked by backend service readiness.

## Design Decisions

- The roadmap prioritizes a proven communication loop before adding depth systems.
- The MVP is intentionally narrower than the long-term vision.

## Future Improvements

- Add post-launch content packs.
- Expand to additional facilities and creature variants.

## Risks

- Over-scoping can delay the first release.
- Feature creep can reduce polish.

## Open Questions — Resolved

- **Which milestone is the minimum viable public playtest?**
  - ✅ **Answer**: The **Vertical Slice** milestone (Core Co-op Loop + One Facility Prototype). The prototype must demonstrate a complete match flow — lobby → facility exploration → asymmetric communication → objective resolution → extraction — with at least one facility and one creature type.

- **What content is required for a convincing vertical slice?**
  - ✅ **Answer**: One fully playable facility, the asymmetric reality system (GDD Ch. 06), the communication loop (GDD Ch. 05), one creature type with escalation behavior (GDD Ch. 10), at least two puzzle types (GDD Ch. 07–08), and the core player systems (GDD Ch. 04). Monetization, cosmetics, and progression are not required for the vertical slice.

