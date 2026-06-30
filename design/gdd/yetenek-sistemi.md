# Yetenek Sistemi (Skill System)

> **Status**: Designed
> **Author**: user + game-designer, systems-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Hisset, Senin Tempon

## Overview

Yetenek Sistemi, Canavar Zindanları'ndaki tüm savaş aksiyonlarını tanımlayan merkezi yetenek çerçevesidir. Savaştaki her varlık — oyuncu karakteri, aktif pet, düşman canavarlar — **cooldown bazlı yetenek slotlarıyla** hareket eder: her yetenek kullanıldıktan sonra N tur beklemek gerekir, N bekleme süresi dolunca yetenek tekrar kullanılabilir.

Oyuncu karakteri **4 yetenek slotuna** sahiptir: Normal Saldırı (cooldown 0 — her tur), Ağır Saldırı (cooldown 3), Destekleyici Yetenek (cooldown 5) ve Sınıf Özel Yeteneği (cooldown 8). İlk 3 slot tüm sınıflarda aynı kategoridedir; 4. slot sınıfa özeldir ve sınıf tasarımıyla belirlenir. Aktif pet, arketipine göre 1 yetenek taşır (cooldown 4). Düşman canavarlar da aynı yapıyı kullanır — normal saldırı her tur, özel saldırı belirli tur aralıklarıyla.

Oto-savaş modunda yetenek önceliklendirmesi kurallara göre otomatik yapılır. Komutan modunda oyuncu hangi yeteneği, ne zaman kullanacağına kendisi karar verir. Sistem bu iki mod arasındaki davranışsal farkın temelidir.

## Player Fantasy

Yetenek sistemi oyuncuya **güç ve zamanlama tatmini** yaşatır. Çekirdek an: Ağır Saldırı'nın cooldown'u dolarken düşmanın 3. turda saldırmak üzere olduğunu fark etmek, bir tur bekleyip düşmandan önce Ağır Saldırı'yı boşaltmak — ve büyük hasar sayısını izlemek. "Doğru anı kolladım" tatmini.

Normal Saldırı savaşın ritmini kurar: her tur hasar, sürekli ilerleme. Ağır Saldırı patlamanın anıdır — 3 tur biriken enerjiyle iki katı hasar. Destekleyici Yetenek güven verir — "canım azalsa bile 5 turda bir toparlanıyorum." Sınıf Özel Yeteneği kimlik ifadesidir — her sınıfın savaşı farklı hissettiren, 8 turda bir gelen ve "sadece bende bu var" duygusu yaşatan yetenek.

Oto-savaş modunda yetenek sistemi görünmez ama hissedilir: savaşlar otomatik biter, rakipler tanıdık bir ritimde çöker. Komutan modunda sistem merkezde durur — her tur bir karar noktasıdır.

**Negatif fantezi (kaçınılacak):** Cooldown beklemek monoton olmamalı. Normal Saldırı yeterince tatmin edici olmalı ki "yetenek dolsun diye beklemek" değil, "yetenek hazır, ne zaman kullansam?" sorusu olsun.

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Design

### Core Rules

**Kural 1 — Cooldown Mekanizması**

Her yetenek slotu bağımsız bir cooldown sayacı taşır:

1. Yetenek kullanıldığında: `cooldown_remaining = cooldown_turns`
2. Her tur başında (kullanım öncesi): `cooldown_remaining -= 1` (minimum 0)
3. Yetenek kullanılabilir: `cooldown_remaining == 0`
4. `cooldown_turns == 0` olan slot her tur kullanılabilir (Normal Saldırı)
5. Savaş başlangıcında tüm cooldown sayaçları 0'dır — tüm yetenekler ilk turdan itibaren hazır

---

**Kural 2 — Oyuncu Yetenek Slotları (4 slot)**

| Slot | Yetenek Türü | Cooldown | Etki | Hedef |
|------|-------------|---------|------|-------|
| 0 | Normal Saldırı | 0 tur | ATK × 1.0 tek hedef hasar | Tek düşman |
| 1 | Ağır Saldırı | 3 tur | ATK × `skill_atk_multiplier` (2.0) tek hedef hasar | Tek düşman |
| 2 | Destekleyici Yetenek | 5 tur | Sınıfa göre: iyileştirme VEYA buff VEYA debuff | Sınıfa özel |
| 3 | Sınıf Özel Yeteneği | 8 tur | Sınıfa özgü güçlü etki (hasar/destek/kontrol) | Sınıfa özel |

