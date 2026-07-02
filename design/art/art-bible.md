# Art Bible: Canavar Zindanları (Monster Dungeons)

*Created: 2026-06-23*
*Status: Complete*

> ⚠️ **AÇIK BAYRAK (2026-07-02)**: Element sistemi (Ateş/Su/Toprak/Hava) prototype kapsamından gameplay mekaniği olarak tamamen kaldırıldı (kullanıcı kararı — bkz. `systems-index.md` Deprecated listesi). Bu dosya "Element Renk Paleti" (Section 4.3), "Element Geometrik Modifikatörü" ve `char_monster_[element]_[archetype]_...` dosya adlandırma şemasını **görsel çeşitlilik ekseni** olarak kullanmaya devam ediyor — bu artık bir gameplay sistemine bağlı değil, sadece kozmetik/tema kimliği. Element'i tam bir görsel eksen olarak değiştirmek (ör. arketip veya nadirlik bazlı yeni bir renk kodlama şemasına geçmek) `art-director` ile ayrı bir tasarım oturumu gerektiren büyük bir kapsam — bu bulk temizlik oturumunun dışında bırakıldı. Tek net mekanik kalıntı (Section 8, "Element Avantaj Okları" UI göstergesi) aşağıda düzeltildi çünkü doğrudan kaldırılan hasar formülüne (element_multiplier) referans veriyordu.

---

## 1. Visual Identity Statement

**Görsel Kuzey Yıldızı (Visual North Star)**

> **"Parlak, güçlü ve cömert — her piksel oyuncuya 'sen güçlüsün ve ödüllendiriliyorsun' demelidir."**

---

**Destekleyici Görsel İlkeler**

**1. Zenginlik Okunur Olmalı (Pillar: Cömert Zindan + Güç Hisset)**

Ekrandaki her ödül, her güçlenme ve her nadir canavar görsel olarak abartılı biçimde kutlanmalıdır. Altın parıltılar, ışık patlamaları, renk doygunluğu artışları — oyuncunun kazancı sessiz kalmamalı.

*Tasarım testi*: "Bu görsel geri bildirim, oyuncunun eline geçen ödülü ekran görüntüsü almak isteyeceği kadar etkileyici gösteriyor mu? Hayır ise, efekti ve kontrastı artır."

**2. Her Canavar Silüette Tanınmalı (Pillar: Topla Hepsini)**

Hiçbir canavar başka bir canavarla karıştırılmamalıdır. Renk paleti, silüet ve boyut oranları yeterince farklı olmalı ki oyuncu 64x64 piksel küçültülmüş bir ikonda bile hangi canavarın hangisi olduğunu anlayabilsin. Element renkleri (ateş=turuncu-kırmızı, su=mavi-turkuaz, toprak=yeşil-kahve, hava=beyaz-mor) her tasarımda baskın renk olarak yer alır.

*Tasarım testi*: "Bu canavarın silüeti siyah-beyaza çevrildiğinde, koleksiyondaki diğer canavarlardan ayırt edilebiliyor mu? Hayır ise, silüeti yeniden tasarla."

**3. Aydınlık Hakimiyet, Karanlık Değil (Pillar: Güç Hisset + Senin Tempon)**

Zindanlar atmosferik olabilir ama asla kasvetli veya korkutucu olmamalıdır. Işık kaynakları sıcak ve bolca yerleştirilir. Arka plan tonları doygun ve canlıdır — soluk, gri veya desatüre paletler yasaktır. Oyuncu bir zindana girerken "tehlikeli bir yere giriyorum" değil, "fethedilecek bir alan buldum" hissetmelidir.

*Tasarım testi*: "Bu sahneyi gece yarısı yatakta telefonda gören bir oyuncu, ekranı karartma ihtiyacı mı hisseder yoksa renkler onu çeker mi? Renkler çekmiyorsa, aydınlatmayı ve doygunluğu artır."

---

## 2. Mood & Atmosphere

### Zindan Keşfi (Dungeon Exploration)

**Birincil Duygu Hedefi**: Meraklı fetih hissi — "Ne bulacağım?" sorusunun heyecanı, keşfedilmemiş topraklara adım atan güçlü bir komutanın özgüveni.

**Aydınlatma Karakteri**: Sıcak altın saat (golden hour) yönlendirmesi. Renk sıcaklığı: 3500-4500K (sıcak amber tonları). Kontrast: orta-düşük — gölgeler yumuşak ve şeffaf, hiçbir köşe tamamen karanlık değil. Ana ışık kaynağı sahne üstünden 45 derece açıyla gelir; meşale, kristal ve büyülü bitki gibi çevresel ışıklar derinlik yaratır.

**Atmosferik Tanımlayıcılar**: Sıcak, gizemli, davetkar, görkemli, keşfe açık.

**Enerji Seviyesi**: Ölçülü (measured) — yavaş ama durağan değil. Ortamda sürekli hareket var (parçacık efektleri, titreşen ışıklar) ama tempo rahat ve kontrol oyuncuda.

**Atmosferi Taşıyan Görsel Eleman**: **Yol gösteren ortam ışıkları** — her koridorun ilerisinde hafifçe parlayan bir ışık kaynağı (meşale, kristal, parıldayan mantar) görünür. Bu ışıklar oyuncuyu ileriye çeker ve "karanlık bir deliğe giriyorsun" hissini "hazine dolu bir yolu keşfediyorsun" hissine dönüştürür. Işıklar kat derinliğine göre renk değiştirir: üst katlar sıcak amber, derin katlar turkuaz-mor — ama hepsi parlak ve doygun kalır.

---

### Savaş (Combat — Normal Düşman Dalgaları)

**Birincil Duygu Hedefi**: Zahmetsiz hakimiyet — oyuncu düşmanları ezip geçen güçlü bir ordunun komutanı gibi hissetmeli. Stres değil, tatmin.

**Aydınlatma Karakteri**: Yüksek doygunluklu gündüz savaş alanı. Renk sıcaklığı: 5000-5500K (nötr-sıcak beyaz). Kontrast: yüksek — canavar yetenekleri ve vuruş efektleri arka plandan net biçimde ayrışır. Sahne aydınlatması keşif modundan %20 daha parlak; yetenek kullanımlarında anlık ışık patlamaları (flash) ortam aydınlatmasını geçici olarak override eder.

**Atmosferik Tanımlayıcılar**: Enerjik, parlak, güçlü, akışkan, tatmin edici.

**Enerji Seviyesi**: Kıpırdak-akışkan (brisk) — sürekli hareket ve geri bildirim var ama kaotik değil. Ritmik bir akış: vuruş → geri bildirim → vuruş → loot. Komutan modunda tempo hafifçe yavaşlar (taktik an), otofarm modunda tam akış hızı.

**Atmosferi Taşıyan Görsel Eleman**: **Element renk patlamaları** — her canavar yeteneği kullandığında o elementin ana renginde (ateş=turuncu-kırmızı, su=mavi-turkuaz, toprak=yeşil-kahve, hava=beyaz-mor) bir ışık dalgası tüm sahneyi anlık olarak boyar. Bu hem güçlü hissettiren bir geri bildirim hem de element sistemini görsel olarak öğreten bir mekanik. Savaş alanı asla renksiz kalmaz.

---

### Boss Savaşı (Boss Battle)

**Birincil Duygu Hedefi**: Epik karşılaşma — "Bu büyük bir an" hissi. Korku değil, heyecan. Boss güçlü görünmeli ama yenilebilir — oyuncu "bu devasa yaratığı ben yendim" diye övünebilmeli.

**Aydınlatma Karakteri**: Dramatik sahne aydınlatması. Renk sıcaklığı ikili: Boss'un element renginde güçlü bir ana ışık (alttan veya arkadan rim light) + oyuncunun takımına vuran sıcak altın dolgu ışığı. Kontrast: çok yüksek — boss arenası dışı kararır (%60 dim), tüm görsel dikkat merkeze çekilir. Boss HP düştükçe sahne aydınlatması kademeli olarak oyuncunun altın tonlarına kayar (hakimiyet geçişi).

**Atmosferik Tanımlayıcılar**: Epik, dramatik, yoğun, görkemli, ödüllendirici.

**Enerji Seviyesi**: Yoğun (intense) — normal savaştan belirgin biçimde daha gergin. Tempo hızlanır, ekrandaki efekt yoğunluğu artar. Ama "bunaltıcı" sınırına geçmez — oyuncu kontrol hissini kaybetmez.

**Atmosferi Taşıyan Görsel Eleman**: **Dinamik hakimiyet aydınlatması** — boss'un HP çubuğu azaldıkça sahne renk dengesi değişir. Savaş başında boss'un element rengi sahneye hakim. HP %50'nin altına düştüğünde oyuncunun altın tonu yükselmeye başlar. Son darbe anında tüm sahne parlak altın-beyaz bir ışık patlamasıyla yıkanır.

---

### Loot / Ödül Anı (Reward Moment)

**Birincil Duygu Hedefi**: Saf sevinç ve bolluk — "Ne kadar çok şey kazandım!" hissi. Oyuncu kendini şanslı, değerli ve ödüllendirilmiş hissetmeli. Bu, oyuncunun ekran görüntüsü almak isteyeceği andır.

**Aydınlatma Karakteri**: Aşırı doygun kutlama aydınlatması. Renk sıcaklığı: 4000K sıcak altın baskın, nadirlik seviyesine göre mor/turkuaz accent ışıklar eklenir. Kontrast: düşük ortam + çok yüksek öğe kontrastı — arka plan soft blur ile %30-40 söner, ödül öğeleri ise parlak glow ve rim light ile öne çıkar.

**Atmosferik Tanımlayıcılar**: Gösterişli, cömert, kutlamalı, parlak, abartılı.

**Enerji Seviyesi**: Patlayıcı-kutlamalı (celebratory burst) — kısa süreli yoğun enerji patlaması, ardından tatmin edici yavaşlama. Sandık açılışında patlama → loot parçacıkları yağmuru → her öğenin teker teker ortaya çıkışı → nadir öğelerde ekstra efekt katmanı.

**Atmosferi Taşıyan Görsel Eleman**: **Nadirlik kademeli ışık tepkisi** — loot nadirliğine göre kademeli olarak artan görsel kutlama sistemi. Yaygın (Common): hafif altın parıltı. Nadir (Rare): mavi-altın ışık halkası + parçacık yağmuru. Efsanevi (Legendary): tüm ekranı kaplayan altın ışık patlaması + yavaşlayan zaman efekti (slow-mo reveal) + parlayan silüet sunumu.

---

### Canavar Yönetimi (Monster Management)

**Birincil Duygu Hedefi**: Gururlu sahiplik — "Bak ne kadrosum var" hissi. Koleksiyoncu tatmini, güçlendirme heyecanı ve stratejik planlama keyfi.

**Aydınlatma Karakteri**: Sıcak iç mekan stüdyo aydınlatması. Renk sıcaklığı: 4500-5000K (sıcak nötr). Kontrast: orta — canavarlar net ve detaylı görünmeli, arka plan onları desteklemeli ama yarışmamalı. Seçili canavar üzerine soft spotlight düşer; canavarın element rengi kartının ve arka plan accent'inin tonunu belirler. Arka plan desatüre koyu lacivert veya koyu mor gradient.

**Atmosferik Tanımlayıcılar**: Düzenli, gururlu, sıcak, detaylı, vitrin benzeri.

**Enerji Seviyesi**: Düşünceli (contemplative) — sakin ve odaklanmış. Animasyonlar yavaş ve zarif. Güçlendirme ve evrim anlarında kısa enerji patlamaları — sonra tekrar sakin ritme dönüş.

**Atmosferi Taşıyan Görsel Eleman**: **Canavar vitrin aydınlatması** — her canavar kartı/modeli seçildiğinde, canavarın element renginde yumuşak bir arka aydınlatma (backlight) aktive olur ve canavar karanlık bir arka plan üzerinde "ışıldar". Evrim anında bu ışık dramatik biçimde yoğunlaşır, renk değişir ve canavar silüeti ışık içinde dönüşür.

---

### Ana Menü / Hub

**Birincil Duygu Hedefi**: Karşılama ve bolluğun vaadi — "Hoş geldin, seni çok şey bekliyor" hissi. Günlük ödüller, yeni içerikler ve görevler görsel olarak "gel, al" der.

**Aydınlatma Karakteri**: Parlak sabah güneşi. Renk sıcaklığı: 5500-6000K (sıcak beyaz, sabah netliği). Kontrast: düşük-orta — her şey okunabilir, hiçbir element gölgede kalmaz. Günlük ödüller ve yeni içerik göstergeleri üzerinde yumuşak pulsing glow efekti dikkat çeker.

**Atmosferik Tanımlayıcılar**: Davetkar, temiz, enerjik, düzenli, umut dolu.

**Enerji Seviyesi**: Canlı-karşılayıcı (welcoming buzz) — durağan değil ama bunaltıcı da değil. Hafif animasyonlar her yerde ama dikkat dağıtıcı değil.

**Atmosferi Taşıyan Görsel Eleman**: **Yaşayan hub mekanı** — hub ekranında oyuncunun lead canavarı (takımdaki en güçlü canavar) idle animasyonuyla görünür ve ortamla etkileşir. Canavarın element rengine göre hub'ın accent aydınlatması hafifçe değişir — oyuncunun hub'ı "onun hub'ı" hisseder.

---

### Arena (PvP — Asenkron)

**Birincil Duygu Hedefi**: Rekabetçi gurur — "Ordumu test ediyorum ve kazanıyorum" hissi. Dostane rekabet, korkutucu PvP gerilimi değil.

**Aydınlatma Karakteri**: Stadyum/arena sahne aydınlatması. Renk sıcaklığı: 5500K nötr beyaz ana ışık + mavi-kırmızı iki taraflı takım aydınlatması. Kontrast: yüksek — arena zemini parlak, kenarlar kararır. Oyuncunun tarafı sıcak altın tonla, rakip taraf soğuk gümüş-mavi tonla aydınlatılır.

**Atmosferik Tanımlayıcılar**: Rekabetçi, gösterişli, adaletli, heyecanlı, sportif.

**Enerji Seviyesi**: Kıpırdak-heyecanlı (competitive brisk) — normal savaştan daha yüksek enerji, boss savaşından daha düşük yoğunluk.

**Atmosferi Taşıyan Görsel Eleman**: **İkili ışık karşıtlığı** — arena sahnesinde oyuncunun tarafı sıcak altın tonla, rakibin tarafı soğuk gümüş-mavi tonla aydınlatılır. Savaş ilerledikçe kazanan tarafın ışığı diğer tarafa doğru genişler. Zafer anında tüm arena kazananın renk tonuyla yıkanır.

---

