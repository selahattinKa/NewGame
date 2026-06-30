# UI Framework

> **Status**: Approved (3rd pass — 8 blockers resolved, 2 ADRs pending before implementation)
> **Author**: User + Claude Code agents
> **Last Updated**: 2026-06-26
> **Implements Pillar**: Pillar 5 (Hep Bir Şey Var) — sezgisel navigasyon; Pillar 4 (Güç Hisset) — akıcı geçişler

## Overview

UI Framework, Canavar Zindanları'nın tüm ekran yönetiminin temel katmanıdır. Ekranlar arası geçiş, modal diyalog, yığın (stack) tabanlı navigasyon ve oyun içi bildirim sistemini tek noktadan yönetir. Bu sistem doğrudan oyuncu etkileşimine konu olmaz — Savaş UI, Koleksiyon/Envanter UI ve Zindan Harita UI'nin üzerine kurulduğu altyapıdır. Unity 6.3 LTS'in önerdiği UI Toolkit (UXML/USS) mimarisini kullanır; tüm bileşenler touch-first, 44×44 dp minimum dokunma hedefine uygun tasarlanır. Bu katman olmadan oyunun hiçbir ekranı render edilemez.

## Player Fantasy

UI Framework oyuncunun bilinçli olarak fark etmediği bir sistemdir — bu onun başarısının kanıtıdır. Oyuncu Savaş UI'nin pürüzsüz geçişlerini, Zindan Haritası'nın anlık açılmasını ve bildirimlerin zamanında belirmesini "iyi hissettiren oyun" olarak algılar, ama altta UI Framework'ü görmez. Bu sistemin ürettiği hisler **üst UI sistemlerine aittir**: hızlı açılan koleksiyon ekranı, anlık loot bildirimi, geri tuşuyla sezgisel navigasyon. UI Framework başarılıysa oyuncu yalnızca içeriği hisseder, altyapıyı değil.

