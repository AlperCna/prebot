# Competitors and distribution: where TR LoL players find a missing teammate (Nov 2026)

Research date: 9 November 2026. Member and online counts are snapshots from public listing pages on that day, unless a different date is noted. Several key sites (lolfinder.net, top.gg, disboard.org) returned HTTP 403 to automated fetching, so some figures come from search-result snippets or other directories and are marked as such.

## 1. Biggest Turkish LoL Discord servers, their LFG channels, and the effect of the Oct 2024 ban

### Takeaway
Turkish LoL Discord is still large in November 2026 despite the ban. The "official" League of Legends Türkiye server shows about 138k members and about 15k online, and has an LFG category. So "hundreds online in the evening" understates the scale. But the online count is server-wide, and no public source shows how many LFG messages these channels get per evening, so actual LFG volume is unverified.

### Cited Findings
**Server snapshots (members / online)**
- **League of Legends Türkiye** (ID 678949756982132756): 138,448 members, 15,210 online. Created 17 Feb 2020. It calls itself the official Discord server for LoL in Turkey and lists "LFG" as a server category ("who's down to join your next match"). — [Discord server page](https://discord.com/servers/league-of-legends-turkiye-678949756982132756)
- The same server on Discord Bot List shows 138,447 members and 14,885 online. Its description mentions chat rooms, fan corners and FAQ rooms, but not LFG/duo/flex channels by name. — [Discord Bot List: lolturkiye](https://discordbotlist.com/servers/lolturkiye-1894)
- **League of Legends TR** (ID 896475140039278592): 19,587 members, 2,827 online. Created 9 Oct 2021. Covers LoL, Valorant and TFT, and says "Takım ve oyuncu bulabilirsin!" It also says players without a team can use "the server's systems" to match with a suitable teammate, which suggests it already runs its own matching bot or system. — [Discord server page](https://discord.com/servers/league-of-legends-tr-896475140039278592)
  - An earlier search snippet showed about 17k, and later 18,003, members for this server, so it seems to be growing. — [Discord server page (search snippets)](https://discord.com/servers/league-of-legends-tr-896475140039278592)
- **Stratera** (streamer Dora "Stratera" Özsoy's official server): 24,692 members, 713 online. Calls itself "one of Turkey's largest LoL communities", with guides, tournaments and pro streams. No explicit LFG channel is mentioned, and joining requires a captcha. — [Discord Bot List: Stratera](https://discordbotlist.com/servers/stratera)
- **Naru** (streamer Koray "Naru" Bıçak): 7,980 members, 349 online. Created 17 Mar 2019. Says members can "find teammates for League of Legends and Valorant." — [Discord server page](https://discord.com/servers/naru-556923338568761354)
- **League of Legends Türkiye** (ID 1195405905576071168, a different, unofficial server): 7,359 members, 766 online. Created 12 Jan 2024. Calls itself "Türkiye'nin en büyük League of Legends topluluğu" and offers finding "oyun arkadaşı" and in-game teams, ranked/ARAM, polls, mini tournaments and prizes. — [Discord server page](https://discord.com/servers/league-of-legends-turkiye-1195405905576071168)
- **LoL Team Finder** (global, all regions): 12,532 members, with Solo/Duo, Flex, Clash and scrim channels. This figure is from a search snippet and was not re-fetched. — [Discord server page](https://discord.com/servers/lol-team-finder-643452664180506654)
- A small Turkish server, "lol ve ötesi", is listed on Disboard under the "flex" tag with the pitch "flex atıyoruz" and the tags "flex" and "türk". This is from a search snippet; Disboard returned 403 to direct fetch. — [Disboard tag: flex](https://disboard.org/servers/tag/flex?fl=hu)

**Ban status (verify: still in force)**
- The block comes from an Ankara 1st Criminal Judgeship of Peace decision dated 9 Oct 2024 (no. 2024/12907), under Article 8 of Law 5651. On 5 June 2026, Transport Minister Uraloğlu said Discord had met Turkey's content-removal criteria and "Muhtemelen bir müddet sonra açacağız." He gave no date. — [Sözcü, 5–6 Jun 2026](https://www.sozcu.com.tr/bakan-uraloglu-acikladi-discord-bir-sure-sonra-erisime-acilacak-p325599)
- A page dated 6 Nov 2026 says Discord is still blocked, with no official decision lifting the ban. This source is self-interested: the page is published by Descall, a Turkish Discord alternative. — [Descall](https://descall.com/tr/discord-acildi-mi)
- OONI data for 28 Sep to 11 Oct 2026: 1,518 of 2,070 Discord measurements (73.3%) were blocked or anomalous. By ISP: Turkcell 100% (75/75), Superonline 99.1% (318/321), Türk Telekom 96.9% (590/609), TurkNet 38.4% (107/279). OONI measurements come from volunteers, are not representative, and "anomalous" does not prove a block. — [Teknoroid, Oct 2026](https://www.teknoroid.net/2026/10/turkiyede-discorda-erisim-saglayiciya-gore-degisiyor-ooni-verileri-ekim-2026.html)
- DNS changes work on some connections but not on every operator. — [Webmasto (search snippet)](https://webmasto.com/android-ios-ve-pcde-discorda-nasil-girilir-vpnsiz-kolay-yontem)
- Reports since 2024 say Turkish users kept reaching Discord through VPNs. VPN use sits in a legal grey area, and many VPN sites are themselves blocked: 18 of 28 OONI measurements of NordVPN's site were blocked or anomalous. — [CyberInsider](https://cyberinsider.com/discord-blocked-in-russia-and-turkey-amid-government-crackdowns/); [Teknoroid](https://www.teknoroid.net/2026/10/turkiyede-discorda-erisim-saglayiciya-gore-degisiyor-ooni-verileri-ekim-2026.html)

### Inferences
- About 15k online in one Turkish LoL server, two years into a ban, shows that the core TR LoL audience (PC players, who already handle VPN/DNS) is still on Discord. The ban did not empty these communities.
- "Online" on Discord includes idle and background clients, so 15k online does not mean 15k people looking for a game. Still, even a small share of that in an LFG channel is far more than LoLFinder's earlier figure of about 3–5 posts per day. In absolute volume, Discord is the bigger competitor.
- League of Legends TR (about 19.6k members) already advertises its own matching "systems." A new bot there would be replacing an incumbent, not filling a gap.
- The OONI split matters for PreBot's core feature. Turkcell (mobile) is 100% blocked, so a phone notification delivered through the Discord app only reaches users with an always-on VPN or DNS setup. A web push from a PWA has no such dependency. This is a structural advantage the claim under test does not mention.

### Gaps
- LFG channel activity (messages per evening, how many are Flex "5. lazım" posts) could not be measured. It would require joining the servers and sampling channels at 20:00–24:00.
- I found no pre-ban (Sep 2024) member or online counts for these servers, so I cannot say whether the ban shrank them. Discord Bot List and Discord Discovery show only current numbers, and no Similarweb data for Discord traffic from Turkey was found.
- Disboard, discord.me, top.gg server listings and discadia returned 403 or gave no usable TR LoL results, so smaller Flex-specific TR servers may be missing.
- Whether the "official" server is run by Riot or by a fan team was not verified. The claim comes from the server's own listing.

## 2. Other channels TR players use, and Riot's own features

### Takeaway
Outside Discord, I found no active Turkish LFG site besides LoLFinder. Riot's client has had no group-finder since Clubs were removed in 2020. The 2026 Riot–Discord integration only sends party invites to existing friends; it does not help find strangers. Its availability in Turkey is doubtful because of the ban.

### Cited Findings
- **LoLFinder.net** returned HTTP 403 to automated fetching on 9 Nov 2026, so its current activity could not be checked. The earlier figure of about 3–5 TR Flex posts per day comes from the project's previous research and was not re-verified. — [lolfinder.net](https://lolfinder.net)
- A Turkish-language search for Flex teammate-finding sites returned only Discord servers and generic English LFG guides. No Turkey-specific non-Discord site appeared. — [League of Legends TR (Discord)](https://discord.com/servers/league-of-legends-tr-896475140039278592); [ExitLag LFG guide](https://www.exitlag.com/blog/lfg-league-of-legends/)
- One English guide calls the official LoL Discord the single platform with the densest region- and role-specific LFG channels, and names RiftQ as a LoL-specific LFG platform that grew out of an EUW Discord community. — [ExitLag blog](https://www.exitlag.com/blog/lfg-league-of-legends/)
- The PreBot repo itself is indexed for the query "takım arkadaşı flex 5. kişi", which suggests few competing pages for this exact query. Its description: "League of Legends TR: Flex için eksik 5. kişiyi dakikalar içinde bul… PWA + Web Push ve Discord botu." — [GitHub: AlperCna/prebot](https://github.com/AlperCna/prebot)
- **Clubs** were removed from the League client and the League+ app with Patch 10.25 (8 Dec 2020), and nothing has replaced them. The replacement claim comes from a community wiki updated Aug 2026. — [Riot: Sunsetting Clubs](https://www.leagueoflegends.com/it-it/news/game-updates/sunsetting-clubs/); [Upcomer](https://upcomer.com/riot-plans-to-shut-down-clubs-in-the-lol-client-and-league-app); [Guild Order wiki](https://guildorder.com/games/league/wiki/no-clan-system-and-clash)
- **Riot–Discord integration:** announced April 2026 (Sheepesports dated it 14 Apr 2026). It links accounts, lets you invite Discord friends into a League party, and shows a "Join" link and party status in Discord. There is no in-client Discord chat. — [Sheepesports](https://www.sheepesports.com/articles/riot-games-announced-discord-integration-for-lol-and-valorant/en); [Riot support](https://support.riotgames.com/en-us/league-of-legends/gameplay/linking-your-riot-account-to-discord)
  - Discord's blog post (21 May 2026) says it works only "where Riot and Discord are both supported" and describes no matchmaking or LFG feature. It does not mention Turkey. — [Discord blog](https://discord.com/blog/making-it-easier-than-ever-to-connect-with-friends-in-league-val)
  - Conflict: a search summary attributed beta dates (Brazil 7 May; US/Canada 20 May; other regions "soon") to Discord's blog, but the fetched blog post gives no dates. Rft.gg reports an Americas beta. — [rft.gg](https://rft.gg/news/riot-games-launches-discord-integration-beta-in-the-americas); [players.com.ua](https://players.com.ua/en/news/riot-launches-beta-version-of-discord-integration/)
- **Team-wide voice chat** (not just premades) was scheduled from Patch 26.5 (4 Mar 2026). — [AlternativeTo](https://alternativeto.net/news/2026/2/league-of-legends-set-to-launch-long-awaited-global-team-voice-chat-feature); [U.GG](https://u.gg/lol/articles/league-of-legends-is-finally-adding-team-voice-chat-but-theres-a-catch)
- **"League Next":** a new client "fully integrated with the in-game experience" is planned after the 2026 season, reportedly in 2027. No LFG feature has been confirmed. — [GamesRadar](https://www.gamesradar.com/games/league-of-legends/league-of-legends-is-getting-a-new-client-entirely-new-visuals-and-a-bit-of-new-gameplay-in-a-massive-2027-update-reportedly-codenamed-league-next); [PC Gamer](https://www.pcgamer.com/games/moba/a-major-overhaul-of-league-of-legends-is-reportedly-coming-in-2027-once-were-done-it-should-be-the-best-time-ever-to-get-your-friends-into-league)
- A search snippet mentions a "Looking for friends" client feature (search by role, rank and availability) available only on Russian servers and possibly temporary, plus a partial Clash team-search tool. Both are undated, and the source article returned 403. — [Dot Esports (403)](https://dotesports.com/?p=441849); [Millenium](https://www.millenium.org/news/378959.html)
- **Descall:** a free Turkish Discord-like app with servers, roles, voice and video, screen share and game templates, available on Windows, iOS, Android and web. It is developed in Turkey by Demir Sarp Kurtlar. No user numbers are published, and the source is the vendor's own page. — [Descall](https://descall.com/tr/discord-acildi-mi)
- **Technopat Sosyal** (a large TR tech forum): a search for LoL Flex teammate threads returned only an unrelated general "Arkadaş arıyorum" thread. — [Technopat](https://www.technopat.net/sosyal/konu/arkadas-ariyorum.2780395/post-22562446)

### Inferences
- In the TR market, the realistic non-Discord alternatives are LoLFinder (a static board), in-game friend lists, and private WhatsApp groups among friends. Riot has no LFG feature on the horizon, so a Riot feature is unlikely to make PreBot obsolete in 2026–27. League Next (2027) is the one wildcard.
- The Riot–Discord integration makes "invite my Discord friend" easier, but only for friends you already have. It does not touch PreBot's actual job: finding a stranger to fill the 5th seat in minutes.
- If the Discord ban is lifted (the minister said "probably"), Discord-based LFG gets stronger, not weaker. That would favour PreBot having a Discord presence.

### Gaps
- No data was found on Reddit (e.g. a TR LoL subreddit), Facebook groups, or WhatsApp/Telegram LFG groups for TR LoL. These searches returned nothing usable. They may exist but are not indexed.
- No sites such as "lolduo" or "takım bul" focused on TR were found. Absence from search results is weak evidence that none exist.
- Whether the Riot–Discord integration is live for TR accounts is unknown.

## 3. Discord LFG bots for LoL: features, scale, TR use

### Takeaway
Several LoL LFG bots exist (FiveStack, Queue Up! Duo Finder, Miko by Duoo.gg, LFG Hub), but all show small or unlisted footprints, and none is TR-specific or shown to be used on TR servers. Rival bots are not the main threat. The main threats are the servers' own LFG channels and in-house "systems."

### Cited Findings
- **FiveStack:** organises a League 5-stack inside a server (`/5stack start`, Join button, role picked with emoji, availability windows, ping everyone when the stack is full). It allows **one group per server at a time** to avoid spam. Discord Bot List shows 0 upvotes in November, no server or user count, and the creator as "beeann#0". It has no language or region settings. — [Discord Bot List: FiveStack](https://discordbotlist.com/bots/fivestack); [top.gg (403; snippet)](https://top.gg/bot/1399290217659105331)
- **Queue Up! | Duo Finder:** matches players by rank, lane, region, playstyle and mic preference **across every server where the bot is installed**. No reviews on top.gg. Server count unavailable (403). — [top.gg (snippet)](https://top.gg/bot/1446819621133877288)
- **Miko by Duoo.gg:** a duo finder using an `/LFG` command, with rank verification that gives members a role matching their in-game rank. Free. — [top.gg search results (snippet only; this bot's top.gg page ID was not confirmed, possibly this link)](https://top.gg/bot/1163043953273217044)
- **LFG Hub:** a generic, multi-game LFG bot (any game, League can be preselected). It creates LFG posts and temporary voice channels, and its listing claims "50K+ members." — [top.gg search results (snippet only; this bot's top.gg page ID was not confirmed, possibly this link)](https://top.gg/api/bot/1096340606235717642)
- Other LoL bots on top.gg (Ekko Bot, Dorans-bot, LeagueHelper, Runemaster, Elder Dragon, League Bot) are stats or utility bots, not matchmakers. — [top.gg: League Bot](https://top.gg/fr/bot/471421776593682432); [top.gg: Elder Dragon](https://top.gg/de/bot/601861497580159006)
- Open-source in-house and team bots exist on GitHub, e.g. mrtolkien/inhouse_bot and ynsrkn/inhouse-bot. — [inhouse-bot](https://github.com/ynsrkn/inhouse-bot); [ecosyste.ms: inhouse_bot](https://awesome.ecosyste.ms/projects/github.com%2Fmrtolkien%2Finhouse_bot)

### Inferences
- FiveStack's "one group per server" limit and lack of no-show tracking mean it cannot serve a server with hundreds of LFG seekers. PreBot's model (many listings at once, first tap takes the seat, no-show record) differs in substance. Queue Up!'s cross-server pool is the closest conceptual rival, but it targets duo, not filling the 5th Flex seat.
- Rank-verified roles (Miko) and temporary voice channels (LFG Hub) are features that rival bots use to win over admins. They hint at what admins value (see section 4).

### Gaps
- Server counts for FiveStack, Queue Up! and Miko could not be retrieved, because top.gg blocks automated fetching. Someone should check them by hand at top.gg.
- No evidence was found either way on which bots the big TR servers run. That would need joining the servers and viewing their member lists.

## 4. Distribution: how a new bot gets into existing servers, and what admins want

### Takeaway
A bot grows only through admin approval, one server at a time. Discord caps unverified bots at 100 servers, and Message Content is a privileged intent that needs review at that scale. For a TR Flex niche, 100 servers is not a real constraint. The real bottleneck is getting the 3–5 big TR servers (official, LoL TR, Stratera, Naru) to accept it. Some of them already have their own systems, and the "official" one probably restricts third-party bots.

### Cited Findings
- Discord requires verification for bots in 100+ servers: "until your bot is verified, you will not be able to grow past 100 servers." Verification includes an opt-in process for privileged gateway intents. — [Discord blog: The future of bots](https://discord.com/blog/the-future-of-bots-on-discord)
- Apps in fewer than 100 servers can switch privileged intents (Presence, Server Members, Message Content) on in the Developer Portal. Apps in 100+ servers must apply, and the review asks why each intent is needed and how the data is stored. — [Discord docs: privileged intent review](https://docs.discord.com/developers/gateway/getting-started-with-privileged-intent-review.md)
- Third-party guides suggest applying around 75 servers, with reviews taking about 1–5 business days. These figures are unofficial. Without approval, a toggled intent is not honoured, and the bot gets gateway error 4014. — [Falix bot growth guide](https://client.falixnodes.net/guides/bot-recipes/bot-growth-checklist); [Stack Junkie](https://www.stack-junkie.com/blog/fix-openclaw-discord-gateway-4014)
- Listing channels: top.gg, Discord Bot List and Disboard (for servers) are the main directories. The TR LoL servers above are discoverable through Discord Discovery and Discord Bot List. — [Discord Bot List: lolturkiye](https://discordbotlist.com/servers/lolturkiye-1894); [Discord Discovery: LoL TR](https://discord.com/servers/league-of-legends-tr-896475140039278592)
- Admin-facing features rival bots advertise: rank verification and roles (Miko), temporary voice channels (LFG Hub), and anti-spam through one group per server (FiveStack). — [top.gg search results (snippets)](https://top.gg/bot/1446819621133877288); [Discord Bot List: FiveStack](https://discordbotlist.com/bots/fivestack)
- Streamer servers advertise teammate-finding as a core reason to join (Naru: "find teammates for LoL and Valorant"). — [Discord: Naru](https://discord.com/servers/naru-556923338568761354)

### Inferences
- PreBot should not depend on Message Content. Slash commands, buttons and modals work without privileged intents, so verification at 100 servers is just paperwork. For a TR-only niche, the bot may never reach 100 servers anyway.
- The realistic route is direct outreach to admins of 5–10 mid-sized TR servers (streamer servers such as Naru and Stratera, and LoL TR or the 2024 LoL Türkiye server) with a pitch that solves their problems. That means less spam in #lfg (one card per listing, auto-close when full), no-show and reliability data for moderation, optional rank roles, and Turkish-language UI. The large "official" server is the hardest to enter. It claims official status, so third-party bots are probably restricted, though this is unverified.
- Streamer partnerships give the best reach per contact. Stratera's 24.7k members took one person's decision. But streamer servers' online counts (349–766) are lower than the official server's.

### Gaps
- No primary statements from TR server admins on what they want from an LFG bot were found. Admin interviews or DMs would be needed.
- No data on how TR server admins vet bots (e.g. whether they require open source or Turkish UI) was found.

## 5. Testing the claim

> "LoLFinder is a static board. The real competitor is Turkish Discord LFG channels with hundreds online in the evening. PreBot's real advantages (push notifications, first-to-tap takes the seat, no-show tracking) only matter if it runs inside those communities, i.e., as a bot in a Discord server."

### Takeaway
The claim is mostly right about the competitor but wrong to say PreBot should be only a Discord bot. TR LoL LFG really does live on Discord, at a larger scale than "hundreds" (about 15k online on the main server). But the Discord ban blocks 97–100% of measurements on the big ISPs, including mobile. That makes a web PWA with web push the one channel that reaches a phone without a VPN. The evidence supports a hybrid: the Discord bot as the distribution and announcement layer inside existing servers, the PWA as the reliable notification and seat-claim layer. This is already PreBot's architecture.

### Cited Findings
- Scale of Discord LFG supply: League of Legends Türkiye has 138,448 members and 15,210 online, with an LFG category. LoL TR has 19,587 members and 2,827 online, with its own matching "systems." Stratera has 24,692 members and 713 online, and Naru 7,980 and 349. — [Discord: LoL Türkiye](https://discord.com/servers/league-of-legends-turkiye-678949756982132756); [Discord: LoL TR](https://discord.com/servers/league-of-legends-tr-896475140039278592); [DBL: Stratera](https://discordbotlist.com/servers/stratera); [Discord: Naru](https://discord.com/servers/naru-556923338568761354)
- Access friction: Turkcell (mobile) 100% blocked or anomalous, Superonline 99.1%, Türk Telekom 96.9%, TurkNet 38.4% (Sep–Oct 2026). — [Teknoroid / OONI](https://www.teknoroid.net/2026/10/turkiyede-discorda-erisim-saglayiciya-gore-degisiyor-ooni-verileri-ekim-2026.html)
- Ban possibly ending: the minister said "probably" in June 2026, with no date, and it was still blocked as of early Nov 2026. — [Sözcü](https://www.sozcu.com.tr/bakan-uraloglu-acikladi-discord-bir-sure-sonra-erisime-acilacak-p325599); [Descall](https://descall.com/tr/discord-acildi-mi)
- Riot provides no in-client LFG. Clubs were removed in 2020, and the 2026 Discord integration covers friend invites only. — [Riot: Sunsetting Clubs](https://www.leagueoflegends.com/it-it/news/game-updates/sunsetting-clubs/); [Discord blog](https://discord.com/blog/making-it-easier-than-ever-to-connect-with-friends-in-league-val)
- Existing Discord LFG bots are either one group per server (FiveStack) or duo-focused (Queue Up!, Miko). None found has first-tap seat claiming plus no-show tracking. — [DBL: FiveStack](https://discordbotlist.com/bots/fivestack); [top.gg: Queue Up!](https://top.gg/bot/1446819621133877288)

### Inferences
- **Right part 1:** LoLFinder is not the benchmark. Discord channels in TR servers, plus server-specific bots and systems, are where the volume is.
- **Right part 2:** a PWA alone has a cold-start problem. Without being placed where the 15k online users already are, it will look like another LoLFinder. Distribution through Discord servers is needed to build up enough listings and users.
- **Wrong part 1:** "hundreds online" understates the scale. The real figure is thousands online on one server. This makes the opportunity bigger, but also means the incumbent is a habit ("post in #lfg") that is free and instant.
- **Wrong part 2:** "only matter if it runs inside Discord" ignores the ban. Push notifications through Discord's own app are the weakest link on Turkish mobile networks. Web push to a PWA works on Turkcell, Türk Telekom and Superonline without a VPN. That is a specific TR advantage over pure Discord bots like FiveStack and Queue Up!, and only PreBot's web side has it.
- **Risk if the ban lifts:** Discord gets easier to reach, and the PWA's notification advantage shrinks. The Discord-bot side becomes more important, so investing in it now is a hedge.
- **Incumbent risk:** some big servers already run their own matching (LoL TR's "systems"). Pitch PreBot as an add-on that handles the hard "4 kişiyiz, 5. lazım" case with seat claiming and no-show records, not as a replacement for their general LFG channel.

### Gaps
- The core unknown is still demand density: how many "5. lazım" Flex posts appear per evening in TR Discord LFG channels, and how fast they fill today. Without this, it is impossible to say whether first tap takes the seat and no-show tracking fix a real pain or a minor one. It needs a manual 3–5 evening sample in 2–3 servers.
- Whether TR Discord users mostly use PC with VPN/DNS (where Discord notifications work) or phones on mobile data (where they often don't) is unmeasured. This decides how much the web-push advantage is worth.
