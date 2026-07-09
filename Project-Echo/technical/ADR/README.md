# Architecture Decision Records

## Purpose

This folder stores architecture decision records for significant technical choices in Project Echo. Each record captures the decision, reasoning, alternatives, and consequences.

## Scope

This folder covers important technical choices that affect architecture, tooling, performance, networking, or long-term maintainability.

## Dependencies

- Each ADR should be linked to the relevant technical or gameplay document.
- ADRs should be written when the team makes a decision that could affect future technical choices or scope.

## Examples

### Example 1: Networking Architecture

A decision to use Photon Fusion 2 for authoritative multiplayer should be recorded with justification, alternatives considered, and consequences.

### Example 2: State Model Approach

A decision to use explicit state machines rather than ad hoc flags should be captured in an ADR.

## Edge Cases

- A decision is made informally and later becomes hard to trace.
- A temporary implementation choice becomes a long-term architecture assumption.
- Two teams interpret the same decision differently because there is no written record.

## Design Decisions

- ADRs should be concise but specific.
- Each record should state the problem, decision, rationale, and consequences.
- The team should update ADRs if a decision changes later.
- Format: numbered files (`NNNN-short-title.md`), each with Status, Problem, Decision, Rationale, Alternatives Considered, Consequences, and Related Documents — this resolves the "what format will the team use" open question below by example rather than by further discussion.

## Index

| ADR | Decision |
|---|---|
| [0001](0001-photon-fusion-2-as-networking-middleware.md) | Use Photon Fusion 2 as the multiplayer networking middleware |
| [0002](0002-network-topology-host-mode.md) | Network topology: Host Mode (client-hosted, with migration), not Shared Mode or dedicated servers |

## Future Improvements

- Add ADR templates and a review process.
- Include links from technical docs to the relevant ADRs.
- Maintain a living index of decisions for onboarding.

## Risks

- Missing ADRs create invisible technical debt.
- Decisions made without context become hard to revisit.
- ADRs that are too verbose become less useful than they should be.

## Open Questions

- Which early technical decisions should be documented first? Networking middleware and topology are now recorded (0001, 0002); the next candidates are the state-machine framework approach (technical/StateMachines.md's own open question) and the save-schema versioning approach (22 Save System.md).
- How often should ADRs be reviewed as the project evolves? Owner: Technical Direction, revisit at each milestone gate in Milestones.md.
