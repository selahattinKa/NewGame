# Savaş Sistemi

> **Status**: Revision Needed → In Progress
> **Author**: user + game-designer, gameplay-programmer, systems-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Hisset, Senin Tempon, Cömert Zindan

## Overview

**Savaş Sistemi**, oyundaki tüm savaş etkileşimlerini yöneten tur bazlı cooldown savaş döngüsüdür. Mekanik olarak, oyuncu (seçilmiş sınıfıyla) ve 4 kişilik pet takımı ile düşman grubunu SPD stat'ına göre sıralanan tur sisteminde karşı karşıya getirir. Her tur sırası gelen birim Hasar Hesaplama üzerinden saldırır, Sağlık/Can Sistemi HP'yi günceller, Düşman AI düşman kararlarını üretir ve döngü bir taraf tamamen savaş dışı kalana veya oyuncu çekilene kadar devam eder.

Sistemin iki ayrı yetenek katmanı vardır: **Oyuncu Sınıf Yetenekleri** (4 slot, cooldown bazlı — Slot 0 anında, Slot 1-3 sırasıyla CD3/CD5/CD8) ve **Pet Yetenekleri** (arketip başına 1 aktif yetenek, enerji bazlı — her 4 turda bir). İki katman birbirinden bağımsız çalışır; oyuncu sınıf yeteneği kullandığında petlerin enerji birikimi durmaz.

Sistem iki mod sunar — **Komutan Modu** (oyuncu yetenek zamanlamasını ve hedef seçimini kontrol eder, +10% ATK bonusu kazanır, sınıf yeteneği zamanlaması oyuncuya ait) ve **Otofarm Modu** (tüm kararlar otomatik alınır, sınıf yeteneği CD dolunca anında kullanılır). Her iki modda da her savaş sonu loot düşer — "Cömert Zindan" sütunu gereği eli boş dönen savaş yoktur.

MVP kapsamında tur döngüsü, iki mod, SPD sıralama, pet enerji sistemi, oyuncu sınıf cooldown sistemi, DoT (Yanma/Zehir), temel status efektleri (Sersemletme, Kalkan, DEF Kırma, ATK Zayıflatma) ve savaş sonu ödül dağıtımı yer alır.

## Player Fantasy

Oyuncu savaş sisteminde **güçlü komutan** fantezisi yaşar. Çekirdek an, canavarlarının otomatik savaştığını izlerken doğru anda yetenek butonuna basıp düşmanı element avantajlı bir saldırıyla devirmektir — "benim zamanlamam fark yarattı" hissi. Komutan modunda oyuncu ordusunun generalıdır: kimlerin ne zaman saldıracağını yönetmez (otomatik), ama kritik anlarda müdahale eder — düşük canavar iyileştirilir, boss'un güçlü saldırısı öncesi savunma tetiklenir, element avantajı olan canavar doğru hedefe yönlendirilir. Bu müdahalelerin karşılığı somut: %20-30 güç bonusu, daha az tur, daha az kayıp.

**Büyüme tatmini**: Aynı zindan katını geçen hafta 8 turda geçerken bu hafta 4 turda geçmek — hasar sayıları büyümüş, HP barları daha az kıpırdıyor, düşmanlar daha hızlı eriyor. "Ne kadar güçlendim" anı savaş sisteminde en belirgin şekilde hissedilir.

**Otofarm fantezisi**: Ordun senin yerine savaşıyor. Geri döndüğünde sandıklar seni bekliyor. "İmparatorluğum bensiz bile çalışıyor" rahatlığı — ama sen müdahale edince %20-30 daha iyi çalışıyor.

**Negatif fantazi (kaçınılacak)**: Savaş asla "bekle ve izle" monotonluğuna düşmemeli. Komutan modunda anlamlı müdahale anları olmalı. Otofarm ise zaten "izlemiyorum" modudur — bu kabul edilebilir. Savaşlar çok uzun sürmemeli — 30 saniyede bir savaş (5-8 tur) ideal. Kayıp bile cezalandırıcı olmamalı — "Cömert Zindan" gereği kaybetsen bile enerji harcanmaz, sadece o kattan loot alamazsın.

**Pillar bağlantısı**: "Güç Hisset" — komutan modu güç bonusu + büyüyen hasar sayıları. "Senin Tempon" — komutan/otofarm seçimi oyuncunun ritmini belirler. "Cömert Zindan" — her savaş loot düşürür, kayıp bile cezasız.

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Rules

### Core Rules

**Kural 1 — Savaş Akışı (Combat Flow)**

Her savaş üç aşamadan oluşur:

```
Savaş Öncesi (Pre-Combat) → Aktif Savaş (Combat) → Savaş Sonu (Post-Combat)
```

1. **Savaş Öncesi**: Takım Kurma'dan aktif takım yüklenir (`GetActiveTeam()`). Düşman grubu Zindan Keşif'ten alınır (`GetFloorEnemies()`). Element sinerjileri hesaplanır. Mod seçimi yapılır (varsayılan: son kullanılan mod). Tüm birimler `current_hp = max_hp`, `energy = 0` ile başlar.

2. **Aktif Savaş**: Tur döngüsü başlar (Kural 2-3). Döngü, tüm düşmanlar savaş dışı (kazanma) veya tüm oyuncu canavarları savaş dışı (kaybetme) olana kadar devam eder. Oyuncu istediği zaman çekilebilir.

3. **Savaş Sonu**:

| Sonuç | Loot | Enerji Maliyeti | Ceza | HP |
|-------|------|-----------------|------|----|
| **Kazanma** | Tam ödül (Loot/Ödül Sistemi) | Harcanır | Yok | Tam iyileşme |
| **Kaybetme** | Yok | Harcanmaz | Yok | Tam iyileşme |
| **Çekilme** | Yok | Harcanmaz | Yok | Tam iyileşme |

"Cömert Zindan" gereği: kaybetme/çekilme cezalandırıcı değil. Zindan enerjisi sadece kazanınca harcanır.

**Kural 2 — Tur Sıralama Sistemi (SPD-Based Turn Order)**

Her raunt'ta tüm hayatta birimler SPD stat'ına göre azalan sırayla hareket eder:

1. Tüm hayatta birimler (oyuncu + düşman) SPD'ye göre sıralanır
2. En yüksek SPD → ilk sıra
3. SPD eşitliğinde: oyuncu canavarı öncelikli. Hâlâ eşitse: takım slot sırası
4. Her birim sırasında 1 aksiyon yapar (Kural 3)
5. Tüm birimler aksiyonunu tamamlayınca raunt biter, yeni raunt başlar
6. Raunt başında savaş dışı birimler sıradan çıkarılır
7. Boss öfke modunda SPD değişimi bir sonraki raunt'ta yansır (Düşman AI GDD ile tutarlı)

**Kural 3 — Birim Tur Fazları**

Her birimin turu 5 fazda işler:

```
1. DoT Tick          → Aktif DoT'lar uygulanır (Yanma/Zehir — Kural 10)
2. Pasif Rejenerasyon → max(1, floor(max_hp × 0.02)) HP iyileşme
3. Enerji Birikimi   → energy += energy_per_turn (25) — pet için
4. Aksiyon Seçimi    → Normal saldırı VEYA Yetenek kullanımı
5. Aksiyon Yürütme   → Hasar/iyileşme/buff uygulama + animasyon
```

- Faz 1: DoT hasarı rejenarasyondan önce gelir — regen hasarı kısmen telafi edebilir
- Faz 1'de birim HP=0'a düşerse savaş dışı kalır, kalan fazlar atlanır
- Faz 2-3 otomatik, mod fark etmez
- Faz 4 moda göre değişir (Kural 4-5); Sersemletme aktifse Faz 4 atlanır
- Faz 5 sonrası savaş dışı kontrolü: hedef HP=0 ise savaş dışı tetiklenir
- Tüm fazlar sıralıdır
- Cooldown sayacı (oyuncu sınıf yetenekleri) Faz 5 sonunda 1 azalır

**Kural 4 — Komutan Modu (Commander Mode)**

Oyuncunun aktif katılımla savaşa müdahale ettiği mod.

**Flat ATK Bonusu**: Tüm oyuncu canavarlarına +10% ATK:
`commander_ATK = floor(effective_ATK × 1.10)`

DEF, SPD, HP'ye bonus yok — sadece ATK. Mod değiştirilirse bonus bir sonraki turdan düşer.

**Oyuncu Müdahale Noktaları**:

| Müdahale | Mekanik | Verimlilik Etkisi |
|----------|---------|-------------------|
| **Hedef Seçimi** | Düşmana dokunarak öncelikli hedef belirle | Element avantajlı hedef seçmek hasar artırır (+50%) |
| **Yetenek Zamanlaması** | Yetenek butonuna basarak optimal anda kullan | Boss saldırısı öncesi Tank koruması, düşük HP'li düşmana güçlü vuruş |
| **Savaş Hızı** | 1x / 2x / 3x toggle | Hesaplamaya etki yok, animasyon hızı |

**Hedef önceliklendirme**: Oyuncu bir düşmana dokunduğunda tüm oyuncu canavarları o hedefe yönelir (hedef savaş dışı olana veya oyuncu değiştirene kadar). Hedef seçilmemişse varsayılan: en yüksek HP oranına sahip düşman.

**Yetenek bekleme**: Yetenek butonu energy=100'de aktifleşir. Oyuncu butona basmadıkça yetenek kullanılmaz — normal saldırı devam eder. Oyuncu yeteneği stratejik olarak "doğru an" için saklayabilir.

