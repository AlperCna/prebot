# PreBot: Flex için anında 5. kişi

**Ranked Flex'e 4 kişiyle girilemiyor.** Riot yalnızca 1, 2, 3 ya da 5 kişilik partilere izin veriyor; dört kişi kaldığında ya 5.'yi bulacaksınız ya da dereceli oynamayacaksınız.

PreBot, League of Legends TR sunucusu için bu sorunu çözen ücretsiz ve açık kaynaklı bir **web uygulaması (PWA)**. İlan panosu gibi beklemez: **şu an müsait olan ve rolü uyan** oyunculara telefonlarına ve bilgisayarlarına anında bildirim gönderir, ilk "Katıl" diyen yeri alır, Riot ID'si ilan sahibine gider. Discord botu da sitenin içinde çalışır ve ilanları Discord kanallarına duyurur.

> PreBot isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing Riot Games properties. Riot Games, and all associated properties are trademarks or registered trademarks of Riot Games, Inc.

## Nasıl çalışır

```
Oyuncu:   "Şu an müsaitim" der (rol, rank, 30 dk – 2 saat) ya da "Bana haber ver" ile saat aralığı kurar
Sen:      İlan açarsın: "Destek lazım" (rank aralığı varsayılan olarak geniş: Demir–Elmas)
PreBot:   Uygun oyunculara dalga dalga push bildirimi gönderir:
            0. dk → en uygun 5 kişi
            2. dk → 10 kişi daha (rank aralığı ±1 kademe)
            5. dk → 15 kişi daha (±2 kademe)
          İlk dalga boşsa ve geniş dalgada uygun biri varsa beklemeden ona geçer.
Oyuncu:   Bildirimdeki "Katıl"a basar → yer onundur (kişisel, 20 dakikalık bağlantı)
Sen:      "🎉 X katıldı, Riot ID: ..." bildirimi alırsın → Riot ID'ye dokun (kopyalanır) → oyunda ekle, davet et
İlan:     15 dakikada dolmazsa kapanır; "Aynı ilanı tekrar aç" tek tuş
Gelmedi:  "Gelmedi, yeniden ara" → yer boşalır, yeniden aranır
```

| Sayfa | Ne yapar |
|---|---|
| `/havuz` | "Şu an müsaitim", canlı havuz, açık ilanlar, başlarken listesi |
| `/ilan-ac` | Eksik kişi için ilan açar |
| `/ilan/{id}` | Katıl, Vazgeç, Gelmedi-yeniden ara, Kapat, Tekrar aç, paylaşılabilir link. Riot ID'leri yalnızca ilandakiler görür. |
| `/bildirimler` | Bu cihazda bildirimleri aç/kapat, test bildirimi, "Bana haber ver" ayarı |
| `/profil` | Görünen isim, Riot ID, çıkış, verilerimi sil |
| `/istatistik` | Herkese açık, anonim: dolum oranı, tipik ve p90 dolma süresi, gelmeme oranı |
| `/gizlilik`, `/kosullar`, `/iletisim` | KVKK aydınlatma metni, kullanım koşulları, iletişim ve şikâyet |

Bütün sayfalar canlı: biri katılınca diğer ekranlar yenilemeye gerek kalmadan güncellenir (Blazor Interactive Server, SignalR).

### Bildirimler ve spam koruması

- Aynı ilan için kimseye iki kez haber gitmez. Müsait oyunculara en fazla 3 dakikada bir bildirim gider (`PreBot:PingCooldownMinutes`).
- "Bana haber ver" diyenlere: önce müsait olanlara sorulur; onlara 2. dalgadan itibaren ya da müsait kimse uymuyorsa hemen gider. Kişi başına 30 dakikada 1, günde en fazla 5 bildirim.
- Bildirim 120 saniyede geçersiz olur; grup dolduktan sonra gelmez.
- Kullanıcı başına sınırlar: saatte en fazla 6 ilan, dakikada 6 katılma, 10 dakikada 3 test bildirimi. Giriş uç noktalarına IP başına sınır.
- **iPhone:** bildirim için siteyi Safari'de **Paylaş → Ana Ekrana Ekle** ile kurmak gerekiyor (Apple kısıtı). Sayfa bunu adım adım anlatıyor.