- Slot 2 ve 3'ün tam etki tablosu Oyuncu Sınıf Sistemi GDD'sinde tanımlanır
- Tüm hasar slotları Hasar Hesaplama pipeline'ını kullanır; **hasar türü sınıfa göre belirlenir**:

| Sınıf | Hasar Türü | defense_reduction_factor |
|-------|-----------|--------------------------|
| Savaşçı | Fiziksel | 2 |
| Hırsız | Fiziksel | 2 |
| Büyücü | Büyü | 4 |
| Şifacı | Büyü | 4 |

- `CalculateDamage()` çağrısında sınıftan gelen `damageType` parametresi iletilir

---

**Kural 3 — Pet Yeteneği (1 slot, otomatik)**

Aktif pet, arketipine göre 1 aktif yetenek taşır. Oyuncu pet'i doğrudan kontrol etmez — pet kendi cooldown'una göre otomatik hareket eder (her iki modda da).

| Arketip | Yetenek | Cooldown | Etki | Hedef | Hasar Türü |
|---------|---------|---------|------|-------|-----------|
| Saldırgan | Güçlü Vuruş | 4 tur | ATK × `skill_atk_multiplier` (2.0) hasar | Tek düşman | Fiziksel |
| Tank | Koruma Duruşu | 4 tur | Kendi DEF × `skill_def_multiplier` (2.0), `skill_buff_duration` (2) tur | Kendisi | — (hasar yok) |
| Destekçi | İyileştirme | 4 tur | En düşük HP% birimi iyileştirir: `healer_skill_rate` (0.20) × hedef max HP | En düşük HP% | — (iyileştirme) |
| Büyücü | Element Dalgası | 4 tur | ATK × `skill_aoe_multiplier` (0.75) tüm düşmanlara ayrı ayrı | Tüm düşmanlar | **Büyü** |

- Pet'in normal saldırısı (CD 0): ATK × 1.0, her tur, otomatik
- Pet yeteneği hazır değilse normal saldırı yapar
- Pet savaş dışına düşerse tüm yetenekler durur

---

**Kural 4 — Düşman Yetenek Çerçevesi**

Düşman canavarlar aynı cooldown mekanizmasını kullanır. Somut yetenek içerikleri Düşman AI GDD'sinde tanımlanır.

| Düşman Tipi | Normal Saldırı | Özel Saldırı Cooldown |
|-------------|---------------|-----------------------|
| Normal düşman | CD 0 (her tur) | CD 3 (arketipe göre) |
| Mini-boss | CD 0 | CD 3, 2. özel yetenek CD 5 |
| Boss | CD 0 | CD 3 + CD 5 + CD 7 (öfke fazında CD -1) |

---

**Kural 5 — Oto-Savaş Öncelik Kuralları (Oyuncu için)**

Oto-savaş modunda her tur şu öncelik sırasıyla yetenek seçilir:

1. **HP < %40 → Slot 2** (Destekleyici, eğer iyileştirme türüyse ve hazırsa)
2. **Slot 3 hazır** → Sınıf Özel Yeteneği kullan
3. **Slot 1 hazır** → Ağır Saldırı kullan
4. **Yoksa** → Slot 0 Normal Saldırı

- Slot 2'nin iyileştirme türü olmadığı sınıflarda HP kontrolü yapılmaz; normal öncelik uygulanır
- Öncelik sırası tek bir yetenek seçer — o tur başka yetenek kullanılmaz

---

**Kural 6 — Komutan Modu Yetenek Kontrolü**

- 4 yetenek butonu ekranın altında görünür
- Cooldown > 0 olan buton: soluk, cooldown sayacı gösterir (kalan tur sayısı)
- Cooldown = 0 olan buton: parlak, dokunulabilir
- Oyuncu bir butona dokunur → o slot çalışır → cooldown başlar
- Fallback: Oyuncu hiçbir butona basmazsa Slot 0 (Normal Saldırı) otomatik çalışır
- Oyuncu istediği an basabilir — timeout yok; savaş tur sırasıyla ilerler

