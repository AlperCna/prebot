# PreBot: Flex için anında 5. kişi

League of Legends TR sunucusunda **"4 kişiyiz, 1 kişi lazım"** sorununu çözen bir Discord botu. Ranked Flex'e 4 kişilik partiyle girilemiyor, Ranked 5s ise tam beşli istiyor. Tek bir eksik kişi bütün akşamı bozabiliyor.

PreBot bir ilan panosu gibi çalışmıyor, çağrı sistemi gibi çalışıyor. İlanı kanala asıp birinin görmesini beklemek yerine, **şu an müsait olan ve rolü ile rankı uyan** kişileri doğrudan etiketliyor.

> PreBot, Riot Games tarafından onaylanmamıştır.

## Nasıl çalışır

```
Oyuncu:  /musaitim rol:Destek rank:Altın sure:1 saat   → 1 saatlik müsaitlik havuzuna girer
Sen:     /pre-lazim rol:Destek en-dusuk:Altın en-yuksek:Platin
Bot:     İlanı kanala asar ve uygun kişileri dalga dalga etiketler:
           0. dk  → en uygun 5 kişi (tam rank aralığı)
           2. dk  → 10 kişi daha (aralık ±1 kademe)
           5. dk  → 15 kişi daha (aralık ±2 kademe)
Oyuncu:  [Katıl]'a basar. İlk basan yeri alır, sana Riot ID'si gönderilir
İlan:    15 dakikada dolmazsa kendiliğinden kapanır
Gelmedi: [Gelmedi, yeniden ara] → yer boşalır, bot yeniden arar
```

**Spam koruması:**
- Aynı kişi 15 dakikada en fazla bir kez etiketlenir.
- Sadece seçilen kişiler etiketlenir; `@everyone` ya da rol pingi atılmaz.
- Müsaitlik süresi dolunca kişi havuzdan kendiliğinden çıkar.

## 🌐 Web uygulaması (yeni, asıl ürün)

Discord Türkiye'de erişime kapalı olduğu için (Ekim 2024'ten beri) asıl ürün artık bir web sitesi. Discord botu yardımcı kanal olarak kalıyor. Gerekçeler [arastirma/reports/Web uygulaması mı Discord mu.md](arastirma/reports/Web%20uygulaması%20mı%20Discord%20mu.md) raporunda.

```bash
dotnet run --project src/PreBot.Web --launch-profile http
```

Tarayıcıda <http://localhost:5215> adresini aç. Yerelde **test girişi** var: herhangi bir isimle giriş yapabilirsin. İki oyuncuyu aynı anda denemek için bir sekmede `localhost:5215`, diğerinde `127.0.0.1:5215` aç; ikisi ayrı oturum gibi davranır.

| Sayfa | Ne yapar |
|---|---|
| `/havuz` | "Şu an müsaitim" butonu, canlı havuz, açık ilanlar. Uygun bir ilan açılınca sayfada uyarı çıkar. |
| `/ilan-ac` | Eksik kişi için ilan açar, uygun oyunculara dalga dalga haber verir |
| `/ilan/{id}` | Katıl, Vazgeç, Gelmedi-yeniden ara, Kapat butonları ve paylaşılabilir link. Riot ID'leri yalnızca ilandakiler görür. |
| `/profil` | Görünen isim, Riot ID, verilerini silme |

Bütün sayfalar canlı: biri katılınca diğer ekranlar yenilemeye gerek kalmadan güncellenir (Blazor Interactive Server, SignalR).

### 🔔 Bildirimler (PWA + Web Push)