**Kural 5 — Otofarm Modu (Auto-Farm Mode)**

Tüm kararlar otomatik, oyuncu müdahalesi yok.

**ATK Bonusu**: Yok. Sinerji dahil base effective_ATK kullanılır.

**Otomatik Hedef Seçimi** (Basit AI ağırlıkları):
- Rastgele: %60
- En düşük HP oranı: %25
- En yüksek ATK (tehdit): %15

**Otomatik Yetenek Kullanımı**: Energy=100 olduğu anda yetenek hemen kullanılır. Hedef seçimi otomatik:
- Saldırgan → en düşük HP düşman
- Tank → kendisi
- Destekçi → en düşük HP% takım arkadaşı
- Büyücü → AoE (hedef seçimi yok)

**Verimlilik Farkı (Komutan vs Otofarm)**:

| Fark Kaynağı | Komutan Avantajı | Tahmini Etki |
|-------------|------------------|-------------|
| Flat +10% ATK bonusu | Her saldırıda | +10% |
| Hedef seçimi | Element avantajlı hedef | +5-10% |
| Yetenek zamanlaması | Optimal an bekleme | +5-10% |
| **Toplam** | | **~20-30%** |

**Kural 6 — Yetenek Sistemi (MVP)**

Her canavar arketipine göre 1 aktif yetenek taşır.

**Enerji Mekanizması**:
- Savaş başlangıcı: `energy = 0`
- Her tur başı: `energy += energy_per_turn` (25)
- Yetenek eşiği: `energy >= energy_threshold` (100)
- Kullanım sonrası: `energy = 0`
- Enerji 100'ü aşmaz (birikim yok)
- Yetenek VEYA normal saldırı — aynı turda ikisi birden kullanılamaz

**Arketip Yetenekleri**:

| Arketip | Yetenek Adı | Etki | Hedef | Çarpan/Oran | Hasar Türü |
|---------|-------------|------|-------|-------------|-----------|
| **Saldırgan** | Güçlü Vuruş | Tek hedefe yüksek hasar | Tek düşman | ATK × `skill_atk_multiplier` (2.0) | Fiziksel (`defense_reduction_factor=2`) |
| **Tank** | Koruma Duruşu | Kendi DEF'ini artırır | Kendisi | DEF × `skill_def_multiplier` (2.0), `skill_buff_duration` (2) tur | — (hasar yok) |
| **Destekçi** | İyileştirme | Takım arkadaşını iyileştirir | En düşük HP% ally | Hedef max_hp × `healer_skill_rate` (0.20) | — (iyileştirme) |
| **Büyücü** | Element Dalgası | Tüm düşmanlara AoE hasar | Tüm düşmanlar | ATK × `skill_aoe_multiplier` (0.75), her hedefe ayrı pipeline | **Büyü** (`magic_defense_factor=4`) |

**Yetenek hasar hesaplama** (Saldırgan/Büyücü): Hasar Hesaplama pipeline'ını kullanır, Adım 1'de `effective_ATK` yerine `effective_ATK × skill_multiplier` geçer. `defense_reduction_factor` arketip hasar türüne göre belirlenir:

`skill_damage = max(1, floor(effective_ATK × skill_multiplier - floor(effective_DEF / defense_reduction_factor)) × element_multiplier × [crit])`

- Saldırgan: `defense_reduction_factor = 2` (fiziksel)
- Büyücü: `magic_defense_factor = 4` (büyü — DEF daha az etkili)

Tank ve Destekçi yeteneği hasar pipeline'ı kullanmaz — doğrudan buff/iyileşme uygular.

**Düşman AI ile tutarlılık**: Oyuncu ve düşman canavarları aynı yetenek setini kullanır. Düşman AI GDD'sindeki mini-boss yetenek çarpanları (Saldırgan 1.5x, Büyücü 0.5x, Tank DEF×2) düşman dengelemesine aittir; bu GDD oyuncu tarafının çarpanlarını tanımlar.

**Kural 7 — Oyuncu Sınıf Yetenek Sistemi (Cooldown)**

Oyuncu sınıfı (Savaşçı/Büyücü/Hırsız/Şifacı), pet yetenek sisteminden bağımsız 4 yetenek slotuna sahiptir.

**Slot yapısı**:

| Slot | Cooldown | Açıklama |
|------|----------|----------|
| Slot 0 | CD0 | Her tur kullanılabilir — temel saldırı / temel eylem |
| Slot 1 | CD3 | 3 tur bekleme — orta güç yetenek |
| Slot 2 | CD5 | 5 tur bekleme — güçlü yetenek |
| Slot 3 | CD8 | 8 tur bekleme — ultimate |

**Cooldown mekanizması**:
- Savaş başlangıcı: tüm CD'ler 0 (tüm yetenekler açık)
- Yetenek kullanıldığında: `current_cd = slot_cd` değeri set edilir
- Her tur sonu (Faz 5 sonrası): `current_cd = max(0, current_cd - 1)`
- Kullanılabilir koşul: `current_cd == 0`

**Komutan modunda seçim**:
- Oyuncu Faz 4'te kullanılabilir slotlardan birini seçer
- Seçilmezse Slot 0 (CD0) otomatik kullanılır — Slot 0 her zaman açık
- Komutan modu yetenek butonları: sadece `current_cd == 0` olanlar aktif

**Otofarm modunda seçim** (öncelik sırası):
1. Slot 3 açıksa → Slot 3
2. Slot 2 açıksa → Slot 2
3. Slot 1 açıksa → Slot 1
4. Her zaman → Slot 0

**Pet yeteneği ile aynı turda**: Oyuncu sınıf yeteneği ile pet yeteneği aynı turda kullanılamaz — oyuncunun turu tek aksiyon. Pet'lerin turları ayrıdır (SPD sıralamasına göre).

**Sınıf yetenek tablosu (özet)** — ayrıntılar Oyuncu Sınıf Sistemi GDD'sinde:

| Sınıf | Slot 0 | Slot 1 (CD3) | Slot 2 (CD5) | Slot 3 (CD8) |
|-------|--------|--------------|--------------|--------------|
| Savaşçı | Normal saldırı | Sersemletme + hasar | Taunt (aggro çek) | AoE fiziksel |
| Büyücü | Büyü saldırısı | Hafif büyü | Ağır büyü + Yanma DoT | Büyü AoE |
| Hırsız | Normal saldırı | Zehir + hasar | Kaçınma duruşu | 5 vuruş combo |
| Şifacı | Hafif büyü | İyileştirme | Diriliş | Takım ATK buff |

---

**Kural 8 — DoT Sistemi (Damage over Time)**

DoT efektleri hasar pipeline'ından bağımsızdır — DEF'i bypass eder, doğrudan HP düşürür.

**DoT türleri**:

| Tip | Kaynak | Oran | Süre | Uygulama |
|-----|--------|------|------|----------|
| **Yanma** | Büyücü Slot 2 | max_hp × 0.05 / tur | 3 tur | Hedef birim tur başı (Faz 1) |
| **Zehir** | Hırsız Slot 1 | max_hp × 0.04 / tur | 4 tur | Hedef birim tur başı (Faz 1) |

**Uygulama kuralları**:
- `dot_damage = max(1, floor(target_max_hp × dot_rate))`
- DoT, rejenerasyondan (Faz 2) önce gelir — regen kısmen dengeleyebilir
- Aynı tipten DoT stack'lenmez: tekrar uygulanırsa süre yenilenir, hasar çarpılmaz
- Farklı tipler (Yanma + Zehir) aynı anda aktif olabilir — her tur ikisi de uygulanır
- DoT ekleyen yetenek isabet etmezse (miss yoksa her zaman uygular) DoT de uygulanır
- DoT uygulanan birim savaş dışı kalırsa DoT sona erer

**Boss bağışıklığı**: Boss ve mini-boss Sersemletme'ye bağışıklıdır (Kural 9). DoT'a bağışıklık yok — Yanma ve Zehir boss'lara uygulanır.

---

**Kural 9 — Status Efektleri**

Savaş sırasında birimlere uygulanabilen geçici durum değişiklikleri:

| Efekt | Kaynak | Etki | Süre | Boss Bağışıklığı |
|-------|--------|------|------|-----------------|
| **Sersemletme** | Savaşçı Slot 1 | Hedef Faz 4'ü atlar (aksiyon yapamaz) | 1 tur | Evet (boss + mini-boss) |
| **DEF Kırma** | Savaşçı Slot 2 | Hedef DEF × 0.70 | 2 tur | Hayır |
| **ATK Zayıflatma** | Şifacı Slot 3 | Hedef ATK × 0.80 | 2 tur | Hayır |
| **Kalkan** | Savaşçı Slot 2 / Şifacı | Hasar absorbe eder (max_hp × 0.25) | 3 tur veya dolana dek | Hayır |
| **Kesin Kritik** | Şifacı Slot 3 | Sonraki saldırı garantili kritik | 1 kullanım | Hayır |
| **Hasar Azaltma** | Savaşçı Slot 1 / Şifacı | Alınan hasar × 0.75 | 2 tur | Hayır |

