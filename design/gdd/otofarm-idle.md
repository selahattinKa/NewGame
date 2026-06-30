# Otofarm / Idle Sistemi (Auto-Farm / Idle System)

> **Status**: Revised
> **Author**: Kullanıcı + Claude Code Game Studios
> **Last Updated**: 2026-06-24
> **Implements Pillar**: Senin Tempon (Play Your Way), Cömert Zindan (Generous Loot)
> **Review History**: design/gdd/reviews/otofarm-idle-review-log.md

## Overview

Otofarm / Idle Sistemi, oyuncunun aktif olmadığı sürelerde canavarlarının zindan katlarını otomatik olarak geçmeye devam etmesini ve geri dönüşte birikmiş ödülleri toplamasını sağlayan arka plan ilerleme sistemidir. Mekanik olarak gerçek arka plan işlemi çalıştırmaz — oyuncu geri döndüğünde Kaydetme/Yükleme sisteminden alınan `offline_duration` değerine göre "geri dönüş anında simülasyon" yapılır. Simülasyon kat-tabanlıdır: geçen sürede kaç kat geçilmiş olacağı hesaplanır, her idle katı için stokastik canavar ve malzeme ruloları yapılır (Loot GDD idle oranları + idle-pity sistemi ile), altın birikimi zamana dayalı hesaplanır (Ekonomi GDD `idle_gold_formula` ile, azalan getiri kademeleriyle). Aktif oynama %100 verimlilikle çalışırken, otofarm %50 altın verimi ve %25 loot verimi sunar — idle'da canavar düşme oranı %25'e düşer ve C+ çok düşük oranlarla mümkündür (F/D dominant). Otofarm **otomatik olarak aktifleşir**: koşullar sağlandığında (en az 1 kat geçilmiş, takımda canavar var) uygulama kapandığında idle otomatik başlar — oyuncunun "başlat" butonuna basmasına gerek yoktur. Bu tasarımla Pillar 3 "Senin Tempon" aktif ve pasif oyuncuyu dengelenir, Pillar 1 "Cömert Zindan" gereği idle bile ödüllendirici kalır. Oyuncu perspektifinden sistem, "oynamamken bile imparatorluğum büyüyor" rahatlığını verir: uygulamayı açtığında birikmiş altın, kazanılmış canavarlar, evrim malzemeleri ve ilerlenmiş katları gösteren bir geri dönüş özet ekranı karşılar. MVP'de tek zindan bölgesinde idle çalıştırma, geri dönüş simülasyonu ve özet ekranı tasarlanır.

## Player Fantasy

Oyuncu Otofarm sisteminde **"imparatorluğum bensiz bile çalışıyor"** fantezisini yaşar. Çekirdek an, uygulamayı açtığında geri dönüş ekranının birikmiş ödüllerle dolmasıdır: altın sayacı yukarı doğru sayar, kazanılmış canavarlar sırayla belirir, evrim malzemeleri listelenir, ilerlenmiş katlar haritada işaretlenir. "8 saat uyudum, ordularım 96 kat geçmiş" keşif anı — zahmetsiz güçlenme ve zenginleşme hissi. Kat-tabanlı loot ruloları sayesinde her geri dönüş farklı: bazen 4 canavar, bazen 2 — her seferinde "bu sefer ne birikmiş?" merakı küçük bir hediye kutusu deneyimi yaratır.

İki katmanda hissedilir: **Rahatlık** — meşgul oyuncu oynamasa bile ilerlediğini bilir, "geride kaldım" kaygısı yoktur. Otofarm otomatik başladığı için "unutma" riski yoktur. **Heyecan** — geri dönüşte stokastik loot ruloları sayesinde her açılış sürpriz içerir. C ve üstü kademe canavar çok düşük oranlarla düşebilir (F/D dominant) ama malzeme ve altın miktarı her seferinde farklıdır.

Negatif fantezi (kaçınılacak): "Otofarm her şeyi yapıyor, neden oynayayım?" — idle verimlilik kasıtlı olarak aktifin yarısıdır (%50 altın, %25 loot). C+ kademede canavarlar idle'da çok düşük oranlarla düşebilir ama aktif oyun çok daha verimlidir — aktif avda C+ oranları kat başına ~7-8x daha yüksek. Komutan modunda oynamak somut fark yaratmalı. Otofarm rakibi değil, **aktif avları besleyen kaynak akışıdır** — idle'dan gelen canavarlar ve malzemeler, aktif oyundaki evrim ve güçlendirme sistemlerini besler. Ara sıra idle'dan gelen sürpriz C tier canavar, "bu sefer ne birikmiş?" merakını canlı tutar.

Pillar bağlantısı: "Senin Tempon" — aktif oyuncuya premium verimlilik, pasif oyuncuya hâlâ anlamlı ilerleme. "Cömert Zindan" — idle bile her zaman ödüllendirici (azalan getiriyle bile 24 saatte anlamlı birikim). "Güç Hisset" — takım gücü arttıkça idle altın kazancı da artar, geri dönüşte büyümüş kaynak ve koleksiyon.

*`creative-director` consulted — Full review, senior synthesis completed.*

## Detailed Rules

### Core Rules

1. **Geri Dönüş Anında Simülasyon Modeli**: Gerçek arka plan işlemi çalışmaz. Oyuncu uygulamayı açtığında, Kaydetme/Yükleme sisteminden `offline_duration` alınır ve geçen süre boyunca ne olmuş olacağı anında hesaplanır. Bu yaklaşım mobilde pil tüketmez ve iOS arka plan kısıtlamalarından etkilenmez.

2. **Otomatik Aktivasyon**: Otofarm aşağıdaki koşullar sağlandığında **otomatik olarak aktifleşir** — oyuncunun manuel buton basmasına gerek yoktur:
   - Oyuncu en az 1 zindan katını başarıyla geçmiş olmalı
   - Oyuncu en az 1 canavara sahip olmalı (aktif takımda)
   - Koşullar sağlandığında, uygulama kapanışında (`OnApplicationPause`) idle otomatik başlar
   - `idle_state` save'e yazılır: `{active: true, start_time, floor, region, team_power, idle_pity_bonus, pending_report: null}`
   - `team_power` başlangıç anında snapshot olarak kaydedilir (geri dönüşte değişmez)
   - `idle_pity_bonus` önceki oturumdan kalıcı olarak taşınır (yeni oturumda sıfırlanmaz)

