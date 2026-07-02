# Active Session State

<!-- STATUS -->
Epic: GDD Design
Feature: Canavar Toplama ve Evrim
Task: REVİZE TAMAMLANDI (2026-07-02, 1. tur) — tam `/design-review` (game-designer, systems-designer, economy-designer, qa-lead, creative-director). Master blocker (Form A/B/C eski model vs F-D-C-B-A-S-SS tier pivotu) bu dosyanın kendi kapsamında ÇÖZÜLDÜ: Pokédex artık (tür,tier) modeli, instance şeması level-deneyim-sistemi.md ile hizalandı, satış formülü exploit'i kapatıldı (sell_level_bonus 0.02→0.01), S/SS otomatik satıştan muaf tutuldu (creative-director kararı). **SIRADAKI ADIM (kullanıcı onayladı)**: /clear sonrası `/design-review canavar-toplama-evrim.md` yeniden çalıştırılıp revizyonlar bağımsız doğrulanacak.
<!-- /STATUS -->

## Current Task (2026-07-02, 1. tur) — REVİZE TAMAMLANDI, RE-REVIEW BEKLİYOR

`/design-review canavar-toplama-evrim.md` çalıştırıldı (full mode, 4 uzman + creative-director). Verdict: MAJOR REVISION NEEDED — 12 blocker bulundu, ~7'si tek bir kök nedene (üç dosyada üç farklı evrim modeli) bağlıydı. Kullanıcı "şimdi revize et" dedi, 3 tasarım kararı alındı:
1. Pokédex modeli: **tür × ulaşılan tier** (değişken giriş sayısı, basit "sadece tür" seçeneği yerine)
2. Satış exploit fix: kullanıcı özel yönlendirme verdi ("seviyelenme XP iksiri/keşif ile kazanılıyor, bedava değil") → level bonusu kaldırılmadı, küçültüldü (0.02→0.01)
3. S/SS otomatik satış: creative-director önerisi kabul edildi — tamamen muaf (30 gün sınırı kaldırıldı)

Tüm 12 blocker + türev AC/formül/dependency güncellemeleri tek oturumda uygulandı (bkz. Files Modified). **Bu revizyonlar HENÜZ bağımsız bir uzman turundan geçmedi** — sıradaki adım kullanıcının seçtiği gibi temiz context'te yeniden `/design-review` çalıştırmak.

### Yeni oturumda ilk adım
1. Bu dosyayı oku (zaten otomatik olacak)
2. `/design-review canavar-toplama-evrim.md` çalıştır — 2. tur re-review, önceki 12 blocker'ın gerçekten çözüldüğünü doğrula
3. Not: Bu revizyon 3 kardeş dosyada (canavar-veritabani.md, canavar-guclendirme.md, kesif-alani.md, loot-odul-sistemi.md) yeni Open Question'lar (#6 kısmen, #7, #8) açtı — bunlar bu GDD'nin kapsamı dışında, ayrı `/design-review` oturumları gerektirir.

