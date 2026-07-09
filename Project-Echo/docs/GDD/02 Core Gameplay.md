# Core Gameplay

## Purpose

This document defines the core player actions and interactions that form the foundation of Project Echo.

## Scope

Covers exploration, interpretation, objective completion, and escape preparation.

## Dependencies

- The game requires a stable communication channel between players.
- The environment must expose enough information for players to interpret and cross-reference.

## Diagrams

```mermaid
flowchart LR
    A[Explore] --> B[Observe]
    B --> C[Communicate]
    C --> D[Act]
    D --> E[Advance or Fail]
```

## Examples

- One player hears a mechanical clicking sound and another sees the corresponding hazard.
- The team must decide whether to proceed, disable the hazard, or retreat.

## Edge Cases

- A player may attempt to interact with an object that only appears in another reality.
- A hazard may become active before the full team is ready.

## Design Decisions

- Gameplay should prioritize interpretation and coordination over reflex-based action.
- Interactions should always have an understandable consequence.

## Future Improvements

- Add more sophisticated environmental interactions.
- Introduce more layered objectives across facilities.

## Risks

- Too many interactive systems may reduce clarity.
- A weak feedback loop may make the player feel powerless.

## Open Questions

- How much of the core loop should be time-limited?
- Should players be allowed to fail objectives and recover, or is failure a hard reset?
