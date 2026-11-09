# PreBot sorunumuzu çözüyor mu? Kesin karar

*9 Kasım 2026. Bu karar, önceki raporlardaki iddiaların birincil kaynaklardan yeniden doğrulanmasına, Türkiye'deki talebin ve rakiplerin araştırılmasına ve kodun sorunla karşılaştırılmasına dayanıyor. Ham notlar: [`research_notes/PreBot karar teyidi/`](../research_notes/PreBot%20karar%20teyidi/).*

## Karar

**EVET: PreBot'u yapmaya ve yayına almaya devam ediyoruz.**

Bu karar üç gerekçeye dayanıyor:

- Sorun gerçek ve oyunun kuralından doğuyor.
- Türkiye'de bu sorunu "şu an" çözen bir araç yok.
- PreBot'un akışı sorunu teknik olarak çözüyor.

Kararın tek koşulu, aynı akşam saatinde yeterince müsait oyuncu olması (likidite). Bunu kod değil, ilk kullanıcılar belirleyecek. Bu yüzden karar ölçülebilir bir devam/dur eşiğine bağlandı (aşağıda).

| Soru | Cevap | Güven |
|---|---|---|
| Sorun gerçek mi? | **Evet.** 4 kişilik parti Ranked Flex'e giremiyor. TR oyuncuları bunun için her gün ilan veriyor. | Yüksek (birincil kaynak + gözlem) |
| Flex TR'de yaşıyor mu? | **Evet.** TR'de Flex dereceli hesap sayısı Solo/Duo'nun yaklaşık %70'i (EUW'de %45). | Orta–yüksek (op.gg sayıları) |
| Başka biri zaten çözüyor mu? | **Kısmen.** LoLFinder.net'te Türkçe bir Flex panosu var, ama anlık bildirim ve "yeri kap" yok. Riot'un PC'de yabancı bulma özelliği yok. | Yüksek |
| Web mi Discord mu? | **İkisi birden, tek havuzda.** Discord resmi olarak engelli, ama TR oyuncuları onu VPN/DNS ile yaygın şekilde kullanmaya devam ediyor. Web, Discord'u olmayanlar ve bildirim için ana kapı; Discord ise oyuncuların zaten toplandığı ikinci kapı. | Engel: yüksek. Kullanım yaygınlığı: kullanıcı gözlemi |
| PreBot'un akışı sorunu çözüyor mu? | **Evet, teknik olarak.** İlan açılıyor, uygun kişilere saniyeler içinde push gidiyor, ilk dokunan yeri alıyor, ilan sahibine Riot ID ile haber gidiyor. Gerçek tarayıcıda uçtan uca denendi. | Yüksek |
| Gerçek hayatta ilanlar dolacak mı? | **Bilinmiyor.** Bu, akşam saatinde havuzda kaç kişi olacağına bağlı. Henüz gerçek kullanıcı yok. | Ölçülmedi |

## 1. Sorun gerçek mi? Evet

**Oyunun kuralı.** Riot'un destek sayfası (7 Mayıs 2026) Ranked Flex'e 1, 2, 3 ya da 5 kişilik partilerin girebileceğini söylüyor. Aynı sayfada 4 kişilik partilerin "unranked" oynamak zorunda olduğu yazıyor. 2026 sezonunda bunu değiştiren bir yama notu yok.

Yani "4 kişiyiz" durumu bir tercih değil, bir engel. Dört kişi ya 5.'yi bulacak ya da dereceli oynamayacak.

**Türkiye'deki talep (gözlem).**
- **LoLFinder.net'in Türkiye Flex panosu:** yaklaşık 465 aktif ilan var ve son dönemde günde 3–5 yeni ilan geliyor. İlanlar tam bizim senaryomuz: "4 kişiyiz 1 kişi daha lazım hızlı", "acil esnek +1 lazım", "ACİL 1 LAZIM".
- **Türkiye'de Flex, Normal'den çok daha yaygın:** aynı sitede 39 sayfa Flex ilanına karşılık yalnızca 4 sayfa Normal ilanı var. Yani insanlar "Normal oynarız" diye vazgeçmiyor, Flex için adam arıyor.
- **Forumlarda zayıf:** Technopat'ta 2026'da ayda yaklaşık bir takım arkadaşı ilanı var. Ekşi, Reddit, X ve TikTok'ta aranabilir bir şey bulunamadı. Bu, arama motorlarının sınırı olabilir; talep yok demek değil.
- **Ölçülemeyen kısım:** Arkadaş gruplarında ve WhatsApp'ta sorulan "1 kişi var mı?" soruları görünmüyor. Gerçek talep görünenden büyük olmalı, ama bunu kanıtlayamıyoruz.

**Sonuç:** Talep gerçek ve her gün tekrarlanıyor, ama küçük. Ayda yüzlerce görünür ilan var, binlerce değil.

## 2. Bugün insanlar ne yapıyor ve neden yetmiyor?

| Yöntem | Sorunu |
|---|---|
| Arkadaşın arkadaşına sormak | Herkes müsait değil. Kimin müsait olduğu bilinmiyor. |
| LoLFinder.net | Statik pano. İlanlar 6 ay duruyor, "acil" ilanlar günlerce asılı kalıyor. Müsait oyuncuya bildirim yok, yeri kapma yok. |
| Discord LFG kanalları | En yaygın yöntemlerden biri: engele rağmen insanlar VPN/DNS ile kullanıyor. Ama kanala yazılan "+1 lazım" mesajı müsait oyuncuya bildirim olarak gitmiyor, kaybolup gidiyor. Bazı kişilerde engeli aşma yöntemleri bozuluyor. |
| OP.GG Duo | Türkiye'de neredeyse boş, son ilan 2 gün önce. |
| Riot'un parti bağlantısı (2026) | Eşleştirme değil, yalnızca bağlantı paylaşma. Resmi olarak ABD, Kanada ve Brezilya'da açıldı. Discord bağlantısı gerektiriyor. |

**Boşluk:** Hiçbiri, "şu an müsait ve sana uyan biri var" bilgisini ilan sahibine dakikalar içinde ulaştırmıyor. PreBot'un farkı tam olarak bu: müsait oyuncuya anlık push bildirimi ve tek dokunuşla yer alma.

## 3. PreBot'un akışı sorunu çözüyor mu? Evet, iki küçük düzeltmeyle

**Bugünkü akış:**

| Adım | Ne oluyor | Süre |
|---|---|---|
| 1 | İlan sahibi ilanı açar (rol, rank aralığı, kaç kişi eksik). | ~20 saniye |
| 2 | İlk dalga: en uygun 5 kişiye bildirim gider. | Saniyeler |
| 3 | Yer dolmazsa 2. dakikada 10 kişiye, 5. dakikada 15 kişiye bildirim gider. Rank aralığı her dalgada genişler. | 2–5 dakika |
| 4 | Biri Katıl'a basar ve yeri alır. | Tek dokunuş |
| 5 | İlan sahibine "🎉 X katıldı, Riot ID: …" bildirimi gider. | Saniyeler |
| 6 | İlan sahibi Riot ID'yi kopyalar, oyunda ekler ve davet eder. | ~30 saniye |

Bu akış gerçek Edge tarayıcısında ve gerçek Windows push servisinde 8 adımlı uçtan uca testle doğrulandı. Otomatik testlerin hepsi (109) geçiyor.

**Teyit turunda kodda bulunan iki sorun.** İkisi de küçük havuzda ilanın dolma hızını doğrudan düşürüyor:

1. **Boş dalga 2 dakika bekletiyor.** İlk dalga tam rank aralığında kimseyi bulamazsa genişletilmiş ikinci dalga yine 2 dakika sonra gidiyor (`PreService.RunNextWaveAsync`, `Matcher.DelayAfterWave`). Küçük havuzda en değerli dakikalar boşa gidiyor. **Düzeltme:** Dalga kimseyi bulamazsa bir sonraki dalgayı hemen çalıştır. Emek: 1–2 saat.
2. **Varsayılan rank aralığı dar.** İlan formu varsayılan olarak Altın–Platin geliyor (`NewListing.razor`). Oysa Riot'un kuralına göre Elmas ve altında Flex'te rank kısıtlaması yok. Dar aralık yalnızca zaten küçük olan havuzu daraltıyor. **Düzeltme:** Varsayılan "Herhangi (Demir–Elmas)" olsun. İsteyen daraltsın. İlk dalga ±1 kademeyle başlasın (yol haritasında F1.3). Emek: 1–2 saat.

**Akışı zayıflatan, ama kodla tamamen çözülemeyen noktalar:**
- **iPhone:** Push için sitenin Safari'de Ana Ekrana eklenmesi gerekiyor. Bu, iOS 16.4'ten Safari 27'ye kadar değişmedi. Site bunu iPhone kullanıcısına anlatıyor.
- **Bilgisayar:** Bildirimin gelmesi için tarayıcının açık olması gerekiyor. Kurulu PWA'nın tarayıcı kapalıyken bildirim alıp alamadığı belgelerde kesin değil.
- **Gelmeyen oyuncu:** Bugün yalnızca ilan sahibinin "Gelmedi, yeniden ara" düğmesi var. Gelme oranı ölçülmüyor (yol haritasında F1.7).

## 3b. Discord hafife alınmamalı: tek havuz

Discord Türkiye'de resmi olarak engelli, ama proje sahibinin kendi gözlemine göre çevresindeki oyuncuların çoğu onu hâlâ bir şekilde kullanıyor. Araştırma da bunu destekliyor: küçük Türk LoL sunucuları Disboard'da dakikalar içinde öne çıkarılıyor. Bu yüzden Discord'u yalnızca "duyuru kanalı" saymak hata olur. **Discord, oyuncuların zaten toplandığı ikinci kapı.**

**Bugünkü sorun: iki ayrı havuz.** Kod şu an web ve Discord'u ayrı topluluklar olarak tutuyor:

- Web kullanıcıları web havuzunda. Discord'da `/musaitim` diyenler o sunucunun ayrı havuzunda.
- Web'de açılan ilan, Discord'da `/musaitim` demiş birine bildirim olarak gitmiyor; yalnızca kanalda duyuru olarak görünüyor.

Kodda: web hesabı `UserAccount` kimliği ile, Discord kullanıcısı Discord kimliği ile tutuluyor. `Communities.Web` ile sunucu kimlikleri de ayrı.

En büyük risk likidite olduğu için **havuzu ikiye bölmek en büyük riski büyütüyor.** Akşam web'de 3, Discord'da 4 müsait kişi varsa iki tarafta da ilan dolmayabilir. Tek havuzda ise 7 kişi olur.

**Karar: tek havuz.** Hangi kapıdan gelirse gelsin herkes aynı havuzda olacak (yol haritasında F1.12):

1. **Tek kimlik.** Bot, Discord kullanıcısını `discord` sağlayıcılı bir `UserAccount` ile eşleştirir; kayıt yoksa oluşturur. Siteye Discord ile giren kişi aynı hesaba düşer. Riot ID ve müsaitlik iki tarafta da aynı olur.
2. **Tek havuz.** Discord'daki `/musaitim` ve `/pre-lazim` web topluluğuna (`Communities.Web`) yazar. Web'de açılan ilan Discord'daki müsait oyunculara, Discord'da açılan ilan web'deki müsait oyunculara bildirim gönderir.
3. **Discord'dan katılma.** Bildirim, Discord'da müsait olan kişiye DM ve **Katıl** butonu olarak gider. Kanal duyurusunda da **Katıl** butonu olur. Butona basan kişi siteye gitmeden yeri alır. Riot ID'si yoksa bir modal ile sorulur.
4. **İlan sahibine bildirim iki kanaldan.** İlan sahibi Discord'luysa katılım haberi ona DM ile de gider.

**Emek:** L (yaklaşık 15–25 saat). **Başarı ölçütü:** İstatistikte ilanların bir kısmı karşı kapıdan gelen oyuncularla doluyor (web ilanı Discord'dan katılan biriyle ya da tersi).

## 4. Riskler (en güçlüden en zayıfa)

1. **Likidite.** Görünür talep günde 3–5 ilan. Akşam 20:00–00:00 arasında havuzda 3–5 müsait kişi yoksa bildirim gidecek kimse olmaz. Bu, en büyük ve tek ölümcül risk.
2. **"Önce arkadaş" kültürü.** Bazı gruplar yabancı almaktansa Solo'ya geçmeyi tercih ediyor.
3. **LoLFinder alışkanlığı.** İnsanlar ilanı zaten oraya veriyor. PreBot'un sadece farklı değil, açıkça daha hızlı olması gerekiyor.
4. **Reşit olmayanlar.** Bu panolarda 13 yaşında olduğunu söyleyen kullanıcılar var. Yaş alt sınırı gerekiyor (yol haritasında F1.6).
5. **Riot'un kendi özelliği.** Riot'un 2027'de gelecek yeni istemcisi bir parti bulucu eklerse PreBot'un yeri daralır.

## 5. Devam / dur eşiği

Yayına aldıktan sonra **ilk 2 hafta, akşam 20:00–00:00 arasında** ölç. Ölçüm için `/istatistik` sayfasına bak.

| Ölçüt | Devam | Uyarı | Dur ya da yön değiştir |
|---|---|---|---|
| Akşam açılan ilanların 15 dakikada dolma oranı | ≥ %50 | %25–50 | < %25 (4 hafta sonunda) |
| Tipik (medyan) dolma süresi | ≤ 10 dk | 10–15 dk | > 15 dk |
| Akşam başına müsait ya da "Bana haber ver" kurmuş kişi | ≥ 15 | 5–15 | < 5 |

- **Uyarı bölgesinde:** yeni özellik ekleme. Önce daha çok kişi getir, rank aralığını genişlet, saatleri tek bir akşam penceresine topla.
- **4 hafta sonunda hâlâ "dur" bölgesindeysek:** PreBot'u anlık bir eşleştirme aracı olarak değil, kendi arkadaş çevremiz ve bir iki topluluk için bir duyuru aracı olarak tut. Proje açık kaynak kalır ve kod kaybolmaz.

## 6. İlk kullanıcılar nereden gelecek?

- **Çekirdek:** Kendi arkadaş grubun ve onların arkadaşları, 10–20 kişi. "Akşam müsaitsen bas" diye anlaşın.
- **Tam hedef kitle:** LoLFinder'da Türkiye Flex ve Duo panosunda ilan verenler. Duo panosunda günde yaklaşık 12 ilan var. Bunlar yabancıyla oynamaya zaten razı olan doldurucular. Onların panosunu spam'leme. Kendi ilanını verirken ya da bir ilana cevap verirken PreBot bağlantısını paylaş.
- **Discord sunucuları (öncelikli):** Türk LoL Discord sunucuları hâlâ aktif. Botu bu sunuculara eklemek, tek havuzla birlikte en hızlı büyüme yolu. Sunucu yöneticilerinden izin alarak bir #flex-pre kanalı aç.
- **Sonra:** Üniversite e-spor kulüpleri ve küçük yayıncılar.

## 7. Önceki planda düzeltilenler

Teyit turu yol haritasında şu düzeltmeleri gerektirdi. Hepsi [yol haritasına](PreBot%20geliştirme%20yol%20haritası.md) işlendi.

- **.NET 10'da `KnownNetworks` eskidi** (ASPDEPR005). Plandaki örnek kod, uyarıları hata sayan CI'ı kırardı. Yerine `KnownIPNetworks` kullanılacak.
- **Hetzner CX23** 9 Kasım 2026'da "Currently not available" görünüyordu. Aynı sınıfta başka bir küçük VPS seçilecek.
- **Riot 2026 sezon yazısının tarihi** 1 Aralık 2025 (2026 değil).
- **"Flex eşleştiricisi parti içindeki rankları tartıyor" iddiasının** birincil kaynağı yok. Plandan çıkarıldı.
- **Riot'un zorunlu ibaresi** şu cümleyle bitmeli: "Riot Games, and all associated properties are trademarks or registered trademarks of Riot Games, Inc." Sitenin altbilgisindeki Türkçe çeviri buna göre gözden geçirilmeli.
- **KVKK:** Proje VERBİS kaydından büyük olasılıkla muaf (50 çalışan / 100 milyon TL eşiği; ticari olmama diye ayrı bir ölçüt yok). Aydınlatma metni ve Madde 9 yükümlülükleri ise geçerli.

**Kapsam notu:** Kapsam yalnızca Flex. Riot'un deneysel Ranked 5s kuyruğu (Cuma–Pazar 16:00–04:00, 7 Ocak 2027'ye kadar) kapsam dışı bırakıldı. Bilgi için: bu kuyruk hafta sonu akşamlarıyla çakışıyor ve bazı beşlileri Flex'ten çekebilir. Hafta sonu Flex verisini yorumlarken bunu göz önünde tut.

## 8. Doğrulanamayanlar

- Türk resmi siteleri (kvkk.gov.tr, Resmî Gazete, BTK) bu ortamdan açılamadı. KVKK maddeleri hukuk bürosu özetlerine dayanıyor.
- Flex'teki Master+/Zümrüt kuralının Flex rankına mı Solo rankına mı baktığı belli değil. Bilgi Riot sayfasında bir görselin içinde.
- Riot parti bağlantısının Türkiye'de çalışıp çalışmadığı belli değil. Resmi açılış listesinde yok, ama bir Türk paylaşımında görüldü.
- Gerçek dolum oranı ve gelmeme oranı, ancak gerçek kullanıcılarla ölçülebilir. Yayımlanmış hiçbir LFG kıyas verisi bulunamadı.

## Sıradaki adımlar

1. **Kod (hemen, toplam 2–4 saat), ardından tek havuz (F1.12):**
   - Boş dalga bekletmesin.
   - Varsayılan rank aralığı geniş olsun.
2. **Kod (yayından önce):** Yol haritasının Faz 0'ı.
   - Sızan anahtarı depodan çıkar.
   - `?katil=1` bağlantısını güvenli hale getir.
   - Hız sınırı ekle.
   - Otomatik yedek kur.
   - KVKK metnini tamamla.
3. **Sen:**
   - VPS ve alan adı al, siteyi yayına al.
   - Aynı gün Riot üretim anahtarına başvur.
   - 10–20 kişilik çekirdek grubu topla.
4. **2 hafta sonra:** `/istatistik` sayfasına bak ve yukarıdaki eşiğe göre devam ya da dur kararı ver.