## Files Modified This Session (2026-07-02, 1. tur)
- design/gdd/canavar-toplama-evrim.md (12 blocker düzeltmesi: Kural 2 instance şeması, Kural 5 Pokédex tier-zinciri modeli, Kural 6 ödüller/milestone, Kural 1 Keşif Alanı kaynağı, Kural 4.3/4.5 S/SS muafiyeti, Formül 2/3 yeniden hesaplama, Dependencies, Edge Cases, Acceptance Criteria #1/5/6/7/8/9/10/21/22 revize + #23-26 yeni, Tuning Knobs, Open Questions #2/#3/#6 revize + #7/#8 yeni)

## Previous Task (2026-07-01, 3. tur) — TAMAMLANDI
`/design-review level-deneyim-sistemi.md` yeniden çalıştırıldı (4 uzman + creative-director, taze oturum). 2. turun 6 blocker'ı bağımsız doğrulandı: 4'ü tam ÇÖZÜLDÜ, 1'i (Kural 8 vs Kural 5 pseudocode) YENİ bir blocker doğurdu (`pet.banked_progress` tanımsız terim — systems-designer + economy-designer bağımsız yakaladı), 1 alan (Kural 3 atomiklik) AC eksikliği nedeniyle hâlâ blocking bulundu (qa-lead). Bu 2 yeni blocker + 5 önerilen madde (SS40 dead-end OQ8'e eklendi, MAX rozeti, overflow-stacking riski Open Questions'a taşındı, materyal-kontrol yarışı için deferral notu, remaining_tier_xp≈0 dejenere UX düzeltmesi) bu oturumda çözüldü. Nice-to-have (SS geçiş anı banked_xp stranding) da düzeltildi.
Kullanıcı disagreement not: game-designer oyuncu Lv30 sonrası büyüme eksikliğini hâlâ Blocking görüyor, creative-director roadmap kalemi olarak tutuyor (OQ#8) — çözülmedi, kayıtlı.

**Kalan durum**: Bu GDD'nin kendi kapsamındaki TÜM blocker'lar çözüldü. Implementasyon hâlâ 3 harici dosyanın revizyonuna bağlı (Open Questions #1, #6, #7) — bu GDD'nin dışında bir iş.

**Sıradaki önerilen adım**: `/design-review canavar-toplama-evrim.md` (bu GDD'yi açan en kritik blocker) veya kullanıcı tercihine göre başka bir sistem.

## Files Modified This Session (2026-07-01, 3. tur)
- design/gdd/level-deneyim-sistemi.md (2 yeni blocker düzeltmesi: banked_progress → XPInvestedThisTier tanımlandı, AC #29 atomiklik testi eklendi; + 5 önerilen madde: OQ#8 genişletildi, MAX rozeti, OQ#10 overflow-stacking, materyal-kontrol yarışı deferral notu, remaining_tier_xp≈0 UX düzeltmesi, SS geçiş anı banked_xp stranding düzeltmesi)

## Files Modified Previous Session (2026-07-01, 2. tur)
- design/gdd/level-deneyim-sistemi.md (1. turun 6 blocker düzeltmesi — bkz. review-log)
- design/gdd/kaydetme-yukleme.md (save şeması: player_level/xp, banked_xp, lifetime_pet_level eklendi)
- design/registry/entities.yaml (potion_lock_threshold_ratio → overfill_cap_ratio yeniden adlandırıldı)

## Files Modified Previous Session (2026-07-01, 1. tur)
- design/gdd/level-deneyim-sistemi.md (YENİ — 8 zorunlu + Visual/Audio + UI Requirements bölümleri, 2 formül, 23 acceptance criteria)
- design/registry/entities.yaml (xp_to_next_level_formula, xp_overflow_gold_formula, player_level_cap, pet_tier_level_caps, xp_to_gold_rate, potion_lock_threshold_ratio eklendi)
- design/gdd/systems-index.md (#34 eklendi, Pet Evrim #8 ve Oyuncu Sınıf #10 dependency güncellendi, Progress Tracker sayıları güncellendi)

## Current Task
**PIVOT + KISALTMALAR TAMAMLANDI (2026-06-26)**
- Tier sistemi: F-D-C-B-A-S-SS (7 kademe)
- Savaş ödülü: EXP ekranı (item düşmüyor)
- Monetization: IAP (gerçek para) — reklam değil
- Strateji: Prototype → Google Play soft launch (2-3 hafta)

**Önerilen sonraki adım: `/prototype tur-bazli-savas`**
Prototype kapsamı: 1 sınıf, 10 pet (F/C/S tier), 5 kat, EXP sistemi, temel IAP stub.
Google Play'e atmak için minimum: internal test → closed beta → production.

## Session Extract — /review-all-gdds 2026-06-25
- Verdict: CONCERNS
- GDDs reviewed: 13
- Flagged for revision: None (warnings only — provisional etiket temizliği + boss terminolojisi)
- Blocking issues: None
- Recommended next: /create-architecture veya provisional tag toplu temizliği
- Report: design/gdd/gdd-cross-review-2026-06-25.md
Lean review: 4 blockers found and resolved (Kural 8 çelişki, Formül 1 dejenere, Loot GDD çapraz tutarsızlık, milestone zamanlama).
Re-review pending: /clear → /design-review canavar-toplama-evrim.md
File: design/gdd/canavar-toplama-evrim.md
Previous: Loot / Ödül Sistemi GDD — REVISED (re-review pending).
Previous: Otofarm / Idle Sistemi GDD — Approved.
Previous: Kaydetme / Yükleme GDD — COMPLETE (Designed).
Previous: Zindan Keşif Sistemi GDD — COMPLETE (Designed).
<!-- CONSISTENCY-CHECK: 2026-06-24 | GDDs checked: 12 | Conflicts found: 0 | Verdict: PASS (zindan-kesif dahil) -->
<!-- CONSISTENCY-CHECK: 2026-06-25 | GDDs checked: 11 | Conflicts found: 0 | Verdict: PASS (canavar-toplama-evrim dahil) -->
<!-- CONSISTENCY-CHECK: 2026-06-25 | GDDs checked: 10 | Conflicts found: 0 | Verdict: PASS (hibrit-savas dahil) -->
<!-- CONSISTENCY-CHECK: 2026-06-25 | GDDs checked: 9 | Conflicts found: 0 | Verdict: PASS -->
<!-- CONSISTENCY-CHECK: 2026-06-25 | GDDs checked: 8 | Conflicts found: 0 | Verdict: PASS -->
<!-- CONSISTENCY-CHECK: 2026-06-24 | GDDs checked: 13 | Conflicts found: 0 | Verdict: PASS (kaydetme-yukleme dahil) -->

## Progress
- [x] Game concept (design/gdd/game-concept.md)
- [x] Art bible (design/art/art-bible.md) — 9 sections complete
- [x] Systems index (design/gdd/systems-index.md) — 22 systems mapped
- [x] GDD: Canavar Veritabanı (design/gdd/canavar-veritabani.md) — Designed
- [x] GDD: Element Sistemi (design/gdd/element-sistemi.md) — Designed
- [x] GDD: Ekonomi / Kaynak Yönetimi (design/gdd/ekonomi.md) — Designed
- [x] GDD: Sağlık / Can Sistemi (design/gdd/saglik-can-sistemi.md) — Designed
- [x] GDD: Hasar Hesaplama (design/gdd/hasar-hesaplama.md) — Designed
- [x] GDD: Canavar Güçlendirme (design/gdd/canavar-guclendirme.md) — Designed
- [x] GDD: Loot / Ödül Sistemi (design/gdd/loot-odul-sistemi.md) — Designed
- [x] GDD: Takım Kurma (design/gdd/takim-kurma.md) — Designed
- [x] GDD: Düşman AI (design/gdd/dusuman-ai.md) — Designed
- [x] GDD: Hibrit Savaş Sistemi (design/gdd/hibrit-savas.md) — Designed
- [x] GDD: Canavar Toplama ve Evrim (design/gdd/canavar-toplama-evrim.md) — In Review (revised, re-review pending)
- [x] GDD: Zindan Keşif Sistemi (design/gdd/zindan-kesif.md) — Designed
- [x] GDD: Kaydetme / Yükleme (design/gdd/kaydetme-yukleme.md) — Designed
- [x] GDD: Otofarm / Idle Sistemi (design/gdd/otofarm-idle.md) — Designed
- [x] GDD: UI Framework (design/gdd/ui-framework.md) — Approved ✅
- [ ] PIVOT revizyonları — bakınız Current Task

## Design Order (prototype-first)
**PATH A — Hızlı (önerilen):**
1. `/prototype tur-bazli-savas` — 2-3 hafta → Google Play
2. Prototype verisi → MVP kararı

**PATH B — Design-first (prototype sonrası veya paralel):**
1. `/design-review canavar-veritabani` — F-D-C-B-A-S-SS tier revision
2. `/design-review canavar-toplama-evrim` — Pet Evrim F→SS
3. `/design-review hibrit-savas` — Savaş Sistemi + EXP
4. `/design-review loot-odul-sistemi` — EXP/milestone sistemi
5. `/design-system oyuncu-sinif-sistemi` — 4 sınıf × 3 dal
6. `/design-system kostum-elbise-sistemi`
7. `/design-system iap-magaza-sistemi` — Google Play IAP

## Yapılacaklar Listesi (Öncelik Sırasıyla)

### Unity Projesi (Kuruldu 2026-06-29)
- [x] Unity 6.3.18f1 projesi oluşturuldu (repo root'ta)
- [x] Packages: URP 17.3.0, InputSystem 1.11.2, Addressables 2.3.1, TMP 5.0.0, NUnit 1.6.0
- [x] Assembly definition: CanavarZindanlari.Runtime.asmdef
- [x] Core scripts: MonsterData, SkillData, DamageCalculator, CombatUnit, CombatManager, EconomyManager, GameManager
- [x] Test: DamageCalculatorTests (element matrix, crit, min floor)
- [ ] Unity Hub'da projeyi aç → Package Manager paketleri indir → URP pipeline asset kur

### Prototype (2-3 hafta — Google Play soft launch için minimum)
- [x] `/prototype tur-bazli-savas` — TAMAMLANDI, karar: **PROCEED** (2026-06-29)
  - Rapor: `prototypes/tur-bazli-savas-concept/REPORT.md`
  - Karar gerekçesi: Tam ekran düşman "güçlü his" yarattı; MMORPG video animasyonuyla uyumsuz; tur bazlı Kling AI pipeline'ıyla %100 uyumlu
- [ ] Düşman kameraya bakıyor — AI görsel üretimi (ilk 5 yaratık, front-facing prompt)
- [ ] Savaş sonu ödül ekranı: EXP + düşen eşyalar listesi
- [ ] Temel IAP mağazası (2-3 ürün — Google Play Billing)
- [ ] AdMob rewarded reklam entegrasyonu
- [ ] Portrait kilit (`Screen.orientation = ScreenOrientation.Portrait`)
- [ ] İntikam Sistemi — prototype'a ekle (basit, etkili)
- [ ] Aranıyor Tahtası — prototype'a ekle (günlük giriş çekici)
- [ ] Yaşayan Kart Portreleri — AI video ile 2-3 frame döngü animasyonu

### MVP Sonrası (Google Play verisi sonrası karar)
- [ ] 4 sınıf × 3 dal tam sistemi
- [ ] 20-30 pet (F-D-C-B-A-S tier)
- [ ] Tam kostüm/elbise sistemi
- [ ] Pet Sadakat Sistemi
- [ ] Canavar Kütüphanesi (Codex)
- [ ] Irk Savaşı Canlı Sıralaması

### Design Revizyonları (zaman buldukça)
- [ ] `/design-review canavar-veritabani` — F-D-C-B-A-S-SS tier şeması
- [ ] `/design-review canavar-toplama-evrim` — Pet Evrim F→SS kuralları
- [ ] `/design-review hibrit-savas` — Savaş Sistemi + ödül ekranı
- [ ] `/design-review loot-odul-sistemi` — Ödül ekranı olarak yeniden yaz
- [ ] `/design-system oyuncu-sinif-sistemi`
- [ ] `/design-system kostum-elbise-sistemi`
- [ ] `/design-system iap-reklam-sistemi`

### Lokalizasyon (aşamalı)
- [ ] **Prototype**: Türkçe ✅ + İngilizce
- [ ] **MVP sonrası**: Portekizce (BR) + İspanyolca (LATAM) + Endonezce
- [ ] **Tier 2**: Filipince + Tayca + Vietnamca + Rusça
- [ ] **Tier 3**: Arapça (RTL — UI yeniden düzenleme) + Almanca + Fransızca + Geleneksel Çince
- [ ] Unity Localization paketi kurulumu — string table mimarisi
- [ ] Font atlas: Tayca, Vietnamca, Arapça için ayrı font setleri
- [ ] Kültürel uyum kontrolü: Orta Doğu pazarı için yaratık tasarımı gözden geçirme

### Pazarlama (prototype hazır olunca)
- [ ] Google Play store sayfası — açıklama + screenshot + ikon
- [ ] TikTok içerik takvimi — pet evrim videoları + "bana bakıyor" savaş görüntüleri
- [ ] Reddit soft launch postu (r/androidgaming, r/indiegaming)
- [ ] AI art prompt seti — yaratık görselleri + savaş ekranı + kart çerçeveleri

## Key Decisions
- Art style: Karanlık Epik canavarlar + parlak dünya kontrastı (değişmedi)
- Monster tone: Monster Hunter / Dark Souls inspired (değişmedi)
- **PIVOT: 4 tier sistemi: B → A → S → SS** (5 nadirlik sistemi kaldırıldı)
- 4 elements: Ateş, Su, Toprak, Hava (değişmedi)
- **PIVOT: MVP: 20 systems, 15-20 pet (B-A-S), 4 sınıf, 2 ırk, 1 zindan, 10 kat**
- **PIVOT: Monetization: reklam tabanlı, tamamen ücretsiz**
- **PIVOT: Savaş: tur bazlı cooldown + oto-savaş toggle; 1 aktif pet**
- **PIVOT: Pet = Canavar; B→SS evrim; görünüş değişiyor; bazıları SS'e ulaşamaz**
- **PIVOT: 2 başlangıç ırkı (hikaye çatışması, etkinlik konsepti)**
- **PIVOT: AI kart görseli (sprite animasyonu yok) + AI video saldırı animasyonları**
- Kostüm/elbise sistemi: oyuncu + pet, B-A-S-SS dereceli ekipman
- Mağaza sistemi: reklam izleyerek kazan

## Files Modified This Session
- design/gdd/zindan-kesif.md (REVISED: "Detailed Design"→"Detailed Rules", FullHeal adımı eklendi, Open Q #3/#5 kapatıldı)
- design/gdd/ekonomi.md (edge case düzeltildi: kaybetme modeli Zindan Keşif'e uyarlandı)
- design/gdd/loot-odul-sistemi.md (first-clear tablosu 140→190 elmas, AC #27 düzeltildi, oran ~42x→~58x)
- design/gdd/hibrit-savas.md (5 provisional etiketi kaldırıldı, Zindan Keşif referansları doğrulandı)
- design/gdd/reviews/zindan-kesif-review-log.md (created — review log, APPROVED)
- design/gdd/systems-index.md (Zindan Keşif: Designed → Approved, reviewed/approved counts 12→13)
- design/registry/entities.yaml (3 first_clear_gems referenced_by'a loot-odul-sistemi.md eklendi)
- production/session-state/active.md (this file)
<!-- CONSISTENCY-CHECK: 2026-06-25 | GDDs checked: 14 | Conflicts found: 0 | 3 stale referenced_by fixed -->
