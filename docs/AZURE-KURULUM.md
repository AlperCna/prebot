# PreBot'u Azure for Students'a kurmak (ücretsiz, kartsız)

Öğrenciler için Azure: **kredi kartı istemez**, okul e-postasıyla doğrulanır, **12 ay boyunca küçük bir sunucu ücretsiz** ve 100 $ kredi verir. Öğrenci olduğun sürece her yıl yenilenebilir. Kredi biterse fatura çıkmaz, abonelik durur.

Toplam süre: yaklaşık 20–30 dakika. Hesap ve sunucuyu sen açıyorsun (tıklamalar aşağıda), kurulum tek komut.

## 1. Hesap (senin işin, ~10 dk)

1. <https://azure.microsoft.com/free/students> → **Start free** (Ücretsiz başla).
2. **Okul e-postanla** giriş yap (ör. `ad.soyad@ogr.universite.edu.tr`). Okulun Microsoft hesabıyla girişi destekliyorsa doğrudan girersin; desteklemiyorsa yeni bir Microsoft hesabı açıp okul e-postasını doğrularsın.
3. Telefon doğrulaması ve öğrenci doğrulaması (okul e-postana gelen kod) yapılır. 18 yaşından büyük olmalısın.
4. Sonunda <https://portal.azure.com> açılır; abonelik adı **Azure for Students** görünür.

## 2. Sunucu (senin işin, ~10 dk)

Portalda üstteki arama kutusuna **Virtual machines** yaz → **Create** → **Azure virtual machine**.

**Basics sekmesi:**
- **Subscription:** Azure for Students
- **Resource group:** Create new → `prebot`
- **Virtual machine name:** `prebot`
- **Region:** Avrupa'da bir bölge: **West Europe (Hollanda)**, **North Europe (İrlanda)**, **Germany West Central**, **Sweden Central**, **Italy North** ya da **Poland Central**. Öğrenci aboneliği bazı bölgelere izin vermeyebilir; hata çıkarsa listeden başka bir Avrupa bölgesi seç.
- **Availability options:** No infrastructure redundancy required
- **Security type:** Standard
- **Image:** **Ubuntu Server 24.04 LTS – x64 Gen2**
- **Size:** **B2ats v2** (2 vCPU, 1 GB) ya da **B1s** (1 vCPU, 1 GB). Yanında **"Free services eligible"** yazan birini seç. ARM olan **B2pts v2**'yi seçme (PreBot'un hazır imajı x64).
- **Authentication type:** SSH public key
- **Username:** `azureuser`
- **SSH public key source:** Generate new key pair, **Key pair name:** `prebot_key`
- **Public inbound ports:** Allow selected ports → **HTTP (80)**, **HTTPS (443)**, **SSH (22)** üçünü de seç.

**Disks sekmesi:** OS disk type **Standard SSD** (varsayılan da olur). Boyut varsayılan kalabilir.

**Management sekmesi:** **Auto-shutdown** kapalı (Off) olmalı; açık kalırsa site her gece kapanır.

**Review + create** → **Create** → çıkan pencerede **Download private key and create resource**. İnen `prebot_key.pem` dosyasını kaybetme (sunucuya girmenin tek anahtarı).

## 3. Sabit adres (senin işin, 1 dk)

Sunucu oluşunca **Go to resource** → **Overview** sayfasında **DNS name: Not configured** yazısına tıkla → **DNS name label** kutusuna bir ad yaz (ör. `prebot-alper`) → **Save**.

Adresin: `prebot-alper.westeurope.cloudapp.azure.com` (bölgene göre ortası değişir). Bu adres ücretsiz ve **sabit**; HTTPS sertifikasını sunucudaki Caddy kendisi alır.

## 4. Kurulum (tek komut)

Bilgisayarında PowerShell'de, anahtar dosyasının izinlerini düzelt (Windows'ta SSH bunu ister):

```bash
icacls "$env:USERPROFILE\Downloads\prebot_key.pem" /inheritance:r /grant:r "$($env:USERNAME):R"
```

Sunucuya bağlan (adresi kendi adresinle değiştir):

```bash
ssh -i "$env:USERPROFILE\Downloads\prebot_key.pem" azureuser@prebot-alper.westeurope.cloudapp.azure.com
```

"Are you sure you want to continue connecting" sorusuna `yes` yaz. Sunucuya girince:

```bash
curl -fsSL https://raw.githubusercontent.com/AlperCna/prebot/master/deploy/sunucu-kurulum.sh -o kurulum.sh && sh kurulum.sh
```

Komut sırayla sorar: sitenin adresi (3. adımdaki adres), iletişim e-postan, işletmeci adı (gizlilik metninde görünür), sunucunun yeri (ör. `Avrupa (Azure, Hollanda)`) ve davet kodu. Sonra Docker'ı kurar, 1 GB RAM için takas alanı açar, GitHub'daki hazır imajı indirir ve siteyi başlatır. 1–2 dakika içinde `https://ADRESIN` açılır.

## 5. Sonrası

- **Güncelleme:** sunucuda aynı komutu tekrar çalıştırman yeterli (ayarların ve verilerin korunur).
- **Yönetici sayfası:** siteye gir, **Profil**'deki kullanıcı numaranı al, sunucuda `nano prebot/.env` ile `SITE_ADMIN_USER_IDS=NUMARAN` yaz, sonra `cd prebot && sudo docker compose up -d --no-build`.
- **Yedek:** site her gece 05:00'te kendi yedeğini alır (README → Yedekleme).
- **Bilgisayardaki deneme sitesi:** Azure'daki site çalışınca durdurabilirsin (`scripts\deneme-durdur.ps1`). Arkadaşların yeni adrese girip bildirimleri bir kez yeniden açmalı.

## Maliyet (öğrenci kredisi 100 $)

- Sunucu (B2ats v2 / B1s): 12 ay boyunca ayda 750 saat ücretsiz, yani sürekli açık bir sunucu bedava.
- Genel IP adresi ve küçük ek kalemler 100 $ kredinin içinden düşer; kredi yıl boyunca yeter. Kredi biterse **fatura çıkmaz**, abonelik durur.
- Portalda **Cost Management + Billing** sayfasından kalan krediyi görebilirsin.
