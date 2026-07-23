# Testing Strategy

## Purpose

This document defines the quality assurance approach for Project Echo across design, technical stability, and multiplayer reliability.

## Scope

Covers functional testing, playtest planning, regression testing, and multiplayer validation.

## Dependencies

- The game must support repeatable local and online playtests.
- The technical stack must expose logs and telemetry to support bug investigation.

## Diagrams

```mermaid
flowchart LR
    A[Design Test Cases] --> B[Build Verification]
    B --> C[Playtest Feedback]
    C --> D[Issue Triage]
    D --> E[Fix and Re-test]
```

## Examples

- A bug in objective state replication should be reproduced by a simple scripted scenario.
- A communication issue should be tested with both voice chat and fallback text input.

## Edge Cases

- Network packet loss during interaction resolution.
- Objective state mismatch between host and client.
- Audio synchronization problems during creature events.

## Design Decisions

- QA should test not only whether the system works, but whether the player experience remains readable and fair.
- Multiplayer testing is mandatory because many failures are only visible under real network conditions.

## Future Improvements

- Add automated integration tests for core objective flows.
- Expand telemetry-driven balancing and regression analysis.

## Risks

- Poor reproducibility can slow bug fixing.
- Design issues can be mistaken for technical bugs without structured playtesting.

## Open Questions — Resolved

- **What test matrix is required for the first public playtest?**
  - ✅ **Answer**: The minimum test matrix for the first public playtest must cover: (1) 2-player and 4-player sessions, (2) host migration or host-disconnect recovery, (3) a complete match flow end-to-end, (4) voice communication quality under normal and degraded network conditions, (5) at least one full objective chain with the asymmetric reality system active, and (6) creature escalation through all states (Probing → Tracking → Hunting).

- **Which issues should be treated as release blockers versus post-launch fixes?**
  - ✅ **Answer**: **Release blockers** are: crashes, data loss, network desync that breaks match state, voice chat failure with no fallback, and any bug that makes the match uncompletable. **Post-launch fixes** are: visual glitches that don't affect gameplay, minor audio sync issues, balance concerns, cosmetic rendering errors, and edge-case creature pathfinding anomalies.