- **`/bildirimler`**: bu cihazda bildirimleri aç/kapat ve test bildirimi gönder. Bir kişi birden fazla cihaz (telefon + PC) kaydedebilir.
- **Bildirimin "Katıl" butonu** ilanı açar ve seni otomatik katar (`/ilan/{id}?katil=1`). Chrome, Edge ve Firefox'ta buton olarak görünür. Safari'de bildirime dokununca ilan açılır.
- **"Bana haber ver"**: her akşam "Müsaitim" demek istemeyenler için kalıcı uyarı. Rol, rank ve Türkiye saatiyle bir zaman aralığı seçilir. Sıra ve sınırlar:
  - Önce o an müsait olanlara haber gider. "Bana haber ver" diyenlere 2. dalgadan itibaren, ya da müsait kimse uymuyorsa hemen gider.
  - Kişi başına 30 dakikada en fazla 1, günde en fazla 5 bildirim.
- **Spam koruması:**
  - Aynı ilan için kimseye iki kez haber gitmez.
  - Müsait oyunculara en fazla 3 dakikada bir bildirim gider.
  - Bildirim 120 saniyede geçersiz olur; grup dolduktan sonra gelmez.
- **iPhone:** bildirim almak için siteyi önce Safari'de **Paylaş → Ana Ekrana Ekle** ile kurmak gerekiyor (Apple kısıtı). Sayfa bunu adım adım anlatıyor.
- **Anahtarlar:** yerelde VAPID anahtarları otomatik üretilip `%LOCALAPPDATA%\PreBot` klasöründe tutulur. Üretimde `Push:PublicKey`, `Push:PrivateKey` ve `Push:Subject` ayarlanmalı.

**Veriler nerede?** Yerelde veritabanı ve anahtarlar `%LOCALAPPDATA%\PreBot` klasöründe tutulur, proje klasöründe değil. Sebebi:
- OneDrive gibi senkronize klasörler SQLite'ın kilit dosyalarını bozuyor.
- Özel anahtarın buluta yüklenmemesi gerekiyor.

Bu konum `DataDirectory` ayarıyla değiştirilebilir.

### 🔑 Google ve Discord ile giriş

Bir sağlayıcı ancak anahtarları ayarlanınca giriş sayfasında görünür. Ayarlanmamışsa yerelde yalnızca test girişi çalışır. Sağlayıcılardan sadece hesap kimliği ve görünen ad alınır, **e-posta adresi istenmez** (KVKK).

**Google (yaklaşık 10 dakika):**
1. <https://console.cloud.google.com> adresinde yeni bir proje oluştur (ör. "PreBot").
2. **APIs & Services → OAuth consent screen** bölümünde:
   - User type olarak **External** seç.
   - Uygulama adına "PreBot" yaz, destek e-postası olarak kendi adresini gir.
   - Scope eklemene gerek yok.
3. **Credentials → Create credentials → OAuth client ID** bölümünde:
   - Application type olarak **Web application** seç.
   - **Authorized redirect URIs** alanına `http://localhost:5215/signin-google` ekle. Yayına alınca `https://ALANADI/signin-google` adresini de ekle.
4. Çıkan Client ID ve Client Secret'ı şu komutla kaydet:

```bash
powershell -ExecutionPolicy Bypass -File .\scripts\giris-ayarla.ps1
```

Komutta **1**'i seç. Secret ekranda görünmez.

