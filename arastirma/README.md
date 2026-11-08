# LoL Pre Bot Projesi — Araştırma Klasörü

**Problem:** League of Legends TR sunucusunda arkadaş grubu Flex için eksik kaldığında (ör. 4 kişiyiz, 5. lazım) hemen bir kişi bulmak.
**Araştırma tarihi:** 8 Kasım 2026

## Önce bunu oku

**[reports/Pre bot en iyi seçenek analizi.md](reports/Pre%20bot%20en%20iyi%20seçenek%20analizi.md)**: Güncel karar raporu. En üstte 10 kararın "en iyi / ikinci seçenek" tablosu ve yol haritası var.

## Özet karar

| Ne zaman | Ne yapılacak |
|---|---|
| **Bu akşam (kodsuz)** | Resmi "League of Legends Türkiye" Discord sunucusunda (138 bin üye) "+1" ilanı aç. Kendi sunucunda `@Flex-Musait` rolünü, rol/rank rollerini ve LFG Hub ya da Puck botunu kur. |
| **Bu hafta** | Minimal bir .NET botu yaz: Discord.Net 3.20.2, tek süreçli worker, SQLite + EF Core. Özellikleri: 60 dk'da kendiliğinden sönen "müsaitim" durumu, rol + rank eşleşmesine göre hedefli çağrı, 15 dk'da kapanan ilan, gelmeyen olursa yedeğe geçiş. |
| **Sonra** | 10–20 yedek oyuncuyla havuzu doldur, bir orta boy TR sunucusuyla ortaklık kur, Cuma–Pazar akşamları "pre saati" ilan et. Dolum oranını ve bekleme süresini ölç. Gerekirse kişisel anahtarla Riot doğrulaması ekle. |

Bu planın darboğazı araç değil, havuz büyüklüğü. Rol ve rank birlikte eşleşecekse, ilanların %90'ının dolması için aynı anda yaklaşık 20–56 müsait kişi gerekiyor.

## Klasör yapısı

- `reports/`: güncel karar raporu
- `onceki raporlar/`
  - `1 - ...araştırması.md`: ilk genel araştırma. Buradaki "haftalık sabit kadro" önerisi artık geçerli değil.
  - `2 - ...derin araştırma.md`: "anında 1 kişi bulma" odaklı ikinci rapor.
- `research_notes/`: üç araştırma turunun ham notları, kaynak URL'leriyle birlikte
  - `LoL Flex takım bulma araştırması/`: rakipler, Riot API, Flex kuralları, grup güvenilirliği
  - `LoL Flex takım bulma derin araştırma/`: TR oyuncu sesi, rakipler ve kapananlar, Riot'taki boşluklar, anlık doldurma sistemleri
  - `Pre bot en iyi seçenek analizi/`: seçenek karşılaştırması, mühendislik, eşleştirme tasarımı, lansman

## Etiketler

- **[D]**: doğrulanmış, birincil kaynaktan
- **[İ]**: iddia veya ikincil kaynak
- **[Ç]**: çıkarım, raporun kendi hesabı
