# PreBot plan fact-check (9 Nov 2026)

Scope: load-bearing factual claims in `reports/PreBot geliştirme yol haritası.md` (and the Discord claim in `reports/Web uygulaması mı Discord mu.md`).
Method: primary sources fetched on 9 Nov 2026 wherever reachable. **Turkish government sites (kvkk.gov.tr, resmigazete.gov.tr) did not resolve from this environment (DNS ENOTFOUND)**, so the KVKK items rest on law-firm summaries.
Verdicts: VERIFIED / PARTLY / WRONG / UNVERIFIABLE.

## Summary table

| # | Claim | Verdict |
|---|---|---|
| 1 | Flex allows parties of 1, 2, 3, 5; a party of 4 cannot queue | **VERIFIED** (Riot Support, 7 May 2026). No later patch note found changing it |
| 2 | No Flex rank restriction at Diamond and below; Master+ needs ≥ Emerald | **VERIFIED** as worded. Whether Flex or Solo rank counts is still unknown (the detail is in an image) |
| 3 | Discord blocked in TR since Oct 2024, still blocked Nov 2026 | **VERIFIED** (OONI measurements up to 9 Nov 2026 + vpnAlert 8 Nov 2026). No unblocking decision found |
| 4 | iOS push needs Home Screen (16.4+); Android/desktop Chrome/Edge need no install | **VERIFIED** for iOS. **PARTLY** for desktop (needs the browser running; "installed PWA gets push" is only a hedged MS Learn line) |
| 5 | Personal key not for public products; production key needs a live, verified site; LFG is an approved use case | **VERIFIED** (one nuance: the requirement is a "verified website", not literally "own domain") |
| 6 | No built-in PC feature for finding strangers for Flex in 2026 | **VERIFIED, with nuance**: the 2026 Discord integration adds a party link you can share "anywhere". It is not matchmaking, and it does not work in TR because of the Discord ban |
| 7 | Riot 2026 dev post: Flex ranks brought more in line with Solo/Duo; "Flex will never pull Solo up" | **VERIFIED** (post dated 1 Dec 2025, not "2026") |
| 8a | VERBİS exemption: <50 employees and <100M TL balance sheet | **VERIFIED (secondary)**. The plan's "non-commercial" framing is not a legal criterion (see below) |
| 8b | 2024 amendment changed Art. 9 transfer rules | **VERIFIED (secondary)**: Law 7499, Resmî Gazete 12 Mar 2024, in force 1 Jun 2024, transition to 1 Sep 2024 |
| 9 | Other issues | See section 9: **Ranked 5s overlap**, **Hetzner CX unavailable**, **KnownNetworks obsolete**, **GosuGamers claim unsupported**, **Riot boilerplate altered**, Politikam reliability |

---

## 1. Ranked Flex party sizes — VERIFIED
- Source: https://support.riotgames.com/en-us/league-of-legends/gameplay/ranked-tiers-divisions-and-queues (author Laserface, dated **7 May 2026**).
- Quote: "you can queue for Flex in groups of 1, 2, 3, and 5" / "parties of 4 will have to stick to unranked matches".
- No 2026 patch note found that changes Flex party sizes. The 2026 dev post changes Flex **MMR**, not party composition. I did not read every patch from 26.16 to 26.22 one by one. Before the landing-page copy goes live, check once in the client that a 4-stack can't queue.

## 2. Flex rank restrictions — VERIFIED (as worded)
- Same page, 7 May 2026: "there are no Flex rank restrictions for Diamond and below"; "If you want to play with Masters and up, however, you'll have to be at least Emerald."
- The detailed restriction table is an image I could not read. **Still open:** does the Emerald/Master check use Flex rank, Solo rank or MMR? The plan correctly lists this as an open question. Separately, Solo/Duo has an MMR-based rule ("players with an MMR of Master or higher" get solo-only restrictions). The text doesn't say whether Flex has a similar rule.

