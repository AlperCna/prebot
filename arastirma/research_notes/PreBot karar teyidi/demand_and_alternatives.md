# PreBot decision check: demand evidence and alternatives (TR, Ranked Flex, "need a 5th now")

Research date: 9 Nov 2026. Method: web search plus direct reading of pages in a real browser. This got past the 403 blocks that limited earlier rounds on Technopat, LoLFinder, Disboard and op.gg leaderboards. Ekşi Sözlük in-title search and LeagueOfGraphs (Cloudflare challenge) were still not readable, and Reddit could not be fetched.

Labels: **[V]** means verified on the live page on 9 Nov 2026. **[S]** means search snippet only. **[I]** means my inference.

---

## 0. Bottom line

- **Demand: real, recurring and small, at about 3–5 public posts a day.** The exact scenario ("4 kişiyiz 1 kişi daha lazım hızlı", roughly "we're 4, need 1 more, quick") appears every day on one board, LoLFinder.net TR Flex. It also shows up a few times a month on Technopat. The posts are urgent in tone ("acil", "hızlı"). Structurally, TR is Flex-heavy: about 598k Flex-ranked accounts against 859k Solo, a ratio of 70%, compared with 45% on EUW. That is strong evidence that the pain exists. It is weak evidence that many people would move to a new tool, because public posting volume is modest. [V + I]
- **Competitor threat: moderate, not fatal.** One product already serves TR Flex: **LoLFinder.net**. It has a Turkish UI, a TR region, a dedicated Flex board, Riot API rank pulls, and is free. TR is one of its most active Flex regions (39 pages, against 43 for EUW). But it is an **asynchronous board**. Posts live for **6 months**, and there is no "available now" state, no push to matching players and no seat-claim. An "acil 1 lazım" post sits there for days. This is exactly the gap PreBot targets. No product found does real-time push to available players for TR. [V]
- **Verdict:** Building PreBot is justified as a small, measured community product. The core bet ("the post finds you within minutes") is unserved. The main risk is **liquidity at the same hour**, not competition. The cheapest way to seed demand is where the posts already appear, LoLFinder TR and Technopat. [I]

---

## 1. Evidence of demand

### 1a. LoLFinder.net, TR Ranked Flex board (strongest evidence)
- [V] https://lolfinder.net/tr/flex/region/tr: the page header reads "Region: Turkey | Find a partner for Flex Queue". It has 39 pages of 12 posts each, so about 465 active posts.
- [V] Post ages, as a timeline:
  - Page 1 covers 3 hours to 5 days.
  - Page 3 is about 1 week old.
  - Page 8 is about 1 month old.
  - Page 20 is about 2 months old.
  - Page 30 is about 3 months old.
  - Page 39 is 5–11 months old.
- [I] That works out to about **100–120 TR Flex posts a month, or 3–5 a day**. Volume rose in the last 3 months.
- [V] Typical texts from pages 1–8 (the last month). About half of the unique comment lines are group-side requests for 1 or 2 players:
  - "4 kişiyiz 1 kişi daha lazım hızlı" ("we're 4, need 1 more, quick")
  - "acil esnek +1 lazım, seri gelin, koridor fark etmez" ("urgent, need +1 for flex, come fast, any lane")
  - "LİG FARK ETMEZ ROL FARK ETMEZ ESNEK 1 KİŞİ LAZIM" ("rank doesn't matter, role doesn't matter, need 1 for flex")
  - "esnek atçaz 5. lazım" ("we're queueing flex, need a 5th")
  - "acil 5 lazım" ("need a 5th, urgent")
  - "2 KİŞİ LAZIM ACİL" ("need 2, urgent")
  - A 3-friend group (ages 24/28/31) looking for 2 more who play "after 8–9 PM".
- [V] One TR post carried a Riot party join link (`gg.riotgames.com/LOL?joinCode=…`). Riot's Discord party links officially launched only in the US, Canada and Brazil. Discord's blog says they are available "where Riot and Discord are both supported". So TR availability is **unconfirmed**. Some TR users may be generating these links anyway, for example through a VPN. [V observation, I interpretation]
- Comparison on the same site [V]:

  | LoLFinder board | Pages | Notes |
  |---|---|---|
  | EUW Flex | 43 | Page 1 covers 4 hours to 1 week |
  | EUNE Flex | 18 | |
  | All regions, Flex | 150 | TR is about 26% of the total |
  | TR Duo ("parties") | 106 | Page 1 covers 3 hours to 1 day, about 12 a day |
  | TR Normal | 4 | |
  | TR Teams | 6 | |

  - [I] TR players post far more for Flex than for Normal. "Just play Normal instead" is not what they reach for.

