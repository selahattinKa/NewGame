# Review Log: Level / Deneyim Sistemi

## Review — 2026-07-01 (1. tur) — Verdict: NEEDS REVISION
Scope signal: L (cross-doc blocking chain elevates coordination risk beyond typical L)
Specialists: game-designer, systems-designer, economy-designer, qa-lead, creative-director
Blocking items: 6 | Recommended: 9
Summary: Formül 2'nin (XP→altın taşması) tek uygulama sınırı yoktu — tavandaki bir SS pete Dev iksir uygulanınca 20.000 altın üretebiliyordu. Ayrıca 3 kritik kural (tier-cap hard-stop, Kural 8/5 önceliği, save/load bütünlüğü) yalnızca düz yazıyla anlatılmıştı, kayıt şeması eksikti, registry senkron değildi, ve AC #12 kendi kendine "çalıştırılamaz" diyordu. Implementasyon ayrıca 3 harici GDD'nin (canavar-toplama-evrim.md, canavar-veritabani.md, canavar-guclendirme.md) revizyonuna bağlıydı.
Prior verdict resolved: First review.

## Revision — 2026-07-01 (kullanıcı onayıyla, 1. tur sonrası)
6 blocker çözüldü: SS Lv40 pete iksir hedef seçimi engellendi (Formül 2'ye potion XP'si giremez oldu), 3 kural pseudocode'a döküldü, kaydetme-yukleme.md güncellendi (player_level/xp, banked_xp, lifetime_pet_level), registry'de potion_lock_threshold_ratio → overfill_cap_ratio düzeltildi, AC #12 → 12a (çalıştırılabilir) + 12b (blocked) olarak bölündü. Ek: evrim "düşüş hissi" için kalıcı lifetime_pet_level ("Toplam Seviye") sayacı eklendi.

## Review — 2026-07-01 (2. tur, yeniden inceleme) — Verdict: NEEDS REVISION (küçük)
Scope signal: S (kalan iş için — GDD'nin genel kapsamı L olarak duruyor)
Specialists: game-designer, systems-designer, economy-designer, qa-lead, creative-director
Blocking items: 2 (YENİ) | Recommended: 6
Summary: 1. turun 6 blocker'ı bağımsız uzmanlarca doğrulandı — 4'ü tam çözülmüş bulundu (iksir→altın açığı 3 katmanlı korumayla kapanmış, tier-cap hard-stop doğru, kayıt şeması + registry doğru, AC12a/12b dürüst). Ama Kural 8/5 önceliği pseudocode'u kendi içinde YENİ bir blocker doğurdu: `pet.banked_progress` tanımsız bir terimdi, `banked_xp` ile karışıp Kural 8'in %50 sınırını sessizce bozabilirdi (systems-designer + economy-designer bağımsız yakaladı). Ayrıca save/load atomikliği kuralının hiç AC'si olmadığı bulundu (qa-lead). game-designer, oyuncu Lv30 sonrası büyüme eksikliğini hâlâ Blocking görüyor; creative-director bunu roadmap kalemi (OQ#8) olarak tutmaya devam ediyor — çözülmemiş disagreement, kayıtlı.
Prior verdict resolved: Yes — 1. turun 6 blocker'ı doğrulandı (4 tam, 2'si yeni blocker/gap açığa çıkardı).

## Revision — 2026-07-01 (kullanıcı onayıyla, 2. tur sonrası)
2 yeni blocker çözüldü: `pet.banked_progress` → `XPInvestedThisTier(pet)` olarak türetilmiş değer şeklinde tanımlandı; AC #29 eklendi (multi-level-up zincirinin save/load sonrası hep son durumda olduğunu test ediyor). 5 önerilen madde de çözüldü: Open Question #8 pet SS40 dead-end'i de kapsayacak şekilde genişletildi, SS40 petlere "MAX" rozeti eklendi, overflow-stacking riski Open Question #10 olarak taşındı (görünürlük), materyal-kontrol yarışı için AC12b desenini takip eden bir deferral notu eklendi, `remaining_tier_xp≈0` dejenere onay-dialogu durumu için Kural 8 + UI'a istisna eklendi. Nice-to-have: SS geçiş anı banked_xp sıkışması da düzeltildi.

**Sonuç**: Bu GDD'nin kendi kapsamındaki TÜM blocker'lar kapandı (2 tur bağımsız uzman incelemesi + creative-director sentezi ile doğrulandı). systems-index.md durumu "Revised — Implementation Blocked" olarak güncellendi — tam Approved için hâlâ 3 harici dosyanın (`canavar-toplama-evrim.md`, `canavar-veritabani.md`, `canavar-guclendirme.md`) revizyonu gerekiyor (Open Questions #1, #6, #7).
