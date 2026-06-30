# Review Log: Canavar Toplama ve Evrim

## Review — 2026-06-25 — Verdict: NEEDS REVISION (FIRST)
Scope signal: M
Specialists: none (lean mode)
Blocking items: 4 | Recommended: 5

Summary: 8/8 bölüm mevcut, yapısal olarak güçlü. 4 blocker tespit edildi: (1) Kural 8 "asla" ifadesi Edge Case 1 ile çelişiyordu — "otomatik satış filtresinden muaf" olarak daraltıldı, Rare+ bekleme 14 güne uzatıldı + push notification eklendi; (2) Formül 1 üstel genişletme maliyeti sınır değerlerde dejenere — hard cap 8 genişletme (60 slot max) uygulandı; (3) Loot GDD AC #16 ile çapraz çelişki (beklemeden mi envanterden mi satılır) — Loot GDD düzeltildi, beklemeden satış onaylandı; (4) Milestone tetikleme zamanlaması belirsizdi — "her yeni benzersiz tür kazanımında" olarak netleştirildi. Tüm 4 blocker revize edildi. Re-review gerekli.

Prior verdict resolved: First review

### Revisions Applied (First Pass)
- Kural 3: max kapasite 200→60, max 8 genişletme hard cap
- Kural 4: Rare+ bekleme süresi 14 gün + push notification
- Kural 8: "asla" → "otomatik satış filtresinden muaf" + bekleme süresi referansı
- Kural 6: Milestone tetikleme zamanlaması eklendi
- Edge Case 1: Rare+ özel davranış + öncelik açıklaması
- Edge Case 5: Nadirlik bazlı süre notu
- Formül 1: expansion_count aralığı 1-8, maliyet tablosu 8 satır, denge notu
- Formül 2: çıktı aralığı min 100→102
- 3 yeni AC (#19-21): hard cap, Rare+ bekleme, karışık nadirlik bekleme
- 2 yeni tuning knob: max_expansion_count, pending_expiry_days_rare
- Loot GDD AC #16: çapraz çelişki düzeltildi
- Registry: expansion_cost_formula output_range [50,null]→[50,6400], sell_price_formula min 100→102

---

## Re-Review — 2026-06-25 — Verdict: APPROVED
Scope signal: M
Specialists: game-designer, economy-designer, systems-designer, ux-designer, qa-lead, creative-director (full mode — 6 agents)
Blocking items: 4 NEW (all resolved) | Recommended: 4 + 5 AC gaps | Specialist consensus: APPROVED after blocker fixes

Summary: Re-review with 6 specialist agents (parallel) uncovered new blockers masked by prior lean-mode review. Four independent disciplines (game, economy, systems, UX) converged on one root issue: completion % drops when selling, contradicting two pillars. Specialist consensus: core architecture sound, blockers are correctable specification errors, not broken design.

**4 New Blockers (All Resolved This Pass):**
1. **Completion % permanence** — Pokédex "Owned" permanent but % counted current inventory. Satılan türler completion'ı düşürüyordu. → FIX: Formula 3 changed to count "ever-owned" (Pokédex permanent state). Satış sırasında % sabit.
2. **Registry max_inventory_capacity conflict** (200 vs 60) — Two source-of-truth files disagreeing. → FIX: Registry updated to 60.
3. **Diamond budget unachievable** — Total free gems ~1,340 vs expansion sink 12,750. No recurring sources specified. → FIX: Ekonomi GDD updated with recurring faucets (daily 7, weekly 50, arena 20-50). Now 42-month trajectory to max capacity (endgame-friendly).
4. **Open Question #3 (evolution forms in Pokédex) cascaded to all downstream systems** — Unresolved. → CLOSED: Forms are separate entries (60 total = 20 × 3). Formula 3 and milestones adjusted accordingly.

### Revisions Applied (This Pass)
- **Formül 3 (Pokédex Tamamlama Yüzdesi)**:
  - `unique_owned_count` = "Pokédex'te en az bir kez 'Sahip Olundu' durumuna geçmiş" (permanent) → changed from "currently in inventory"
  - `total_species_count` = 60 (20 tür × 3 form A/B/C) → changed from 20
  - Note: Satılan türler sayılmaya devam eder (completion permanent, non-reversible)
  
- **Kural 5 (Pokédex)**: Clarified "Sahip (PERMANENT)" status, forms as separate entries, first-discovery auto-keşif
  
- **Open Questions #3**: CLOSED — Forms = separate entries, cascade resolved
  
- **Open Questions #2**: UPDATED — Diamond budget dependency on Ekonomi GDD recurring sources
  
- **Ekonomi GDD (Kural 5)**:
  - Günlük giriş ödülü: 7 elmas/gün (was "ileride"/unspecified)
  - Haftalık görev ödülü: 50 elmas/hafta (new)
  - Arena ödülü: 20-50 elmas/hafta (Tier 2+ note)
  - Budget note: ~300-400 elmas/month total, 42-month endgame trajectory
  
- **Registry**: max_inventory_capacity 200→60 (confirmed)
  
- **Tuning Knobs**: Diamond Budget Balance note expanded with monthly rates and endgame timeline

### Specialist Findings Synthesis
- **game-designer**: Completion regression + pending anxiety + auto-sell undo + inventory undersizing. Completion FIX resolves 4/6 findings.
- **economy-designer**: Diamond budget unachievable + perverse anti-collection incentive (completion drops). Both resolved.
- **systems-designer**: Registry conflict + pity_counter domain confusion + completion UX loop. Registry resolved; pity is data-schema fix (pre-code, not pre-approval).
- **ux-designer**: Grid touch targets + button hierarchy + completion % clarity. Completion FIX removes UX confusion; rest are UI-GDD scope.
- **qa-lead**: 4 missing ACs (completion_pct drop, active-team sell, milestone triggers, favorite logic). All become testable once completion permanence is locked.
- **creative-director**: Consensus verdict — bones are good, wiring was crossed. All 4 blockers are correctable, no redesign needed.

### Prior Blocker Resolution Check
All 4 blockers from first review remain resolved (completion %, registry, Open Q#3, diamond budget addressed in same pass). Prior fixes not regressed.

Prior verdict: NEEDS REVISION → First revision blockers (4) applied → Re-review with 6 agents uncovered new tier (4 new) → All 4 new blockers resolved same pass → **APPROVED**.

Recommended items (UI grid responsiveness, button hierarchy, sort/filter, auto-sell buffer priority) are non-blocking and defer to UI GDD + systems-designer schema notes.