*(Saf altyapı — oyuncu bu sistemi doğrudan hissetmez; Savaş UI, Koleksiyon UI ve Zindan Harita UI üst katmanları player fantasy'yi teslim eder.)*

## Detailed Rules

### Core Rules

1. **Navigasyon Modeli (Hibrit):** Alt sekme çubuğu (Tab Bar) ana bölümleri yönetir; her bölümün içinde bağımsız bir ekran yığını (Screen Stack) çalışır. **Aktif sekmede aynı sekmeye tekrar basmak** yığını köke sıfırlar (iç yığın tamamen temizlenir). **Farklı sekmeden bu sekmeye geçmek** kaldığı yerden devam ettirir — sekmenin yığın durumu korunur, sıfırlanmaz.

2. **Sekme Barı Bölümleri (MVP):**
   - **Ana** — oyuna giriş ekranı / idle özet
   - **Zindan** — zindan seçim + harita (Zindan Harita UI)
   - **Koleksiyon** — canavar koleksiyonu + envanter (Koleksiyon/Envanter UI)
   - **Takım** — takım kurma ve savaş başlatma (Savaş UI)
   - Uygulama ilk açıldığında aktif sekme **Ana** sekmesidir.

3. **Ekran Yığını Kuralları:**
   - Bir ekrana gidildiğinde yığına eklenir (push).
   - Geri navigasyonu yığından çıkarır (pop). Yığın boşsa sekme kökündeyiz — geri navigasyon yok.
   - Modal/panel açıldığında yığın dondurulur (modal kapanana kadar yığın pop edilemez).
   - Sekme geçişi aktif sekmenin yığın durumunu korur (sekmeye geri dönünce kaldığı yerden devam eder).

4. **Modal Sistemi:**
   - Modallar yığının üstüne bağımsız katman olarak açılır.
   - **API imzası:** `UIManager.ShowModal(VisualTreeAsset modal, object context, Action<bool> onClose, bool priority = false): bool` — `modal` UXML asset referansı; `context` modal içeriğinin ihtiyaç duyduğu veri (canavar ID, zindan bilgisi vb.); `onClose` kapanışta tetiklenen callback (`true` = onaylandı, `false` = iptal edildi); `priority = true` modal kuyruğun başına geçer (diğer normal modallerin önüne alınır). **Dönüş değeri:** `true` = kuyruğa alındı veya anında açıldı; `false` = kuyruk dolu, talep reddedildi. Çağıran sistem `false` alırsa kullanıcıya alternatif bildirim kanalı (toast veya yeniden deneme) sağlamalıdır.
   - Aynı anda yalnızca 1 modal aktif olabilir. İkinci modal talebi kuyruğa alınır.
   - **Maksimum kapasite: 1 aktif + 4 kuyrukta = toplam 5.** Kapasite doluyken yeni `ShowModal()` talebi reddedilir ve `false` döner — eski talepler korunur. Priority modal (`priority = true`) doluyken dahi kuyruğun başına yerleştirilebilir; bu durumda kuyruğun son (en eski) elemanı çıkarılır.
   - Modal arkasında ekran karartma (dim overlay, opacity %60) zorunlu.
   - **`UIManager.ForceCloseActiveModal()`** — aktif modalı animasyonsuz `onClose(false)` ile kapatır; kuyrukta bekleyen modaller etkilenmez. Debug araçları veya kritik session reset akışları için; normal oyun kodu bu API'yi kullanmamalıdır. Açık modal yoksa sessizce hiçbir şey yapmaz.

5. **HUD Katmanı (Dinamik Görünürlük):**
   - HUD üst çubuğu altın, elmas, enerji değerlerini gösterir.
   - Her ekran kendi açılışında HUD görünürlüğünü belirtir: `ShowHUD()` veya `HideHUD()`.
   - Varsayılan: HUD görünür. Savaş ekranı açılırken HUD gizlenir (savaşın kendi HUD'u devralır).
   - HUD değerleri canlı güncellenir; herhangi bir kaynak değişiminde tüm aktif HUD bileşenleri anlık yenilenir.

6. **Bildirim/Toast Sistemi (Kuyruklu):**
   - Toast bildirimleri FIFO kuyrukta bekler; 1 toast ekrandayken sonraki tetiklense bile kuyrukta bekler.
   - Her toast 3 saniye ekranda kalır, ardından 0.4s fade animasyonuyla kaybolur. Bir sonraki toast fade **başladığı** anda (t=3.0s) kuyruktan alınır ve gösterilir — fade tamamlanmasını beklenmez.
   - **Geçiş penceresi (t=3.0s–3.4s):** Toast B, Toast A ile aynı konumda (HUD altı) render edilir; A opacity 1→0 solar, B arkasında tam görünür (opacity 1.0) kalır. t=3.4s'de A VisualElement ağacından kaldırılır.
   - Toast ekranın üst kısmında görünür, HUD çubuğunun altında.
   - Toast türleri: `info` (mavi), `success` (yeşil), `warning` (turuncu). Her biri farklı ikon.
   - Savaş ekranında toastlar gizlenir — savaş kendi feedback sistemini kullanır.

7. **Geçiş Animasyonları:**
   - Sekme geçişi: yatay kaydırma (300ms, ease-out).
   - Stack push: yeni ekran soldan girer (250ms).
   - Stack pop: aktif ekran sola çıkar (200ms, ease-in).
   - Modal aç: alttan yukarı (250ms, spring easing). Kapat: aşağı kayar (200ms).
   - Tüm animasyonlar oyun içi erişilebilirlik ayarından kapatılabilir (Ayarlar → Azaltılmış Hareket toggle'ı, `PlayerPrefs` bool). OS-seviye otomatik tespit yapılmaz.

8. **Touch Hedefleri:** Her etkileşimli eleman minimum 44×44 dp. Dokunma hedeflerinin aralarında minimum 8 dp boşluk.

9. **Tab Bar Kilitleme (Yalnızca Aktif Savaş Ekranı):**
   - Savaş ekranı açılırken `UIManager.LockTabs()` çağrılır; sekme barı **disabled** duruma geçer — ikonlar gri/pasif görünür, dokunuşlara yanıt vermez.
   - Kilit durumunda herhangi bir sekme ikonuna dokunulunca hiçbir navigasyon gerçekleşmez; UI sessizce olayı yok sayar.
   - Savaş ekranı kapanırken `UIManager.UnlockTabs()` çağrılır; sekme barı normal etkileşimli durumuna döner.
   - **Otofarm modu `LockTabs()` çağırmaz.** Otofarm arkaplanda çalışırken sekme barı serbest kalır — oyuncu Koleksiyon, Zindan veya Takım sekmesine geçebilir. (Pillar 3 "Senin Tempon" uyumluluğu.)
   - `LockTabs()` / `UnlockTabs()` çifti reference count ile yönetilir (`_lockCount++` / `_lockCount = max(0, _lockCount-1)`). Bu şekilde birden fazla caller aynı anda kilit telebinde bulunursa yalnızca tüm callerlar `UnlockTabs()` çağırdığında bar açılır. (MVP'de tek caller: savaş ekranı — ancak mimari ilerideki edge case'lere karşı güvenlidir.)

**UIManager Query & Control API (test ve runtime introspection):**
- `UIManager.GetTopScreen(TabIndex tab): VisualElement` — belirtilen sekmenin yığın başındaki **instantiate edilmiş** ekranı döner (asset referansı değil, canlı element)
- `UIManager.GetStackDepth(TabIndex tab): int` — belirtilen sekmenin yığın derinliğini döner
- `UIManager.IsModalOpen(): bool` — herhangi bir modal açık mı
- `UIManager.GetModalQueueCount(): int` — kuyrukta bekleyen modal sayısı
- `UIManager.GetToastQueueCount(): int` — kuyrukta bekleyen toast sayısı
- `UIManager.IsTabsLocked(): bool` — tab bar kilitli mi
- `UIManager.ReducedMotionEnabled: bool` (get/set) — `PlayerPrefs.GetInt("accessibility_reduced_motion", 0) == 1` ile başlatılır; test ortamında doğrudan `UIManager.ReducedMotionEnabled = true` ile override edilebilir
- `UIManager.ShowFullscreenReveal(RewardData data)` — **API stub** (tam implementasyon `/design-system idle-reward-reveal` oturumunda tasarlanacak). Otofarm/idle ödül reveal akışı (devamsızlıktan-dönüş) için ayrılmış kanal. `RewardData` placeholder: `List<LootItem> rewards`, `string title`. Bu API tanımlanana kadar Otofarm sistemi `ShowToast()` kullanmaz — reveal akışı bu stub üzerinden bağlanır.

---

### UIDocument Topolojisi

UI Framework 4 bağımsız UIDocument katmanı kullanır. Her katmanın `PanelSettings` sort order değeri z-sıralamasını ve input önceliğini belirler:

| Katman | UIDocument | Sort Order | İçerik |
|--------|-----------|-----------|--------|
| Game | `UIDocument_Game` | 0 | Sekme barı + ekran içeriği (tüm sekme stack'leri) |
| HUD | `UIDocument_HUD` | 10 | Üst HUD çubuğu (altın/elmas/enerji) |
| Modal | `UIDocument_Modal` | 20 | Dim overlay + modal paneli |
| Toast | `UIDocument_Toast` | 30 | Toast bildirimleri |

**Temel kararlar:**
- Dim overlay (sort 20 > HUD sort 10) HUD'u da örter — modal açıkken HUD görünür ama dokunulamaz.
- Sekme geçişinde aktif sekmenin container'ı `display: flex`, diğerleri `display: none` olur (state korunur).
- Stack push/pop: aktif sekmenin container'ı içinde VisualElement ekle/kaldır (root ekran kalıcı, push'lananlar geçici).
- `UIManager.NavigateTo(screen)` argümanı: `VisualTreeAsset` (UXML) referansıdır; `UIManager` instantiate eder.

**Safe area implementasyon kararı:** Her UIDocument bileşeni `Awake()`'de `Screen.safeArea` okur ve root VisualElement'e USS padding olarak uygular.
- `UIDocument_Game`: `style.paddingBottom = Screen.safeArea.yMin` — tab bar notch/gesture bar üzerinde kalır
- `UIDocument_HUD`: `style.paddingTop = Screen.height - Screen.safeArea.yMax` — status bar/notch altında kalır
- `UIDocument_Modal` ve `UIDocument_Toast`: safe area inset almaz (overlay tam ekran)
- Koordinat dönüşüm detayları ve PanelSettings scale mode ile ilişkisi `adr-ui-document-topology.md`'de belgelenir.

**Animasyon driver:** Tüm geçiş animasyonları (sekme, stack push/pop, modal) USS transition yerine **PrimeTween** ile yürütülür (Unity 6.3 native async, zero-alloc). USS cubic-bezier değerleri referans olarak belgelenmiştir; uygulama PrimeTween'in `Tween.Custom()` veya easing parametreli yöntemleriyle C# tarafında interpolasyon uygular. Gerekçe: (1) USS `display: none` transition desteklemez; (2) animasyon interrupt (snap-to-end) `Tween.Complete()` ile sağlanır; (3) `VisualElement.transform` Unity 6.3'te deprecated — yerine `style.translate` / `style.rotate` / `style.scale` kullanılır. **ADR yazılmalı (BLOCKING — implementasyon öncesi):** `docs/architecture/adr-ui-animation-driver.md`

**UIDocument topolojisi ADR:** 4 UIDocument mimarisi cross-panel draw call batching'i engeller (her panel için batch sıfırdan başlar). Bu mimari ≤40 DC sub-budget ile birlikte ADR olarak belgelenmeli (BLOCKING): `docs/architecture/adr-ui-document-topology.md`. Peak state DC tahmini (HUD + Modal + Toast aktif) implementasyon öncesi build üzerinde ölçülmeli.

**Dim overlay input blocking:** `UIDocument_Modal` (sort 20) dim overlay VisualElement'i **`pickingMode = PickingMode.Position`** olarak tanımlanmalıdır — bu olmadan sort ordering dokunma olaylarının altta kalan UIDocument katmanlarına geçmesini engellemez ve HUD modal arkasında dokunulabilir kalır. `UIDocument_Toast` (sort 30) tıklanabilir alan içermez; modal açıkken Toast layer'ı input tüketmez.

---

### States and Transitions

| Durum | Açıklama | Geçiş Koşulu |
|-------|----------|--------------|
| `TabBar_Active` | Sekme barı görünür, aktif sekmenin yığını aktif | Her zaman (başlangıç durumu) |
| `ScreenStack_Push` | Yeni ekran yığına eklendi | Herhangi bir NavigateTo() çağrısı |
| `ScreenStack_Pop` | Üst ekran yığından çıkarıldı | Geri navigasyon veya Back() çağrısı |
| `Modal_Open` | Modal açık, arka yığın donmuş | ShowModal() çağrısı |
| `Modal_Closed` | Modal kapandı, yığın donması çözüldü | Modal onClose callback tamamlandı |
| `HUD_Visible` | Üst HUD çubuğu aktif | ShowHUD() çağrısı veya varsayılan |
| `HUD_Hidden` | HUD gizli | HideHUD() çağrısı (örn. savaş ekranı) |
| `Toast_Showing` | Aktif toast ekranda | Kuyruğun başından toast alındı |
| `Toast_Queue` | Bekleyen toastlar kuyrukta | Kuyruk doluyken yeni toast eklendi |
| `TabBar_Locked` | Sekme barı görünür ama disabled/gri; dokunuşlar yok sayılır | UIManager.LockTabs() çağrısı (savaş / otofarm başlangıcı) |
| `TabBar_Unlocked` | Sekme barı normal etkileşimli duruma döner | UIManager.UnlockTabs() çağrısı (savaş / otofarm sonu) |

---

### Interactions with Other Systems

| Sistem | Veri Akışı | Yön | Arayüz |
|--------|-----------|-----|--------|
| **Savaş UI** | HideHUD() komutu gönderir; savaş başlarken tab bar kilitler, savaş bitince açar | → UI Framework | `UIManager.HideHUD()`, `UIManager.LockTabs()`, `UIManager.UnlockTabs()` |
| **Koleksiyon/Envanter UI** | Canavar detay ekranını yığına push eder; onay modalı açar | → UI Framework | `UIManager.NavigateTo(screen)`, `UIManager.ShowModal(modal)` |
| **Zindan Harita UI** | Zindan seçim ve kat ekranlarını push eder | → UI Framework | `UIManager.NavigateTo(screen)` |
| **Ekonomi Sistemi** | Altın, elmas, enerji güncel değeri | → HUD bileşeni | Event/callback: `OnResourceChanged(type, value)` |
| **Kaydetme/Yükleme** | Otofarm bildirimi (uyku sonrası) | → Toast kuyruğu | `UIManager.ShowToast(type, message)` |
| **Otofarm/Idle** | Idle ödül reveal akışı | → Fullscreen reveal | `UIManager.ShowFullscreenReveal(RewardData)` — toast DEĞİL |

## Formulas

UI Framework matematiksel hesaplama içermez; formüller bölümü üç kural kümesini kapsar: **gösterim formatlama**, **zamanlama sabitleri** ve **güncelleme sıklığı**.

---

**Altın Gösterge Formatı (GOLD_DISPLAY_FORMAT)**

```
GOLD_DISPLAY(V) =
  V < 1,000                          → str(V)                                               (örn. "847")
  1,000 ≤ V < 10,000                → format(floor(V / 100f) / 10f, "0.0") + "K"           (örn. "1.0K", "9.9K")
  10,000 ≤ V < 100,000              → format(floor(V / 100f) / 10f, "0.#") + "K"           (örn. "12.5K", "50K")
  100,000 ≤ V < 1,000,000           → str(floor(V / 1,000f)) + "K"                         (örn. "350K")
  1,000,000 ≤ V < 10,000,000        → format(floor(V / 100,000f) / 10f, "0.0") + "M"      (örn. "1.0M", "1.7M")
  10,000,000 ≤ V < 1,000,000,000    → str(V / 1_000_000L) + "M"                           (örn. "25M")
  V ≥ 1,000,000,000                 → format(floor(V / 100,000,000f) / 10f, "0.0") + "B"  (örn. "1.0B", "2.1B")
```

**C# uygulama notu:** `V` `long` türündedir. `format(val, "0.0")` = her zaman 1 ondalık (1.0K); `format(val, "0.#")` = ondalık sıfırsa gösterme (50K, değilse 12.5K). Decimal separator: invariant culture (`CultureInfo.InvariantCulture`) — TR locale'de nokta yerine virgül çıkmasını önler. **`int` yerine `long` zorunlu:** idle RPG birikimi int.MaxValue (~2.1B) sınırını aşar; C# unchecked overflow negatife sarmalar ve HUD "0" gösterir.

**Float precision uyarısı (Band 5 ve 6):** `long / float` işleminde `float32` 23-bit mantissa sınırı nedeniyle V ≈ 10⁹ için 64-birim yuvarlama hatası oluşur: `(float)999_999_999` → 1,000,000,000f → "1000M" (yanlış). **Düzeltme:** Band 6 (`10M ≤ V < 1B`) için `V / 1_000_000L` (integer bölme, kesin). Ondalıklı bandlar (Band 4 ve Band 5) için: `(V / 100_000L) / 10f` — böylece `long` bölmesi küçük sayıya indirir, float precision sorunu oluşmaz.

| Sembol | Tür | Aralık | Açıklama |
|--------|-----|--------|----------|
| V | long | 0 – ∞ | Ham altın değeri (`long` — int.MaxValue ~2.1B'de overflow'u önler) |
| T_K | const long | 1,000 | K kısaltma eşiği |
| T_M | const long | 1,000,000 | M kısaltma eşiği |
| T_B | const long | 1,000,000,000 | B kısaltma eşiği |
| result | string | "0" – "∞B" | HUD'da gösterilecek metin (invariant culture) |

**Çıktı örnekleri:** `847` → "847" | `1,000` → "1.0K" | `9,999` → "9.9K" | `12,500` → "12.5K" | `50,000` → "50K" | `350,000` → "350K" | `1,000,000` → "1.0M" | `1,750,000` → "1.7M" | `25,000,000` → "25M" | `1,000,000,000` → "1.0B" | `2,147,483,647` → "2.1B"

**Elmas ve Enerji Formatı**

| Kaynak | Format | Örnek | Açıklama |
|--------|--------|-------|----------|
| Elmas | `str(V)` | "247" | Tam sayı, kısaltma yok (aralık 0–9,999) |
| Enerji | `str(E) + "/" + str(MAX_ENERGY)` | "75/100" | Kapasite referansını oyuncuya gösterir |

| Sembol | Tür | Aralık | Açıklama |
|--------|-----|--------|----------|
| V | int | 0 – 9,999 | Ham elmas değeri |
| E | int | 0 – 100 | Güncel enerji; `max_energy` registry sabiti = 100 |

---

**Zamanlama Sabitleri**

| Sabit | Değer | Birim | Açıklama |
|-------|-------|-------|----------|
| `toast_display_duration` | 3.0 | saniye | Toast ekranda kalma süresi (sabit — mesaj uzunluğuna bağlı değil) |
| `toast_fade_duration` | 0.4 | saniye | Toast solma animasyon süresi (display süresine dahil değil) |
| `modal_open_duration` | 0.25 | saniye | Modal açılma animasyonu (alttan yukarı, spring easing) |
| `modal_close_duration` | 0.20 | saniye | Modal kapanma animasyonu |
| `tab_transition_duration` | 0.30 | saniye | Sekme geçiş süresi (yatay kaydırma) |
| `screen_push_duration` | 0.25 | saniye | Stack push animasyonu |
| `screen_pop_duration` | 0.20 | saniye | Stack pop animasyonu |
| `dim_overlay_opacity` | 0.60 | 0–1 | Modal arka plan karartma yoğunluğu |

---

**HUD Güncelleme Frekansı**

```
hud_update = throttle_first(OnResourceChanged, window: 100ms)
```

| Değişken | Değer | Açıklama |
|----------|-------|----------|
| `hud_throttle_first_ms` | 100 | **Leading throttle:** Event geldiği anda (t=0ms) hemen HUD güncellenir, ardından 100ms pencere açılır. Pencere boyunca gelen tüm sonraki eventler yoksayılır; pencere sıfırlanmaz. Pencere kapanınca (t=100ms) sistem hazır, bir sonraki event yeniden anında render tetikler. Trailing debounce DEĞİL. Loot patlamasında (50 event/100ms) ilk event anında render + kalan 49 yoksayılır = tek ekran güncellemesi. **Callback, fire anında `Economy.GetCurrentGold()` okur — event payload önbelleğine güvenmez.** |

**Çıktı aralığı:** Her kaynak değişiminde 0–1 HUD yenileme (throttle window sonrası).
**Örnek:** Aynı turda 3 farklı altın kazanımı gelirse → 100ms sonunda tek HUD güncellemesi.

---

**Animasyon Easing Eğrileri (USS cubic-bezier)**

| Animasyon | Easing Tipi | cubic-bezier(P1x, P1y, P2x, P2y) | Hissi |
|-----------|-------------|-----------------------------------|-------|
| Sekme geçişi | ease-out | `cubic-bezier(0.0, 0.0, 0.2, 1.0)` | Hızlı başlar, yavaşlayarak durur |
| Stack push | ease-out | `cubic-bezier(0.0, 0.0, 0.2, 1.0)` | Hızlı girer, yumuşak oturur |
| Stack pop | ease-in | `cubic-bezier(0.4, 0.0, 1.0, 1.0)` | İvmelenerek çıkar |
| Modal aç | spring (approx) | `cubic-bezier(0.34, 1.56, 0.64, 1.0)` | Hafif overshoot → geri gelir (zımba hissi) |
| Modal kapat | ease-in | `cubic-bezier(0.4, 0.0, 1.0, 1.0)` | Hızlı kapanır |

**Not:** UI Toolkit CSS cubic-bezier destekler, fizik spring desteklemez. Y>1.0 değeri overshoot simüle eder. `reduced_motion` açıkken tüm easing değerleri yoksayılır, animasyon süresi 0ms olur.

## Edge Cases

- **Eğer uygulama savaş ekranındayken bir toast tetiklenirse:** Toast kuyruğa alınır, savaş ekranı kapatılınca (veya oyuncu menüye dönünce) kuyruktaki toast gösterilir. Savaş sırasında hiçbir toast ekranda beliremez.

- **Eğer oyuncu modal açıkken geri tuşuna basarsa:** Modal `onClose(false)` olarak tetiklenir (`false` = iptal) ve modal kapanır. Arka plandaki stack yığını değişmez.

- **Eğer 2. bir modal açma talebi gelirken 1. modal hâlâ açıksa:** İkinci modal talebi kuyruğa alınır. Birinci modal kapanır kapanmaz ikincisi açılır. Üçüncü ve sonraki talepler de kuyruğa eklenir — **maksimum toplam kapasite 5 (1 aktif + 4 kuyrukta)**. Kapasite doluyken yeni `ShowModal()` talebi sessizce reddedilir; mevcut 5 modal'ın sırası değişmez.

- **Eğer oyuncu aynı sekme butonuna tekrar basarsa:** Sekmenin yığını kök ekrana (depth 0) sıfırlanır. Yığında birden fazla ekran varsa tümü pop edilir. Kök ekrandaysa hiçbir şey olmaz.

- **Eğer HUD, kaynak güncellemesini debounce süresinde 5'ten fazla kez alırsa:** Debounce penceresi sıfırlanmaz — hâlâ ilk tetiklenme anından 100ms sonra güncelleme yapılır. Patlama halindeki loot (örn. 50 kat sonucu aynı anda) tek bir HUD yenilemeyle karşılanır.

- **Eğer network gecikmesi nedeniyle kaynak değeri negatif bir değer alırsa:** HUD `max(0, raw_value)` ile sınırlandırır; negatif gösterim yapılmaz. Bu durumu game sistemleri zaten önlemeli — UI sadece savunma katmanıdır.

- **Eğer animasyon oynatılırken sekme geçişi gelirse:** Devam eden animasyon anında tamamlanmış kabul edilir (snap), yeni sekme geçiş animasyonu başlar. Animasyonlar kesilmez — hızlı ardışık dokunuşlarda biriken gecikme oluşmaz.

- **Eğer cihaz ekran boyutu minimum desteklenen değerin altındaysa (< 320dp genişlik):** UI elemanları %90 ölçeğe küçülür; 44dp minimum dokunma hedefi korunur. Alt sınır 280dp. Daha küçük ekranlarda "Cihazınız desteklenmiyor" mesajı gösterilir.

- **Eğer oyun içi "Azaltılmış Hareket" toggle'ı açıksa (Ayarlar → erişilebilirlik, `PlayerPrefs` bool):** Tüm geçiş animasyonları (sekme, stack, modal) anlık snap ile değiştirilir; zamanlama sabitleri 0ms'ye düşer. Toast hâlâ 3 saniye görünür kalır ancak fade animasyonu olmaz. OS seviyesinde otomatik tespit yapılmaz — kullanıcı manuel açmalıdır.

- **Eğer toast kuyruğunda zaten 3 toast bekleniyorken yeni toast tetiklenirse:** En eski kuyruk elemanı sessizce iptal edilir, yeni toast kuyruğun sonuna eklenir (`toast_queue_max = 3`). Önemli bildirimler (modal yoluyla iletilmeli) hiçbir zaman toast olarak gönderilmemelidir.

- **Toast A fade animasyonu (t=3.0s–3.4s) sırasında dokunma input'u:** Toast A'nın opacity 1→0 ile solduğu geçiş süresinde A VisualElement'inin `pickingMode`, fade başladığı anda `PickingMode.Ignore` olarak değiştirilir. Bu olmadan opacity=0'a yakın A, B'ye yönelik dokunuşları tüketebilir. t=3.4s'de A tamamen VisualElement ağacından kaldırılır.

- **Eğer elmas değeri 9,999 sınırını aşarsa:** Ekonomi sistemi hard cap garantisi sağlar (elmas ≥10,000 mümkün değildir). Bununla birlikte HUD savunma katmanı olarak `min(V, 9999)` ile sınırlandırır; 5 basamaklı değer hiçbir zaman HUD'a ulaşmaz. Olağandışı overflow durumunda "9999+" gösterimi uygulanır.

- **Eğer screen stack derinliği 8'e ulaşırsa:** Yeni `NavigateTo()` çağrısı yoksayılır ve bir hata loglanır. Stack, 8 derinlik sınırı içinde kalır (`stack_max_depth = 8`). Bu durum navigasyon döngüsü veya programlama hatası göstergesidir.

- **Eğer bir modal `onClose` callback'ini hiç tetiklemezse:** Modal timeout yoktur (`modal_timeout = 0` — süresiz bekler). Oyun sistemi modalı açmakla sorumludur; kapatmakla da sorumludur. Timeout'suz tasarım, yanlışlıkla modalı kapatan zamanlayıcılar yerine belirleyici kontrol akışı sağlar. **Kurtarma mekanizması:** `UIManager.ForceCloseActiveModal()` debug veya session reset senaryolarında çağrılabilir.

- **Eğer kilitli tab bar durumunda (`TabBar_Locked`) bir sekme ikonuna dokunulursa:** Hiçbir navigasyon gerçekleşmez; UI sessizce olayı yok sayar. Sekme barı gri/disabled görünmeye devam eder; `UIManager.IsTabsLocked() == true` kalır.

- **Eğer uygulama `LockTabs()` aktifken arka plana alınır veya OS tarafından sonlandırılırsa:** Kaydetme/Yükleme sistemi savaş durumunu restore ederse → Kaydetme sistemi `UIManager.LockTabs()`'ı yeniden çağırmalıdır. Savaş durumu restore edilmezse → `UIManager.UnlockTabs()` çağrılır ve normal duruma dönülür. UIManager yalnızca API'yi sağlar; restore kararı Kaydetme/Yükleme sistemine aittir.

- **Eğer `UIManager.ForceCloseActiveModal()` açık modal yokken çağrılırsa:** Sessizce hiçbir şey yapılmaz; hata veya çökme olmaz; kuyruk durumu değişmez.

## Dependencies

### Upstream (UI Framework'ün bağlı olduğu sistemler)

UI Framework Foundation katmanındadır — hiçbir oyun sistemine bağlı değildir. Yalnızca Unity 6.3 LTS engine altyapısına (UI Toolkit / UXML / USS) bağımlıdır.

### Downstream (UI Framework'e bağlı sistemler)

| Sistem | Bağımlılık Türü | UI Framework'ten Beklentisi |
|--------|-----------------|----------------------------|
| **Savaş UI** | Sert (olmadan çalışamaz) | NavigateTo(), HideHUD(), LockTabs(), ShowModal() |
| **Koleksiyon / Envanter UI** | Sert | NavigateTo(), ShowModal(), ShowHUD() |
| **Zindan Harita UI** | Sert | NavigateTo(), ShowHUD(), ShowToast() |
| **Ekonomi Sistemi** | Yumuşak (HUD kaynağını besler) | OnResourceChanged event — HUD bu eventi dinler |
| **Kaydetme / Yükleme** | Yumuşak (bildirim kanalı) | ShowToast() — idle birikimi bildirimi |
| **Otofarm / Idle** | Yumuşak (bildirim kanalı) | ShowToast() — "Loot hazır" bildirimi |

### Engine Bağımlılıkları

| Bağımlılık | Versiyon | Amaç |
|------------|----------|-------|
| Unity UI Toolkit | Unity 6.3 LTS built-in | UXML yapısı, USS stillemesi, runtime UI render |
| Unity Input System | 1.11+ | Touch input yönlendirmesi |
| **PrimeTween** | 1.2+ (üçüncü parti) | Geçiş animasyonları (USS transition yerine) — zero-alloc, snap-to-end (`Tween.Complete()`) |

## Tuning Knobs

| Knob | Varsayılan | Güvenli Aralık | Çok düşükse | Çok yüksekse |
|------|-----------|----------------|-------------|--------------|
| `toast_display_duration` | 3.0 s | 1.5 – 5.0 s | Oyuncu bildirimi okuyamadan kaybolur | Bildirimlerin kaybolması için çok uzun bekler, kuyruk birikimine yol açar |
| `toast_fade_duration` | 0.4 s | 0.1 – 0.8 s | Ani kesme — kaba hissettir | Çok yavaş — bir sonraki toast gecikmeli başlar |
| `dim_overlay_opacity` | 0.60 | 0.40 – 0.85 | Arka plan çok görünür, modal içeriğine odaklanmak zorlaşır | Arka plan tamamen siyah — oyuncu nerede olduğunu kaybeder |
| `modal_queue_max_depth` | 5 | 3 – 10 | Modaller atlanır, oyuncu önemli bildirimleri kaçırır | Kuyruk çok büyür, eski modaller alakasız hale gelir |
| `hud_throttle_first_ms` | 100 ms | 50 – 300 ms | Her küçük değişimde HUD yenilenir — mobilde gereksiz render | HUD 300ms geriye düşer, kaynak değişimi gecikmiş görünür |
| `tab_transition_duration` | 300 ms | 150 – 500 ms | Animasyon çok hızlı — sekme geçişi sert hissettir | Navigasyon ağır ve yavaş hissettir |
| `screen_push_duration` | 250 ms | 150 – 400 ms | Ekran geçişleri sert | Ekran geçişleri oyun akışını yavaşlatır |
| `min_touch_target_dp` | 44 dp | 44 – ∞ dp | **44dp altına düşürülemez** — erişilebilirlik standardı | Daha büyük hedefler her zaman iyidir |
| `toast_queue_max` | 3 | 1 – 8 | Bildirimler hızla kaybolur, oyuncu önemli toast'ı kaçırır | Kuyruk çok büyür, eski toastlar alakasız hale gelir |
| `stack_max_depth` | 8 | 4 – 15 | Derin navigasyon akışları kesilebilir | Sonsuz navigasyon döngüleri yakalanmaz |

**Not:** Animasyon sürelerini değiştirmek tüm UI sistemlerini etkiler (Savaş UI, Koleksiyon UI, Zindan Harita UI). Tek bir değişiklik tüm oyunun UI hızını değiştirir — dikkatli tuning gerektirir.

## Visual/Audio Requirements

### Görsel Stil Kuralları (UI Framework Düzeyinde)

- **Renk paleti:** Art Bible'dan türetilir. Birincil: koyu arka plan (#1A1A2E bölgesi), accent: altın/sarı (#F4C430 bölgesi), tehlike: kırmızı (#C0392B), başarı: yeşil (#27AE60), bilgi: mavi (#2980B9).
- **Font:** Tüm UI bileşenlerinde tek tip font ailesi (Art Bible'dan belirlenecek). Minimum font boyutu 14sp (mobil okunabilirlik standardı).
- **Toast ikonları:** info = ℹ️ ikonu, success = ✓ ikonu, warning = ⚠️ ikonu. Renkli arka plan bandı.
- **Dim overlay:** Düz siyah (#000000), opacity 0.60. Özel doku veya blur yok — draw call bütçesini zorlamaz.
- **Tab bar:** Alt kısımda sabit, aktif sekme accent rengiyle vurgulanır. Sekme ikonu + etiket metni.

### Ses Gereksinimleri

- **Tab geçişi:** Hafif, sert olmayan tık sesi (opsiyonel — MVP'de sessiz olabilir).
- **Modal açılış:** Yumuşak bir "whoosh" sesi (opsiyonel — MVP'de sessiz olabilir).
- **Toast (success):** Kısa, tatmin edici "ding" sesi.
- **Toast (warning):** Hafif "buzz" sesi.
- Tüm UI sesleri müzik seviyesinden bağımsız SFX kanalında çalar. Kapatılabilir.

## UI Requirements

UI Framework'ün kendisi üst-düzey UI'ı tanımlamaz — Savaş UI, Koleksiyon UI ve Zindan Harita UI bunu yapar. Ancak Framework şu standartları tüm ekranlara zorunlu kılar:

- **Tab Bar yüksekliği:** 56dp (standart Android/iOS bottom nav yüksekliği).
- **HUD üst çubuğu yüksekliği:** 48dp. Sol: altın göstergesi. Orta: enerji çubuğu. Sağ: elmas göstergesi.
- **Safe area:** iOS notch ve Android navigation bar safe area'larına saygı gösterilir — UI elemanları safe area içine yerleştirilir.
- **Modal genişliği:** Ekran genişliğinin %90'ı, maksimum 360dp. Dikey ortalanmış.
- **Toast genişliği:** Ekran genişliğinin %85'i, HUD çubuğunun 8dp altında başlar.
- **Scroll:** Her ekran dikey scroll destekleyebilir; yatay scroll yalnızca koleksiyon grid'inde kullanılır.

## Acceptance Criteria

### A. Navigasyon ve Sekme Sistemi
*Logic (state machine) — BLOCKING*

- **AC-NAV-01** — **GIVEN** kullanıcı Koleksiyon sekmesinde 2 ekran derinliğinde (Koleksiyon Kök → Canavar Detay) iken, **WHEN** Zindan sekme ikonuna dokunur, **THEN** Zindan sekmesinin kök ekranı **300ms ±16ms** içinde aktif olur; Koleksiyon sekmesinin stack derinliği programatik incelemede 2 olarak doğrulanır ve alt sekme barında Zindan aktif işaretlenir.

- **AC-NAV-02** — **GIVEN** kullanıcı Zindan sekmesinde Zindan Kat ekranındayken Ana sekmeye geçmiş, **WHEN** tekrar Zindan sekme ikonuna dokunur, **THEN** Zindan sekmesi Zindan Kat ekranını gösterir — kök Zindan Seçim ekranına sıfırlanmaz.

- **AC-NAV-03** — **GIVEN** uygulama ilk açıldığında, **WHEN** ana ekran render edilir, **THEN** alt sekme barında soldan sağa sırayla Ana, Zindan, Koleksiyon, Takım sekme ikonları görünür. Başka sekme ikonu bulunmaz.

- **AC-NAV-04** — **GIVEN** kullanıcı herhangi bir ekrandayken, **WHEN** `UIManager.NavigateTo(hedefEkran)` çağrılır, **THEN** hedef ekran 250ms push animasyonuyla (soldan girerek) stack'e eklenir ve geri navigasyon aktif olur.

- **AC-NAV-05** — **GIVEN** kullanıcı stack derinliği ≥ 2 olan ekranda iken, **WHEN** geri navigasyon tetiklenir, **THEN** üst ekran 200ms pop animasyonuyla (sola çıkarak) kaldırılır ve bir alt ekran görünür olur.

- **AC-NAV-06** — **GIVEN** kullanıcı sekmenin kök ekranında (stack derinliği = 0) iken, **WHEN** geri navigasyon tetiklenir, **THEN** hiçbir stack değişikliği olmaz; kullanıcı aynı ekranda kalır ve uygulama kapanmaz.

- **AC-NAV-07** — **GIVEN** bir modal açıkken, **WHEN** geri navigasyon tetiklenir, **THEN** stack pop gerçekleşmez; yalnızca modal kapanır. Arkadaki ekran stack'i değişmez.

- **AC-NAV-08** — **GIVEN** kullanıcı Koleksiyon sekmesinde kökten 3 ekran derinliğinde iken, **WHEN** Koleksiyon sekme ikonuna tekrar dokunur, **THEN** stack tamamen temizlenerek Koleksiyon kök ekranı 300ms animasyonla gösterilir; depth = 0 olur.

- **AC-NAV-09** — **GIVEN** kullanıcı Takım sekmesinin kök ekranında (depth = 0) iken, **WHEN** Takım sekme ikonuna tekrar dokunur, **THEN** hiçbir navigasyon değişikliği olmaz.

- **AC-NAV-10** — **GIVEN** screen stack derinliği 8'e ulaşmışken, **WHEN** yeni `NavigateTo()` çağrısı gelir, **THEN** navigasyon yoksayılır, `Debug.LogError` kanalına `"UIManager: NavigateTo rejected — stack max depth (8) reached"` içeren mesaj yazılır ve mevcut ekran değişmez.

- **AC-NAV-11 (BLOCKING)** — **GIVEN** `UIManager.LockTabs()` çağrılmışken (`UIManager.IsTabsLocked() == true`), **WHEN** herhangi bir sekme ikonuna dokunulur, **THEN** navigasyon gerçekleşmez; aktif sekmenin `UIManager.GetStackDepth(sekme)` değeri değişmez; `UIManager.IsTabsLocked()` hâlâ `true` döner; uygulama çökmez.

- **AC-NAV-12 (BLOCKING)** — **GIVEN** `UIManager.LockTabs()` çağrılmış ve tabs kilitliyken, **WHEN** `UIManager.UnlockTabs()` çağrılır, **THEN** `UIManager.IsTabsLocked() == false` döner; sonraki sekme dokunuşu 300ms animasyonla normal navigasyonu tetikler.

- **AC-NAV-13 (ADVISORY — Visual)** — **GIVEN** `UIManager.LockTabs()` çağrılmışken, **WHEN** sekme barı gözlemlenir, **THEN** sekme ikonları görsel olarak disabled/gri state gösterir (aktif durumdan ayırt edilebilir). Screenshot kanıtı `production/qa/evidence/` altına kaydedilir.

### B. Modal Sistemi
*Logic (queue state machine) — BLOCKING*

- **AC-MOD-01** — **GIVEN** Modal A açık durumdayken, **WHEN** `ShowModal(modalB)` çağrısı gelir, **THEN** Modal B ekranda görünmez ve kuyrukta bekler. Ekranda yalnızca Modal A aktif kalır.

- **AC-MOD-02** — **GIVEN** Modal A açıkken sırasıyla Modal B ve C kuyruğa alınmışken, **WHEN** Modal A kapatılır, **THEN** Modal B 250ms açılış animasyonuyla gösterilir; B kapandıktan sonra C gösterilir. Sıra değişmez.

- **AC-MOD-03** — **GIVEN** herhangi bir modal açıldığında, **WHEN** dim overlay render edilir, **THEN** overlay VisualElement'inin `resolvedStyle.opacity` değeri `0.60f ±0.005f` aralığında programatik olarak doğrulanır.

- **AC-MOD-04** — **GIVEN** bir modal açıkken kullanıcı "Kapat" butonuna dokunur, **WHEN** kapanış tamamlanır, **THEN** `onClose` callback tetiklenir ve modal 200ms kapanış animasyonuyla ekrandan kaybolur.

- **AC-MOD-05** — **GIVEN** 1 aktif modal ve kuyrukta 4 bekleyen (toplam 5, maksimum kapasite) varken, **WHEN** 6. modal `ShowModal()` ile talep edilir, **THEN** talep sessizce reddedilir; mevcut 5 modalın sırası değişmez; hata veya çökme olmaz.

- **AC-MOD-06** — **GIVEN** herhangi bir modal açıkken, **WHEN** geri tuşuna basılır, **THEN** modal `onClose(false)` callback'ini tetikler (false = iptal) ve 200ms animasyonla kapanır. Arka plandaki stack değişmez.

- **AC-MOD-07 (BLOCKING)** — **GIVEN** bir modal açık durumdayken, **WHEN** `UIManager.ForceCloseActiveModal()` çağrılır, **THEN** şu sırayla gerçekleşir: (1) aktif modal VisualElement ağacından animasyonsuz kaldırılır; (2) `onClose(false)` callback senkron tetiklenir; (3) kuyrukta bekleyen modal varsa bir sonraki modal push animasyonuyla açılmaya başlar. `ForceCloseActiveModal()` dönüşünden *hemen* sonra (aynı call stack) `UIManager.IsModalOpen()` çağrılırsa: kuyruk boşsa `false`, kuyrukta bekleyen modal varsa `true` döner (bir sonraki modal zaten açılmaya başlamıştır). Test kodu bunu `ForceCloseActiveModal()` → 1 frame bekleme → `IsModalOpen()` sorgulama sırasıyla doğrular.

### C. HUD Katmanı
*Integration (Ekonomi → HUD veri akışı) — BLOCKING*

- **AC-HUD-01** — **GIVEN** HUD gizli durumdayken, **WHEN** `UIManager.ShowHUD()` çağrılır, **THEN** üst HUD çubuğu altın, elmas ve enerji değerleriyle görünür olur.

- **AC-HUD-02** — **GIVEN** kullanıcı HUD görünürken savaşı başlatır, **WHEN** Savaş ekranı `UIManager.HideHUD()` çağırır, **THEN** üst HUD çubuğu tamamen gizlenir; savaş devam ettiği sürece görünmez kalır.

- **AC-HUD-03** — **GIVEN** savaş ekranında HUD gizliyken savaş sona erer ve önceki ekrana dönülür, **WHEN** önceki ekran `ShowHUD()` çağırır, **THEN** HUD tekrar görünür olur ve güncel kaynak değerlerini gösterir.

- **AC-HUD-04a** — **GIVEN** HUD görünürken (`HUD_Visible`) `ShowHUD()`/`HideHUD()` çağırmayan bir ekran açılır, **WHEN** ekran render tamamlanır, **THEN** HUD görünür kalmaya devam eder.

- **AC-HUD-04b** — **GIVEN** HUD gizliyken (`HUD_Hidden`) `ShowHUD()`/`HideHUD()` çağırmayan bir ekran açılır, **WHEN** ekran render tamamlanır, **THEN** HUD gizli kalmaya devam eder.

### D. Toast / Bildirim Sistemi
*Logic (queue + timer state machine) — BLOCKING*

- **AC-TOAST-01** — **GIVEN** herhangi bir toast `ShowToast()` ile tetiklendiğinde, **WHEN** toast ekranda belirir, **THEN** 3.0 saniye **±16ms** sonra fade animasyonu başlar. İçerik bu süreyi değiştirmez.

- **AC-TOAST-02** — **GIVEN** toast 3.0 saniye gösterilip fade başladığında, **WHEN** fade ölçülür, **THEN** 0.4 saniye ±16ms sürer. Bir sonraki toast fade **başladığı** anda (t=3.0s, ±16ms tolerans) gösterilmeye başlar — fade bitmesini beklemez. t=3.0s–3.4s arası: yeni toast A ile aynı konumda render edilir, A arkada opacity 1→0 solar.

- **AC-TOAST-03** — **GIVEN** Toast A ekranda görünürken B ve C sırasıyla kuyruğa alınmışken, **WHEN** Toast A'nın t=3.0s anı gelir ve fade başlar, **THEN** Toast B aynı anda (aynı frame veya bir sonraki frame — cihazın o anki frame süresinden bağımsız olarak `OnToastFadeStart` callback'inde) ekranda belirir; A arkada solar, B tam görünür kalır. Test kodu: `OnToastFadeStart` event handler'ında B'nin `resolvedStyle.opacity == 1.0f` doğrulanır — hardware frame süresi ölçülmez. Toast B tamamlandıktan sonra Toast C gösterilir.

- **AC-TOAST-04** — **GIVEN** info, success ve warning türlerinde birer toast gösterildiğinde, **WHEN** her toast ekranda iken gözlemlenir, **THEN** info mavi+bilgi ikonu, success yeşil+onay ikonu, warning turuncu+uyarı ikonu ile gösterilir; 3 tür birbirinden görsel olarak ayırt edilebilir.

- **AC-TOAST-05** — **GIVEN** bir toast gösterildiğinde, **WHEN** toast konumu ölçülür, **THEN** toast'un üst kenarı HUD çubuğunun hemen altında başlar; HUD ile üst üste binmez.

- **AC-TOAST-06** — **GIVEN** savaş ekranı aktifken `ShowToast()` çağrılır, **WHEN** savaş ekranı aktif olduğu süre boyunca ekran gözlemlenir, **THEN** hiçbir toast render edilmez ve toast kuyrukta bekler.

- **AC-TOAST-07** — **GIVEN** savaş sırasında kuyruğa alınmış en az 1 toast varken, **WHEN** savaş ekranından çıkılır, **THEN** kuyruktaki toast FIFO sırasıyla gösterilir.

### E. Geçiş Animasyonları
*Visual/Feel — ADVISORY (reduced motion mantığı BLOCKING)*

- **AC-ANIM-01** — **GIVEN** reduced motion kapalıyken, **WHEN** sekme değişimi tetiklenir, **THEN** yatay kaydırma animasyonu 300ms ±16ms sürer.

- **AC-ANIM-02** — **GIVEN** reduced motion kapalıyken, **WHEN** `NavigateTo()` ile ekran push edilir, **THEN** soldan giriş animasyonu 250ms ±16ms sürer.

- **AC-ANIM-03** — **GIVEN** reduced motion kapalıyken, **WHEN** geri navigasyon tetiklenir, **THEN** sola çıkış animasyonu 200ms ±16ms sürer.

- **AC-ANIM-04** — **GIVEN** reduced motion kapalıyken, **WHEN** `ShowModal()` çağrılır, **THEN** modal alttan yukarı spring easing ile 250ms ±16ms içinde tam görünür olur.

- **AC-ANIM-05** — **GIVEN** reduced motion kapalıyken, **WHEN** modal kapatılır, **THEN** aşağı kayarak 200ms ±16ms içinde ekrandan çıkar.

- **AC-ANIM-06 (BLOCKING)** — **GIVEN** `UIManager.ReducedMotionEnabled = true` olarak test setup'ında programatik olarak set edilmişken, **WHEN** herhangi bir geçiş (sekme, push, pop, modal açılış/kapanış) tetiklenir, **THEN** geçiş snap ile gerçekleşir: `NavigateTo()` / `ShowModal()` / tab tap çağrısının ardından gelen **ilk `LateUpdate()`**'te hedef ekran final transform'unda (`translate: 0px, 0px`) bulunur; ara konumda frame render edilmez. Test kodu: `UIManager.NavigateTo(screen)` → `yield return null` (1 frame) → `GetTopScreen().resolvedStyle.translate == StyleTranslate.None` doğrulanır. *Not: OS seviyesinde reduced motion tespiti (iOS `UIAccessibility.isReduceMotionEnabled`, Android `Settings.Global.TRANSITION_ANIMATION_SCALE`) native bridge üzerinden C#'a erişilebilir; implementasyon adr-ui-animation-driver.md'de belgelenecek. PlayerPrefs toggle bunun manuel karşılığıdır.*

- **AC-ANIM-07** — **GIVEN** reduced motion aktifken bir toast tetiklendiğinde, **WHEN** toast gösterilir, **THEN** toast 3.0 saniye tam görünür kalır; fade animasyonu olmadan anlık kaybolur.

- **AC-ANIM-08** — **GIVEN** stack push animasyonu devam ederken, **WHEN** kullanıcı farklı sekmeye dokunur, **THEN** devam eden animasyon anlık tamamlanmış sayılır (snap) ve yeni geçiş animasyonu hemen başlar; UI donmaz.

### F. Touch Hedefleri
*UI — ADVISORY*

- **AC-TOUCH-01** — **GIVEN** UI'daki herhangi bir tıklanabilir eleman (sekme ikonu, buton, modal kapatma, liste öğesi dahil), **WHEN** dokunma hedefi alanı ölçülür, **THEN** minimum 44dp × 44dp boyutundadır.

- **AC-TOUCH-02** — **GIVEN** yan yana veya dikey bitişik iki farklı tıklanabilir eleman olduğunda, **WHEN** iki hedef arası boşluk ölçülür, **THEN** en az 8dp boşluk bulunur.

### G. Kaynak Formatı
*Logic — BLOCKING*

- **AC-FMT-01** — **GIVEN** altın miktarı 734 iken, **WHEN** HUD render edilir, **THEN** "734" gösterilir (K/M suffix yok).
- **AC-FMT-02** — **GIVEN** altın miktarı tam 999 iken, **WHEN** HUD render edilir, **THEN** "999" gösterilir.
- **AC-FMT-03** — **GIVEN** altın miktarı tam 1.000 iken, **WHEN** HUD render edilir, **THEN** "1.0K" gösterilir (`T_K = 1.000`, K eşiği).
- **AC-FMT-03b** — **GIVEN** altın miktarı 5.500 iken, **WHEN** HUD render edilir, **THEN** "5.5K" gösterilir.
- **AC-FMT-03c** — **GIVEN** altın miktarı 9.999 iken, **WHEN** HUD render edilir, **THEN** "9.9K" gösterilir.
- **AC-FMT-04** — **GIVEN** altın miktarı 12.500 iken, **WHEN** HUD render edilir, **THEN** "12.5K" gösterilir (`floor(12500/100f)/10f = 125/10 = 12.5 → "12.5K"`).
- **AC-FMT-05** — **GIVEN** altın miktarı 50.000 iken, **WHEN** HUD render edilir, **THEN** "50K" gösterilir (`floor(50000/100f)/10f = 500/10 = 50.0 → "50K"` — format "0.#" sıfır ondalığı göstermez).
- **AC-FMT-06** — **GIVEN** altın miktarı tam 1.000.000 iken, **WHEN** HUD render edilir, **THEN** "1.0M" gösterilir.
- **AC-FMT-07** — **GIVEN** altın miktarı 1.500.000 iken, **WHEN** HUD render edilir, **THEN** "1.5M" gösterilir.
- **AC-FMT-08** — **GIVEN** altın kaynağı 100ms içinde 3 kez güncellenir (her 30ms'de bir), **WHEN** 100ms throttle penceresi tamamlanır, **THEN** HUD yalnızca 1 kez yenilenir ve güncel değeri gösterir (callback `Economy.GetCurrentGold()` okur).
- **AC-FMT-09** — **GIVEN** altın kaynağı yalnızca 1 kez değişir ve 100ms içinde başka güncelleme gelmez, **WHEN** değişimden 100ms geçer, **THEN** HUD yeni değeri gösterir.
- **AC-FMT-10** — **GIVEN** raw_value = -100 ile HUD güncelleme eventi geldiğinde, **WHEN** HUD render edilir, **THEN** "0" gösterilir; negatif değer hiçbir zaman ekranda görünmez.

### G2. Elmas ve Enerji Formatı
*Logic — BLOCKING*

- **AC-FMT-11** — **GIVEN** elmas miktarı 247 iken, **WHEN** HUD render edilir, **THEN** "247" gösterilir (K/M suffix yok).
- **AC-FMT-12** — **GIVEN** elmas miktarı 0 iken, **WHEN** HUD render edilir, **THEN** "0" gösterilir.
- **AC-FMT-13** — **GIVEN** elmas miktarı 9999 (maksimum) iken, **WHEN** HUD render edilir, **THEN** "9999" gösterilir (K suffix çıkmaz).
- **AC-FMT-14** — **GIVEN** E=75 ve MAX_ENERGY=100 iken, **WHEN** HUD render edilir, **THEN** "75/100" gösterilir.
- **AC-FMT-15** — **GIVEN** E=0 iken, **WHEN** HUD render edilir, **THEN** "0/100" gösterilir.
- **AC-FMT-16** — **GIVEN** E=100 (tam dolu) iken, **WHEN** HUD render edilir, **THEN** "100/100" gösterilir.
- **AC-FMT-17** — **GIVEN** raw_elmas = -50 ile HUD güncelleme eventi geldiğinde, **WHEN** HUD render edilir, **THEN** "0" gösterilir; negatif elmas değeri ekranda görünmez.

### H. Performans
*Integration (performans eşiği) — BLOCKING*

- **AC-PERF-01** — **GIVEN** referans test cihazında (**Samsung Galaxy A32 5G — SM-A326B — Snapdragon 720G, 4GB RAM**) oyun çalışırken, **WHEN** kullanıcı 3 farklı sekme arasında **30 ardışık dokunuşla** geçiş yapar (animasyon snap-interrupt tetiklenir), **THEN** `FrameTimingManager` ile ölçülen frame süresi ortalama ≤16.6ms, 95. persentil ≤20ms kalır. **Test prosedürü:** (1) IL2CPP build (Mono kabul edilmez — IL2CPP %15–40 farklı frame time üretir); (2) cihaz 5 dakika ısındırma döngüsü sonrası ölçüm (thermal throttle aktif olabilir — soğuk başlangıç kabul edilmez); (3) background process'ler minimize edilir (uçak modu önerilir). Test 3 kez bağımsız olarak tekrar edilir; **her çalıştırma ayrı ayrı değerlendirilir** — herhangi birinde ortalama >16.6ms veya P95 >20ms ise AC başarısız. Release build üzerinde ölçülür (Deep Profile kapalı).

- **AC-PERF-02** — **GIVEN** aynı anda HUD görünür + 1 modal açık + kuyrukta 3 toast + stack derinliği 3 iken, **WHEN** bu durum 10 saniye sürdürülür, **THEN** ortalama frame rate 60 FPS'i korur; anlık düşüşler 3 ardışık frame'i (50ms) aşmaz.

- **AC-PERF-03** *(Visual/Feel — ADVISORY, manuel sign-off gerektirir)* — **GIVEN** UI Framework tam aktif durumdayken (HUD görünür, 1 modal açık, 1 toast ekranda), **WHEN** Unity Frame Debugger (Editor Play Mode'da) ile draw call sayısı incelenir, **THEN** UI katmanlarının (`UIDocument_Game` + `UIDocument_HUD` + `UIDocument_Modal` + `UIDocument_Toast`) toplam katkısı **≤40 draw call** kalır. *Kanıt: Mühendis Frame Debugger annotated screenshot'ını `production/qa/evidence/perf-draw-calls-[tarih].png` altına kaydeder; lead sign-off verir. Not: Projenin toplam mobil bütçesi <100 DC; UI alt-bütçesi 40, kalan 60 gameplay renderına rezervedir. 4-UIDocument mimarisi cross-panel batching engelliyor — ADR onaylanana kadar bu bütçe tahmini doğrulanmamış.*

## Open Questions

1. **Idle Ödül Reveal Flow — PARTIALLY RESOLVED:** API stub eklendi (`UIManager.ShowFullscreenReveal(RewardData data)`). Otofarm sistemi artık `ShowToast()` kullanmayacak. Tam reveal sistem tasarımı (cold-start intercept, animasyon akışı, RewardData şeması) ayrı `/design-system idle-reward-reveal` oturumunda yapılacak.

2. **ADR — UI Animation Driver (BLOCKING):** PrimeTween seçildi. `VisualElement.transform` deprecated — yerine `style.translate` / `style.rotate` / `style.scale`. ADR'de belgelenmesi gerekenler: PrimeTween kurulum ve bağımlılık bildirimi; `Tween.Custom()` ile `StyleTranslate` animasyonu; snap-to-end mekanizması (`Tween.Complete()`); cubic-bezier Y>1.0 evaluator stratejisi; `style.translate` → `StyleTranslate(new Translate(x, y))` tip dönüşümü; OS reduced motion native bridge implementasyonu (iOS `UIAccessibility`, Android `Settings.Global`). `docs/architecture/adr-ui-animation-driver.md`

3. **ADR — UIDocument Topolojisi (BLOCKING):** 4 UIDocument mimarisi cross-panel batching'i engeller. ADR'de: 4-panel DC maliyeti vs 2-panel karşılaştırması, peak state ölçümü, ≤40 DC sub-budget kanıtı. `docs/architecture/adr-ui-document-topology.md`

4. **Cubic-bezier spring doğrulama:** `cubic-bezier(0.34, 1.56, 0.64, 1.0)` — PrimeTween animasyonunda Y>1.0 easing değeri C# evaluator'da doğrulanmalı. Unity 6.3 USS CSS'te clamp riski ADR-2'de ele alınacak; runtime PrimeTween custom curve ile fallback (`ease-out spring approx`) tanımlanmalı.

5. **Safe area implementasyon detayları — RESOLVED:** Karar alındı (bkz. UIDocument Topolojisi bölümü — "Safe area implementasyon kararı"). Koordinat dönüşüm detayları ve PanelSettings scale mode ilişkisi `adr-ui-document-topology.md`'de belgelenecek.

6. **`ForceCloseActiveModal()` erişim koruması:** Bu metot yalnızca debug ve kritik session reset senaryolarında kullanılmalıdır. Implementasyonda `[Conditional("UNITY_EDITOR || DEVELOPMENT_BUILD")]` attribute veya `internal` erişim belirleyici kullanılması önerilir — release build'de normal oyun kodu bu metoda derleyici seviyesinde erişemez.