### Durum Geçişleri ve Görsel Tutarlılık

- **Renk sıcaklığı sürekliliği**: Hiçbir geçiş ani soğuk-sıcak atlama yapmamalı. Geçişler kademeli olmalı.
- **Aydınlık sütunu korunmalı**: Hiçbir durum %40'ın altına karartılmamalı. En dramatik anda bile merkez alan parlak kalır.
- **Enerji eğrisi**: Keşif (ölçülü) → Savaş (kıpırdak) → Boss (yoğun) → Loot (patlayıcı kutlama) → Yönetim (düşünceli) → Hub (canlı karşılama) → Arena (rekabetçi)
- **Element renkleri her durumda tutarlı**: Ateş=turuncu-kırmızı, su=mavi-turkuaz, toprak=yeşil-kahve, hava=beyaz-mor — bu renkler hiçbir durumda değişmez, sadece yoğunluk ve uygulama biçimi farklılaşır.

---

## 3. Shape Language

### Temel Felsefe: "Güçlü ve Okunabilir Geometri"

Oyunun form dili tek bir kuralla özetlenir: **her şekil bir duygu taşır ve o duyguyu 64x64 piksel boyutunda bile iletebilir.** Renkler neyin önemli olduğunu söyler; şekiller ise neyin güçlü, neyin tehlikeli, neyin güvenli olduğunu söyler.

Oyunun geometrik dilinde iki kutup vardır:

| Kutup | Geometri | Duygu | Kullanım |
|-------|----------|-------|----------|
| **Güç Kutbu** | Geniş omuzlar, yukarı doğru genişleyen formlar, kalın konturlar, sivri aksan noktaları | Hakimiyet, güç, heybetlilik | Oyuncunun canavarları, boss'lar, ödül çerçeveleri |
| **Konfor Kutbu** | Yuvarlatılmış köşeler, dairesel öğeler, yumuşak geçişler | Güvenlik, zenginlik, erişilebilirlik | UI elementleri, hub mekanları, loot gösterimi |

---

### 3.1 Canavar / Karakter Silüet Felsefesi

#### Küçük Boyut Okunabilirliği Kuralı

Her canavar tasarımı aşağıdaki silüet testini geçmelidir:

1. **Siyah-Beyaz Silüet Testi**: Canavarın silüeti tamamen siyah doldurulduğunda, koleksiyondaki diğer tüm canavarlardan ayırt edilebilmeli.
2. **64x64 Piksel Testi**: Canavar 64x64 piksel boyutuna küçültüldüğünde en az 1 tanımlayıcı şekil özelliği seçilebilmeli.
3. **3 Saniye Hafıza Testi**: Oyuncu silüete 3 saniye bakıp gözlerini kapatınca, en az 1 ayırt edici özelliği hatırlayabilmeli.

#### Arketip Bazlı Geometrik Sözlük

| Arketip | Birincil Geometri | Silüet Özelliği | Duygusal İletişim | Pillar Bağlantısı |
|---------|-------------------|------------------|--------------------|--------------------|
| **Saldırgan (Striker)** | Sivri üçgenler, ileri doğru eğimli formlar, dar ve uzun oranlar | Sivri pençeler, boynuzlar veya dikenler silüeti kırar | "Bu canavar hızlı ve tehlikeli vurur" | Güç Hisset |
| **Tank (Defender)** | Geniş dikdörtgenler, yatay baskın oranlar, kalın bacaklar | Kare-geniş gövde silüeti, kısa boyun, masif omuzlar | "Bu canavar yıkılmaz bir duvar" | Güç Hisset |
| **Destekçi (Support)** | Yuvarlak ve organik formlar, simetrik eğriler, yumuşak kenarlar | Yuvarlak baş-gövde oranı, kanatlar veya sarmallar | "Bu canavar iyileştirici ve koruyucu" | Senin Tempon |
| **Büyücü (Mage)** | İnce, dikey baskın oranlar, yüzen veya asimetrik parçalar | Uzun boylu dar silüet, yüzen element efektleri | "Bu canavar gizemli ve güçlü enerjiler kontrol ediyor" | Topla Hepsini |
| **Canavar Lordu (Boss)** | Arketiplerin abartılmışı: daha büyük, daha karmaşık, daha çok çıkıntı | Normal canavarın 2-3x silüet alanı, çok sayıda kırıcı çıkıntı | "Bu büyük bir mücadele, ama yenilebilir" | Güç Hisset |

**Arketip-Geometri Çarpışma Kuralı**: Aynı arketipteki iki canavarın silüeti %30'dan fazla örtüşemez.

#### Element Renginin Geometriyle Etkileşimi

| Element | Renk Paleti | Geometrik Modifikatör | Örnek |
|---------|-------------|----------------------|-------|
| **Ateş** | Turuncu-kırmızı | Üçgensel sivri uçlar, düzensiz/yırtık kenarlar | Tank zırhında sivri alev çıkıntıları |
| **Su** | Mavi-turkuaz | Dalga formu eğrileri, akışkan S-kıvrımları, yuvarlatılmış köşeler | Saldırganın dalga formunda kıvrılmış pençeleri |
| **Toprak** | Yeşil-kahverengi | Köşeli, kristalize formlar, kırık/facetli yüzeyler, ağır tabanlı geometri | Büyücünün etrafında yüzen geometrik kayalar |
| **Hava** | Beyaz-mor | Hafif, ince, dağılmış formlar, parçalanmış kenarlar, boşluklu silüet | Destekçinin duman gibi dağılan gövde kenarları |

**Kuralı**: Element geometrisi arketip geometrisini geçersiz kılmaz, üzerine eklenir.

#### Nadirlik Kademesinin Geometriye Etkisi

| Nadirlik | Kontur | Detay Yoğunluğu | Silüet Karmaşıklığı | Ek Geometrik Özellik |
|----------|--------|------------------|-----------------------|----------------------|
| **Yaygın (Common)** | 2px düz siyah kontur | Düşük — temel form | 3-5 çıkıntı noktası | Yok |
| **Nadir (Rare)** | 2px siyah + 1px element rengi iç çizgi | Orta — zırh deseni, eklem detayları | 5-8 çıkıntı noktası | 1 yüzen aksesuar |
| **Efsanevi (Legendary)** | 3px kontur + 2px element rengi dış glow | Yüksek — kanat desenleri, yüzey doku çeşitliliği | 8-12 çıkıntı noktası | 2-3 yüzen aksesuar + enerji efektleri |

**Geometri Bütçesi**: Yaygın max 3 parça, Nadir 4-5 parça, Efsanevi 6-8 parça + yüzen efektler.

---

### 3.2 Çevre Geometrisi

#### Genel Felsefe

Zindan ortamları organik-geometrik hibrit: **yapısal elemanlar (duvarlar, zemin) geometrik ve okunabilir, doğal elemanlar (bitkiler, kayalar) organik ve atmosferik.** Oyuncu geometrik yapıları "etkileşilebilir alan", organik formları "dekoratif atmosfer" olarak okur.

**Yatay-Dikey Oran Kuralı**: Zindan koridorları en az 2:1 genişlik:yükseklik. Dikey baskınlık sadece boss odaları ve ödül anlarında.

#### Bölge Bazlı Geometrik Farklılaşma

| Bölge | Birincil Geometri | Yapısal Motif | Duygusal Hedef |
|-------|-------------------|---------------|----------------|
| **Kristal Mağaraları** (başlangıç) | Facetli geometrik, altıgen/beşgen duvar desenleri | Kristal kümeleri ışık yansıtır | "Hazine dolu mağara keşfi" |
| **Lavlı Derinlikler** | Akışkan eğriler + kırık keskin kenarlar | Soğumuş lav: düzensiz, çatlak yüzeyler | "Tehlikeli ama güçlü bölge" |
| **Batık Tapınak** | Dikey dikdörtgenler, sütunlar, simetrik kemerler | Yarı yıkık sütunlar — düzen içinde bozulma | "Eski görkemli yeri fethetme" |
| **Gökyüzü Kalesi** | Hafif ince dikey formlar, sivri kuleler, boşluklar | Yüzen kale parçaları, bulut köprüleri | "Zirveye ulaştın — başarı" |

**Derinlik Kademesi**: Üst katlar (1-3): düzenli, simetrik, geniş → Orta (4-7): hafif asimetri, daralan → Derin (8-10): belirgin asimetri, yoğun detay. Derinlik geometrikle ifade edilir, aydınlatma kısılmasıyla değil.

#### Alan Tipi Geometrisi

| Alan Tipi | Geometrik Kural | Neden |
|-----------|-----------------|-------|
| **Koridor** | Yatay, dar, yönlendirici çizgiler, ileride ışık | Keşif merakı |
| **Savaş alanı** | Geniş, açık arena; basit arka plan | Savaş okunabilirliği |
| **Boss odası** | Dikey genişleme, merkezi boss alanı | Boss büyüklüğünü vurgulama |
| **Loot odası** | Merkeze daralan çizgiler, yükseltilmiş platform | Ödüle dikkat çekme |

---

### 3.3 UI Form Grameri

#### UI-Dünya İlişkisi

UI, dünya estetiğinden bağımsız bir dil kullanır ama onu tamamlar:
- **Dünya**: Organik-geometrik hibrit, element bazlı renk çeşitliliği
- **UI**: Temiz, yuvarlatılmış, tutarlı, element nötr

**Bağlantı noktaları**: Element renk kodları UI'da da geçerli, nadirlik glow'ları UI çerçevelerinde de kullanılır.

#### Temel UI Şekil Kuralları

| UI Elementi | Şekil | Köşe Yarıçapı | Kontur |
|-------------|-------|---------------|--------|
| **Birincil Buton** | Yuvarlatılmış dikdörtgen | 12-16px | 2px altın/amber gradient |
| **İkincil Buton** | Yuvarlatılmış dikdörtgen | 8-12px | 1px yarı saydam beyaz |
| **Kart (canavar, eşya)** | Yuvarlatılmış dikdörtgen, alt düz | 12px üst, 0px alt | Nadirlik renginde 2px + iç glow |
| **Panel / Modal** | Yuvarlatılmış dikdörtgen | 16-20px | 1px yarı saydam + gölge |
| **HP/Enerji Çubuğu** | Yatay kapsül | Tam yuvarlak uçlar | Dolgu rengi iletişim sağlar |
| **İkon çerçevesi** | Kare, yuvarlatılmış | 8px | Element/nadirlik renginde 2px |

**44x44dp Minimum Dokunma Alanı**: Tüm etkileşimli UI öğeleri en az 44x44dp dokunma hedefine sahiptir.

#### Nadirlik Çerçeve Sistemi

| Nadirlik | Çerçeve | Köşe Detayı | Ek Efekt |
|----------|---------|-------------|----------|
| **Yaygın** | 2px gri kontur | 8px yuvarlak | Yok |
| **Nadir** | 2px mavi gradient | 8px + elmas motifi | Hafif iç glow |
| **Efsanevi** | 3px altın gradient + dış glow | 12px + yıldız/taç motifi | Pulsing altın glow + parçacık |

---

### 3.4 Ön Plan vs Arka Plan Şekil Kontrastı

#### "Kahraman Keskin, Sahne Yumuşak"

| Katman | Kontur | Detay | Doygunluk | Rol |
|--------|--------|-------|-----------|-----|
| **4 — Oyuncu Canavarları** | 3px siyah + 1px element | Maksimum | %100 | Ana dikkat odağı |
| **3 — Düşman Canavarları** | 2px koyu gri | Yüksek | %80-90 | Yenilecek hedef |
| **2 — Ön plan çevre** | 1px yumuşak | Orta | %60-70 | Derinlik çerçevesi |
| **1 — Orta plan çevre** | Kontur yok | Düşük-orta | %50-60 | Mekan bağlamı |
| **0 — Uzak arka plan** | Kontur yok | Minimal | %30-40 | Atmosferik derinlik |

**Ödül Anında Tersine Çevirme**: Arka plan %30-40 blur+dim, ödül öğesi 1.5-2x büyük, altın ışık halkaları. "Dünya durur, sadece kazanç var."

---

### 3.5 AI Prompt Rehberi (Şekil Kontrol Parametreleri)

**Canavar prompt anahtar kelimeleri:**
- Arketip: "angular/spiky" (saldırgan), "wide/blocky" (tank), "round/smooth" (destekçi), "tall/slender floating" (büyücü)
- Element: "flame-like jagged" (ateş), "flowing wave curves" (su), "crystalline faceted" (toprak), "wispy dissolving" (hava)
- Nadirlik: "simple bold shapes" (yaygın), "moderate detail + 1 floating accessory" (nadir), "complex silhouette + multiple floating energy elements" (efsanevi)
- Her zaman: "clear readable silhouette, distinguishable at small sizes, thick outlines"

**Çevre prompt anahtar kelimeleri:**
- "Soft background elements, no hard outlines on environment"
- "Strong figure-ground separation, characters pop against background"
- "Warm lighting, never gloomy or desaturated"

---

### 3.6 Tasarım Testleri

1. **Silüet Testi**: Siyah siluete çevrildiğinde ne olduğu anlaşılabiliyor mu?
2. **64px Testi**: Küçültüldüğünde ayırt edici şekil görülebiliyor mu?
3. **Hiyerarşi Testi**: Sahnede arka plandan net biçimde ayrışıyor mu?
4. **Arketip Testi**: Geometrik DNA arketip tablosuyla uyuşuyor mu?
5. **Element Testi**: Element geometrik modifikatörü uygulanmış mı?
6. **Nadirlik Testi**: Karmaşıklık nadirlik kademesiyle orantılı mı?
7. **Pillar Testi**: Bu şekil seçimi hangi pillar'ı destekliyor?

---

## 4. Color System

### 4.1 Birincil Palet (Primary Palette)

| Rol | Renk Adı | Hex | Anlam ve Kullanım |
|-----|----------|-----|-------------------|
| **Ana Altın** | Zafer Altını | `#FFD700` | Gücün, zenginliğin ve başarının rengi. Loot parıltıları, nadirlik vurgusu, başarı efektleri, UI vurgu çizgileri. |
| **Sıcak Amber** | Yuva Amberi | `#F5A623` | Güvenlik, sıcaklık ve ev hissi. Hub arka plan tonları, UI buton gradientleri, XP çubuğu, yol gösteren ışıklar. |
| **Derin Lacivert** | Gece Mavisi | `#1A1A3E` | Derinlik, gizem ve UI temeli. Panel arka planları, karanlık katmanlar, kontrast zemini. |
| **Canlı Kırmızı** | Tehlike Kırmızısı | `#E53E3E` | Hasar, HP kaybı, kritik uyarılar, düşman saldırıları. Uyarıcı ama korkutucu değil. |
| **Taze Yeşil** | Büyüme Yeşili | `#48BB78` | İyileşme, seviye atlama, başarı tamamlama, canavar evrimi. Pozitif ilerleme. |
| **Parlak Beyaz** | Işık Beyazı | `#F7FAFC` | Temizlik, okunabilirlik, UI metinleri, güç patlamaları. Asla saf `#FFFFFF` kullanılmaz. |
| **Kral Moru** | Nadirlik Moru | `#805AD5` | Gizem, nadirlik, özel içerik, sezon etkinlikleri. |

