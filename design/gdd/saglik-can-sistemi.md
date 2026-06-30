# Sağlık / Can Sistemi (Health System)

> **Status**: Designed
> **Author**: user + game-designer, systems-designer
> **Last Updated**: 2026-06-24
> **Implements Pillar**: Güç Hisset, Cömert Zindan

## Overview

**Sağlık / Can Sistemi**, savaştaki tüm birimlerin (oyuncu canavarları ve düşmanlar) dayanıklılığını yöneten HP (Hit Points) mekanizmasıdır. Her canavar, Canavar Veritabanı'ndaki arketip ve nadirlik kademesine göre belirlenen bir maksimum HP değeriyle savaşa girer. Hasar aldıkça HP azalır; sıfıra düştüğünde birim savaş dışı kalır ve o savaşta artık saldıramaz veya hasar alamaz. İyileşme mekanikleri (destek yeteneği, pasif rejenerasyon) HP'yi kısmi olarak geri kazandırır ancak asla max HP'yi aşamaz.

Oyuncu açısından can sistemi savaşın nabzıdır — HP barlarının azalması gerilim, iyileşme rahatlama, savaş dışı kalma ise stratejik kayıp yaratır. "Güç Hisset" sütunu gereği HP büyümesi seviye ve evrimle belirgin şekilde artar; oyuncu eski düşmanları daha az HP kaybederek ezmeli. "Cömert Zindan" sütunu gereği tüm canavarlar savaş sonunda tam HP ile başlar — kalıcı HP cezası yoktur.