## 3. Discord blocked in Turkey — VERIFIED (still blocked)
- **Primary measurement:** OONI API, TR, discord.com, web_connectivity:
  - 1–9 Nov 2026: 477 measurements, 316 confirmed blocked, 57 anomalies, 102 OK. https://api.ooni.io/api/v1/aggregation?probe_cc=TR&domain=discord.com&since=2026-11-01&until=2026-11-09&test_name=web_connectivity
  - 1 Oct–9 Nov 2026: 3572 measurements, 2285 confirmed, 597 anomalies, 658 OK.
- vpnAlert (https://vpnalert.com/tools/censorship-monitor/turkey/discord/): "Discord has been blocked in Turkey since October 2024 under an Ankara court order". Last confirmed block 8 Nov 2026 16:31 UTC (Türk Telekom, Superonline).
- Politikam, 30 Oct 2026: still blocked, no reopening date. **Reliability caveat:** the same article says Roblox is still blocked, but several outlets (Türkiye Gazetesi, Milliyet, 18 Jun 2026) reported that Roblox reopened on 18 Jun 2026. Use Politikam only as weak corroboration.
- 2026 statements: Minister Uraloğlu said on 5–6 Jun 2026 that Discord would "probably" reopen, but gave no date. As of 18 Jun 2026 (Türkiye Gazetesi) the ban had "henüz sona ermedi" (not yet ended). **No court decision or BTK notice lifting the ban was found for 2025–2026.** The BTK and e-Devlet sites could not be checked from here.
- The plan cites NTV [İ] for this. Better to cite OONI as the primary source.

## 4. Web push platforms — iOS VERIFIED; desktop PARTLY
- iOS: WebKit, 16 Feb 2023, https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/ : "A web app that has been added to the Home Screen can request permission…". Permission must also come "in response to direct user interaction".
- Still true in 2026: neither the Safari 27 beta post (8 Jun 2026, https://webkit.org/blog/17967/) nor the Safari 27.0 features post (17 Sep 2026, https://webkit.org/blog/18325/, first ~100k characters read) mentions any web push change. In iOS 26, Home Screen sites open as web apps by default (secondary sources). That helps installs but does not add push to Safari tabs.
- Declarative Web Push: `"web_push": 8030` confirmed; tested on iOS/iPadOS 18.4 and the macOS 15.5 beta (https://webkit.org/blog/16535/meet-declarative-web-push/, 27 Mar 2025). The post doesn't say whether iOS drops the Home Screen requirement for it, so assume it doesn't.
- Android Chrome and desktop Chrome/Edge: web push works without installing. This is standard Push API behaviour (MDN Baseline "widely available" since Mar 2023). MS Learn (https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/push-notifications, updated 4 Nov 2026): "the browser must be running, or the notification appears the next time that the browser is opened"; "When using the installed PWA, the notification **should** be delivered even if the user isn't running the app." The second sentence is hedged and isn't backed by a browser vendor doc. F1.9 ("install on desktop so you don't miss notifications") rests on it, so test on a real Windows PC with Chrome and Edge fully closed.

## 5. Riot developer policy — VERIFIED
- Portal (https://developer.riotgames.com/docs/portal, undated): "You may not run your application for public consumption using a personal key… public consumption includes open alpha/beta tests." Production keys: "should be used for products that are intended for large communities or the Internet as a whole" (so the plan's "large communities" wording **is** on this page, not on /docs/lol). Starting limits: 500 requests per 10 s and 30,000 per 10 min, per region.
- FAQ (https://developer.riotgames.com/docs/faqs): "We cannot grant production keys to applications without a verified website"; ToS and Privacy Policy must be viewable; "unable to accept Github repositories and source code"; verification is a "text string to upload to your URL"; reviews "typically… weekly", "can take up to three weeks"; "RSO Clients are only available for applications that have an existing, approved production application ID."
- /docs/lol (https://developer.riotgames.com/docs/lol) lists "Looking For Game (LFG) tools." as an approved use case. Production keys "are meant for larger-size, professional projects." (The fetched page headings came out garbled, but LFG sits in the approved-use-case lists.)
- Nuance: Riot asks for a *verified website*. "Own domain" is implied by the verification string, not stated.

## 6. Riot built-in stranger-finding on PC — VERIFIED with nuance
- No PC party finder or LFG feature in the League client was found for 2025–2026. Wild Rift has Party Finder (mobile only). A Russia-only "looking for friends" feature from years ago came up in searches, but I could not date it.
- The 2026 Discord integration (beta with patch 26.8, mid-Apr 2026; Brazil from 7 May and US/Canada from 20 May per Discord):
  - Riot: https://www.riotgames.com/en/riot-games-discord-account-linking : "Copy a lobby link and share it with your Discord server or wherever your friends are." Available "only in regions where Riot and Discord are both supported"; "As the integration rolls out globally, access may vary by region." Turkey is not mentioned.
  - Riot Support, 15 May 2026: https://support.riotgames.com/en-us/league-of-legends/gameplay/linking-your-riot-account-to-discord : the invite URL can be shared "anywhere".
  - Discord blog, 21 May 2026: https://discord.com/blog/making-it-easier-than-ever-to-connect-with-friends-in-league-val : "Drop a link to join your party into a Discord server, DM, or anywhere else".
- Assessment: the plan's "lobby link + friend status, no matchmaking with strangers" is correct. But it is **more than friend status**: a shareable party-join link lowers the "copy Riot ID / send invite" friction that PreBot works around. It is effectively unavailable in TR because Discord is blocked and no TR or EU rollout has been announced. Whether non-friends or unlinked users can join through the link is undocumented. The F3.4 "lobiye katıl" idea is sound, but no source says the link works outside Discord.

## 7. Riot /dev: Ranked 2026 — VERIFIED (date nuance)
- https://www.leagueoflegends.com/en-us/news/dev/dev-ranked-2026 (Christina Zhao and Matthew Leung-Harrison, **1 Dec 2025**): "bringing Flex ranks more in-line with Solo/Duo ranks (Flex will never pull Solo up, however.)"
- The post was published in Dec 2025, for the 2026 season, so "Riot 2026'da … dedi" is slightly off. It says nothing about party-size or Flex party rank changes, only "within reason".

## 8. KVKK
### 8a. VERBİS exemption — VERIFIED (secondary; government sites unreachable)
- Erdem & Erdem, 2 Oct 2025: https://www.erdem-erdem.av.tr/bilgi-bankasi/verbis-kayit-yukumlulugune-iliskin-istisna-kriteri-degistirildi . Kurul decision 2025/1572 (4 Sep 2025), Resmî Gazete 1 Oct 2025 no. 33034. Existing exemption: "yıllık çalışan sayısı 50'den az ve yıllık mali bilanço toplamı 100 milyon TL'nin altında", provided the main activity isn't processing special-category data. New: controllers whose main activity *is* special-category processing are exempt below 10 employees and 10M TL.
- Erdem & Erdem, 14 Jan 2026: https://www.erdem-erdem.av.tr/bilgi-bankasi/verbis-kayit-yukumlulugune-iliskin-yeni-istisnalarin-uygulama-esaslari-aciklandi . Decision 2025/2393 (25 Dec 2025): for controllers that don't keep balance-sheet books, "yalnızca yıllık çalışan sayısı kriteri" applies. That is the case relevant to an individual running PreBot.
- **Correction to the framing:** there is no "non-commercial" or "small" category in the rule. The test is employee count plus balance sheet, combined with main activity. An individual with no balance sheet and 0 employees whose main activity isn't special-category data falls under the exemption. The plan's conclusion ("büyük olasılıkla geçerli", and Art. 10 and 12 still apply) is right.
- The cited hukukihaber page was not opened. The law-firm pages above say the same thing.
### 8b. Art. 9 amendment — VERIFIED (secondary)
- Law No. 7499, Resmî Gazete 12 Mar 2024, in force 1 Jun 2024. Old and new Art. 9 ran side by side until 1 Sep 2024. Transfer routes: adequacy, then appropriate safeguards (BCRs, standard contracts notified within 5 business days, written undertakings with Board permission), then incidental exceptions. Explicit consent is now one of the incidental exceptions only. Sources: Mondaq https://www.mondaq.com/turkey/data-protection/1443842/ ; Bilal Alyar (May 2026) https://bilalalyar.av.tr/kvkk-uluslararasi-veri-aktarimi/ ; Prighter Art. 9 text https://prighter.com/resources/laws/turkish-kvkk/the-personal-data-protection-law/articles/article-9 .
- The plan's statement ("Haziran 2024'ten beri… açık rıza sürekli aktarımlar için uygun değil") is correct. The KVKK announcement link (kvkk.gov.tr/Icerik/8043) was not reachable.

## 9. Other load-bearing items
1. **Ranked 5s overlaps the product's core window — scope decision should be re-examined (not a factual error, but the "temporary experiment" framing understates it).** Riot Support (19 Jun 2026), https://support.riotgames.com/en-us/league-of-legends/events/ranked-5s : TR is included; "You'll need a full team to enter the game queue"; Fri–Sun 16:00–04:00 server time; Experiment 2 runs **11 Sep 2026 – 7 Jan 2027**; no rank restrictions. So right now a 4-stack can't play Ranked 5s either, and its hours fully cover the proposed "pre saati" (Fri–Sun 20:00–01:00). This is a second, concurrent source of "we're 4, need 1" demand. It may also pull 5-stacks out of Flex. A cheap hedge: a "Flex / Ranked 5s" toggle on the listing.
2. **Hetzner CX23 may not be orderable — PARTLY.** The price (~€5.99/mo excl. VAT after the 15 Jun 2026 price change) matches secondary sources. But https://www.hetzner.com/cloud/ (fetched 9 Nov 2026) marks the Cost-Optimized plan "Currently not available". F0.12 says to buy it "Hemen", so have a fallback (CPX/CAX line or another EU provider) and recheck the price.
3. **.NET 10 `KnownNetworks` is obsolete — resolves an open question in F0.3.** MS Learn breaking change: https://learn.microsoft.com/en-us/aspnet/core/breaking-changes/10/ipnetwork-knownnetworks-obsolete?view=aspnetcore-10.0 . `ForwardedHeadersOptions.KnownNetworks` and `HttpOverrides.IPNetwork` are obsolete (warning ASPDEPR005). Use `KnownIPNetworks` with `System.Net.IPNetwork`. With warnings-as-errors in CI, the code as written in F0.3 would **fail the build**.
4. **The F1.3 claim "2026'dan itibaren Flex eşleştiricisi parti içindeki bireysel rankları hesaba katıyor" (GosuGamers) — WRONG / unsupported.** Neither GosuGamers nor the Riot dev post says this. Both say only that Flex MMR is brought in line with Solo/Duo. Drop the claim, or reword it to the MMR alignment.
5. **Riot boilerplate is altered — minor.** The plan's footer says "League of Legends and Riot Games are trademarks…". Riot's required text (https://developer.riotgames.com/policies/general, updated 29 May 2025) ends "Riot Games, and all associated properties are trademarks or registered trademarks of Riot Games, Inc." Use it verbatim in F0.6.
6. **Riot Legal domain rule — PARTLY.** https://www.riotgames.com/en/legal ("Last Updated: August 2018") says you "may not register domain names… that uses Riot Games" trademarks. The page does not name "LoL" or "League" explicitly, but they are Riot trademarks, so the plan's advice holds. `prebot.com.tr` is fine.
7. **Watchtower archived on 17 Dec 2025 — VERIFIED.** https://github.com/containrrr/watchtower : "archived by the owner on Dec 17, 2025".
8. **Riot review time "haftalar ile aylar" — PARTLY.** The official FAQ says weekly, "up to three weeks". The months figure comes only from community issues. The plan already treats it that way.
9. **15-yaş social media law — UNVERIFIABLE here.** I did not try to confirm whether it was enacted. The plan already marks it [İ].

## Not checked / could not reach
- kvkk.gov.tr, resmigazete.gov.tr, BTK (DNS failure from this environment).
- The image-based Flex restriction table on the Riot Support page.
- The Safari 27.0 post past the first ~100k characters, and the full Safari 27.x release notes.
- Whether the Riot Discord party link works for non-friends, unlinked users, or outside Discord.