## Yerelde çalıştırma

**Gerekenler:** [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/PreBot.Web --launch-profile http
```

<http://localhost:5215> adresini aç. Yerelde **test girişi** var: herhangi bir isimle girebilirsin. İki oyuncuyu aynı anda denemek için bir sekmede `localhost:5215`, diğerinde `127.0.0.1:5215` aç.

Yerelde veritabanı ve anahtarlar `%LOCALAPPDATA%\PreBot` klasöründe tutulur, proje klasöründe değil (OneDrive gibi senkron klasörler SQLite'ı bozuyor, anahtarlar da buluta yüklenmemeli). Konum `DataDirectory` ayarıyla değiştirilebilir. Siteyi Discord botu kapalı denemek için: `Discord__Enabled=false`.

### Google ve Discord ile giriş

Bir sağlayıcı ancak anahtarları ayarlanınca giriş sayfasında görünür. Sağlayıcılardan yalnızca hesap kimliği ve görünen ad alınır, **e-posta istenmez**. Giriş, "Aydınlatma metnini okudum, Kullanım Koşulları'nı kabul ediyorum ve 16 yaşından büyüğüm" kutucuğu işaretlenmeden başlamaz.

**Google (yaklaşık 10 dakika):**
1. <https://console.cloud.google.com> → yeni proje (ör. "PreBot").
2. **APIs & Services → OAuth consent screen:** External, uygulama adı "PreBot", destek e-postası. Scope eklemeye gerek yok. Yayına alınca **Publish app** yap; yoksa yalnızca test kullanıcıları girebilir.
3. **Credentials → Create credentials → OAuth client ID:** Web application. **Authorized redirect URIs:** `http://localhost:5215/signin-google` ve yayında `https://ALANADI/signin-google`.
4. Client ID ve Secret'ı kaydet (secret ekranda görünmez):

```bash
powershell -ExecutionPolicy Bypass -File .\scripts\giris-ayarla.ps1
```

**Discord (VPN'li kullanıcılar için):** <https://discord.com/developers/applications> → uygulaman → **OAuth2** → **Redirects** alanına `http://localhost:5215/signin-discord` (yayında `https://ALANADI/signin-discord`) ekle, **Reset Secret** ile secret al, aynı komutta **2**'yi seç.

### Discord botu (sitenin içinde)

Token ayarlıysa bot sitenin içinde çalışır: tek süreç, tek veritabanı. Sitede açılan her ilan seçtiğin kanallara **"Sitede katıl"** bağlantısıyla duyurulur (kimse etiketlenmez) ve ilan dolunca/kapanınca duyuru güncellenir. Slash komutları (`/musaitim`, `/musait-degilim`, `/havuz`, `/pre-lazim`, `/profil`, `/gizlilik`, `/verilerimi-sil`) aynen çalışır.

1. <https://discord.com/developers/applications> → **Bot → Reset Token**. Ayrıcalıklı intent gerekmez. Davet bağlantısı (`APP_ID` yerine Application ID): `https://discord.com/oauth2/authorize?client_id=APP_ID&scope=bot+applications.commands&permissions=84992` (yalnızca kanal görme, mesaj gönderme, bağlantı yerleştirme, geçmiş okuma).
2. Token'ı kaydet (ekranda görünmez):

```bash
powershell -ExecutionPolicy Bypass -File .\scripts\token-ayarla.ps1 -Proje web
```

3. Duyuru kanalı: Discord'da Geliştirici Modu açıkken kanala sağ tık → **Kanal Kimliğini Kopyala**:

```bash
dotnet user-secrets set "Discord:FeedChannels" "KANAL_ID" --project src/PreBot.Web
```

Botu token'ı silmeden kapatmak için `Discord:Enabled=false`. Botu siteden bağımsız çalıştırmak istersen `src/PreBot` projesi (ve compose'daki `discord` profili) hâlâ var.

## Arkadaşlarla ücretsiz deneme (sunucu almadan)

Site kendi bilgisayarında "yayın modunda" çalışır, Cloudflare'in hesapsız Quick Tunnel'ı ile internetten HTTPS adresi alır. Giriş, Google ayarı gerektirmeyen **isim + davet kodu** ile yapılır. Discord botu kapalıdır.

1. İlk seferde `cloudflared`'i indirmek için tüneli bir kez elle aç (Cloudflare'in resmi GitHub sayfasından indirilir, imzası kontrol edilir), sonra Ctrl+C ile kapat:

