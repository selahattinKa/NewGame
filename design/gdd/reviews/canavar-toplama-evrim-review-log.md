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

---

## Re-Review — 2026-07-02 — Verdict: MAJOR REVISION NEEDED → Revised same session (re-review pending)
Scope signal: L-XL (kullanıcı "şimdi birleştir" kararıyla XL'e yaklaştı)
Specialists: game-designer, systems-designer, economy-designer, qa-lead, creative-director (full mode — 5 agents)
Blocking items: 10 (tümü bu oturumda ele alındı) | Recommended: 6 (2 kullanıcı kararıyla "belgele, değiştirme" olarak kapatıldı, kalanı doküman içi notlarla kapatıldı)

**Not**: Bu turun başlangıcında dosya zaten aynı gün (2026-07-02) içinde büyük bir self-revizyon geçirmişti (Form A/B/C model kaldırılmış, tier-zinciri Pokédex modeli getirilmişti) ama bu hiç review log'a işlenmemişti — önceki log entry hâlâ 2026-06-25 APPROVED gösteriyordu. Bu tur, hem o self-revizyonu hem de yeni bulunan sorunları kapsayan resmi bir re-review'dur.

**Kök neden bulgusu**: systems-index.md "Canavar Güçlendirme bu dosyaya birleşti" diyordu (mimari karar) ama GDD'nin kendisi `canavar-guclendirme.md`'yi hâlâ ayrı/canlı sert bağımlılık sayıyordu — verilmemiş bir mimari karar, birçok alt-bloklayıcının kaynağıydı (qa-lead'in AC fixture çelişkisi, systems-designer'ın evolution_stage veri kaynağı kopukluğu).

### Kullanıcı Kararları (bu turda)
1. **Canavar Güçlendirme**: Ayrı tut değil, **şimdi birleştir** — kapsamı genişletti (yeni Kural 10/11, Formül 4)
2. **S/SS süresiz bekleme**: Orta yol — kalıcı beklemedeki S/SS, Güçlendirme/takım için envantere taşınana kadar kullanılamaz (Kural 4.7)
3. **Pokédex nadirlik-ödül ters orantı**: Flat ödül korundu, Player Fantasy'ye dürüstlük notu eklendi
4. **Formül 2 tier-sınır ihlali (B+)**: Sadece belgelendi, sayılar değiştirilmedi

### Revisions Applied (This Pass)
- **Canavar Güçlendirme birleşmesi**: Kural 10 (Evrim Yürütme — deterministik, pity kaldırıldı), Kural 11 (Yıldız Sistemi, F-D-C-B-A-S-SS çarpanları), Formül 4 (Yıldız Altın Maliyeti) eklendi. `canavar-guclendirme.md` Status → Deprecated + yönlendirme notu. Dependencies tablosu, Interactions tablosu, States/Transitions güncellendi.
- **AC fixture düzeltmesi**: AC#1/#6, `canavar-veritabani.md`'nin kanonik fire-striker-infernalclaw (C-rarity) örneğiyle tutarlı hale getirildi (F→C, D→B)
- **AC#7/#8**: Mock-fixture (`total_pokedex_entries=62`) ile unit-test olarak yeniden yazıldı, test harness notu eklendi
- **6 yeni AC (27-32)**: bekleme-alanı birleştirme tüketimi, evrim yürütme, malzeme eksikliği reddi, yıldız yükseltme, max-yıldız kopya davranışı, S/SS kullanım kısıtı
- **Kural 4/Kural 8 çelişkisi**: Kural 8'deki stale "S/SS 30 gün" notu kaldırıldı
- **Kural 4.7 (yeni)**: Kalıcı beklemedeki S/SS'in Güçlendirme/takım kullanım kısıtı
- **Diamond Budget**: Düzeltilmiş musluk hesabı yazıldı (~263 elmas/ay, ~48.5 ay — önceki geçersiz "42 ay"ın yerine), açık "bu sayı belirlenene kadar implemente etme" notu eklendi
- **Player Fantasy**: Fusion'ın asıl değer yolu olduğu netliği + Discovery/Mastery dürüstlük notu eklendi
- **5 yeni Edge Case**: evrim malzeme hataları, race condition, max-yıldız kopya, S/SS kullanım kısıtı
- **Open Questions #2/#6 güncellendi**: #6 büyük ölçüde çözüldü (tek kalan kırıntı: `canavar-veritabani.md` Kural 6 form-gate çelişkisi)

### Prior Blocker Resolution Check
2026-06-25 turundaki 4+4 blocker (completion %, registry, Open Q#3, diamond budget) rejenerasyonda regresyona uğramadı — hepsi bu turda da geçerliliğini koruyor. Ama dosya bu APPROVED verdict'ten SONRA, review log'a işlenmeyen bir self-revizyon geçirmişti (Form A/B/C → Tier Zinciri pivotu) — bu turun asıl işi o pivotu resmi olarak doğrulamak ve yeni ortaya çıkan (özellikle Güçlendirme birleşmesi) sorunları kapatmaktı.

Prior verdict: APPROVED (2026-06-25, stale — dosya sonradan işlenmemiş şekilde revize edildi) → Bu tur MAJOR REVISION NEEDED tespit etti → Kullanıcı kararlarıyla aynı oturumda revize edildi → **Re-review yeni oturumda (context temiz) planlanıyor, henüz onaylanmadı.**

Recommended items (Formül 2 tier-sınır ihlali, S/SS satış riski) bilinçli olarak "belgele, değiştirme" kararıyla kapatıldı — kullanıcı bu riskleri kabul etti, gelecekteki balance testing'de izlenecek.

---

## Re-Review — 2026-07-02 — Verdict: NEEDS REVISION → Revised same session (re-review pending)
Scope signal: M
Specialists: game-designer, systems-designer, economy-designer, qa-lead, creative-director (full mode — 5 agents, parallel)
Blocking items: 8 (tümü bu oturumda ele alındı) | Recommended: 7 (3'ü bu oturumda ek olarak çözüldü — AC#16/#18 netleştirmesi, S/SS VFX/Audio beat'i)

**Not**: Bu tur, 2. turun aynı gün içindeki self-revizyonunun (Canavar Güçlendirme birleşmesi, tier-zinciri Pokédex modeli) temiz context'te resmi doğrulamasıydı. 4 uzman ajan paralel çalıştırıldı, ardından creative-director sentez yaptı.

### Uzman Yakınsamaları (Kritik Bulgular)
1. **Üç uzmanın (game-designer, systems-designer, creative-director) yakınsadığı bulgu**: `canavar-veritabani.md` Kural 6'nın tier-içi Form 1-3 gate'i, dosyanın kendi "tek kalan kırıntı" çerçevesinin öngördüğünden daha ciddi — hem Kural 5'in giriş-sayısı tablosunu (tier başına 2-3× eksik sayım riski) hem de `GetMonsterDefinition()`'ın "tier-başına base stat havuzu" arayüz varsayımını geçersiz kılabilir (upstream hâlâ Deprecated Canavar Güçlendirme'ye bağlı).
2. **İki uzmanın (game-designer, economy-designer) yakınsadığı bulgu**: Flat Pokédex ödülü (Kural 6a) + değişken tier-zinciri uzunluğu (Kural 5), "nadir düşüş = doruk an" Player Fantasy iddiasını sayısal olarak zayıflatıyor — düşük-tier türleri toplu evrimleştirmek nadir tür ilerletmekten daha gem-verimli.
3. **Ekonomi çerçeve tersine-dönüşü**: economy-designer, 2. turda "kesif-alani.md verisi bekleniyor" gerekçesiyle ertelenen "S/SS satış riski"ni `ekonomi.md`'nin MEVCUT idle-altın formülüyle somutlaştırdı (SS ★5 = 1 günlük idle geliri) — erteleme gerekçesi geçersiz olduğundan creative-director bu notun bu turda güncellenmesini onayladı.

### Revisions Applied (This Pass)
- **Open Questions #6**: "Büyük ölçüde çözüldü / tek kalan kırıntı" → **onaylı blocker**'a yükseltildi, iki somut sonuç eklendi (arayüz sözleşmesi eksikliği + giriş-sayısı eksik-sayım riski)
- **Kural 5 (Tier Zinciri Girişleri)**: Giriş-sayısı tablosuna "provisional — canavar-veritabani.md revizyonuna bağlı" uyarısı eklendi
- **Formül 3**: Sayısal örnek notu, ikili bağımlılığı (roster + Kural 6 çözümü) yansıtacak şekilde güncellendi + mock-değer kod-koruma notu eklendi
- **Formül 2**: "Bilinen sınırlama" notuna tolere-edilen-davranış + UI sıralama notu eklendi; "Satış-yoluyla-altın riski" notu ekonomi.md idle-altın rakamlarıyla somutlaştırıldı
- **Formül 4**: Yeni "Ölçek uyumsuzluğu" notu eklendi (SS yıldız yolu F'den ~16× daha kolay kendi kendini finanse ediyor)
- **Tuning Knobs**: S/SS satış riski notu, soyut "izlenmeli" çerçevesinden somut sayılara (ekonomi.md idle formülü referansıyla) güncellendi
- **Edge Cases**: Kural 11 (yıldız birleştirme) için Kural 10 ile simetrik yeni race-condition edge case eklendi
- **Dependencies**: `ekonomi.md`'nin stale altın-evrim modeli + idle-altın/Formül 4 sink uyumsuzluğu için yeni çapraz-bağımlılık notu eklendi
- **Acceptance Criteria**: Yeni AC#33 (Kural 4.7 yeniden-izin yarısı) eklendi; AC#7/#8 mock-değer adlandırması `MOCK_TOTAL_POKEDEX_ENTRIES__DO_NOT_SHIP` kuralına bağlandı; AC#11 altına Formül 2 tier-sınır ihlali için "tolere edilen davranış" notu eklendi; AC#16 kapsamı (yalnızca F-A bekleme, kalıcı S/SS hariç) netleştirildi; AC#18 event-log sıralamasıyla test edilebilir hale getirildi
- **Visual/Audio Requirements**: S/SS'e özel VFX ("doruk an" tam ekran beat'i) ve Audio (orkestral stinger) satırları eklendi
- **Status blockquote**: 3. tur özeti + güncellenmiş açık madde listesi ile yenilendi

### Prior Blocker Resolution Check
2. ve 1. turun tüm blocker'ları (completion %, registry, Open Q#3, diamond budget, Güçlendirme birleşmesi, fixture düzeltmesi) bu turda da regresyona uğramadan geçerliliğini koruyor. Bu turun asıl işi, o revizyonların temiz-context doğrulamasıydı ve doğrulama sürecinde 8 yeni blocker (çoğu spesifikasyon sıkılaştırması, 1 tanesi upstream-bağımlılık yükseltmesi) ortaya çıkardı.

Prior verdict: MAJOR REVISION NEEDED (2. tur, aynı gün revize edildi, re-review bekliyordu) → Bu tur (3. tur, temiz context) NEEDS REVISION tespit etti (mimari sağlam, redesign gerekmedi) → Kullanıcı "şimdi revize et" kararıyla aynı oturumda revize edildi → **Re-review yeni oturumda (context temiz) planlanıyor, henüz onaylanmadı.**

Recommended items (Kural 4.7'nin gerçek davranışsal kısıt yaratmadığı notu, Kural 11 fusion'ın "gerçek seçim" olmadığı çerçevesi) belgeleme düzeyinde bırakıldı — mekanik değişikliği gerektirmiyor, kullanıcı onayı bekliyor.
