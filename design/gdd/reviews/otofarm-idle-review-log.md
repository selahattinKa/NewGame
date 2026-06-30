# Design Review Log: Otofarm / Idle Sistemi

## Review — 2026-06-24 — Verdict: MAJOR REVISION NEEDED → Revisions Applied
Scope signal: L (Large)
Specialists: game-designer, systems-designer, economy-designer, qa-lead, gameplay-programmer, creative-director (senior synthesis)
Blocking items: 8 | Recommended: 10
Summary: Yapısal olarak tam (8/8 bölüm) ama ciddi entegrasyon boşlukları tespit edildi. En kritik 3 sorun: (1) teamPower → gold rate fonksiyonu hiçbir GDD'de tanımlı değildi, (2) ödül modeli zaman-tabanlı/kat-tabanlı çelişkisi, (3) pity sistemi idle etkileşimi tanımsızdı. Ayrıca idle_loot_efficiency aralık çelişkisi, eksik malzeme formülü, edge case/formül tutarsızlığı ve test edilemez AC'ler. Creative-director sentezi: vizyon sağlam, sorun GDD'ler arası entegrasyon seçiminde. Tüm 8 blocker aynı oturumda revize edildi — otomatik aktivasyon, stokastik kat-tabanlı loot, azalan getiri kademeleri, idle-pity sistemi, malzeme formülü eklendi. 20 AC'ye genişletildi. 4 cross-GDD güncelleme gerekiyor (Ekonomi, Loot, Kaydetme/Yükleme, Registry).
Prior verdict resolved: First review
Re-review pending: Evet — temiz context ile tam re-review önerildi.

## Review — 2026-06-24 — Verdict: NEEDS REVISION → Revisions Applied
Scope signal: L (Large)
Specialists: game-designer, systems-designer, economy-designer, qa-lead, creative-director (senior synthesis)
Blocking items: 3 kategoride (formül sahiplik çelişkileri, Player Fantasy/mekanik uyumsuzluğu, AC testability) | Recommended: 14
Summary: Re-review — önceki 8 blocker tasarım seviyesinde çözülmüş. Kalan sorunlar propagasyon borcu: (1) Cross-GDD formül çelişkileri (idle_gold 3 kaynakta farklı, teamPower→gold sahipsiz, idle-pity Loot GDD'de yok), (2) Player Fantasy Common/Uncommon tavanını aşan vaat, (3) 5 AC deterministik pass/fail üretemez. Creative-director sentezi: Common/Uncommon tavanı korunsun (game-designer karşı çıktı), Player Fantasy ifadesi düzeltilsin. Kullanıcı kararı: Rare+ izin verildi (game-designer önerisi kabul), pity oturumlar arası kalıcı, M_floor alt sınırı 3.0. Tüm 3 blocker + 6 recommended aynı oturumda revize edildi — 5 dosya güncellendi (otofarm-idle, ekonomi, loot, kaydetme-yukleme, registry). AC sayısı 20→25'e genişletildi. Cross-GDD güncellemeler tamamlandı.
Prior verdict resolved: Evet — önceki re-review talebi karşılandı.
Re-review pending: Evet — temiz context ile son kontrol önerildi (oturum %65+ context kullandı).

## Review — 2026-06-24 — Verdict: NEEDS REVISION → Revisions Applied
Scope signal: L (Large)
Specialists: game-designer, systems-designer, economy-designer, qa-lead, creative-director (senior synthesis)
Blocking items: 10 | Recommended: 10
Summary: 3. review — önceki 2 round'daki revizyonlar uygulanmış, yapı sağlam (8/8). Temel sorun: matematik fanteziye hizmet etmiyor. (1) Geri dönüş ekranı "hediye kutusu" fantezisi kısa oturumlarda boş — %3.75/kat ile 2-3 saatte 0 canavar bekleniyor. (2) Pity sistemi hem istismar edilebilir (mikro-oturum exploit) hem etkisiz (0.03 increment idle'ın %3.75 base rate'i için yetersiz). (3) Cross-GDD çelişkileri: Loot GDD "yalnızca Common/Uncommon" diyor, Otofarm tüm nadirliklere izin veriyor; Loot GDD idle gold formülü eski (tiered yok). (4) Boss katları Cömert Zindan pillarıyla çelişiyor. (5) Registry output_range tutarsız. (6) System.Random cross-platform determinizm sorunu. (7-9) AC testability: golden-file yok, chi-squared flaky, boss AC eksik.
Çözümler: Rule 11 (idle minimum hasat garantisi: ≥60dk → 1 Common canavar + 1 malzeme), Rule 12 (mütevazı idle boss bonus: 1.5x altın + garantili Common malzeme), idle_pity_increment 0.03→0.06, min_offline_for_pity=30dk, pending_report zorunlu persist, AC 4 golden-file, AC 5 split (5a deterministik + 5b advisory), 5 yeni AC (26-30), 6 yeni tuning knob. Loot GDD Formula 8 + Formül 3b + AC 14 güncellendi. Registry output_range [1,480] güncellendi. AC sayısı 25→30'a genişletildi.
Dosyalar: otofarm-idle.md, loot-odul-sistemi.md, entities.yaml
Prior verdict resolved: Evet — önceki 2 round'un re-review talebi karşılandı.
Re-review pending: Evet — temiz context ile lean re-review önerildi.

## Review — 2026-06-24 — Verdict: APPROVED
Scope signal: L (Large)
Specialists: — (lean mode, no specialist agents)
Blocking items: 0 | Recommended: 3
Summary: 4. review (lean re-review). 3 round yoğun revizyon sonrası tüm önceki blocker'lar çözülmüş. 8/8 bölüm tam, formüller dahili tutarlı, 30 AC test edilebilir, 7 bağımlılık GDD'si mevcut, cross-GDD tutarlılık doğrulandı. Kalan 3 recommended: (1) M_floor değişken tablosu min 2.0→3.0, (2) Registry'de idle_pity_increment girdisi eksik, (3) teamPower formül sahiplik notu güncellenmeli. Tümü non-blocking — aynı oturumda uygulandı.
Prior verdict resolved: Evet — önceki 3 review'ın tüm blocker'ları çözüldü.