```bash
powershell -ExecutionPolicy Bypass -File .\scripts\deneme-tuneli.ps1
```

2. Sonraki her seferde tek komut: site ve tünel arka planda açılır, adres ve arkadaşlara gönderilecek hazır mesaj ekrana yazılır (ayrıca `%LOCALAPPDATA%\PreBot-deneme\davet-mesaji.txt`):

```bash
powershell -ExecutionPolicy Bypass -File .\scripts\deneme-baslat.ps1
```

3. Durdurmak için:

```bash
powershell -ExecutionPolicy Bypass -File .\scripts\deneme-durdur.ps1
```

Bilgisayarın açık kalmalı. Tünel adresi her açılışta değişir; adres değişince herkes bildirimleri yeniden açmalı. Veriler `%LOCALAPPDATA%\PreBot-deneme` klasöründe, geliştirme verilerinden ayrı tutulur. Davet kodunu bilen herkes istediği isimle girebilir; bu yüzden yalnızca kapalı arkadaş denemesi içindir.

Deneme tutarsa ücretsiz kalıcı sunucu için: öğrenciysen [docs/AZURE-KURULUM.md](docs/AZURE-KURULUM.md) (Azure for Students, kartsız, sabit adres dahil); diğer seçenekler [arastirma/reports/Ücretsiz sunucu alternatifleri.md](arastirma/reports/%C3%9Ccretsiz%20sunucu%20alternatifleri.md) ve [docs/UCRETSIZ-SUNUCU.md](docs/UCRETSIZ-SUNUCU.md).

## Yayına alma (VPS + Docker + otomatik HTTPS)

Gerekenler: küçük bir Linux VPS (2 vCPU / 4 GB yeterli) ve bir alan adı. Alan adında "LoL" ya da "League" geçmemeli (Riot kuralı).

1. DNS'te alan adını VPS'in IP'sine yönlendiren bir **A kaydı** ekle.
2. VPS'e Docker kur:

```bash
curl -fsSL https://get.docker.com | sh
```

3. Projeyi indir ve ayar dosyasını oluştur:

```bash
git clone https://github.com/AlperCna/prebot.git prebot && cd prebot && cp deploy/.env.example .env
```

4. `.env` dosyasını doldur. **Zorunlu:** `DOMAIN`, `PUSH_SUBJECT`, `SITE_OPERATOR_NAME`, `SITE_CONTACT_EMAIL` (aydınlatma metninde veri sorumlusu ve iletişim olarak gösterilir), `SITE_HOSTING_LOCATION`. İsteğe bağlı: Google/Discord anahtarları, `DISCORD_BOT_TOKEN`, `DISCORD_FEED_CHANNELS`, `BACKUP_HEARTBEAT_URL`, `RIOT_VERIFICATION_CODE`.
5. Başlat:

```bash
docker compose up -d --build
```