MVP kapsamında temel HP havuzu (Canavar Veritabanı'ndan), hasar alma, savaş dışı kalma, savaş sonu tam iyileşme ve basit HP barı gösterimi yer alır. İyileşme yetenekleri Savaş Sistemi'nde, kalkan/bariyer mekanikleri ise Tier 2+'da tanımlanacaktır.

## Player Fantasy

Oyuncu can sistemiyle iki katmanlı bir deneyim yaşar. **Yüzeyde**, HP barları savaşın dramatik gerilimini yaratır — her darbe HP barını eritir, düşmana son darbeyi vururken kendi canavarının HP'sinin azalmasını izlemek "yetişecek mi?" heyecanı üretir. Bir canavarı kıl payı kaybetmek stratejik bir kayıptır; tüm takımı ayakta tutarak katı geçmek ise ustalık tatminidir.

**Çekirdek duygu**: Dayanıklılık ve kontrol. Oyuncu takımının ne kadar hasar absorbe edebildiğini görür ve bu dayanıklılık "güçlü bir ordum var" fantezisinin somut kanıtıdır. Tank arketipli bir canavarın HP barının neredeyse kıpırdamaması — "bu canavar yıkılmaz" hissini verir. Saldırgan arketipin hızla eriyen HP'si ise risk-ödül gerilimini yaratır.

**Güç büyümesi tatmini**: Seviye atladıkça ve evrimleştikçe HP barları belirgin şekilde uzar. Eskiden zorlandığın düşmanlar artık HP barını zar zor çizer — "ne kadar güçlendim" anı. Bu duygu "Güç Hisset" sütununa doğrudan bağlıdır.

**Negatif fantazi (kaçınılacak)**: Can kaybı asla kalıcı ceza hissi vermemeli. Savaş dışı kalan canavar bir sonraki savaşta tam canla döner. "Cömert Zindan" sütunu gereği oyuncu kaybettiğinde canavar ölmez, eşya kaybetmez — sadece o kattan loot alamaz. HP sıfıra düşmek "başarısızlık" değil, "bir sonraki sefere daha güçlü gelmelisin" mesajıdır.

**Otofarm perspektifi**: İdle modda HP yönetimi otomatiktir — oyuncu geri döndüğünde sonuçları görür, süreci değil. Aktif oyunda HP barlarının kıpırdamasını izlemek gerilim yaratır; idle'da bu gerilim yoktur ama güç büyümesinin sonuçları birikimde görünür.

**Pillar bağlantısı**: "Güç Hisset" — HP büyümesi gücün en görünür kanıtı. "Cömert Zindan" — can kaybı cezalandırıcı değil. "Senin Tempon" — otofarm'da otomatik, aktif oyunda izlenebilir.

## Detailed Design

### Core Rules

**Kural 1 — Maksimum HP Kaynağı**

Her canavarın maksimum HP değeri Canavar Veritabanı'ndaki stat dağılım formülünden gelir:

`max_hp = floor(total_stat_pool * archetype_hp_percentage)`

Bu değer Canavar Veritabanı GDD'sinde tanımlanmıştır (Kural 2-3). Sağlık/Can Sistemi bu değeri okur, üretmez.

| Arketip | HP% | Common Lv1 | Rare Lv1 | Legendary Lv1 | Legendary Form C |
|---------|-----|-----------|----------|---------------|-----------------|
| Saldırgan | 20% | 20 | 30 | 45 | 88 |
| Tank | 30% | 30 | 45 | 67 | 132 |
| Destekçi | 28% | 28 | 42 | 63 | 123 |
| Büyücü | 18% | 18 | 27 | 40 | 79 |

**Kural 2 — Anlık HP (Current HP)**

Her canavar instance'ı savaş boyunca bir `current_hp` değeri taşır:
- Savaş başlangıcında: `current_hp = max_hp`
- Hasar alınca: `current_hp = max(0, current_hp - incoming_damage)`
- İyileşince: `current_hp = min(max_hp, current_hp + heal_amount)`
- `current_hp` asla 0'ın altına veya `max_hp`'nin üstüne çıkamaz.

**Kural 3 — Hasar Alma**

Hasar Hesaplama sistemi bir final hasar değeri üretir ve bu sisteme `TakeDamage(target, amount)` aracılığıyla iletir. Bu sistem:
1. `current_hp`'den `amount` çıkarır
2. Sonucu 0'da clamp eder
3. `current_hp == 0` ise Savaş Dışı durumunu tetikler
4. HP değişimini Savaş UI'a bildirir (olay/event yayınlar)

Hasar hesaplaması (ATK, DEF, element çarpanı, kritik vb.) bu sistemin sorumluluğunda **değildir** — Hasar Hesaplama GDD'sinde tanımlanacaktır.

**Kural 4 — Savaş Dışı Kalma (Knockout)**

`current_hp == 0` olduğunda:
- Canavar savaş dışı kalır ve o savaşın geri kalanında aktif olamaz
- Saldıramaz, hasar alamaz, yetenek kullanamaz
- Savaş sırasında geri getirilemez — MVP'de diriltme yok
- HP barı "0" gösterir, canavar savaş alanında soluk/gri gösterilir
- Savaş dışı kalma olayı Savaş Sistemi'ne bildirilir (takım boyutu kontrolü)

**Kural 5 — İyileşme (Healing)**

İki iyileşme kaynağı mevcuttur:

| Kaynak | Oran | Tetikleyici | Kısıt |
|--------|------|-------------|-------|
| **Pasif Rejenerasyon** | max_hp'nin %2'si / savaş turu | Her turun başında otomatik | Sadece hayatta olan canavarlar |
| **Destekçi Yeteneği** | max_hp'nin %15-25'i (tek hedef) | Destekçi arketip yetenek kullanımı | Yetenek detayları Hibrit Savaş GDD'sinde |

- İyileşme `max_hp`'yi aşamaz — fazlası kaybolur (overheal yok)
- Savaş dışı canavarlar iyileştirilemez
- Pasif rejenerasyon **tüm** hayatta olan canavarlara uygulanır (arketip farkı yok)

**Kural 6 — Kat Arası ve Savaş Sonu İyileşme**

Bu GDD'de "savaş" bir kat karşılaşmasının tamamını (tüm dalgaları) ifade eder — tek bir dalgayı değil.

- **Dalgalar arası**: Bir kat birden fazla dalga içeriyorsa (normal kat: 2 dalga, boss katı: 3 dalga), dalgalar arasında HP korunur — iyileşme yok. Bu mekanik Zindan Keşif GDD'sinde tanımlanmıştır (Kural 3). Dalga 1'de alınan hasar Dalga 2'ye taşınır.
- **Kat sonu (savaş sonu)**: Bir kat tamamlandığında (tüm dalgalar temizlendiğinde) tüm canavarlar tam HP'ye döner (`current_hp = max_hp`). Savaş dışı kalmış canavarlar da dahil.
- Kalıcı HP hasarı **yoktur** — "Cömert Zindan" sütunu gereği her kat temiz başlangıç.

**Kural 7 — Seviye ve Evrim ile HP Büyümesi**

HP büyümesi Canavar Veritabanı ve Canavar Güçlendirme GDD'lerinin sorumluluğundadır. Bu sistem sadece güncel `max_hp` değerini okur. Büyüme eğrisi:
- Seviye atlama: stat havuzu büyüme oranına göre artar (Canavar Güçlendirme GDD'sinde tanımlanacak)
- Evrim: stat havuzu %40 artar (evolution_bonus = 0.40, Canavar Veritabanı Formül 2)

### States and Transitions

Her savaş birimi (canavar) aşağıdaki HP durumlarından birinde bulunur:

| Durum | Koşul | Tanım |
|-------|-------|-------|
| **Tam Can** | `current_hp == max_hp` | Hiç hasar almamış veya tam iyileşmiş |
| **Hasarlı** | `0 < current_hp < max_hp` | Hasar almış ama hayatta |
| **Kritik** | `current_hp <= max_hp * critical_threshold` | Can tehlikeli düzeyde düşük |
| **Savaş Dışı** | `current_hp == 0` | Bu savaşta artık aktif değil |

Geçişler:

```
Tam Can ──(hasar)──→ Hasarlı ──(hasar)──→ Kritik ──(hasar)──→ Savaş Dışı
   ↑                    ↑                    ↑
   └──(iyileşme)────────┴──(iyileşme)────────┘
                                              
Savaş Dışı ──(kat sonu)──→ Tam Can
```

- **Kritik → Tam Can**: Yeterli iyileşme ile doğrudan geçiş mümkün
- **Savaş Dışı → Tam Can**: Sadece savaş/kat sonu tam iyileşmede
- **Savaş Dışı → Hasarlı**: Mümkün değil (savaş içi diriltme yok)
- `critical_threshold` varsayılan değeri: %25 (Tuning Knobs'ta ayarlanabilir)
- Kritik durum mekanik bir fark yaratmaz — sadece görsel uyarı (HP barı kırmızıya döner, titrer)

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Canavar Veritabanı** | ← okur | max_hp (base stat) | `GetBaseStats(monsterId, level)` → {hp, atk, def, spd} — HP alanı kullanılır |
| **Canavar Güçlendirme** | ← okur | Güncel max_hp (seviye/evrim sonrası) | Güçlendirme sonrası max_hp güncellenir |
| **Hasar Hesaplama** | ← alır | Final hasar miktarı | `TakeDamage(targetId, amount)` — hasar bu sisteme uygulanır |
| **Savaş Sistemi** | → sağlar | Hayatta mı?, current_hp | `IsAlive(monsterId)` → bool, `GetCurrentHP(monsterId)` → int |
| **Savaş Sistemi** | ← alır | İyileşme miktarı | `Heal(targetId, amount)` — destekçi yeteneği iyileşmesi |
| **Element Sistemi** | dolaylı | Sinerji DEF bonusu → Hasar Hesaplama → azaltılmış hasar | Doğrudan arayüz yok — etki Hasar Hesaplama üzerinden |
| **Düşman AI** | → sağlar | HP oranı (hedef seçimi için) | `GetHPRatio(monsterId)` → float (0.0–1.0) |
| **Savaş UI** | → sağlar | current_hp, max_hp, durum | `OnHPChanged` event → {monsterId, current_hp, max_hp, state} |
| **Zindan Keşif** | ← tetikler | Kat sonu tam iyileşme | `FullHeal(teamId)` — tüm takımı tam HP'ye döndürür |

**Veri akışı özeti**: Canavar Veritabanı → max_hp sağlar. Hasar Hesaplama → hasarı gönderir. Bu sistem HP'yi izler ve durumu savaş/UI sistemlerine bildirir. Tek yönlü okuma + olay tabanlı bildirim.

## Formulas

### Formül 1: Pasif Rejenerasyon

`regen_amount = max(1, floor(max_hp * passive_regen_rate))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Maksimum HP | max_hp | int | 18–132 | Canavarın güncel max HP değeri |
| Pasif regen oranı | passive_regen_rate | float | 0.02 | Tur başına max HP yüzdesi |
| Rejenerasyon miktarı | regen_amount | int | 1–2 | Floor ile yuvarlanır, minimum 1 |

**Çıktı Aralığı**: 1 (Büyücü Common Lv1: floor(18 * 0.02) = 0 → min 1) ile 2 (Tank Legendary Form C: floor(132 * 0.02) = 2)

**Minimum Kısıtı**: Sıfır rejenerasyon anlamsızdır — her zaman en az 1 HP iyileşir.

**Örnek**: Rare Tank Lv1 (max_hp=45) → floor(45 * 0.02) = floor(0.9) = 0 → min 1 → **1 HP/tur**

**Not**: Bu oranlarla pasif regen çok yavaştır — savaşı tek başına kurtaramaz, sadece küçük chip hasarını telafi eder. Ana iyileşme kaynağı destekçi yeteneğidir.

### Formül 2: Destekçi İyileşme Miktarı

`heal_amount = max(1, floor(target_max_hp * healer_skill_rate))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Hedef max HP | target_max_hp | int | 18–132 | İyileştirilen canavarın max HP'si |
| İyileştirme oranı | healer_skill_rate | float | 0.15–0.25 | Yetenek seviyesine göre |
| İyileşme miktarı | heal_amount | int | 2–33 | Floor ile yuvarlanır, minimum 1 garantili |

**Çıktı Aralığı**: 2 (Büyücü Common Lv1, %15: floor(18 * 0.15) = 2, max(1,2) = 2) ile 33 (Tank Legendary Form C, %25: floor(132 * 0.25) = 33)

**Not**: `healer_skill_rate` yetenek seviyesine göre ölçeklenir — detayları Savaş Sistemi GDD'sinde tanımlanacak. Bu GDD sadece iyileşme uygulama mekaniklerini tanımlar.

**Örnek**: Rare Tank (max_hp=45), %20 iyileşme → floor(45 * 0.20) = floor(9.0) = **9 HP**

### Formül 3: HP Büyüme Eğrisi (Referans)

> Bu formüller Canavar Veritabanı GDD'sinde tanımlanmıştır — burada referans verilir.

**Evrim ile HP büyümesi**:
`evolved_max_hp = floor(evolved_stat_pool * archetype_hp_percentage)`
Kaynak: `design/gdd/canavar-veritabani.md` — Formül 2

**Seviye ile HP büyümesi**:
Canavar Güçlendirme GDD'sinde tanımlanacak. Bu GDD, güncel max_hp değerini Canavar Veritabanı'ndan alır.

### Formül 4: Efektif HP (Hasar Sonrası Kalan)

`effective_hp = max(0, current_hp - incoming_damage)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Anlık HP | current_hp | int | 0–132 | Hasar öncesi güncel HP |
| Gelen hasar | incoming_damage | int | 1–∞ | Hasar Hesaplama'dan gelen final hasar |
| Efektif HP | effective_hp | int | 0–132 | Hasar sonrası güncel HP |

**Çıktı Aralığı**: 0 (savaş dışı) ile max_hp (hasar absorbe eden savunma durumu)

**Overkill davranışı**: `incoming_damage > current_hp` durumunda fazla hasar kaybolur — başka birime aktarılmaz.

## Edge Cases

- **If `incoming_damage` 0 veya negatifse**: Hasar uygulanmaz, `current_hp` değişmez. Hata loglanır (Hasar Hesaplama'da bug olasılığı).

- **If `current_hp` zaten 0 iken tekrar hasar gelirse**: Hasar yok sayılır — savaş dışı canavar hasar alamaz. Overkill kaydedilmez.

- **If `heal_amount` 0 veya negatifse**: İyileşme uygulanmaz. Hata loglanır.

- **If iyileşme `max_hp`'yi aşarsa**: `current_hp = min(max_hp, current_hp + heal_amount)` — overheal yok, fazlası kaybolur.

- **If savaş dışı canavara iyileşme denenirse**: İyileşme reddedilir. MVP'de diriltme yok — savaş dışı canavar savaş sonuna kadar iyileştirilemez.

- **If takımdaki tüm canavarlar savaş dışı kalırsa (TPK — Total Party Kill)**: Savaş kaybedilir. Savaş Sistemi kaybetme akışını tetikler. "Cömert Zindan" gereği eşya kaybı yok — sadece o kattan loot kazanılamaz.

- **If `max_hp` savaş sırasında değişirse (buff/debuff)**: `current_hp`, yeni `max_hp`'den yüksekse yeni max'a clamp edilir. `max_hp` artarsa `current_hp` **artmaz** — sadece tavan yükselir. Bu kural ileride buff/debuff mekanikleri eklendiğinde geçerli olacaktır (MVP'de savaş içi max_hp değişimi yok).

- **If pasif rejenerasyon ile `current_hp` tam HP'ye ulaşırsa**: Rejenerasyon o tur atlanır — gereksiz hesaplama yapılmaz. Durum "Tam Can"a geçer.

- **If aynı anda birden fazla hasar kaynağı gelirse (eş zamanlı saldırı)**: Hasar sıralı uygulanır — ilk hasar `current_hp`'yi 0'a düşürürse sonraki hasarlar yok sayılır (çünkü canavar artık savaş dışı).

- **If kritik eşiği (%25) ile savaş dışı kalma (0) arasında rejenerasyon loop'u oluşursa**: Oluşamaz — rejenerasyon ve hasar ayrı tur fazlarında uygulanır (Savaş Sistemi tur sırasını belirler).

- **If `max_hp` 0 veya negatifse (veri hatası)**: `max_hp = 1` olarak clamp edilir. Canavar savaşa girer ama neredeyse anında savaş dışı kalır. Hata loglanır.

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Canavar Veritabanı** | Sert | `GetBaseStats(monsterId, level)` → {hp, atk, def, spd} — HP alanı | Olmadan max_hp belirlenemez |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Hasar Hesaplama** | Sert | `TakeDamage(targetId, amount)` → HP azalır | Olmadan hasar uygulanamaz |
| **Savaş Sistemi** | Sert | `IsAlive(monsterId)`, `GetCurrentHP(monsterId)`, `Heal(targetId, amount)` | Olmadan savaş döngüsü çalışamaz |
| **Düşman AI** | Yumuşak | `GetHPRatio(monsterId)` → float | AI hedef seçimini geliştirir; olmadan rastgele seçer |
| **Savaş UI** | Yumuşak | `OnHPChanged` event → {monsterId, current_hp, max_hp, state} | Olmadan HP barı gösterilemez |
| **Zindan Keşif** | Yumuşak | `FullHeal(teamId)` — kat sonu tam iyileşme tetikleyicisi | Olmadan kat arası iyileşme yok |
| **Canavar Güçlendirme** | dolaylı | max_hp güncelleme (seviye/evrim sonrası) | Güçlendirme olduğunda max_hp yenilenir |

**Bağımlılık doğası**: Canavar Veritabanı'ndan max_hp alır (tek upstream). Hasar ve iyileşme komutlarını alır, HP durumunu aşağıya bildirir. Çift yönlü bağımlılık yok.

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `passive_regen_rate` | 0.02 | 0.01–0.05 | Regen savaşı trivialleştirir — hasar etkisi hissedilmez | Regen anlamsız — %1'de zaten minimum 1 HP |
| `healer_skill_rate_min` | 0.15 | 0.10–0.20 | Lv1 destekçi bile çok güçlü → tank gereksizleşir | İyileşme anlamsız → destekçi arketip değersiz |
| `healer_skill_rate_max` | 0.25 | 0.20–0.35 | Max seviye destekçi ölümsüzlük sağlar | Max seviye destekçi yetersiz → end-game'de anlamsız |
| `critical_threshold` | 0.25 | 0.15–0.35 | Kritik uyarısı çok erken → gerilim azalır | Çok geç → oyuncu uyarılmadan canavar düşer |
| `ko_visual_fade` | 0.5 | 0.3–1.0 | Savaş dışı kalma çok yavaş → gecikme hissi | Çok hızlı → oyuncu ne olduğunu fark etmez |

**Etkileşim Uyarıları**:
- `passive_regen_rate` × savaş turu süresi (Hibrit Savaş'ta tanımlanacak) birlikte bir savaşta toplam iyileşme miktarını belirler. Regen oranını artırırken tur sayısını da göz önünde bulundur.
- `healer_skill_rate_max` × Tank arketip HP'si (max_hp=132 at Legendary C) birlikte tek bir iyileşmenin etkisini belirler (132 * 0.25 = 33 HP). Bu, gelen hasara göre dengelenmelidir (Hasar Hesaplama GDD'sinde).
- Canavar Veritabanı'ndaki arketip HP yüzdeleri bu sistemdeki tüm HP değerlerini doğrudan etkiler — HP% değişirse bu tablodaki tüm "güvenli aralık" değerleri yeniden değerlendirilmelidir.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Süre | Öncelik |
|------|-----|------|---------|
| Hasar alınca | HP barı kırmızıya flash + hasar sayısı yukarı uçuşur | 0.3s flash | MVP |
| Kritik HP eşiği (%25) | HP barı kırmızıya döner + yavaş titreme efekti | Sürekli (kritik durumda kalırken) | MVP |
| Savaş dışı kalma (HP=0) | Canavar sprite'ı soluklaşır (opacity %30) + kısa "düşme" animasyonu | 0.5s fade | MVP |
| İyileşme (heal) | Hedef canavarın üstünde yeşil "+X HP" yazısı + yeşil parçacık efekti | 0.5s | MVP |
| Pasif rejenerasyon | Hafif yeşil parıltı (iyileşmeden daha az belirgin) | 0.3s | Nice-to-have |
| Tam can iyileşme (kat sonu) | Tüm HP barları dolma animasyonu (soldan sağa) | 0.5s | Nice-to-have |

### HP Barı Görsel Tasarımı

| Durum | HP Barı Rengi | Ek Efekt |
|-------|--------------|----------|
| Tam Can (100%) | Parlak yeşil | — |
| Hasarlı (26-99%) | Yeşil → sarıya gradient (HP oranına göre) | — |
| Kritik (≤25%) | Kırmızı | Yavaş titreme |
| Savaş Dışı (0%) | Boş (gri arka plan) | Soluk sprite |

### Audio Gereksinimleri

| Olay | Ses Türü | Ton | Öncelik |
|------|----------|-----|---------|
| Hasar alma | Kısa "darbe" sesi — etli, fiziksel | Element rengine göre değişebilir (Hasar Hesaplama'da) | MVP |
| Kritik HP uyarısı | Kalp atışı benzeri düşük ritimli ses | Gerilim, aciliyet | MVP |
| Savaş dışı kalma | Düşen-kaybolma sesi | Kesin ama dramatik değil — cezalandırıcı hissettirmemeli | MVP |
| İyileşme | Yükselen, parlak "ding" + yumuşak chime | Pozitif, rahatlatıcı | MVP |
| Tam can iyileşme | Kısa, tatmin edici "tam dolma" sesi | Başarı hissi | Nice-to-have |

## UI Requirements

### Savaş Ekranı
- Her canavarın (oyuncu + düşman) üstünde HP barı: arka plan gri, doluluk rengi HP oranına göre (yeşil → sarı → kırmızı)
- HP barı boyutu: genişlik 80px, yükseklik 8px (canavar sprite'ının altında veya üstünde)
- HP barı yanında sayısal gösterim: `current_hp / max_hp` (opsiyonel — tuning knob ile açılıp kapatılabilir)
- Hasar alınca HP barında "gecikmeli azalma" efekti: önce beyaz bölüm kalır, sonra kırmızıya dönerek kaybolur (0.3s gecikme)
- Kritik durumda HP barı titrer (CSS shake veya sinüs pulse)
- Savaş dışı canavarın HP barı tamamen boş + gri

### Takım Yönetimi Ekranı
- Canavar kartında HP stat gösterimi: kalp ikonu + max_hp sayısı
- Güçlendirme sonrası HP artışını yeşil renkte göster (ör: "30 → **35** (+5)")
- Evrim önizlemesinde yeni max_hp gösterimi

### Minimum Dokunma Hedefi
- HP barlarına dokunulduğunda detay popup (current_hp / max_hp, regen oranı) — minimum 44×44 dp dokunma alanı

> **UX Flag — Sağlık / Can Sistemi**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te (Pre-Production) `/ux-design` çalıştırarak savaş ekranı HP barı ve takım yönetimi ekranı için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** Rare Saldırgan Lv1 (stat havuzu=150, HP%=20%), **WHEN** savaşa girilirse, **THEN** `max_hp = 30` ve `current_hp = 30`.

2. **GIVEN** Common Tank Lv1 (stat havuzu=100, HP%=30%), **WHEN** savaşa girilirse, **THEN** `max_hp = 30` ve `current_hp = 30`.

3. **GIVEN** canavar `current_hp = 25`, **WHEN** `TakeDamage(target, 10)` çağrılırsa, **THEN** `current_hp = 15`.

4. **GIVEN** canavar `current_hp = 5`, **WHEN** `TakeDamage(target, 20)` çağrılırsa (overkill), **THEN** `current_hp = 0` (negatife düşmez).

5. **GIVEN** canavar `current_hp = 0`, **WHEN** `TakeDamage(target, 10)` çağrılırsa, **THEN** hasar yok sayılır, `current_hp` hâlâ 0.

6. **GIVEN** canavar `current_hp = 0`, **WHEN** savaş dışı kalırsa, **THEN** saldıramaz, hasar alamaz, yetenek kullanamaz — savaş sonuna kadar aktif değil.

7. **GIVEN** canavar `current_hp = 20, max_hp = 45`, **WHEN** `Heal(target, 9)` çağrılırsa, **THEN** `current_hp = 29`.

8. **GIVEN** canavar `current_hp = 40, max_hp = 45`, **WHEN** `Heal(target, 9)` çağrılırsa (overheal), **THEN** `current_hp = 45` (max_hp'yi aşmaz).

9. **GIVEN** canavar `current_hp = 0` (savaş dışı), **WHEN** `Heal(target, 20)` çağrılırsa, **THEN** iyileşme reddedilir, `current_hp` hâlâ 0.

10. **GIVEN** canavar `max_hp = 45`, **WHEN** pasif rejenerasyon uygulanırsa, **THEN** `regen_amount = max(1, floor(45 * 0.02)) = 1`.

11. **GIVEN** takımdaki tüm 4 canavar savaş dışı, **WHEN** son canavar düşerse, **THEN** savaş kaybetme olayı tetiklenir.

12. **GIVEN** zindan katı tamamlanmış, **WHEN** yeni kata geçilirse, **THEN** tüm canavarlar `current_hp = max_hp` ile başlar (savaş dışı kalmış dahil).

13. **GIVEN** canavar `current_hp = 10, max_hp = 45, critical_threshold = 0.25`, **WHEN** HP kontrol edilirse, **THEN** `10 <= 45 * 0.25 = 11.25` → Kritik durumda, HP barı kırmızı.

14. **GIVEN** canavar Legendary Tank Form C (stat havuzu=441, HP%=30%), **WHEN** savaşa girilirse, **THEN** `max_hp = floor(441 * 0.30) = 132`.

15. **GIVEN** `max_hp = 0` (veri hatası), **WHEN** savaşa girilirse, **THEN** `max_hp = 1` olarak clamp edilir, hata loglanır.

## Open Questions

1. **Kalkan/Bariyer mekanikliği (Tier 2+)**: İleride HP'nin üstüne geçici kalkan mekanizması eklenecek mi? Kalkan hasar alır, HP'den önce tükenir. → Hibrit Savaş veya ayrı bir GDD ile tanımlanacak.

2. **HP tabanlı tetiklemeler**: Düşük HP'de aktifleşen pasif yetenekler olacak mı? (ör: "HP %20 altındayken +30% ATK — son çaba") → Savaş Sistemi GDD'sinde tanımlanacak.

3. **Seviye ile HP büyüme eğrisi**: max_hp seviye başına ne kadar artar? Lineer mi, logaritmik mi? → Canavar Güçlendirme GDD'sinde tanımlanacak.

4. ~~**Otofarm'da HP yönetimi**~~: **ÇÖZÜLDÜ** — Otofarm / Idle Sistemi GDD'sinde (Approved) "Geri Dönüş Anında Simülasyon Modeli" tanımlanmıştır. Gerçek arka plan HP hesaplaması yok — oyuncu geri döndüğünde geçen süre simüle edilir.
