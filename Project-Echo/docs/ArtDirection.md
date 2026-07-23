# Art Direction Guide

> **Last Updated:** 2026-07-15
> **Primary Visual Target:** Stylized 2.5D Storybook Look

This document establishes the artistic vision, visual constraints, and asset guidelines for Project Echo. It ensures all visual elements align with the game's unique asymmetric atmosphere.

---

## 1. Visual Pillars

### I. Stylized 2.5D Storybook Look
The environment is built using 3D geometry but rendered to evoke a hand-drawn, tactile feel.
- **Line Work**: Subtle, dynamic outline shaders on geometry edges (no harsh, uniform cartoon borders).
- **Textures**: Hand-painted, watercolor-like textures with visible brushstrokes and paper grain. Avoid realistic procedural materials.
- **Lighting**: Cel-shading gradients with soft thresholds. Highlights should look like ink or paint pools.

### II. Asymmetric Atmosphere
Since players inhabit different perception layers, the art must support dual states:
- **Default/Cohesive State**: Warm, inviting, and slightly faded — similar to a cozy children's illustration that has grown dusty.
- **Fractured/Threat State**: Cold, sharp, and high-contrast. Shadows lengthen, textures lose their warm undertones, and edges appear scratchy/distorted.

### III. Tactile & Mechanical
Interactable elements (valves, dials, levers, panels) must feel physically heavy and functional.
- Design focus on chunky, satisfying mechanics (screws, exposed wiring, analog gauges).
- Movement animations for machinery should have weight and drag.

---

## 2. Color Palette

```
  Warm/Cohesive (Primary)      Cool/Threat (Shifted)
┌─────────────────────────┐   ┌─────────────────────────┐
│ #D4C5B9 (Parchment)     │   │ #1E252B (Charcoal)      │
│ #8C7C72 (Driftwood)     │   │ #3B4B59 (Slate Blue)    │
│ #5C6B5E (Sage Green)    │   │ #7C8D9B (Ice Blue)      │
│ #D97E6A (Muted Coral)   │   │ #BF4545 (Warning Red)   │
└─────────────────────────┘   └─────────────────────────┘
```

- **Calm Mode Override**: When Calm Mode is active, color contrast is slightly flattened, high-frequency patterns are blurred, and flash effects are replaced with gentle fades.

---

## 3. Shader & Post-Processing Guidelines

### Outline Shader
- Screen-space depth-and-normal outline shader.
- Thickness must scale with distance from camera to prevent visual noise at long ranges.
- Enabled/disabled per-object based on visibility states.

### Paper/Texture Overlay
- A global, subtle fullscreen paper-grain noise overlay to maintain the "storybook" identity.
- Static or very slow-moving noise; high-frequency flicker is forbidden (triggers sensory issues).

### Post-Processing Profiles
- **Profile A (Normal)**: Low saturation, warm vignettes, soft paper bloom.
- **Profile B (Stress/Threat)**: Vignette darkens to cool blue-black, color saturation shifts to cool spectrum, contrast increases, outline lines become slightly jagged.

---

## 4. UI/UX Visual Style

- **Typography**: Legible, humanist sans-serif or clean serif (avoid overly decorative fonts). Native support for OpenDyslexic font swap.
- **HUD Elements**: Minimal. Most information should be integrated into the physical world (diegetic UI on tools/monitors) or displayed in clean, semi-transparent overlays.
- **Contrast**: Text must maintain a minimum 4.5:1 contrast ratio against UI backgrounds.

---

## 5. Asset Delivery Checklist

Before exporting any asset (3D model, texture, animation):

- [ ] Texture maps include diffuse/color and a custom paper-normal map (no standard PBR roughness/metallic maps).
- [ ] Vertex counts for static objects kept under 5,000 tris to fit low-end performance budgets.
- [ ] Edge outlines tested on the model's LODs.
- [ ] Calm Mode visual alternative prepared (if asset includes movement or flashing elements).
- [ ] Naming conventions followed (e.g., `prop_facility_valve_01` per naming convention doc).
