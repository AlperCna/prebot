# PreBot durum raporu (9 Kasım 2026)

**Sorun:** TR sunucusunda Flex oynuyoruz, 4 kişiyiz, hemen bir 5. kişi lazım.

**Kısa cevap:** Yazılım tarafında sorunu çözen akışın tamamı çalışıyor ve gerçek tarayıcıda baştan sona denendi. Sorunu gerçekten çözüp çözmediği artık koda değil **kullanıcıya** bağlı. Havuzda akşamları yeterince insan olursa ilanlar dakikalar içinde doluyor. Bunu ölçmek için `/istatistik` sayfası eklendi. Sıradaki adım siteyi yayına alıp gerçek oyuncularla denemek; bu adımı senin yapman gerekiyor (aşağıda).

## Güncelleme: Faz 0 (yayın öncesi güvenlik ve ölçüm) tamamlandı

[Yol haritasındaki](../arastirma/reports/PreBot%20geliştirme%20yol%20haritası.md) Faz 0'ın kod tarafı bitti:

| Madde | Yapılan |
|---|---|
| F0.1 Sızan anahtar | Data Protection anahtarı depodan çıkarıldı. `keys/` git ve Docker dışında. CI'da gitleaks taraması, Dependabot. |
| Eşleştirme | Boş dalga 2 dakika bekletmiyor (genişletilmiş dalgada uygun biri varsa hemen gidiyor). İlan formu varsayılan olarak Demir–Elmas. |
| F0.2 `?katil=1` | Bildirimdeki Katıl bağlantısı kişiye ve ilana bağlı, 20 dakikalık şifreli token taşıyor. Kopyalanan bağlantı kimseyi oturtmuyor. |
| F0.3 Proxy | `X-Forwarded-*` yalnızca Caddy'nin sabit Docker ağından kabul ediliyor. |
| F0.4 Hız sınırı | IP başına genel ve giriş sınırı (429 + Retry-After). Kullanıcı başına ilan (6/saat), katılma (6/dk), test bildirimi (3/10 dk). |
| F0.5 Yedek | Her gece 05:00'te veritabanı + oturum anahtarları + push anahtarı. 7 gün / 4 pazar / 6 ay saklama, heartbeat, sunucu dışı rclone betiği. |
| F0.6 Uyum | Yeni KVKK aydınlatma metni (veri sorumlusu, saklama süreleri, push verisi, yurt dışı aktarımlar, çerezler). Girişte "okudum, kabul ediyorum, 16+" kutucuğu. `/iletisim` sayfası. Kapanan ilanlar 90 gün sonra siliniyor. |
| F0.7 README | Baştan yazıldı; Riot maddesi düzeltildi (üretim anahtarı). |
| F0.8 Sıkılaştırma | Güvenlik başlıkları (Caddy + uygulama), rapor modunda CSP, çıkış artık antiforgery'li POST, `AllowedHosts`, Caddy sürümü sabit. |
| F0.9 İzleme | JSON loglar, log rotasyonu, `/health/live` ve `/health/ready`, push sonuç logları. |
| F0.10 Ölçüm | Kimliksiz olay günlüğü; istatistikler hesap silinince değişmiyor. Gelmeme ve vazgeçme ayrı; p90 dolma süresi ve gelmeme oranı `/istatistik`'te. |
| F0.11 Rank etiketi | Rankların beyan olduğu yazıyor; ilan formunda Flex parti kuralı. |
| F0.12 (kod kısmı) | Riot alan adı doğrulaması için `/riot.txt` (`RIOT_VERIFICATION_CODE`). |

**Doğrulama:** 132 otomatik test, uyarıları hata sayan Release derleme, Docker imajı `/health` denemesi ve gerçek Edge ile 9 adımlı push E2E testi (kopyalanan bağlantının reddedilmesi dahil) geçti.