**Uygulama kuralları**:
- Aynı tip status efekti stack'lenmez — tekrar uygulanırsa süre yenilenir
- Farklı status efektleri (DEF Kırma + ATK Zayıflatma) aynı anda aktif olabilir
- Kalkan aktifken gelen hasar önce Kalkan'ı tüketir; Kalkan bitince HP düşer
- Kalkan kapasitesi: `shield_hp = floor(target_max_hp × 0.25)`. Hasar kapasiteyi aştığında kalan hasar HP'ye uygulanır
- Sersemletme uygulandığında hedefin mevcut turu o tur yoksa sonraki turda işler
- DEF Kırma ve ATK Zayıflatma Hasar Hesaplama pipeline'ına girmeden önce stat'ı değiştirir (effective_DEF/ATK üzerine uygulanır)
- Status efektleri süre sayacı: her UnitTurnEnd'de 1 azalır (sahibinin turunda değil, etkilenen birimin turunda)

---

**Kural 10 — Mod Geçişi**

- Savaş öncesinde mod seçilir (varsayılan: son kullanılan)
- Savaş sırasında mod toggle edilebilir (tek dokunuş)
- Değişiklik bir sonraki turdan geçerli (mevcut tur eski modla biter)
- Komutan → Otofarm: ATK bonusu düşer, yetenek butonları kaybolur
- Otofarm → Komutan: ATK bonusu aktifleşir, yetenek butonları belirir
- Mod değişikliği sınırsız
- Cooldown sayaçları mod değişiminden etkilenmez

**Kural 11 — Savaş Hız Kontrolü**

| Hız | Tur Süresi (animasyon dahil) | Kullanım |
|-----|------------------------------|----------|
| 1x | ~2 saniye/tur | İlk savaşlar, savaşı izlemek |
| 2x | ~1 saniye/tur | Normal oyun |
| 3x | ~0.5 saniye/tur | Farming, tekrarlı katlar |

- Hız yalnızca animasyon süresini etkiler — hesaplama, formül, sonuç değişmez
- Komutan modunda yetenek bekleme süresi hıza göre ölçeklenmez — oyuncu istediği kadar bekleyebilir
- Hız tercihi kaydedilir (son seçim persist)
- Otofarm modunda hız genellikle 3x'te bırakılır

### States and Transitions

**Üst Düzey Savaş Durumları**

| Durum | Açıklama | Giriş Tetikleyici | Çıkış |
|-------|----------|-------------------|-------|
| **PreCombat** | Takım yükleme, mod seçimi, sinerji hesaplama | Savaş başlatma komutu | → Combat |
| **Combat** | Aktif tur döngüsü çalışıyor | PreCombat tamamlanınca | → Victory / Defeat / Retreat |
| **Victory** | Tüm düşmanlar savaş dışı | Son düşman HP=0 | → PostCombat |
| **Defeat** | Tüm oyuncu canavarları savaş dışı (TPK) | Son oyuncu canavarı HP=0 | → PostCombat |
| **Retreat** | Oyuncu savaştan çekildi | "Çekil" butonu | → PostCombat |
| **PostCombat** | Loot dağıtımı, iyileşme, sonuç ekranı | Victory/Defeat/Retreat | → Zindan Keşif'e döner |

```
PreCombat → Combat ──→ Victory  ──→ PostCombat
                   ├──→ Defeat   ──→ PostCombat
                   └──→ Retreat  ──→ PostCombat
```

**Combat İçi Tur Döngüsü**

```
RoundStart → [her birim için SPD sırasıyla]:
  UnitTurnStart → DoTPhase → RegenPhase → EnergyPhase → DecisionPhase → ActionPhase → ResolutionPhase → UnitTurnEnd
→ RoundEnd → (kazanma/kaybetme kontrolü) → RoundStart (veya Victory/Defeat)
```

| Faz | Ne Olur | Süre |
|-----|---------|------|
| **RoundStart** | Sıra listesi SPD'ye göre güncellenir, savaş dışı birimler çıkarılır | Anında |
| **UnitTurnStart** | Aktif birim belirlenir, UI vurgulaması | ~0.2s |
| **DoTPhase** | Aktif DoT'lar (Yanma/Zehir) uygulanır; HP=0 ise savaş dışı → UnitTurnEnd | Anında (VFX: 0.3s) |
| **RegenPhase** | Pasif rejenerasyon uygulanır (Sağlık/Can GDD) | Anında (VFX: 0.3s) |
| **EnergyPhase** | energy += 25, enerji barı güncellenir (pet için) | Anında (VFX: 0.2s) |
| **DecisionPhase** | Mod'a göre aksiyon belirlenir; Sersemletme aktifse atlanır | Komutan: oyuncu girdisi bekler. Otofarm/Düşman/Stun: anında |
| **ActionPhase** | Saldırı/yetenek animasyonu + hasar/iyileşme hesaplama | 0.5-1.0s (hıza göre) |
| **ResolutionPhase** | HP güncelleme, savaş dışı kontrolü | Anında |
| **UnitTurnEnd** | Cooldown sayaçları -1, status efekt süreleri -1, sonraki birime geç | Anında |
| **RoundEnd** | Kazanma/kaybetme kontrolü — savaş devam mı? | Anında |

**Mod Durumları**

```
CommanderMode ←──(toggle)──→ AutoFarmMode
```

Mod değişikliği her iki yönde sınırsız, bir sonraki turdan geçerli.

**Status Efekti ve Buff Durum İzleme**

Savaş sırasında aktif efektler tur bazlı süre ile izlenir:

| Efekt | Başlangıç | Süre Azaltma | Bitiş |
|-------|-----------|-------------|-------|
| Koruma Duruşu — DEF×2 (pet Tank) | Yetenek kullanımında | Her UnitTurnEnd'de -1 | Süre=0 → DEF eski değere |
| Yanma DoT | Büyücü Slot 2 uyguladığında | Her DoTPhase'de -1 tick | Kalan tur=0 → sona erer |
| Zehir DoT | Hırsız Slot 1 uyguladığında | Her DoTPhase'de -1 tick | Kalan tur=0 → sona erer |
| Sersemletme | Savaşçı Slot 1 uyguladığında | Her UnitTurnEnd'de -1 | Süre=0 → aksiyon normal |
| DEF Kırma | Savaşçı Slot 2 uyguladığında | Her UnitTurnEnd'de -1 | Süre=0 → DEF normal |
| Kalkan | Şifacı/Savaşçı uyguladığında | Hasar gelince kapasiteden düş | Kapasite=0 veya süre=0 |
| ATK Zayıflatma | Şifacı Slot 3 uyguladığında | Her UnitTurnEnd'de -1 | Süre=0 → ATK normal |