**Oran Kuralı**: %50-60 nötr zemin (Gece Mavisi), %25-30 sıcak tonlar (Altın + Amber), %10-15 vurgu renkleri, %5 parlak beyaz.

**Yasak**: Gri baskın sahneler, soğuk mavi baskın sahneler (Arena hariç), desatüre paletler.

---

### 4.2 Semantik Renk Kullanımı

| Renk | İletişim | Kullanım Alanları | Neden? |
|------|----------|-------------------|--------|
| **Altın** (`#FFD700`–`#FFC107`) | "Ödüllendiriliyorsun" | Loot parıltısı, madeni para, nadirlik glow, XP kazanımı, başarı rozetleri | Pillar 1 + 4 |
| **Kırmızı** (`#E53E3E`–`#FC8181`) | "Dikkat / hasar" | HP çubuğu kaybı, düşman saldırıları, kritik uyarılar | Biyolojik tehlike içgüdüsü |
| **Yeşil** (`#48BB78`–`#9AE6B4`) | "İyileşme / büyüme" | İyileşme efektleri, seviye atlama, görev tamamlama | Pozitif ilerleme |
| **Mavi** (`#4299E1`–`#90CDF4`) | "Bilgi / mana / su" | Mana/enerji çubuğu, bilgi tooltip'leri, su element efektleri | Sakinlik ve bilgi |
| **Mor** (`#805AD5`–`#B794F4`) | "Nadirlik / gizem" | Efsanevi efektler, özel etkinlik işareti, hava element efektleri | Kraliyet ve enderlik |
| **Turuncu** (`#ED8936`–`#FBD38D`) | "Enerji / ateş / hareket" | Ateş element efektleri, komutan modu aktif göstergesi | Enerji ve aksiyon |
| **Beyaz** (`#F7FAFC`–`#FFFFFF`) | "Saf güç / netlik" | Kritik vuruş efekti, boss son darbe patlaması, UI metin | Doruk güç anı |

**Çakışma Kuralı**: Kırmızı (hasar) ve yeşil (iyileşme) aynı sahnede ise mekansal ayrım uygulanır.

---

### 4.3 Element Renk Paleti

#### Ateş Elementi

| Ton | Hex | Kullanım |
|-----|-----|----------|
| **Ana** | `#E8530A` | Canavar ana gövde, element ikonu, UI kart kenarı |
| **Açık** | `#FBD38D` | Ateş efektlerinin dış kenarı, parıltılar |
| **Koyu** | `#9C2A0A` | Gölge tonları, koyu detaylar |
| **Aksan** | `#FFF01F` | Alev uçları, kritik vuruş kıvılcımı |
| **Glow** | `#FF6B35` (alpha %40-60) | Canavar ortam ışığı, arka plan ışık kaynakları |

#### Su Elementi

| Ton | Hex | Kullanım |
|-----|-----|----------|
| **Ana** | `#2B6CB0` | Canavar ana gövde, element ikonu |
| **Açık** | `#BEE3F8` | Su yansımaları, iyileşme efekti |
| **Koyu** | `#1A365D` | Derin su gölgeleri, koyu detaylar |
| **Aksan** | `#00E5FF` | Su damlaları parıltısı, kritik yetenek |
| **Glow** | `#63B3ED` (alpha %40-60) | Canavar ortam ışığı |

#### Toprak Elementi

| Ton | Hex | Kullanım |
|-----|-----|----------|
| **Ana** | `#38A169` | Canavar gövdesinde baskın yeşil |
| **Açık** | `#C6F6D5` | Yaprak parıltısı, büyüme efekti |
| **Koyu** | `#5D4037` | Toprak ve kaya tonları, gölge |
| **Aksan** | `#F6E05E` | Kristal parıltısı, enerji merkezi |
| **Glow** | `#68D391` (alpha %40-60) | Canavar ortam ışığı |

#### Hava Elementi

| Ton | Hex | Kullanım |
|-----|-----|----------|
| **Ana** | `#805AD5` | Canavar gövdesinde baskın mor |
| **Açık** | `#E9D8FD` | Rüzgar izleri, sis efekti |
| **Koyu** | `#44337A` | Derin gece gökyüzü, gölge |
| **Aksan** | `#FFFFFF` | Yıldız kıvılcımı, şimşek |
| **Glow** | `#D6BCFA` (alpha %40-60) | Yüzen enerji halkası |

#### Çoklu Element Kuralları

1. **Dominant Element**: Sahnede en fazla 1 element arka planı boyar
2. **Komşuluk**: Aynı arketipte 2 farklı element yan yana ise açık/koyu ton farklılaştırılır
3. **Takım Dengesi**: 4-5 canavar takımında her element eşit görsel ağırlıkta
4. **Boss İstisnası**: Boss'un element rengi sahneye hakim olur
5. **Yakın Tonlar**: Ateş+Toprak gibi yakın sıcaklıklarda birinin doygunluğu %15 düşürülür

---

### 4.4 Nadirlik Renk Sistemi

| Nadirlik | Ana Renk | Hex | Glow | Efekt |
|----------|----------|-----|------|-------|
| **Yaygın (Common)** | Gri-gümüş | `#A0AEC0` | Yok | Yok |
| **Seyrek (Uncommon)** | Yeşil | `#48BB78` | Hafif, alpha %20 | 1px yeşil kenar |
| **Nadir (Rare)** | Mavi-safir | `#4299E1` | Orta, alpha %40, 2px | Mavi parçacık + ışık halkası |
| **Epik (Epic)** | Mor-ametist | `#805AD5` | Güçlü, alpha %60, 3px | Mor enerji spirali + pulse |
| **Efsanevi (Legendary)** | Altın | `#FFD700` | Yoğun, alpha %80, 4px + dış halka | Altın patlama + slow-mo + parçacık |

**Glow Formülü**: Yaygın=0px → Seyrek=2px/blur4 → Nadir=4px/blur8 → Epik=6px/blur12 → Efsanevi=10px/blur20+pulse

**Loot Anı Özel Davranışı**: Nadirliğe göre sahne renk tint'i: Common=%0 → Uncommon=%5 yeşil → Rare=%15 mavi → Epic=%25 mor → Legendary=%40 altın + slow-mo + kamera zoom

---

### 4.5 Biyom Bazlı Renk Sıcaklığı

| Biyom | Sıcaklık | Baskın Tonlar | Aksan Renkleri | Kimlik |
|-------|----------|---------------|----------------|--------|
| **Kristal Mağaraları** | 4000-4500K | Amber altın + turkuaz | `#4FD1C5`, `#B794F4` | "Altın mağarada turkuaz kristaller" |
| **Lavlı Derinlikler** | 3000-3500K | Koyu kırmızı + parlak turuncu-sarı | `#FFF01F`, `#1A1A2E` | "Alevlerin içinde ışık karanlığı yener" |
| **Batık Tapınak** | 4500-5000K | Derin turkuaz + kumtası bej | `#D69E2E`, `#2B6CB0` | "Su altında kaybolan hazineler" |
| **Gökyüzü Kalesi** | 5500-6500K | Açık gök mavisi + beyaz + altın | `#FFD700`, `#B794F4` | "Göklerin üstünde güneş ışığında" |

**Geçiş Kuralı**: Biyomlar arası 2-3 saniye kademeli renk sıcaklığı geçişi. Ani atlama yasak.

---

### 4.6 UI Paleti

| Rol | Renk | Hex |
|-----|------|-----|
| **UI Arka Plan (Birincil)** | Koyu lacivert | `#1A1A3E` |
| **UI Arka Plan (İkincil)** | Orta lacivert | `#2D2D5E` |
| **UI Arka Plan (Üçüncül)** | Koyu mor-gri | `#3D3D6E` |
| **Metin (Birincil)** | Sıcak beyaz | `#F7FAFC` |
| **Metin (İkincil)** | Soluk lavanta | `#A0AEC0` |
| **Metin (Devre Dışı)** | Koyu gri | `#4A5568` |
| **Buton (CTA)** | Altın gradient | `#FFD700` → `#F5A623` |
| **Buton (İkincil)** | Yarı saydam beyaz | `#FFFFFF` alpha %15 |
| **Buton (Tehlike)** | Canlı kırmızı | `#E53E3E` |
| **Başarı** | Taze yeşil | `#48BB78` |
| **Uyarı** | Amber | `#ED8936` |
| **Hata** | Kırmızı | `#E53E3E` |
| **Bilgi** | Açık mavi | `#4299E1` |

**Etkileşim Durumları**: Normal → Dokunma (+%15 parlaklık, 2px glow) → Basılı (-%10, iç gölge, scale 0.96) → Devre Dışı (gri, %50 opasite) → Yeni (kırmızı nokta + pulse)

**Saydamlık**: Panel %85-95, overlay %60 siyah dim + blur, toast %90 opak.

---

### 4.7 Renk Körlüğü Güvenliği

#### Riskli Çiftler ve Yedek İpuçları

| Risk Çifti | Risk | Yedek 1: Şekil | Yedek 2: Hareket | Yedek 3: Ses |
|------------|------|----------------|------------------|--------------|
| **Kırmızı-Yeşil** (hasar/iyileşme) | YÜKSEK | Hasar: ▼ ok, İyileşme: ▲ ok | Hasar: sarsıntılı, İyileşme: yumuşak süzülme | Hasar: sert "tink", İyileşme: yumuşak "chime" |
| **Element Ayrımı** (4 element) | ORTA | Ateş=alev, Su=damla, Toprak=yaprak, Hava=spiral | Çerçeve deseni: zigzag/dalga/nokta/spiral | Ateş=çatırdı, Su=sıçrama, Toprak=gürültü, Hava=ıslık |
| **Nadirlik Kademesi** | ORTA | Motif: düz/1 yıldız/2 yıldız/taç/kanatlı taç | Glow hızı: yok→çok yavaş→yavaş→orta→hızlı pulse | Ses yoğunluğu kademeli artış |

#### Renk Körlüğü Modları

| Mod | Değişiklik | Hedef |
|-----|-----------|-------|
| **Protanopi** | Kırmızı → sarı-turuncu, yeşil → mavi-yeşil; ikonlar büyütülür | Kırmızı-kör |
| **Deuteranopi** | Yeşil → açık cyan, kırmızı doygunluk artırılır; şekil ipuçları aktif | Yeşil-kör |
| **Tritanopi** | Mavi → açık cyan, mor → pembe | Mavi-kör |

**Genel Erişilebilirlik**: Şekil/ikon ipuçları varsayılan aktif (kapatılamaz), HP çubuğu yanında % sayısı, element ikonu yanında harf kısaltması (A/S/T/H), nadirlik glow yanında yıldız sayısı.

---

### Renk Sistemi Tasarım Testleri

1. **5 Saniye Testi**: Yeni oyuncu altın="iyi", kırmızı="dikkat" bağını hemen kurabiliyor mu?
2. **Element Testi**: 4 element canavarı yan yana net ayrışıyor mu?
3. **Nadirlik Testi**: Legendary ödül gerçekten "vay canına" dedirtiyor mu?
4. **Biyom Testi**: Her biyom farklı hissedip "aynı oyun" gibi görünüyor mu?
5. **UI Okunabilirlik**: Kontrast oranı minimum 4.5:1 (WCAG AA)?
6. **Renk Körlüğü Simülasyonu**: Protanopi/deuteranopi'de tüm kritik bilgi ayırt edilebiliyor mu?
7. **Güneş Işığında Mobil**: Düşük parlaklıkta renk farkları anlaşılıyor mu?

---

## 5. Character Design Direction

### Temel Felsefe: "Karanlık Güç, Parlak Dünyada"

Canavarlar oyunun görsel yıldızlarıdır. Dünya sıcak, aydınlık ve cömert iken canavarlar karanlık, heybetli ve korkutucu görünür. Bu kontrast kasıtlıdır ve "Güç Hisset" pillar'ını besler: oyuncu bu karanlık, güçlü yaratıkları kontrol eden bir Canavar Lordu'dur.

**Ton**: Monster Hunter / Dark Souls estetiği. Heybetli savaşçılar — ne sevimli ne grotesk. Gerçekçi-abartılmış oranlar, chibi kesinlikle yasak.

```
[Sevimli Maskot] -------- [Heybetli Savaşçı] -------- [Korku Yaratığı]
      X                        ★ BURADAYIZ                  X
```

---

### 5.1 Canavar Görsel Arketipleri

#### Saldırgan (Striker)

**Poz**: 3/4 açı, gövde öne eğik, bir pençe/silah öne uzanmış — "hücuma hazır" anı. Kuyruk/kanat arkaya agresif yayılır.

**İfade**: Sert, odaklanmış bakış. Küçük keskin gözler. Ağız hafif aralık, dişler görünür. Kaşlar çatık. Gülümseyen/sevimli ifadeler **yasak**.

**Oranlar**: Savaş %20-25, koleksiyon %70-80, ikon: baş + öne uzanmış pençe.

**Örnekler**:
1. **Ateş Saldırganı**: Volkanik obsidyen zırh plakalarından magma sızan, uzun pençeli çevik yırtıcı. Sırtından alev izleri, gözler erimiş altın.
2. **Hava Saldırganı**: Mor-siyah şimşek desenli, jilet kanat uçlu kuş-sürüngen hibriti. Etrafında elektrik arkları.
3. **Su Saldırganı**: Bioluminesan mavi çizgili koyu lacivert zırhlı, tırtıklı yüzgeç-bıçaklı derin deniz avcısı.

#### Tank (Defender)

**Poz**: 3/4 açı, geniş omuzlar tamamen açık, ayaklar geniş basılmış — "hiçbir güç oynatamaz" duruşu. Baş öne eğik, boynuz/miğfer altından tehditkâr bakış.

**İfade**: Ağır, sakin tehdit. Küçük gözler, ağır kaş çıkıntıları. Çene sıkılmış. Şişman/komik oranlar **yasak**.

**Oranlar**: Savaş %22-28 (geniş ve kısa), koleksiyon yatay %85-90, ikon: masif omuz + boynuz.