**Sende kalanlar (değişmedi):** VPS ve alan adı, `.env` doldurmak (yeni zorunlu alanlar: `SITE_OPERATOR_NAME`, `SITE_CONTACT_EMAIL`, `SITE_HOSTING_LOCATION`), Google "Publish app", yedek için depolama ve Healthchecks.io hesabı, Riot üretim başvurusu, KVKK md. 9 için hukukçu görüşü. İsteğe bağlı: git geçmişindeki eski anahtarın temizlenmesi (üretim kendi anahtarını ürettiği için zorunlu değil).

## Akış: "4 kişiyiz, 1 kişi lazım" → oyunda

1. **Oyuncu B** (müsait olan): Siteye girer → "Şu an müsaitim" der (rol + rank). Ya da her akşam uğraşmamak için **"Bana haber ver"** ile saat aralığı kurar.
2. **Oyuncu A** (ilan sahibi): **İlan aç** → rol, rank aralığı, kaç kişi eksik → "İlanı aç ve haber ver".
3. Uygun oyunculara **site kapalıyken bile telefon/PC bildirimi** gider. Bildirimler dalga dalga gider (önce en uygun 5 kişi, sonra rank aralığı genişler). İlan ayrıca Discord kanalına "Sitede katıl" bağlantısıyla duyurulur.
4. B bildirimdeki **Katıl**'a basar → yer tek dokunuşta kapılır.
5. **A'ya bildirim gelir:** "🎉 B katıldı! Riot ID: …". A, Riot ID'ye dokunur (kopyalanır) → oyunda Arkadaş ekle → lobiye davet. Discord duyurusu "✅ Doldu" olarak güncellenir.
6. Kimse gelmezse ilan 15 dakikada kapanır, A'ya "süresi doldu" bildirimi gider. **"🔁 Aynı ilanı tekrar aç"** tek tuşla yeniden açar.

## Bu gece yapılan denetim ve düzeltmeler

Akışı "gerçekten işe yarar mı, kullanması kolay mı" diye baştan sona yürüdüm. Bulduğum eksikler ve yaptıklarım:

| Eksik | Neden önemliydi | Yapılan |
|---|---|---|
| İlan sahibine biri katılınca haber gitmiyordu | A sekmeyi kapatırsa katılanı göremiyor, B lobide bekleyip gidiyordu | Katılma, vazgeçme ve süre dolma anında ilan sahibine push bildirimi |
| Discord duyurusu ilan dolunca da "açık" kalıyordu | İnsanlar dolu ilana tıklayıp hayal kırıklığına uğrardı | Duyuru ilan dolunca, kapanınca ya da süresi bitince kendiliğinden güncelleniyor (gri, bağlantısız) |
| Riot ID profil sayfasına gömülüydü | Riot ID yoksa A, B'yi oyunda bulamaz; akışın tam kırıldığı yer | İlan açarken, ilanda ve havuzda **yerinde** Riot ID kutusu |
| Yeni kullanıcı ne yapacağını bilmiyordu | Bildirimleri açmayan kullanıcıya hiç haber gitmez | Havuzda "Başlarken" listesi: Riot ID → bildirimler → müsaitim / bana haber ver |
| Riot ID'yi elle yazmak gerekiyordu | `İsim#TR1` yazım hatası çok olur | Riot ID'ye dokununca kopyalanıyor |
| Süresi dolan ilan baştan girilmek zorundaydı | Akşam 2. denemede vazgeçme sebebi | "Aynı ilanı tekrar aç" düğmesi |
| İşe yarayıp yaramadığını ölçmenin yolu yoktu | Ölçemediğimiz şeyi iyileştiremeyiz | `/istatistik`: dolum oranı, tipik ve en hızlı dolma süresi, bildirim sayısı, müsait kişi |

Ayrıca: README'deki eski yayına alma notu (botun ayrı çalıştırılması) ve "sonraki adımlar" listesi güncellendi.

## Doğrulananlar

