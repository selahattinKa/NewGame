---
name: project-ui-framework-review
description: UI Framework GDD adversarial UX review — 16 BLOCKERS, 6 RECOMMENDED, 3 NICE-TO-HAVE (2026-06-25)
metadata:
  type: project
---

UI Framework GDD (`design/gdd/ui-framework.md`) adversarial UX review — 2026-06-25.

**Core finding:** GDD is infrastructure-first, player-journey-last. Transition mechanics, queue state machines, and timers are well-defined. What is missing is the player perspective layer — feedback on locked states, affordance clarity for visible-but-non-interactive elements, OS accessibility integration, and the idle-return reveal moment that is the emotional peak of the genre.

---

## BLOCKING UX Gaps

### Navigation
1. **iOS swipe-from-left back gesture undefined** — GDD only defines tap. iOS 13+ users expect left-edge swipe = stack pop. Undefined whether it triggers pop, closes modal, or is swallowed. Conflicts with AC-MOD-06 if gesture fires in front of modal.
2. **Destructive stack clear (tab re-tap) has no feedback** — Tapping active tab at depth 3 silently wipes all history. No animation distinction, no haptic, no "returned to root" signal. Feels like accidental data loss.

### Modal
3. **Modal queue depth indicator missing** — 5-modal first-session sequence (tutorial + event + level-up + new monster + friend invite) is a real scenario. No "X more notifications waiting" progress signal. Modal "bomb" makes closing feel mechanical, not meaningful.
4. **Modal stale content / TTL undefined** — A limited-time offer queued at T=0 shown at T=15min after 4 preceding modals: offer has expired, modal still shows "50% off — 30 min left." No timestamp, expiry check, or content-validity hook in the API.

### Toast
5. **Critical toasts drop silently under drop-oldest policy** — "Loot hazır!" (idle-return, the most important notification in the game) is toast #1. Three rapid subsequent toasts fill the queue; "Loot hazır!" is dropped. Core loop feedback lost. No priority parameter in `ShowToast()`.
6. **Toast type (info/success/warning) disconnected from queue priority** — A `warning` toast (critical) can be dropped behind an `info` toast (trivial). API needs `ShowToast(type, message, priority: ToastPriority)` where High-priority toasts are never dropped.

### Idle Loop (most critical)
7. **Idle-return reveal screen has no API and no design** — `ShowToast("Loot hazır!")` is a 3s HUD-bottom banner. Idle RPG genre standard (AFK Arena, Tap Titans 2, Monster Warlord): full-screen animated reveal, coin counter animation, particle FX, collect button. Framework cannot deliver Pillar 4 (Güç Hisset) for the highest-retention moment.
8. **Cold-start intercept API missing** — Normal navigation starts immediately on launch. No `UIManager.ShowFullscreenReveal(VisualTreeAsset, Action onCollect)` or equivalent layer (sort > 30, outside normal stack). Kaydetme/Yükleme and Otofarm systems have no hook to intercept launch flow for idle-reveal.

### Tab Lock
9. **LockTabs() provides no "why locked" signal** — Tab bar grays out. User taps tab, nothing happens. First interpretation: touch not registered. Second: app frozen. Third: bug. Need minimum: shake animation (150ms) or "Battle in progress" context label.
10. **Silent ignore on locked tap = perceived bug** — AC-NAV-11 only tests that navigation does not happen. No AC tests that user receives feedback. This is a UX gap masquerading as a technical pass.

### HUD / Modal
11. **Visible-but-non-interactive HUD = broken affordance** — Modal dim overlay (sort=20) covers HUD (sort=10). User sees gold/energy values, taps them, nothing happens. Either: (a) dim HUD to passive opacity (0.3–0.4), or (b) hide HUD entirely during modal. Current design does neither.

### Accessibility
12. **OS-level reduced motion auto-detect missing — iOS and Android APIs exist** — AC-ANIM-06 note claims "OS accessibility API unavailable in Unity 6.3." This is incomplete: iOS `UIAccessibility.isReduceMotionEnabled` accessible via native plugin; Android `Settings.Global.TRANSITION_ANIMATION_SCALE == 0` accessible via `AndroidJavaClass`. Apple HIG and Material Design 3 mandate OS preference respect. Advisory is not enough.
13. **Circular reduced motion problem** — To disable animations, user must navigate Settings (animated). Vestibular/epilepsy-risk users are exposed to motion before they can disable it.

### Safe Area
14. **Tab bar (56dp) + iOS home indicator (34dp) = physical overlap, no spec** — Spec says "safe area respected" but does not state whether 56dp is inside or in addition to safe area inset. Without explicit spec, tab icons land in home-indicator swipe zone; "Ana" tab tap triggers app minimize.
15. **HUD (48dp) + Dynamic Island / Notch = same overlap, no spec** — Dynamic Island is 37×126pt centered at top. HUD starting from y=0 renders behind Dynamic Island; energy/gold values occluded.
16. **Android 3 scenarios unaddressed** — Gesture nav (24-48dp bottom), 3-button nav (48-56dp bottom), punch-hole (24-36dp top). No per-scenario inset spec. This is a UX design decision, not an implementation detail UI Programmer can resolve alone.

---

## RECOMMENDED Gaps

- Tab transition animation direction (left/right) calculation rule undefined for non-adjacent tabs.
- No "breath gap" between sequential modals — modal train UX (queue fires immediately on close).
- HUD tap behavior (gold → market, energy → refill offer, diamonds → premium shop) undefined — if intentionally passive, state it explicitly.
- Dim overlay at 0.60 opacity over HUD: WCAG 2.1 AA 4.5:1 contrast ratio of HUD text not verified under overlay.
- Settings screen location (which tab, how many levels deep) unspecified; reduces accessibility discoverability.
- Toast + HUD + safe area Y-coordinate coordination unspecified for notch/Dynamic Island devices.

---

## NICE-TO-HAVE

- First boot: OS detect failing → light-mode animations default until user confirms preference.
- Toast tap-to-navigate affordance (tap toast → go to relevant screen).
- Tab bar during lock: show battle/autofarm progress indicator so user knows when tabs unlock.

---

**Why:** These are omissions in the player-journey layer, not contradictions in infrastructure. The GDD covers queue mechanics, timers, and state transitions well. What is absent: feedback on locked states, affordance clarity for visible-but-non-interactive elements, OS accessibility integration, and the idle-return reveal that is the emotional peak of the genre.

**How to apply:** When writing downstream UX specs (Savaş UI, Koleksiyon UI, Zindan Harita UI): cold-start intercept API must exist before Kaydetme/Yükleme UX can be specced; badge system and empty states are pre-requisites; safe area spec must be resolved before any screen layout is finalized.

[[project-game-vision]]
