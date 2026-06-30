# Active Session State

<!-- STATUS -->
Epic: GDD Design
Feature: Yetenek Sistemi
Task: TAMAMLANDI — Tüm bölümler yazıldı, registry güncellendi, status: Designed
<!-- /STATUS -->

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