**Discord (VPN'li kullanıcılar için, yaklaşık 3 dakika):**
1. <https://discord.com/developers/applications> → **PreBot** → **OAuth2** sayfasına git.
2. **Redirects** alanına `http://localhost:5215/signin-discord` ekle.
3. **Reset Secret** ile yeni bir secret al.
4. Aynı komutu çalıştır ve **2**'yi seç. Client ID hazır gelir, sadece secret'ı yapıştır.

### 🚀 Yayına alma (VPS + Docker + otomatik HTTPS)

Gerekenler:
- Linux bir VPS (ör. Hetzner, aylık yaklaşık 6 €)
- Bir alan adı (ör. `prebot.com.tr`)

1. Alan adının DNS ayarlarında, alan adını VPS'in IP adresine yönlendiren bir **A kaydı** ekle.
2. VPS'e Docker'ı kur:

```bash
curl -fsSL https://get.docker.com | sh
```

3. Projeyi VPS'e indir ve ayar dosyasını oluştur:

```bash
git clone <repo-adresi> prebot && cd prebot && cp deploy/.env.example .env
```

4. `.env` dosyasında en az `DOMAIN` ve `PUSH_SUBJECT` değerlerini doldur. Google ve Discord anahtarları isteğe bağlı. Google ve Discord konsollarına yönlendirme adresi (redirect URI) olarak `https://ALANADI/signin-google` ve `https://ALANADI/signin-discord` adreslerini de ekle.
5. Siteyi başlat:

```bash
docker compose up -d --build
```

Caddy, HTTPS sertifikasını Let's Encrypt'ten kendisi alır ve yeniler. Push anahtarları ilk açılışta üretilip `prebot-data` volume'unda saklanır. Discord botunu da çalıştırmak istersen `.env` dosyasına `DISCORD_BOT_TOKEN` (ve duyuru kanalı için `DISCORD_FEED_CHANNELS`) eklemen yeterli: bot sitenin içinde çalışır, ayrı bir komut gerekmez. (`--profile discord` yalnızca botu siteden bağımsız, tek başına çalıştırmak içindir.)

**Yedekleme:** veriler `prebot-data` volume'unda duruyor: veritabanı, push anahtarları ve oturum anahtarları. Volume'u düzenli olarak yedekle.

**CI:** `.github/workflows/ci.yml` her push'ta şunları yapar:
- uyarıları hata sayarak derler,
- unutulmuş bir veritabanı migration'ı olup olmadığını kontrol eder,
- bütün testleri çalıştırır,
- Docker imajlarını derler ve web imajını `/health` adresiyle dener.

### Veritabanı değişikliği yaparken

Bir entity'yi değiştirdikten sonra şu komutla yeni bir migration ekle:

```bash
dotnet ef migrations add <Isim> --project src/PreBot.Core --output-dir Data/Migrations
```

Site ve bot açılışta veritabanını kendiliğinden günceller. Migration eklemeyi unutursan testler ve CI hata verir.

### 🤖 Discord botu sitenin içinde (önerilen)

Bot ayrı bir program olarak da çalışabilir. Ama sitenin içinde çalıştırmak daha iyi, çünkü:
- Tek süreç ve tek veritabanı kullanılır.
- **Sitede açılan her ilan**, seçtiğin Discord kanallarına **"Sitede katıl"** bağlantısıyla duyurulur. Kimse etiketlenmez; bu bir bildirim değil, duyuru.

Discord'u kullanmaya devam eden oyuncular böylece siteye yönlenir. Bütün slash komutları da aynen çalışır.

1. Token'ı sitenin ayarlarına kaydet:

```bash
powershell -ExecutionPolicy Bypass -File .\scripts\token-ayarla.ps1 -Proje web
```

2. Duyuru kanalını ayarla. Discord'da Geliştirici Modu açıkken kanala sağ tıkla, **Kanal Kimliğini Kopyala**'yı seç ve şunu çalıştır:

```bash
dotnet user-secrets set "Discord:FeedChannels" "KANAL_ID" --project src/PreBot.Web
```

3. Yayında `.env` dosyasına `DISCORD_BOT_TOKEN` yazman yeterli. Duyuru kanalı için `DISCORD_FEED_CHANNELS` (birden fazlaysa virgülle) ayarı ekle. Bağlantılar `https://ALANADI` adresine kendiliğinden yönlenir.

Botu token’ı silmeden geçici olarak kapatmak için `Discord:Enabled=false` (ortam değişkeni: `Discord__Enabled=false`) kullan.

Site ile botun bildirim dalgaları birbirine karışmaz: site yalnızca kendi ilanlarını, bot yalnızca Discord sunucularındaki ilanları işler.

## Discord botu komutları

| Komut | Ne yapar |
|---|---|
| `/musaitim` | Havuza girersin (rol, rank, 30 dk / 1 saat / 2 saat) |
| `/musait-degilim` | Havuzdan çıkarsın |
| `/havuz` | Şu an kaç kişinin müsait olduğunu rol rol gösterir |
| `/pre-lazim` | Eksik kişi için ilan açar (rol, rank aralığı, 1–3 kişi, not) |
| `/profil` | Riot ID'ni kaydeder (ör. `Oyuncu#TR1`); katıldığın ilanda ilan sahibine gösterilir |
| `/gizlilik` | Hangi verilerin saklandığını gösterir |
| `/verilerimi-sil` | Tüm verilerini siler |

İlan butonları: **Katıl**, **Vazgeç**, **Gelmedi, yeniden ara** (sadece ilan sahibi), **İlanı kapat** (sadece ilan sahibi).

## Kurulum (yaklaşık 10 dakika)

**Gerekenler:** [.NET 10 SDK](https://dotnet.microsoft.com/download) ve bir Discord sunucusu (test için kendi sunucun yeterli).

### 1. Discord'da bot oluştur
1. <https://discord.com/developers/applications> → **New Application** → bir isim ver (ör. PreBot).
2. **Bot** sekmesi → **Reset Token** → token'ı kopyala. Token'ı kimseyle paylaşma, git'e de koyma.
3. Ayrıcalıklı intent'lerin (Presence, Server Members, Message Content) **hiçbirini açmana gerek yok.**
4. **General Information** sekmesinden **Application ID**'yi kopyala.

### 2. Botu sunucuna davet et
Aşağıdaki adreste `APP_ID` yerine kendi Application ID'ni yaz ve tarayıcıda aç:

```
https://discord.com/oauth2/authorize?client_id=APP_ID&scope=bot+applications.commands&permissions=84992
```

Bu adresteki `84992` değeri yalnızca dört izin istiyor: Kanalları Görüntüle, Mesaj Gönder, Bağlantı Yerleştir, Mesaj Geçmişini Oku. Yönetici izni istemiyor.

### 3. Token'ı ve test sunucusunu ayarla
Discord'da **Ayarlar → Gelişmiş → Geliştirici Modu**'nu aç. Sonra sunucuna sağ tıkla → **Sunucu Kimliğini Kopyala**.

```bash
dotnet user-secrets set "Discord:Token" "BURAYA_TOKEN" --project src/PreBot
```

```bash
dotnet user-secrets set "Discord:GuildId" "BURAYA_SUNUCU_KIMLIGI" --project src/PreBot
```

`GuildId` ayarlanınca komutlar o sunucuda hemen görünür. Ayarlanmazsa komutlar tüm sunuculara kaydedilir ve görünmeleri biraz zaman alabilir.

### 4. Çalıştır

```bash
dotnet run --project src/PreBot
```

Konsolda `Slash commands registered to test server ...` satırını görünce Discord'da `/` yazıp komutları dene. Veritabanı (`prebot.db`) ilk çalıştırmada kendiliğinden oluşur.

### Testler

```bash
dotnet test
```

25 test var. Kapsadıkları:
- eşleştirme kuralları,
- aynı anda 12 kişi **Katıl**'a bastığında yalnızca birinin yeri alması,
- vazgeçme ve gelmeyen kişi akışı,
- süre dolması,
- veri silme,
- Discord komut tanımlarının geçerliliği.

Testler gerçek bir SQLite dosyası üzerinde çalışır, sahte bir bellek veritabanı kullanılmaz.

## Sunucuya kurma (sürekli açık kalsın diye)

Ucuz bir VPS'te (ör. Hetzner, aylık birkaç €) Docker ile:

```bash
echo "DISCORD_TOKEN=BURAYA_TOKEN" > .env
```

```bash
docker compose up -d --build
```

Veritabanı `prebot-data` volume'unda saklanır. İlk hafta botu kendi bilgisayarında `dotnet run` ile çalıştırman da yeterli.

## Mimari

```
src/PreBot.Core     Discord'dan bağımsız iş mantığı (test edilebilir)
  Domain/           Rol, rank, ilan, müsaitlik; Türkçe isimler; Riot ID biçim kontrolü
  Matching/         Saf eşleştirme kuralları: dalgalar, rank genişletme, sıralama, spam koruması
  Data/             EF Core + SQLite
  Services/         PreService: havuz, ilan, atomik yer alma, dalgalar, süre dolumu, KVKK silme
src/PreBot          Discord katmanı (Discord.Net 3.20.2, .NET Generic Host)
  Bot/PreModule     Slash komutları ve buton işleyicileri
  Bot/ListingRenderer / ListingNotifier   İlan mesajı, yenileme, hedefli etiketleme
  Bot/ExpiryWorker  20 saniyede bir: süresi dolanları kapatır, sırası gelen dalgaları gönderir
tests/PreBot.Tests  xUnit
```

**Önemli tasarım kararları** (gerekçeleri `arastirma/` klasöründe):

- **Yarış durumu.** "Son yeri iki kişi aynı anda aldı" sorunu tek bir koşullu `UPDATE ... WHERE SlotsFilled < SlotsNeeded` sorgusuyla çözülüyor. Ek olarak benzersiz indeks var. Aynı dalga, atomik bir sayaç sayesinde iki kez gönderilemiyor.
- **Süre dolumu.** Doğruluk zamanlayıcıya bağlı değil. Bütün sorgular `ExpiresAtUtc > şimdi` filtresini kendisi uyguluyor; zamanlayıcı yalnızca temizlik yapıyor ve mesajları güncelliyor.
- **Ayrıcalıklı intent yok.** Bot yalnızca `Guilds` intent'iyle çalışıyor. Bu yüzden 100 sunucu sınırında Discord'un ek onayına takılmıyor.
- **Riot API yok (şimdilik).** Rank, kullanıcının kendi beyanına dayanıyor. Riot'un politikası gereği bot kendi beceri puanını (MMR) hesaplamıyor, yalnızca resmi kademeleri kullanıyor.

## Veri ve gizlilik (KVKK)

Bot şunları saklıyor:
- Discord kullanıcı kimliği,
- seçilen rol ve rank,
- müsaitlik süresi,
- isteğe bağlı Riot ID,
- ilan kayıtları.

Müsaitlik kayıtları süresi dolunca siliniyor. Kullanıcı `/verilerimi-sil` ile her şeyi anında silebiliyor. Bot yayına alınmadan önce bir hukukçuyla kontrol edilmeli; bu bölüm hukuki tavsiye değildir.

## Sonraki adımlar

- [ ] Yayına al (VPS + alan adı) ve 3 akşam gerçek oyuncularla dene: `/istatistik` sayfasında hedef dolum ≥ %50, medyan dolma süresi ≤ 10 dk
- [x] Metrikler: `/istatistik` (dolum oranı, dolma süresi, bildirim sayısı, müsait kişi)
- [ ] Katılıp gelmeme ("Gelmedi") oranını istatistiğe ekle
- [ ] Riot doğrulaması: kişisel API anahtarıyla profil ikonu doğrulaması, gerçek Flex rankını çekme
- [ ] Kadro dolunca geçici ses kanalı açma
- [x] EF migrations
- [x] GitHub Actions ile otomatik derleme ve test (CI)

Güncel durum için: [docs/DURUM-RAPORU.md](docs/DURUM-RAPORU.md).

## Araştırma

Bu botun neden böyle tasarlandığını anlatan araştırma raporları `arastirma/` klasöründe. Önce şunu oku: [Pre bot en iyi seçenek analizi](arastirma/reports/Pre%20bot%20en%20iyi%20seçenek%20analizi.md).

## Lisans

[MIT](LICENSE). PreBot, Riot Games tarafından onaylanmamıştır. League of Legends, Riot Games, Inc.'in ticari markasıdır.
