# Design Review Log: Canavar Veritabanı (Monster Database)

## Review — 2026-06-24 — Verdict: MAJOR REVISION NEEDED
Scope signal: L (Large)
Specialists: game-designer, systems-designer, economy-designer, qa-lead, creative-director (senior synthesis)
Blocking items: 5 | Recommended: 6
Summary: Matematiksel iskelet sağlam (systems-designer: dejenerasyon yok, formüller doğru). Ancak temel tasarım kararı — rarite+arketip başına özdeş statlar — "Topla Hepsini" ve "Güç Hisset" pillar'larını aktif olarak çökertiyor. 5 blocker: (1) tür-başına mekanik farklılaştırıcı yok (aynı rarite+arketip = mekanik ikiz), (2) Common/Uncommon yapısal çöp (%94 düşüş değersiz, 2-aşama evrim sınırı), (3) cross-GDD drop oranı tutarsızlığı (Uncommon %29.4 vs %25, Legendary %0.6 vs %0.3), (4) Formül 1 HP=31 vs floor=30 çelişkisi + registry range hataları, (5) 4 AC test edilemez + 7 AC kategorisi eksik. Creative-director sentezi: kimlik düzeyinde revizyon gerekli — matematik doğru ama yanlış şeyi (özdeşliği) hesaplıyor.
Prior verdict resolved: First review

## Review — 2026-06-25 (Re-review) — Verdict: NEEDS REVISION
Scope signal: M
Specialists: None (lean mode — single-session analysis)
Blocking items: 2 | Recommended: 4
Summary: Önceki MAJOR REVISION'dan 3/5 blocker çözülmüş (cross-GDD tutarsızlık, formül çelişkisi, AC test edilebilirliği). Kalan 2 blocker: (1) mekanik farklılaştırıcı tanımsız — Kural 7 per-monster unique yetenekler olarak netleştirildi, (2) edge case AC kapsamı eksik — 6 yeni AC eklendi (12-17). Ek düzeltmeler: Common/Uncommon gacha fodder pattern explicit savunması, Formül 1 range 15-79→15-78, rounding rule formülleştirildi, evrim stat dağılımı açıklandı.

### Uygulanan Revizyonlar
| # | Blocker/Issue | Düzeltme |
|---|-------------|----------|
| B1 | Mekanik farklılaştırıcı yok | Kural 7: per-monster unique yetenekler netleştirildi |
| B2 | Edge case AC kapsamı eksik | 6 yeni AC (12-17) eklendi |
| R1 | Common/Uncommon viability | Kural 4'e gacha fodder tasarım notu |
| R2 | Formül 1 range | 15-79 → 15-78 |
| R3 | Rounding rule | Explicit formül eklendi |
| R4 | Evrim dağılımı | "Aynı arketip yüzdeleri" notu |

Prior verdict resolved: 3/5 tamamen, 2/5 bu turda düzeltildi
