# Accessibility Compliance Matrix

> **Last Updated:** 2026-07-15
> **Reference Standard:** WCAG 2.1 AA + Game Accessibility Guidelines (GAG)
> **Primary GDD Reference:** GDD Ch. 25 (Accessibility)

This matrix maps specific accessibility requirements to the game systems responsible for implementing them and tracks their implementation status.

---

## Legend

| Status | Meaning |
|---|---|
| ✅ | Designed and specified |
| 🔧 | Designed, implementation pending |
| ❌ | Not yet addressed |

---

## Visual Accessibility

| Requirement | WCAG Criterion | Implementing System | Status | Notes |
|---|---|---|---|---|
| Colorblind-safe palettes | 1.4.1 Use of Color | Rendering Pipeline, UI | 🔧 | Color + shape cues for all warnings and states |
| High contrast UI mode | 1.4.3 Contrast (Minimum) | UI System | 🔧 | Minimum 4.5:1 contrast ratio for text |
| Subtitle captions | 1.2.2 Captions | Audio System, UI | 🔧 | Environmental audio and voice cues captioned |
| Scalable UI text | 1.4.4 Resize Text | UI System, Localization | 🔧 | Support 100%–200% text scaling |
| Reduced motion mode | 2.3.1 Three Flashes | Rendering Pipeline | 🔧 | Disable screen shake, reduce particle effects |
| Clear visual hierarchy | 1.3.1 Info and Relationships | UI System | 🔧 | Strong hierarchy in all menus and HUD |
| Screen reader support | 4.1.2 Name, Role, Value | UI System | ❌ | Future improvement — requires UI Toolkit integration |

## Audio Accessibility

| Requirement | WCAG Criterion | Implementing System | Status | Notes |
|---|---|---|---|---|
| Visual indicators for audio cues | 1.1.1 Non-text Content | UI System, Audio System | 🔧 | On-screen indicators for directional sounds |
| Volume controls per channel | — (GAG) | Audio System | 🔧 | Separate sliders: Master, SFX, Music, Voice, Ambient |
| Subtitle display for speech | 1.2.2 Captions | Audio System, UI | 🔧 | Size, background, position configurable |
| Mono audio option | — (GAG) | Audio System | 🔧 | For single-sided hearing |

## Input & Motor Accessibility

| Requirement | WCAG Criterion | Implementing System | Status | Notes |
|---|---|---|---|---|
| Fully remappable controls | 2.1.1 Keyboard | Input Manager | 🔧 | All actions remappable |
| Toggle vs hold options | — (GAG) | Input Manager | 🔧 | All hold interactions have toggle alternative |
| Configurable dead zones | — (GAG) | Input Manager | 🔧 | Analog stick dead zone adjustment |
| Adjustable interaction timing | 2.2.1 Timing Adjustable | Player Systems, Input Manager | 🔧 | Extended hold durations (1.5x, 2x) |
| One-handed play support | — (GAG) | Input Manager | ❌ | Future improvement — requires alternate control scheme |
| Controller support | 2.1.1 Keyboard | Input Manager, Steam Input | 🔧 | Via Steam Input API |

## Communication Accessibility

| Requirement | Standard | Implementing System | Status | Notes |
|---|---|---|---|---|
| Non-voice fallback communication | — (GAG) | Communication System | 🔧 | Ping system + contextual tags |
| Text chat option | — (GAG) | Communication System, UI | 🔧 | For players who cannot use voice |
| Speech-to-text | — (GAG) | Communication System | ❌ | Future improvement |
| Text-to-speech | — (GAG) | Communication System | ❌ | Future improvement |
| Quick communication phrases | — (GAG) | Communication System | 🔧 | Pre-defined context-sensitive callouts |

## Cognitive Accessibility

| Requirement | Standard | Implementing System | Status | Notes |
|---|---|---|---|---|
| Clear objective markers | — (GAG) | Objective System, UI | 🔧 | Always visible when enabled |
| Tutorial / onboarding flow | — (GAG) | Game State, UI | 🔧 | Teach communication loop before live play |
| Consistent visual language | — (GAG) | UI System, Rendering | 🔧 | Same symbols/colors mean same things throughout |
| Difficulty options | — (GAG) | Creature AI, Objective System | ❌ | Future — creature aggression and timer adjustments |
| Dyslexia-friendly font option | — (GAG) | Localization System, UI | 🔧 | OpenDyslexic or similar font toggle |
| Pause functionality | — (GAG) | Game State | ❌ | Complex in multiplayer — requires team vote or host control |

---

## MVP Accessibility Requirements

The following are **mandatory for the first public release** (per GDD Ch. 25 and Testing open questions):

1. ✅ Colorblind-safe visual design (color + shape)
2. ✅ Subtitle/caption system for environmental and voice audio
3. ✅ Remappable controls
4. ✅ Toggle alternatives for all hold interactions
5. ✅ Non-voice fallback communication (ping + contextual tags)
6. ✅ Volume controls per audio channel
7. ✅ Configurable text size
8. ✅ Reduced motion option

---

## Post-Launch Accessibility Roadmap

| Phase | Feature | Priority |
|---|---|---|
| **Update 1** | Difficulty options (creature aggression scaling) | High |
| **Update 2** | Speech-to-text integration | Medium |
| **Update 3** | One-handed control scheme | Medium |
| **Update 4** | Screen reader support for menus | Low |
