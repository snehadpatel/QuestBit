# Platform Deployment Guide

> **Last Updated:** 2026-07-15

This document defines the platform targets, build requirements, and distribution strategy for Project Echo.

---

## Platform Targets

### Primary (MVP)

| Platform | Target | Distribution | Priority |
|---|---|---|---|
| **Windows PC** | Windows 10+ (64-bit) | Steam | 🔴 Launch platform |

### Secondary (Post-Launch)

| Platform | Target | Distribution | Priority |
|---|---|---|---|
| **macOS** | macOS 12+ (Apple Silicon + Intel) | Steam | 🟡 Post-launch |
| **Linux** | Ubuntu 20.04+ / SteamOS | Steam (Steam Deck) | 🟡 Post-launch |

### Future Consideration

| Platform | Target | Distribution | Priority |
|---|---|---|---|
| **PlayStation 5** | Console | PlayStation Store | 🟢 If demand exists |
| **Xbox Series X/S** | Console | Microsoft Store / Game Pass | 🟢 If demand exists |
| **Nintendo Switch 2** | Console | Nintendo eShop | 🟢 Requires perf evaluation |

---

## Minimum System Requirements (PC)

| Component | Minimum | Recommended |
|---|---|---|
| **OS** | Windows 10 (64-bit) | Windows 11 (64-bit) |
| **CPU** | Intel Core i5-8400 / AMD Ryzen 5 2600 | Intel Core i7-10700 / AMD Ryzen 7 3700X |
| **RAM** | 8 GB | 16 GB |
| **GPU** | NVIDIA GTX 1050 Ti / AMD RX 570 (4GB VRAM) | NVIDIA RTX 2060 / AMD RX 5700 (6GB VRAM) |
| **Storage** | 10 GB available space (SSD recommended) | 10 GB SSD |
| **Network** | Broadband internet connection | Broadband internet connection |
| **Audio** | Microphone required for voice communication | Headset recommended |

---

## Build Configuration

### Unity Build Settings

| Setting | Value | Rationale |
|---|---|---|
| **Scripting Backend** | IL2CPP | Better performance, code stripping, platform compat |
| **API Compatibility** | .NET Standard 2.1 | Modern C# features without full .NET Framework bloat |
| **Color Space** | Linear | Required for correct PBR and post-processing |
| **Graphics API (Windows)** | DirectX 12 (primary), Vulkan (fallback) | Modern rendering with DX12 default |
| **Target Architecture** | x86_64 | 64-bit only |
| **Compression** | LZ4HC | Good compression ratio, fast decompression |
| **Managed Stripping Level** | Medium | Balance between size reduction and reflection safety |

### Build Size Targets

| Component | Target | Notes |
|---|---|---|
| **Initial download** | < 5 GB | Steam download size |
| **Installed size** | < 10 GB | Including all assets |
| **Per-facility asset bundle** | < 200 MB | Loaded at match start |
| **Patch size** | < 500 MB | Incremental updates |

---

## Steam Distribution

### Store Requirements

- [ ] Steam App ID registered
- [ ] Store page created with capsule images, screenshots, and trailer
- [ ] Steamworks SDK integrated (authentication, achievements, overlay)
- [ ] Steam Input API configured (controller support)
- [ ] Steam Rich Presence configured (show match status)
- [ ] Age rating: Mature (horror content)
- [ ] Regional pricing configured
- [ ] Steam Cloud save sync (via PlayFab → Steam Cloud bridge)

### Steam-Specific Features

| Feature | Status | Notes |
|---|---|---|
| **Steam Authentication** | Required | PlayFab uses Steam identity for PC accounts |
| **Steam Overlay** | Required | Must not conflict with in-game UI |
| **Steam Input** | Required | Controller support via Steam Input API |
| **Achievements** | Post-MVP | Define after progression system is finalized |
| **Trading Cards** | Post-launch | Low priority, community engagement feature |
| **Workshop** | Future | Potential for community-created facilities |
| **Remote Play Together** | Evaluate | May enable local co-op streaming |

### Release Checklist

- [ ] Build passes Steamworks compatibility tests
- [ ] Depot configuration for all supported platforms
- [ ] Beta branch configured for playtest builds
- [ ] Review and release pipeline documented
- [ ] Valve content review submission prepared
- [ ] Launch discount strategy decided

---

## Network Requirements

| Requirement | Specification |
|---|---|
| **Minimum bandwidth** | 1 Mbps upload/download |
| **Maximum latency** | 150ms (playable), 100ms (optimal) |
| **Ports** | Photon Fusion 2 default ports (UDP 5055-5058) |
| **Voice bandwidth** | ~40 kbps per player (Vivox) |
| **NAT traversal** | Handled by Photon relay servers |

---

## Certification & Compliance

| Requirement | Status | Notes |
|---|---|---|
| **ESRB Rating** | Required before launch | Horror content, online interactions |
| **PEGI Rating** | Required for EU distribution | Horror content classification |
| **Steam Content Review** | Required | Valve review for store listing |
| **GDPR (General)** | Required | Privacy policy, data handling for EU players |
| **Accessibility** | Best effort (MVP) | See GDD Ch. 25 and Accessibility Matrix |

---

## Post-Launch Update Pipeline

### Update Cadence

| Update Type | Frequency | Content |
|---|---|---|
| **Hotfix** | As needed | Critical bug fixes, crash fixes |
| **Patch** | Bi-weekly | Bug fixes, balance adjustments, QoL improvements |
| **Content Update** | Quarterly | New facilities, creature variants, cosmetics |
| **Major Update** | Bi-annually | New game modes, significant feature additions |

### Deployment Process

1. Build passes all automated tests
2. QA validation on staging branch
3. Deploy to Steam beta branch for community testing (48 hours)
4. Promote to default branch (live release)
5. Monitor crash reports and player feedback for 24 hours post-release
6. Hotfix if critical issues are discovered
