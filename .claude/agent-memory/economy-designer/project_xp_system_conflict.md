---
name: project-xp-system-conflict
description: canavar-guclendirme.md's old gold-to-XP injection system conflicts with level-deneyim-sistemi.md's new tier-capped battle-XP design
metadata:
  type: project
---

Discovered 2026-07-01 while reviewing `design/gdd/level-deneyim-sistemi.md` (new
Level/XP GDD, in design). `design/gdd/canavar-guclendirme.md` (Kural 2, "Seviye
Atlama — XP Hibrit Sistemi") already describes a DIFFERENT monster leveling
model: max level 50 (flat, no tier caps), gold-injection to buy XP directly via
`level_up_cost_formula` (registry, source `ekonomi.md`), a "Hızlı Seviye" bulk-buy
button, and no mention of tier-based caps (F=10/D=15/C=20/B=25/A=30/S=35/SS=40)
or evolution-triggered reset to level 1.

The new `level-deneyim-sistemi.md` locked context (approved this session) says
pet leveling is battle-XP-only (full ~240 XP grant per victory, shared
simultaneously with player), with tier-based level caps and reset-to-1 on
evolution — no gold-injection mechanic mentioned.

**Why this matters**: `level_up_cost_formula` is a registered cross-system
formula referenced by both `ekonomi.md` and `canavar-guclendirme.md`. If the
new GDD supersedes the gold-injection sub-system, that formula and its
Acceptance Criteria (ekonomi.md lines ~370-372) and canavar-guclendirme.md's
Kural 2/Edge Cases (Lv50 cap, partial-XP gold injection, "Hızlı Seviye" button)
need to be explicitly deprecated or reconciled — not left silently
contradicting the new GDD.

**How to apply**: Before finalizing Formulas/Dependencies sections of
level-deneyim-sistemi.md, flag this to the user as a required decision:
(a) deprecate canavar-guclendirme.md's gold-injection XP sub-system entirely
in favor of tier-capped battle XP, or (b) keep gold-injection as a supplementary
XP source on top of battle XP and reconcile the level cap mismatch (50 vs
tier caps 10-40). Do not silently pick one — this crosses into
canavar-guclendirme.md (systems-designer's likely domain) and needs explicit
sign-off. See [[project_xp_overflow_sink_proposal]] once resolved.