### States and Transitions

Her yetenek slotu iki durum arasında geçer:

```
[Hazır (CD=0)] ──(kullanım)──→ [Bekleme (CD=N)] ──(her tur -1)──→ [Hazır]
```

| Durum | Koşul | UI Gösterimi |
|-------|-------|-------------|
| **Hazır** | cooldown_remaining = 0 | Parlak buton, aktif |
| **Bekleme** | cooldown_remaining > 0 | Soluk buton, sayaç: "X tur" |

Pet yeteneği için aynı yapı — oyuncuya gösterilmez, arka planda çalışır.

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı |
|--------|-----|-----------|
| **Hasar Hesaplama** | → tetikler | Slot 0/1 ve hasar türündeki Slot 2/3, pet Saldırgan/Büyücü yeteneği pipeline'ı çalıştırır; `damageType` ("physical"/"magic") sınıftan alınır |
| **Oyuncu Sınıf Sistemi** | ← okur | Her sınıfın `damageType` değeri (Savaşçı/Hırsız=physical, Büyücü/Şifacı=magic) |
| **Sağlık / Can Sistemi** | → tetikler | Slot 2 iyileştirme ve pet Destekçi yeteneği HP günceller |
| **Oyuncu Sınıf Sistemi** | ← okur | Slot 2 ve Slot 3'ün tam etki içeriğini sınıf tanımından alır |
| **Savaş Sistemi** | ↔ çift yönlü | Tur döngüsünde cooldown tik atar; yetenek kararını bu GDD kurallarına göre yapar |
| **Düşman AI** | ← okur | Düşman yetenek cooldown çerçevesini bu GDD'den alır; somut içeriği Düşman AI GDD tanımlar |
| **Savaş UI** | → sağlar | Her slotun cooldown durumu, buton aktif/pasif, animasyon tetikleme |
| **Hibrit Savaş GDD** | ⚠️ revize | Enerji sistemi (eski Kural 6) bu GDD'deki cooldown sistemiyle değiştirilmeli |

## Formulas

### Formül 1: Cooldown Azaltma

