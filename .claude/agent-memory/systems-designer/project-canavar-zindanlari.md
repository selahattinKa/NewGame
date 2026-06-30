---
name: project-canavar-zindanlari
description: Canavar Zindanları mobile RPG key design facts — pivot decisions, deprecated systems, stat ranges
metadata:
  type: project
---

Mobile 2D turn-based RPG. Unity 6.3 LTS, iOS/Android.

**Pivot (2026-06-29): 4-monster team → 1 active pet.** All GDDs written for 4-unit team; hibrit-savas.md and related docs are stale on this dimension.

**Why:** Design simplification post-prototype. How to apply: flag 4-unit assumptions in any GDD review.

**Energy system deprecated (2026-06-29):** energy_per_turn=25 and energy_threshold=100 replaced by cooldown-based yetenek-sistemi. New constants: cd_heavy=3, cd_support=5, cd_special=8, cd_pet=4. hibrit-savas.md Formül 6 and Kural 6 are entirely stale.

**Why:** Cooldown system is simpler and more predictable for 1-pet design. How to apply: never reference energy_per_turn/energy_threshold as active — point to yetenek-sistemi.md cooldowns.

**Stat ranges (authoritative from context + registry):**
- ATK: 15–599 (pipeline+synergy, no commander bonus). Commander max: 658.
- DEF: 15–137
- HP: 18–258 (player monsters with full pipeline+synergy)
- Enemy HP max: ~195 (enemy_stat_formula output_range [15,195])
- defense_reduction_factor: 2 (divisor for DEF in damage formula)
- crit_multiplier: 2.0, crit_chance: 0.10
- element_multiplier: 0.75–1.50

**healer_heal_formula registry bug:** Registry says output_range [3,33] but with max_hp=258 and healer_skill_rate=0.20, true max = floor(258×0.20)=51. Registry is stale — was written with older max_hp.
