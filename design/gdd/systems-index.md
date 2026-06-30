# Systems Index: Canavar Zindanları (Monster Dungeons)

> **Status**: Approved
> **Created**: 2026-06-24
> **Last Updated**: 2026-06-29
> **Source Concept**: design/gdd/game-concept.md

---

## Overview

Canavar Zindanları, 2D Tur Bazlı RPG + Pet Toplama + Zindan Crawler olarak 29 bağımsız sistem gerektirir. Core loop — pet topla, evrimleştir, sınıfını geliştir, zindana gir, loot kazan — 5 sütun etrafında döner: Cömert Zindan, Evrimleştir Hepsini, Senin Tempon, Güç Hisset, Hep Bir Şey Var. MVP 20 sistemi kapsar (tek oyunculu core loop: 2 ırk, 4 sınıf, tur bazlı oto/manuel savaş, 15-20 pet B-A-S, kostüm, mağaza, 1 zindan bölgesi, 10 kat). Tier 2'de Arena PvP, ırk etkinlikleri, günlük görevler, tutorial ve bildirimler eklenir.

> **Pivottaki değişiklikler (2026-06-26):** Takım Kurma ve Hibrit Savaş Sistemi kaldırıldı (Savaş Sistemi + Oyuncu Sınıf Sistemi ile değiştirildi). Canavar Güçlendirme, Pet Evrim Sistemi ile birleştirildi. Kostüm/Elbise Sistemi ve Mağaza Sistemi eklendi. Irk Etkinlik Sistemi eklendi. Canavar Veritabanı → Pet/Canavar Veritabanı olarak güncellendi.

---

## Systems Enumeration