**Örnekler**:
1. **Toprak Tankı**: Granit-obsidyen golem. Omuzlardan kristalize kaya çıkıntıları, göğüste yeşil enerji çatlakları. Yüzü savaş maskesi gibi oyulmuş.
2. **Ateş Tankı**: Erimiş metal zırhlı, boğa boynuzlu geniş canavar. Zırh eklerinden magma sızar.
3. **Su Tankı**: Mercan-deniz kabuğu zırhlı devasa kaplumbağa-ejder. Turkuaz bioluminesan desenler.

#### Destekçi (Support)

**Poz**: Dik, sakin, kontrollü — "güce hükmeden" duruş. Eller/kanatlar hafif açık, element enerjisi akışı. Sevimli değil, **ritüelistik** yuvarlak — antik tapınak muhafızı estetiği.

**İfade**: Bilge ama tehditkâr. Gözler element renginde ışık yayar veya tamamen beyaz/boş. Yüz mermer heykel soğukluğunda. Gülümseyen/şefkatli ifadeler **yasak**.

**Oranlar**: Savaş %20-25, koleksiyon %75-85 (yüzen efektler dahil), ikon: parlayan gözler + yuvarlak gövde + 1 yüzen öğe.

**Örnekler**:
1. **Hava Destekçisi**: Koyu mor kukuleta altında belirsiz form. Sadece beyaz parlayan gözler. Dönen rün halkaları.
2. **Su Destekçisi**: Mercan taçlı, derin turkuaz tenli deniz ruhu. Ellerden su akıntıları, buz mavisi gözler.
3. **Toprak Destekçisi**: Yosun-sarmaşık kaplı antik orman koruyucusu. Ağaç kabuğu-taş doku, yeşil enerji gözler.

#### Büyücü (Mage)

**Poz**: Uzun, dik, havada süzülen — ayaklar zar zor zemine değer veya tamamen havada. Bir el yukarı uzanmış element enerjisi topluyor. Kıyafet/parçalar yerçekimine meydan okur.

**İfade**: Soğuk, hesapçı bakış. Gözler element renginde yoğun ışık, göz bebekleri görünmez. Yüzün yarısı gölgede olabilir. Dostane büyücü estetiği **yasak**.

**Oranlar**: Savaş %25-30 (en uzun arketip), koleksiyon dikey tamamen doldurur, ikon: parlayan gözler + element topu + dar siluet.

**Örnekler**:
1. **Ateş Büyücüsü**: Kömürleşmiş kemik zırhlı havada süzülen iskelet-ejder. Kafatasından turuncu alev, yüzen lav küreleri.
2. **Hava Büyücüsü**: Fırtına bulutu formunda yarı-şeffaf varlık. Gövde dağılıp yeniden oluşur, mor şimşekler.
3. **Toprak Büyücüsü**: Yerden sökülen kaya parçalarından yüzen form. Göğüste parlayan yeşil kristal çekirdek.

---

### 5.2 Ayırt Edici Özellik Kuralları

#### "Görsel Parmak İzi" Sistemi (4 Katman)

| Katman | İletir | Okunabilirlik |
|--------|--------|---------------|
| **1. Siluet** | Arketip + genel form | 64x64 (ikon) |
| **2. Element Rengi** | Element aidiyeti | 64x64 (ikon) |
| **3. İmza Özellik** | Bireysel kimlik | 128px (savaş) |
| **4. Yüzey Detayı** | Nadirlik + karakter derinliği | 512px (koleksiyon) |

#### İmza Özellik Kuralı

Her canavar **tam 1 baskın imza özelliğe** sahiptir: benzersiz, silueti kıran, 128px'de okunabilir, hikaye taşıyan.

**Kategoriler**: Boynuz/Diş/Diken | Kanat/Yüzgeç/Pelerin | Kuyruk/Uzantı | Zırh/Kabuk/Doku | Yüzen Öğe | Anatomik Anomali

**Çarpışma Önleme**: Aynı kategori + benzer form zaten varsa farklı kategori seçilmeli.

#### Farklılaşma Matrisi

Aynı arketipteki canavarlar 3 eksenden en az 2'sinde farklılaşmalı:
- **Gövde Oranı**: Min %20 farklı
- **Siluet Kırıcı Yönü**: Farklı yön (yukarı/yana/aşağı/arkaya)
- **İmza Özellik Kategorisi**: Farklı kategori

#### Evrim Kuralı

Evrimde imza özellik korunur ama büyür/karmaşıklaşır. Siluet tanınabilirliği korunur. Renk doygunluğu ve glow artar.

---

### 5.3 İfade / Poz Stili

#### Karanlık Epik Referansları

**Yakın**: Monster Hunter (Rathalos, Nergigante), Dark Souls (Sif, Artorias), Shadow of the Colossus
**Uzak (Kaçınılacak)**: Pokemon (çok sevimli), Diablo (çok grotesk), Summoners War (çok anime)

#### İfade Yoğunluğu Kademesi

| Nadirlik | İfade | Göz | Genel Hava |
|----------|-------|-----|------------|
| **Yaygın** | Ölçülü tehdit | Normal, sert bakış | "Güçlü yırtıcı" |
| **Seyrek** | Belirgin tehdit | Keskin bakış, hafif glow | "Deneyimli avcı" |
| **Nadir** | Yoğun tehdit | Güçlü glow, daralmış göz | "Alfa yırtıcı" |
| **Epik** | Korkutucu heybet | Element ışıma, insandışı | "Doğaüstü güç" |
| **Efsanevi** | Mutlak hakimiyet | Tamamen ışıyan, enerji boşluğu | "Tanrısal varlık" |

#### Vitrin Pozu Kuralları

1. 3/4 ön-yan açı (30-45 derece). Tam profil/ön yasak.
2. Asimetrik ağırlık dağılımı.
3. Göz teması doğrudan kameraya.
4. Alan kullanımı %70-85.
5. Tank/Striker zemine sağlam, Mage havada, Support ikisi arasında.

**Yasak**: El sallama, göz kırpma, simetrik A/T-pose, sırtı dönük, aşırı dinamik aksiyon.

#### İdle Animasyon

"Nefes alan heykel" — yırtıcının dikkatli bekleme anı.

| Arketip | Hareket | Döngü |
|---------|---------|-------|
| **Striker** | Öne-arkaya salınım + pençe gerilimi | 2-3s |
| **Tank** | Çok yavaş nefes (göğüs genişleme) | 4-5s |
| **Support** | Yüzen öğelerin dönüşü + hafif yüzme | 3-4s |
| **Mage** | Yerçekimsiz süzülme + enerji akışı | 3-4s |

---

### 5.4 LOD Felsefesi

| Özellik | İkon (64x64) | Savaş (128px) | Koleksiyon (512px) |
|---------|-------------|---------------|---------------------|
| **Siluet** | Tam korunur | Tam korunur | Tam korunur |
| **Element Rengi** | Tam korunur | Tam korunur | Tam korunur |
| **İmza Özellik** | Sadeleştirilmiş | Tam okunabilir | Tam detaylı |
| **Yüz İfadesi** | Göz rengi/glow | Göz + genel ifade | Tam detay |
| **Yüzey Dokusu** | Düz renk | Temel desen | Tam detay |
| **Yüzen Öğeler** | 0-1 basit | 1-2 basit form | Tam sayı + efekt |
| **Glow/Parçacık** | Basit halo | Orta glow | Tam animasyonlu |

**AI Üretim LOD Stratejisi**: 512px ana kaynak üretilir → 128px otomatik küçültme + temizlik → 64x64 ağır sadeleştirme.

---

### 5.5 Boss Tasarım Felsefesi

#### Boss vs Normal Canavar

| Boyut | Normal | Boss | Oran |
|-------|--------|------|------|
| Ekran alanı | %20-30 | %50-70 | 2-3x |
| Siluet karmaşıklığı | 3-8 çıkıntı | 10-15+ | 2x+ |
| Yüzen öğe | 0-3 | 4-8 | 2-3x |
| Kontur | 2-3px | 4-5px | ~1.5x |
| Glow | Nadirliğe göre | Her zaman maksimum | Sabit yüksek |

#### Boss Görsel Drama

1. **Giriş**: Siluet önce karanlık form → element renginde dramatik aydınlanma. Parçalar kademeli ortaya çıkar.
2. **Aşama Geçişi** (HP <%50): Ek parçalar açılır, enerji yoğunlaşır — boss güçleniyor, zayıflamıyor.
3. **Yenilgi**: Element efektleri söner, parçalar dağılır → altın ışık patlaması. "Güçlü varlık yenildi" hissi korunur.

#### Mini-Boss vs Ana Boss

| | Mini-Boss | Ana Boss |
|---|---|---|
| Boyut | 1.5-2x normal | 2.5-3.5x normal |
| Giriş | 1-2 saniye | 3-5 saniye, dramatik |
| Aşamalar | 0-1 | 2-3 |
| Aydınlatma | Hafif dim (%20) | Tam dramatik |

---

### 5.6 AI Üretim Tutarlılık Kuralları

#### Zorunlu Prompt Şablonu

```
[STYLE ANCHOR]
Dark epic fantasy creature, semi-realistic style, detailed anatomy,
fierce expression, menacing presence, thick black outlines,
painted texture style, 3/4 view trophy pose facing camera,
dark atmospheric creature on transparent background

[ARCHETYPE]
{striker/tank/support/mage — arketip geometrisi}

[ELEMENT]
{fire/water/earth/air — element rengi ve efektleri}

[RARITY DETAIL LEVEL]
{common→legendary — detay kademesi}

[SIGNATURE FEATURE]
{Benzersiz imza özellik açık tanımı}

[NEGATIVE PROMPT]
cute, chibi, cartoon, kawaii, smiling, friendly, pastel colors,
flat shading, pixel art, anime, horror grotesque gore,
full action pose, back turned, symmetrical A-pose T-pose
```

#### Referans Sprite Matrisi

16 temel referans (4 arketip × 4 element): `ref_[archetype]_[element]_v[version].png`, 1024x1024px

#### 6 Maddelik Kalite Kapısı

| # | Kontrol | Kriter |
|---|---------|--------|
| **K1** | Siluet Okunabilirliği | Siyah siluette arketip+imza tanınır, 64x64'te ayırt edilir |
| **K2** | Element Renk Doğruluğu | Section 4.3 paletine uygun, ±15 hex tolerans |
| **K3** | Ton Tutarlılığı | Karanlık Epik spektrumda, referans sprite ile aynı stil |
| **K4** | Poz Kuralları | 3/4 açı, göz teması, arketip duruşu, yasak unsur yok |
| **K5** | Nadirlik-Detay Dengesi | Geometri bütçesine uygun karmaşıklık |
| **K6** | Teknik Kalite | Temiz kenarlar, tutarlı ışık (sol üst), saydam arka plan, min 512x512 |

#### Asset Adlandırma

```
char_monster_[element]_[archetype]_[name]_[rarity].[ext]
char_monster_fire_striker_infernalclaw_rare.png

LOD varyantları: _512 (koleksiyon), _128 (savaş), _64 (ikon)
Referanslar: ref_[archetype]_[element]_v[version].png
```

---

## 6. Environment Design Language

### 6.1 Mimari Stil ve Dünya Kültürü: "Kayıp Canavar Lordu Medeniyeti"

Zindanlar doğal oluşumlar değil — antik bir Canavar Lordu medeniyeti tarafından canavarları barındırmak, beslemek ve güçlendirmek için inşa edilmiş yapılar. Oyuncunun "burayı benim gibi biri inşa etmiş" hissi "Güç Hisset" pillar'ını destekler.

#### Mimari DNA

| Özellik | Görsel İpucu |
|---------|--------------|
| **Devasa Ölçek** | Kapılar 3-4x insan boyutu, geniş koridorlar. Kırık mobilyalar ölçek referansı |
| **Canavar Sembolleri** | Duvarlarda stilize canavar kabartmaları, pençe izleri, element sembolleri |
| **Enerji Kanalları** | Duvar/zeminde akan element enerjisi damarları — yapılar "yaşıyor" |
| **Katmanlı Tarih** | Üst katlar bakımlı, derin katlar yıpranmış ama daha görkemli |
| **Fonksiyonel Alanlar** | Arena=yuvarlak, hazine=dar giriş + geniş iç, salon=sütunlu uzun |

#### Biyom-Medeniyet İlişkisi

| Biyom | Medeniyet Fonksiyonu | Mimari Karakter |
|-------|---------------------|-----------------|
| **Kristal Mağaraları** | Enerji toplama tesisi | Doğal mağara + işlenmiş kristal kanallar |
| **Lavlı Derinlikler** | Canavar güçlendirme ocakları | Endüstriyel devasa yapılar, kanal sistemleri |
| **Batık Tapınak** | Bilgi/ritüel merkezi, canavar evrim tapınağı | Klasik tapınak — sütunlar, kemerler, havuzlar |
| **Gökyüzü Kalesi** | Canavar Lordu'nun taht kalesi | Görkemli kale — kuleler, köprüler, taht odası |

**Stil**: "İşlenmiş Antik" — ne tamamen harap ne yeni. Terk edilmiş ama hala işleyen mekanizmalar.

**Yasak**: Tamamen yıkılmış yapılar, steril yeni yapılar, modern/endüstriyel, gotik korku mimarisi.

---

### 6.2 Doku Felsefesi: "Stilize Boyama"

El boyası hissi, yumuşak fırça darbeleri, detaylı ama sert kenarlardan kaçınan stilize boyama.

| Neden Bu Stil? | Açıklama |
|----------------|----------|
| Canavarlarla Kontrast | Yumuşak boyalı ortam vs sert konturlu Karanlık Epik canavarlar = net figür-zemin ayrımı |
| AI Tutarlılığı | Boyama stili daha geniş tolerans, PBR stil sapmasına çok açık |
| Mobil Performans | Düşük çözünürlükte okunabilir, çoklu doku haritaları gereksiz |
| Sıcak Atmosfer | Boyama stili doğası gereği sıcak ve davetkar |

#### Doku Kuralları

- **Fırça Darbesi**: Yakında yumuşak fırça darbeleri görünmeli. Tam düz veya fotogerçekçi yasak.
- **Renk Bloklama**: Yüzey başına max 3-4 renk tonu (ana, gölge, vurgu, aksan).
- **Kenar İşleme**: Ortam = yumuşak blend kenarlar. Sert kenarlar sadece interaktif ön plan öğelerinde.
- **Işık**: Dokuya baked-in fırça darbeleriyle boyanan ışık noktaları.
- **Detay Mesafe**: Arka plan = büyük renk blokları, orta plan = doku görünür, ön plan = yüzey okunabilir.

#### Malzeme Dili

