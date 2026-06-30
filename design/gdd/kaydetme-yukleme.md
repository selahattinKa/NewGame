# Kaydetme / Yükleme (Save/Load)

> **Status**: Designed
> **Author**: Kullanıcı + Claude Code Game Studios
> **Last Updated**: 2026-06-24
> **Implements Pillar**: Senin Tempon (Play Your Way), Güç Hisset (Power Fantasy)

## Overview

Kaydetme / Yükleme sistemi, oyunun tüm kalıcı verilerini (canavar koleksiyonu, kaynak bakiyeleri, zindan ilerlemesi, takım konfigürasyonları, ayarlar) cihaz üzerinde serileştiren ve geri yükleyen Foundation katmanı persistence framework'üdür. Mobil platformda beklenmedik kapanma, arka plana atılma ve düşük pil senaryolarına karşı dayanıklı bir otomatik kaydetme modeli sunar — oyuncu manuel kaydetme/yükleme ile uğraşmaz. Sistem aynı zamanda çevrimdışı süre takibi yaparak Otofarm ve idle birikimlerini hesaplamak için gereken zaman damgasını sağlar. MVP'de tüm veri yerel cihazda saklanır (sunucu tarafı senkronizasyonu kapsam dışıdır); tek bir save slot kullanılır; veri formatı ileriye dönük genişletilebilir tasarlanır (versiyon damgalı şema ile geriye uyumlu migrasyon desteği).

## Player Fantasy

Kaydetme / Yükleme doğrudan bir oyuncu fantezisi sunmaz — oyuncu bu sistemle bilinçli olarak etkileşmez. Ancak dolaylı olarak tüm sütunları destekler: oyuncu saatlerce biriktirdiği canavarların, kazandığı kaynakların ve ulaştığı zindan katının her zaman güvende olduğunu bilir. "Telefonumu kapattım, geri döndüğümde her şey yerindeydi" hissi bu sistemin başarı göstergesidir. Sistem görünmez olduğunda başarılıdır — oyuncu kaydetmeyi düşünmez, çünkü kaydetme her zaman gerçekleşmiştir. Başarısızlık ise yıkıcıdır: tek bir kayıp save, oyuncunun güvenini kalıcı olarak kırar ve Pillar 4 "Güç Hisset" fantezisini tamamen yok eder.

## Detailed Rules

### Core Rules

1. **Tek Save Slot Modeli**: Oyunun tek bir aktif kayıt dosyası vardır. Birden fazla profil veya save slot MVP kapsamında değildir.

2. **Save Veri Yapısı**: Tüm kalıcı oyun durumu tek bir `SaveData` nesnesi altında toplanır:
   - `schema_version` (int) — veri format versiyonu, migrasyon için kullanılır
   - `save_timestamp` (ISO 8601 string) — son kaydetme anının UTC zaman damgası
   - `play_time_seconds` (int) — toplam aktif oynama süresi
   - `player_profile` — oyuncu adı, seviye, deneyim puanı
   - `resources` — altın, elmas, enerji miktarları + enerji son yenilenme zamanı
   - `monster_collection` — sahip olunan canavarlar listesi (her biri: `monster_id`, `level`, `xp`, `star_rank`, `evolution_stage`, `element`, `rarity`, `stats`, `acquired_date`)
   - `pending_monsters` — bekleyen canavar buffer'ı (max 10, `pending_expiry_days`=7 gün)
   - `team_presets` — kayıtlı takım konfigürasyonları (max slot sayısı açılmış kadar)
   - `active_team_index` — şu an aktif takım preset indeksi
   - `dungeon_progress` — her bölge için en yüksek ulaşılan kat, first-clear durumları
   - `inventory_capacity` — mevcut envanter genişletme sayısı
   - `settings` — ses, bildirim, dil gibi oyuncu tercihleri
   - `idle_state` — `{active: bool, start_time: timestamp, floor: int, region: string, team_power: int, idle_pity_bonus: float, pending_report: ReturnReport|null}` — otofarm durumu, takım gücü snapshot'u, kalıcı pity bonusu ve bekleyen rapor
   - `statistics` — toplam öldürülen düşman, toplam kazanılan altın vb. istatistikler

