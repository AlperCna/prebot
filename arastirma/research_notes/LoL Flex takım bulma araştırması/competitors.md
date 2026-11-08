# Competitor Landscape: LoL LFG / Teammate-Finding Tools (Ranked Flex + TR1 focus)

Research date: 2026-11-08. Method: web search and page fetches. Several key sites (lolfinder.net, exitlag.com, top.gg bot pages, rdy.gg) returned HTTP 403 to the fetcher, so some claims rely on search-result snippets and are marked as such. "Verified" below means I read it on the product's own page or store listing on the research date. Everything else is a marketing claim or a third-party claim.

## What do the named products offer, and are they alive today?

### Takeaway
The market is full of generic multi-game LFG apps: Teamplay.gg, GameTree, Noobly, GamerLink, Premate.gg, E-Pal. There are also LoL duo-finder Discord bots: Duoo.gg/Miko, Queue Up!, and the GameTree Scout bot. Most still look alive in late 2026, judging by recent app updates. Almost all are built around one-off duo or "gamer friend" matching. Only Teamplay.gg clearly offers scheduled sessions with rosters. I could not verify that several named products exist at all: GankPls, TapTeam, GameVerse, FindPre, and LoLFinder.net (the site blocked fetches).

### Cited Findings

**Teamplay.gg (multi-game LFG website + Discord bot + Android app): ALIVE**
- Homepage claims "40K+" players and "23K+" LFG posts and covers 28+ games, LoL included. It says it is "Free forever. No credit card." Android app is on Google Play, iOS is "Coming soon." — [teamplay.gg](https://teamplay.gg) (fetched 2026-11-08; numbers are self-reported)
- Supports scheduled sessions: "Choose a time, invite friends and find the missing teammates." Sessions have a start time and a team roster, and players can match by game, rank, platform and schedule. — [teamplay.gg](https://teamplay.gg)
- The homepage mentions no rank verification and no reputation or no-show mechanics. — [teamplay.gg](https://teamplay.gg)
- Discord server listing: the search snippet showed about 4,750 members in one snapshot and about 6,812 in another, with "23,000+ players on the platform." The bot lets users create and browse LFG posts inside Discord, and members get pinged for new LFGs in their game roles. — [Discord: teamplaygg](https://discord.com/servers/teamplaygg-1042150011422658750); [Discord: teamplay-gg](https://discord.com/servers/teamplay-gg-1042150011422658750)
- Brand profile page exists. — [parse.gl/brands/teamplay-gg](https://parse.gl/brands/teamplay-gg)

**LoLFinder.net: UNVERIFIED**
- The site returned HTTP 403 to automated fetches, and a web search returned no third-party coverage, reviews or Reddit mentions. — [lolfinder.net](https://lolfinder.net) (403)

**GameTree (multi-game "gamer friends" app + Discord Scout bot): ALIVE**
- Free app from GameTree PBC. It markets "AI matchmaking" and a Discord bot for forming squads. Google Play shows about 1.5M installs and an update dated Mar 18, 2026. Ratings: 3.51★ on Google Play and 4.26★ on iOS (per the search snippet). — [appgoblin GameTree Android](https://appgoblin.info/apps/com.gametreeapp); [App Store](https://apps.apple.com/us/app/gametree-find-gaming-friends/id1181404496)
- The GameTree LFG Discord server lists 26,835 members and points users to its "Scout" bot. The full app promises "deeper personality matching." — [Discord: GameTree LFG](https://discord.com/servers/gametree-lfg-524219697236017163)
- Some App Store reviews report crashes on load. — [App Store](https://apps.apple.com/us/app/gametree-find-gaming-friends/id1181404496) (via search snippet)

**Noobly (swipe-style gamer-friend app): ALIVE**
- Developer: Jakub Stachecki. Rated 4.5★ from 975 ratings. Latest version is 0.3.08, released "3d ago" (early Nov 2026); the previous version, 0.3.06, came out Oct 20. Claims support for 2,500+ games. Uses swipe browsing and a time-zone availability indicator. — [App Store: Noobly](https://apps.apple.com/us/app/-/id1587868646)
- Monetization runs on an "Energy" currency: packs cost $0.99–$9.99, and "Power-Up" subscriptions are $5.99/week, $13.99/month and $19.99/quarter. — [App Store: Noobly](https://apps.apple.com/us/app/-/id1587868646)
- Reviewers complain that invites are capped (10 normal, 2 power-up) and say the app "feels like a dating app" and has "creeps." Age verification was added in v0.2.79 for some regions. Users can block and report. There is no rank or game-account verification. — [App Store: Noobly](https://apps.apple.com/us/app/-/id1587868646)

**GamerLink (LFG app, Xbox/PS/PC/mobile): ALIVE but weak**
- The listing says the app has been "refined for over 6 years." Latest version is 5.1.5, released "2 days ago"; earlier releases were 5.1.4 (26 Oct) and 5.1.3 (28 Sep). The description claims both "300" and "600+" games. — [App Store (UZ): GamerLink](https://apps.apple.com/uz/app/id974772739)
- That storefront shows only 1 rating (2.0★), and the review complains about ads. Privacy labels show tracking data used for third-party advertising, so the app appears to be ad-funded. It is focused on console shooters (Destiny, Apex, CoD), not LoL. — [App Store (UZ): GamerLink](https://apps.apple.com/uz/app/id974772739)

**Duoo.gg / "Miko by Duoo.gg" Discord bot (LoL duo finder): ALIVE (bot listed)**
- The top.gg listing advertises "Verified Ranks | /LFG | Duofinder | Verification | Smurfs Support." Once a user confirms account ownership, the bot gives them a rank role in the server. It also offers /profile, duo requests and duo reviews. — [top.gg: Miko by Duoo.gg](https://top.gg/bot/1096340606235717642) (search snippet; page 403)
- An affiliated article claims a custom rating system that scores level of play, communication, friendliness and attitude. It also says the bot supports multiple accounts across regions (smurfs). — [TheXboxHub: Duoo.gg](https://www.thexboxhub.com/1-best-way-to-find-any-duo-for-lol-duoo-gg/) (promotional)

**Queue Up! | Duo Finder (Discord bot): ALIVE (listed)**
- Matches players on rank, lane, region, playstyle and mic preference using a "smart database." Its bot ID (1446819621133877288) suggests a late-2025 creation date (inference from the Discord snowflake). — [top.gg: Queue Up!](https://top.gg/bot/1446819621133877288) (search snippet; page 403)

**Premate.gg (multi-game LFG, Valorant-heavy): ALIVE**
- Free LFG for Valorant, LoL, CS2, PUBG Mobile and Brawl Stars. The FAQ says it supports "League of Legends duo and team finding." Verified players with a connected Riot account are highlighted. — [premate.gg FAQ](https://premate.gg/faq); [premate.gg about](https://premate.gg/en/about-us)
- I found no LoL-specific guide; the content is mostly about Valorant. — [premate.gg Valorant guide](https://premate.gg/en/guides/how-to-find-valorant-teammates)

**E-Pal (paid gaming companions): ALIVE**
- Users hire "ePals" to play with them, and LoL is among 100+ games. Monetization: "Buff" credit packs from $4.99 to $99.99 and a VIP subscription at $6.49/month (prices possibly outdated). — [App Store: E-Pal](https://apps.apple.com/us/app/id1515406923?l=en-us); [dealroom: E-Pal](https://app.dealroom.co/companies/epal)
- This is a different model: you pay for a companion's time, not peer matching. A review notes the shift away from an "attractive companion" model. — [write.as review](https://write.as/thechronicenquirer/h1companion-service-review-2-egirl-gg-epal-gg-h1)

**TapIn.gg (paid duo/companion matching): ALIVE**
- Paid service that matches players on preferences and in-game data. It publishes many SEO blog posts about LoL duo-finding. — [tapin.gg blog](https://tapin.gg/blogs/find-league-of-legends-duo-partner)

**DOR (clip.dor.gg, guild + clip platform): ALIVE**
- Pitches guilds with recurring weekly schedules for "ranked fives." Its blog was published Aug 4, 2026 and updated Oct 23, 2026. — [clip.dor.gg: ranked fives team finder](https://clip.dor.gg/en/blog/lol-ranked-fives-team-finder); [clip.dor.gg: LoL duo finder](https://clip.dor.gg/en/blog/lol-duo-finder)

**Gankster (scrim-finding for organized teams): ALIVE (funded)**
- Raised a $4.25M seed round in Dec 2022, co-led by Bessemer Venture Partners and Hetz Ventures. It started as a tool for pro and amateur teams to find scrims. — [Calcalist](https://www.calcalistech.com/ctechnews/article/syeswcawo); [BVP portfolio](https://www.bvp.com/portfolio/gankster)
- Has a paid "Gankster Elite" tier; an ExitLag promo handed out 100,000 one-month keys (undated). — [ExitLag blog (ES)](https://www.exitlag.com/blog/es/gankster-exitlag-alianza-con-1-mes-gratis-de-gankster-elite/)
- Monetized partly through ads (Nitro publisher showcase). — [Nitro blog](https://blog.nitropay.com/publisher-showcase-gankster/)

**Other names seen**
- RiftQ is described as a "purpose-built LoL LFG platform." Tracker.gg and OP.gg are said to keep LFG sections where users browse by rank, role and region. I could not verify any of this directly. — [ExitLag LoL LFG guide](https://www.exitlag.com/blog/lfg-league-of-legends/) (search snippet; page 403)
- Guild Order has a Turkish-language "LoL Takım Bulucu" page, which showed 0 listed teams at search time. — [guildorder.com/tr](https://guildorder.com/tr/games/league/teams)
- Curry.gg hosts team-recruitment pages, for example a Flex team aiming for Challenger, mostly on Western servers. — [curry.gg team page](https://curry.gg/en/league-of-legends/teams/3662-bloodspire)
- TurboSmurfs (an account seller) runs a LoL duo-finder page as a lead-gen funnel. — [turbosmurfs.gg/lol-duo-finder](https://turbosmurfs.gg/lol-duo-finder)
- Generic LoL Discord bots focus on stats or custom-game balancing, not LFG: Leagly, ELO.GG, Leaguecord, "league custom game helper" (MMR-balanced 5v5 customs), and the open-source mrtolkien inhouse_bot. — [discordbotlist Leagly](https://discordbotlist.com/bots/930924283599925260); [discordbotlist custom game helper](https://discordbotlist.com/bots/66gk64e7jqw664); [inhouse_bot](https://awesome.ecosyste.ms/projects/github.com%2Fmrtolkien%2Finhouse_bot)

### Inferences
- Teamplay.gg is the closest Western analog to a "scheduled Flex 5-stack" product: it has sessions, start times, rosters and a Discord bot. But it has no reputation or rank verification, and its numbers (40K players, 23K posts across 28 games) are thin per game and per region.
- Monetization across the category is ads (GamerLink, Gankster), virtual currency or energy gating (Noobly), paid companions (E-Pal, TapIn), or lead-gen for account/boosting sellers (TurboSmurfs). No one shows a clear LFG-only subscription business at scale.
- The proliferation of SEO blogs (tapin.gg, dor.gg, premate.gg, guildorder) suggests discovery happens mainly through search content. User-facing products are commoditized.

### Gaps
- **GankPls, TapTeam, GameVerse:** searches returned no results. They may be dead, renamed or obscure, and I could not verify that they exist.
- **LoLFinder.net:** the site blocks automated fetches (403), and I found no third-party coverage.
- **OP.GG / u.gg / Mobalytics / Blitz / Porofessor duo or LFG features:** I found no verifiable current page. The only claim that OP.GG has an LFG section comes from secondary sources (ExitLag, 1v9.gg). I could not confirm whether op.gg's duo finder still exists in 2026.
- **Discord member counts and Riot RSO use for Duoo.gg and Queue Up!:** the top.gg pages returned 403, so these are unconfirmed.

## Which tools support Flex 5-stacks or fixed rosters and scheduled sessions, rather than one-off duo posts?

### Takeaway
Almost everything is built for duo or one-off matching. Teamplay.gg (scheduled sessions with rosters) and DOR (guilds with weekly schedules) are the only consumer tools I found that explicitly model a time-boxed group. Gankster models fixed teams, but for scrims, not ranked queue. Riot's own history pushes in two directions: Flex itself is an open party queue, and the 2026 "Ranked 5s" experiment is a premade-5-only queue that runs only during limited windows.

### Cited Findings
- **Teamplay.gg:** "Choose a time, invite friends and find the missing teammates"; sessions have a start time and a roster. — [teamplay.gg](https://teamplay.gg)
- **DOR:** advises joining a guild and posting "a recurring weekly schedule" for five-player play. — [clip.dor.gg](https://clip.dor.gg/en/blog/lol-ranked-fives-team-finder)
- **Duo-only bots:** Queue Up!, Miko/Duoo.gg and TapIn are framed around duo. — [top.gg Queue Up!](https://top.gg/bot/1446819621133877288); [top.gg Miko](https://top.gg/bot/1096340606235717642); [tapin.gg](https://tapin.gg/blogs/find-league-of-legends-duo-partner)
- **Flex party rule:** a 4-player party cannot queue Flex; parties of 1, 2, 3 or 5 can. This creates a concrete "need a fifth / need a sub" problem. — [dodge.gg Ranked Flex Guide 2026](https://www.dodge.gg/news/ranked-flex-guide-2026) (via search snippet)
- **Flex rank restriction:** Riot removed rank restrictions on Flex queue parties, so players of any rank can play together. — [Upcomer](https://upcomer.com/?p=59980)
- **Flex MMR complaint:** players report that Solo/Duo MMR doesn't factor into Flex MMR, which produces wide skill disparities. — [devtrackers.gg thread](https://devtrackers.gg/leagueoflegends/p/2dd7c2f1-if-riot-wants-flex-to-be-taken-seriously-it-needs-some-serious-work)
- **Riot's Team Builder:** introduced in 2013 and permanently removed in early 2016 (patch 6.2 per one source). Riot's stated reason was that the new draft pick (NDP) already let players choose a role. — [SurrenderAt20: Introducing Team Builder](https://surrenderat20.net/2013/10/introducing-team-builder.html?m=1); [Runeterra archive: Why is Team Builder gone?](https://archive.runeterra.net/na/miscellaneous/tkdgGeEV)
- **Dynamic Queue → Flex:** Dynamic Queue (2016) was replaced in the 2017 season by Solo/Duo plus a new Flex queue, which also replaced Ranked Teams. — [SurrenderAt20: 2017 Ranked changes](https://surrenderat20.net/2016/10/riot-pls-ranked-pls-2017-season-ranked.html); [Fandom: ranked 5 team queue returns](https://www.fandom.com/articles/ranked-5-team-queue-returns-league-legends)
- **Ranked 5s experiment (2026):** reportedly announced May 27, 2026. It is a premade-5-only queue using Tournament Draft with its own LP ladder. Reported windows: June 26 to September 6, weekends only (one source) or Friday–Sunday 8 PM–1 AM (another). Riot reportedly did not confirm it would be permanent. — [GameGrin](https://www.gamegrin.com/news/league-of-legends-brings-back-ranked-5s-with-a-caveat/); [LoLTheory blog](https://blog.loltheory.gg/ranked-5s-lol/); [jeu.video (ES)](https://jeu.video/es/guide/como-desbloquear-ranked-league-of-legends-que-se-sabe); [rdy.gg](https://rdy.gg/en/lol/news/ranked-5s-league-of-legends-return) (403; title only)
- **Riot–Discord linking:** in supported regions, players can link Riot and Discord accounts to invite friends or share a lobby link. — [clip.dor.gg](https://clip.dor.gg/en/blog/lol-ranked-fives-team-finder)

### Inferences
- A Ranked 5s queue that only runs on weekend evenings directly rewards a scheduled, fixed-roster tool: a team must be assembled for a specific time window. That is exactly where existing duo bots don't help. Whether TR1 got Ranked 5s is unverified (see Gaps).
- Riot has twice built and removed in-client team-assembly features (Team Builder, Dynamic Queue, Ranked Teams). Riot could in principle re-enter the space, but its history suggests it prefers queue design to social and LFG tooling.

### Gaps
- I could not reach Riot's official Ranked 5s dev post, so the dates and windows conflict between secondary sources. TR1 availability is unconfirmed.
- I found no tool that tracks a persistent Flex roster: members, substitutes, attendance and shared Flex LP.

## Which have shut down or pivoted, and why?

### Takeaway
I found no founder post-mortems or shutdown news for the named LFG tools. Most named products still ship updates. The products I could not find (GankPls, TapTeam, GameVerse, FindPre) may be dead, but there is no evidence of why. Category-level failure signals do show up in what survives: monetization by gating (energy, invite caps), ads, pivots to paid companions, and dating-app drift.

### Cited Findings
- I found no shutdown reports for GankPls, Noobly or GamerLink. Noobly and GamerLink were updated in Oct–Nov 2026. — [App Store: Noobly](https://apps.apple.com/us/app/-/id1587868646); [App Store: GamerLink](https://apps.apple.com/uz/app/id974772739)
- Gankster: I found no shutdown news; it is listed as an active seed-stage company. — [startupim: Gankster](https://startupim.com/company/gankster); [CB Insights](https://www.cbinsights.com/company/gankster)
- Pivot signal, E-Pal: moved from an "attractive companion" model toward casual companions. — [write.as review](https://write.as/thechronicenquirer/h1companion-service-review-2-egirl-gg-epal-gg-h1)
- Monetization friction, Noobly: invite caps and paid boosts are the top review complaint. — [App Store: Noobly](https://apps.apple.com/us/app/-/id1587868646)
- Ad friction, GamerLink: the only review complains about ads. — [App Store: GamerLink](https://apps.apple.com/uz/app/id974772739)
- Adjacent category shutdown: Meta closed its standalone Facebook Gaming app. — [Engadget](https://www.engadget.com/meta-killing-standalone-facebook-gaming-app-054950726.html)
- Riot's own features (Team Builder, Dynamic Queue) were removed, citing redundancy with draft pick and imbalanced games from mismatched party sizes. — [Runeterra archive](https://archive.runeterra.net/na/miscellaneous/tkdgGeEV); [Dot Esports](https://dotesports.com/league-of-legends/news/ranked-queues-are-changingheres-what-you-need-to-know-4863)

### Inferences
- The products that survive are generic and multi-game. Scale across games compensates for thin per-game liquidity, which points to cold-start problems for single-game, single-region tools. This is an inference; I found no direct evidence.
- Swipe and "friend" apps drift toward dating dynamics and toxicity complaints. Ranked-focused tools avoid this by matching on competitive criteria.

### Gaps
- I found no founder post-mortems, Reddit shutdown threads, Crunchbase "closed" statuses or Riot API policy-related shutdowns for any LoL LFG tool. If needed, a targeted Reddit search (site:reddit.com "duo finder" shut down) is a next step.
- I found no evidence on Riot API or Developer Portal policy limits for LFG apps.

## What does the Turkish (TR1) ecosystem look like?

### Takeaway
Finding Flex teammates on TR1 happens in large general-purpose Turkish LoL Discord servers (about 7K–20K members) and on scattered, low-traffic forum threads. The only Turkish web product found, Mate4Game, is a generic duo-post board with no rank verification and no Flex or 5-stack support. I could not find the "FindPre" bot.

### Cited Findings
- **"League of Legends TR" Discord:** 19,585 members and 3,522 online at fetch time (Nov 2026); an earlier search snapshot showed 17,596 members and 984 online. It covers LoL, Valorant and TFT, tied to the TR server: "Takım ve oyuncu bulabilirsin!" It claims "advanced systems" to match users with teammates. There is no explicit pre or Flex feature. — [Discord: League of Legends TR](https://discord.com/servers/league-of-legends-tr-896475140039278592)
- **"League of Legends Türkiye" Discord servers:** several exist. One was founded in 2024, calls itself Türkiye's largest LoL community and lists about 7,228 members (search snippet). — [Discord 678949756982132756](https://discord.com/servers/league-of-legends-turkiye-678949756982132756); [Discord 1195405905576071168](https://discord.com/servers/league-of-legends-turkiye-1195405905576071168); [discordbotlist: lolturkiye](https://discordbotlist.com/servers/lolturkiye-1894/join)
- **Stratera:** self-described as one of Türkiye's largest LoL communities. — [discordbotlist: Stratera](https://discordbotlist.com/servers/stratera/upvote)
- **Wild Rift Türkiye:** about 12,178 members (mobile game, not PC LoL). — [Discord: Wild Rift Türkiye](https://discord.com/servers/league-of-legends-wild-rift-turkiye-766665106888851486)
- **Mate4Game (mate4game.com):** a Turkish-language board for finding gaming partners, with filters for game, platform, age, server (TR / EUW / EUNE), mic, and Discord or Instagram contact. It has a "ready to play now" section and shows last-active timestamps. A sample LoL post: "Duo lazım lane farketmez" on TR. Ranks are self-reported tags with no verification. There is no Flex or 5-stack support, and no user counts are shown. — [mate4game.com](https://mate4game.com)
- **DonanımHaber:** the thread "TR Server Flex e pre lazım" (Oct 2, 2017) got 5 replies, never resolved and names no tools, which shows how ad-hoc forum LFG is. — [DonanımHaber thread](https://forum.donanimhaber.com/tr-server-flex-e-pre-lazim--129212783)
- **"LoL Arkadaşları":** an iOS app listed on Tamindir (Turkish download site); details unverified. — [tamindir: LoL Arkadaşları](https://www.tamindir.com/ios/lol-arkadaslari/)
- **Guild Order (Turkish UI):** LoL team finder showed 0 teams. — [guildorder.com/tr](https://guildorder.com/tr/games/league/teams)
- **Turkish Discord bots found:** Preless, Ineffable and Türk Bot are moderation or fun bots, not LFG. — [discordbotlist: Preless](https://discordbotlist.com/bots/preless); [discordbotlist: Ineffable](https://discordbotlist.com/bots/ineffable)

### Inferences
- The demand signal is real (about 20K members in one TR server, about 3.5K online), but the tooling there is generic channel posts. A TR-native tool could plug into these existing servers as a bot instead of having to build an audience from scratch.
- Mate4Game's competitive moat in TR is mostly language and local server filters. It does not address Flex, rosters, reliability or verification.

### Gaps
- **FindPre:** the Turkish Discord bot was not found under any spelling I tried ("FindPre," "pre bul"). It may be private, renamed or defunct.
- **Disboard:** I found no TR LoL listings with member counts; search returned only a generic Turkish-language filter page. — [disboard.org TR filter](https://disboard.org/ko/servers?fl=tr)
- **Technopat Sosyal and Ekşi Sözlük:** I found no threads specifically about finding Flex teammates. Search returned unrelated entries.
- I found no r/LeagueOfLegendsTR or r/Turkey threads on this topic.

## Do any have anti-no-show, reputation, scheduling or substitute mechanics, and how do users review them?

### Takeaway
Reputation exists only in a thin form: Duoo.gg/Miko's post-duo reviews and multi-factor rating (a marketing claim), and Riot-account-verified badges (Duoo.gg, Premate.gg). Scheduling exists only in Teamplay.gg and DOR. I found no tool with explicit no-show penalties, attendance tracking, reliability scores or substitute/bench management, for duo or for Flex. This is the clearest gap.

### Cited Findings
- **Duoo.gg/Miko:** verified ranks via account-ownership confirmation, duo reviews, and a "custom rating system" (level of play, communication, friendliness, attitude). — [top.gg: Miko](https://top.gg/bot/1096340606235717642); [TheXboxHub](https://www.thexboxhub.com/1-best-way-to-find-any-duo-for-lol-duoo-gg/) (promotional)
- **Premate.gg:** verified players with Riot account connection are highlighted. — [premate.gg FAQ](https://premate.gg/faq)
- **Teamplay.gg:** has scheduling and rosters; reputation, verification and no-show handling are not mentioned. — [teamplay.gg](https://teamplay.gg)
- **Noobly:** has block/report and regional age verification, but no game-skill reputation. Reviews: 4.5★ (975), with complaints about invite caps, dating-app feel and "creeps." — [App Store: Noobly](https://apps.apple.com/us/app/-/id1587868646)
- **GamerLink:** 2.0★ (1 rating, UZ store), with complaints about ads and bugs. — [App Store: GamerLink](https://apps.apple.com/uz/app/id974772739)
- **GameTree:** 3.51★ on Google Play and 4.26★ on iOS, with crash complaints. — [appgoblin](https://appgoblin.info/apps/com.gametreeapp); [App Store](https://apps.apple.com/us/app/gametree-find-gaming-friends/id1181404496)
- **Mate4Game:** last-active timestamps and an online indicator are its only liveness signal; it has no reputation features. — [mate4game.com](https://mate4game.com)
- **DOR:** suggests Riot friends and guild members after games, and warns that one clip shouldn't be treated as proof of skill. — [clip.dor.gg](https://clip.dor.gg/en/blog/lol-ranked-fives-team-finder)

### Inferences (the gap)
No existing product combines all of the following:
1. A fixed Flex roster (5 core players plus subs) with persistent identity.
2. Scheduled sessions tied to time windows. This matters more now that Ranked 5s runs only on weekend evenings, and Flex bans 4-player parties, so a no-show breaks the whole session.
3. Reliability or no-show reputation: attendance rate, late cancellations.
4. Automatic substitute calling from a vetted bench at a similar rank.
5. Riot RSO-verified rank (Flex rank specifically, not just Solo/Duo).
6. Turkish-language, TR1-native operation inside existing large TR Discord servers (about 20K members).

Teamplay.gg covers (2) generically, Duoo.gg covers part of (3) and (5) for duo, and Mate4Game covers (6) for duo posts. No one covers (1) + (3) + (4) for Flex, and nothing covers all six for TR1.

### Gaps
- I could not retrieve Reddit user sentiment (r/leagueoflegends, r/summonerschool) about specific LFG tools, so user reviews come only from app-store listings.
- Duoo.gg's actual review volume and active-user numbers are unknown; the top.gg page returned 403.