| Malzeme | Doku Karakteri | Renk | Biyom |
|---------|---------------|------|-------|
| **Taş/Kaya** | Geniş fırça, facetli, çatlak | Sıcak gri-kahve (soğuk gri yasak) | Tümü |
| **Kristal** | Düz-parlak, iç glow, yarı-saydam | Element rengi, doygun | Kristal |
| **Metal** | Yıpranmış, patinalı, çizilmiş parlak noktalar | Koyu bronz-bakır (krom yasak) | Lav |
| **Su/Sıvı** | Akışkan fırça, S-kıvrımları, yarı-saydam | Turkuaz-mavi gradient | Batık |
| **Bulut/Gök** | Çok yumuşak, blur, büyük geçişler | Açık mavi → beyaz → hafif mor | Gökyüzü |
| **Lav/Erimiş** | Akışkan, parlak çekirdek → kararan kenar | Sarı → turuncu → kırmızı → siyah | Lav |

---

### 6.3 Obje Yoğunluğu Kuralları

#### Alan Tipi Bazlı

| Alan | Yoğunluk | Max Prop | Animasyonlu Limit |
|------|----------|----------|-------------------|
| **Koridor** | Düşük-Orta | 4-6 | 1-2 |
| **Savaş Alanı** | Düşük | 2-4 (kenarlar, merkez temiz) | 1 |
| **Boss Odası** | Orta kenar + boş merkez | 6-8 | 2-3 |
| **Loot Odası** | Orta-Yüksek | 8-12 | 3-4 |
| **Gizli Oda** | Yüksek | 10-15 | 4-5 |

#### Performans Kuralları

- Ekranda max 15-20 obje (proplar + canavarlar + UI), draw call <100
- Animasyonlu prop max 5, parçacık sistemi max 3
- Biyom başına max 2 doku atlas'ı (2048x2048) + 1 ortak atlas (1024x1024)
- Aynı prop max 3 kez aynı ekranda (farklı varyasyonla)

---

### 6.4 Çevresel Hikaye Anlatımı

#### 3 Görsel Kanal

**Kanal 1 — Lore**: Canavar kabartmaları, element sembolleri, aktif enerji kanalları, devasa ölçek kapılar, katmanlı inşaat izleri

**Kanal 2 — Tehlike**: Pençe izleri (hafif çizik → derin oyuk), element hasarı (yanık/don/çatlak/aşınma), boss yaklaştıkça %60'a kısılan ışık, canavar yuvaları

**Kanal 3 — Ödül**: Altın çatlaklar, kristal yoğunlaşması, parlayan enerji kanalları, ışık yoğunlaşması

**Ödül Gradyanı**: Koridor seyrek → loot odasına yaklaştıkça yoğunlaşır → loot odasında patlama noktası.

#### Kat Hikaye Akışı

```
Giriş koridoru → İlk savaş (tehlike başlar) → Orta koridorlar (tehlike + ödül ipuçları)
→ Mini-boss (tehlike doruk) → Loot odası (ödül patlama) → Boss koridoru (dramatik karartma)
→ Boss odası (mimari doruk noktası)
```

---

### 6.5 Biyom Bazlı Görsel Kimlik

#### Kristal Mağaraları

- **Sıcaklık**: 4000-4500K (sıcak amber)
- **Malzeme**: Sıcak gri-kahve kaya + amber-altın kristaller + turkuaz kristal damarları
- **İmza Proplar**: Büyük kristal kümesi, kristal enerji kanalı, kristal meşale, altıgen rün taşı, parlayan mantar
- **Atmosfer**: Yüzen kristal parçacıkları, kristal ışık haleleri, sıcak toz, ince amber sis
- **Derinlik**: Üst katlar geniş/aydınlık → derin katlar devasa kristal odaları, en parlak renkler

#### Lavlı Derinlikler

- **Sıcaklık**: 3000-3500K (en sıcak biyom)
- **Malzeme**: Koyu bazalt-obsidyen + erimiş lav (sarı→turuncu→kırmızı→siyah) + koyu bronz metal
- **İmza Proplar**: Lav akıntısı, devasa antik örs, sarkan zincirler, çatlak obsidyen sütun, buhar deliği
- **Atmosfer**: Kor parçacıkları, lav ışık yansımaları, hafif ısı distorsiyonu
- **Özel Kural**: Lav her zaman parlak — minimum parlaklık %50 (diğer biyomlardan %10 fazla)

#### Batık Tapınak

- **Sıcaklık**: 4500-5000K (nötr-sıcak)
- **Malzeme**: Kumtaşı-bej (su yıpranmış) + turkuaz su + altın-bronz süsleme + mercan
- **İmza Proplar**: Yarı batık süslü sütun, turkuaz ritüel havuzu, antik canavar heykeli, duvar yazıtları, iç şelale
- **Atmosfer**: Caustic ışık desenleri (tavanda dans eden turkuaz çizgiler), yüzen su damlaları, su dalgalanması
- **Derinlik**: Üst katlar az su → derin katlar neredeyse tamamen su altı, dev ritüel salonu

#### Gökyüzü Kalesi

- **Sıcaklık**: 5500-6500K (en soğuk — altın aksanlarla dengelenir)
- **Malzeme**: Beyaz-açık gri mermer + açık gök mavisi + parlak altın süslemeler
- **İmza Proplar**: Yüzen mermer köprü, antik altın taht, yırtık lord sancağı, uzak kule, bulut platformu, dev gökyüzü penceresi
- **Atmosfer**: Yatay akan bulutlar, güneş ışığı huzmeleri, yüzen altın parçacıklar, uzak bulut denizi
- **Özel Kural**: "Kale" hissi — gökyüzü görünür, kapalı alanlar bile pencerelerle açık havaya bağlı

#### Biyom Geçiş Kuralları

Ani renk sıcaklığı atlaması yasak. Her geçişte 2-3 saniye kademeli renk/malzeme köprüsü. İki biyomun aksan renkleri %50/%50 blend.

---

### 6.6 Parallax ve Katman Sistemi (6 Katman)

| Katman | İsim | Scroll | İçerik | Detay | Doygunluk |
|--------|------|--------|--------|-------|-----------|
| **L0** | Uzak Arka Plan | %10 | Biyom panoraması | Minimal, blur | %30-40 |
| **L1** | Orta Arka Plan | %25 | İkincil mimari siluetleri | Düşük | %40-50 |
| **L2** | Yakın Arka Plan | %50 | Birincil duvarlar, büyük proplar | Orta, fırça darbeli | %50-60 |
| **L3** | Oyun Alanı | %100 | Zemin, canavarlar, interaktif proplar | Yüksek, konturlu | %70-80 |
| **L4** | Ön Plan Detay | %120 | Sarkıtlar, bitkiler, çerçeve | Orta, yarı-saydam | %50-60 |
| **L5** | Atmosferik Overlay | %140 | Parçacıklar, sis, ışık huzmeleri | Minimal, alpha | %20-30 |

#### Derinlik Teknikleri

Hız farkı + doygunluk gradyanı + detay gradyanı + kontur gradyanı + renk perspektifi + boyut ölçeği

#### Performans

- L0-L2 opak sprite, single pass. L4-L5 alpha-blended, max toplam alpha kaplama %100
- L3: max 10-15 sprite. L4: max 2-4. L5: max 1-2 overlay
- Parçacık max 3 aktif sistem
- Unity URP: her katman ayrı SortingLayer

#### Asset Adlandırma

```
env_[biome]_[object]_[descriptor]_[size].[ext]
Biyomlar: crystal, lava, temple, sky, common
Parallax: env_[biome]_bg_L[0-5]_[segment].[ext]
Tile: env_[biome]_tile_[type]_[variant].[ext]
```

---

## 7. UI/HUD Visual Direction

### 7.1 Ekran-Uzay HUD: "Temiz Çerçeve, Dolu Sahne"

Tamamen screen-space HUD — diegetik UI yok. 2D sanat stili, mobil okunabilirlik, idle/otofarm uyumu ve "Cömert Zindan" pillar'ı (ödülü saklamak yasak) bunu gerektirir.

```
┌─────────────────────────────────────────┐
│  ◄ Üst Çubuk: Durum Bilgisi (Safe Area) │
│─────────────────────────────────────────│
│          SAHNE / SAVAŞ ALANI            │
│            (dokunulmaz alan)             │
│─────────────────────────────────────────│
│  ▼ Alt Çubuk: Eylem Alanı (Safe Area)   │
└─────────────────────────────────────────┘
```

**Safe Area**: iOS notch ve Android punch-hole için üst/alt 44dp + yan 16dp.

| Durum | HUD Yoğunluğu |
|-------|----------------|
| **Hub** | Orta — kaynaklar, nav butonlar, bildirimler, lead canavar |
| **Zindan Keşfi** | Düşük — kat göstergesi, takım sağlığı, loot sayacı |
| **Normal Savaş** | Orta — HP, yetenekler, dalga sayacı, loot, otofarm toggle |
| **Boss Savaşı** | Yüksek — boss HP, takım HP, yetenekler, aşama göstergesi |
| **Loot/Ödül** | Minimal — sadece loot gösterimi + devam butonu |

---

### 7.2 Tipografi

#### Font Katmanları

| Katman | Karakter | Font Tipi | Kullanım |
|--------|----------|-----------|----------|
| **Display** | Kalın, geniş, hafif keskin serif/slab | Cinzel Bold benzeri | Ekran başlıkları, boss isimleri, büyük rakamlar |
| **Body** | Temiz geometrik sans-serif, geniş x-yüksekliği | Inter/Nunito Sans benzeri | UI metinleri, açıklamalar, istatistikler |
| **Data** | Tabular lining, sabit genişlik | Inter Tabular benzeri | HP sayıları, hasar, zamanlayıcılar |

**Türkçe Zorunlu**: Tüm fontlarda ş, ç, ğ, ı, ö, ü, İ tam desteği kontrol edilmeli.

#### Boyut Hiyerarşisi

| Kademe | Boyut (sp) | Ağırlık | Kullanım |
|--------|------------|---------|----------|
| **H1** | 28-32 | Bold/Black | Ekran isimleri |
| **H2** | 22-24 | Bold | Panel başlıkları |
| **H3** | 18-20 | SemiBold | Canavar/eşya isimleri |
| **Body** | 15-16 | Regular | Açıklamalar, tooltiplar |
| **Caption** | 12-13 | Regular | İkincil bilgi |
| **Micro** | 10-11 | Medium | Rozet sayıları (minimum) |

**10sp altı metin yasak.**

#### Kontrast Tablosu

| Metin | Renk | Zemin | Kontrast |
|-------|------|-------|----------|
| Birincil | `#F7FAFC` | `#1A1A3E` | 14.3:1 (AAA) |
| İkincil | `#A0AEC0` | `#1A1A3E` | 6.8:1 (AA) |
| Vurgu | `#FFD700` | `#1A1A3E` | 10.2:1 (AAA) |
| Buton | `#1A1A3E` | `#FFD700` | 10.2:1 (AAA) |

**HUD Metinlerinde**: 1px koyu drop shadow (`#000000` alpha %60) — sahne üzerinde okunabilirlik garanti.

#### Savaş Sayıları

| Tip | Boyut | Renk | Animasyon |
|-----|-------|------|-----------|
| Normal Hasar | 24sp | `#F7FAFC` | Yukarı süzülme + fade (0.8s) |
| Kritik Hasar | 32sp | `#FFD700` | Büyüyerek çıkma + titreme (1.2s) |
| İyileşme | 22sp | `#48BB78` | Yumuşak süzülme (0.6s) |
| Loot | 20sp | Nadirlik rengi | Yandan kayma + pulse (1.0s) |

---

### 7.3 İkonografi: "Dolgulu Yumuşak Kontur"

Dolgulu form + 1.5-2px yumuşak kontur + hafif iç gölge. Küçükte okunabilir, büyükte detaylı, boyama stiliyle uyumlu.

#### İkon Grid

| Kullanım | Grid | Dokunma Alanı |
|----------|------|---------------|
| Navigasyon | 24x24dp | 44x44dp |
| Eylem (Savaş) | 32x32dp | 56x56dp |
| Durum (HUD) | 20x20dp | — (bilgi) |
| Eşya/Canavar | 48x48dp | 56x56dp |

#### Element İkonları

| Element | Form | Renk | Kısaltma |
|---------|------|------|----------|
| Ateş | Stilize alev (3 dilli) | `#E8530A` | **A** |
| Su | Damla + dalga | `#2B6CB0` | **S** |
| Toprak | Yaprak + kristal | `#38A169` | **T** |
| Hava | Spiral rüzgar | `#805AD5` | **H** |

#### Nadirlik İkonları

Yaygın=ikon yok, Seyrek=1 yeşil yıldız, Nadir=2 mavi yıldız, Epik=3 mor yıldız, Efsanevi=kanatlı taç

#### Navigasyon İkonları

Zindan=kale kapısı, Takım=3 canavar silueti, Koleksiyon=kitap, Arena=çapraz kılıçlar, Mağaza=sandık. Aktif=altın dolgu + 3dp altın nokta, Pasif=kontur gri.

---

### 7.4 UI Animasyon: "Akışkan ve Cömert"

**Easing**: Tüm UI'da ease-out (cubic-bezier 0.25, 0.1, 0.25, 1.0). Bounce sadece ödül/kutlama.

#### Ekran Geçişleri

| Geçiş | Süre | Animasyon |
|--------|------|-----------|
| İleri nav | 300ms | Sağdan kayma |
| Geri nav | 250ms | Soldan kayma |
| Modal açma | 250ms | Scale 0→1 + dim |
| Modal kapama | 200ms | Scale 1→0.95 + fade |
| Sahne değişikliği | 800ms | Fade to black → fade in |

**Max 800ms — oyuncu 1s'den fazla bekletilmez.**

#### Buton Geri Bildirimi

- Dokunma: Scale 0.96 + parlaklık +%15 (anında)
- Bırakma: Scale 1.0 + altın ripple (150ms)
- CTA idle: Hafif altın glow pulse (2s döngü)

#### Loot Reveal Sekansı (3 Aşama)

1. **Beklenti** (500ms): Sandık titreme + içeri çekilen parçacıklar
2. **Patlama** (300ms): Sandık açılır + altın ışık patlaması
3. **Reveal** (400ms/eşya): Teker teker ortaya çıkma + nadirlik kademeli kutlama

| Nadirlik | Reveal Efekti |
|----------|---------------|
| Yaygın | Düz scale-up |
| Seyrek | + yeşil glow |
| Nadir | + mavi halkası + ekran tint (200ms gecikme) |
| Epik | + mor spiral + kamera sarsıntısı + slow-mo (400ms gecikme) |
| Efsanevi | + tam ekran altın patlama + full slow-mo + silüet sunumu (600ms gecikme) |

Sonunda toplam kazanım sayıları "slot makinesi" animasyonuyla yukarı sayar.

---

### 7.5 Savaş HUD Düzeni