Stack kuralı: aynı tip efekt stack'lenmez, süre yenilenir. Farklı tipler aynı anda aktif olabilir.

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Takım Kurma** | ← okur | Aktif takım (canavar ID'leri, slot sırası, effective stats) | `GetActiveTeam()` → [{monsterId, slot, effective_stats, element}] |
| **Canavar Veritabanı** | ← okur | Canavar kimliği (arketip, element), yetenek tanımı | `GetMonsterIdentity(monsterId)` → {element, archetype}; `GetSkillDef(archetype)` → {skillName, multiplier, targetType} |
| **Canavar Güçlendirme** | ← okur | Pipeline stat çıktısı (seviye + evrim + yıldız) | `GetEffectiveStats(monsterId)` → {hp, atk, def, spd} |
| **Element Sistemi** | ← okur | Element çarpanı, sinerji bonusları | `GetElementMultiplier(atkElement, defElement)` → float; `CalculateSynergy(teamElements[])` → {atk_bonus, def_bonus, spd_bonus} |
| **Hasar Hesaplama** | → tetikler | Saldırı komutu → hasar değeri döner | `CalculateDamage(attackerId, targetId, damageType)` → int; `EstimateDamage(attackerId, targetId, damageType)` → int |
| **Oyuncu Sınıf Sistemi** | ← okur | Oyuncunun `damageType` değeri (Büyücü/Şifacı=magic, Savaşçı/Hırsız=physical) | Savaş sistemi hasar türünü buradan alır |
| **Sağlık / Can Sistemi** | ↔ çift yönlü | HP durumu okur; hasar/iyileşme uygular | `IsAlive(monsterId)` → bool; `GetCurrentHP(monsterId)` → int; `GetHPRatio(monsterId)` → float; `TakeDamage(targetId, amount)`; `Heal(targetId, amount)`; `FullHeal(teamId)` |
| **Düşman AI** | ← okur | Düşman aksiyon kararları | `GetEnemyAction(enemyId)` → {actionType, targetId, skillId} |
| **Loot / Ödül Sistemi** | → tetikler | Savaş sonucu → loot dağıtımı | `DistributeLoot(battleResult, floorNumber)` |
| **Zindan Keşif** | ↔ çift yönlü | Düşman listesi alır; savaş sonucu bildirir | `GetFloorEnemies(floorNumber)` ←; `OnBattleComplete(result)` → |
| **Savaş UI** | → sağlar | Tüm savaş durumu, animasyon tetiklemeleri | `OnTurnStart`, `OnActionExecuted`, `OnBattleEnd`, `OnModeChanged` events |
| **Ekonomi** | dolaylı | Enerji harcama (zindan girişi Zindan Keşif üzerinden) | Doğrudan arayüz yok |

**Veri akışı özeti**: Takım Kurma + Canavar Veritabanı + Element Sistemi → savaş öncesi girdi. Düşman AI → düşman kararları. Hasar Hesaplama ↔ Sağlık/Can → savaş anı pipeline. Savaş sonucu → Loot + Zindan Keşif. UI'a sürekli event yayını. Bu sistem orkestratör — diğer sistemleri doğru sırayla çağırır, kendisi hesaplama yapmaz.

**Bidirectional check**:
- Hasar Hesaplama GDD: Hibrit Savaş → sert downstream ✓
- Sağlık/Can GDD: Hibrit Savaş → sert downstream ✓
- Takım Kurma GDD: Hibrit Savaş → sert downstream ✓
- Düşman AI GDD: Hibrit Savaş → sert downstream ✓
- Loot/Ödül GDD: `DistributeLoot(battleResult, floorNumber)` upstream eklendi ✓
- Zindan Keşif GDD: ✅ Yazıldı — `GetFloorEnemies` ve `OnBattleComplete` arayüzleri doğrulandı

*Specialist agents not consulted — Lean mode. Review manually before production.*

## Formulas

### Formül 1: Komutan ATK Bonusu (commander_ATK)

`commander_ATK = floor(effective_ATK × (1 + commander_atk_bonus))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Efektif ATK | effective_ATK | int | 15–599 | Sinerji dahil ATK (pipeline çıktısı + sinerji) |
| Komutan bonus oranı | commander_atk_bonus | float | 0.10 | Sabit %10 |
| Komutan ATK | commander_ATK | int | 16–658 | Komutan modunda kullanılan ATK |

**Çıktı Aralığı**: 16 (Common Tank Lv1, ATK=15) – 658 (Legendary ★5 Saldırgan + tam sinerji)

**Pipeline sırası**: pipeline_stat → sinerji → commander bonus → hasar formülü. Commander bonus element ve crit çarpanlarıyla da çarpılır.

**Örnek — Erken oyun**: Common Saldırgan (effective_ATK=35, sinerji yok)
→ commander_ATK = floor(35 × 1.10) = **38** (+3 ATK)

**Örnek — Orta oyun**: Rare Saldırgan Lv20 Form B (effective_ATK=117, 2'li sinerji dahil)
→ commander_ATK = floor(117 × 1.10) = **128** (+11 ATK)

**Not**: Flat +10% ATK, DEF azaltması sabit olduğundan efektif DPS artışı +12-19% arasındadır. DEF yüksek hedeflere karşı avantaj daha belirgin.

### Formül 2: Saldırgan Yeteneği — Güçlü Vuruş (skill_damage_attacker)

**Fiziksel hasar** (`defense_reduction_factor = 2`). ATK yerine `ATK × skill_atk_multiplier` girer:

```
1. boosted_ATK = floor(ATK_source × skill_atk_multiplier)
2. def_reduction = floor(target_DEF / 2)
3. base_damage = max(1, boosted_ATK - def_reduction)
4. element_damage = floor(base_damage × element_multiplier)
5. crit_damage = was_crit ? floor(element_damage × crit_multiplier) : element_damage
6. skill_damage = max(1, crit_damage)
```

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| ATK kaynağı | ATK_source | int | 15–658 | Komutan modunda commander_ATK, otofarm'da effective_ATK |
| Yetenek çarpanı | skill_atk_multiplier | float | 2.0 | Güçlü Vuruş sabit çarpanı |
| DEF faktörü | defense_reduction_factor | int | 2 | Fiziksel hasar |
| Hedef DEF | target_DEF | int | 15–137 | Sinerji dahil DEF |
| Element çarpanı | element_multiplier | float | 0.75–1.50 | Element Sistemi'nden |
| Kritik çarpanı | crit_multiplier | float | 2.0 | Sabit |
| Yetenek hasarı | skill_damage_attacker | int | 1–3924 | Final hasar (crit ile teorik max) |

**Örnek — Erken oyun (Komutan)**: Common Saldırgan (ATK=38) vs Common Tank (DEF=35), nötr, crit yok
→ boosted=76, def_red=floor(35/2)=17, base=59, element=59, final=**59**
(Normal saldırı aynı koşulda: 21. Yetenek **2.8x** efektif etki.)

**Örnek — Orta oyun (Komutan)**: Rare Saldırgan Lv20 (ATK=128) vs Rare Tank Lv20 (DEF=106), avantajlı, crit yok
→ boosted=256, def_red=53, base=203, element=floor(203×1.5)=304, final=**304**

### Formül 3: Büyücü Yeteneği — Element Dalgası (skill_damage_mage)

**Büyü hasarı** (`magic_defense_factor = 4`). Her düşmana ayrı pipeline hesaplanır:

```
1. boosted_ATK = floor(ATK_source × skill_aoe_multiplier)
2. def_reduction = floor(target_DEF / 4)
3. base_damage = max(1, boosted_ATK - def_reduction)
4. element_damage = floor(base_damage × element_multiplier_vs_target)
5. crit_damage = was_crit ? floor(element_damage × crit_multiplier) : element_damage
6. per_target_damage = max(1, crit_damage)
```

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| ATK kaynağı | ATK_source | int | 15–658 | Mod'a bağlı |
| AoE çarpanı | skill_aoe_multiplier | float | 0.75 | Element Dalgası sabit çarpanı |
| DEF faktörü | magic_defense_factor | int | 4 | Büyü hasar — DEF daha az etkili |
| Hedef DEF | target_DEF | int | 15–137 | Her hedef için ayrı |
| Element çarpanı | element_multiplier_vs_target | float | 0.75–1.50 | Her hedefe ayrı |
| Hedef başına hasar | per_target_damage | int | 1–∞ | Tek hedef hasarı (crit hariç) |

`total_aoe_damage = Σ(per_target_damage)` tüm hayatta düşmanlar için

**Örnek — Erken oyun**: Common Büyücü (ATK=38) vs 3 düşman (DEF: 35, 15, 17), nötr
→ boosted=floor(38×0.75)=28
→ Hedef 1 (DEF=35): 28-floor(35/4)=28-8=**20**
→ Hedef 2 (DEF=15): 28-floor(15/4)=28-3=**25**
→ Hedef 3 (DEF=17): 28-floor(17/4)=28-4=**24**
→ Toplam: **69** (Normal tek hedef: 28. AoE toplam **2.5x** verimlilik, 3 hedefte)

*Eski değerler (fiziksel hatalı hesap: 11/21/20=52) düzeltildi — büyü hasarı DEF'e daha az takılır.*

### Formül 4: Tank Yeteneği — Koruma Duruşu (buffed_DEF)

`buffed_DEF = floor(effective_DEF × skill_def_multiplier)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Efektif DEF | effective_DEF | int | 15–137 | Sinerji dahil DEF |
| Savunma çarpanı | skill_def_multiplier | float | 2.0 | Koruma Duruşu sabit çarpanı |
| Buff süresi | skill_buff_duration | int | 2 | Tur |
| Buff'lı DEF | buffed_DEF | int | 30–274 | 2 tur boyunca geçerli |

**Buff kuralı**: Stack'lenmez. Tekrar kullanılırsa süre yenilenir (DEF×2 kalır, DEF×4 olmaz).

**Örnek — Erken oyun**: Common Tank (DEF=35)
→ buffed_DEF = 70
→ Saldırgan (ATK=35) vs buffed: max(1, 35-35) = **1** (normal: 18 → **%94 azalma**)

**Örnek — Orta oyun**: Rare Tank Lv20 (DEF=116, sinerji dahil)
→ buffed_DEF = 232
→ Rare Saldırgan (ATK=128): 128-116 = **12** (normal: 70 → **%83 azalma**)

### Formül 5: Destekçi Yeteneği — İyileştirme (heal_amount) — Referans

Sağlık / Can Sistemi GDD'sinde tanımlıdır (`healer_heal_formula`). Hibrit Savaş bu formülü tetikler.

`heal_amount = floor(target_max_hp × healer_skill_rate)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Hedef max HP | target_max_hp | int | 18–258 | Pipeline max HP |
| İyileşme oranı | healer_skill_rate | float | 0.20 | Registry sabiti (MVP) |
| İyileşme miktarı | heal_amount | int | 3–51 | Sağlık/Can GDD'sinden |

**Örnek**: Common Tank (max_hp=30) → heal = floor(30×0.20) = **6 HP**
Rare Tank Lv20 Form B (max_hp=92) → heal = floor(92×0.20) = **18 HP**

### Formül 6: Enerji Progresyonu

`turns_to_ability = ceil(energy_threshold / energy_per_turn)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Enerji eşiği | energy_threshold | int | 100 | Yetenek kullanım eşiği |
| Tur başına enerji | energy_per_turn | int | 25 | Sabit |
| Yeteneğe tur sayısı | turns_to_ability | int | 4 | İlk ve ardışık yetenekler arası |

**Enerji tablosu**:

| Tur | Enerji (tur sonu) | Yetenek? |
|-----|-------------------|----------|
| 1 | 25 | Hayır |
| 2 | 50 | Hayır |
| 3 | 75 | Hayır |
| 4 | 100 | **Evet** |
| 5 (kullanım sonrası) | 25 | Hayır |
| 8 | 100 | **Evet (2. kullanım)** |

**Savaş süresi bağlantısı**: 5-8 tur hedefinde her canavar 1-2 yetenek kullanır. Komutan modunda oyuncu yetenek kullanımını 1 tur erteleyebilir (stratejik bekleme).

### Formül 7: Savaş Süresi Tahmini (Planlama Formülü)

`estimated_turns = ceil(total_enemy_HP / team_damage_per_round)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Düşman toplam HP | total_enemy_HP | int | 72–1032 | Tüm düşman HP toplamı |
| Takım tur hasarı | team_damage_per_round | int | 4–2000+ | 4 canavar toplam DPS |
| Tahmini süre | estimated_turns | int | 1–20+ | Düşmanları yenme süresi |

**Not**: Planlama formülüdür, runtime değil. Gerçek süre element, crit, yetenek ve birim ölümleriyle değişir.

**Senaryo — Erken oyun (Komutan)**: 4× Common Lv1 vs 3× Common Lv1
- Takım DPS: 27+5+7+27 = ~66/tur. Düşman HP: ~68.
- Tek taraflı ~2 tur → gerçekte **3-4 tur** (düşmanlar da hasar verir)

**Senaryo — Orta oyun (Komutan)**: 4× Rare Lv20 vs 4× Rare Lv20
- Takım DPS: ~235/tur, düşman DPS: ~184/tur
- **3-5 tur** tahmini (birim ölümleri dahil)

### Formül 8: DoT Hasarı

`dot_damage_per_tick = max(1, floor(target_max_hp × dot_rate))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Hedef max HP | target_max_hp | int | 18–258 | Pipeline max HP |
| DoT oranı (Yanma) | yanma_rate | float | 0.05 | Büyücü Slot 2 |
| DoT oranı (Zehir) | zehir_rate | float | 0.04 | Hırsız Slot 1 |
| Tick hasarı | dot_damage_per_tick | int | 1–12 | Her tur başı uygulanır |

**Çıktı Aralığı**: MVP'de 1–12 (Common Lv1 min=1, Rare Lv20 max ~12).

**Örnek — Yanma** (Rare Tank Lv20, max_hp=92):
`max(1, floor(92 × 0.05))` = `max(1, 4)` = **4 hasar/tur × 3 tur = 12 toplam**

**Örnek — Zehir** (Common Saldırgan Lv1, max_hp=18):
`max(1, floor(18 × 0.04))` = `max(1, 0)` = **1 hasar/tur × 4 tur = 4 toplam** (min garantisi)

**Yanma + Zehir aynı anda** (Rare Saldırgan, max_hp=52):
- Yanma: `floor(52 × 0.05)` = **2/tur**
- Zehir: `floor(52 × 0.04)` = **2/tur**
- Toplam: **4 hasar/tur** (bağımsız, iki tick ayrı ayrı uygulanır)

### Formül 9: Cooldown Yönetimi

`turns_until_available = max(0, current_cd - turns_elapsed)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Slot CD değeri | slot_cd | int | {0, 3, 5, 8} | Slot 0/1/2/3 sabit CD |
| Mevcut CD | current_cd | int | 0–8 | Kalan bekleme turu |
| Kullanılabilir | is_available | bool | — | `current_cd == 0` ise true |

**Cooldown tablosu** (yetenek kullanıldıktan sonra):

| Tur | Slot 1 (CD3) | Slot 2 (CD5) | Slot 3 (CD8) |
|-----|-------------|-------------|-------------|
| Kullanım | CD=3 | CD=5 | CD=8 |
| +1 | CD=2 | CD=4 | CD=7 |
| +2 | CD=1 | CD=3 | CD=6 |
| +3 | **CD=0 ✓** | CD=2 | CD=5 |
| +5 | — | **CD=0 ✓** | CD=3 |
| +8 | — | — | **CD=0 ✓** |

**Not**: Slot 0 CD=0 (her zaman açık). Savaş başlangıcında tüm CD'ler 0 → tüm yetenekler ilk turda kullanılabilir.

### Formül 9: Komutan vs Otofarm Verimlilik Doğrulama

`commander_efficiency_ratio = 1.0 + atk_bonus_effect + targeting_effect + timing_effect`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| ATK bonus etkisi | atk_bonus_effect | float | 0.12–0.19 | DEF'e bağlı efektif ATK farkı |
| Hedef seçim etkisi | targeting_effect | float | 0.03–0.12 | Element eşleşmesine bağlı |
| Zamanlama etkisi | timing_effect | float | 0.03–0.10 | Oyuncu becerisine bağlı |
| Verimlilik oranı | commander_efficiency_ratio | float | 1.18–1.41 | Komutan/Otofarm DPS oranı |

**Doğrulama tablosu**:

| Kaynak | Min | Max | Ortalama |
|--------|-----|-----|----------|
| ATK Bonusu (+10% flat → efektif) | +12% | +19% | +14% |
| Hedef Seçimi | +3% | +12% | +7% |
| Yetenek Zamanlaması | +3% | +10% | +6% |
| **Toplam** | **+18%** | **+41%** | **~+27%** |

**Sonuç**: Game concept'teki %20-30 hedefi doğrulanır. Ortalama ~%27; deneyimli oyuncuda %30+'a çıkabilir.

## Edge Cases

- **If oyuncu canavarının enerjisi 100'deyken savaş dışı kalırsa**: Enerji kaybolur. Savaş dışı canavar yetenek kullanamaz. Savaş sonu tüm enerji sıfırlanır (bir sonraki savaşta 0'dan başlar).

- **If komutan modunda oyuncu yetenek butonuna basmadan canavar sırası geçerse**: Normal saldırı yapılır. Enerji 100'de kalır — harcanmaz. Oyuncu sonraki turda tekrar butona basabilir. Timeout yok — oyuncu butona basana kadar normal saldırı devam eder.

- **If otofarm modunda destekçi iyileştirme hedefi seçerken tüm takım tam HP'deyse**: Yetenek kullanılmaz, enerji 100'de bekler. Sonraki turda tekrar kontrol edilir. Enerji boşa harcanmaz.

- **If komutan modunda oyuncu hedef seçtiği düşman başka birim tarafından savaş dışı bırakılırsa (aynı raunt içinde)**: Hedef otomatik olarak en yüksek HP'li hayatta düşmana geçer. Oyuncuya kısa "hedef değişti" bildirimi gösterilir.

- **If savaş sırasında mod komutandan otofarm'a değiştirilir ve o turda canavar yetenek bekliyorsa**: Yetenek bir sonraki turda otofarm kurallarıyla otomatik kullanılır (energy=100 ise anında). Mevcut tur eski modla tamamlanır.

- **If Tank Koruma Duruşu aktifken Tank savaş dışı kalırsa**: Buff anında sona erer (taşıyıcı yok). Tank'ın buff'ı diğer canavarlara transfer olmaz.

- **If Tank Koruma Duruşu aktifken Tank tekrar yetenek kullanırsa (8. turda energy=100)**: Buff süresi yenilenir (2 tura sıfırlanır). DEF çarpanı stack'lenmez — DEF×2 kalır, DEF×4 olmaz.

- **If Büyücü Element Dalgası kullanırken tek düşman kaldıysa**: AoE hasarı tek hedefe tam uygulanır (0.75x ATK). Çoklu hedef avantajı kaybolur ama ceza uygulanmaz.

- **If tüm düşmanlar aynı turda savaş dışı kalırsa (birden fazla canavar aynı raunt'ta son düşmanı hedef alır)**: İlk öldüren saldırıdan sonra kalan saldırılar iptal edilir — Victory tetiklenir. Kalan canavarların turları atlanır.

- **If tüm oyuncu canavarları aynı turda savaş dışı kalırsa (düşman AoE)**: Defeat anında tetiklenir. O raunt'taki kalan düşman turları atlanır.

- **If savaş 20+ tur sürerse (uzun savaş)**: Savaş devam eder — tur limiti yok. Dengeleme sorunu olarak loglanır. Pratik durum: düşük DPS + yüksek DEF = çok yavaş hasar. Çözüm: zindan zorluk ölçeklemesinde, savaş mekaniklerinde değil.

- **If oyuncu savaş başlamadan "Çekil" butonuna basarsa**: Savaş hiç başlamaz. Enerji harcanmaz, loot yok, ceza yok. PreCombat → PostCombat direkt geçiş.

- **If 1x hızda komutan modu ve oyuncu hiç müdahale etmezse (AFK commander)**: Canavarlar normal saldırılarla devam eder (varsayılan hedef: en yüksek HP düşman). Yetenekler kullanılmaz (buton basılmadığı için enerji 100'de birikir). ATK bonusu uygulanır ama yetenek avantajı kaybolur.

- **If SPD eşitliğinde 4+ birim aynı SPD'ye sahipse**: Oyuncu canavarları önce (takım slot sırasıyla 1→2→3→4), sonra düşmanlar. Düşmanler arası eşitlikte: düşman listesindeki sıra. Deterministik — aynı koşulda aynı sıra.

- **If savaş sırasında uygulama arka plana atılırsa (mobil)**: Savaş durumu kaydedilir. Uygulama geri geldiğinde kaldığı yerdan devam eder. Otofarm modundaysa arka planda tamamlanır (Otofarm/Idle GDD Kural 1-3).

- **If düşman boss öfke modu aktifleştiği turda oyuncu mod değiştirirse**: Her iki değişiklik bir sonraki turda geçerli. Boss öfke SPD artışı + oyuncu mod değişikliği bağımsız state değişiklikleri — çakışma yok.

- **If DoT uygulanan birim DoTPhase'de savaş dışı kalırsa**: Savaş dışı tetiklenir, kalan fazlar (Regen, Energy, Decision, Action) atlanır. DoT sona erer. Hasar, normal saldırı hasarından önce geldiği için savaş dışı bırakma DoT ile de gerçekleşebilir.

- **If aynı birimde Yanma ve Zehir aynı anda aktifse**: Her iki DoT bağımsız olarak her DoTPhase'de uygulanır. Toplam hasar = yanma_tick + zehir_tick. Süreleri bağımsız sayılır — biri bitince diğeri devam eder.

- **If Yanma aktifken düşman ikinci kez Yanma alırsa**: Yanma süresi yenilenir (3 tura sıfırlanır). Hasar oranı değişmez — stack'lenmez.

- **If Sersemletme boss'a uygulanırsa**: Stun uygulanmaz (boss bağışıklığı). Yeteneğin diğer etkileri (hasar) normal uygulanır — sadece stun atlanır.

- **If Sersemletme süresi 1 turken birim o tur zaten aksiyonunu tamamladıysa**: Stun bir sonraki birim turunda işler — birim hasar alır, aksiyon yapamaz. Stun "gelecek tur" geçerlidir, geriye dönük değil.

- **If Kalkan aktifken gelen hasar kalkan kapasitesinden büyükse**: Kalan hasar HP'ye uygulanır. `hp_damage = total_damage - shield_remaining`. Kalkan sıfırlanır ve kaldırılır.

- **If oyuncu sınıf Slot 3 (CD8) savaş başında kullanılırsa (CD=0)**: Kullanılır, CD=8 olarak set edilir. İlk kullanımda herhangi bir kısıtlama yok — CD0 kuralı savaş başlangıcı için geçerli.

- **If otofarm modunda Slot 2 ve Slot 3 aynı anda açıksa**: Öncelik Slot 3'e aittir (Kural 7 otofarm öncelik sırası). Slot 2 bir sonraki tura saklı kalır.

- **If Şifacı Slot 2 (Diriliş) ile canavar diriltilirse**: Diriltilen canavar HP=1 ile savaşa döner (ayrıntılar Oyuncu Sınıf Sistemi GDD). Sıra listesine bir sonraki raundda eklenir.

- **If energy_per_turn veya skill_atk_multiplier 0 olursa (yapılandırma hatası)**: energy_per_turn = 1 olarak fallback; skill çarpanı = 0 ise base_damage = max(1, ...) = 1. Hata loglanır.

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Hasar Hesaplama** | Sert | `CalculateDamage(attackerId, targetId, damageType)` → int; `EstimateDamage(attackerId, targetId, damageType)` → int | Olmadan saldırılar hasar üretemez |
| **Oyuncu Sınıf Sistemi** | Sert | Oyuncunun `damageType` ("physical"/"magic") — sınıfa göre belirlenir | Olmadan magic/physical ayrımı yapılamaz |
| **Sağlık / Can Sistemi** | Sert | `IsAlive(monsterId)` → bool; `GetCurrentHP(monsterId)` → int; `GetHPRatio(monsterId)` → float; `TakeDamage(targetId, amount)`; `Heal(targetId, amount)`; `FullHeal(teamId)` | Olmadan savaş döngüsü çalışmaz |
| **Takım Kurma** | Sert | `GetActiveTeam()` → [{monsterId, slot, effective_stats, element}] | Olmadan oyuncu takımı belirlenemez |
| **Düşman AI** | Sert | `GetEnemyAction(enemyId)` → {actionType, targetId, skillId} | Olmadan düşmanlar aksiyon alamaz |
| **Element Sistemi** | Sert | `GetElementMultiplier(atkElement, defElement)` → float; `CalculateSynergy(teamElements[])` → {atk_bonus, def_bonus, spd_bonus} | Olmadan element farkı ve sinerji yok |
| **Canavar Veritabanı** | Sert | `GetMonsterIdentity(monsterId)` → {element, archetype}; `GetSkillDef(archetype)` → yetenek tanımı | Olmadan canavar kimliği ve yetenek bilinmez |
| **Canavar Güçlendirme** | Sert | `GetEffectiveStats(monsterId)` → {hp, atk, def, spd} | Olmadan stat pipeline çıktısı alınamaz |
| **Zindan Keşif** | Sert | `GetFloorEnemies(floorNumber)` → düşman tanımları | Olmadan düşman grubu oluşturulamaz |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Loot / Ödül Sistemi** | Sert | `DistributeLoot(battleResult, floorNumber)` — savaş sonucu tetikler | Olmadan savaş kazanınca ödül dağıtılamaz |
| **Savaş UI** | Sert | `OnTurnStart`, `OnActionExecuted`, `OnBattleEnd`, `OnModeChanged` events | Olmadan savaş görüntülenemez |
| **Zindan Keşif** | Sert | `OnBattleComplete(result)` — savaş sonucu zindan ilerlemesini günceller | Olmadan zindan ilerleme takibi yapılamaz |
| **Otofarm / Idle Sistemi** | Sert | Savaş simülasyonu (basitleştirilmiş) | İdle modda savaş sonuçlarını simüle eder |

**Bağımlılık doğası**: Bu sistem orkestratör — 8 upstream'den veri alır, 4 downstream'e sonuç bildirir. Kendisi hesaplama yapmaz, doğru sırayla diğer sistemleri çağırır.

**Çift yönlü**: Zindan Keşif ↔ Hibrit Savaş (düşman listesi alır ←, savaş sonucu bildirir →). Sağlık/Can ↔ Hibrit Savaş (HP okur ←, hasar/iyileşme gönderir →).

**Bidirectional check**:
- Hasar Hesaplama GDD: "Hibrit Savaş → sert downstream" ✓
- Sağlık/Can GDD: "Hibrit Savaş → sert downstream" ✓
- Takım Kurma GDD: "Hibrit Savaş → sert downstream" ✓
- Düşman AI GDD: "Hibrit Savaş → sert downstream" ✓
- Loot/Ödül GDD: `DistributeLoot(battleResult, floorNumber)` upstream eklendi ✓
- Zindan Keşif GDD: ✅ Yazıldı — çift yönlü arayüzler doğrulandı
- Otofarm GDD: ✅ Yazıldı — savaş simülasyonu arayüzü doğrulandı

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `commander_atk_bonus` | 0.10 | 0.05–0.20 | Komutan modu çok güçlü → otofarm anlamsız | Fark hissedilmez → aktif oynama motivasyonu düşer |
| `energy_per_turn` | 25 | 15–50 | Yetenek çok sık → savaşlar kısa, yetenek dominant | Yetenek çok seyrek → monotonluk, komutan modu anlamsız |
| `energy_threshold` | 100 | 50–200 | Yetenek çok sık (energy_per_turn ile birlikte) | Oyuncu yetenek göremeden savaş biter |
| `skill_atk_multiplier` | 2.0 | 1.5–3.0 | Güçlü Vuruş tek vuruşta öldürür | Normal saldırıdan zar zor iyi → motivasyon yok |
| `skill_aoe_multiplier` | 0.75 | 0.50–1.0 | AoE çok güçlü → tek hedef anlamsız | AoE çok zayıf → Büyücü değersiz |
| `skill_def_multiplier` | 2.0 | 1.5–3.0 | Tank hasar almaz → savaşlar uzar | Koruma fark edilmez → Tank değersiz |
| `skill_buff_duration` | 2 | 1–4 | Kalıcı ölümsüzlük (4 tur + yenileme = sürekli) | 1 tur = anlık etki, zamanlama değersiz |
| `healer_skill_rate` | 0.20 | 0.15–0.25 | Ölümsüzlük sağlar | İyileşme anlamsız → Destekçi değersiz |
| `speed_1x_turn_duration` | 2.0s | 1.5–3.0s | Savaş yavaş hisseder | Animasyonlar görülmez |
| `speed_2x_multiplier` | 0.5 | 0.3–0.7 | Hâlâ yavaş | Çok hızlı → anlaşılmaz |
| `speed_3x_multiplier` | 0.25 | 0.15–0.4 | Farming verimsiz | Animasyonlar görünmez |
| `autofarm_random_weight` | 60 | 40–80 | Çok rastgele → verimsiz | Çok hedefli → komutan farkı azalır |
| `autofarm_lowHP_weight` | 25 | 10–40 | Komutan farkı azalır | "Aptal" hisseder |
| `autofarm_highATK_weight` | 15 | 5–25 | Taktik AI gibi → fark azalır | Stratejik fark artar |
| `yanma_rate` | 0.05 | 0.03–0.10 | DoT tek başına öldürür → diğer hasarlar anlamsız | Yanma fark edilmez |
| `yanma_duration` | 3 | 2–5 | Uzun savaşlarda ezici kümülatif hasar | Çok kısa → bir tur | 
| `zehir_rate` | 0.04 | 0.02–0.08 | Zehir tek başına yeterli → Hırsız overpowered | Zehir motivasyonu yok |
| `zehir_duration` | 4 | 3–6 | Boss'u bile eritir kümülatif | Çok kısa → değersiz |
| `shield_rate` | 0.25 | 0.15–0.40 | Ölümsüzlük — tank gibi oynar | Kalkan anlamsız |
| `shield_duration` | 3 | 2–4 | Sürekli kalkan (3 tur + 3 CD) | Tek vurş bozulur |
| `def_break_mult` | 0.70 | 0.50–0.85 | DEF tamamen anlamsız | Kırma fark edilmez |
| `def_break_duration` | 2 | 1–3 | Uzun → sürekli kırık DEF | Zamanlama kritik ama kısa |
| `atk_weaken_mult` | 0.80 | 0.60–0.90 | Düşman saldırı gücü sıfıra yakın | Zayıflatma hissedilmez |
| `damage_reduce_mult` | 0.75 | 0.60–0.90 | Hasar azaltma kalkanla birlikte çok güçlü | Hayatta kalma fark yok |

**Etkileşim Uyarıları**:
- `commander_atk_bonus` × `defense_reduction_factor` (registry=2): DEF azaltması sabit olduğundan ATK bonusu efektif etkisi +10%'dan fazla (+12-19%). Bonus'u artırırken bu amplifikasyonu hesaba kat.
- `energy_per_turn` / `energy_threshold` birlikte yetenek sıklığını belirler. İkisini aynı anda değiştirmek etkiyi katlar (25/100=4 tur, 50/100=2 tur).
- `skill_atk_multiplier` × `crit_multiplier` (registry=2.0) birlikte burst hasarı belirler: max 2.0×2.0=4.0x normal hasar. Düşman HP dengesini kontrol et.
- `skill_def_multiplier` × `skill_buff_duration` birlikte Tank dayanıklılığını belirler. 2.0×2 tur vs 3.0×3 tur = "yenilmez" hissi.
- `healer_skill_rate` Sağlık/Can GDD'sinde tanımlı — tek kaynak orada. Değişiklik orada yapılmalı.
- Otofarm hedef ağırlıkları (60/25/15) Düşman AI Basit AI ağırlıklarıyla aynı — farklılaştırılırsa her iki GDD'de güncellenmelidir.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Süre | Öncelik |
|------|-----|------|---------|
| Savaş başlangıcı | Takım giriş (soldan) + düşman belirme (sağdan), savaş alanı aydınlanma | 1.5s | MVP |
| Tur sırası göstergesi | Aktif birimin sprite çevresinde parlak çerçeve (altın=oyuncu, kırmızı=düşman) | Sürekli (tur boyunca) | MVP |
| Enerji barı dolma | Kademeli dolma (element renginde). %100'de parlama + yetenek butonu pulse | 0.3s dolma + sürekli pulse | MVP |
| Saldırgan — Güçlü Vuruş | Kısa charge + büyük slash efekti + element renginde darbe dalgası + ekran sarsıntısı | 0.8s | MVP |
| Tank — Koruma Duruşu | Kalkan oluşma (mavi-altın) + defansif aura (2 tur sürekli) + DEF artış ikonu | 0.5s aktifleşme + sürekli aura | MVP |
| Destekçi — İyileştirme | Yeşil ışınlar (hedef üstüne yükselen) + HP barı dolma + "+X HP" yeşil text | 0.6s | MVP |
| Büyücü — Element Dalgası | Element renginde genişleyen dalga (sahneyi boyayan renk patlaması) + tüm düşmanlara eş zamanlı darbe | 1.0s | MVP |
| Komutan modu aktif | Ekran köşesinde altın komutan rozeti + takım çevresinde hafif altın aura | Sürekli | MVP |
| Otofarm modu aktif | Ekran köşesinde gri/mavi çark ikonu + "AUTO" yazısı | Sürekli | MVP |
| Mod değişimi | Eski ikon fade-out + yeni ikon fade-in + kısa flash geçiş | 0.5s | MVP |
| Kazanma (Victory) | Parlak altın-beyaz ışık patlaması + "ZAFER!" yazısı + loot sandığı belirme | 2.0s | MVP |
| Kaybetme (Defeat) | Ekran hafif kararma + "YENİLDİN" yazısı (soluk, cezalandırıcı değil) + "Tekrar Dene" | 1.5s | MVP |
| Çekilme | Canavarlar sola kayarak çıkış + "Çekildin" yazısı | 1.0s | MVP |
| Boss savaşı — sahne geçişi | Aydınlatma dramatikleşir (boss element rengi baskın + oyuncu altın dolgu) | 1.0s | MVP |
| Hız değişimi | Hız ikonu güncelleme + animasyonlar hızlanır | Anında | MVP |

### İkon Gereksinimleri

| İkon | Boyut | Kullanım Yeri |
|------|-------|---------------|
| Yetenek butonu (×4 arketip) | 64×64 px | Ekran altında, komutan modunda aktif |
| Komutan modu rozeti | 32×32 px | Ekran köşesi, mod göstergesi |
| Otofarm modu çark ikonu | 32×32 px | Ekran köşesi, mod göstergesi |
| Hız kontrol butonu (1x/2x/3x) | 48×48 px | Ekran köşesi |
| Çekil butonu ikonu | 48×48 px | Savaş sırasında sürekli görünür |
| Enerji barı | 8×48 px (bar) | Her canavar sprite'ı altında |
| Buff süre göstergesi | 24×24 px | Buff aktif canavarın yanında |

### Audio Gereksinimleri

| Olay | Ses Türü | Ton | Öncelik |
|------|----------|-----|---------|
| Savaş başlangıcı | Kısa epik fanfar + savaş trompeti | Heyecan, güç | MVP |
| Tur geçişi | Çok kısa "tick" — birim değişimi | Nötr, dikkat dağıtmaz | MVP |
| Enerji %100 (yetenek hazır) | Yükselen melodic chime + parlama | Ödüllendirici, "hazır!" | MVP |
| Saldırgan — Güçlü Vuruş | Ağır keskin slash + darbe + güç patlaması | Güçlü, tatmin edici | MVP |
| Tank — Koruma Duruşu | Kalkan "clang" + düşük hum (sürekli) | Sağlam, koruyucu | MVP |
| Destekçi — İyileştirme | Yükselen parlak chime + yumuşak çan | Rahatlatıcı, pozitif | MVP |
| Büyücü — Element Dalgası | Genişleyen büyü sesi + çoklu darbe | Büyük, kapsayıcı | MVP |
| Komutan → Otofarm geçiş | Kısa "click-off" + düşen ton | Gevşeme | MVP |
| Otofarm → Komutan geçiş | Kısa "click-on" + yükselen ton | Dikkat, aktifleşme | MVP |
| Kazanma | Zafer fanfarı + altın yağmuru sesi | Kutlama, başarı | MVP |
| Kaybetme | Düşen ton + yumuşak "neredeyse" hissi | Hafif, cezalandırıcı değil | MVP |
| Hız değişimi | Kısa "tık" + hız ritmi değişimi | Nötr | Nice-to-have |

> **Asset Spec Flag**: Visual/Audio gereksinimleri tanımlandı. Art bible onaylandıktan sonra `/asset-spec system:hibrit-savas` çalıştırarak savaş VFX, ikon ve ses asset spesifikasyonları üretilebilir.

## UI Requirements

### Savaş Ekranı Ana Layout

- **Üst bölüm**: Düşman canavarlar (sağda sıralı, sprite + HP barı + enerji barı + element ikonu)
- **Orta bölüm**: Savaş alanı — saldırı animasyonları, hasar sayıları, VFX
- **Alt bölüm**: Oyuncu canavarları (solda sıralı, sprite + HP barı + enerji barı + element ikonu)
- **Ekran alt kenarı**: Yetenek butonları (komutan modunda, 4 slot)
- **Ekran üst köşe (sol)**: Mod göstergesi + mod değiştir butonu
- **Ekran üst köşe (sağ)**: Hız butonu (1x/2x/3x) + Çekil butonu
- **Sıra göstergesi**: Yatay tur sırası barı — birim ikonları SPD sırasıyla

### Yetenek Butonları (Komutan Modu)

- 4 buton (her canavar için 1), ekran altında yatay dizi
- Her buton: canavar küçük portresi + yetenek ikonu + enerji radial fill göstergesi
- Enerji < 100: buton soluk (devre dışı), enerji % gösterilir
- Enerji = 100: buton parlak + pulse animasyonu, "HAZIR" göstergesi
- Dokunulduğunda: yetenek kullanılır, enerji sıfırlanır
- Canavar savaş dışı: buton gri + çarpı ikonu
- Otofarm modunda: butonlar gizlenir veya küçülür (enerji barı hâlâ görünür)
- Minimum dokunma hedefi: 64×64 dp (savaş sırasında hızlı dokunma)

### Hedef Seçimi (Komutan Modu)

- Düşman sprite'larına dokunarak hedef belirlenir
- Seçili hedef: parlak altın çerçeve + "HEDEF" yazısı
- Tüm oyuncu canavarlarından hedefe ince ok çizgileri
- Hedef değiştirildiğinde: ok geçiş animasyonu (0.2s)
- Hedef savaş dışı olduğunda: otomatik değişim + bildirim

### Mod Değiştir Butonu

- Ekran sol üst: toggle switch (Komutan/Otofarm)
- Komutan: altın renk, kılıç/komutan ikonu
- Otofarm: mavi-gri, çark ikonu + "AUTO"
- Tek dokunuşla geçiş (bir sonraki turdan geçerli)
- Minimum dokunma hedefi: 48×48 dp

### Hız Kontrol Butonu

- Ekran sağ üst: dairesel buton, mevcut hız büyük font (1×/2×/3×)
- Her dokunuşta: 1x → 2x → 3x → 1x döngüsü
- Minimum dokunma hedefi: 48×48 dp

### Enerji Barı

- Her canavarın sprite altında ince enerji barı (HP barının altında, 4px yükseklik)
- Renk: element renginde dolma (ateş=turuncu, su=mavi, toprak=yeşil, hava=mor)
- %100'de: tamamen dolu + hafif parlama

### Savaş Sonu Ekranı

- **Kazanma**: Altın arka plan + "ZAFER!" + loot listesi (kaydırılabilir) + "Devam" butonu
- **Kaybetme**: Soluk arka plan + "Tekrar Dene" + "Çekil" butonu
- **Çekilme**: "Çekildin" mesajı + zindan haritasına dönüş

### Tur Sırası Barı

- Ekran üstünde yatay bar, birim daire ikonları (çerçeve: oyuncu=altın, düşman=kırmızı)
- Aktif birim: büyük + parlak vurgu
- Sıra soldan sağa: mevcut → sonraki
- Savaş dışı birimler: gri, sıradan çıkar

### Minimum Dokunma Hedefleri

Tüm interaktif elemanlar minimum 44×44 dp (technical-preferences.md). Yetenek butonları 64×64 dp.

> **UX Flag — Savaş Sistemi**: Bu sistem kapsamlı UI gereksinimleri içeriyor. Phase 4'te `/ux-design` çalıştırarak savaş ekranı, yetenek butonları, hedef seçimi, mod geçişi ve savaş sonu ekranı için UX spec oluşturulmalı — stories `design/ux/savas-ekrani.md`'yi referans almalı.

## Acceptance Criteria

1. **GIVEN** savaş başlatıldığında, **WHEN** PreCombat tamamlanırsa, **THEN** tüm birimler current_hp=max_hp ve energy=0 ile başlar.

2. **GIVEN** 4 canavar takımda (SPD: 30, 25, 20, 15), **WHEN** ilk raunt başlarsa, **THEN** tur sırası SPD azalan: 30→25→20→15.

3. **GIVEN** SPD eşitliğinde (oyuncu canavarı SPD=30, düşman SPD=30), **WHEN** sıra belirlenirse, **THEN** oyuncu canavarı önce hareket eder.

4. **GIVEN** komutan modunda Common Saldırgan (effective_ATK=35), **WHEN** saldırırsa, **THEN** commander_ATK = floor(35×1.10) = 38 kullanılır.

5. **GIVEN** otofarm modunda aynı canavar, **WHEN** saldırırsa, **THEN** effective_ATK = 35 kullanılır (commander bonusu yok).

6. **GIVEN** canavar energy=75, **WHEN** enerji fazı çalışırsa, **THEN** energy = 100, yetenek butonu aktifleşir.

7. **GIVEN** canavar energy=100, komutan modu, **WHEN** oyuncu yetenek butonuna basarsa, **THEN** yetenek kullanılır ve energy = 0'a döner.

8. **GIVEN** canavar energy=100, komutan modu, **WHEN** oyuncu butona basmazsa, **THEN** normal saldırı yapılır ve energy = 100 kalır (harcanmaz).

9. **GIVEN** canavar energy=100, otofarm modu, **WHEN** canavar sırası gelirse, **THEN** yetenek anında otomatik kullanılır, energy = 0.

10. **GIVEN** Saldırgan Güçlü Vuruş (ATK=38, multiplier=2.0) vs Tank (DEF=35), nötr, crit yok, **WHEN** yetenek kullanılırsa, **THEN** boosted=76, base=76-17=59, final=**59**.

11. **GIVEN** Büyücü Element Dalgası (ATK=38, multiplier=0.75, büyü hasar) vs 3 düşman (DEF: 35,15,17), nötr, **WHEN** yetenek kullanılırsa, **THEN** boosted=28; def_red: floor(35/4)=8, floor(15/4)=3, floor(17/4)=4. Hedef başına: 20, 25, 24. Toplam=**69**.

12. **GIVEN** Tank Koruma Duruşu (DEF=35, multiplier=2.0), **WHEN** yetenek kullanılırsa, **THEN** buffed_DEF=70, 2 tur sürer. 3. turda DEF=35'e döner.

13. **GIVEN** Destekçi İyileştirme, hedef Common Tank (max_hp=30, current_hp=15), **WHEN** yetenek kullanılırsa, **THEN** heal=floor(30×0.20)=6, current_hp=21.

14. **GIVEN** tüm düşmanlar savaş dışı (HP=0), **WHEN** son düşman düşerse, **THEN** Victory tetiklenir, loot dağıtılır, tüm canavarlar tam HP'ye döner.

15. **GIVEN** tüm oyuncu canavarları savaş dışı (TPK), **WHEN** son canavar düşerse, **THEN** Defeat tetiklenir, loot yok, enerji harcanmaz, ceza yok.

16. **GIVEN** savaş devam ederken, **WHEN** oyuncu "Çekil" basarsa, **THEN** savaş biter, loot yok, enerji harcanmaz.

17. **GIVEN** komutan modu aktif, **WHEN** oyuncu mod toggle'a basarsa, **THEN** bir sonraki turdan otofarm aktif, ATK bonusu düşer.

18. **GIVEN** 1x hızda savaş, **WHEN** hız 3x'e değiştirilirse, **THEN** animasyon hızlanır, hasar/formül sonuçları değişmez.

19. **GIVEN** komutan modu, oyuncu düşman A'yı hedef seçmiş, **WHEN** düşman A savaş dışı bırakılırsa, **THEN** hedef otomatik en yüksek HP düşmana geçer.

20. **GIVEN** Tank Koruma Duruşu aktif (2 tur), **WHEN** Tank savaş dışı kalırsa, **THEN** buff anında sona erer.

21. **GIVEN** Büyücü Slot 2 ile Yanma DoT uygulandı (hedef max_hp=52), **WHEN** hedefin turu gelirse, **THEN** DoTPhase'de `max(1, floor(52×0.05))` = **2 hasar** uygulanır, 3 tur devam eder.

22. **GIVEN** aynı hedefe Yanma (kalan 2 tur) tekrar Yanma uygulanırsa, **WHEN** DoT set edilirse, **THEN** süre 3 tura yenilenir, hasar oranı değişmez (stack'lenmez).

23. **GIVEN** Hırsız Slot 1 ile Zehir DoT uygulandı (hedef max_hp=18), **WHEN** 4 tur boyunca DoTPhase çalışırsa, **THEN** her tur `max(1, floor(18×0.04))` = **1 hasar**, toplam 4 hasar.

24. **GIVEN** Yanma ve Zehir aynı anda aktif (hedef max_hp=52), **WHEN** DoTPhase çalışırsa, **THEN** Yanma 2 + Zehir `floor(52×0.04)`=2 → toplam **4 hasar** o turda.

25. **GIVEN** Savaşçı Slot 1 stun uyguladı (normal düşman, stun_duration=1), **WHEN** hedef birimin turu gelirse, **THEN** DecisionPhase atlanır, aksiyon yapamaz. Sonraki turda normal.

26. **GIVEN** Savaşçı Slot 1 boss'a stun uyguladı, **WHEN** stun set edilirse, **THEN** stun uygulanmaz (boss bağışıklığı), diğer hasarlar normal.

27. **GIVEN** Kalkan aktif (max_hp=40, shield_hp=10), 15 hasar geldi, **WHEN** hasar uygulanırsa, **THEN** 10 hasar kalkanı tüketir, kalan 5 hasar HP'ye gider.

28. **GIVEN** oyuncu Slot 3 (CD8) savaş başında kullanır (current_cd=0), **WHEN** yetenek kullanılırsa, **THEN** kullanılır, current_cd=8 set edilir.

29. **GIVEN** oyuncu Slot 3 kullandı (current_cd=8), **WHEN** 8 tur geçerse, **THEN** current_cd=0, yetenek tekrar kullanılabilir.

30. **GIVEN** otofarm modunda Slot 2 ve Slot 3 aynı anda açık (current_cd=0), **WHEN** oyuncu turu gelirse, **THEN** Slot 3 kullanılır (öncelik kuralı).

*`qa-lead` not consulted — Lean mode. Review manually before production.*

## Open Questions

1. **Oyuncu yetenek çarpanları vs düşman yetenek çarpanları**: Oyuncu Saldırgan 2.0x, düşman mini-boss Saldırgan 1.5x — bu fark bilinçli mi, yoksa aynı mı olmalı? → Playtest'te dengelenecek. Oyuncunun daha güçlü hissetmesi "Güç Hisset" sütunuyla uyumlu.

2. **Buff/debuff sistemi (Tier 2+)**: ATK/DEF geçici değişiklikleri (zehir, yanma, hız azaltma) formül pipeline'ına nasıl eklenir? → Ayrı buff/debuff GDD'si veya Hibrit Savaş Tier 2 güncellemesi olarak tanımlanacak.

3. **Pasif yetenekler (Tier 2+)**: Her arketipe 1 pasif yetenek eklenmesi planlanıyor mu? (ör: "HP %20 altındayken +30% ATK") → Tier 2+ genişletmesi.

4. **Otofarm savaş simülasyonu**: İdle modda her tur mu hesaplanacak, yoksa sonuç simülasyonu mu? (güç oranına göre kazanma olasılığı → loot) → Otofarm / Idle Sistemi GDD'sinde tanımlanacak.

5. **Savaş sırasında takım düzenleme**: Kat arası düzenleme var (Takım Kurma GDD) ama savaş içi canavar değişimi (benç sistemi) planlanıyor mu? → MVP'de yok. Tier 3+ değerlendirmesi.

6. **Enerji taşma mekaniği**: Energy 100'de kalıyor (overfill yok). İleride "enerji taşmasıyla bonus hasar" gibi bir mekanik eklenecek mi? → Tier 2+ pasif yetenek sistemiyle birlikte değerlendirilecek.

7. **Çoklu hedef seçimi (Komutan modu)**: Komutan modunda her canavar için ayrı hedef seçilebilmeli mi, yoksa tek global hedef mi yeterli? → MVP'de tek global hedef. Playtest feedback'ine göre Tier 2+'da canavar başına hedef eklenebilir.

8. **Savaş kaydı/tekrarı**: Oyuncu geçmiş savaşlarını izleyebilmeli mi (replay)? → Tier 3+ özellik, MVP dışı.