Caddy HTTPS sertifikasını kendisi alır ve yeniler, güvenlik başlıklarını (HSTS vb.) ekler. Uygulama `X-Forwarded-*` başlıklarını yalnızca Caddy'nin sabit Docker ağından (`172.30.0.0/24`) kabul eder; böylece hız sınırları gerçek IP'yi görür. Push ve oturum anahtarları ilk açılışta üretilip `prebot-data` volume'unda saklanır.

### Yedekleme

Site her gece **05:00'te (TR saati)** kendi yedeğini alır: tutarlı bir veritabanı kopyası (`VACUUM INTO`), oturum anahtarları ve push (VAPID) anahtarı `/data/backups/YYYY-MM-DD/` altına yazılır. Son 7 gün, son 4 pazar ve 6 aylık ay başları saklanır. Aynı iş, 90 günden eski kapanmış ilanları siler (istatistikler anonim olay günlüğünde kalır).

Yedeği sunucu dışına şifreli kopyalamak için [`deploy/backup-offsite.sh`](deploy/backup-offsite.sh) betiğini host'un cron'una ekle (rclone + crypt; kurulum betiğin içinde). `BACKUP_HEARTBEAT_URL` (ör. Healthchecks.io) ayarlanırsa her başarılı yedekte ping atılır; ping gelmezse e-posta alırsın.

**Geri yükleme:** siteyi durdur, `prebot.db` dosyasını `/data/prebot-web.db` olarak, `keys/` klasörünü `/data/keys/`, `vapid-keys.json` dosyasını `/data/` altına kopyala, siteyi başlat. Bunu yayından önce bir kez dene.

### İzleme

- `/health` (veritabanı dahil), `/health/live` (süreç ayakta mı), `/health/ready`.
- Loglar üretimde satır başına bir JSON nesnesi; konteyner logları 10 MB × 5 dosya döner. Push sonuçları ve reddedilme kodları (413, 429) loglanır.
- Dış izleme için UptimeRobot ya da Better Stack'in ücretsiz katmanıyla `https://ALANADI/health` adresini izle (aynı VPS'teki bir izleyici VPS çökünce haber veremez).

### Riot API

Riot ID ve rank şu an kullanıcının beyanı, doğrulanmıyor. Doğrulama için site yayına girince Riot Developer Portal'dan **üretim anahtarına** başvur. Kişisel anahtar herkese açık sitelerde (açık beta dahil) kullanılamaz; yalnızca kapalı bir geliştirme grubunda denenebilir. Portal'ın verdiği doğrulama kodunu `RIOT_VERIFICATION_CODE` olarak ayarla; site onu `https://ALANADI/riot.txt` adresinde sunar.

## Geliştirme

```bash
dotnet test
```

130'dan fazla test var: eşleştirme ve dalga kuralları, aynı anda 12 kişi **Katıl**'a bastığında yalnızca birinin yeri alması, vazgeçme ve gelmeme akışı, süre dolması, hız sınırları, kişisel katılma bağlantıları, istatistikler, yedek, veri silme ve saklama, giriş ve kabul adımı, güvenlik başlıkları, Discord komut tanımları. Testler gerçek SQLite dosyası üzerinde çalışır.

Gerçek tarayıcıyla uçtan uca push testi (Edge/Chrome, çalışan siteye karşı, Discord botu kapalı):

```bash
dotnet run --project tools/PreBot.PushE2E -- http://localhost:5215
```

**Veritabanı değişikliği:** entity'yi değiştirdikten sonra migration ekle. Site açılışta veritabanını kendiliğinden günceller; migration unutulursa testler ve CI hata verir.

```bash
dotnet ef migrations add <Isim> --project src/PreBot.Core --output-dir Data/Migrations
```

**CI** (`.github/workflows/ci.yml`) her push'ta: gizli bilgi taraması (gitleaks), uyarıları hata sayan derleme, migration kontrolü, testler, Docker imajları ve `/health` denemesi. Dependabot NuGet, GitHub Actions ve Docker güncellemelerini haftalık PR olarak açar.

## Mimari