```
┌─────────────────────────────────────────────────┐
│ [Dalga 3/5] [Kat 7]              [Loot 142] [E12]│
│─────────────────────────────────────────────────│
│                Boss HP Bar                       │
│                                                 │
│   [Düşman 1]  [Düşman 2]  [Düşman 3]            │
│                                                 │
│   [Canavar A]  [Canavar B]  [Canavar C] [Can D]  │
│─────────────────────────────────────────────────│
│ [Y1] [Y2] [Y3] [Y4]          [Hız x2] [OTO]    │
└─────────────────────────────────────────────────┘
```

#### Komutan vs Otofarm Modu

| Element | Komutan | Otofarm |
|---------|---------|---------|
| Yetenek Butonları | Aktif, dokunulabilir | Gri/gizli |
| Güç Bonus Rozeti | "Komutan +%25" altın | Gizli |
| Otomatik Rozeti | Gizli | "OTOMATİK" yeşil pulse |
| Loot Akışı | Bireysel her düşüş | Birikimli 10s'de toplam |

**Mod geçişi**: 300ms kayma animasyonu.

**Minimal Mod**: Oyuncu tercihi. Detaylı=tümü, Minimal=HP + yetenekler + dalga sayacı. Boss savaşında otomatik minimal.

---

### 7.6 UX Uyumluluk Kontrolü

| Kontrol | Risk | Çözüm |
|---------|------|-------|
| Koyu UI + parlak sahne | DÜŞÜK | Gradient fade kenarlar + biyom bazlı saydamlık (%85-90) |
| Altın CTA görünürlüğü | DÜŞÜK | CTA her zaman koyu panel üzerinde + 2px koyu kontur + dış glow |
| 320dp ekran sıkışma | ORTA | Yetenek butonları 48dp'ye küçülür. Prototipte test gerekli |
| Renk körlüğü | DÜŞÜK | Section 4.7 çözümleri + element harf kısaltmaları + toggle metin etiketi |
| Güneş ışığında okunabilirlik | DÜŞÜK | WCAG AA+ kontrastlar, isteğe bağlı Yüksek Kontrast Modu |
| Bildirim aşırı yüklenmesi | ORTA | Max 3 rozet görünür, öncelik sıralaması, "3+" birleştirme |

---

## 8. Asset Standards

### 8.1 Dosya Formatları ve Sıkıştırma

#### Sprite / Texture Formatları

| Varlık Tipi | Kaynak Format | Dışa Aktarım Formatı | Renk Profili | Not |
|-------------|---------------|-----------------------|--------------|-----|
| Canavar sprite | PNG-32 (RGBA) | PNG-32 | sRGB | Saydam arka plan zorunlu |
| Çevre parallax (L0-L2) | PNG-24 (RGB) | PNG-24 | sRGB | Opak katmanlar, alpha gereksiz |
| Çevre parallax (L4-L5) | PNG-32 (RGBA) | PNG-32 | sRGB | Alpha-blended overlay katmanları |
| Çevre prop / tile | PNG-32 (RGBA) | PNG-32 | sRGB | Kenar saydamlığı için alpha kanalı |
| UI panel / buton | PNG-32 (RGBA) | PNG-32 | sRGB | 9-slice uyumlu kesilmeli |
| UI ikon | SVG → PNG-32 | PNG-32 | sRGB | Kaynak SVG saklanır, dışa aktarım PNG |
| VFX sprite sheet | PNG-32 (RGBA) | PNG-32 | sRGB | Pre-multiplied alpha |
| Nadirlik glow / efekt | PNG-32 (RGBA) | PNG-32 | sRGB | Additive blending için ayrı katman |

**Kaynak Arşivleme**: AI üretimi orijinal dosyalar `assets/art/source/` altında saklanır. Asla silinmez. Dışa aktarılmış (crop/resize/cleanup) dosyalar `assets/art/sprites/` altında kullanılır.

#### Ses Formatları

| Ses Tipi | Kaynak Format | Dışa Aktarım Formatı | Sample Rate | Bit Depth | Not |
|----------|---------------|-----------------------|-------------|-----------|-----|
| Müzik (BGM) | WAV / FLAC | OGG Vorbis | 44100 Hz | 16-bit | Kalite: 70% (mobil optimum) |
| Kısa SFX (<2s) | WAV | WAV (uncompressed) | 44100 Hz | 16-bit | Unity Load Type: Decompress On Load |
| Uzun SFX (>2s) | WAV | OGG Vorbis | 44100 Hz | 16-bit | Unity Load Type: Streaming |
| UI ses | WAV | WAV (uncompressed) | 22050 Hz | 16-bit | Düşük gecikme kritik |

#### Platform Bazlı Texture Sıkıştırma (Unity Import Settings)

| Platform | Sıkıştırma Formatı | Kalite | Neden |
|----------|---------------------|--------|-------|
| **iOS** | ASTC 6x6 (varsayılan) | High | Apple GPU'lar için en verimli; 6x6 kalite/boyut dengesi |
| **iOS (UI metin/ikon)** | ASTC 4x4 | High | Keskin kenarlar korunmalı |
| **iOS (arka plan L0-L1)** | ASTC 8x8 | Normal | Uzak katmanlar, detay kaybı kabul edilir |
| **Android** | ETC2 RGBA (varsayılan) | High | OpenGL ES 3.0+ tüm hedef cihazlarda destekli |
| **Android (UI metin/ikon)** | ETC2 RGBA | High | Keskin kenarlar |
| **Android (arka plan L0-L1)** | ETC2 RGB (alpha yok) | Normal | Opak katmanlarda alpha gereksiz |
| **Editor** | Sıkıştırma yok | — | Geliştirme hızı için |

**ASTC vs ETC2 Tradeoff Notu**: ASTC 6x6, ETC2'den ~%30 daha küçük dosya boyutu ve daha iyi kalite sunar. Ancak ETC2, Android'de evrensel destek garantisi verir. Hedef Android cihazlar ASTC destekliyorsa (Vulkan + Android 9+), Android'de de ASTC 6x6 tercih edilir — Unity Platform Override ile her iki profil ayarlanmalı.

---

### 8.2 Çözünürlük Kademeleri

#### Canavar Sprite Çözünürlükleri

| LOD Kademesi | Çözünürlük | Kullanım | Detay Seviyesi | Bellek (RGBA) |
|--------------|------------|----------|----------------|---------------|
| **Koleksiyon** | 512x512 px | Canavar vitrin ekranı, evrim, detaylı inceleme | Tam detay — yüzey doku, yüzen öğe, glow | ~1 MB (sıkıştırılmamış) |
| **Savaş** | 128x128 px | Savaş sahnesinde aktif canavar | İmza özellik + element renk + temel form | ~64 KB |
| **İkon** | 64x64 px | Takım listesi, envanter, küçük referans | Siluet + element renk, sadeleştirilmiş | ~16 KB |

**Üretim Akışı**: AI 1024x1024+ çıktı üretir → sanatçı cleanup → 512px master kaydedilir → 128px ve 64px otomatik downscale + manuel sadeleştirme.

**Sprite Sheet Boyutları (Animasyonlu)**:

| LOD | Frame Sayısı (idle) | Sheet Boyutu | Format |
|-----|---------------------|--------------|--------|
| Koleksiyon 512px | 4-6 frame | 2048x512 (4 frame) veya 3072x512 (6 frame) | Horizontal strip |
| Savaş 128px | 4-8 frame | 1024x128 (8 frame) veya 512x256 (4x2 grid) | Grid veya strip |
| İkon 64px | 1 frame (statik) | 64x64 | Tek sprite |

#### Çevre Çözünürlükleri

| Varlık Tipi | Çözünürlük | Not |
|-------------|------------|-----|
| Parallax L0 (uzak) | 1024x512 px | Minimum detay, büyük renk blokları |
| Parallax L1 | 1536x768 px | Düşük detay siluetler |
| Parallax L2 | 2048x1024 px | Orta detay, fırça darbesi görünür |
| Parallax L3 (oyun alanı) | Tile-based: 128x128 px/tile | Yüksek detay, konturlu |
| Parallax L4 (ön plan) | 512x256 px / parça | Yarı-saydam dekoratif |
| Parallax L5 (overlay) | 512x512 px | Minimal alpha efekt |
| Prop (küçük) | 64x64 — 128x128 px | Meşale, mantar, küçük kristal |
| Prop (orta) | 128x128 — 256x256 px | Sütun, heykelt, büyük kristal |
| Prop (büyük) | 256x256 — 512x512 px | Kapı, büyük yapı parçası |

#### UI Çözünürlükleri

| Varlık Tipi | Tasarım Çözünürlüğü | Dışa Aktarım | 9-Slice |
|-------------|----------------------|--------------|---------|
| Panel arka plan | @2x referans (750x1334) | @1x, @2x, @3x | Evet |
| Buton | 200x64 dp (@2x = 400x128 px) | @1x, @2x, @3x | Evet |
| İkon (nav) | 24x24 dp (@2x = 48x48 px) | @1x, @2x, @3x | Hayır |
| İkon (eylem) | 32x32 dp (@2x = 64x64 px) | @1x, @2x, @3x | Hayır |
| İkon (eşya/canavar) | 48x48 dp (@2x = 96x96 px) | @1x, @2x, @3x | Hayır |
| Nadirlik çerçeve | 56x56 dp (@2x = 112x112 px) | @1x, @2x, @3x | Evet |

**@1x/@2x/@3x Stratejisi**: Unity'de tek yüksek çözünürlüklü (@3x) varlık kullanılır, Unity MaxTextureSize ayarıyla platforma göre otomatik küçültülür. Ayrı dosya dışa aktarımı gerekmez.

**Mobil Optimizasyon Kademesi**: Düşük-uçlu cihazlar için `QualitySettings` bazlı texture resolution halving — canavar 512→256, çevre 2048→1024.

#### VFX Çözünürlükleri

| VFX Tipi | Çözünürlük | Frame | Sheet |
|----------|------------|-------|-------|
| Element patlama | 128x128 / frame | 8-12 | 1024x256 (strip) |
| Loot parıltısı | 64x64 / frame | 6-8 | 512x64 (strip) |
| Nadirlik glow | 256x256 | 1 (statik, alpha) | Tek sprite |
| Hasar sayısı efekti | 64x64 | 4-6 | 256x64 (strip) |
| Boss giriş efekti | 256x256 / frame | 12-16 | 1024x1024 (4x4 grid) |

---

### 8.3 Sprite Spesifikasyonları

#### Canavar Sprite Sheet Formatı

**Grid vs Strip**: Horizontal strip tercih edilir (tek satır). 6+ frame'de 2 satırlı grid'e geçilebilir.

**Pivot Noktaları**:

| Arketip | Pivot X | Pivot Y | Neden |
|---------|---------|---------|-------|
| Striker | 0.5 | 0.15 | Ayak seviyesi, öne eğik poz |
| Tank | 0.5 | 0.1 | Geniş taban, ayak seviyesi |
| Support | 0.5 | 0.2 | Hafif havada, taban üstü |
| Mage | 0.5 | 0.25 | Süzülen poz, ayaklardan uzak |
| Boss | 0.5 | 0.1 | Devasa boyut, zemin referansı |

**Pivot (0,0) = sol alt köşe, (1,1) = sağ üst köşe.**

#### Animasyon Frame Bütçeleri

| Animasyon | Striker | Tank | Support | Mage | Boss |
|-----------|---------|------|---------|------|------|
| **Idle** | 4 frame | 4 frame | 4 frame | 4 frame | 6 frame |
| **Saldırı** | 6 frame | 4 frame | 4 frame | 6 frame | 8 frame |
| **Yetenek** | 6 frame | 6 frame | 6 frame | 8 frame | 10 frame |
| **Hasar alma** | 2 frame | 2 frame | 2 frame | 2 frame | 4 frame |
| **Ölüm** | 4 frame | 4 frame | 4 frame | 4 frame | 8 frame |
| **Toplam** | **22** | **20** | **20** | **24** | **36** |

**FPS**: Idle=8 fps, Saldırı/Yetenek=12 fps, Hasar=10 fps, Ölüm=8 fps.

**İdeal vs Kısıtlı**: Yukarıdaki bütçe tam vizyon hedefidir. MVP'de sadece **idle (4f) + saldırı (4f) + hasar (2f) = 10 frame** yeterlidir. Yetenek ve ölüm animasyonları Tier 2'de eklenir.

#### Unity Import Ayarları (Canavar Sprite)

```
Texture Type:           Sprite (2D and UI)
Sprite Mode:            Multiple (sprite sheet için)
Pixels Per Unit:        100  (128px canavar ≈ 1.28 Unity unit)
Mesh Type:              Tight (draw call azaltma)
Extrude Edges:          1 (atlas bleeding önleme)
Filter Mode:            Bilinear (stilize boyama için yeterli)
Max Texture Size:       2048 (sheet boyutuna göre)
Compression:            Platform Override (bkz. 8.1)
Generate Mip Maps:      HAYIR (2D oyun, mipmap gereksiz)
Read/Write Enabled:     HAYIR (bellek tasarrufu)
sRGB (Color Texture):   EVET
Alpha Source:            Input Texture Alpha
Alpha Is Transparency:  EVET
```

**Neden Mipmap Kapalı?**: 2D oyunda kamera yakınlaştırma/uzaklaştırma yok. Tüm sprite'lar sabit boyutta render edilir. Mipmap açmak bellek kullanımını %33 artırır — 1GB bütçede kabul edilemez.

---

### 8.4 Atlas Stratejisi

#### Atlas Boyut Limitleri

| Atlas Tipi | Max Boyut | Sıkıştırılmış (ASTC 6x6) | Sıkıştırılmış (ETC2) |
|------------|-----------|---------------------------|----------------------|
| Biyom atlas | 2048x2048 | ~2.7 MB | ~4 MB |
| Ortak atlas | 1024x1024 | ~0.7 MB | ~1 MB |
| UI atlas | 2048x2048 | ~2.7 MB | ~4 MB |
| VFX atlas | 1024x1024 | ~0.7 MB | ~1 MB |
| Canavar atlas (per-rarity) | 2048x2048 | ~2.7 MB | ~4 MB |

#### Biyom Bazlı Atlas Organizasyonu

Her biyom için **2 atlas + 1 paylaşılan ortak atlas**:

| Atlas | İçerik | Boyut | Yükleme |
|-------|--------|-------|---------|
| `atlas_env_[biome]_props` | Prop sprite'ları, tile'lar, interaktif objeler | 2048x2048 | Biyom girişinde |
| `atlas_env_[biome]_bg` | Parallax katmanları (L0-L2, L4-L5), büyük arka plan parçaları | 2048x2048 | Biyom girişinde |
| `atlas_env_common` | Tüm biyomlarda ortak proplar (meşale, sandık, kapı, merdiven) | 1024x1024 | Uygulama başlangıcında |

