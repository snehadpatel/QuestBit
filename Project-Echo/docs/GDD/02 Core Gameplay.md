# Core Gameplay

## Purpose

This document defines the primary gameplay actions and interaction patterns that make Project Echo playable. It establishes the player-facing loop, the authority of actions, the role of communication, and the pacing expectations for every session.

## Scope

This document covers:

- Exploration and routefinding
- Observation and interpretation
- Interaction and objective completion
- Risk management and retreat
- Match pacing and feedback loops

This document does not define every room-specific puzzle or every monster behavior pattern.

## Dependencies

- The core gameplay loop depends on the asymmetric reality model in [docs/GDD/06 Asymmetric Reality.md](docs/GDD/06%20Asymmetric%20Reality.md).
- Core actions must integrate with the player system in [docs/GDD/04 Player Systems.md](docs/GDD/04%20Player%20Systems.md).
- Match pacing must align with the objective system in [docs/GDD/09 Objective System.md](docs/GDD/09%20Objective%20System.md).
- The creature pressure system in [docs/GDD/10 Monster AI.md](docs/GDD/10%20Monster%20AI.md) must react to core gameplay decisions.

## Player Fantasy

The player fantasy is not “fight the monster.” The fantasy is “interpret a fractured environment, coordinate under pressure, and decide whether to act, wait, or withdraw.”

The game should make players feel capable, uncertain, and responsible in equal measure. They should feel that their choices matter, but they should never feel punished for failing to immediately understand the environment.

## Core Loop

The minimum repeatable loop is:

1. Explore the environment.
2. Observe a clue, hazard, or objective state.
3. Share information with the team.
4. Decide whether to act, delay, or retreat.
5. Resolve the action.
6. Receive feedback and update the match state.

This loop should be visible in every session, whether the team is solving a puzzle, avoiding the creature, or preparing an escape.

## Core Actions

### 1. Observe

Players can inspect objects, environmental states, and ambiguous systems. Observation should be fast, readable, and meaningful. A successful observation should create either:

- New information
- New suspicion
- A new actionable path
- A new risk

### 2. Interact

Players can manipulate objects that are relevant to objectives, safety, or environmental control. Interaction should be simple enough to execute quickly, but meaningful enough to change the match state.

### 3. Communicate

Communication is a gameplay action, not a passive feature. Players should be able to convey observations, share interpretations, request help, and trigger coordinated decisions.

### 4. Reposition

Players must constantly choose where to stand, when to move, and whether to retreat. Movement should be reliable and easy to understand, especially when the creature or environment becomes dangerous.

### 5. Compromise

The game should allow players to make decisions under uncertainty. A team may choose to act early, hold position, or abandon a risky objective rather than push blindly.

## Gameplay Principles

### Principle 1: Clarity Over Complexity

The game should present a readable world. Even when the information is incomplete, the player should understand what is happening and what their options are.

### Principle 2: Consequence Over Randomness

Every core action should produce a clear outcome. If a player interacts with an object, the result should be understandable and visible.

### Principle 3: Shared Agency

No single player should be forced into a solo role. The game should support fluid cooperation where anyone can help in different moments.

### Principle 4: Pressure Without Frustration

The game should create urgency, but it should not turn every mistake into a dead-end. Recovery must remain possible.

### Principle 5: Information Has Weight

Knowledge should be a resource. The best teams are not those who simply react fast, but those who interpret accurately and communicate effectively.

## Core Interaction Rules

### Rule 1: Interaction Must Be Deterministic

If a player successfully uses an interactable, the game should resolve the outcome consistently for all clients.

### Rule 2: Interactions Must Have Readable Feedback

A successful action should produce an obvious signal. A failed action should also produce a clear reason.

### Rule 3: Actions Must Be Understandable in Context

The game should not require a player to know hidden systems to perform a basic interaction. The environment should make the action legible.

### Rule 4: Actions Should Create Trade-offs

Every action should affect safety, time, objective progression, or communication demand. The player should feel that action carries cost.