3. **Farm Katı Belirleme**: Otofarm her zaman oyuncunun **en yüksek geçilmiş katı** üzerinde çalışır. Yeni kat açmaz — yeni katlar sadece aktif oynama ile açılır.

4. **Idle Verimlilik Oranları**:
   - **Altın**: Aktifin %50'si (`idle_efficiency = 0.50`, Ekonomi GDD). Azalan getiri kademeleri uygulanır (bkz. Kural 5).
   - **Loot (canavar)**: Her idle katı için stokastik rulo — `base_monster_rate × idle_loot_efficiency = 0.15 × 0.25 = %3.75/kat`. İdle-pity uygulanır (bkz. Formüller).
   - **Loot (evrim malzemesi)**: Her idle katı için stokastik rulo — `base_material_rate × idle_loot_efficiency = 0.08 × 0.25 = %2/kat`. Pity uygulanmaz.
   - **Canavar kademesi**: Tüm kademeler düşebilir ama C+ çok düşük oranlarla. İdle kademe ağırlıkları:

     | Kademe | İdle Ağırlık | İdle Olasılık | Aktif Olasılık (referans) |
     |--------|-------------|---------------|--------------------------|
     | F | 0.700 | %69.9 | %58.6 |
     | D | 0.240 | %24.0 | %29.3 |
     | C | 0.050 | %5.0 | %8.8 |
     | B | 0.008 | %0.80 | %2.3 |
     | A | 0.002 | %0.20 | %0.59 |
     | S | 0.001 | %0.10 | %0.29 |
     | SS | — | — (idle'da düşmez) | %0.06 |
   - **XP İksiri**: İdle'da düşmez
   - **Elmas**: İdle'da düşmez

5. **Azalan Getiri Kademeleri** (altın için):

   | Kademe | Süre Aralığı | Verimlilik | Açıklama |
   |--------|-------------|------------|----------|
   | Tier 1 | 0–480 dk (0-8 saat) | %100 | Tam idle altın oranı |
   | Tier 2 | 481–960 dk (8-16 saat) | %75 | Azalan getiri başlar |
   | Tier 3 | 961–1440 dk (16-24 saat) | %50 | Minimum idle oranı |

   Bu tasarım günde 2-3 giriş teşvik eder: 8 saatte bir toplayan oyuncu her zaman Tier 1 oranından faydalanır.

6. **Enerji Tüketimi**: Otofarm enerji tüketmez. Enerji yalnızca aktif zindan girişinde harcanır. İdle sırasında enerji normal hızda yenilenir (1 enerji / 5 dk, tavan 100). Enerji hesaplaması Ekonomi sistemi tarafından `offline_duration` kullanılarak bağımsız yapılır.

7. **Süre Sınırı**: Maksimum 1440 dakika (24 saat) birikimi (`max_offline_minutes`). Bu süreyi aşan çevrimdışılık hesaplanmaz.

8. **Simülasyon Hesaplama Adımları** (geri dönüşte):
   - Adım 1: `offline_duration` al (Kaydetme/Yükleme'den, dakika). `max_offline_minutes` ile sınırla.
   - Adım 2: `idle_floors_cleared = max(1, floor(offline_duration / idle_minutes_per_floor))` hesapla (yalnızca `offline_duration > 0` ise).
   - Adım 3: `idle_gold` hesapla — azalan getiri kademeleriyle (bkz. Formüller). `team_power` save'deki snapshot değerini kullanır.
   - Adım 4: Her idle katı (1..idle_floors_cleared) için seeded-RNG ile stokastik canavar rulosu yap. İdle-pity uygula (oturumlar arası kalıcı). İdle kademe tablosu uygulanır (F/D dominant, C+ çok düşük).
   - Adım 5: Her idle katı için seeded-RNG ile stokastik evrim malzemesi rulosu yap. Bölge elementine göre ağırlıklı (%70 bölge, %30 diğer).
   - Adım 6: Sonuçları paketle → `ReturnReport` oluştur → `idle_state.pending_report`'a yaz ve save yap.
   - Adım 7: Geri dönüş ekranı göster (modal) → oyuncu "Topla" butonuna basar → ödülleri oyuncu kaynaklarına ekle → `idle_state.pending_report = null`, save yap.

   **Not:** Adım 6 ve Adım 7 asenkrondur — aralarında kullanıcı etkileşimi (modal rapor ekranı) vardır. Ödüller yalnızca Adım 7'de uygulanır, Adım 6'da değil.

9. **Geri Dönüş Özet Ekranı** (modal — kapatılmadan navigasyon yapılamaz):
   - Çevrimdışı süre ("8 saat 23 dakika boyunca ordun savaştı!")
   - Toplam kazanılan altın (animasyonlu sayaç)
   - Kazanılan canavarlar (varsa — nadirlik sırasıyla gösterilir)
   - Kazanılan evrim malzemeleri (element renkleriyle)
   - Geçilen kat sayısı
   - "Topla" butonu ile tüm ödüller envantere aktarılır
   - "Animasyonu Atla" butonu: tüm animasyonları atlar, doğrudan "Topla"ya geçer

10. **Aktif Zindan Etkileşimi**: Oyuncu aktif zindan girişi yaptığında:
    - O ana kadarki idle birikimi hesaplanır (start_time'dan şu ana kadar)
    - Kısmi birikim geri dönüş raporu olarak gösterilir (aynı modal format)
    - "Topla" ile toplanır
    - Aktif zindan oturumu biter bitmez, otofarm **otomatik olarak yeniden başlar** (koşullar hâlâ sağlanıyorsa). Yeniden başlatma gerekmez.

11. **İdle Minimum Hasat Garantisi**: `offline_duration >= 60 dk` olan her idle oturumunda, stokastik rulolardan bağımsız olarak minimum garantili ödüller verilir:
    - **Garantili F tier canavar**: 1 adet (bölge havuzundan rastgele seçilir)
    - **Garantili F tier malzeme**: 1 adet (bölge elementinde)
    - Bu garantili ödüller stokastik rulo sonuçlarına **eklenir** (yerine geçmez)
    - `offline_duration < 60 dk` ise garanti uygulanmaz — yalnızca stokastik rulo sonuçları verilir
    - Garantili ödüller `ReturnReport`'ta normal loot gibi sunulur — oyuncu mekanizmayı görmez
    - Simülasyon adımında: garantili ödüller Adım 4-5'ten önce pakete eklenir, stokastik rulolar üzerine eklenir

12. **İdle Boss Katı Mütevazı Bonusu**: İdle sırasında boss katları (her 5. kat) normal katlara kıyasla mütevazı ek ödül verir:
    - **Altın**: Boss katlarında idle altın oranı **1.5x** uygulanır (aktif: 3x)
    - **Malzeme**: Boss katlarında **1 F tier malzeme garantili** düşer (aktif: garantili + %30 ek)
    - **Canavar**: Normal idle loot tablosu kullanılır (boss canavar düşüşü idle'da yok)
    - Bu bonus aktif boss avının ödül değerini korur (aktif: 3x altın + boss canavar + garanti) ama idle'da boss katları "tamamen boş" hissettirmez

### States and Transitions

| State | Açıklama | Geçiş Tetikleyicisi |
|-------|----------|---------------------|
| **Locked** | Henüz koşullar sağlanmadı (0 kat geçilmiş veya aktif takımda 0 canavar) | İlk kat geçildiğinde + takımda canavar var → Idle |
| **Idle** | Koşullar sağlandı, uygulama açık, idle "beklemede" | Uygulama kapanır (OnApplicationPause) → Accumulating |
| **Accumulating** | Oyuncu çevrimdışı, birikim hesaplanacak | Oyuncu geri döner → ReturnPending |
| **ReturnPending** | Geri dönüş ekranı gösteriliyor (modal) | "Topla" butonu → Idle |
| **ActiveSession** | Oyuncu aktif zindan girişi yaptı, idle duraklatıldı | Aktif oturum biter → Idle (otomatik yeniden başlatma) |

**Kritik geçişler:**
- `Idle → Accumulating`: Uygulama kapanışında otomatik — oyuncu etkileşimi gerekmez.
- `Accumulating → ReturnPending`: Uygulama açıldığında — simülasyon hesaplanır, rapor oluşturulur.
- `ReturnPending → Idle`: "Topla" basıldığında — ödüller uygulanır, pending_report temizlenir.
- `Idle → ActiveSession`: Aktif zindan girişinde — kısmi idle birikim hesaplanır, rapor gösterilir.
- `ActiveSession → Idle`: Aktif oturum bitişinde — otomatik yeniden başlatma. Yeni `start_time` kaydedilir.
- `Locked → Idle`: İlk kat geçildiğinde — "Otofarm açıldı!" bildirimi gösterilir.

### Interactions with Other Systems

| Sistem | Yön | Arayüz | Detay |
|--------|------|--------|-------|
| **Kaydetme / Yükleme** | ← `offline_duration` alır, → `idle_state` yazar | `idle_state`: {active, start_time, floor, region, team_power, idle_pity_bonus, pending_report} | Çevrimdışı süre + idle durumu + pity kalıcılığı + bekleyen rapor persist |
| **Ekonomi / Kaynak Yönetimi** | ← Altın oranı alır, → ödülleri uygular | `GetIdleGoldRate(teamPower)`, `GrantReward()` | idle_gold_per_minute × offline_minutes (kademeli azalan getiri) |
| **Loot / Ödül Sistemi** | ← İdle loot oranları ve pity parametreleri alır | `GetIdleLootRate(teamPower, region)`, `GetPityParams()` | Kat-tabanlı stokastik rulo, idle-pity ayrı counter |
| **Savaş Sistemi** | ← Otofarm verimlilik farkı | Mod bilgisi | Otofarm %50-25 verim, komutan %100 |
| **Zindan Keşif** | ← En yüksek geçilmiş kat | `GetHighestClearedFloor(region)` | Farm katı belirleme |
| **Canavar Toplama** | → Kazanılan canavarları ekler | `OnMonsterDropped(monsterId)` | İdle canavarları envantere/beklemeye |
| **Takım Kurma** | ← Aktif takım gücü | `GetActiveTeamPower()` | Idle gold rate hesaplama (başlangıç snapshot) |
| **UI Framework** | → Geri dönüş ekranı verileri | `GetReturnReport(): ReturnReport` | Özet ekranı rendering |

**`ReturnReport` veri yapısı:**
```
ReturnReport {
    offlineDuration: TimeSpan
    goldEarned: int
    monstersEarned: List<MonsterDrop>  // {monsterId, rarity}
    materialsEarned: List<MaterialDrop>  // {materialId, element, quantity}
    floorsCleared: int
    inventoryFull: bool
    waitingAreaCount: int  // bekleme alanına alınan canavar sayısı
    autoSoldCount: int  // otomatik satılan canavar sayısı
    autoSoldGold: int  // otomatik satış geliri
}
```

## Formulas

### teamPower → active_gold_per_minute (Geçici — Ekonomi GDD'ye taşınacak)

`active_gold_per_minute = 10 + 90 × (clamp(teamPower, 60, 6864) - 60) / 6804`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|------|--------|----------|
| teamPower | TP | int | 60–6864 | Takım toplam güç skoru (Takım Kurma GDD) |
| active_gold_per_minute | G_act | float | 10–100 | Dakika başı aktif altın oranı |

**Çıktı Örnekleri:** TP=60 → 10/dk, TP=3462 → ~55/dk, TP=6864 → 100/dk.

> **Sahiplik Notu**: Bu formülün kalıcı sahibi Ekonomi GDD'dir (Formül 3). Buradaki kopya referans amaçlıdır — güncellemeler Ekonomi GDD'den yapılmalıdır.

### idle_floors_cleared (İdle Geçilen Kat)

`idle_floors_cleared = max(1, floor(offline_duration / idle_minutes_per_floor))` (offline_duration > 0 ise)

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|------|--------|----------|
| offline_duration | D_off | int | 0–1440 | Çevrimdışı süre (dk, Kaydetme/Yükleme'den, max_offline_minutes ile sınırlı) |
| idle_minutes_per_floor | M_floor | float | 5.0 (min 3.0) | İdle'da kat başına süre (aktif ~2 dk, idle 2.5x yavaş) |

**Güvenlik:** `M_floor` hiçbir zaman ≤ 0 olamaz — runtime'da `max(M_floor, 3.0)` clamp uygulanır.

**Çıktı Aralığı:** 1–288 kat (varsayılan M_floor=5.0, D_off>0 ise). Duration=0 ise hesaplama yapılmaz, rapor gösterilmez.

**Örnekler:**
- 8 saat = 480 dk → `max(1, floor(480/5)) = 96 kat`
- 3 dk → `max(1, floor(3/5)) = max(1, 0) = 1 kat`
- 0 dk → hesaplama yapılmaz

**Cascade uyarısı:** `idle_minutes_per_floor` değiştirildiğinde kat sayısı ve dolayısıyla loot rulosu sayısı doğrudan değişir. M_floor=3.0'da: 1440/3 = 480 kat, 480 rulo — canavar/malzeme kazanımı ~1.7x artar. Tuning'de dikkat.

**Kat farmlama modeli:** İdle her zaman oyuncunun en yüksek geçilmiş katını **tekrar** farmlar — idle yeni kat açmaz ve kat aralığı simüle etmez. Tüm idle katları aynı kat numarası ve bölge bağlamında işlenir. En yüksek geçilmiş kat boss katıysa (her 5. kat), idle boss mütevazı bonusu uygulanır (Kural 12: 1.5x altın, garantili F tier malzeme).

### idle_gold (İdle Altın Birikimi — Azalan Getirili)

```
idle_gold = Tier1_gold + Tier2_gold + Tier3_gold

Tier1_gold = idle_gold_per_minute × min(D_off, 480)
Tier2_gold = idle_gold_per_minute × tier_2_multiplier × min(max(0, D_off - 480), 480)
Tier3_gold = idle_gold_per_minute × tier_3_multiplier × min(max(0, D_off - 960), 480)

idle_gold_per_minute = active_gold_per_minute × idle_efficiency
```

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|------|--------|----------|
| active_gold_per_minute | G_act | float | 10–100 | Takım gücüne göre (yukarıdaki formül) |
| idle_efficiency | E_idle | float | 0.50 | Aktifin %50'si (Kaynak: Ekonomi GDD) |
| offline_duration | D_off | int | 0–1440 | Çevrimdışı süre (dk) |
| tier_2_multiplier | T2 | float | 0.75 | 8-16 saat verimlilik çarpanı |
| tier_3_multiplier | T3 | float | 0.50 | 16-24 saat verimlilik çarpanı |

**Çıktı Örnekleri (idle_gold_per_minute = 25, orta oyun):**
- 8 saat (480 dk): 25 × 480 = **12.000 altın** (tamamı Tier 1)
- 16 saat (960 dk): 12.000 + 25 × 0.75 × 480 = **21.000 altın** (Tier 1 + Tier 2)
- 24 saat (1440 dk): 21.000 + 25 × 0.50 × 480 = **27.000 altın** (tüm kademeler)

**Maksimum (idle_gold_per_minute = 50, ileri oyun):** 24h → 54.000 altın.

### idle_monster_drops (İdle Canavar Düşmesi — Stokastik Kat-Tabanlı)

Her idle katı (1..idle_floors_cleared) için bağımsız stokastik rulo:

```
idle_pity_bonus = idle_state.idle_pity_bonus  // oturumlar arası kalıcı
pity_enabled = (offline_duration >= 30)  // 30 dk altı oturumlarda pity artmaz

for each idle_floor in 1..idle_floors_cleared:
    monster_chance = base_monster_rate × idle_loot_efficiency + idle_pity_bonus
    roll = seeded_random(0.0, 1.0)  // seed: idle_state.start_time
    if roll < monster_chance:
        tier = weighted_random(F: 0.700, D: 0.240, C: 0.050, B: 0.008, A: 0.002, S: 0.001)
        add monster(rarity) to results
        idle_pity_bonus = 0.0  // pity sıfırla
    else:
        if pity_enabled:
            idle_pity_bonus = min(idle_pity_bonus + pity_increment, max_pity)
```

| Değişken | Sembol | Tip | Değer | Kaynak |
|----------|--------|------|-------|--------|
| base_monster_rate | R_mon | float | 0.15 | Loot GDD |
| idle_loot_efficiency | E_loot | float | 0.25 | Loot GDD (aralık: 0.10–0.40) |
| pity_increment | P_inc | float | 0.06 | İdle-spesifik (aktif: 0.03) |
| max_pity | P_max | float | 0.45 | Loot GDD |
| idle_pity_bonus | P_idle | float | 0.0 (başlangıç) | İdle-spesifik counter |

**Kat başı temel şans:** 0.15 × 0.25 = **%3.75/kat** (pity 0 iken).
**Pity tavanda şans:** %3.75 + %45 = **%48.75/kat**.

**İdle-pity kuralları:**
- İdle-pity, aktif oyun pity counter'ından tamamen bağımsızdır.
- İdle-pity oturumlar arasında **kalıcıdır** — oturum değişikliğinde sıfırlanmaz. `idle_pity_bonus` değeri `idle_state`'te persist edilir.
- İdle-pity yalnızca canavar düştüğünde sıfırlanır (`idle_pity_bonus = 0.0`). "Topla" ile toplanma veya yeni oturum başlatma sıfırlamaz.
- Aktif pity'yi etkilemez.
- **Mikro-oturum koruması**: İdle-pity yalnızca `offline_duration >= 30 dk` olan oturumlarda birikir. 30 dk'dan kısa oturumlarda pity artmaz — sık giriş/çıkış ile pity exploit'unu engeller.
- **İdle pity increment**: 0.06 (aktif: 0.03). İdle'ın düşük base rate'ini (%3.75 vs %15) telafi etmek için 2x hızlı ramp. 8 ardışık miss'te tavana ulaşır (aktif: 10 miss).

**Seeded RNG Spesifikasyonu:**
- **Algoritma:** `System.Random` (C# — aynı seed, aynı platform → deterministik)
- **Seed formatı:** `idle_state.start_time` Unix timestamp saniye cinsinden (int32)
- **Tüketim sırası:** Tek `Random` instance oluşturulur, her kat için sırayla: (1) canavar rulosu, (2) canavar nadirlik rulosu (düştüyse), (3) malzeme rulosu, (4) malzeme element rulosu (düştüyse)
- **Deterministik garanti:** Aynı `start_time` + aynı `idle_state` ile aynı sonuçlar üretilir
- **pending_report zorunlu persist:** `pending_report` Adım 6'da her zaman save'e yazılır. Geri dönüşte `pending_report` varsa doğrudan okunur — re-simülasyon yapılmaz. Bu tasarım cross-platform determinizm sorunlarını (`System.Random` iOS vs Android) ortadan kaldırır

**Beklenen Çıktı Aralığı (yaklaşık, idle pity 0.06 + minimum paket dahil):**
- 8 saat (96 kat): ~7-9 canavar (~5 F, ~2 D, ~%30 şansla 1 C) — 1 garantili dahil
- 24 saat (288 kat): ~19-23 canavar (~14 F, ~4 D, ~1-2 C, nadir B) — 1 garantili dahil
- Kademe dağılımı: F dominant (%69.9), C+ sürpriz olarak mümkün
- `offline_duration < 60 dk` ise garantili canavar yok, yalnızca stokastik

### idle_material_drops (İdle Evrim Malzemesi Düşmesi — Stokastik Kat-Tabanlı)

Her idle katı için bağımsız stokastik rulo:

```
for each idle_floor in 1..idle_floors_cleared:
    material_chance = base_material_rate × idle_loot_efficiency
    roll = seeded_random(0.0, 1.0)
    if roll < material_chance:
        element = weighted_random(region_element: 0.70, other_element_1: 0.10, other_element_2: 0.10, other_element_3: 0.10)
        add material(element) to results
```

| Değişken | Sembol | Tip | Değer | Kaynak |
|----------|--------|------|-------|--------|
| base_material_rate | R_mat | float | 0.08 | Loot GDD |
| idle_loot_efficiency | E_loot | float | 0.25 | Loot GDD |

**Kat başı şans:** 0.08 × 0.25 = **%2/kat**. Pity uygulanmaz.

**Beklenen Çıktı Aralığı:**
- 8 saat (96 kat): ~2 malzeme
- 24 saat (288 kat): ~6 malzeme
- Element dağılımı: %70 bölge elementi, %30 diğer elementler

## Edge Cases

- **If offline_duration = 0** (hemen geri dönüş): Geri dönüş raporu gösterilmez, doğrudan ana menüye geçilir. İdle birikimi hesaplanmaz.

- **If offline_duration çok kısa** (> 0, < 5 dk): Geri dönüş raporu gösterilir. `idle_floors_cleared = max(1, floor(D/M))` ile minimum 1 kat hesaplanır, 1 kat için loot rulosu yapılır, altın birikimi hesaplanır.

- **If oyuncu aktif zindan girişi yaparsa** (idle Idle durumundayken): İdle kısmi birikim hesaplanır (start_time'dan şu ana kadar). Geri dönüş raporu gösterilir (aynı modal format), "Topla" ile toplanır. Aktif zindan oturumu bitince otofarm **otomatik yeniden başlar**.

- **If oyuncunun aktif takımı boşsa** (tüm canavarlar satılmış): Otofarm Locked durumda kalır. Uygulama kapanışında idle başlamaz. Oyuncu geri döndüğünde idle birikimi yok (yalnızca enerji yenilenmiş).

- **If idle'da canavar düşer ve envanter doluysa**: Canavar Toplama GDD kuralları geçerli — bekleme alanına alınır (max 10, 7 gün süre). Bekleme alanı da doluysa (10/10), sığmayan canavarlar otomatik satılır ve satış altını oyuncuya eklenir.

- **If oyuncu cihaz saatini ileri alarak idle birikimi exploit etmeye çalışırsa**: Kaydetme/Yükleme edge case'i devreye girer — `offline_duration = 0`, birikim yok.

- **If idle farm katında boss varsa** (her 5. kat): İdle'da boss katları **mütevazı bonus** ile işlenir (Kural 12): 1.5x altın çarpanı (aktif: 3x) ve 1 garantili F tier malzeme (aktif: garantili + %30 ek). Boss canavar düşüşü idle'da yoktur — boss canavar avı aktif oyunun ödülüdür. Normal idle canavar loot tablosu uygulanır.

- **If idle sırasında enerji tamamen dolarsa**: Enerji yenilenmesi tavanı (100) aşmaz. İdle enerji tüketmediğinden bu normal davranıştır. Enerji hesaplaması: `energy = min(saved_energy + floor(offline_duration / 5), 100)` — Ekonomi sistemi tarafından bağımsız yürütülür.

- **If idle birikim raporu gösterilirken uygulama kapanırsa**: `pending_report` save'de kayıtlıdır (Adım 6'da yazıldı). Bir sonraki açılışta `pending_report` doğrudan save'den okunur ve aynı rapor tekrar gösterilir — re-simülasyon yapılmaz (cross-platform determinizm garantisi). Ödüller yalnızca "Topla" ile uygulanır (Adım 7).

- **If koşullar idle sırasında değişirse** (örn. son canavar aktif oturumda satıldı): Mevcut idle birikimi (varsa) normal şekilde hesaplanır ve geri dönüşte verilir. Bir sonraki uygulama kapanışında koşullar kontrol edilir — sağlanmıyorsa idle başlamaz (Locked'a döner).

- **If idle_state.start_time bozuksa** (null, 0, gelecek tarihi): Otofarm durumu `Idle`'a sıfırlanır, birikim hesaplanmaz, hata sessizce loglanır. Defansif kontrol: `start_time == 0 || start_time > now → birikim yok`.

## Dependencies

**Upstream Bağımlılıklar (Hard):**

| Sistem | Bağımlılık Tipi | Arayüz | Açıklama |
|--------|----------------|--------|----------|
| **Kaydetme / Yükleme** | Hard | `offline_duration`, `idle_state` (genişletilmiş şema) | Çevrimdışı süre, idle durumu, `team_power` snapshot ve `pending_report` persist. Şema genişletme gerekiyor. |
| **Ekonomi / Kaynak Yönetimi** | Hard | `GetIdleGoldRate(teamPower)`, `GrantReward()` | Idle altın oranı ve ödül uygulama. `teamPower → active_gold_per_minute` formülü Ekonomi GDD'de tanımlanmalı (şu an geçici olarak burada). |
| **Loot / Ödül Sistemi** | Hard | `GetIdleLootRate(teamPower, region)`, `GetPityParams()` | Idle canavar/malzeme oranları ve pity parametreleri. İdle-pity kuralı Loot GDD'ye eklenmelidir. |

**Upstream Bağımlılıklar (Soft):**

| Sistem | Bağımlılık Tipi | Arayüz | Açıklama |
|--------|----------------|--------|----------|
| **Savaş Sistemi** | Soft | Mod tanımı, verimlilik farkı | Otofarm mod tanımı savaş sisteminden gelir ama idle sistem savaş motoru çalıştırmaz. |
| **Zindan Keşif** | Soft | `GetHighestClearedFloor(region)` | Farm katı belirleme. |
| **Savaş Sistemi** | Soft | `GetActiveTeamPower()` | Idle gold rate hesaplama. teamPower otofarm başlangıcında snapshot olarak kaydedilir. |
| **Canavar Toplama** | Soft | `OnMonsterDropped(monsterId)` | İdle canavarları envantere ekleme. |

**Downstream Bağımlılıklar:**

| Sistem | Bağımlılık Tipi | Arayüz | Açıklama |
|--------|----------------|--------|----------|
| **UI Framework** | Soft | `GetReturnReport(): ReturnReport` | Geri dönüş özet ekranı. |
| **Bildirim Sistemi** (Tier 2) | Soft | Push bildirimi | "Birikim hazır!" bildirimi (Tier 2). |

## Tuning Knobs

| Knob | Varsayılan | Güvenli Aralık | Çok Düşükse | Çok Yüksekse | Etkilenen Oynanış |
|------|-----------|----------------|-------------|-------------|-------------------|
| `idle_minutes_per_floor` | 5.0 | 3.0–10.0 | Çok hızlı farm → loot rulosu fazla → aktif denge bozulur | Çok yavaş → idle tatminsiz | İdle hızı + loot rulosu sayısı. **Cascade:** Bu knob canavar/malzeme düşüş sayısını doğrudan etkiler (daha fazla kat = daha fazla rulo). 3.0'da: 480 kat/24h → ~1.7x canavar artışı. Alt sınır 3.0: 2.0'da idle malzemesi aktifi aşıyordu (~14 vs ~10/gün). |
| `idle_efficiency` | 0.50 | 0.30–0.70 | İdle anlamsız → Pillar 3 ihlali | Aktif oynama gereksiz → Pillar 3 ihlali | Altın verimi (Kaynak: Ekonomi GDD) |
| `idle_loot_efficiency` | 0.25 | 0.10–0.40 | Canavar düşmesi çok nadir | Aktif farm avantajı azalır | Loot verimi (**Kaynak: Loot GDD**, aralık 0.10–0.40) |
| `max_offline_minutes` | 1440 | 720–1440 | Kısa cap → sık giriş baskısı | 1440 üstü → ekonomi bozulur | Offline birikim tavanı (Kaydetme/Yükleme ile paylaşılır) |
| `min_idle_duration_for_report` | 0 | 0–15 | Her pozitif süre rapor gösterir | Kısa birikimi kaçırır | Rapor gösterim eşiği (dk). Simülasyon kuralı: `D_off >= min_idle_duration_for_report` ise rapor göster |
| `idle_boss_gold_multiplier` | 1.5 | 1.0–2.0 | 1.0 = boss farkı yok | Aktif boss avı değeri düşer | İdle boss katı altın çarpanı (aktif: 3.0) |
| `idle_boss_material_guaranteed` | true | true/false | Boss malzeme yok → idle boss farkı sadece altın | Her boss katı +1 malzeme → malzeme bolluğu | Boss katı garantili F tier malzeme |
| `min_idle_duration_for_guarantee` | 60 | 30–120 | 30 dk'da garanti → çok cömert | 120 dk = 2 saat bekleme → kısa oturumlarda boş ekran | İdle minimum hasat garantisi eşiği (dk) |
| `min_offline_for_pity` | 30 | 15–60 | Kısa oturumlarda pity birikir → exploit açığı | Uzun eşik → pity koruması geç devreye girer | Pity birikimi minimum offline süresi (dk, mikro-oturum koruması) |
| `idle_pity_increment` | 0.06 | 0.03–0.10 | Yavaş ramp → dry streak uzun | Hızlı ramp → canavar çok sık düşer | İdle pity artış hızı (aktif: 0.03) |
| `tier_2_multiplier` | 0.75 | 0.50–1.00 | 8+ saat oturumlar çok cezalandırılır | Azalan getiri etkisiz | 8-16 saat altın verimlilik çarpanı |
| `tier_3_multiplier` | 0.50 | 0.25–0.75 | 16+ saat oturumlar çok cezalandırılır | Azalan getiri etkisiz | 16-24 saat altın verimlilik çarpanı |

**Etkileşimler:**
- `idle_efficiency` ve `idle_loot_efficiency` birlikte otofarm'ın toplam değerini belirler. İkisi de yüksekse aktif oynama gereksizleşir.
- `idle_minutes_per_floor` hem kat sayısını hem loot rulosu sayısını belirler — **cascade etkisi**: 2.0'a ayarlandığında rulo sayısı ~2.5x artar, canavar/malzeme kazanımı orantılı yükselir.
- `idle_efficiency` Ekonomi GDD'deki knob'un referansıdır — kaynak: Ekonomi GDD.
- `idle_loot_efficiency` Loot GDD'deki knob'un referansıdır — kaynak: Loot GDD (aralık 0.10–0.40).
- `max_offline_minutes` üst sınırı 1440'ta tutulmalı — 2880 gibi değerler altın enflasyonu yaratır.
- `idle_pity_increment` × `min_offline_for_pity` birlikte pity davranışını belirler. Increment yüksek + eşik düşük = exploit riski; increment düşük + eşik yüksek = pity etkisiz.
- `min_idle_duration_for_guarantee` × `idle_minutes_per_floor` birlikte minimum hasat garantisinin tetiklenme sıklığını belirler. 60 dk / 5 dk = 12 kat minimum oturum.
- `idle_boss_gold_multiplier` × `idle_boss_material_guaranteed` birlikte idle boss katlarının ödül değerini belirler. İkisi de yüksekse aktif boss avı değeri düşer.

## Visual/Audio Requirements

[To be designed]

## UI Requirements

[To be designed]

## Acceptance Criteria

1. **GIVEN** koşullar sağlanmış (1+ kat, 1+ canavar) ve oyuncu uygulamayı kapatır, **WHEN** 480 dk sonra geri dönerse, **THEN** geri dönüş ekranı gösterilir ve `idle_floors_cleared = max(1, floor(480/5)) = 96` kat raporlanır.

2. **GIVEN** teamPower=3462 (active_gold_per_minute ≈ 55, idle_gold_per_minute ≈ 27.5) ve oyuncu 480 dk çevrimdışı, **WHEN** idle altın hesaplanırsa, **THEN** `idle_gold = 27.5 × 480 = 13.200 altın` (tamamı Tier 1).

3. **GIVEN** idle_gold_per_minute=25 ve oyuncu 960 dk (16 saat) çevrimdışı, **WHEN** idle altın hesaplanırsa, **THEN** `idle_gold = 25×480 + 25×0.75×480 = 12.000 + 9.000 = 21.000 altın` (Tier 1 + Tier 2 azalan getiri).

4. **GIVEN** seeded RNG (seed=1719216000) ile 96 idle katı simülasyonu ve idle_pity_bonus=0.0, **WHEN** canavar ve malzeme ruloları yapılırsa, **THEN** sonuçlar `tests/fixtures/idle_golden_seed_1719216000.json` dosyasındaki beklenen çıktıyla birebir eşleşir (canavar sayısı, nadirlikler, malzeme sayısı, elementler). Golden-file referans implementasyondan üretilir ve CI'da deterministik assert edilir.

5a. **GIVEN** idle kademe ağırlık tablosu (F: 0.700, D: 0.240, C: 0.050, B: 0.008, A: 0.002, S: 0.001; SS idle'da düşmez), **WHEN** ağırlıklar toplanırsa, **THEN** toplam = 1.001 (±0.002) ve her ağırlık spec değeriyle eşleşir. (Deterministik unit test — CI blocking.)

5b. **GIVEN** 100.000 idle katı simülasyonu (offline istatistiksel doğrulama), **WHEN** canavar kademe dağılımı incelenirse, **THEN** gözlemlenen oranlar idle kademe tablosuna uyar: F ~%69.9, D ~%24.0, C ~%5.0, B ~%0.80, A ~%0.20, S ~%0.10 (chi-squared p > 0.001). (Offline advisory test — CI-blocking değil.)

6. **GIVEN** seeded RNG ile 96 idle katı simülasyonu, **WHEN** malzeme ruloları yapılırsa, **THEN** sonuçlar deterministik olarak tekrarlanabilir ve bölge elementi malzemelerin ~%70'ini oluşturur, diğer 3 element ~%10'ar pay alır.

6b. **GIVEN** herhangi bir geçerli idle oturumu (1-288 kat), **WHEN** tüm kat ruloları tamamlanırsa, **THEN** ReturnReport'ta XP İksiri veya Elmas tipinde hiçbir item bulunmaz — idle loot tablosunda bu tipler tanımlı değildir.

7. **GIVEN** oyuncu 0 kat geçmiş (Locked durumda), **WHEN** uygulamayı kapatıp 8 saat sonra açarsa, **THEN** idle birikimi hesaplanmaz ve geri dönüş raporu gösterilmez.

8. **GIVEN** oyuncunun aktif takımı boş (0 canavar), **WHEN** uygulamayı kapatıp geri dönerse, **THEN** idle başlamamıştır (Locked), birikim yok.

9a. **GIVEN** otofarm Idle durumda (start_time=T0) ve 120 dk geçmiş, **WHEN** oyuncu aktif zindan girişine basarsa, **THEN** idle birikim 120 dk üzerinden hesaplanır: 24 kat, idle_gold = idle_gold_per_minute × 120.

9b. **GIVEN** kısmi birikim hesaplanmış, **WHEN** rapor oluşturulursa, **THEN** geri dönüş ekranı modal olarak gösterilir — arka planda navigasyon mümkün değildir.

9c. **GIVEN** modal rapor açık, **WHEN** "Topla" butonuna basılırsa, **THEN** ödüller envantere eklenir ve pending_report = null yapılır.

9d. **GIVEN** aktif zindan oturumu bitmiş ve otofarm koşulları sağlanıyorsa, **WHEN** ana ekrana dönülürse, **THEN** idle_state.active = true ve start_time = oturum bitiş zamanı olarak kaydedilir.

10. **GIVEN** 30 saat çevrimdışı kalınmış, **WHEN** birikim hesaplanırsa, **THEN** tüm formüller (altın, kat, canavar, malzeme) yalnızca 1440 dk üzerinden hesaplama yapar.

11. **GIVEN** geri dönüş ekranında goldEarned=5000, monstersEarned=[F tier×2], materialsEarned=[Ateş×1], **WHEN** "Topla" butonuna basılırsa, **THEN** (1) oyuncu altın bakiyesi +5000 artar, (2) envantere 2 F tier canavar eklenir, (3) envantere 1 Ateş malzemesi eklenir, (4) idle_state.pending_report = null olur, (5) save tetiklenir.

12. **GIVEN** idle'da canavar düşer, envanter 20/20 dolu ve bekleme alanı 8/10, **WHEN** "Topla" basılırsa, **THEN** canavarlar bekleme alanına alınır ve "Envanter dolu! Beklemede X canavar" bildirimi gösterilir.

13. **GIVEN** idle'da 3 canavar düşer, envanter 20/20 dolu ve bekleme alanı 9/10, **WHEN** "Topla" basılırsa, **THEN** 1 canavar bekleme alanına alınır (10/10), kalan 2 canavar otomatik satılır, satış altını eklenir ve raporda "otomatik satıldı" notu gösterilir.

14. **GIVEN** geri dönüş raporu gösterilirken uygulama kapanırsa, **WHEN** bir sonraki açılışta, **THEN** aynı rapor tekrar gösterilir (pending_report save'den okunur), ödüller hâlâ eklenmemiştir ve "Topla" ile normal toplanır.

15. **GIVEN** oyuncu ilk zindan katını geçer ve takımda canavar var, **WHEN** Locked → Idle geçişi gerçekleşirse, **THEN** "Otofarm açıldı!" bildirimi gösterilir.

16. **GIVEN** idle aktif, **WHEN** cihaz saati ileri alınırsa, **THEN** Kaydetme/Yükleme `offline_duration = 0` döndürür ve idle birikimi sıfırdır.

17a. **GIVEN** idle_pity_bonus = 0.0, offline_duration >= 30 dk ve seeded RNG ile 8 ardışık katta canavar düşmez, **WHEN** pity_bonus hesaplanırsa, **THEN** idle_pity_bonus = min(8 × 0.06, 0.45) = 0.45.

17b. **GIVEN** idle_pity_bonus = 0.45 (tavan), **WHEN** bir sonraki idle katında canavar düşmezse, **THEN** idle_pity_bonus = 0.45 kalır (artmaz).

17c. **GIVEN** idle_pity_bonus = 0.45 (tavan), **WHEN** bir sonraki katta canavar düşerse, **THEN** toplam şans %3.75 + %45 = %48.75 idi, canavar düştü ve idle_pity_bonus = 0.0'a sıfırlanır.

17d. **GIVEN** önceki idle oturumunda idle_pity_bonus = 0.30 birikmiş ve canavar düşmemiş, **WHEN** yeni idle oturumu başlarsa, **THEN** idle_pity_bonus = 0.30 olarak devam eder (oturum sıfırlaması yok).

18. **GIVEN** idle_efficiency = 0.50 ve active_gold_per_minute = 20, **WHEN** idle_gold_per_minute hesaplanırsa, **THEN** sonuç = 20 × 0.50 = 10.

19. **GIVEN** offline_duration = 0, **WHEN** uygulama açılırsa, **THEN** geri dönüş raporu gösterilmez ve doğrudan ana menüye geçilir.

20. **GIVEN** koşullar sağlanmış, **WHEN** uygulama kapanırsa (OnApplicationPause), **THEN** idle_state save'e yazılır: active=true, start_time=now, floor=en yüksek kat, region=aktif bölge, team_power=GetActiveTeamPower(), idle_pity_bonus=mevcut değer.

21. **GIVEN** idle_gold_per_minute=25 ve offline_duration=1440 dk (24 saat), **WHEN** idle altın hesaplanırsa, **THEN** idle_gold = 25×480 + 25×0.75×480 + 25×0.50×480 = 12.000 + 9.000 + 6.000 = 27.000 altın (tüm 3 kademe).

22. **GIVEN** idle_minutes_per_floor config'de 1.0 (güvenli aralık dışı), **WHEN** idle_floors_cleared hesaplanırsa, **THEN** M_floor = 3.0 olarak clamp edilir ve division-by-zero oluşmaz.

23. **GIVEN** idle_state.start_time = 0 (bozuk) veya gelecek tarihi, **WHEN** uygulama açılırsa, **THEN** idle birikim hesaplanmaz, idle_state Idle'a sıfırlanır, hata loglanır.

24. **(AC 4 ile birleştirildi — golden-file yaklaşımı determinizm ve doğruluğu birlikte test eder.)**

25. **GIVEN** otofarm Idle durumda, oyuncu aktif oturumda son canavarını satarsa, **WHEN** uygulama kapanırsa, **THEN** idle başlamaz (Locked'a döner), idle_state.active = false.

26. **GIVEN** idle farm katı boss katı (ör. kat 25, her 5. kat) ve idle aktif, **WHEN** idle loot ruloları yapılırsa, **THEN** (1) altın oranı 1.5x uygulanır (idle_boss_gold_multiplier), (2) 1 F tier malzeme garantili eklenir (bölge elementinde), (3) boss canavar düşüşü yapılmaz, (4) normal idle canavar loot tablosu uygulanır.

27. **GIVEN** offline_duration = 90 dk (>= 60 dk eşiği), **WHEN** idle simülasyonu tamamlanırsa, **THEN** ReturnReport'ta stokastik rulo sonuçlarına ek olarak en az 1 F tier canavar ve 1 F tier malzeme (bölge elementinde) bulunur (idle minimum hasat garantisi).

28. **GIVEN** offline_duration = 45 dk (< 60 dk eşiği), **WHEN** idle simülasyonu tamamlanırsa, **THEN** yalnızca stokastik rulo sonuçları verilir — garantili minimum canavar veya malzeme eklenmez.

29. **GIVEN** offline_duration = 20 dk (< 30 dk min_offline_for_pity), idle_pity_bonus = 0.0 ve hiç canavar düşmez, **WHEN** pity hesaplanırsa, **THEN** idle_pity_bonus = 0.0 kalır (artmaz — mikro-oturum koruması).

30. **GIVEN** teamPower = 60 (minimum), **WHEN** active_gold_per_minute hesaplanırsa, **THEN** sonuç = 10. **GIVEN** teamPower = 6864 (maximum), **THEN** sonuç = 100. **GIVEN** teamPower = 30 (clamp altı), **THEN** sonuç = 10 (60'a clamp edilir).

## Open Questions

1. ~~**İdle Hız Çarpanı**: Takım gücü arttıkça idle kat geçme hızı da artmalı mı?~~ → **Deferred to Tier 2.** MVP'de `idle_minutes_per_floor` sabit 5.0. Tier 2'de `idle_minutes_per_floor = base / log(teamPower)` gibi bir formül değerlendirilebilir.
2. **Çoklu Bölge İdle**: Tier 2+'da birden fazla zindan bölgesi açıldığında, idle farklı bölgelerde aynı anda çalışabilmeli mi?
3. **İdle Boost**: Reklam izleyerek veya elmas harcayarak idle verimi geçici olarak %100'e çıkarma özelliği eklenecek mi? (Tier 2 monetizasyon kararı)
4. **Bildirim Zamanlaması**: Tier 2'de push bildirim eklendiğinde, "birikim hazır" bildirimi ne kadar agresif olmalı?

## Cross-GDD Updates — Status

Design-review revizyon geçişinde aşağıdaki cross-GDD güncellemeleri uygulandı:

1. ✅ **Ekonomi GDD** (`design/gdd/ekonomi.md`): `teamPower → active_gold_per_minute` formülü eklendi (Formül 3). İdle altın formülü azalan getirili versiyona güncellendi (Formül 4). Çıktı aralığı 54.000'e güncellendi.

**3. Review (2026-06-24) — Ek cross-GDD güncellemeleri:**

5. ✅ **Loot GDD** (`design/gdd/loot-odul-sistemi.md`): Formül 8 idle altın formülü azalan getirili kademeli formüle güncellendi. "Yalnızca F/D" kısıtlaması kaldırıldı — tüm kademeler idle'da düşebilir (idle kademe tablosu). AC 14 güncellendi. Formül 3b idle pity increment 0.03→0.06, min_offline_for_pity=30dk eklendi.
6. ✅ **Registry** (`design/registry/entities.yaml`): `idle_floors_cleared_formula` output_range [1,288]→[1,480] güncellendi (M_floor=3.0 minimum clamp'e uyumlu).
2. ✅ **Loot GDD** (`design/gdd/loot-odul-sistemi.md`): İdle-pity kuralları eklendi (Formül 3b). Float accumulator, oturumlar arası kalıcı, idle nadirlik tablosu. Aktif/idle pity farkları belgelendi.
3. ✅ **Kaydetme / Yükleme GDD** (`design/gdd/kaydetme-yukleme.md`): `idle_state` şeması genişletildi — `team_power`, `idle_pity_bonus`, `pending_report` alanları eklendi.
4. ✅ **Registry** (`design/registry/entities.yaml`): `idle_gold_formula` azalan getirili güncellendi, `idle_floors_cleared_formula` max(1,...) eklendi, `team_power_formula` / `base_monster_rate` / `monster_pity_increment` / `offline_duration_formula` referenced_by'larına otofarm eklendi.