**4 biyom toplam çevre atlas belleği**: (4 biyom × 2 atlas × ~3 MB) + ~1 MB ortak = **~25 MB** (sadece aktif biyom yüklü = ~7 MB)

#### Canavar Atlas Organizasyonu

Canavarlar nadirlik bazlı gruplanır (aynı nadirlik = benzer detay seviyesi = verimli packing):

| Atlas | İçerik | Boyut | Not |
|-------|--------|-------|-----|
| `atlas_char_battle_common` | Tüm Common + Uncommon savaş sprite'ları (128px) | 2048x2048 | En sık yüklenen |
| `atlas_char_battle_rare` | Tüm Rare + Epic savaş sprite'ları (128px) | 2048x2048 | İhtiyaçta yüklenir |
| `atlas_char_battle_legendary` | Legendary savaş sprite'ları (128px) | 1024x1024 | İhtiyaçta yüklenir |
| `atlas_char_icon` | Tüm canavar ikonları (64px) | 1024x1024 | Uygulama başlangıcında |
| `atlas_char_collection_[page]` | Koleksiyon sprite'ları (512px), 8-12 canavar / atlas | 2048x2048 | Ekran açıldığında |

**Koleksiyon 512px sprite'lar atlas'a KONULMAZ**: Tek tek Addressable olarak yüklenir/boşaltılır. 512px sprite'lar çok büyük — atlas'a koymak 2048x2048 atlas'a sadece 16 sprite sığdırır ve bellek israfı yaratır.

**Düzeltme — İdeal vs Kısıtlı**: İdeal durumda her canavar koleksiyon sprite'ı ayrı Addressable olur. Kısıtlı durumda (Addressable sistemi henüz kurulmadıysa), element bazlı 2048x2048 atlas'lara 16'şar sprite konulabilir — ama bu geçici çözümdür.

#### UI Atlas Organizasyonu

| Atlas | İçerik | Boyut |
|-------|--------|-------|
| `atlas_ui_common` | Paneller, butonlar, çerçeveler, slider'lar, 9-slice parçaları | 2048x2048 |
| `atlas_ui_icons` | Navigasyon, element, nadirlik, durum ikonları | 1024x1024 |
| `atlas_ui_rarity` | Nadirlik çerçeveleri, glow overlay'leri, efekt sprite'ları | 1024x1024 |

#### Unity Sprite Atlas Ayarları

```
Type:                   Master (Variant atlas kullanılmayacak)
Include in Build:       EVET (ortak ve savaş atlas'ları)
Allow Rotation:         HAYIR (sprite yönü korunmalı)
Tight Packing:          EVET (boşluk azaltma)
Alpha Dilation:         EVET (kenar bleeding önleme)
Padding:                4 px (ASTC sıkıştırma blok sınırı için 2 yeterli, güvenlik marjı 4)
Read/Write Enabled:     HAYIR
Generate Mip Maps:      HAYIR
Filter Mode:            Bilinear
Max Texture Size:       2048
```

**Atlas Doluluk Hedefi**: Her atlas en az %75 doluluk oranına sahip olmalı. %75'in altındaysa küçük atlas boyutuna geçilir veya diğer atlas ile birleştirilir. Unity'nin Sprite Atlas Analyzer aracı ile doğrulanır.

**Late Binding (Addressable Atlas)**: Biyom atlas'ları ve koleksiyon sprite'ları Addressable ile late-bind edilir — yalnızca ilgili sahne/ekran açıldığında belleğe yüklenir, kapatıldığında serbest bırakılır.

---

### 8.5 Adlandırma Konvansiyonları

#### Genel Format

```
[kategori]_[alt-kategori]_[isim]_[varyant]_[boyut].[ext]
```

Tüm isimler: **küçük harf**, **İngilizce**, **alt çizgi ayırıcı**, **boşluk ve Türkçe karakter yok**.

#### Kategori Bazlı Adlandırma Tablosu

**Canavar Sprite'ları**:
```
char_monster_[element]_[archetype]_[name]_[rarity]_[lod].[ext]
char_monster_[element]_[archetype]_[name]_[rarity]_[lod]_[anim].[ext]

Örnekler:
char_monster_fire_striker_infernalclaw_rare_512.png
char_monster_fire_striker_infernalclaw_rare_128_idle.png
char_monster_fire_striker_infernalclaw_rare_128_attack.png
char_monster_fire_striker_infernalclaw_rare_64.png

Element:    fire, water, earth, air
Arketip:    striker, tank, support, mage
Nadirlik:   common, uncommon, rare, epic, legendary
LOD:        512, 128, 64
Animasyon:  idle, attack, skill, hurt, death
```

**Boss Sprite'ları**:
```
char_boss_[element]_[name]_[rarity]_[lod]_[anim].[ext]

Örnekler:
char_boss_fire_volcanoth_legendary_512.png
char_boss_fire_volcanoth_legendary_128_idle.png
```

**Referans Sprite'lar**:
```
ref_[archetype]_[element]_v[version].png

Örnekler:
ref_striker_fire_v01.png
ref_tank_earth_v02.png
```

**Çevre / Ortam**:
```
env_[biome]_[type]_[name]_[variant]_[size].[ext]

Biyom:      crystal, lava, temple, sky, common
Tip:        bg (parallax), tile, prop, deco, light
Boyut:      small, medium, large (proplar için)

Örnekler:
env_crystal_bg_L0_segment01.png
env_crystal_prop_mushroom_glowing_small.png
env_lava_tile_floor_cracked_01.png
env_common_prop_torch_lit_medium.png
env_temple_bg_L2_segment03.png
```

**UI Varlıkları**:
```
ui_[alt-kategori]_[isim]_[durum].[ext]

Alt-kategori: btn, panel, icon, frame, bar, badge, bg, modal
Durum:        default, hover, pressed, disabled, active (uygulanabilirse)

Örnekler:
ui_btn_primary_default.png
ui_btn_primary_pressed.png
ui_panel_inventory_bg.png
ui_icon_element_fire.png
ui_icon_nav_dungeon_active.png
ui_icon_nav_dungeon_inactive.png
ui_frame_rarity_legendary.png
ui_bar_hp_fill.png
ui_badge_notification.png
ui_modal_reward_bg.png
```

**VFX Sprite'ları**:
```
vfx_[efekt]_[varyant]_[boyut].[ext]

Örnekler:
vfx_explosion_fire_medium.png
vfx_loot_sparkle_loop_small.png
vfx_rarity_glow_legendary.png
vfx_hit_slash_01.png
vfx_boss_entrance_smoke.png
```

**Ses Dosyaları**:
```
sfx_[kategori]_[isim]_[varyant].[ext]     (efekt sesleri)
bgm_[sahne]_[mood]_[varyant].[ext]        (müzik)
ui_sfx_[isim].[ext]                       (UI sesleri)

Örnekler:
sfx_combat_hit_sword_01.ogg
sfx_combat_hit_sword_02.ogg
sfx_loot_chest_open.wav
sfx_monster_roar_fire_boss.ogg
bgm_dungeon_crystal_exploration_01.ogg
bgm_boss_battle_epic_01.ogg
bgm_hub_idle_calm_01.ogg
ui_sfx_button_tap.wav
ui_sfx_notification_pop.wav
ui_sfx_reward_reveal.wav
```

**Veri Dosyaları**:
```
data_[sistem]_[isim].[ext]

Örnekler:
data_monster_stats.json
data_dungeon_loot_tables.json
data_element_matrix.json
data_rarity_weights.json
```

**Atlas Dosyaları**:
```
atlas_[kategori]_[isim].[spriteatlas]

Örnekler:
atlas_env_crystal_props.spriteatlas
atlas_env_crystal_bg.spriteatlas
atlas_env_common.spriteatlas
atlas_char_battle_common.spriteatlas
atlas_char_icon.spriteatlas
atlas_ui_common.spriteatlas
atlas_ui_icons.spriteatlas
```

#### Versiyon ve İterasyon

- Varyant numaraları `_01`, `_02` formatında (sıfır dolgulu 2 haneli)
- Aynı varlığın farklı iterasyonları `_v01`, `_v02` ile ayrılır (sadece source dosyalarda)
- Dışa aktarılmış son sürüm versiyon numarası taşımaz — her zaman güncel olan aktiftir

---

### 8.6 Dışa Aktarım Kontrol Listesi

#### Canavar Sprite Kontrol Listesi

| # | Kontrol | Kriter | Seviye |
|---|---------|--------|--------|
| **T1** | Saydam Arka Plan | Alpha kanalı temiz, arka plan kalıntısı yok | ZORUNLU |
| **T2** | Kenar Temizliği | Anti-alias halosu yok, kenarlar temiz (mat/beyaz fringe yok) | ZORUNLU |
| **T3** | Çözünürlük | Master 512x512, LOD varyantları doğru boyutta | ZORUNLU |
| **T4** | Renk Profili | sRGB, gömülü profil | ZORUNLU |
| **T5** | İsimlendirme | Adlandırma konvansiyonuna tam uyum (8.5) | ZORUNLU |
| **T6** | Pivot Doğruluğu | Arketip tablosuna uygun pivot noktası (8.3) | ZORUNLU |
| **T7** | Siluet Testi | Siyah siluet 64px'te ayırt edilebilir (Section 3.1) | ZORUNLU |
| **T8** | Element Renk Doğruluğu | Section 4.3 paletine uygun, ±15 hex tolerans | ZORUNLU |
| **T9** | Nadirlik-Detay Dengesi | Geometri bütçesine uygun (Section 3.1) | ZORUNLU |
| **T10** | Ton Tutarlılığı | Referans sprite ile aynı stil, Karanlık Epik spektrum | ÖNERİLEN |
| **T11** | Kontur Kalınlığı | Nadirlik kademesine uygun (Section 3.1) | ÖNERİLEN |
| **T12** | Işık Yönü | Birincil ışık sol üstten, tüm sprite'larda tutarlı | ÖNERİLEN |

**Kalite Kapısı**: T1-T9 tamamı PASS olmalı. T10-T12'den 2/3 PASS olmalı. Aksi halde revizyon gerekir.

#### Çevre Sprite Kontrol Listesi

| # | Kontrol | Kriter | Seviye |
|---|---------|--------|--------|
| **E1** | Boyut | Katman çözünürlük tablosuna uygun (8.2) | ZORUNLU |
| **E2** | Tileable Kenarlar | Tile sprite'larında dikişsiz tekrar (4 yön test) | ZORUNLU |
| **E3** | Renk Sıcaklığı | Biyom renk sıcaklığı aralığına uygun (Section 6.5) | ZORUNLU |
| **E4** | Doku Stili | Stilize boyama — düz veya fotogerçekçi yasak (Section 6.2) | ZORUNLU |
| **E5** | İsimlendirme | Adlandırma konvansiyonuna tam uyum (8.5) | ZORUNLU |
| **E6** | Parallax Uyumu | Aynı biyomun diğer katmanlarıyla test edilmiş | ÖNERİLEN |
| **E7** | Kontur Tutarlılığı | Katman bazlı kontur kurallarına uygun (Section 3.4) | ÖNERİLEN |

#### UI Sprite Kontrol Listesi

| # | Kontrol | Kriter | Seviye |
|---|---------|--------|--------|
| **U1** | 9-Slice Uygunluğu | Panel/butonlarda 9-slice border'lar doğru kesilmiş | ZORUNLU |
| **U2** | Dokunma Alanı | Etkileşimli öğe min 44x44dp | ZORUNLU |
| **U3** | Kontrast | WCAG AA (4.5:1 metin), AA Large (3:1 büyük metin) | ZORUNLU |
| **U4** | Renk Paleti | UI paleti Section 4.6'ya uygun | ZORUNLU |
| **U5** | İsimlendirme | Tüm durumlar (default/pressed/disabled) mevcut | ZORUNLU |
| **U6** | Ölçeklenme | 320dp-428dp arası ekranlarda kırılma yok | ÖNERİLEN |

#### VFX Sprite Kontrol Listesi

| # | Kontrol | Kriter | Seviye |
|---|---------|--------|--------|
| **V1** | Pre-multiplied Alpha | Additive efektler için alpha doğru uygulanmış | ZORUNLU |
| **V2** | Frame Sayısı | Bütçe dahilinde (8.2 VFX tablosu) | ZORUNLU |
| **V3** | Loop Uyumu | Döngüsel efektlerde ilk/son frame geçişi pürüzsüz | ZORUNLU |
| **V4** | Parçacık Bütçesi | Ekranda max 3 aktif parçacık sistemi kuralına uygun | ÖNERİLEN |

#### Ses Kontrol Listesi

| # | Kontrol | Kriter | Seviye |
|---|---------|--------|--------|
| **A1** | Format | Tablo 8.1'e uygun format ve sample rate | ZORUNLU |
| **A2** | Normalize | Ses seviyesi -3dB ile -1dB arasında normalize | ZORUNLU |
| **A3** | Kırpma | Başta/sonda sessizlik max 50ms | ZORUNLU |
| **A4** | Loop (BGM) | Müzik dosyalarında kesintisiz loop noktası | ZORUNLU |
| **A5** | İsimlendirme | Adlandırma konvansiyonuna uygun (8.5) | ZORUNLU |

---

### 8.7 Bellek Bütçesi Dağılımı

#### Toplam Bütçe: 1024 MB (1 GB)

| Kategori | Bütçe | Yüzde | Detay |
|----------|-------|-------|-------|
| **Unity Runtime + Framework** | 150 MB | ~15% | Engine, Mono/IL2CPP, GC, temel sistemler |
| **Canavar Sprite'ları** | 200 MB | ~20% | Aktif savaş atlas'ları + koleksiyon ekranı sprite'ları |
| **Çevre Atlas'ları** | 120 MB | ~12% | Aktif biyom (2 atlas) + ortak atlas |
| **UI Atlas'ları** | 80 MB | ~8% | Tüm UI atlas'ları (her zaman yüklü) |
| **VFX / Parçacık** | 60 MB | ~6% | VFX atlas + parçacık sistem verileri |
| **Ses (Audio)** | 100 MB | ~10% | Aktif BGM (streaming) + yüklü SFX havuzu |
| **Animasyon Verileri** | 40 MB | ~4% | Sprite animasyon clip'leri, AnimationController'lar |
| **Oyun Verileri / Kod** | 60 MB | ~6% | ScriptableObject, JSON config, gameplay state |
| **Font / Metin** | 20 MB | ~2% | Font atlas'ları (Türkçe karakter dahil), lokalizasyon |
| **Güvenlik Marjı** | 194 MB | ~19% | OS değişkenliği, GC spike'lar, beklenmeyen yükler |

