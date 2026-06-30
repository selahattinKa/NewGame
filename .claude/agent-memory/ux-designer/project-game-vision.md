---
name: project-game-vision
description: Core game context for Canavar Zindanları — genre, platform, pillars relevant to UX decisions
metadata:
  type: project
---

Game is **Canavar Zindanları** — a 2D mobile idle RPG inspired by Monster Warlord.
Platform: iOS + Android, touch-only (no gamepad, no hover).
Engine: Unity 6.3 LTS with URP and UI Toolkit (UXML/USS).
Minimum touch target: 44×44 dp. Minimum font: 14sp.

**Design Pillars (UX-relevant):**
- Pillar 4 (Güç Hisset / Feel Powerful): fluid transitions, responsive feedback reinforce player agency.
- Pillar 5 (Hep Bir Şey Var / Always Something Happening): navigation should surface pending actions; idle loop must be visible.

**Core idle loop:** offline resource/loot generation → player returns → collect → manage team → run dungeon → repeat.

**UX implication:** The most important moment in any session is the first 5 seconds after launch — surfacing offline gains. Every UX decision for the home/navigation layer should optimize for this moment.

**Currency system:** Gold (soft, primary, accumulates fast), Diamonds (premium, 0-9999 range, no abbreviation), Energy (gating resource, regenerates over time, shown as E/MAX).

**Why:** Knowing the game is idle-first (not action-first) changes every UX priority — persistence of missed information, badge-driven re-engagement, and empty states are more critical here than in a real-time action game.

**How to apply:** When designing any screen flow, ask: "What does a returning player (95% of sessions) see and do in the first 5 seconds?" That is the primary success criterion.

[[project-ui-framework-review]]