- **109/109 otomatik test** geçiyor (68 çekirdek + 41 web). Yeni testler: ilan sahibine bildirim (site kapalıyken, yüksek öncelik), Discord duyurusunun güncellenmesi, ilanı yeniden açma (yalnızca sahibi), istatistik hesapları.
- **Gerçek tarayıcı testi (Edge, gerçek Windows push servisi):** giriş → bildirimleri aç → müsaitim → başka hesap ilan açar → push gelir → bildirimdeki Katıl yeri kapar → ilan sahibinin ekranı canlı güncellenir. **8/8 adım başarılı.**
- **Telefon genişliğinde (375 px) ekran kontrolü:** havuz, ilan açma, bildirimler, istatistik sayfaları düzgün.
- Uyarıları hata sayan Release derlemesi temiz, unutulmuş veritabanı migration'ı yok.
- GitHub'a gönderildi (`master`). CI derleme, test ve Docker imajı + `/health` denemesini her push'ta yapıyor.
- Bu gece Discord'a **hiçbir test mesajı gönderilmedi** (bütün denemeler bot kapalıyken yapıldı).

## Senin yapman gerekenler (sırayla)

Bunları ben yapamam: satın alma, hesap açma ve şartları kabul etme senin kararın.

1. **VPS kirala** (ör. Hetzner, aylık yaklaşık 6 €) ve **alan adı al** (ör. `prebot.com.tr`). DNS'te alan adını VPS'in IP'sine yönlendiren bir A kaydı ekle.
2. VPS'te: README → "Yayına alma" adımları. `.env` dosyasını doldur:
   - `DOMAIN`, `PUSH_SUBJECT` (`mailto:seninmailin`)
   - `GOOGLE_CLIENT_ID` / `GOOGLE_CLIENT_SECRET`
   - `DISCORD_BOT_TOKEN`, `DISCORD_FEED_CHANNELS` (duyuru kanalının kimliği)
3. **Google Cloud Console:**
   - Yetkili yönlendirme adreslerine `https://ALANADI/signin-google` ekle.
   - OAuth ekranında **"Publish app"** yap; yoksa yalnızca test kullanıcıları giriş yapabilir.
4. (İsteğe bağlı) Discord Developer Portal → OAuth2 → yönlendirme adresine `https://ALANADI/signin-discord` ekle.
5. (İsteğe bağlı) Daha önceki denemeden #bot-komut kanalında kalan 2 test duyurusunu sil.
6. **Arkadaşlarını davet et.** İlk hafta en az 15–20 kişi hedefle; akşam 20:00–00:00 arası havuzda 3–5 kişi olunca ilanlar dakikalar içinde dolar.

## Riskler ve dürüst notlar

- **En büyük risk kullanıcı sayısı.** Havuz boşsa bildirim gidecek kimse yok. Bu yüzden "Bana haber ver" var: insanlar her akşam siteye girmeden de uygun ilanlardan haberdar oluyor. Yine de ilk kullanıcıları sen ve arkadaş çevren getirmeli. Discord duyurusu, Discord'daki insanları siteye çekmek için.
- **iPhone:** Bildirimler için siteyi Safari'de **Ana Ekrana Ekle** gerekiyor (Apple kısıtlaması). Site bunu iPhone kullanıcılarına kendisi anlatıyor. Android ve PC'de doğrudan çalışıyor.
- **Riot API bağlı değil.** Rank ve Riot ID kullanıcının beyanı. Gerçek doğrulama için Riot'tan üretim API anahtarı gerekiyor (başvuru, onay süreci). Arkadaş çevresi için şart değil; büyürse sıradaki iş bu.
- **Gelmeyen oyuncu:** "Gelmedi, yeniden ara" düğmesi var ve onu ilandan çıkarıp yeniden arıyor. Ama "gelmeme oranı" henüz istatistikte yok.

## Başarı ölçütü

Yayına aldıktan sonra 3 akşam dene ve `/istatistik` sayfasına bak:

- **Dolum oranı ≥ %50** ve **tipik dolma süresi ≤ 10 dk** → sorun çözülüyor, büyütmeye geç.
- Altındaysa → sorun yazılımda değil havuzda: daha çok kişi davet et ya da daha çok kişiyi "Bana haber ver" kurmaya teşvik et.
