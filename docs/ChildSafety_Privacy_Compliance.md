# Child Safety & Privacy Compliance

> **Last Updated:** 2026-07-15
> **Applicable Project:** QuestBit (Educational Game)
> **Compliance Baseline:** COPPA, GDPR-K, WCAG 2.1 AA, ADA Section 504, CIPA

This document consolidates all child safety and privacy requirements from across the QuestBit technical architecture into a single auditable reference. It serves as the primary document for legal review, platform certification, and compliance verification.

---

## 1. Regulatory Framework

| Regulation | Jurisdiction | Key Requirements | QuestBit Compliance |
|---|---|---|---|
| **COPPA** | United States | Parental consent for data collection from children under 13, data minimization, no behavioral advertising | ✅ Designed in |
| **GDPR-K** | European Union | Parental consent for data processing of children, right to erasure, data portability, privacy by design | ✅ Designed in |
| **WCAG 2.1 AA** | International | Web content accessibility guidelines | ✅ Integrated into UI and input systems |
| **ADA Section 504** | United States | Accessibility in federally-funded programs | ✅ Integrated into input and accessibility systems |
| **CIPA** | United States | Internet safety for educational platforms | ✅ No unmoderated communication, no external links |

---

## 2. Data Minimization Principles

### What We Collect

| Data Type | Collected? | Storage Location | Justification |
|---|---|---|---|
| **Gameplay progress** | ✅ Yes | Local device (encrypted) | Required for save/resume functionality |
| **Mastery engine state** | ✅ Yes | Local device (encrypted) | Required for adaptive difficulty |
| **Telemetry events** | ✅ Yes | Local queue → server batch | Educational analytics (learning milestones, productive struggle time) |
| **Player name/username** | ❌ No | — | Not required — players identified by local profile |
| **Email address** | ❌ No | — | Not collected from children |
| **Location data** | ❌ No | — | Not required for any feature |
| **Device identifiers** | ❌ No | — | No advertising ID, IDFA, or GAID collection |
| **Photos/camera** | ❌ No | — | No camera access requested |
| **Microphone** | ❌ No | — | No voice features for children |
| **Contact list** | ❌ No | — | No social features requiring contacts |

### Architecture References

- Save System: `docs/architecture/15_save_system.md` — AES-256 local-first encryption, no PII storage
- Data Pipeline: `docs/architecture/16_data_pipeline.md` — local cache <10MB, no third-party SDKs
- Networking: `docs/architecture/21_networking_architecture.md` — async non-blocking, parental PIN gates

---

## 3. Parental Controls

| Feature | Implementation | Architecture Reference |
|---|---|---|
| **Parental PIN gate** | Required for cloud sync, account linking, and any network communication | Save System (15), Networking (21) |
| **Consent flow** | Must be completed before any data leaves the device | Data Pipeline (16) |
| **Data deletion request** | Parents can request full data erasure | Data Pipeline (16) |
| **Play time controls** | Parents can set daily play time limits | Quest System (19) — no streak-shaming |
| **Content controls** | Parents can restrict biome access or difficulty tiers | Quest System (19) |

---

## 4. Network Safety

| Requirement | Implementation | Architecture Reference |
|---|---|---|
| **No real-time player chat** | The game does not support text or voice chat between players | Networking (21) |
| **No lobbies** | Players cannot browse, join, or create multiplayer sessions | Networking (21) |
| **Async ghost visuals only** | Other players' progress is visible as ghost markers, not live avatars | Networking (21) |
| **No external links** | The game does not contain links to external websites | UI System |
| **No third-party SDKs** | Zero advertising, analytics, or social SDKs from third parties | Data Pipeline (16) |
| **No user-generated content** | Players cannot create, share, or receive content from others | By design |

---

## 5. Telemetry Compliance

### Telemetry Schema Constraints

| Constraint | Requirement |
|---|---|
| **No PII in payloads** | Telemetry events contain only anonymized gameplay data |
| **Local-first batching** | Events are queued locally and sent in batches (not real-time streaming) |
| **Local cache limit** | Maximum 10MB local telemetry cache |
| **No advertising IDs** | Zero IDFA, GAID, or device fingerprinting |
| **Educational purpose only** | Telemetry tracks learning milestones, productive struggle time, and Clue Journal interactions |
| **Parental consent required** | No telemetry leaves the device without parental PIN + consent |

### Telemetry Payload Structure

Per `docs/architecture/16_data_pipeline.md`:
- `skillId`: Identifies the learning skill (e.g., `math_fraction_halves`)
- `eventType`: Milestone completion, struggle detection, journal entry
- `duration`: Time spent on activity (anonymized)
- `outcome`: Success/failure/retry
- **No player identifiers, device IDs, or location data**

---

## 6. Save System Compliance

| Requirement | Implementation |
|---|---|
| **Encryption** | AES-256 encryption for all local save files |
| **No PII in save data** | Save schema contains gameplay state only |
| **Local-first** | Game is fully functional offline; cloud sync is optional and gated |
| **No streak-shaming** | Returning after absence shows positive messaging, not guilt |
| **No progress loss** | Conflict resolution prioritizes player progress preservation |
| **Parental PIN for cloud sync** | Cloud save features require parental authentication |

---

## 7. Accessibility as Safety

| Feature | Purpose | Architecture Reference |
|---|---|---|
| **Calm Mode** | Reduces visual/audio intensity for anxiety-sensitive children | Rendering (11), Audio (13), Event Bus (06) |
| **Dyslexia-optimized fonts** | Dynamic font switching with 1.5x typewriter delay | Localization (14), Dialogue (18) |
| **Switch-scan input** | Single-switch navigation for motor-impaired children | Input Manager (08) |
| **No time pressure** | Learning activities have no countdown timers | Quest System (19) |
| **No public leaderboards** | Progress is private — no competitive ranking of children | By design |

---

## 8. Compliance Verification Checklist

This checklist must be completed before any release:

- [ ] All telemetry payloads reviewed for PII leakage
- [ ] Parental consent flow tested end-to-end
- [ ] Data deletion request flow tested
- [ ] No third-party SDK network traffic confirmed via proxy inspection
- [ ] Save file encryption verified
- [ ] No external URLs or links in any UI
- [ ] No real-time communication between players
- [ ] Calm Mode tested for visual and audio reduction
- [ ] Accessibility features tested (switch-scan, font switching, text scaling)
- [ ] Privacy policy updated and accessible from the game
- [ ] Terms of service reviewed by legal counsel

---

## 9. Incident Response

If a compliance violation is discovered post-release:

1. **Immediately disable** the affected feature via remote config (if available) or emergency patch
2. **Notify** the compliance officer and legal team within 24 hours
3. **Document** the violation, root cause, and remediation steps
4. **Report** to relevant authorities (FTC for COPPA, DPA for GDPR-K) within required timeframes
5. **Verify** the fix through the compliance checklist before re-enabling the feature
