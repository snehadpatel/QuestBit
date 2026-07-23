# Third-Party Dependency Register

> **Last Updated:** 2026-07-15

This document tracks all third-party dependencies used in Project Echo, including versions, license types, cost implications, and fallback strategies.

---

## Core Dependencies

| Dependency | Version | License | Cost Model | Fallback | ADR |
|---|---|---|---|---|---|
| **Unity 6** | 6.x LTS | Unity EULA (per-seat + runtime fee) | Per-seat subscription + revenue-based runtime fee | Godot 4 (major rewrite) | [ADR-001](ADR/ADR-001-engine-selection.md) |
| **Photon Fusion 2** | Latest | Photon License (commercial) | Free tier (20 CCU), paid tiers per CCU | Mirror (open source, significant rework) | [ADR-002](ADR/ADR-002-networking-photon-fusion.md) |
| **PlayFab** | Latest | Microsoft ToS | Free tier (100K users/month), pay-per-API-call beyond | Firebase or custom backend | [ADR-003](ADR/ADR-003-persistence-playfab.md) |
| **Vivox** | Unity SDK | Unity/Vivox ToS | Free tier (5K peak users), paid beyond | Photon Voice 2 or Steam Voice | [ADR-004](ADR/ADR-004-voice-vivox.md) |

## SDK & Plugin Dependencies

| Dependency | Version | License | Purpose | Notes |
|---|---|---|---|---|
| **Steamworks.NET** | Latest | MIT | Steam API integration (auth, achievements, overlay) | Required for Steam distribution |
| **ParrelSync** | Latest | MIT | Multi-editor instances for local multiplayer testing | Dev-only, not shipped |
| **Git LFS** | 2.x+ | MIT | Large file storage for art assets | Infrastructure requirement |

## Unity Packages (via Package Manager)

| Package | Version | Purpose | Notes |
|---|---|---|---|
| **Universal Render Pipeline (URP)** | Bundled with Unity 6 | Rendering pipeline | Required for target visual quality |
| **Input System** | Bundled | Modern input handling | Replaces legacy Input Manager |
| **TextMeshPro** | Bundled | UI text rendering | Required for accessibility (font switching) |
| **Addressables** | Bundled | Asset loading and bundling | Per-facility asset streaming |

---

## Monitoring & Review Schedule

| Review Trigger | Action |
|---|---|
| **Quarterly** | Check all dependencies for security patches and version updates |
| **Before each milestone** | Verify license compliance and cost projections |
| **When a dependency releases a major version** | Evaluate upgrade path and breaking changes |
| **If a vendor changes pricing** | Assess cost impact and evaluate alternatives |

---

## Risk Assessment

| Dependency | Vendor Lock-in Risk | Mitigation |
|---|---|---|
| **Unity 6** | 🔴 High | Architecture is modular — gameplay logic is engine-agnostic where possible |
| **Photon Fusion 2** | 🟡 Medium | Authority model and gameplay logic are abstracted behind interfaces |
| **PlayFab** | 🟡 Medium | Data export APIs available; standard REST/GraphQL patterns |
| **Vivox** | 🟢 Low | Voice is a leaf dependency; swappable with minimal gameplay impact |
| **Steamworks** | 🟢 Low | Steam is the primary platform; wrapper pattern isolates platform APIs |