3. **Olay Tabanlı Otomatik Kaydetme**: Sistem aşağıdaki olaylarda otomatik kaydeder:
   - Zindan katı tamamlandığında (başarılı veya başarısız)
   - Canavar kazanıldığında, satıldığında veya serbest bırakıldığında
   - Canavar güçlendirme yapıldığında (seviye atlama, evrim, yıldız)
   - Kaynak harcandığında veya kazanıldığında (altın, elmas, enerji)
   - Takım preset kaydedildiğinde veya değiştirildiğinde
   - Otofarm başlatıldığında veya durdurulduğunda
   - Uygulama arka plana atıldığında (`OnApplicationPause(true)`)
   - Uygulama odak kaybettiğinde (`OnApplicationFocus(false)`)
   - Uygulama kapatıldığında (`OnApplicationQuit`)
   - Ayarlar değiştirildiğinde

4. **Kaydetme Süreci (Write-Rename + Yedek)**:
   - Adım 1: `SaveData` nesnesini JSON'a serileştir
   - Adım 2: JSON string'e checksum (SHA-256 hash) ekle → `{checksum}:{json_data}` formatı
   - Adım 3: Geçici dosyaya yaz (`save_temp.json`)
   - Adım 4: Geçici dosyanın yazılmasını doğrula (boyut > 0, checksum tutarlı)
   - Adım 5: Mevcut `save.json`'ı `save_backup.json` olarak yeniden adlandır
   - Adım 6: `save_temp.json`'ı `save.json` olarak yeniden adlandır
   - Adım 7: Başarıyı logla
   - Herhangi bir adımda hata olursa: geçici dosyayı sil, mevcut save'e dokunma, hatayı logla

5. **Yükleme Süreci**:
   - Adım 1: `save.json` var mı kontrol et
   - Adım 2: Varsa oku, checksum doğrula
   - Adım 3: Checksum geçersizse → `save_backup.json`'dan yükle (varsa ve checksum geçerliyse)
   - Adım 4: Her iki dosya da bozuksa → yeni oyun başlat, bozuk dosyaları `save_corrupted_[timestamp].json` olarak yedekle
   - Adım 5: `schema_version` kontrolü yap — mevcut versiyondan eskiyse migrasyon pipeline'ını çalıştır
   - Adım 6: JSON'dan `SaveData` nesnesine deserialize et
   - Adım 7: Çevrimdışı süreyi hesapla: `offline_duration = now_utc - save_timestamp`

6. **Şema Versiyonlama ve Migrasyon**:
   - Her save yapısı değişikliği `schema_version`'ı 1 artırır
   - Migrasyon fonksiyonları zincirli çalışır: v1→v2→v3 (doğrudan v1→v3 atlamaz)
   - Her migrasyon fonksiyonu: eski JSON'ı okur, yeni alanları varsayılan değerlerle ekler, kaldırılan alanları temizler
   - Migrasyon sonrası hemen kaydetme tetiklenir (yeni formatta)

7. **Dosya Konumu**: `Application.persistentDataPath` kullanılır — platform bağımsız, uygulama güncellemelerinde korunur.

8. **Kaydetme Debounce**: Ardışık olaylar (ör. zindan katı tamamlanınca hem loot hem canavar hem kaynak değişir) tek bir kaydetme işlemine birleştirilir. Save request'i alındığında 0.5 saniyelik debounce penceresi açılır; pencere süresince gelen ek save request'leri birleştirilir.

### States and Transitions