| # | System Name | Category | Priority | Status | Design Doc | Depends On |
|---|-------------|----------|----------|--------|------------|------------|
| 1 | Pet/Canavar Veritabanı | Core | MVP | Revision Needed | design/gdd/canavar-veritabani.md | — |
| 2 | Element Sistemi | Core | MVP | Approved | design/gdd/element-sistemi.md | — |
| 3 | Ekonomi / Kaynak Yönetimi | Economy | MVP | Approved | design/gdd/ekonomi.md | — |
| 4 | Kaydetme / Yükleme | Persistence | MVP | Approved | design/gdd/kaydetme-yukleme.md | — |
| 5 | UI Framework | UI | MVP | Approved | design/gdd/ui-framework.md | — |
| 6 | Sağlık / Can Sistemi | Core | MVP | Approved | design/gdd/saglik-can-sistemi.md | Pet/Canavar Veritabanı |
| 7 | Hasar Hesaplama | Core | MVP | Approved | design/gdd/hasar-hesaplama.md | Pet/Canavar Veritabanı, Element Sistemi |
| 8 | Pet Evrim Sistemi | Progression | MVP | Revision Needed | design/gdd/canavar-toplama-evrim.md | Pet/Canavar Veritabanı, Ekonomi |
| 9 | Ödül Ekranı / Loot Sistemi | Economy | MVP | Revision Needed | design/gdd/loot-odul-sistemi.md | Pet/Canavar Veritabanı, Ekonomi |
| 10 | Oyuncu Sınıf Sistemi | Gameplay | MVP | Designed | design/gdd/oyuncu-sinif-sistemi.md | Ekonomi, Kaydetme, Hasar Hesaplama, Sağlık/Can, Yetenek Sistemi |
| 11 | Düşman AI | Gameplay | MVP | Approved | design/gdd/dusuman-ai.md | Hasar Hesaplama, Sağlık, Element Sistemi |
| 12 | Savaş Sistemi | Gameplay | MVP | Revision Needed | design/gdd/hibrit-savas.md | Hasar Hesaplama, Sağlık, Düşman AI, Oyuncu Sınıf Sistemi, Yetenek Sistemi |
| 13 | Zindan Keşif Sistemi | Gameplay | MVP | Approved | design/gdd/zindan-kesif.md | Savaş Sistemi, Loot, Pet Evrim Sistemi |
| 14 | Otofarm / Idle Sistemi | Gameplay | MVP | Approved | design/gdd/otofarm-idle.md | Savaş Sistemi, Loot, Ekonomi, Kaydetme |
| 15 | Kostüm / Elbise Sistemi | Gameplay | MVP | Not Started | — | Pet/Canavar Veritabanı, Ekonomi |
| 16 | IAP + Reklam Sistemi | Economy | MVP | Not Started | — | Ekonomi, Kostüm/Elbise Sistemi, Pet/Canavar Veritabanı |
| 17 | Savaş UI | UI | MVP | Not Started | — | Savaş Sistemi, UI Framework |
| 18 | Koleksiyon / Envanter UI | UI | MVP | Not Started | — | Pet Evrim Sistemi, Kostüm/Elbise Sistemi, UI Framework |
| 19 | Zindan Harita UI | UI | MVP | Not Started | — | Zindan Keşif, UI Framework |
| 20 | Mağaza UI | UI | MVP | Not Started | — | IAP + Reklam Sistemi, UI Framework |
| 21 | Arena (Asenkron PvP) | Gameplay | Tier 2 | Not Started | — | Savaş Sistemi, Oyuncu Sınıf Sistemi, Ekonomi |
| 22 | Irk Etkinlik + Canlı Sıralama | Meta | Tier 2 | Not Started | — | Zindan Keşif, Ekonomi, Oyuncu Sınıf Sistemi |
| 23 | Tutorial / Onboarding | Meta | Tier 2 | Not Started | — | Tüm MVP sistemleri |
| 24 | Bildirim Sistemi | Meta | Tier 2 | Not Started | — | Otofarm, UI Framework |
| 25 | Yaşayan Kart Portreleri | UI | MVP | Not Started | — | Pet/Canavar Veritabanı, UI Framework |
| 26 | İntikam Sistemi | Gameplay | MVP | Not Started | — | Savaş Sistemi, Kaydetme/Yükleme |
| 27 | Aranıyor Tahtası | Meta | MVP | Not Started | — | Pet/Canavar Veritabanı, Zindan Keşif, UI Framework |
| 28 | Pet Sadakat Sistemi | Progression | Tier 2 | Not Started | — | Pet/Canavar Veritabanı, Savaş Sistemi, Kaydetme/Yükleme |
| 29 | Canavar Kütüphanesi (Codex) | Meta | Tier 2 | Not Started | — | Pet/Canavar Veritabanı, Zindan Keşif |
| 30 | Lokalizasyon Sistemi | Meta | MVP | Not Started | — | UI Framework, Tüm UI sistemleri |
| 31 | Yetenek Sistemi | Gameplay | MVP | Designed | design/gdd/yetenek-sistemi.md | Hasar Hesaplama, Sağlık/Can, Oyuncu Sınıf Sistemi, Pet/Canavar Veritabanı |

