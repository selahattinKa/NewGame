# Loot / Ödül Sistemi — Review Log

## Review — 2026-06-25 — Verdict: NEEDS REVISION
Scope signal: L
Specialists: game-designer, systems-designer, economy-designer, ux-designer, qa-lead, creative-director (synthesis)
Blocking items: 7 (distilled from 37 raw) | Recommended: 10+ | Advisory: 48

Summary: Tasarım konsept olarak sağlam ama "her zindan cömert, her sandık heyecanlı" fantezisini edge case'lerde ihlal ediyor. 7 kritik blocker tespit edildi: (1) Rare pity counter'a cap yok — Epic/Legendary dilüsyonu, (2) Hard pity yok — sınırsız kuru dizi mümkün, (3) Cömert Zindan garantisi sadece Kat 1'de aktif, (4) Late-game ekonomi dengesizliği (lineer gelir vs üstel maliyet), (5) İdle efficiency parametreleri karışık (%50 altın vs %25 loot), (6) AC'ler test edilemez (olasılık bazlı, kapsam boşlukları), (7) İdle dönüş ekranı tanımsız. Tüm blocker'lar revize edildi, re-review gerekli.

### Uygulanan Revizyonlar
| # | Blocker | Düzeltme |
|---|---------|----------|
| 1 | Rare pity unbounded | rare_pity_cap=20 eklendi (Kural 10b, Formül 4) |
| 2 | Hard pity yok | monster_hard_pity=10 kat garanti Common (Kural 10a, Formül 3) |
| 3 | Cömert Zindan sadece Kat 1 | Kat-ölçekli: floor_gold × 0.80 (Kural 11, Formül 7) |
| 4 | Late-game ekonomi | Open Questions #7-10'a eklendi, Formül 9 düzeltildi |
| 5 | Idle efficiency karışıklığı | Formül 8 parametreleri ayrıldı, idle rate cap eklendi |
| 6 | AC'ler test edilemez | 19→28 AC, deterministik yeniden yazıldı, 9 yeni AC |
| 7 | Idle dönüş ekranı | Open Questions #11'e eklendi |

### Ek Düzeltmeler
- Rapor sıralaması: common→rare artan heyecan deseni (Kural 12, UI Requirements)
- Skip/hızlandırma seçeneği eklendi (UI Requirements)
- Renk-bağımsız nadirlik gösterimi eklendi (UI Requirements)
- Formül 3 olasılık hesabı düzeltildi (4.7% → ~3.2%)
- Boss canavar rulosu state machine'e eklendi
- Edge Cases güncellendi (hard pity, rare pity cap)
- Tuning Knobs: monster_hard_pity, rare_pity_cap, comert_floor_ratio eklendi

Prior verdict resolved: First review

## Review — 2026-06-25 (Re-review) — Verdict: NEEDS REVISION
Scope signal: L
Specialists: game-designer, economy-designer, systems-designer, ux-designer, qa-lead, creative-director (synthesis)
Blocking items: 6 | Recommended: 8
Summary: Önceki 7 blocker'dan 5'i tamamen çözülmüş, 2'si teknik olarak düzeltilmiş ama ruhuna göre yetersiz. Yeni bulguların ana teması: Cömert Zindan pillar'ı mekanik olarak ölü (hiçbir koşulda tetiklenmiyor), GDD'ler arası tutarsızlıklar hâlâ açık, idle pity cap aktifle aynı seviyede (denge sorunu), formül implementasyon tuzakları (total_weight sabit, idle pity birimi belirsiz), ve 5 eksik + 2 belirsiz AC. Tüm blocker'lar revize edildi, re-review gerekli.

### Uygulanan Revizyonlar
| # | Blocker | Düzeltme |
|---|---------|----------|
| 1 | Cömert Zindan ölü mekanik | Kural 3a eklendi: ilk 3 katta garanti Common canavar. first_floors_guaranteed_monster=3. |
| 2 | GDD'ler arası tutarsızlık | Single source of truth notu. İlk temizleme elması MVP sabit 5. Cross-GDD güncelleme gereksinimleri belirtildi. |
| 3 | Formül 2 total_weight sabit | Aralık 1.70→1.70-2.10, "rare pity aktifken dinamik" notu. |
| 4 | İdle pity increment birimi | "Her idle simüle edilen kat başına" olarak netleştirildi. |
| 5 | İdle→aktif pity izolasyonu | Tam izolasyon kuralı yazıldı. Aktif canavar idle pity'yi sıfırlamaz. |
| 5b | İdle pity cap denge | idle_pity_cap %45→%30. Formüller, AC'ler, tuning knobs güncellendi. |
| 6 | Eksik/belirsiz AC'ler | 7 yeni AC (29-35) eklendi: garanti canavar, pity reset, rare pity reset, idle/rare izolasyon, evrim zindanı, boss katı istisnası, idle→aktif izolasyon. 6 mevcut AC güncellendi. Toplam 28→35. |

### Ek Düzeltmeler
- Formül 9 canavar beklentisi ~1.9→~4.45 (ilk 3 kat garanti ile)
- First-clear elmas toplamı ~200→~140 (sabit 5/kat ile)
- AC #16 satış fiyatı netleştirildi (Ekonomi GDD referansı)

Prior verdict resolved: 5/7 tamamen, 2/7 teknik düzeltme → bu turda köklü çözüm uygulandı

## Review — 2026-06-25 (Re-review #2) — Verdict: APPROVED
Scope signal: L
Specialists: None (lean mode — single-session analysis)
Blocking items: 0 | Recommended: 3
Summary: Tüm 13 önceki blocker (7 ilk review + 6 ikinci review) çözülmüş. Formüller matematiksel olarak doğrulanmış (9 formül, tüm sınır değerleri kontrol edildi). 35 deterministik AC mevcut. Pity sistemleri (soft, hard, rare, idle) iyi tanımlanmış ve izole. Cross-GDD tutarsızlıklar (Otofarm nadirlik değerleri, Güçlendirme evrim oranı) bu GDD'de doğru — diğer GDD'lerde güncellenmeli. 3 recommended revision: envanter dolu edge case netliği, çoklu bekleme canavarı kaskad davranışı, cross-GDD güncelleme takibi.
Prior verdict resolved: Yes — 13/13 blocker resolved across two revision rounds