`cooldown_remaining(t) = max(0, cooldown_turns - turns_since_use)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Tanımlı cooldown | cooldown_turns | int | 0, 3, 4, 5, 8 | Slota göre sabit (Normal=0, Ağır=3, Pet=4, Destek=5, Özel=8) |
| Kullanımdan bu yana tur | turns_since_use | int | 0–∞ | Her tur +1 artar |
| Kalan bekleme | cooldown_remaining | int | 0–8 | 0 = kullanılabilir |

**Çıktı Aralığı:** 0 (hazır) — 8 (yeni kullanılan Özel Yetenek)

**Örnek:** Ağır Saldırı kullanıldı (CD=3). Tur 1: kalan=3. Tur 2: kalan=2. Tur 3: kalan=1. Tur 4 başında: kalan=0 → hazır.

---

### Formül 2: Normal Saldırı Hasarı

Hasar Hesaplama pipeline'ına girer, ATK multiplier = 1.0. `defense_reduction_factor` sınıfın hasar türüne göre değişir:

`normal_damage = max(1, floor(effective_ATK × 1.0 - floor(effective_DEF / defense_reduction_factor)) × element_multiplier × [crit])`

| Değişken | Sembol | Tip | Değer | Açıklama |
|----------|--------|-----|-------|----------|
| ATK çarpanı | normal_atk_multiplier | float | 1.0 | Sabit, tüm sınıflar |
| DEF faktörü (fiziksel) | defense_reduction_factor | int | 2 | Savaşçı, Hırsız |
| DEF faktörü (büyü) | magic_defense_factor | int | 4 | Büyücü, Şifacı |
| Efektif ATK | effective_ATK | int | 15–599 | Pipeline çıktısı |
| Hedef DEF | effective_DEF | int | 15–137 | Hedef pipeline çıktısı |

**Çıktı Aralığı:** 1–~900 (crit yok), ~1800 (crit ile)

---

### Formül 3: Ağır Saldırı Hasarı

`heavy_damage = max(1, floor(effective_ATK × skill_atk_multiplier - floor(effective_DEF / defense_reduction_factor)) × element_multiplier × [crit])`

`defense_reduction_factor` sınıfın hasar türüne göre Formül 2 ile aynı kuralı izler (fiziksel=2, büyü=4).

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| ATK çarpanı | skill_atk_multiplier | float | 2.0 | Registry sabiti (kaynak: hibrit-savas.md) |
| Efektif ATK | effective_ATK | int | 15–599 | Pipeline çıktısı |
| Ağır hasar | heavy_damage | int | 1–3600 | Crit ile teorik max |

**Çıktı Aralığı:** 1–~3600 (crit ile)

**Örnek:** Common Saldırgan (ATK=35) vs Common Tank (DEF=35), nötr, crit yok:
→ boosted=70, def_red=17, base=53, final=**53** (Normal saldırı: 18 → Ağır **2.9x** daha etkili)

---

### Formül 4: Pet Destekçi İyileştirmesi — Referans

`heal_amount = floor(target_max_hp × healer_skill_rate)`

`healer_skill_rate` = 0.20 (registry sabiti, kaynak: `saglik-can-sistemi.md`)

**Çıktı Aralığı:** 3–51 HP

---

### Formül 5: Savaşta Yetenek Kullanım Sıklığı (Planlama)

`uses_per_combat ≈ floor(total_turns / (cooldown_turns + 1))`

| Slot | CD | 5 turda | 8 turda | 10 turda |
|------|----|---------|---------|---------|
| Normal (CD 0) | 0 | 5 | 8 | 10 |
| Ağır (CD 3) | 3 | 1–2 | 2 | 2–3 |
| Pet (CD 4) | 4 | 1 | 1–2 | 2 |
| Destek (CD 5) | 5 | 1 | 1 | 1–2 |
| Özel (CD 8) | 8 | 0–1 | 1 | 1 |

**Hedef savaş süresi:** 5–8 tur. Bu tabloya göre her savaşta Ağır Saldırı 1-2 kez, Özel Yetenek 0-1 kez kullanılır — "anlamlı ama nadir" dengesi tutarlı.

## Edge Cases

- **If savaş başlarsa tüm cooldown sayaçları 0'dır**: Tüm yetenekler ilk turdan itibaren hazır — oyuncu Özel Yeteneği ilk turda kullanabilir. Kasıtlı: "Güç Hisset" sütunu.

- **If Slot 0 (Normal Saldırı) hiçbir zaman bekleme durumuna girmez**: CD=0 olduğu için `cooldown_remaining` daima 0. Devre dışı kalmaz, her tur kullanılabilir.

- **If oto-savaşta HP %40 altında ama Slot 2 iyileştirme türü değilse**: HP kontrolü atlanır, normal öncelik sırası uygulanır (Slot 3 → Slot 1 → Slot 0).

- **If pet savaş dışı kalırsa**: Pet normal saldırısı ve pet yeteneği durur. Oyuncu kendi slot'larıyla savaşmaya devam eder.

- **If aynı turda hem oyuncu hem pet yeteneği kullanılabilir durumda olursa**: Her ikisi de kullanılır — oyuncu aksiyonu ve pet aksiyonu bağımsız olarak aynı tur içinde sırasıyla çalışır.

- **If komutan modunda oyuncu hiçbir butona basmazsa**: Fallback — Slot 0 (Normal Saldırı) otomatik çalışır. Oyuncu pasif kalırsa komutan modunun tek etkisi +10% ATK bonusudur.

- **If boss "öfke fazında" CD -1 uygulanırsa ve CD 0'ın altına düşerse**: `max(0, cd - 1)` garantisi — CD 0 en düşük değer. CD=0 zaten "her tur" demektir, öfke fazı zaten maksimum sıklığa getirir.

- **If Slot 2 veya Slot 3 sınıf tanımsızsa (Oyuncu Sınıf Sistemi henüz tamamlanmadıysa)**: İlgili slotlar devre dışı bırakılır (gri, "Kilitli" gösterimi). Prototype'ta sadece Slot 0 ve Slot 1 aktiftir.

- **If aynı anda oyuncu ve pet saldırısı son düşmanı hedef alırsa**: İlk vuruş düşmanı deviririr, ikinci saldırı iptal edilir. Deterministik: oyuncu aksiyonu pet aksiyonundan önce işlenir.

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Hasar Hesaplama** | Sert | `CalculateDamage(attackerId, targetId, multiplier)` — hasar slotları çarpanı iletir | Olmadan saldırı yetenekleri hasar üretemez |
| **Sağlık / Can Sistemi** | Sert | `Heal(targetId, amount)` — Destekçi/iyileştirme türü yetenekler | Olmadan iyileştirme uygulanamaz |
| **Oyuncu Sınıf Sistemi** | Sert | Slot 2 ve Slot 3'ün etki içeriğini (tip, çarpan, hedef) ve Slot 0/1'in hasar türünü (`damageType`) tanımlar | Olmadan bu slotlar devre dışı kalır; hasar türü belirlenemez |
| **Pet/Canavar Veritabanı** | Sert | Pet arketipini sağlar → hangi pet yeteneğinin aktif olduğunu belirler | Olmadan pet yeteneği belirlenemez |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Savaş Sistemi** | Sert | Cooldown tik, yetenek karar mantığı (Kural 5/6), fallback kuralları | Savaş döngüsünün yetenek katmanı |
| **Düşman AI** | Sert | Düşman cooldown çerçevesini (CD 0/3/5/7) bu GDD'den alır; somut yetenek içeriği Düşman AI'da | Olmadan düşman yetenek zamanlaması tanımsız |
| **Savaş UI** | Sert | Buton aktif/pasif durumu, cooldown sayacı gösterimi, yetenek animasyon tetiklemesi | Olmadan oyuncu yetenek durumunu göremez |

**Çift yönlü kontrol:**
- Hasar Hesaplama GDD: downstream olarak bu sistem tanımlı ✓
- Sağlık/Can GDD: downstream olarak bu sistem tanımlı ✓
- Hibrit Savaş GDD: Kural 6 (enerji sistemi) bu GDD ile çelişiyor ⚠️ → Hibrit Savaş revizyonunda güncellenmeli

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `cd_heavy` | 3 tur | 2–5 | Ağır çok nadir → Normal monotonlaşır | Her tur Ağır kullanılır → Normal anlamsız |
| `cd_support` | 5 tur | 3–8 | Destek yeteneği yok sayılır | Savaşlar çok kolay (sürekli buff/heal) |
| `cd_special` | 8 tur | 6–12 | Savaşta hiç görülmez | Çok sık → sınıf özgünlüğü önemsizleşir |
| `cd_pet` | 4 tur | 3–6 | Pet aktifliği azalır, pasife döner | Pet çok dominant — oyuncu ikinci plana düşer |
| `normal_atk_multiplier` | 1.0 | 0.8–1.2 | Normal çok güçlü → diğer slotlar anlamsız | Her tur verilen hasar çok düşük → yavaş savaş |
| `skill_atk_multiplier` (Ağır) | 2.0 | 1.5–3.0 | Tek vuruşla ölüm — savaş süresi bozulur | Cooldown maliyetini karşılamaz → Ağır anlamsız |
| `autobattle_hp_threshold` | 0.40 | 0.25–0.60 | Çok sık iyileştirme → savaş uzar | İyileştirme çok geç → gereksiz ölüm riski |

**Etkileşim Uyarıları:**
- `cd_heavy` (3) ile hedef savaş süresi (5-8 tur): Ağır Saldırı 1-2 kez kullanılır. CD=2'ye indirilirse savaş başına 2-4 kez → sınıf ayrımı ve yetenek tatmini azalır.
- `cd_special` (8) savaş süresinin üst sınırına (8 tur) eşit: Bazı savaşlarda Özel Yetenek hiç kullanılmaz. CD=6'ya indirilirse savaş başına güvenilir 1 kullanım sağlanır — "efsane an" hissi güçlenir ama nadir hissi azalır.
- `cd_pet` (4) ile `cd_heavy` (3): Her 4 turda bir pet yeteneği, her 3 turda bir Ağır Saldırı — birbirinden bağımsız. Çakışma savaşı özellikle yoğun hissettirebilir (pozitif).

## Visual/Audio Requirements

### VFX Gereksinimleri

| Yetenek / Olay | VFX | Süre | Öncelik |
|----------------|-----|------|---------|
| Normal Saldırı (Slot 0) | Hafif slash efekti + beyaz hasar sayısı | 0.4s | MVP |
| Ağır Saldırı (Slot 1) | Güçlü slash + darbe dalgası + element rengi + ekran hafif sarsıntısı | 0.8s | MVP |
| Slot 2 (iyileştirme türü) | Yeşil ışınlar + HP barı dolma + "+X HP" yazısı | 0.6s | MVP |
| Slot 2 (buff türü) | Altın aura burst + stat artış ikonu | 0.5s | MVP |
| Slot 3 (Özel Yetenek) | Sınıfa özgü büyük efekt — hasar türü: büyük enerji patlaması; destek türü: saha kaplayan renk dalgası | 1.2s | MVP |
| Pet — Güçlü Vuruş | Saldırgan pet saldırı flash + darbe efekti | 0.7s | MVP |
| Pet — Koruma Duruşu | Kalkan büyüme + DEF buff aura (2 tur sürekli) | 0.5s aktif + sürekli | MVP |
| Pet — İyileştirme | Yeşil ışınlar + HP barı dolma | 0.6s | MVP |
| Pet — Element Dalgası | Sahneyi kaplayan renk dalgası, tüm düşmanlara aynı anda | 1.0s | MVP |
| Cooldown dolma | Buton rengi koyulaşır (gri→aktif), buton hafif pulse | 0.3s | MVP |
| Slot 3 hazır (CD=0) | Ekstra parlama efekti — diğer slotlardan belirgin | 0.5s | MVP |

### Audio Gereksinimleri

| Yetenek / Olay | Ses | Ton | Öncelik |
|----------------|-----|-----|---------|
| Normal Saldırı | Kısa kesik darbe | Nötr, sık tekrarda yıkıcı değil | MVP |
| Ağır Saldırı | Ağır yüklü darbe + titreşim | Güçlü, tatmin edici | MVP |
| Slot 2 — iyileştirme | Yükselen parlak chime | Rahatlatıcı | MVP |
| Slot 3 — Özel Yetenek | Sınıfa özgü imza sesi — tur döngüsünün en belirgin sesi | Dramatik, tanınabilir | MVP |
| Pet yeteneği | Normal saldırıdan ayrıştırılabilen ama daha sessiz ses | İkincil, oyuncunun aksiyonunu gölgede bırakmaz | MVP |
| Cooldown dolma | Kısa yükselen "ping" | Ödüllendirici, dikkat çekici | MVP |

*`art-director` not consulted — Lean mode. Review manually before production.*

> **Asset Spec Flag**: Visual/Audio gereksinimleri tanımlandı. Art bible onaylandıktan sonra `/asset-spec system:yetenek-sistemi` çalıştırarak yetenek VFX ve ses asset spesifikasyonları üretilebilir.

## UI Requirements

### Yetenek Butonları (Komutan Modu)

- Ekranın alt kısmında yatay dizi: 4 oyuncu slot butonu
- Her buton: yetenek ikonu + CD sayacı (kalan tur, büyük font)
- **Hazır (CD=0):** Tam parlak, dokunulabilir
- **Bekleme (CD>0):** Soluk + kalan tur sayısı (1, 2, 3...) buton üstünde
- **Slot 3 (Özel Yetenek) CD=0:** Diğer slotlardan belirgin — ekstra parlama çerçevesi
- Minimum dokunma hedefi: 64×64 dp (savaş sırasında hızlı dokunma)
- Oto-savaş modunda: butonlar gizlenir veya küçülür (hâlâ cooldown barı gösterilir)

### Pet Göstergesi

- Pet yeteneği UI'da doğrudan gösterilmez — pet otomatik hareket eder
- Opsiyonel: Pet portresinin yanında küçük CD göstergesi (Savaş UI GDD kararı)

### Cooldown Görsel Dili

- CD sayacı büyük ve okunabilir — oyuncu birden fazla butona bakarak karar verir
- Sayaç azaldıkça renk değişimi (kırmızı → sarı → yeşil) opsiyonel, Savaş UI GDD'de karara bağlanır
- Tüm eleman minimum 44×44 dp (technical-preferences.md)

> **UX Flag — Yetenek Sistemi**: Bu sistem kapsamlı buton ve cooldown UI gereksinimleri içeriyor. Phase 4'te `/ux-design` çalıştırarak yetenek butonları, CD gösterimi ve pet paneli için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** savaş başladığında, **WHEN** ilk tur gelirse, **THEN** tüm 4 slot cooldown_remaining=0 (tüm yetenekler hazır).

2. **GIVEN** Ağır Saldırı kullanılırsa (CD=3), **WHEN** 3 tur geçerse, **THEN** 4. turun başında cooldown_remaining=0 — yetenek tekrar hazır.

3. **GIVEN** Normal Saldırı (CD=0), **WHEN** kullanılırsa, **THEN** cooldown_remaining bir sonraki turda 0 kalır.

4. **GIVEN** Saldırgan pet (ATK=35, CD=4) yeteneği kullanılırsa, **WHEN** CD=0 olduğunda, **THEN** hasar = max(1, floor(35×2.0 - floor(enemy_DEF/2)) × element_multiplier).

5. **GIVEN** Büyücü pet Element Dalgası (ATK=35, multiplier=0.75) vs 3 düşman (DEF: 35, 15, 17), nötr, **WHEN** yetenek kullanılırsa, **THEN** her hedefe ayrı hesap: 11, 21, 20.

6. **GIVEN** oto-savaş, oyuncu HP=%35, Slot 2 iyileştirme türü ve CD=0, Slot 1 ve Slot 3 da hazır, **WHEN** oyuncu turu gelirse, **THEN** Slot 2 seçilir (HP < %40 kontrolü Slot 3 ve Slot 1'den önce).

7. **GIVEN** komutan modunda oyuncu 1 tur boyunca hiçbir butona basmazsa, **WHEN** tur fallback süresi dolunca, **THEN** Slot 0 (Normal Saldırı) otomatik çalışır.

8. **GIVEN** Slot 2 ve Slot 3 tanımsız (sınıf atanmamış), **WHEN** savaş başlarsa, **THEN** bu slotlar devre dışı gösterilir (gri), Slot 0 ve Slot 1 aktif çalışır.

9. **GIVEN** pet ve oyuncu aynı turda son düşmanı hedef alırsa, **WHEN** oyuncu aksiyonu önce işlenirse, **THEN** düşman savaş dışı kalır, pet aksiyonu iptal edilir.

10. **GIVEN** boss öfke fazında düşman CD -1 alırsa ve bir yeteneğin CD=0 ise, **WHEN** sonraki tur gelirse, **THEN** CD max(0, 0-1) = 0 — düşman yine her tur kullanır, alt sınır aşılmaz.

*`qa-lead` not consulted — Lean mode. Review manually before production.*

## Open Questions

1. **CD=8 Özel Yetenek, 5-8 tur savaşta yeterince görülüyor mu?** Tabloya göre 0-1 kez kullanılır — "efsane an" mı, yoksa "asla göremiyorum" mu hissi verir? → Playtest verisiyle karar. CD=6 alternatif eşik.

2. **Pet cooldown UI'a yansıtılmalı mı?** Şu an pet CD durumu oyuncuya gösterilmiyor. Savaş UI GDD'sinde karara bağlanacak.

3. **Slot 2 tipi sınıfa göre değişince oto-savaş mantığı nasıl güncellenir?** HP threshold kontrolü yalnızca iyileştirme türü Slot 2 için geçerli. Oyuncu Sınıf GDD'si tamamlandığında `IsHealSlot(class, slot)` kontrolü eklenmeli.

4. **Komutan modu fallback süresi ne kadar?** Şu an "tur sonunda fallback" — tur kaç saniye sürer? 1x hızda ~2 saniye: oyuncu 2 saniyede basmazsa Normal Saldırı mı tetiklenir? → Hibrit Savaş hız tablosuyla koordine edilmeli.

5. **Düşman boss CD-1 öfke kuralı bu GDD'den mi, Düşman AI GDD'sinden mi yönetilmeli?** Çerçeve burada, uygulama orada — çakışma yok ama net sorumluluk ayrımı Düşman AI GDD revizyonunda doğrulanacak.
