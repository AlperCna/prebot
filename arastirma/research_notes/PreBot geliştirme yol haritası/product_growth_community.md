# Product, growth and community best practices for PreBot (LoL TR Flex LFG), as of Nov 2026

Research date: 2026-11-09. About 30 searches and fetches. Several primary pages (Medium/Google Play, lolprofile.net) returned 403. Facts carry inline sources. Items under "Inferences" are my reasoning, not sourced facts. Older items are labelled with their year.

---

## 1. How comparable LFG products work (2025–2026): what drives success and what failed

### Takeaway
Riot's PC client still has no built-in LoL party finder. The nearest options are Wild Rift's Party Finder, which ranks friends and friends-of-friends first, and an official Discord integration announced in April 2026, which is lobby links and friend presence rather than matchmaking with strangers. Third-party LoL duo finders are mostly Discord bots that sell rank verification and reputation. The strongest evidence on what works comes from Overwatch: LFG plus endorsements went with a measurable drop in disruptive behaviour, but in-game LFG was later dropped, partly over toxicity and low use.

### Cited Findings
- **Riot, PC LoL:** I found no confirmed built-in "Party Finder" for the League PC client in 2025–2026. — [search summary; Wild Rift support page](https://support.riotgames.com/wild-rift/gameplay/party-finder)
- **Wild Rift Party Finder (Riot, mobile):** a friend's or friend-of-friend's party "will appear higher on the list than any others". Hosts can choose friends-only or open parties and add tags. — [Riot Support – Wild Rift Party Finder](https://support.riotgames.com/wild-rift/gameplay/party-finder)
- **Riot Discord integration (reported April 2026):** described as "official Discord integration designed to help players connect and form parties more easily" for LoL and Valorant. Reported Valorant features are inviting Discord friends to a party without switching windows, shareable lobby links for DMs or servers, and seeing which Discord friends are playing. Valorant rollout: beta in Brazil, then US/Canada on April 21, then global with Patch 12.08 on April 29. No LoL-specific feature list was given. The report is secondary, citing Sheep Esports. — [Esports News UK, updated 15/04/2026](https://esports-news.co.uk/?p=188683)
- **Overwatch 1 LFG (2018) plus endorsements:** at GDC, Blizzard reported about a 40% reduction in matches with disruptive behaviour, measured by reports per match. Between 50% and 70% of players gave endorsements. — [PC Gamer](https://www.pcgamer.com/overwatchs-endorsement-system-has-cut-disruptive-behavior-by-40-percent); [Digital Trends](https://digitaltrends.com/gaming/overwatch-toxicity-down-40-percent-thanks-to-endorsement-system) credits endorsements and LFG together.
- **Earlier, from Kaplan (2018):** abusive chat in competitive matches fell by more than 26% in the Americas and 16% in Korea. He credited LFG and endorsements together. — [Inven Global](https://www.invenglobal.com/articles/5645/jeff-kaplan-reports-abusive-chat-on-the-decline-thanks-to-lfg-and-endorsement-system)
- **OW LFG design:** it grouped players by endorsement level and preferred communication style. Blizzard stressed it is "not an option for matchmaking". — [Digital Trends](https://digitaltrends.com/gaming/overwatch-toxicity-down-40-percent-thanks-to-endorsement-system); [MassivelyOP](https://massivelyop.com/?p=175768)
- **What failed:** LFG was missing from Overwatch 2 at its October 2022 launch, and the executive producer later said LFG in OW2 was "unlikely to happen soon". Forum users recalled the reasons as low usage ("no one was using it") and unsolved toxicity. Players petitioned to bring it back. This is 2022–2023 information. — [esports.gg](https://esports.gg/news/overwatch/overwatch-2-lfg-is-unlikely-to-happen-soon); [Blizzard forums](https://us.forums.blizzard.com/en/overwatch/t/please-add-lfg-back-into-ow2/890055); [Change.org](https://www.change.org/p/bring-back-the-looking-for-groups-feature-in-overwatch-2)
- **Teams.gg** added Overwatch 2 to its team-finding service, with extra anti-toxicity measures for under-16s. — [Esports News UK](https://esports-news.co.uk/?p=54749)
- **FACEIT Party Finder** exists for CS2 and Overwatch 2, not LoL. — [FACEIT support](https://support.faceit.com/hc/en-us/articles/14996733545884)
- **Third-party LoL duo finders (self-published marketing):**
  - **Miko by Duoo.gg** (Discord bot): rank verification, duo requests, post-game reviews, multi-server sharing via /LFG, and linking smurf accounts. — [top.gg](https://top.gg/bot/1096340606235717642)
  - **"Queue Up! | Duo Finder"** (Discord bot): matches on rank, lane, region, playstyle and mic preference, and advertises a reputation system. — [top.gg](https://top.gg/bot/1446819621133877288)
  - **General LFG apps** (Arena, Player 2, PlayerFinder, GamerLink) cover many games, and none is LoL-specific. — [App Store: Arena](https://apps.apple.com/app/id6740140093); [PlayerFinder](https://apps.apple.com/mn/app/id1672325368?l=en-gb)
- **Discord LFG servers remain the default:** guides recommend regional LFG Discord servers and Disboard. One example is "LoL Team Finder (all regions)", with more than 11,000 members covering Solo/Duo, Flex, Clash and scrims. — [ExitLag blog](https://www.exitlag.com/blog/lfg-league-of-legends/)

### Inferences
- **The niche is open.** Riot has no in-client LoL PC finder for strangers, and its Discord integration is friend and lobby-link based. A "fill the 5th seat now" tool for TR is not directly competing with Riot. That could change if the LoL Discord integration adds open lobbies, so watch Riot patch notes.
- **Copy Wild Rift's ranking rule.** Show listings from friends or past teammates first. In PreBot terms, notify players who previously played with this host, or endorsed them, in wave 1. This uses the social graph to raise join and show rates.
- **Copy Overwatch's lesson.** Pair LFG with a lightweight positive signal (endorsements) plus "communication style" tags (mic, chill or tryhard, Turkish only). Expect low usage to kill the feature unless liquidity is concentrated; see section 2.
- **Rank verification and reputation are table stakes.** Duo-finder bots advertise both. PreBot already has Riot ID entry. Fetching the rank via the Riot API instead of trusting self-reported rank is the obvious next step against rank misreporting. API terms were not researched here.

### Gaps
- No usage numbers for Discord LoL duo bots, GamerLink, LFGroup or Guilded (Guilded's status in 2026 was not researched).
- No primary Riot announcement of the LoL side of the Discord integration was found.
- Riot's Valorant in-client features were not researched.
- No TR-specific LFG sites or apps were found in searches.
- The Turkish-language Ekşi Sözlük search returned nothing relevant.

---

## 2. Cold-start and liquidity tactics for a zero-budget project

### Takeaway
Build one "atomic network" first. That means the smallest group dense enough that a listing reliably fills: one time window (prime time), one queue (Flex), one server (TR), seeded from a few existing communities. Expand only after that works. For PreBot this means concentrating supply into scheduled windows and leaning on existing Turkish communities: university clubs through Riot campus ambassadors, Discord servers, and streamers.

### Cited Findings
- **Atomic network:** the smallest network that can sustain itself. Build one, then copy it into adjacent networks. Slack needed "under ten in a single company". Uber started with narrow moments like "5pm at the Caltrain station at 5th and King St.", with ops directing drivers to busy spots, then expanded city by city. — [Lenny's Newsletter, "The Atomic Network" (Andrew Chen)](https://www.lennysnewsletter.com/p/atomic-network)
- **Launch tactics:** launch in the "simplest possible form" with a "dead simple value proposition". Target a tiny network. Take a "do whatever it takes" approach even when unscalable. The first network is probably far smaller than you expect, perhaps "hundreds of people" at one moment. — [Lenny's Newsletter](https://www.lennysnewsletter.com/p/atomic-network)
- **Kick-off hacks cited:** Slack's invite-only launch, PayPal's $5 referral, Dropbox's Hacker News demo video, Uber Ice Cream promotions. Dense local connections raise engagement and virality because prospects see friends already using the product. — [Lenny's Newsletter](https://www.lennysnewsletter.com/p/atomic-network)
- **The cold start problem:** new users arrive, find an empty network, and leave (anti-network effects). Chen separates the "easy" and "hard" sides of a network. — [Chen book summaries; primary book 2021](https://www.antoinebuteau.com/the-cold-start-problem-book-summary.md) (secondary summary; thresholds should be checked against the book)
- **Turkish university esports:** Riot Turkey's University League 2025 required a "Riot Kampüs Elçisi" (campus ambassador) at the university for the closed qualifiers. An ambassador lookup form existed. Prizes: 40,000 TL plus a UEM slot for 1st, 20,000 TL for 2nd, 10,000 TL for 3rd. Registration was through optimi.gg. Posted March 27, 2025. — [Riot TR news](https://www.leagueoflegends.com/tr-tr/news/league-of-legends-niversite-ligi-2025-balyor/)
- **TÜSF ÜNİLİG** runs esports qualifiers that include LoL. For example, GTU's LoL team reached the semifinals. — [GTU news](https://www.gtu.edu.tr/en/icerik/8/25955/display.aspx)
- **Discord in Turkey:** blocked since 9 Oct 2024 by an Ankara 1st Criminal Judgeship of Peace decision. Discord made age verification mandatory for all users from early March 2026, which raised hopes of reopening. I found no official confirmation that the block was lifted in 2026. — [NTV](https://www.ntv.com.tr/turkiye/galeri-discord-turkiyede-acilacak-mi-kuresel-yas-dogrulama-karari-sonrasi-gozler-o-tarihte-1711249); [Gazete Birlik](https://www.gazetebirlik.com/genel/discord-erisim-sorunu-var-mi-kalkti-mi-2026-turkiyede-discord-acildi-mi-ve-son-durum-ne/942165)
- **VPN use in Turkey** is reported as rising, for example after the Instagram block in Aug 2024, but no percentage was found. A draft rule requiring identity verification for VPN services was reported as a plan, not law. — [TGRT Haber](https://www.tgrthaber.com/vpn?p=2); [Türkgün](https://www.turkgun.com/gundem/vpn-turkiyede-yasaklaniyor-mu-2026-vpn-e-kisitlama-mi-geliyor/370826)
- **Discord LFG mega-servers** with more than 11k members exist across regions. — [ExitLag](https://www.exitlag.com/blog/lfg-league-of-legends/)

### Inferences (actionable for PreBot)

**Concentrate time**
- Default the "notify me" window to one advertised prime-time slot, for example "Flex Saati 21:00–00:00", and show a daily countdown. Uber's "5pm at Caltrain" lesson suggests one dense window beats 24/7 sparse availability.
- Consider "scheduled listings" (a missing feature) as a top priority. A listing for "tonight 22:00" gives hours for notification waves to find a 5th instead of minutes.

**Concentrate audience**
- Launch with 3–5 seed groups (friend groups, one university club, one Discord server) and measure fill rate per seed. Copy the onboarding that works into the next community, as Slack went team to company.

**Easy side vs hard side**
- 4-stacks needing a 5th are motivated hosts (the easy side). Available solo players willing to join strangers are the hard side.
- To recruit the hard side, pitch the solo player: "skip Solo/Duo autofill and play with a premade", "no 4-stack restriction", "Flex rank now closer to your Solo rank".

**Work around Discord being blocked**
- Keep the Discord bot, but make the PWA plus Telegram or WhatsApp channel shares first-class. For example, a shareable link card for each listing with an Open Graph preview.
- Many TR users reach Discord only via VPN. The legal and network situation is unstable.

**University clubs**
- Contact Riot campus ambassadors and university esports clubs. They already gather 5-person rosters and have Flex needs during practice.
- Offer a "club" mode: private listings visible only to club members first, then public. Zero budget is required.

**Streamers and Kick/Twitch TR**
- Offer streamers a "viewer games" listing link ("izleyicilerle flex") that fills from their audience. This is a natural fit and costs nothing.
- Lolalytics lists TR Twitch LoL streamers for outreach targeting. — [Lolalytics TR Twitch leaderboard](https://lolalytics.com/tr/lol/twitch/leaderboard/?region=tr)

**Do unscalable things**
- The maintainer and friends "flintstone" liquidity early: sit in the "available now" pool during prime time so the first listings fill. Chen's "do whatever it takes" applies.

**Social proof on the landing page**
- Show "Bu akşam X kişi müsait / son 7 günde Y ilan doldu" on the landing page, from the existing stats page. This is honest only if the numbers are real. Show nothing (or "be the first") when low, rather than inflating.

### Gaps
- No TR-specific LFG cold-start case study was found.
- No data was found on Ekşi Sözlük, Reddit or Turkish Discord sizes for LoL Flex.
- No TR Kick or Twitch LoL viewer numbers were fetched.

---

## 3. Retention and habit loops without dark patterns; notification fatigue

### Takeaway
"Too frequent" is the top reason users opt out of push (Airship 2025 survey). Gaming apps have among the lowest opt-in and click rates. There is no magic frequency. Use a preference centre, quiet hours, a per-day cap, and transactional (high-relevance) notifications rather than promotional ones. On iOS, web push only works after the PWA is added to the Home Screen and permission is asked on a user tap.

### Cited Findings
- **Airship 2026 benchmarks** (data Jan–Dec 2025, 15 verticals):
  - Median opt-in is 53.3% on Android and 48.85% on iOS.
  - Gambling & Gaming has the lowest Android 10th-percentile opt-in floor cited, at 21.35%.
  - The iOS median direct open rate fell 13.71% year over year.
  - The top reason users opt out is messages sent "too frequently" (Global Consumer Survey).
  - Top-decile senders see 72% more sessions per active user than their category average.
  - There is no "magic number". Use preference centres and surveys to set frequency.
  
  — [Airship, Mobile App Push Notification Benchmarks for 2026](https://www.airship.com/mobile-app-push-notification-benchmarks-for-2026/); [Airship 2025 PDF](https://growth.airship.com/rs/313-QPJ-195/images/Airship-2025-Push-Notification-Benchmarks-EN.pdf?version=0)
- **Monthly push volume** at the 90th percentile is about 179 (Android) and 188 (iOS) per user, while the Android median fell about 15% YoY. — [Airship 2026](https://www.airship.com/mobile-app-push-notification-benchmarks-for-2026/)
- **Pushwoosh:** game apps had the lowest opt-in rates of the industries analysed, 83.2% on Android and 29.98% on iOS. The study date was not confirmed and may predate 2025. — [PocketGamer.biz on Pushwoosh](https://Www.pocketgamer.biz/news/78993/android-games-struggle-to-retain-players-states-pushwoosh-report/); [Pushwoosh benchmarks](https://www.pushwoosh.com/blog/push-notification-benchmarks/)
- **Gaming push click rate:** 4.8% average, the lowest vertical in one benchmark (date unconfirmed). — [MobiLoud push stats](https://www.mobiloud.com/blog/push-notification-statistics)
- **iOS web push:**
  - Since iOS/iPadOS 16.4, web push works only for web apps added to the Home Screen.
  - Permission may be requested only in response to a direct user interaction, such as a tap.
  - The Badging API is supported, and notifications integrate with Focus.
  
  — [WebKit blog](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados)
- **Later Safari/iOS changes:** Safari 18.4 added Declarative Web Push, and iOS 26 opens Home Screen sites as web apps by default. — [Monterail (secondary)](https://www.monterail.com/blog/pwa-for-apple-ios)

### Inferences (actionable)
**Notification types**
- PreBot's notifications are transactional ("a seat matching you is open now"). That is the high-value kind, but they lose value fast because the seat goes in seconds.
- Send "seat taken" follow-ups only if the user tapped. Never push a listing that is already full.

**Hard caps**
- Suggested default: at most N pushes per evening, for example 3–5, plus a per-listing cap of 1.
- Add a user-selectable "only Silver–Gold, only my main role" filter so each push matters.
- Honour quiet hours by default, for example 01:00–12:00. These numbers are design suggestions, not sourced.

**Opt-in flow**
- Ask for push permission after the user sets their first "notify me" window, not on first load. This is required on iOS, and it is also when the value is clear.
- Show an "Add to Home Screen" guide for iPhone users. Without it there is no push on iOS.
- Track the opt-in rate separately per platform. The benchmark is about 49–53% median, and gaming is lower.

**Auto-dormancy**
- If a user ignores K consecutive pushes, lower their wave priority or pause them and send one "still want alerts?" message.
- This protects the open rate and avoids "too frequent" opt-outs.

**Digest instead of streaks**
- Prefer an optional daily or weekly digest, such as "this week 12 Flex lobbies in your rank filled, avg 3 min". Prefer honest live counts ("şu an 7 kişi müsait") over streaks.
- Streaks fit poorly with a "need a 5th now" utility. No source was found on LFG streaks.

**Habit anchor**
- The real loop is: friend group wants Flex, opens PreBot, fills in minutes. Retention follows fill rate. Each failed listing (no 5th) is the main churn driver. Invest in fill rate before engagement features.

### Gaps
- Google Play's "Optimizing notifications in games" article (Medium) returned 403, so no game-specific frequency study was captured.
- No study on web push (PWA) opt-out versus native was found.
- No LFG-specific retention or streak case study was found.

---

## 4. Trust and safety: no-shows, reputation, reporting, age, KVKK/legal, abuse, rank misreporting

### Takeaway
Positive-first reputation (Overwatch-style endorsements) has the best evidence, with about 40% fewer disruptive matches. Turkey's legal environment is tightening:
- An under-15 social media restriction passed committee in April 2026. Age checks are reported for Nov 1, 2026 and closure of under-15 accounts by Mar 31, 2027. Whether it became law is unconfirmed.
- Discord was blocked partly over child-safety claims.

PreBot should be 15+ or 16+, minimise personal data, and have a visible report/block path and a contact point, which the 5651 law requires.

### Cited Findings
- **Endorsements:** about a 40% reduction in matches with disruptive behaviour, with 50–70% of players endorsing. — [PC Gamer](https://www.pcgamer.com/overwatchs-endorsement-system-has-cut-disruptive-behavior-by-40-percent)
- **The figure is about share of matches with disruptive behaviour.** Many bad matches remain. One critic argues endorsements can be farmed (opinion). — [Digital Trends](https://digitaltrends.com/gaming/overwatch-toxicity-down-40-percent-thanks-to-endorsement-system); [Gamespace](https://gamespace.com/all-articles/opinions/overwatch-farming-endorsements-does-it-really-matter/)
- **Under-15 social media bill (Turkey):**
  - It passed the TBMM Health, Family, Labour and Social Affairs Committee (reports dated April 2026).
  - Minister Göktaş said in May 2026 that the regulation would be implemented within 6 months.
  - Sanctions include bandwidth throttling and an ad ban.
  - An international briefing reports age verification starting 1 Nov 2026 and under-15 account closure by 31 Mar 2027.
  - Sources conflict on whether the bill passed the General Assembly.
  
  — [Webtekno](https://www.webtekno.com/15-yasindan-kucuklere-sosyal-medya-yasagi-tbmm-komisyonundan-gecti-h214619.html); [Evrensel 19.05.2026](https://www.evrensel.net/haber/5984761/15-yas-altina-sosyal-medya-yasagi-yonetmelik-icin-tarih-verildi); [Ground News briefing](https://ground.news/daily-briefing/31177039-ed94-46fa-ad2b-f415bee178d1); [Gazete Birlik](https://www.gazetebirlik.com/gundem/15-yas-altina-sosyal-medya-yasagi-geldi-mi-meclisten-gecti-mi-kimleri-kapsiyor-ne-zaman-yururluge-girecek/970935); [Hürriyet Daily News](https://www.hurriyetdailynews.com/turkiye-weighs-under-15-ban-in-social-media-regulation-for-minors-216457)
- **Discord block rationale:** suspicions of child abuse, obscenity and illegal content. — [NTV](https://www.ntv.com.tr/turkiye/galeri-discord-turkiyede-acilacak-mi-kuresel-yas-dogrulama-karari-sonrasi-gozler-o-tarihte-1711249)
- **Law 5651 (hosting providers):**
  - Article 5 sets the hosting provider's obligations.
  - A hosting provider has no general duty to monitor or investigate content.
  - Content and hosting providers must keep identifying information current and accessible on their site.
  - "Uyar-kaldır" (notice-and-takedown) is the most common removal route for forums.
  
  — [Erdem & Erdem: 5651 internet actors](https://www.erdem-erdem.av.tr/bilgi-bankasi/5651-sayili-kanun-kapsaminda-internet-aktorleri); [Hukuki Haber](https://www.hukukihaber.net/internet-ortaminda-yer-saglayicinin-hukuka-aykiri-icerikten-dogan-sorumlulugu); [Kanun metni PDF](https://aile.gov.tr/uploads/chgm/uploads/pages/kanunlar/5651-sayili-internet-ortaminda-yapilan-yayinlarin-duzenlenmesi-ve-bu-yayinlar-yoluyla-islenen-suclarla-mucadele-edilmesi-hakkinda-kanun.pdf)
- **Article 9 annulled:** one law-firm source says the AYM decision (E.2020/76, K.2023/172) annulled Article 9 of 5651 (personal-rights removal) effective 10.10.2024. This is a single source and needs verification. — [Reform Avukatlık](https://reformavukatlik.com.tr/internet-icerigi-nasil-kaldirilir/)
- **Teams.gg** added extra anti-toxicity measures for under-16s when adding Overwatch 2. — [Esports News UK](https://esports-news.co.uk/?p=54749)
- **Duo-finder bots** advertise rank verification, post-game reviews and reputation as core features. — [Miko/Duoo.gg](https://top.gg/bot/1096340606235717642); [Queue Up!](https://top.gg/bot/1446819621133877288)

### Inferences (actionable)
**No-show handling**
- PreBot has "no-show, search again". Add a private no-show counter: the host flags a joiner who did not accept the invite within X minutes.
- After 2–3 flags in 30 days, apply a short cooldown before they can claim seats. Do not show public shame.
- Also add a joiner-side flag: "host never invited".

**Reputation**
- Use positive-only endorsements after a listing closes: "iyi takım arkadaşı / iletişim iyi / zamanında geldi". Show counts as a badge.
- Use endorsement level as a wave-ordering signal: high-endorsed players get wave 1. This mirrors Overwatch's LFG grouping by endorsement level. Avoid public negative ratings, which invite revenge rating.

**Block and report**
- Blocking: a blocked user never sees or is notified about your listings. This is cheap and very high value.
- Reporting: one button sending to a maintainer queue, with categories (toxic, impersonation, spam, minor). Add a published contact email (5651 identity/contact duty) and a notice-and-takedown page.

**Age**
- Require a self-declared 16+ (or at least 15+ to align with the Turkish bill) with an explicit checkbox.
- PreBot is a matching tool, not a content feed. The legal classification as "sosyal ağ sağlayıcı" is unclear and likely below thresholds, but a minor-safety posture is prudent given the Discord precedent.
- Do not add free-text chat, which is the highest-risk surface. Keep the Riot ID copy and in-game invite flow.

**KVKK / data minimisation**
- Store only Riot ID, rank/role, push subscription and time windows.
- Publish an "aydınlatma metni" (privacy notice), auto-delete stale availability, and allow account deletion. KVKK specifics were not researched; that is a gap.

**Rank misreporting**
- Verify rank via the Riot API from the Riot ID, or at least show a "doğrulanmış" badge when verified. Duo bots market exactly this.
- Riot's 2026 change brings Flex MMR closer to Solo MMR, so showing the Solo rank alongside Flex is more meaningful now.

**Anti-spam**
- Rate-limit listing creation per account, for example at most 3 open listings per hour.
- Require a verified Riot ID before hosting.
- Throttle push waves per host to prevent notification spam via fake listings.

### Gaps
- KVKK obligations for a non-commercial, open-source hobby project (VERBİS exemption, aydınlatma metni content) were not researched; searches returned nothing.
- Whether the under-15 law applies to non-social matching tools is unknown.
- The final status of the under-15 bill (General Assembly, Resmî Gazete) is unconfirmed.
- Riot Developer API policy on displaying rank for third-party LFG was not checked.

---

## 5. Success metrics and benchmarks for an LFG app

### Takeaway
I found no published LFG-specific benchmarks for fill rate, time-to-fill or no-show rate. Only general push benchmarks exist (above). PreBot should define its own north-star metric and use Chen's atomic-network logic: the key metric is "listings filled within N minutes during prime time".

### Cited Findings
- **Push benchmarks:**
  - Median opt-in is 53.3% on Android and 48.85% on iOS.
  - The gaming Android floor is 21.35%.
  - The iOS median direct open rate fell 13.71% YoY.
  
  — [Airship 2026](https://www.airship.com/mobile-app-push-notification-benchmarks-for-2026/)
- **Gaming push click rate:** about 4.8%, the lowest vertical (date unconfirmed). — [MobiLoud](https://www.mobiloud.com/blog/push-notification-statistics)
- **Overwatch** used reports per match as its safety metric (disruptive-match share) and endorsement participation (50–70%). — [PC Gamer](https://www.pcgamer.com/overwatchs-endorsement-system-has-cut-disruptive-behavior-by-40-percent)
- **Atomic network framing:** the first network may be just "hundreds of people" at one moment. — [Lenny's Newsletter](https://www.lennysnewsletter.com/p/atomic-network)

### Inferences (suggested metric set; targets are my proposals, not benchmarks)
- **North star:** % of listings filled within 10 minutes, for prime time (20:00–00:00) and off-peak separately.
- **Funnel per listing:** pushes sent → delivered → opened (CTR) → "Join" tapped → seat claimed → invited in-game → game played (host confirms) → no-show.
  - Log each step server-side. No third-party analytics is needed, and KVKK-friendly self-hosted counts are enough.
- **Liquidity metrics:**
  - "available now" users at 21:00 on weekdays
  - median time-to-fill
  - pushes per filled seat (efficiency)
  - % of listings needing more than one wave
- **Retention:** D1/D7/D30 for hosts and joiners separately. Also "repeat host rate": a friend group that comes back next week is the true retention signal.
- **Notification health:**
  - opt-in rate by platform (compare to about 49–53% median)
  - per-user CTR
  - opt-outs or unsubscribes per 100 pushes
  - users auto-paused for ignoring alerts
- **Trust:** no-show rate per seat claimed, reports per 100 games, endorsement participation (Overwatch saw 50–70%).
- **Possible target heuristics:** fill rate at or above 70% in prime time before investing in growth, and no-show rate under 10%. These are unsourced heuristics and should be marked as such in the report.

### Gaps
- No public LFG benchmarks were found (fill rate, time-to-fill, no-show).
- No benchmark for web-push CTR specifically in gaming was found.

---

## 6. LoL TR player base and Flex queue health (2025–2026)

### Takeaway
The key fact for PreBot: Riot does not allow ranked Flex parties of 4, only 1, 2, 3 or 5. A 4-stack must find a 5th or drop to 3 to play ranked, which validates the core use case. In the 2026 season Riot aligned Flex MMR more closely with Solo/Duo and now considers individual ranks within premades. Flex has no rank restrictions at Diamond and below. Five-stacks see slower queues. No reliable TR population numbers were found.

### Cited Findings
- **Flex party sizes:** "you can queue for Flex in groups of 1, 2, 3, and 5"; "parties of 4 will have to stick to unranked matches." — [Riot Support – Ranked Tiers, Divisions and Queues (page dated May 7, 2026)](https://support.riotgames.com/league-of-legends/gameplay/ranked-tiers-divisions-and-queues)
- **Flex rank restrictions:** "there are no Flex rank restrictions for Diamond and below". To play with Master and above, you must be at least Emerald. — [Riot Support](https://support.riotgames.com/league-of-legends/gameplay/ranked-tiers-divisions-and-queues)
- **Riot "/dev: Ranked 2026" (Dec 1, 2025):**
  - Riot is "making changes to how Flex and Solo MMR are determined" and "bringing Flex ranks more in-line with Solo/Duo ranks".
  - "Flex will never pull Solo up."
  - The goal is that Flex games "feel a little more fair". Friends can still play together "(within reason)".
  - Master+ dodges count as a full loss.
  - Dodging no longer resets autofill.
  
  — [Riot dev post](https://www.leagueoflegends.com/en-us/news/dev/dev-ranked-2026)
- **Matchmaking details** (Dodge.gg guide, March 23, 2026):
  - Quad queue is disabled so a solo player isn't forced into the 4th spot.
  - Five-stacks are matched primarily against five-stacks, with an MMR offset if none are available.
  - Flex queue pop is "Slower (especially five-stacks)" and game quality "More variable" than Solo/Duo.
  - The matchmaker adjusts for large rank gaps within a party.
  
  — [Dodge.gg Ranked Flex Guide 2026](https://www.dodge.gg/en-US/lol/news/ranked-flex-guide-2026)
- **Flex accounts for individual ranks** within teams in 2026, to fix Gold Flex lobbies meeting Master-level solo players. — [GosuGamers](https://www.gosugamers.net/lol/news/77660-league-of-legends-will-roll-out-2026-ranked-changes-soon-with-harsher-dodging-and-lp-protection)
- **Historical context (2020):** Patch 10.15 removed most Flex rank restrictions. — [Upcomer](https://upcomer.com/?p=59980); [Dexerto](https://www.dexerto.com/league-of-legends/riot-reveals-major-flex-queue-changes-coming-in-league-patch-10-15-1391185)
- **Solo/Duo (2026):** duo was re-enabled across ranks per the 2026 changes, but sources conflict on Master+ duo details. Riot Support currently says Grandmaster/Challenger "Duo is not available". — [Riot Support](https://support.riotgames.com/league-of-legends/gameplay/ranked-tiers-divisions-and-queues); [GosuGamers](https://www.gosugamers.net/lol/news/77660-league-of-legends-will-roll-out-2026-ranked-changes-soon-with-harsher-dodging-and-lp-protection)
- **Global player base:** 130–160 million monthly active players in 2025, per a third-party estimate with no regional breakdown. — [WeCoach 2025](https://wecoach.gg/blog/article/league-of-legends-player-count-and-statistics-2025)
- **TR ranked entries:** a LoLProfile TR statistics page snippet showed "24,901 Ranked Entries". The definition is unclear and the page returned 403 on fetch, so treat this as unreliable and likely only tracked accounts. — [lolprofile.net/statistics/tr](https://lolprofile.net/statistics/tr)

### Inferences (actionable)
**Lead with the 4-stack rule**
- Use it in onboarding and landing copy: "4 kişiyle Ranked Flex'e giremezsiniz; 5.yi 2 dakikada bulun."
- This is a concrete, Riot-confirmed pain point that makes the value proposition "dead simple" in Chen's terms.

**Listing types**
- The most valuable listing is "4 → need 1". "3 → need 2" is also useful (a 3-stack can already queue ranked, so urgency is lower).
- Show different urgency in notifications ("4 kişi bekliyor" vs "3 kişi").
- Consider allowing a "2 + 3 merge" listing (premade groups feature).

**Rank range validation**
- Enforce Riot's rule in the listing form: if the host's party includes Master+, the minimum rank must be Emerald.
- Below Diamond, no restriction applies, so let hosts set wide ranges and stress fun, or narrow ranges for climbing.
- Since 2026 the matchmaker accounts for individual ranks, so a big gap matters less for opponent fairness.

**Five-stack queue reality**
- Five-stacks face slower queues. Set expectations in the UI and suggest off-peak hours are slower still. This links to the prime-time concentration in section 2.

**Watch Riot changes quarterly**
- Flex rules shifted in 2020 and 2026. Monitor the Riot Support page and patch notes, and keep the rank-rule logic configurable.

### Gaps
- No reliable TR server population, TR ranked count, or Flex vs Solo share for TR was found. leagueofgraphs TR LoL distribution was not fetched; only TFT TR pages appeared.
- No Riot data on Flex queue health or population for 2025–2026 was found.
- No information on whether any 2026 patch (for example 26.x) changed Flex party sizes after the Dec 2025 post, beyond the May 2026 support page, which still says no 4-stacks.
