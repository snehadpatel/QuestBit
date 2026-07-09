# Milestones

## Purpose

This document converts the roadmap into concrete development checkpoints.

## Scope

Covers milestone criteria, delivery expectations, and success gates for each phase.

## Dependencies

- Each milestone depends on prior design and technical decisions.
- QA and production must review milestone readiness before the next phase begins.

## Diagrams

```mermaid
flowchart TD
    A[Prototype Complete] --> B[Vertical Slice Ready]
    B --> C[Playtest Pass]
    C --> D[Production Build]
    D --> E[Launch Candidates]
```

## Examples

- The prototype milestone requires a complete match flow from lobby to exit.
- The playtest milestone requires at least one successful session with a non-team member.

## Edge Cases

- A milestone may be blocked by an unimplemented backend dependency.
- A milestone can be considered complete only if it is testable end to end.

## Design Decisions

- Milestones are structured around completed player experience rather than raw task completion.
- Each milestone must produce something that can be playtested.

## Future Improvements

- Add milestone retrospectives into the workflow.
- Introduce more granular content delivery gates.

## Risks

- Milestones that are too broad create poor visibility.
- Team members may optimize for implementation speed instead of playtest quality.

## Open Questions

- What is the threshold for a milestone to be considered stable enough for external playtests?
- Should internal milestone reviews include a design and technical sign-off?
