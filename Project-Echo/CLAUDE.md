# Project Echo Engineering and Design Notes

## Purpose

This file provides project-specific instructions for future contributors working on Project Echo documentation and implementation planning.

## Scope

Use this file to preserve decisions that affect design quality, architecture, and production scope.

## Dependencies

- The repository documentation structure must remain consistent.
- Any gameplay or technical change should be reflected in the relevant design document.
- New documents should follow the standard structure used across the repository.

## Core Directives

1. Treat communication as the primary gameplay pillar.
2. Avoid copying existing co-op horror mechanics without meaningful redesign.
3. Make every system testable and observable.
4. Prefer clarity over novelty when a design choice could create confusion.
5. Keep the MVP scope controlled and vertically sliceable.

## Documentation Rules

- Every markdown document must include Purpose, Scope, Dependencies, Diagrams, Examples, Edge Cases, Design Decisions, Future Improvements, Risks, and Open Questions.
- Use mermaid diagrams where appropriate.
- Include implementation notes and balancing notes whenever a system has gameplay consequences.
- Keep terminology consistent across all documents.

## Current Design Position

Project Echo is not an escape room, not a pure monster hunt, and not a simple puzzle game. It is a communication-driven co-op horror experience where fragmented reality creates dependency and pressure.