| State | Açıklama | Geçiş Tetikleyicisi |
|-------|----------|---------------------|
| **Uninitialized** | Sistem henüz başlatılmadı | Uygulama başlangıcı → Loading |
| **Loading** | Save dosyası okunuyor ve doğrulanıyor | Başarılı → Ready, Dosya yok → NewGame, Bozuk → Recovery |
| **NewGame** | İlk oyun başlangıcı, varsayılan SaveData oluşturuluyor | Oluşturma tamamlandı → Ready |
| **Recovery** | Birincil save bozuk, yedekten kurtarma deneniyor | Yedek başarılı → Ready, Yedek de bozuk → NewGame (bozuk dosyalar yedeklenir) |
| **Ready** | Normal çalışma durumu, save/load hazır | Save tetikleyici → Saving |
| **Saving** | Veri serileştiriliyor ve diske yazılıyor | Yazma başarılı → Ready, Yazma hatası → SaveError |
| **SaveError** | Kaydetme başarısız oldu | Hata loglandı, mevcut save korundu → Ready (UI'da uyarı gösterilebilir) |

### Interactions with Other Systems

| Sistem | Yön | Arayüz | Detay |
|--------|------|--------|-------|
| **Ekonomi / Kaynak Yönetimi** | ← Kaynak verisini sağlar | `resources` bloğu | Altın, elmas, enerji miktarları + enerji regen timestamp |
| **Canavar Toplama ve Evrim** | ← Canavar verisini sağlar | `monster_collection`, `pending_monsters` | Tüm canavar detayları, pending buffer |
| **Canavar Güçlendirme** | ← Güçlendirme verisini sağlar | `monster_collection` içinde | Seviye, XP, yıldız, evrim aşaması |
| **Takım Kurma** | ← Takım verisini sağlar | `team_presets`, `active_team_index` | Preset listesi ve aktif takım |
| **Zindan Keşif** | ← İlerleme verisini sağlar | `dungeon_progress` | Kat ilerlemesi, first-clear durumları |
| **Otofarm / Idle** | ← Idle state sağlar, → çevrimdışı süre sağlar | `idle_state`, `save_timestamp` | Otofarm başlangıç zamanı; offline_duration hesaplaması |
| **Savaş Sistemi** | → Savaş sonu save tetikler | Event: `OnBattleComplete` | Savaş bitince kaydet |
| **UI Framework** | → Save durumu bildirir | Event: `OnSaveStateChanged` | Loading göstergesi, hata bildirimi |

## Formulas

### offline_duration (Çevrimdışı Süre Hesaplama)

`offline_duration = clamp(now_utc - save_timestamp, 0, max_offline_minutes)`

**Değişkenler:**
| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|------|--------|----------|
| now_utc | T_now | DateTime | - | Yükleme anındaki UTC zaman |
| save_timestamp | T_save | DateTime | - | Son kaydetme anındaki UTC zaman |
| max_offline_minutes | M_max | int | 1440 | Maksimum çevrimdışı süre tavanı (24 saat) |
| offline_duration | D_off | TimeSpan | 0–1440 dk | Çevrimdışı geçen süre (dakika) |

**Çıktı Aralığı:** 0 dakika (hemen geri dönüş) ile 1440 dakika (24 saat — Ekonomi GDD'deki `idle_gold_formula` tavanı) arası. 1440 dakikayı aşan değerler 1440'a clamp edilir.

**Örnek:** Oyuncu 22:00 UTC'de çıktı, 08:00 UTC'de geri döndü → `offline_duration = 600 dakika`. Ekonomi sistemi bu değeri `idle_gold_formula`'ya besler.

### save_file_size (Dosya Boyutu Tahmini)

`estimated_size_kb = base_overhead + (monster_count × per_monster_size) + (preset_count × per_preset_size) + (region_count × per_region_size)`

**Değişkenler:**
| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|------|--------|----------|
| base_overhead | S_base | float | ~2 KB | Metadata, resources, settings, statistics |
| monster_count | N_mon | int | 0–200 | Sahip olunan canavar sayısı (max_inventory_capacity) |
| per_monster_size | S_mon | float | ~0.3 KB | Canavar başına JSON boyutu |
| preset_count | N_pre | int | 3–10 | Açılmış preset slot sayısı |
| per_preset_size | S_pre | float | ~0.1 KB | Preset başına JSON boyutu |
| region_count | N_reg | int | 1–10+ | Keşfedilen bölge sayısı |
| per_region_size | S_reg | float | ~0.2 KB | Bölge ilerleme verisi boyutu |

**Çıktı Aralığı:** MVP'de ~5–10 KB (20 canavar, 1 bölge), tam vizyonda ~70 KB (200 canavar, 10 bölge). JSON formatında bu boyutlar mobilde sorunsuz.

**Örnek:** MVP save: `2 + (20 × 0.3) + (3 × 0.1) + (1 × 0.2) = 2 + 6 + 0.3 + 0.2 = 8.5 KB`

### debounce_window

Sabit değer: **0.5 saniye**. Debounce penceresi içinde gelen tüm save request'leri tek bir yazma işlemine birleştirilir. Tuning Knobs bölümünde ayarlanabilir olarak tanımlanır.

## Edge Cases

- **If `save_timestamp` gelecekte ise** (oyuncu cihaz saatini ileri almış): `offline_duration = 0` olarak kabul et, `save_timestamp`'ı şimdiki zamana güncelle. Çevrimdışı ödül hesaplanmaz — saat manipülasyonunu cezalandırmak yerine ödülsüz bırak.

- **If cihaz saati geriye alınmışsa** (save_timestamp > now_utc): `offline_duration = 0` olarak clamp et. Oyuncu hiçbir şey kaybetmez ama idle ödülü de almaz.

- **If save dosyası ve yedek dosya aynı anda bozuksa**: Yeni oyun başlat. Bozuk dosyaları `save_corrupted_[timestamp].json` olarak sakla. Oyuncuya "Kayıt dosyası kurtarılamadı, yeni oyun başlatıldı" bildirimi göster.

- **If save dosyası mevcut schema_version'dan yüksekse** (oyun eski sürüme dönmüş): Yüklemeyi reddet, "Oyunu güncelleyin" mesajı göster. Dosyaya dokunma — ileriye dönük veri kaybını önle.

- **If kaydetme sırasında disk alanı yetersizse**: Geçici dosya yazma başarısız olur. Mevcut save'e dokunulmaz (`save.json` korunur). Oyuncuya "Depolama alanı yetersiz" uyarısı göster.

- **If uygulama OnApplicationQuit sırasında kaydetme tamamlanamadan kapanırsa**: Write-rename + yedek mekanizması bunu korur — ya eski `save.json` ya da `save_backup.json` sağlam kalır. Bir sonraki açılışta sağlam olan yüklenir.

- **If debounce penceresi sırasında uygulama arka plana atılırsa**: Debounce'u hemen iptal et ve anında kaydet. `OnApplicationPause` save'i debounce'a tabi değildir — her zaman hemen çalışır.

- **If oyuncu aynı anda iki cihazda oynuyorsa** (MVP'de sunucu yok): Tanımsız davranış. Her cihaz kendi yerel save'ini yönetir. Cloud sync kapsam dışı — ileride eklenirse çatışma çözümü gerekir.

- **If pending_monsters buffer'ındaki canavarların süresi dolmuşsa** (7 gün): Yükleme anında `pending_expiry_days` kontrolü yapılır. Süresi dolmuş canavarlar otomatik silinir, oyuncuya "X canavar süresi dolduğu için silindi" bildirimi gösterilir.

- **If migrasyon sırasında hata olursa**: Hata loglanır, migrasyon adımı atlanmaz — eksik alanlar varsayılan değerlerle doldurulur. Migrasyon sonrası checksum doğrulaması yapılır.

- **If save dosyası 0 byte ise**: Bozuk dosya olarak kabul et → Recovery state'ine geç.

## Dependencies

**Upstream Bağımlılıklar:** Yok. Kaydetme / Yükleme Foundation katmanında bağımsız bir sistemdir.

**Downstream Bağımlılıklar:**

| Sistem | Bağımlılık Tipi | Arayüz | Açıklama |
|--------|----------------|--------|----------|
| **Otofarm / Idle Sistemi** | Hard | `save_timestamp`, `idle_state`, `offline_duration` | Çevrimdışı birikimi hesaplamak için `offline_duration`'a mutlak bağımlı. Bu sistem olmadan idle ödülleri hesaplanamaz. |
| **Ekonomi / Kaynak Yönetimi** | Soft | `resources` bloğu | Kaynak bakiyeleri persist edilir. Save olmadan veriler oturum sonunda kaybolur ama sistem çalışabilir. |
| **Canavar Toplama ve Evrim** | Soft | `monster_collection`, `pending_monsters` | Canavar koleksiyonu persist edilir. `pending_expiry_days` (7 gün) kontrolü yükleme anında yapılır. |
| **Canavar Güçlendirme** | Soft | `monster_collection` içinde | Seviye, XP, yıldız, evrim verileri persist edilir. |
| **Takım Kurma** | Soft | `team_presets`, `active_team_index` | Takım preset'leri persist edilir. |
| **Zindan Keşif** | Soft | `dungeon_progress` | Kat ilerlemesi ve first-clear durumları persist edilir. |
| **Savaş Sistemi** | Soft | Event: `OnBattleComplete` → save tetikler | Savaş sonuçları save'i tetikler ama savaş sistemi save'e bağımlı değildir. |
| **UI Framework** | Soft | Event: `OnSaveStateChanged` | Save durumu değişikliklerini dinler (loading spinner, hata mesajı). |

**Çift Yönlü Tutarlılık Notu:** Bu GDD tamamlandığında, yukarıdaki downstream GDD'lerin Dependencies bölümlerinde "Kaydetme / Yükleme" referansı güncellenmelidir.

## Tuning Knobs

| Tuning Knob | Varsayılan | Güvenli Aralık | Çok Düşükse | Çok Yüksekse | Etkilenen Oynanış |
|-------------|-----------|----------------|-------------|-------------|-------------------|
| `debounce_window_seconds` | 0.5 | 0.1–2.0 | Çok sık disk yazma, performans etkisi | Oyuncu ani kapamada veri kaybedebilir | Save sıklığı vs. performans dengesi |
| `max_offline_minutes` | 1440 (24h) | 60–4320 | Idle ödülleri çok düşük, oyuncu hayal kırıklığı | Çok uzun süre birikimle oyun ekonomisi bozulur | Offline birikimi tavanı (Ekonomi GDD `idle_gold_formula` ile paylaşılır) |
| `checksum_enabled` | true | true/false | (false) Save dosyası bozulma tespiti yapılamaz | - | Veri güvenliği vs. kaydetme hızı |
| `backup_enabled` | true | true/false | (false) Bozuk save'den kurtarma şansı yok | - | Veri güvenliği vs. disk kullanımı |
| `max_corrupted_backups` | 3 | 1–10 | Eski bozuk dosyalar hemen silinir | Gereksiz disk kullanımı | Debug ve destek kolaylığı |
| `schema_version` | 1 | 1–∞ | - | - | Migrasyon pipeline'ı tetikleme eşiği |
| `save_file_name` | "save.json" | - | - | - | Dosya yolu konfigürasyonu |
| `backup_file_name` | "save_backup.json" | - | - | - | Yedek dosya yolu konfigürasyonu |

**Etkileşimler:**
- `max_offline_minutes` değiştirilirse Ekonomi GDD'deki `idle_gold_formula`'nın tavanı da güncellenmeli — bu iki knob birbirine bağlı.
- `debounce_window_seconds` çok yüksekse ve `backup_enabled` false ise veri kaybı riski artar.

## Visual/Audio Requirements

[To be designed]

## UI Requirements

[To be designed]

## Acceptance Criteria

1. **GIVEN** uygulama normal çalışırken, **WHEN** bir save tetikleyici olay gerçekleşirse (ör. zindan katı tamamlanma), **THEN** `save.json` dosyası 0.5 saniye debounce sonrasında güncellenir ve checksum geçerlidir.

2. **GIVEN** geçerli bir `save.json` dosyası varken, **WHEN** uygulama yeniden başlatılırsa, **THEN** tüm oyun verileri (canavarlar, kaynaklar, zindan ilerlemesi, takım preset'leri) tam olarak geri yüklenir.

3. **GIVEN** `save.json` bozukken (geçersiz checksum), **WHEN** uygulama açılırsa, **THEN** sistem `save_backup.json`'dan yükler ve oyuncu bilgilendirilir.

4. **GIVEN** hem `save.json` hem `save_backup.json` bozukken, **WHEN** uygulama açılırsa, **THEN** yeni oyun başlar, bozuk dosyalar `save_corrupted_[timestamp].json` olarak yedeklenir ve oyuncuya bildirim gösterilir.

5. **GIVEN** oyuncu 10 saat önce çıkmışken, **WHEN** uygulamayı yeniden açarsa, **THEN** `offline_duration = 600 dakika` hesaplanır ve downstream sistemlere (Otofarm/Ekonomi) sağlanır.

6. **GIVEN** cihaz saati 2 gün ileriye alınmışken, **WHEN** uygulama açılırsa, **THEN** `offline_duration = 0` olur ve çevrimdışı ödül hesaplanmaz.

7. **GIVEN** `schema_version` 1 olan bir save dosyası ve oyunun `schema_version` 2 beklediği bir durumda, **WHEN** uygulama açılırsa, **THEN** v1→v2 migrasyon fonksiyonu çalışır, yeni alanlar varsayılan değerlerle eklenir ve save yeni formatta kaydedilir.

8. **GIVEN** kaydetme sırasında disk alanı yetersizse, **WHEN** save tetiklenirse, **THEN** mevcut `save.json` korunur (üzerine yazılmaz) ve oyuncuya uyarı gösterilir.

9. **GIVEN** ardışık 3 save tetikleyici olay 0.3 saniye içinde gerçekleşirse, **WHEN** debounce penceresi kapanırsa, **THEN** yalnızca 1 disk yazma işlemi gerçekleşir.

10. **GIVEN** uygulama `OnApplicationPause(true)` alırsa, **WHEN** save tetiklenirse, **THEN** debounce atlanarak anında kaydetme yapılır.

11. **GIVEN** MVP save dosyası (20 canavar, 1 bölge, 3 preset), **WHEN** kaydetme boyutu ölçülürse, **THEN** dosya boyutu 50 KB'ı aşmaz.

12. **GIVEN** kaydetme işlemi başlatılırsa, **WHEN** işlem süresi ölçülürse, **THEN** tüm serileştirme + yazma süreci 100ms'i aşmaz (16.6ms frame budget'ı içinde kalmak için async veya frame dağıtımı uygulanabilir).

## Open Questions

1. **Cloud Save**: Tier 2+'da sunucu tarafı save senkronizasyonu eklenecek mi? Eklenmesi durumunda çatışma çözüm stratejisi (last-write-wins, merge, oyuncu seçimi) ne olacak?
2. **Save Şifreleme**: Oyuncu save dosyasını düzenleyerek hile yapabilir mi (JSON okunabilir format). MVP'de checksum yeterli mi, yoksa şifreleme eklenmeli mi?
3. **Analytics Entegrasyonu**: Save/Load olayları (bozuk dosya, migrasyon, recovery) analytics'e raporlanmalı mı? Hangi olaylar izlenmeli?
4. **Birden Fazla Profil**: Tier 2+ için birden fazla save slot gerekecek mi (ör. aile üyeleri aynı cihaz)?