### 1b. Technopat Sosyal
- [V] Subforum "Oyun Arkadaşı" (bolum 354), 301 threads, mostly 2025–2026. **8 LoL threads in 2026**, which is only about 1 a month:
  - "LoL EUW 5. Player ormancı arıyoruz, elo Gold civarı" ("looking for a 5th player, jungler, around Gold elo", 18 Sep 2026; TR players on EUW)
  - "LoL turnuvaya oyuncu aranıyor" ("player wanted for a LoL tournament", 6 Sep 2026)
  - Two Clash team searches (19 Jul 2026)
  - A Jan 2026 reply: "arada arkadaşlarla 5'li giriyoruz… oyuncu bulursak Flex5'li atıyoruz" ("sometimes we go in as a 5 with friends… if we find a player we queue Flex as a 5"). This is the exact scenario: a friend group that queues Flex only when it finds an extra player.
  - A Mar 2026 post: "Discord kapandığından beri kullanmıyorum" ("I haven't used it since Discord was closed"). This is a direct ban effect.
  - Sources: https://www.technopat.net/sosyal/konu/lol-euw-gold-elo-icin-ormanci-oyun-arkadasi.4178529/ · https://www.technopat.net/sosyal/konu/lol-valorant-vs-oynayan-oyun-arkadasi-araniyor.4016347/ · https://www.technopat.net/sosyal/konu/lol-west-takim-arkadasi.4065471/
- [V] MOBA subforum (bolum 61): of 500 recent threads (468 from 2025–2026), only 1 is a LoL teammate request. There is also a thread on whether to grind Flex or Solo (5 May 2025). Replies there: "Ekibin varsa esnek yoksa tek çift" ("if you have a team, Flex; if not, Solo/Duo"), and "Esnek girenlerin çoğunluğu arkadaş grubu" ("most people in Flex are friend groups"). https://www.technopat.net/sosyal/konu/esnek-kasmak-mi-daha-iyi-yoksa-tek-cift-mi.3758039/
- [V] The "lol" tag, May–Oct 2026 (about 24 threads): no teammate requests at all, mostly technical and ban topics. Forums are not where TR players look for a 5th.

### 1c. DonanımHaber, Ekşi Sözlük, Reddit, X, TikTok, Instagram
- [V] DonanımHaber LoL forum (f677), front page of 2025–2026 threads: **no** pre or 5th requests. Only 2017–2019 era threads exist (cited in earlier notes).
- [V/absent] Ekşi Sözlük: a title search for "esnek dereceli" returned 0 titles. In-title entry search could not be run.
- [absent] Reddit, X, TikTok and Instagram: search turned up no indexed TR "esnek 5. lazım" posts. A Twitch stream title pattern "Esnek giriyoruz – nick bırakın" ("going into Flex, leave your nick") was seen in a snippet [S]. Streamers fill Flex slots from their viewers. **Absence here is a search-index limit, not proof of absence.**
- [V] OP.GG Duo, TR region (duo.op.gg/en/lol/?region=tr): about 40 posts over 19 days, newest 2 days old, nearly all duo requests. One poster says they are 13. OP.GG's board is quiet in TR.

### 1d. Structural demand drivers
- [V, earlier notes] Ranked Flex allows parties of 1, 2, 3 or 5, so a 4-stack cannot queue. This is unchanged in the Riot "Dev: Ranked 2026" post (1 Dec 2025). That post only aligns Flex MMR closer to Solo, with the caveat "Flex will never pull Solo up". https://www.leagueoflegends.com/en-us/news/dev/dev-ranked-2026
- [V] op.gg TR ladder on 9 Nov 2026 shows **597,634 Flex-ranked summoners against 859,314 Solo, about 70%**:

  | Region | Flex | Solo | Ratio |
  |---|---|---|---|
  | TR | 597,634 | 859,314 | about 70% |
  | EUW | 1,673,277 | 3,699,257 | 45% |
  | EUNE | 917,190 | 1,578,569 | 58% |

  - [I] TR is unusually Flex-heavy. These are season-cumulative ranked accounts, not daily active players.
  - Source: https://op.gg/lol/leaderboards/tier?region=tr&type=ladder-flex
- [I] The 4-to-5 gap is therefore a frequent moment in a large Flex population. Most of those groups solve it privately, by asking friends of friends, so it never becomes a public post. Public posts are the tip of the iceberg, but how big the iceberg is cannot be measured from outside.

---

## 2. Current workarounds and their pain points

