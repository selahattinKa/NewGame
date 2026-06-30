# Design Review Log: Ekonomi / Kaynak Yönetimi (Economy / Resource Management)

## Review — 2026-06-25 — Verdict: APPROVED (after revision)
Scope signal: L
Specialists: None (lean mode — single-session analysis)
Blocking items: 2 (resolved) | Recommended: 6 (3 resolved, 3 open)
Summary: Formül 1 output range 2x hatalıydı (~106.000 → düzeltme: ~51.500, formula doğru), Kural 7 Cömert Zindan garantisi undefined değişken kullanıyordu (gold_per_energy_ratio → düzeltme: iki katmanlı garanti, Loot GDD per-floor + Ekonomi session-level). Ek düzeltmeler: formül numaralama (iki Formül 4 → 4,5,6,7), base_boss_gold tanımsız → Loot GDD cross-ref, AC #2 yuvarlama (2.844 → 2.846 + floor()). Kalan 3 recommended: difficulty_multiplier ölçekleme fonksiyonu tanımsız, AC #13 imprecise, Formül 3 registry'de yok.
Prior verdict resolved: First review

### Uygulanan Revizyonlar
| # | Blocker/Issue | Düzeltme |
|---|-------------|----------|
| B1 | Formül 1 output range ~106.000 | → ~51.500 (floor(50×3.0×343)=51.450). floor() eklendi. Registry güncellendi. |
| B2 | Kural 7 gold_per_energy_ratio tanımsız | → İki katmanlı garanti: Katman 1 kat bazlı (Loot GDD), Katman 2 oturum bazlı (gold_per_energy_base=50). |
| R1 | Duplicate Formül 4 numarası | → 4,5,6,7 olarak düzeltildi |
| R3 | base_boss_gold tanımsız | → Loot GDD cross-ref: floor_gold × boss_gold_multiplier (3.0x) |
| R4 | AC #2 yuvarlama 2.844 | → floor(2.846,07) = 2.846 |
