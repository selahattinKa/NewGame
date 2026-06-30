---
name: project-ui-performance
description: UI Toolkit mobile performance risks identified across two adversarial reviews of ui-framework.md (2026-06-25 and 2026-06-26)
metadata:
  type: project
---

## First Pass Review (2026-06-25) — 5 BLOCKERS identified

1. Device spec conflict — A32 4G vs 5G SKU confusion; spec said Snapdragon 720G but 4G variant uses Helio G80.
2. 4 UIDocuments breaks cross-panel batching in UI Toolkit. Makes ≤40 DC budget harder and unvalidated.
3. IVisualElementScheduler vs tween library unspecified — raw lambda callbacks allocate GC heap.
4. AC-PERF-03 ≤40 draw call budget asserted with zero supporting estimate. Rough peak-state estimate 19–39 DCs.
5. No memory acceptance criterion. VisualTreeAsset instantiation, event subscription cleanup unspecified.

RECOMMENDED: PanelSettings output mode unspecified (Render Texture = 4 full-screen RTs). display:none tab switch causes layout recalculation spike (3–5ms on mobile). HUD debounce mechanism and thread-safety unspecified.

## Second Pass Review (2026-06-26) — Status After "6 Blockers Addressed"

GDD is now "2nd pass, 6 blockers addressed, 2 ADRs pending." PrimeTween was selected (resolves tween library ambiguity).

**Still BLOCKING:**

1. **HUD throttle semantics contradictory.** Formula says `throttle_first` (leading = fire immediately, suppress for 100ms) but description says "İlk tetiklenme anından 100ms sonra güncellenir" (trailing behavior = always 100ms lag). For isolated single events (buy, reward), this means 6-frame HUD display delay. Must clarify: leading or fixed-window trailing?

2. **SM-A326B uses MediaTek Dimensity 720, NOT Snapdragon 720G.** GDD now says "Samsung Galaxy A32 5G (SM-A326B, Snapdragon 720G)" — SM-A326B Europe variant uses Dimensity 720. SM-A326U (US) uses Snapdragon 750G. Incorrect chip target invalidates perf benchmarks.

3. **≤40 DC budget still unvalidated, ADR not written.** Peak state raw estimate: Game 8-15 DC + HUD 3-5 DC + Modal 4-8 DC + Toast 2-4 DC = 17–32 DC before any game content. ADR blocking implementation.

4. **Economy.GetCurrentGold() thread-safety undocumented.** Callback deliberately ignores event payload and re-reads Economy synchronously. If OnResourceChanged fires from idle/autofarm thread, this is a race condition. Thread-safety guarantee not specified anywhere.

**Still RECOMMENDED:**

5. display:none tab switch triggers full layout recalculation for revealed tab subtree (up to 8 stack frames × 150 VEs = ~3-5ms spike on first animation frame). AC-PERF-01 may catch this but only if trace is captured.

6. PrimeTween pool size not configured in GDD or pending ADR. Rapid tab switching (3 taps in 300ms) generates 3 Complete() + 3 new tweens. ADR-2 must cover pool capacity.

**MINOR:**

7. Toast overlap (t=3.0–3.4s): 2 concurrent VisualElements in same UIDocument. Opacity animation is mesh-level, does not break batching. Estimated cost: 0–1 extra DC. Acceptable.

**Why:** These were surfaced during adversarial performance reviews requested 2026-06-25 and 2026-06-26.
**How to apply:** Flag all BLOCKING items as must-resolve before UIManager implementation begins. Escalate 4-UIDocument ADR and device spec correction to technical-director. HUD throttle semantics must be resolved with game-designer before any HUD code is written.