| Workaround | Evidence | Works? | Pain points |
|---|---|---|---|
| Ask friends / friends-of-friends, in-game friend list | Implied by "Ekibin varsa esnek" and "Esnek girenlerin çoğunluğu arkadaş grubu" [V] | Best when it works (trust, voice) | Limited pool. Fails late at night. Not measurable [I] |
| LoLFinder.net TR Flex board | 3–5 posts/day [V] | Partly: it is the main public venue | 6-month post life. No "online now". No push. You hand out your Riot ID and wait for friend requests. Posts are duplicated (original plus auto-translation). Ads [V] |
| Discord servers (VPN/DNS) | Disboard TR LoL tag: about 22 small servers bumped within minutes to days. Online counts 21–1,381. One advertises "Her eloya uygun pre bulma kanalları" ("pre-finding channels for every elo") [V]. Official LoL Türkiye page still shows 138,447 members / 20,958 online, probably a stale snapshot [V/I]. People still swap "DC: nick" in Oct 2026 [V] | For the VPN core only | The ban means new users can't join. "GoodbyeDPI … bir süredir bu işe yaramıyor" ("GoodbyeDPI hasn't worked for a while", Aug 2026 Technopat [V]). Some users quit Discord entirely [V]. "DC çöplük bir yer" ("DC is a dump", Oct 2026 [V]) |
| Forums (Technopat, DH) | about 1 post/month [V] | No: too slow | Replies come hours to days later. Rank mismatch (e.g. "Bronze 1 benim maalesef", "unfortunately I'm Bronze 1") [V] |
| Drop to 3 or play Solo/Duo instead | "Ekibin varsa esnek yoksa tek çift" [V] | Fallback | Breaks the 4-friend session. One friend sits out [I] |
| Play Normal instead | LoLFinder TR Normal board is only 4 pages, against 39 for Flex [V] | Fallback | Players clearly prefer ranked Flex. Normal doesn't need a 5th urgently [I] |
| Streamer viewer fill ("nick bırakın") | Twitch title [S] | Niche | Only for streamers |
| Turkish voice apps (Micup, Descall) | Micup recommended in Oct 2026 [V] | Unknown | No LFG layer seen [I] |
| WhatsApp / Telegram groups | **No evidence found** | Unknown | Not observable [absent] |

**Best current option:** LoLFinder for strangers, and friends' networks for trusted players. **Shared pain:** none of them tells a ready, matching player *right now* that a seat is open, and none closes the seat.

---

## 3. Competitors and alternatives (TR relevance)

| Product | TR region | Turkish UI | Flex-specific | Real-time push to available players | Status / notes |
|---|---|---|---|---|---|
| **LoLFinder.net** | **Yes** [V] | **Yes** [V] | **Yes**, a dedicated board [V] | **No.** Static listings, 6-month life, optional paid bump in USDT [V] | **Most relevant competitor.** Active in TR. Russian-origin (default RU UI), with Telegram and Instagram channels [V] |
| RiftQ | **No** (EUW/EUNE/NA/ME/BR only) [V] | No | Yes ("spots left") | No | Discord login required, which is blocked in TR [V] |
| OP.GG Duo | Yes [V] | Partly | Has a Flex filter [V] | No | Quiet in TR: about 2 posts/day, all queues [V] |
| GameVerse | No TR in its region list (earlier notes) | No | Yes (5-stack) | No | Earlier notes |
| TeamTavern | Location filter | No | Groups | Match alerts, but 30-day posts | Earlier notes, not checked for TR |
| Premate.gg | Unclear | Has TR presence per a 2020s forum reply | Team finding | No | Earlier notes |
| GameTree, Teamplay.gg, Noobly, GamerLink, Gankster Duos ("Play Now", duo only) | Global | No | No | Gankster Duos only, and for duos | Earlier notes |
| Discord LFG bots (Miko/Duoo, Queue Up!) | Inside Discord | No | Duo-centric | DM-based | Blocked in TR without a VPN |
| **Riot** | n/a | n/a | n/a | **No PC LFG.** Wild Rift has Party Finder. RU "Find Friends" was an old test. Discord party links (26.8) are US/CA/BR. "League Next" client is due 2027 [V/S] | **Platform risk:** a native LFG or party-link rollout in a 2027 client would weaken any third-party board [I] |
| Ranked 5s (op.gg lists a "Ranked 5s" ladder) [V] | TR | | Needs a full 5 | | Out of scope, but it competes for and adds to 5-stack demand [I] |

**What all of them lack for this scenario:**
- An "I'm available for the next X minutes" state with role and rank.
- A push to matching available players.
- First-tap seat claim and short post expiry.
- TR-first operation without Discord.

PreBot's planned design covers exactly these. [I]

---

