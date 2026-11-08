# LoL Pre Bot Projesi — Araştırma Klasörü

**Problem:** League of Legends TR sunucusunda arkadaş grubu Flex için eksik kaldığında (ör. 4 kişiyiz, 5. lazım) hemen bir kişi bulmak.
**Araştırma tarihi:** 8 Kasım 2026

## Önce bunu oku

**[reports/PreBot geliştirme yol haritası.md](reports/PreBot%20geliştirme%20yol%20haritası.md)**: Güncel yol haritası (9 Kasım 2026). Neyin yapıldığını, neyin eksik kaldığını ve sıradaki işleri fazlara ayırarak anlatıyor.

**[reports/Web uygulaması mı Discord mu.md](reports/Web%20uygulaması%20mı%20Discord%20mu.md)**: Kanal ve mimari kararı.

Karar şu: Ürünün çekirdeği, telefona ve bilgisayara anlık bildirim gönderen bir web uygulaması (PWA) olacak. Mevcut Discord botu yardımcı kanal olarak kalacak.

Bu kararın sebebi: Discord, Ekim 2024'ten beri Türkiye'de erişime kapalı ve Kasım 2026 itibarıyla hâlâ kapalı.

## Özet karar

| Konu | Karar |
|---|---|
| **Kanal** | Önce web: PWA + web push çekirdek ürün. Discord botu, web uygulamasına bağlantı taşıyan isteğe bağlı bir besleyici. |
| **Mimari** | Tek bir .NET 10 Blazor Web App. `PreBot.Core` aynen kullanılıyor. Canlı güncellemeler SignalR ile. Veritabanı SQLite (WAL modu). |
| **Giriş** | Google ve passkey. Discord ile giriş yalnızca isteğe bağlı (VPN olmadan çalışmıyor). |
| **Riot API** | MVP Riot API'siz başlıyor. Site tamamlanınca üretim anahtarına başvurulacak. |
| **Barındırma** | VPS + Docker + Caddy. Aylık yaklaşık 6 € artı alan adı. |
| **Süre** | 60–110 saat, yani haftada 15 saatle 4–7 hafta. Bu bir tahmin. |
| **Ana risk** | Likidite: aynı anda yeterince müsait oyuncu olması. Kanal değiştirmek bunu çözmüyor. |

## Rapor geçmişi

- `reports/PreBot geliştirme yol haritası.md`: **güncel plan**. Önceki önerilerin durumu, acil bulgular (sızan anahtar, `?katil=1`, yedek, KVKK), Ranked 5s fırsatı ve Faz 0–3 planı.
- `reports/Web uygulaması mı Discord mu.md`: kanal seçimini ve web mimarisini anlatıyor; bu karar hâlâ geçerli.
- `reports/Pre bot en iyi seçenek analizi.md`: Discord botu öncelikli karar raporu. Kanal kararı yukarıdaki raporla değişti. Eşleştirme, lansman ve havuz büyüklüğü bölümleri hâlâ geçerli.
- `onceki raporlar/1 - ...araştırması.md`: ilk genel araştırma. Buradaki "haftalık sabit kadro" önerisi artık geçerli değil.
- `onceki raporlar/2 - ...derin araştırma.md`: "anında 1 kişi bulma" odaklı ikinci rapor.

## Ham notlar (`research_notes/`)

Hepsi kaynak URL'leriyle birlikte.

- `LoL Flex takım bulma araştırması/`: rakipler, Riot API, Flex kuralları, grup güvenilirliği
- `LoL Flex takım bulma derin araştırma/`: TR oyuncu sesi, rakipler ve kapananlar, Riot'taki boşluklar, anlık doldurma sistemleri
- `Pre bot en iyi seçenek analizi/`: seçenek karşılaştırması, mühendislik, eşleştirme tasarımı, lansman
- `Web uygulaması mı Discord mu/`: kanal karşılaştırması, PWA ve push bildirimleri (Türkiye), .NET web mimarisi, kanala göre kullanıcı edinimi

## Etiketler

- **[D]**: doğrulanmış, birincil kaynaktan
- **[İ]**: iddia veya ikincil kaynak
- **[Ç]**: çıkarım, raporun kendi hesabı
