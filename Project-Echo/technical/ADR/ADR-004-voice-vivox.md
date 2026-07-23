# ADR-004: Voice Communication — Vivox

- **Status**: Accepted
- **Date**: 2026-07-01
- **Decision Maker**: Principal Architect

## Context

Project Echo's core gameplay loop depends on real-time voice communication between 2–4 players. Voice is the primary means of exchanging asymmetric information, and the game must support:
- Low-latency real-time voice chat
- Reliable session management tied to game sessions
- Quality of service under variable network conditions
- Fallback strategies when voice quality degrades

## Decision

**Use Vivox** for real-time voice communication.

## Alternatives Considered

| Option | Pros | Cons | Verdict |
|---|---|---|---|
| **Vivox** | Industry standard, Unity SDK, proven scale, positional audio support, free tier available | Vendor lock-in, limited customization of voice processing | ✅ Selected |
| **Photon Voice 2** | Integrates with Photon Fusion, same vendor | Less mature voice quality, fewer production references, limited spatial audio | ❌ Rejected |
| **Discord SDK** | Players already use Discord, familiar UX | External dependency, no in-game control, breaks immersion | ❌ Rejected |
| **Steam Voice** | Free with Steamworks, no extra vendor | Steam-only (limits future platform expansion), limited features | ❌ Rejected |

## Consequences

- Voice channels are created and managed per game session
- Voice state integrates with the communication system gameplay mechanics (GDD Ch. 05)
- Fallback communication (pings, text shortcuts) must exist for when voice quality degrades
- Voice is not recorded or processed for content moderation in MVP (reviewed for post-launch)
- Future: Positional audio support can be enabled for immersion

## Related Documents

- [Architecture.md](../Architecture.md)
- [GDD 05 Communication System](../../docs/GDD/05%20Communication%20System.md)