### Rule 5: Retreat Must Be a Valid Strategic Choice

Players should not be punished simply for deciding to withdraw, reassess, or regroup.

## Match Pacing

The match should feel like a controlled escalation rather than a constant state of panic.

### Early Match

The player learns the environment and the basic rules. Tension is low to medium. The team is still establishing trust and understanding.

### Mid Match

The team begins to understand the facility and the asymmetry. Pressure rises as puzzles, hazards, and creature escalation interact.

### Late Match

The team is under clear time and safety pressure. Decisions become more consequential, and successful communication matters more than speed alone.

## Failure and Recovery

Failure should rarely mean complete defeat. Instead, it should create one or more of the following:

- New danger
- Objective delay
- Lost time
- Information uncertainty
- Increased creature pressure

Recovery should be possible through teamwork, environmental adaptation, and clear communication.

## Example Scenarios

### Scenario 1: Safe Investigation

A player notices a panel in their reality that appears sealed. They report this to the team. Another player identifies a corresponding hazard in their own perspective. The team decides to investigate together, leading to a safe and successful interaction.

### Scenario 2: Risky Action

The team attempts to force a system before understanding it fully. The action fails, the environment reacts, and the creature begins to move more aggressively. The team must decide whether to recover the state, retreat, or try again with better information.

### Scenario 3: Communication-Driven Success

The team notices that a clue is only meaningful when two observations are combined. They share the information, coordinate their positions, and successfully complete a multi-step objective.

## Edge Cases

- A player attempts to use an object that is not currently valid in their reality.
- The team acts while the creature is nearby and creates an unintended escalation.
- A player begins an interaction but the match state changes before completion.
- Two players attempt conflicting actions on the same system.
- A player disconnects during a critical interaction and the system must degrade gracefully.

## Design Decisions

### Decision 1: The Core Loop Must Favor Interpretation Over Reflex

The game should encourage players to think and communicate, not simply react quickly. This is the central differentiator of the experience.

### Decision 2: The Player Should Always Understand the Consequence of an Action

Even when the system is complex, the immediate outcome of an action should be legible.

### Decision 3: The Game Should Reward Team Coordination, Not Solo Mastery

The best outcomes should come from shared understanding, not from one player being more skilled than the others.

### Decision 4: Failure Must Create Tension, Not Total Lockout

The game should preserve momentum even when the team makes a mistake.

## Balancing Notes

- Interaction timing should be short enough to maintain urgency, but long enough to allow teammates to react.
- The game should not rely on twitch precision for core progression.
- The creature and hazards should increase pressure when the team acts recklessly, but not punish every uncertain attempt.
- The session should feel like a sequence of meaningful decisions, not a series of repetitive tasks.

## Developer Notes

- Core actions should be implemented as explicit game actions with state validation and feedback.
- The interaction model should support both immediate actions and timed actions.
- Player-facing state should be represented in a unified interface so the UI and systems can respond consistently.
- The core loop should be easy to test through scripted scenarios and debug playback.

## Implementation Notes

- Represent actions with a shared command model that includes validation, execution, interruption, and feedback.
- Keep the action resolution path authoritative to prevent client mismatch.
- Tie core gameplay systems to event hooks so objectives, creature pressure, and UI can react to the same action stream.
- Support simple authored scenarios for playtesting the core loop without requiring full content.

## Future Improvements

- Expand the interaction vocabulary to include more varied environmental manipulation.
- Introduce dynamic environmental states that change how players solve core problems.
- Add more layered objective flows that require different styles of coordination.

## Risks

- If the core loop is too complex, players will feel overwhelmed rather than engaged.
- If the action feedback is weak, the game will feel unfair or opaque.
- If the game relies too heavily on combat or reflex, it will lose the intended identity.

## Open Questions

- How much of the core loop should be timed versus state-driven?
- Should players be allowed to fail an objective and recover without a hard reset?
- How much environmental complexity should be visible in the MVP versus reserved for later content?
