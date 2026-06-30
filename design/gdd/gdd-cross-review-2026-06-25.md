# Cross-GDD Review Report

> **Date**: 2026-06-25
> **GDDs Reviewed**: 13
> **Verdict**: CONCERNS (blocking yok, 10 warning)

## Systems Covered

Canavar Veritabanı, Element Sistemi, Ekonomi, Sağlık/Can, Hasar Hesaplama, Canavar Güçlendirme, Loot/Ödül, Takım Kurma, Düşman AI, Hibrit Savaş, Canavar Toplama/Evrim, Zindan Keşif, Otofarm/Idle

---

## Consistency Issues

### Resolved This Session

- ~~RC-1: Loot GDD satır 459 eski çıkış modeli~~ — Düzeltildi: kaybetme/çekilme → loot yok + enerji harcanmaz.

### Warnings

- **TK-1**: `difficulty_multiplier` güvenli aralıkları tutarsız — Düşman AI (0.8–2.0), Zindan Keşif (0.5–2.0), Ekonomi (0.8–3.0). Tek sahip belirlenmeli.
- **TK-2**: `energy_per_floor` çift sahiplik — Ekonomi'deki girdi "referans (kaynak: Zindan Keşif)" olarak işaretlenmeli.
- **SR-1**: Bayat "provisional/henüz yazılmadı" etiketleri 5 GDD'de 9 yerde — toplu temizlik gerekli.

---

## Game Design Issues

### Warnings

- **W-1**: Tek-bölge element baskınlığı — Bölge elementine avantajlı element strict-dominant. Tier 2'de çözülür.
- **W-2**: Otofarm sıfır-efor dikkat-başına-altın oranı — Prototipte A/B test şart.
- **W-3**: Elmas faucet'i first-clear sonrası kuruyor — MVP için kabul edilebilir, Tier 2 kaynaklar eklenmeli.
- **W-4**: Evrim taşı element kıtlığı (tek bölge) — Tier 2'de çözülür.
- **W-5**: Zorluk eğrisi ıraksaması — Güç fantezisi için kasıtlı; Challenge sütunu MVP'de zayıf.
- **W-6**: "Boss" terminolojisi çelişkisi — Zindan Keşif "boss" vs Düşman AI "mini-boss" (Kat 5). Hizalanmalı.
- **W-7**: 24h idle + dolu envanter = Rare auto-sell UX riski — Envanter uyarısı veya Rare+ buffer önceliği önerilir.

---

## Cross-System Scenario Issues

Scenarios walked: 3 (İlk Boss Savaşı Kat 5, Komutan Element Sinerji, 24h İdle Dönüş)

- ℹ️ Pasif savaş XP'si atıl (~%10-15 katkı)
- ℹ️ Mono-element vs esnek takım trade-off — sağlıklı emergent derinlik
- ℹ️ team_power snapshot sürprizi — gaming önleme ama farkındalık gerektirir

---

## Clean Areas

- ✅ İlerleme döngüsü: Tek net primary loop (Zindan Keşif)
- ✅ Dikkat bütçesi: 2 aktif karar — casual-friendly
- ✅ Sütun hizalaması: 13/13 hizalı, anti-sütun ihlali yok
- ✅ Oyuncu fantezisi: Tek tutarlı kimlik
- ✅ Çarpan yığma sırası: doğrulandı
- ✅ Kayıp-cezasızlığı: 3 GDD uyumlu
- ✅ Bağımlılık çift yönlülüğü: asimetri yok

---

## GDDs Flagged for Revision

| GDD | Reason | Type | Priority |
|-----|--------|------|----------|
| 5 GDD (9 yer) | Bayat "provisional" etiketleri | Consistency | Warning |
| zindan-kesif + dusuman-ai | Boss terminolojisi | Design Theory | Warning |

---

## Verdict: CONCERNS

Blocking sorun yok. Mimari başlamak için engel yok. Warning'lerin çoğu MVP tek-bölge doğasından kaynaklanıyor (W-1, W-3, W-4) ve Tier 2'de doğal çözülüyor. W-2 ve W-7 prototip validasyonu gerektiriyor. W-6 hızlı terminoloji düzeltmesi.
