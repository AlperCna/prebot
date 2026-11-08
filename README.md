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

## Komutlar

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

- [ ] 3 akşam deneme: dolum oranını ve ortalama bekleme süresini ölç (hedef: dolum ≥ %50, medyan bekleme ≤ 10 dk)
- [ ] Metrikleri kaydet: ilanın dolma süresi, havuz büyüklüğü, katılıp gelmeme oranı
- [ ] Riot doğrulaması: kişisel API anahtarıyla profil ikonu doğrulaması, gerçek Flex rankını çekme
- [ ] Kadro dolunca geçici ses kanalı açma
- [ ] Şemayı değiştirmeden önce `EnsureCreated` yerine EF migrations'a geçiş
- [ ] GitHub Actions ile otomatik derleme ve test (CI)

## Araştırma

Bu botun neden böyle tasarlandığını anlatan araştırma raporları `arastirma/` klasöründe. Önce şunu oku: [Pre bot en iyi seçenek analizi](arastirma/reports/Pre%20bot%20en%20iyi%20seçenek%20analizi.md).