```
src/PreBot.Core      İş mantığı (Discord'dan ve web'den bağımsız, test edilebilir)
  Domain/            Rol, rank, ilan, müsaitlik, olay günlüğü; Türkçe isimler; Riot ID biçim kontrolü
  Matching/          Saf eşleştirme kuralları: dalgalar, rank genişletme, sıralama
  Data/              EF Core 10 + SQLite (WAL), migration'lar
  Services/          PreService (havuz, ilan, atomik yer alma, dalgalar, süre dolumu, KVKK silme ve saklama),
                     ActionLimiter, StatsService, AccountService, SubscriptionService
src/PreBot.Web       Blazor Web App (asıl ürün): sayfalar, push, giriş, yedek, Discord duyurusu
src/PreBot.Discord   Discord botu kütüphanesi (Discord.Net): slash komutları, butonlar, duyurular
src/PreBot           Botu siteden bağımsız çalıştırmak için ince bir host
tests/               xUnit: PreBot.Tests (çekirdek), PreBot.Web.Tests (WebApplicationFactory)
tools/PreBot.PushE2E Gerçek tarayıcıyla uçtan uca push testi
deploy/              Caddyfile, .env örneği, sunucu dışı yedek betiği
```

**Önemli tasarım kararları:**

- **Yarış durumu:** "son yeri iki kişi aynı anda aldı" sorunu tek bir koşullu `UPDATE ... WHERE SlotsFilled < SlotsNeeded` ile çözülüyor; aynı dalga atomik bir sayaçla iki kez gönderilemiyor.
- **Süre dolumu zamanlayıcıya bağlı değil:** bütün sorgular `ExpiresAtUtc > şimdi` filtresini kendisi uyguluyor; zamanlayıcı yalnızca temizlik yapıyor.
- **İstatistikler anonim olay günlüğünden:** kullanıcı kimliği tutulmuyor; hesap silinince geçmiş değişmiyor.
- **Rank beyanı:** PreBot kendi beceri puanını (MMR) hesaplamıyor; yalnızca resmi kademeleri kullanıyor (Riot politikası).
- **Ayrıcalıklı Discord intent'i yok:** bot yalnızca `Guilds` intent'iyle çalışıyor.

## Veri ve gizlilik (KVKK)

Hangi verilerin, hangi amaçla ve ne kadar süre saklandığı sitedeki [aydınlatma metninde](src/PreBot.Web/Components/Pages/Privacy.razor) (`/gizlilik`) yazıyor. Kısaca: hesap kimliği ve görünen ad, isteğe bağlı Riot ID, müsaitlik (süresi dolunca silinir), ilanlar (kapandıktan 90 gün sonra silinir), push abonelikleri ve güvenlik logları. E-posta, konum ya da ödeme bilgisi alınmıyor. Kullanıcı **Profil → Verilerimi sil** (Discord'da `/verilerimi-sil`) ile her şeyi anında silebiliyor. Bu bölüm hukuki tavsiye değildir; yurt dışı aktarımlar (KVKK md. 9) için bir hukukçuya danışılması önerilir.

## Yol haritası

Güncel plan ve kararlar `arastirma/` klasöründe:

- [PreBot kesin karar](arastirma/reports/PreBot%20kesin%20karar.md): sorunu çözüyor mu, talep ve rakipler, devam/dur eşiği.
- [PreBot geliştirme yol haritası](arastirma/reports/PreBot%20geliştirme%20yol%20haritası.md): Faz 0–3 planı.
- [Durum raporu](docs/DURUM-RAPORU.md).

Sıradaki büyük işler: yayına alma ve ilk kullanıcılar, Discord ile web'in tek havuzda birleşmesi, parti boyutu yardımcısı, bildirim hijyeni (sessize alma, sessiz saatler), engelleme ve şikâyet, Riot doğrulaması (üretim anahtarı gelince).

## Lisans

[MIT](LICENSE).
