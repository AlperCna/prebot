# PreBot'u ücretsiz sunucuya kurmak (Oracle Cloud Always Free)

Bilgisayardaki deneme işe yararsa PreBot'u 7/24 açık, kalıcı bir adrese taşımak için. Para ödemeden: Oracle Cloud'un "Always Free" sunucusu ve DuckDNS'in ücretsiz alt alan adı. Toplam süre yaklaşık 20–30 dakika. Sunucuyu kuran komutlar hazır; senin yapacakların hesap açmak ve birkaç tıklama.

> **Bilmen gerekenler (2026):**
> - Oracle kayıtta **kredi kartı doğrulaması** istiyor. "Always Free" kotasında kaldıkça ücret alınmıyor.
> - Ücretsiz ARM kotası 2026'da düşürüldü (raporlara göre 2 çekirdek / 12 GB). PreBot için fazlasıyla yeterli.
> - Bazı bölgelerde ücretsiz ARM sunucu için "kapasite yok" hatası çıkabiliyor; bir süre sonra tekrar dene ya da x86 "E2.1.Micro" seç.
> - Oracle çok az kullanılan ücretsiz sunucuları geri alabildiğini söylüyor. Böyle bir şey olursa yedekten geri kurmak kolay (README → Yedekleme).

## 1. Oracle hesabı ve sunucu (senin işin, ~15 dk)

1. <https://www.oracle.com/cloud/free/> → **Start for free**. Ev bölgesi (home region) olarak Avrupa'da bir bölge seç (ör. Frankfurt). Bu sonradan değişmez.
2. Konsolda **Compute → Instances → Create instance**:
   - **Image:** Canonical **Ubuntu 24.04**
   - **Shape:** Ampere **VM.Standard.A1.Flex**, 1 OCPU / 6 GB yeterli. Hata verirse **VM.Standard.E2.1.Micro**.
   - **Add SSH keys:** "Generate a key pair" → **özel anahtarı indir** (kaybetme).
   - **Create**. Birkaç dakika sonra **Public IP address** görünür; not al.
3. Aynı sayfada **Subnet** bağlantısı → **Default Security List** → **Add Ingress Rules**:
   - Source CIDR `0.0.0.0/0`, IP Protocol **TCP**, Destination Port **80**
   - Aynısını **443** için ekle.

## 2. Ücretsiz alan adı (senin işin, ~3 dk)

1. <https://www.duckdns.org> → GitHub ya da Google ile giriş.
2. Bir ad seç (ör. `prebot-tr`) → **add domain**.
3. **current ip** kutusuna Oracle'daki **Public IP**'yi yaz → **update ip**.

Adresin: `https://prebot-tr.duckdns.org` (HTTPS sertifikasını sunucudaki Caddy kendisi alır).

## 3. Sunucuya kurulum (tek komut)

Bilgisayarında PowerShell'de (anahtar dosyasının yolunu ve IP'yi kendine göre değiştir):

```bash
ssh -i C:\Users\Alper\Downloads\ssh-key.key ubuntu@SUNUCU_IP
```

Sunucuda:

```bash
curl -fsSL https://raw.githubusercontent.com/AlperCna/prebot/master/deploy/sunucu-kurulum.sh -o kurulum.sh && sh kurulum.sh
```

Komut şunları sorar: alan adı (`prebot-tr.duckdns.org`), iletişim e-postası, işletmeci adı (gizlilik metninde görünür) ve davet kodu. Sonra Docker'ı kurar, portları açar ve siteyi başlatır. Birkaç dakika sonra site `https://ALANADI` adresinde açılır.

Giriş, bilgisayardaki denemedeki gibi **isim + davet kodu** ile yapılır; Google ayarı gerekmez. Kalıcı ve herkese açık kullanım için ileride Google girişini eklemek istersen README'deki "Google ve Discord ile giriş" bölümüne bak (DuckDNS adresinin Google'da kabul edilip edilmediği doğrulanmadı).

## 4. Güncelleme

Yeni bir sürüm çıkınca sunucuda:

```bash
cd prebot && git pull && sudo docker compose up -d --build
```

## 5. Yedek

Site her gece 05:00'te kendi yedeğini sunucuya alır. Sunucu dışına kopyalamak için README → "Yedekleme" (rclone). Ücretsiz bir seçenek olarak Oracle'ın kendi "Object Storage" ücretsiz kotası (20 GB) kullanılabilir.