> **Deprecated (kaldırılan sistemler):**
> - ~~Takım Kurma~~ (design/gdd/takim-kurma.md) — Oyuncu Sınıf Sistemi + tek pet modeli ile değiştirildi
> - ~~Hibrit Savaş Sistemi~~ (design/gdd/hibrit-savas.md) — Savaş Sistemi (#12) olarak yeniden yazılacak; dosya revision needed
> - ~~Canavar Güçlendirme~~ (design/gdd/canavar-guclendirme.md) — Pet Evrim Sistemi (#8) ile birleştirildi

---

## Categories

| Category | Description |
|----------|-------------|
| **Core** | Tüm sistemlerin dayandığı temel veri ve hesaplama katmanları |
| **Gameplay** | Oyunun eğlencesini oluşturan mekanik sistemler |
| **Progression** | Oyuncunun zaman içinde nasıl güçlendiğini yöneten sistemler |
| **Economy** | Kaynak kazanım ve tüketim dengesi |
| **Persistence** | İlerleme kaydetme ve süreklilik |
| **UI** | Oyuncuya bilgi gösteren arayüz sistemleri |
| **Meta** | Core loop dışındaki destek sistemleri |

---

## Priority Tiers

| Tier | Definition | Target | Sistem Sayısı |
|------|------------|--------|---------------|
| **MVP** | Core loop'un çalışması için gerekli — "bu eğlenceli mi?" sorusunu cevaplar | 3-4 hafta | 18 |
| **Tier 2** | Retention ve rekabet katmanı — günlük döngü, PvP, onboarding | +4-6 hafta | 4 |

---

## Dependency Map

### Foundation Layer (bağımlılık yok)

1. **Pet/Canavar Veritabanı** — Tüm pet verisi buradan akar; kart şeması, F-D-C-B-A-S-SS tier (7 kademe), AI görsel referansları; 10+ sistem doğrudan bağımlı
2. **Element Sistemi** — 4 element matrisi, avantaj/dezavantaj tablosu; hasar ve sinerji temeli
3. **Ekonomi / Kaynak Yönetimi** — Altın, enerji, elmas, evrim materyali; loot ve mağazanın para birimi
4. **Kaydetme / Yükleme** — Serializasyon framework'ü; mobilde ilerleme koruması
5. **UI Framework** — Ekran yönetimi, modal, bildirim; tüm UI sistemlerinin altyapısı

### Core Layer (Foundation'a bağlı)

1. **Sağlık / Can Sistemi** — bağlı: Pet/Canavar Veritabanı
2. **Hasar Hesaplama** — bağlı: Pet/Canavar Veritabanı, Element Sistemi
3. **Pet Evrim Sistemi** — bağlı: Pet/Canavar Veritabanı, Ekonomi; F→D→C→B→A→S→SS (7 tier); item düşmüyor — EXP + milestone
4. **Ödül Ekranı / Loot Sistemi** — bağlı: Pet/Canavar Veritabanı, Ekonomi; savaş sonu ödül ekranı: EXP + düşen eşyalar (elbise/silah/takı/pet kartı); savaş sırasında ekrana saçılmıyor — ödül ekranında listeleniyor
5. **Oyuncu Sınıf Sistemi** — bağlı: Ekonomi, Kaydetme

### Feature Layer (Core'a bağlı)

1. **Düşman AI** — bağlı: Hasar Hesaplama, Sağlık, Element Sistemi
2. **Savaş Sistemi** — bağlı: Hasar Hesaplama, Sağlık, Düşman AI, Oyuncu Sınıf Sistemi; tur bazlı cooldown + oto-savaş toggle
3. **Zindan Keşif Sistemi** — bağlı: Savaş Sistemi, Loot, Pet Evrim Sistemi
4. **Otofarm / Idle Sistemi** — bağlı: Savaş Sistemi, Loot, Ekonomi, Kaydetme
5. **Kostüm / Elbise Sistemi** — bağlı: Pet/Canavar Veritabanı, Ekonomi; F-D-C-B-A-S-SS dereceli ekipman
6. **IAP + Reklam Sistemi** — bağlı: Ekonomi, Kostüm/Elbise Sistemi, Pet/Canavar Veritabanı; Google Play Billing (IAP) + AdMob (rewarded/interstitial); kozmetik + özel pet satışı + enerji/bonus reklam ödülü

### Presentation Layer (Feature'a bağlı)

1. **Savaş UI** — bağlı: Savaş Sistemi, UI Framework
2. **Koleksiyon / Envanter UI** — bağlı: Pet Evrim Sistemi, Kostüm/Elbise Sistemi, UI Framework
3. **Zindan Harita UI** — bağlı: Zindan Keşif, UI Framework
4. **Mağaza UI** — bağlı: Mağaza Sistemi, UI Framework

### Polish Layer (meta sistemler)

1. **Arena (Asenkron PvP)** — bağlı: Savaş Sistemi, Oyuncu Sınıf Sistemi, Ekonomi
2. **Irk Etkinlik Sistemi** — bağlı: Zindan Keşif, Ekonomi, Oyuncu Sınıf Sistemi
3. **Tutorial / Onboarding** — bağlı: Tüm MVP sistemleri
4. **Bildirim Sistemi** — bağlı: Otofarm, UI Framework

---

## Recommended Design Order

| Order | System | Priority | Layer | Agent(s) | Est. Effort |
|-------|--------|----------|-------|----------|-------------|
| 1 | Pet/Canavar Veritabanı *(revision)* | MVP | Foundation | game-designer, systems-designer | M |
| 2 | Element Sistemi | MVP | Foundation | game-designer, systems-designer | S |
| 3 | Ekonomi / Kaynak Yönetimi | MVP | Foundation | economy-designer | M |
| 4 | Sağlık / Can Sistemi | MVP | Core | game-designer | S |
| 5 | Hasar Hesaplama | MVP | Core | systems-designer | M |
| 6 | Pet Evrim Sistemi *(revision)* | MVP | Core | game-designer, systems-designer | M |
| 7 | Loot / Ödül Sistemi | MVP | Core | economy-designer | M |
| 8 | Oyuncu Sınıf Sistemi *(Designed — yan sınıf TBD)* | MVP | Core | game-designer | M |
| 9 | Düşman AI | MVP | Feature | ai-programmer, game-designer | M |
| 10 | Savaş Sistemi *(revision)* | MVP | Feature | game-designer, gameplay-programmer | L |
| 11 | Zindan Keşif Sistemi | MVP | Feature | game-designer, level-designer | L |
| 12 | Otofarm / Idle Sistemi | MVP | Feature | game-designer, gameplay-programmer | M |
| 13 | Kostüm / Elbise Sistemi *(yeni)* | MVP | Feature | game-designer, economy-designer | M |
| 14 | Mağaza Sistemi *(yeni)* | MVP | Feature | economy-designer | S |
| 15 | Kaydetme / Yükleme | MVP | Foundation | gameplay-programmer | S |
| 16 | UI Framework | MVP | Foundation | ui-programmer | S |
| 17 | Savaş UI | MVP | Presentation | ux-designer, ui-programmer | M |
| 18 | Koleksiyon / Envanter UI | MVP | Presentation | ux-designer, ui-programmer | M |
| 19 | Zindan Harita UI | MVP | Presentation | ux-designer, ui-programmer | S |
| 20 | Mağaza UI | MVP | Presentation | ux-designer, ui-programmer | S |
| 21 | Arena (Asenkron PvP) | Tier 2 | Polish | game-designer, network-programmer | L |
| 22 | Irk Etkinlik Sistemi *(yeni)* | Tier 2 | Polish | game-designer, economy-designer | M |
| 23 | Tutorial / Onboarding | Tier 2 | Polish | ux-designer, game-designer | M |
| 24 | Bildirim Sistemi | Tier 2 | Polish | ui-programmer | S |

**Effort**: S = 1 oturum, M = 2-3 oturum, L = 4+ oturum

---

## Circular Dependencies

Tespit edilmedi. Tüm bağımlılıklar tek yönlü akar.

---

## High-Risk Systems

| System | Risk Type | Risk Description | Mitigation |
|--------|-----------|-----------------|------------|
| Pet/Canavar Veritabanı | Scope | 7 tier (F-D-C-B-A-S-SS) × 50+ pet = 350+ AI görsel. Prototype'ta 3 tier ile başla. | Prototype: 10-15 pet, 3 tier (F/C/S). Her tier'ı onayladıkça ilerle. SS ve D/B görselleri MVP'ye ertele. |
| Savaş Sistemi | Design | Manuel/oto-savaş dengesi: oto çok güçlüyse kimse manuel oynamaz; cooldown tasarımı tatmin edici olmalı. | MVP prototipinde test. Oto-savaş ve manuel sonuçları karşılaştır, farkı kapat veya oyna. |
| Otofarm / Idle Sistemi | Technical | Arka plan hesaplaması mobilde pil/performans sorunları yaratabilir. iOS arka plan kısıtlamaları. | Arka plan hesaplamayı "geri dönüş anında simüle et" olarak uygula, gerçek arka plan işlem yerine. |
| Hasar Hesaplama | Design | Element çarpanları + sınıf dalı bonusları + pet tier farkı birlikte dengelenmesi zorlaşır. | Formülleri katmanlı tasarla (temel → element → sınıf bonus). Her katmanı bağımsız test et. |
| AI Görsel Tutarlılığı | Technical | Aynı petin B ve SS görselleri arasında tanınabilirlik sağlamak (aynı silüet, farklı detay) zor. | Art bible stil çapası + her pet için referans silüet. Tier geçişinde sadece detay değişmeli, form değil. |
| AI Video Entegrasyonu | Technical | VideoPlayer mobil formatları, alpha-channel sorunları, bellek yönetimi ve döngü kusurları. | MVP'de MP4 + black BG ile test başla. Tier 2'de alpha-channel WebM değerlendir. |

---

## Progress Tracker

| Metric | Count |
|--------|-------|
| Total systems identified | 31 |
| Design docs started | 14 |
| Design docs reviewed | 14 |
| Design docs approved | 9 (pivot sonrası re-review gerekli olanlar hariç) |
| MVP systems approved | 9/21 |
| MVP systems designed (awaiting review) | 1 (Yetenek Sistemi) |
| MVP systems revision needed | 3 (Pet Veritabanı, Pet Evrim, Savaş Sistemi) |
| MVP systems not started | 4 (Kostüm/Elbise, Mağaza, UI sistemleri) |
| Tier 2 systems designed | 0/4 |

> **Not:** Element Sistemi, Ekonomi, Kaydetme/Yükleme, Sağlık, Hasar Hesaplama, Loot, Düşman AI, Zindan Keşif, Otofarm ve UI Framework approved — pivot bu sistemleri etkilemedi.

---

## Next Steps

**Prototype yolu (önerilen — 2-3 hafta → Google Play soft launch):**
- [ ] `/prototype tur-bazli-savas` — Temel savaş mekanik prototipi (1 sınıf, 5 pet F/C/S, 5 kat, EXP sistemi, temel IAP stub)
- [ ] Google Play internal test → closed test → soft launch
- [ ] Prototype sonrası: `/brainstorm` veya direkt MVP'ye geç (veriye göre karar)

**Design revision gerekli (prototype sonrası veya paralel):**
- [ ] `/design-review canavar-veritabani` — F-D-C-B-A-S-SS tier şeması, AI görsel referansları
- [ ] `/design-review canavar-toplama-evrim` — Pet Evrim: F→SS kuralları, EXP gereksinimleri, SS ulaşamayan petler
- [ ] `/design-review hibrit-savas` — Savaş Sistemi: tur bazlı, oto toggle, EXP ekranı, item düşmüyor
- [ ] `/design-review loot-odul-sistemi` — Ödül ekranı: EXP + eşya drop tablosu (elbise/silah/takı/pet kartı); savaş sırasında değil savaş sonunda gösterim
- [x] `/design-system oyuncu-sinif-sistemi` — 4 sınıf × 2 dal (yan sınıf içerikleri TBD)
- [ ] `/design-system kostum-elbise-sistemi` — F-D-C-B-A-S-SS dereceli ekipman
- [ ] `/design-system iap-reklam-sistemi` — Google Play IAP + AdMob entegrasyonu, rewarded/interstitial kuralları
- [ ] `/gate-check pre-production` tam MVP GDD'leri tamamlandığında