## 4. Discord in Turkey (Nov 2026)
- [V/S] Still blocked. It has been blocked since an Ankara court order of 8–9 Oct 2024. The latest news found (late Oct 2026, Politikam and Habertürk) says the block continues, with no reopening date.
  - The minister said in Jun 2026 that it would "muhtemelen" (probably) reopen.
  - NTV (10 Feb 2026) linked hopes to Discord's global age verification from Mar 2026. That did not lead to reopening.
  - Sources: https://www.ntv.com.tr/turkiye/galeri-discord-turkiyede-acilacak-mi-kuresel-yas-dogrulama-karari-sonrasi-gozler-o-tarihte-1711249 · https://www.politikam.com/discord-ve-roblox-turkiyede-acildi-mi-ekim-2026-itibariyla-erisim-engeli-hala-devam-ediyor-tarih-bekleniyor · https://www.haberturk.com/discord-son-durum-discord-acildi-mi-acilacak-mi-son-durum-nedir-discord-erisim-engeli-kalkti-mi-3893897
- [V] Usage continues through workarounds:
  - Disboard TR LoL servers are bumped within minutes.
  - Technopat users still swap Discord tags.
  - Bypass tools are getting less reliable: GoodbyeDPI "işe yaramıyor" ("doesn't work"), Aug 2026, and DPI tools cause disconnects in LoL and DC (Jun 2026).
- [absent] No figure exists for how many TR players still use Discord. The share is unknown. This is consistent with earlier reports.
- [I] Discord still holds the hardcore community, but LoLFinder's TR traction shows that many players already use a non-Discord web board. That supports PreBot's web-first choice.

## 5. Flex queue health on TR
- [V] op.gg, 9 Nov 2026: 597,634 TR Flex-ranked summoners against 859,314 Solo, about 70%. EUW is 45% and EUNE 58%. The Flex ladder has Challengers at about 1,500–3,800 LP, so the top of the ladder is populated.
- [absent] No 2025–2026 TR Flex queue-time complaints were found on Technopat, DH or Ekşi in this pass. The only Flex complaint type seen is stomps by Solo-high players in Flex lobbies, which Riot addressed in Ranked 2026.
- [I] Flex is healthy on TR. Liquidity risk for PreBot comes from concentrating enough *available* people in the same 15 minutes, not from the size of the Flex queue.

## 6. Risks (why PreBot could fail)
1. **Liquidity at the same hour [I, strongest].** About 3–5 public posts a day spread over 24 hours means very few concurrent seekers. PreBot needs fillers who mark themselves available, and the demand side alone won't seed them. LoLFinder's Duo board (about 12 posts a day) suggests a larger pool of *individuals* who want partners. Those people are the natural fillers to recruit.
2. **Friends-first culture [V signals].**
   - "Ekibin varsa esnek" (if you have a team, play Flex).
   - Suspicion of strangers' "pre": a 13-year-old on OP.GG asking for a duo; "Toxic ve presi olanlar eklemesin" ("toxic people and people with a pre shouldn't add me").
   - Many groups would rather drop to Solo than take a stranger.
3. **Incumbent "good enough" board [V].** LoLFinder is free, Turkish, indexed by Google, and already the habit. PreBot must be clearly faster, not just different.
4. **No-shows and toxicity [I].** Not directly evidenced for TR in this pass. Earlier notes treat it as plausible. Rank-range mismatch, however, is evidenced.
5. **Platform risk [S/I].** Riot's party links or "League Next" (2027) could add native LFG. A Discord reopening would revive Discord bots.
6. **Minors / child safety [V signal].** Self-declared 13-year-olds post on these boards, and the Discord ban was itself justified on child-safety grounds.
7. **Flex unpopular? [V: no].** TR is Flex-heavy, and Normal LFG volume is tiny. This risk looks low.

## 7. Implications for the decision [I]
- The problem is real and frequent, with a daily stream of explicit "4 kişiyiz 1 lazım" posts. But public demand is **niche-sized**: hundreds of posts a month, not thousands.
- Positioning: **"the urgent version of LoLFinder for TR"**. That means available-now pool, push, first-tap seat and 15-minute expiry. LoLFinder posts that say "acil" and are 5 days old are the clearest proof of the gap.
- Launch seeding:
  - Monitor the LoLFinder TR Flex board as an observation and outreach source, respecting its terms, with no scraping or spam.
  - Recruit fillers from TR Duo seekers.
  - Target the 20:00–01:00 window ("after 8–9 PM" also appears in posts).
- Success metric is unchanged from earlier reports: fill rate of a "1 kişi lazım" post within 10–15 minutes at evening peak.

## 8. Gaps
- No private-channel data (WhatsApp/Telegram, in-game friend lists, Discord message volume). It is unknown whether LoLFinder TR posts actually get filled, or how fast.
- No TR Flex queue-time data. No 2026 Riot TR player count.
- It is unconfirmed whether Riot party links work in TR (one joinCode link was seen in a TR post).
- Ekşi, Reddit, X, TikTok and Instagram could not be meaningfully searched.