**Kritik Kural**: Güvenlik marjı %15'in altına düşmemeli. Düşük-uçlu cihazlarda (2 GB RAM) toplam uygulama belleği 600 MB'ı geçmemeli — bu durumda her kategori %60'a ölçeklenir.

#### Kategori Bazlı Detay Bütçeleri

**Canavar Bellek Bütçesi (200 MB)**:

| Durum | Yüklü İçerik | Tahmini Bellek |
|-------|--------------|----------------|
| Savaş ekranı | 5 oyuncu + 5 düşman canavar (128px, sıkıştırılmış) + idle+attack anim | ~15-25 MB |
| Koleksiyon ekranı | 1 aktif 512px sprite + 20-30 ikon (64px) | ~5-10 MB |
| Boss savaşı | 5 oyuncu + 1 boss (büyük sprite) + efektler | ~20-30 MB |
| Savaş atlas (common) | Tam atlas bellekte | ~3-4 MB |
| Savaş atlas (rare) | İhtiyaçta yüklenir | ~3-4 MB |
| İkon atlas | Her zaman yüklü | ~1 MB |

**Aktif kullanım genellikle 30-50 MB civarında.** 200 MB bütçe, koleksiyon büyüdükçe ve çoklu atlas'lar yüklendikçe headroom sağlar.

**Çevre Bellek Bütçesi (120 MB)**:

| İçerik | Bellek |
|--------|--------|
| Aktif biyom 2 atlas (sıkıştırılmış) | ~6-8 MB |
| Ortak atlas | ~1 MB |
| Parallax katmanları (6 adet, aktif) | ~5-10 MB |
| Runtime prop instance'ları | ~2-5 MB |
| **Aktif toplam** | **~15-25 MB** |

**120 MB bütçe neden bu kadar yüksek?** Biyom geçişlerinde iki biyomun atlas'ı aynı anda bellekte olabilir (eski boşaltılırken yeni yüklenir). Bu geçici durum için headroom gerekir.

**Ses Bellek Bütçesi (100 MB)**:

| İçerik | Yükleme Modu | Bellek |
|--------|--------------|--------|
| Aktif BGM | Streaming (bellekte ~200 KB buffer) | ~0.5 MB |
| Savaş SFX havuzu | Decompress On Load | ~15-25 MB |
| UI SFX | Decompress On Load | ~3-5 MB |
| Ortam SFX | Compressed In Memory | ~5-10 MB |

#### Bellek İzleme Kuralları

1. **Unity Profiler** ile her milestone'da bellek snapshot'ı alınır
2. Herhangi bir kategori bütçesinin %90'ını aşarsa uyarı — %100'ü aşarsa optimizasyon zorunlu
3. Düşük-uçlu cihaz testi (2 GB RAM, Android): toplam uygulama belleği < 600 MB
4. Yüksek-uçlu cihaz testi (6+ GB RAM, iOS): toplam uygulama belleği < 900 MB
5. `UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()` ile runtime izleme

---

### 8.8 Unity Import Ayarları Referansı

#### Sprite Import Ayarları (Genel)

```
Texture Type:               Sprite (2D and UI)
Sprite Mode:                Single (tek sprite) / Multiple (sprite sheet)
Pixels Per Unit:            100
Mesh Type:                  Tight
Extrude Edges:              1
Filter Mode:                Bilinear
Generate Mip Maps:          HAYIR
Read/Write Enabled:         HAYIR
sRGB (Color Texture):       EVET
Alpha Source:               Input Texture Alpha
Alpha Is Transparency:      EVET
Wrap Mode:                  Clamp
```

#### Platform Override — iOS

```
Max Texture Size:           2048
Resize Algorithm:           Mitchell
Format:                     ASTC 6x6 (varsayılan)
                            ASTC 4x4 (UI ikon/metin — keskin kenar)
                            ASTC 8x8 (arka plan L0-L1 — düşük detay)
Compression Quality:        Best (shipping) / Fast (development)
Override For iOS:           EVET
```

#### Platform Override — Android

```
Max Texture Size:           2048
Resize Algorithm:           Mitchell
Format:                     ETC2 RGBA (alfa gereken sprite)
                            ETC2 RGB (opak sprite, L0-L2 parallax)
Compression Quality:        Best (shipping) / Fast (development)
Override For Android:       EVET
```

**ETC2 Kısıtlama Notu**: ETC2 sabit 4x4 blok boyutu kullanır — ASTC'nin değişken blok boyutu esnekliğine sahip değildir. Bu nedenle ETC2'de kalite kaybı daha belirgin olabilir, özellikle gradient geçişlerinde ve yumuşak kenar bölgelerinde. Stilize boyama stili (Section 6.2) bu sorunu hafifletir çünkü dokuların sert detay gereksinimleri düşüktür.

#### Sprite Atlas Import Ayarları

```
Packing Settings:
  Allow Rotation:           HAYIR
  Tight Packing:            EVET
  Alpha Dilation:           EVET
  Padding:                  4

Texture Settings:
  Read/Write:               HAYIR
  Generate Mip Maps:        HAYIR
  sRGB:                     EVET
  Filter Mode:              Bilinear
  
Include in Build:           EVET (common, battle, UI atlas'ları)
                            HAYIR (collection atlas'ları — Addressable)
```

#### Ses Import Ayarları

```
BGM (OGG):
  Load Type:                Streaming
  Compression Format:       Vorbis
  Quality:                  70%
  Sample Rate:              Preserve Sample Rate
  Preload Audio Data:       HAYIR
  Load In Background:       EVET

Kısa SFX (WAV, <2s):
  Load Type:                Decompress On Load
  Compression Format:       PCM
  Sample Rate:              Preserve Sample Rate
  Preload Audio Data:       EVET
  Load In Background:       HAYIR

Uzun SFX (OGG, >2s):
  Load Type:                Compressed In Memory
  Compression Format:       Vorbis
  Quality:                  100%
  Sample Rate:              Preserve Sample Rate
  Preload Audio Data:       EVET
  Load In Background:       EVET

UI SFX (WAV):
  Load Type:                Decompress On Load
  Compression Format:       PCM
  Sample Rate:              Preserve Sample Rate (22050 Hz)
  Preload Audio Data:       EVET
  Load In Background:       HAYIR
```

#### URP 2D Renderer Ayarları

```
Sorting Layers (sıralama):
  Background-Far            (L0)
  Background-Mid            (L1)
  Background-Near           (L2)
  Gameplay                  (L3 — canavarlar, proplar, interaktif)
  Foreground                (L4)
  Overlay                   (L5 — atmosferik efektler)
  UI                        (Screen-space overlay)

Her katman ayrı Sorting Layer'da — draw call batching katman içinde çalışır.
Aynı Sorting Layer + aynı atlas + aynı material = 1 draw call (SRP Batcher).
```

#### Addressable Yükleme Stratejisi

| Grup | Yükleme Zamanı | Boşaltma Zamanı | Not |
|------|----------------|-----------------|-----|
| `core-ui` | Uygulama başlangıcı | Asla | UI atlas, font, ortak ikon |
| `core-audio` | Uygulama başlangıcı | Asla | UI SFX, temel ses efektleri |
| `common-env` | Uygulama başlangıcı | Asla | Ortak çevre atlas |
| `char-icons` | Uygulama başlangıcı | Asla | Canavar ikon atlas |
| `char-battle-common` | İlk savaş öncesi | Asla | Common/Uncommon savaş atlas |
| `char-battle-rare` | Savaş yükleme | Savaş sonu | Rare/Epic savaş atlas |
| `char-battle-legendary` | Savaş yükleme | Savaş sonu | Legendary savaş atlas |
| `char-collection-[id]` | Ekran açıldığında | Ekran kapandığında | Tekil 512px koleksiyon sprite |
| `biome-[name]` | Biyom girişinde | Biyom çıkışında | Biyom atlas çifti + BGM |
| `boss-[name]` | Boss karşılaşması | Boss savaşı sonu | Boss sprite + efekt + BGM |

**Yükleme Ekranı Kuralı**: Addressable grup yüklemesi 500ms'yi aşarsa ilerleme çubuğu gösterilir. 200ms altı yüklemeler görünmez geçiş.

---

## 9. Reference Direction

Referanslar **kalibrasyon aracıdır, kopyalama kılavuzu değil** — "bu kadar" ve "buraya kadar" sınırlarını çizer.

---

### Referans 1: Monster Hunter Serisi (Capcom) — Canavar Tasarım Tonu

**Ne Alınacak**:
- **Anatomik inandırıcılık katmanı**: Her zırh plakası, diken ve kanat fonksiyonel bir neden taşımalı. Rathalos'un kanat-bacak ilişkisi, Nergigante'nin diken büyüme mantığı gibi.
- **"Trophy pose" felsefesi**: Canavar sahnenin yıldızı, 3/4 açıda en heybetli siluet. Section 5.3 vitrin pozunun kaynağı.
- **Nadirlik-heybet korelasyonu**: Elder Dragon'lar → daha karmaşık, daha çok ışıyan element. Section 5.1 nadirlik-detay kademesinin referansı.

**Kaçınılacak**: Fotogerçekçi PBR doku (AI tutarlılığını bozar), karmaşık aksiyon pozları (biz "nefes alan heykel"), doğal habitat entegrasyonu (canavarlar bağımsız varlıklar).

**Bağlantı**: Section 5 (Character Design Direction)

---

### Referans 2: Ori and the Blind Forest / Will of the Wisps (Moon Studios) — Çevre Sanatı ve Aydınlatma

**Ne Alınacak**:
- **Boyama hisli parallax derinliği**: Uzak katmanlar büyük renk blokları, yakın katmanlar keskin fırça darbeleri. Section 6.2 ve 6.6'nın görsel prototipi.
- **Sıcak ışık yönlendirmesi**: Karanlık mağaralarda bile altın-amber tonlu ışık kaynakları oyuncuyu çeker. Section 2 zindan keşfi atmosferinin esin kaynağı.
- **Doygunluk gradyanı ile derinlik**: Section 3.4 katman bazlı doygunluk kurallarının teknik referansı.

**Kaçınılacak**: Melankolik/kırılgan atmosfer (biz fetih heyecanı), solgun mavi-mor baskın palet (biz sıcak amber-altın), küçük savunmasız protagonist (canavarlarımız heybetli güçler).

**Bağlantı**: Section 2, Section 6, Section 3.4

---

### Referans 3: AFK Arena (Lilith Games) — UI Düzeni ve Mobil UX Akışı

**Ne Alınacak**:
- **Koyu panel + altın vurgu UI formülü**: Section 4.6 UI paletinin doğrudan esin kaynağı. Mobilde güneş ışığında bile yüksek kontrast.
- **"Her zaman kazanıyorsun" geri bildirim dili**: Idle birikim toplama, sandık açma, ödül yağmuru animasyonları. Section 7.4 loot reveal sekansının referansı.
- **Bildirim rozeti yönetimi**: Max 3 görünür rozet kuralımız bu deneyimden çıkarılan sınır.
- **Canavar vitrin kartı düzeni**: Karanlık gradient + karakter + element ikonu + nadirlik çerçevesi.

**Kaçınılacak**: Aşırı UI katmanlaşması (4-5 üst üste modal), anime/chibi karakter stili (biz Karanlık Epik), ekran başına 20+ dokunulabilir öğe.

**Bağlantı**: Section 7, Section 4.6, Section 4.4

---

### Referans 4: Darkest Dungeon (Red Hook Studios) — Renk Anlatımı ve Dramatik Kontrast

**Ne Alınacak**:
- **Kısıtlı paletle durum anlatımı**: Her durum 2-3 baskın tonla tanımlanır. Section 4.2 semantik renk kullanımının ("renk=duygu" disiplini) referansı. Fark: DD korkutucu, biz güçlendirici.
- **Dramatik ışık kontrastı ile odak yönetimi**: Boss savaşı "arena dışı %60 dim" kuralının pozitif versiyonu. Karartma epik an odağı için.
- **Kalın kontur + sınırlı renk bloğu**: Mike Mignola etkisi. Section 5.6 "thick black outlines" prompt zorunluluğunun kaynağı — AI tutarlılığı için etkili stil çerçevesi.

**Kaçınılacak**: Kasvetli, umutsuz atmosfer ("her an öleceksin" → biz "her an kazanacaksın"), desatüre/soğuk/soluk tonlar (Section 1'de yasaklı), stres ve çaresizlik ifadeleri.

**Bağlantı**: Section 4, Section 2, Section 5.6

---

### Referans 5: Hades (Supergiant Games) — Animasyon Hissi ve Ödül Kutlama Enerjisi

**Ne Alınacak**:
- **"Ekranda canlı" animasyon enerjisi**: 4-6 frame ile "yaşayan varlık" hissi. Section 5.3 "nefes alan heykel" idle animasyonu felsefesinin kaynağı.
- **Ödül patlama → ritim düşüşü**: Oda temizleme sonrası kısa duraklama + parlak efekt + rahatlatıcı ses. Section 2 loot/ödül anı enerji eğrisi ve Section 7.4 loot reveal sekansıyla birebir örtüşür.
- **Element efektlerinin sahneyi boyaması**: Boon kullanımında sahne ilgili tanrının renginde tint alır. Section 2 "element renk patlamaları" tekniğinin doğrudan uyarlaması.

**Kaçınılacak**: Hızlı aksiyon temposu (biz idle RPG), izometrik perspektif ve çok yönlü hareket (biz sabit 3/4 açı), batı mitolojisi görsel dili (biz orijinal dünya).

**Bağlantı**: Section 5.3, Section 2, Section 7.4

---

### Referans Kapsam Matrisi

| Görsel Eksen | Birincil Referans | Destekleyici |
|---|---|---|
| **Canavar tasarım tonu** | Monster Hunter | Darkest Dungeon (kontur stili) |
| **Çevre/dünya boyama** | Ori and the Blind Forest | — |
| **UI/UX mobil akışı** | AFK Arena | — |
| **Renk anlatımı/kontrast** | Darkest Dungeon | Hades (element tint) |
| **Animasyon hissi/ödül enerjisi** | Hades | AFK Arena (loot UX) |

### Referans Kullanım Kuralları

1. **Kalibrasyon, kopyalama değil**: Hiçbir referansın görsel dilini birebir almayın.
2. **KAÇINILACAK kısımlar ZORUNLU**: AI prompt'larında ve asset review'larda her iki yönde kontrol yapılır.
3. **Kesişim noktalarında art bible hakimdir**: Referans art bible kararıyla çeliştiğinde art bible kazanır.
4. **Yeni referans ekleme**: Mevcut eksenle örtüşen eklenmez, mevcut güncellenir.
