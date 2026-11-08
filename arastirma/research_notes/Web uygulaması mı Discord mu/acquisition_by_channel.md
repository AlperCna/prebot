# How LFG / Teammate-Finder Products Acquire and Retain Users by Channel (Web vs Mobile vs Discord Bot), and What It Means for a TR "Need a 5th Now" LoL Product (as of Nov 2026)

Labels used: **[DATA]** = measured or reported figure; **[ANECDOTE]** = single report, user claim or self-reported marketing; **[OPINION]** = commentary or analysis. Figures a company reports about itself are marked self-reported.

Context the coordinator gave me, not re-checked here: TR players post "1 kişi lazım" in Discord servers. The official LoL Türkiye Discord has about 138K members. Web LFG boards (LoLFinder, RiftQ, TeamTavern, Teamplay.gg) often have stale posts. TEAMS.gg shut down after 6 years. The100.io shows about 218 weekly active players.

---

## Q0 (deciding factor, checked first): Is Discord blocked in Turkey as of Nov 2026?

### Takeaway
Yes. Discord is still blocked in Turkey as of 8 Nov 2026. A court order from October 2024 is still in force on the major home ISPs. In June 2026 the transport minister said it would "probably" reopen, but gave no date, and no decision lifting the block has been published. Anyone in TR who reaches Discord today goes through a VPN, DNS or anti-DPI workaround. This is the most important fact for choosing a channel.

### Cited Findings
- [DATA] An Ankara criminal judgeship of peace ordered the block on 8–9 Oct 2024, at the Ankara Chief Public Prosecutor's request, citing suspected child sexual abuse and obscenity offences. Sources disagree by a day on the date. — [vpnAlert censorship monitor](https://vpnalert.com/tools/censorship-monitor/turkey/discord/); [Medyascope, 9 Oct 2024](https://medyascope.tv/2024/10/09/discorda-erisim-engeli-getirildi/); [BirGün](https://www.birgun.net/haber/discord-a-erisim-engeli-getirildi-565828)
- [DATA] Latest test on 8 Nov 2026: discord.com fails to open on Türk Telekom and Turkcell Superonline home connections. The site recorded the block in every daily test over about 39 days. A separate OONI measurement on Superonline on 7 Oct 2026 also found it blocked. — [vpnAlert](https://vpnalert.com/tools/censorship-monitor/turkey/discord/)
- [DATA] Officials said Discord had refused to share user data, such as IP addresses, with security units. — [vpnAlert](https://vpnalert.com/tools/censorship-monitor/turkey/discord/); [CNN Türk](https://www.cnnturk.com/teknoloji/discord-yasaklandi-mi-erisim-engeli-geldi-mi-discord-nedir-2162987)
- [DATA/statement] On 5–6 June 2026, Transport Minister Abdulkadir Uraloğlu said Discord had largely met Turkey's content-removal requirements and would "probably" be reopened after some time. He gave no date. — [Sözcü](https://www.sozcu.com.tr/bakan-uraloglu-acikladi-discord-bir-sure-sonra-erisime-acilacak-p325599); [ShiftDelete](https://shiftdelete.net/bakan-acikladi-discord-icin-erisim-engeli-kalkiyor); [GZT](https://www.gzt.com/gundem/robloxun-ardindan-gozler-discorda-cevrildi-discord-erisim-engeli-kalkti-mi-bakanliktan-yesil-isik-muhtemelen-acacagiz-4233744); Technopat, 6 Jun 2026 (https://www.technopat.net/2026/06/06/discord-turkiye-yeniden-aciliyor/), which returned 403 so I could not read it.
- [DATA] No decision lifting the block was found by Oct–Nov 2026. — [vpnAlert](https://vpnalert.com/tools/censorship-monitor/turkey/discord/); [Descall explainer, updated Oct 2026](https://descall.com/tr/discord-acildi-mi); [myvpn.ai, "Still Blocked (Aug 2026)"](https://myvpn.ai/discord-banned-turkey/). The last two have a commercial interest.
- [DATA, from a search snippet only] TGRT Haber reportedly said in Oct 2024 that Discord was removed from the Turkish App Store and Google Play after the ruling. I did not open the article. — [TGRT Haber VPN tag page](https://www.tgrthaber.com/vpn?p=2)
- [DATA] VPN providers' own websites, such as nordvpn.com and surfshark.com, do not open on some Turkish networks. New users therefore have to install a VPN in advance, which adds a lot of friction for anyone new to Discord. — [vpnAlert](https://vpnalert.com/tools/censorship-monitor/turkey/discord/)
- [OPINION] Using a VPN is not a crime in itself for ordinary users in Turkey, but VPN services are tightly policed and many are blocked. — [netzwelt.de](https://www.netzwelt.de/news/235016-tuerkei-blockiert-discord-grund-so-umgeht-sperre.html); [2-spyware](https://www.2-spyware.com/discord-blocked-in-turkey-and-russia-over-legal-compliance-issues)
- [ANECDOTE] Users said "they just lost their entire community" when the bans took effect. — [TechRadar](https://www.techradar.com/pro/vpn/discord-is-banned-in-turkey-and-russia-with-users-claiming-they-just-lost-their-entire-community)
- [OPINION/self-interested] A Turkish Discord-style app, Descall, says VPN or DNS workarounds hurt voice quality and reliability, and that it is hard to get every member of a group to set one up, so many groups move to a directly accessible alternative. It mentions WhatsApp and Telegram for simple messaging. Descall does not publish user numbers. — [Descall](https://descall.com/tr/discord-acildi-mi)
- [ANECDOTE, source not identified] Turkish alternatives roundups say TeamSpeak was widely used in TR before Discord, still has many TR users, and is preferred by FPS players. The snippet did not say which roundup this came from. — [Webtekno roundup](https://www.webtekno.com/discord-alternatifi-uygulamalar-h150237.html); [TGRT alternatives article](https://www.tgrthaber.com/aktuel/erisim-engeli-getirilen-discord-yerine-kullanilabilecek-benzer-uygulamalar-2976561)
- [DATA] Riot's account linking to Discord works only through the Discord desktop app on PC or Mac. — [Riot Support TR](https://support.riotgames.com/tr-tr/league-of-legends/gameplay/linking-your-riot-account-to-discord)

### Inferences
- The idea that LoL players sit at their PC with Discord open is weaker in TR than in any EU or NA market. The people still on Discord are a self-selected group who already run a VPN, DNS or anti-DPI tool such as GoodbyeDPI. That group is probably still large among core PC gamers, given the coordinator's "1 kişi lazım" observation. But it is hard to grow: a newcomer cannot simply install Discord and click "Add bot".
- Member counts of TR Discord servers, such as the 138K for the official server, are cumulative and include pre-ban members. They overstate how many people are actively reachable today.
- Discord's legal status is a binary platform risk in both directions. If it reopens, which the June 2026 signal makes plausible but which has no date, a bot becomes much more viable overnight. If it stays blocked, a Discord-only product stays capped. Whatever is chosen has to survive both outcomes.
- From what I can see, Riot Türkiye has not moved its official community to another platform. I found no Riot TR statement on the ban.

### Gaps
- No data on what share of TR LoL players still use Discord through workarounds. I found no BTK, Similarweb, Sensor Tower or survey figures, and no traffic figures for discord.com in TR after the ban.
- I could not confirm the app-store removal beyond a search snippet.
- No Riot TR statement on Discord or community channels found.

---

## Q1: Which acquisition channels worked for web LFG sites, bots and apps?

### Takeaway
The best-documented growth in this space came from creators and communities, not from SEO. Mobalytics used streamers and pro players plus a waitlist. LF.Group grew through partnerships with WoW guilds and streamers, plus a Discord bot that launched before its website. GameTree grew mostly organically with little paid marketing. Bot-first tools like Raid-Helper reached six-figure server counts through server-to-server word of mouth. I found no data on TR search volume for LFG keywords.

### Cited Findings
- [DATA] Mobalytics raised a $2.6M seed round in 2016. Its private beta admitted the first 10,000 people on a waitlist. — [TechCrunch 2016](https://techcrunch.com/2016/11/16/gaming-analytics-startup-mobalytics-grabs-2-6-million-in-seed-funding)
- [DATA, self-reported] Mobalytics passed 500,000 users in the first three months of open beta, and had 7M monthly active users by May 2020. — [AIN.ua 2020](https://en.ain.ua/2020/07/22/mobalytics-raises-11-million); [Kyiv Post](https://archive.kyivpost.com/technology/ukrainian-lebanese-e-sports-startup-attracts-2-6-million.html)
- [DATA] Mobalytics shipped a Twitch extension for LoL streamers in January 2018. It answers routine viewer questions, which puts the product in front of stream audiences. — [Twitch blog](https://blog.twitch.com/en/2018/01/31/behind-the-extension-mobalytics-a-new-extension-for-league-streamers-3c7b76d3e79c/)
- [DATA] Mobalytics brought in LS and Nemesis to "reach more players and more different populations of players". — [GamesBeat](https://gamesbeat.com/mobalytics-enlists-esports-stars-in-quest-to-be-your-league-of-legends-companion/)
- [DATA] LF.Group raised $1M pre-seed (lead: Grishin Robotics). It reports more than 50,000 gamer profiles and 200 teams on a waitlist. Its first product was an LFG Discord bot, which included a Lost Ark merchant tracker; the website for recruiters came later. Acquisition went through partnerships with WoW guilds and streamers, including Echo and Method. It covers LoL, Wild Rift, WoW and Lost Ark. — [Esports News UK](https://esports-news.co.uk/?p=49445)
- [DATA] GameTree had more than 600K users with "minimal paid marketing", and raised $1.7M, including an Overwolf grant (2023). — [AIN.ua 2023](https://en.ain.ua/2023/12/05/ukrainian-founded-gametree-raises-1-7m); [Startup Rise](https://startuprise.co.uk/gametree-raises-1-7-million-in-seed-funding/)
- [DATA, third-party estimate] GameTree's Android app: about 1.5M installs, about 57.6K MAU, about 3.1K installs per week (2026 snapshot). — [AppGoblin](https://appgoblin.info/apps/com.gametreeapp)
- [ANECDOTE, self-reported] Teamplay.gg says it has "23,000+ players". Its Discord server listings show 4,750 and 6,812 members, which may be snapshots from different dates. It offers a Discord bot for creating, browsing and joining LFG posts inside Discord, plus an Android app. — [Teamplay.gg Discord listing](https://discord.com/servers/teamplay-gg-1042150011422658750); [second listing](https://discord.com/servers/teamplaygg-1042150011422658750)
- [DATA, conflicting] Raid-Helper, a bot-first tool with a web dashboard, is reported in about 150K, about 175K and about 271K servers. The 271K figure was checked in July 2026; the other two are undated. — [alternative.me](https://alternative.me/discord/bots/raid-helper); [TechCult](https://techcult.com/how-to-use-raid-helper-bot-in-discord); [top.gg advertise page](https://top.gg/advertise)
- [DATA] top.gg sells ads to grow bots and servers, a paid channel available to bots. — [top.gg advertise](https://top.gg/advertise)

### Inferences
- In every documented case, distribution came from places players already gather: streams, guild and community servers, and an existing tool. SEO-led web LFG growth is not documented in what I found.
- The products that scaled (Mobalytics, Porofessor-style tools, GameTree) offered value on their own, such as stats, a companion or a social profile. Pure "find a 5th" boards depend on liquidity and go stale, which fits the coordinator's prior findings on stale boards and TEAMS.gg shutting down.
- For TR, the closest equivalent to "streamer shoutouts plus guild partnerships" is Turkish LoL streamers on Twitch and Kick, YouTubers, and community Discord or WhatsApp groups. Under the ban, the Discord-server partnership channel reaches only people who use workarounds.

### Gaps
- Google Trends TR and keyword volume for "lol duo bul", "esnek pre bul" and "lol takım arkadaşı bul": not retrieved, because Google Trends is JS-rendered and I found no third-party keyword data.
- Not researched within budget: Duoo.gg, the op.gg duo board, Porofessor's early growth, TikTok or Reddit-driven LFG growth, and Similarweb traffic for LoLFinder, RiftQ and TeamTavern.

---

## Q2: The "where is the player right now" problem

### Takeaway
Hybrid products deal with this by sending notifications to wherever the user already is (Discord DM or the browser) and by making the website the place to manage things, not a place you have to check. In TR, the browser is the one surface every PC LoL player has open without a workaround.

### Cited Findings
- [DATA] LF.Group sends new-candidate notifications by Discord or in the browser, and uses its website for managing applicants. — [Esports News UK](https://esports-news.co.uk/?p=49445)
- [DATA] Teamplay.gg's bot lets users create, browse and join LFG sessions without leaving Discord, while the web and app hold profiles. — [Teamplay.gg Discord listing](https://discord.com/servers/teamplay-gg-1042150011422658750)
- [DATA] Discord itself is unreachable in TR without a workaround (see Q0). — [vpnAlert](https://vpnalert.com/tools/censorship-monitor/turkey/discord/)

### Inferences
- A separate website does add a step compared with an in-Discord bot, for users who already have Discord open. For a "need a 5th now" moment, that step can be shortened with:
  - a PWA pinned as a browser tab or installed to the desktop;
  - web push notifications ("a team is looking for a support, Gold, join within 2 minutes");
  - one-click share links that people paste into whatever chat they use: Discord, WhatsApp groups, Telegram, TeamSpeak, Descall.
- Discord OAuth login is risky as the only sign-in in TR, because it depends on reaching discord.com. Offer Riot RSO, Google or email login, with Discord as optional.

### Gaps
- No measured data comparing conversion between in-Discord and click-through-to-web for LFG.

---

## Q3: Hybrid patterns (web as source of truth plus bot or webhooks)

### Takeaway
The successful tools that scaled are hybrids. Raid-Helper is a bot with a web dashboard. LF.Group started with a bot and added a web dashboard with Discord and browser notifications. Teamplay.gg runs web, a bot and an Android app. Bot-first worked when Discord was the native gathering place. In TR that assumption is broken, so the TR version of the pattern is web first with optional bot or webhook adapters.

### Cited Findings
- [DATA] Raid-Helper is in about 150K–271K servers (sources conflict). — [alternative.me](https://alternative.me/discord/bots/raid-helper); [TechCult](https://techcult.com/how-to-use-raid-helper-bot-in-discord)
- [DATA] LF.Group went from a Discord bot to a website, with notifications through both. — [Esports News UK](https://esports-news.co.uk/?p=49445)
- [ANECDOTE] Teamplay.gg runs web, a Discord bot and Android, but has a modest community (about 5–7K Discord members). — [Teamplay.gg listing](https://discord.com/servers/teamplay-gg-1042150011422658750)

### Inferences
- I found no controlled evidence that hybrid beats pure web or pure bot. The pattern among survivors suggests it does, but survivorship bias applies.
- In TR, a hybrid hedges against the Discord block. The web PWA works for everyone today. A bot or webhook that posts "5. kişi aranıyor, Plat, ADC → [Katıl]" into partner servers reaches the VPN-using core now and scales immediately if Discord reopens.

### Gaps
- Not researched within budget: Sesh, Apollo, LFG Hub, and the Challengermode or start.gg Discord integrations.

---

## Q4: Retention by channel

### Takeaway
Evidence is thin. Third-party app analytics suggest standalone LFG apps struggle with retention. LF.Group's design shows the common answer: push notifications for new matches, either in Discord or in the browser.

### Cited Findings
- [DATA, third-party estimate] GameTree: about 1.5M Android installs against about 57.6K MAU, which is a low active-to-install ratio. — [AppGoblin](https://appgoblin.info/apps/com.gametreeapp)
- [OPINION, AI-generated-style analysis] GameTree "struggles with user retention", with app instability as one factor, and loses casual users to swipe-style social apps. — [marlvel.ai](https://marlvel.ai/intel-report/social-networking/gametree-find-gaming-friends)
- [DATA, coordinator context] The100.io shows about 218 weekly active players, and TEAMS.gg closed after 6 years. These are signs that standalone LFG products decay. — not re-verified.

### Inferences
- For a "need a 5th now" product, retention comes from (a) the speed of a successful fill, (b) saved teammates or friends and a quick "invite last group again", and (c) notifications. A standalone site without notifications will fail to bring people back.
- Which channel delivers the notification matters less than whether one exists. In TR, web push and email are the channels that work regardless of the Discord ban.

### Gaps
- No channel-specific retention data (D1, D7, D30) for any LFG product.

---

## Q5: Turkish market specifics

### Takeaway
Turkey is a mobile-heavy gaming market, but LoL is played on PC, and the Discord block has pushed parts of the community to VPN-based Discord, WhatsApp or Telegram, TeamSpeak, and Turkish alternatives such as Descall. No data shows how the TR LoL community is split across these.

### Cited Findings
- [DATA] The Turkish gaming market reached about $1.01B in 2025 (+25% in USD). — [Forbes TR](https://www.forbes.com.tr/ekonomi/turkiye-oyun-pazari-1-milyar-dolar-sinirini-asti); [Bloomberg HT](https://www.bloomberght.com/turkiye-de-oyun-sektoru-1-milyar-dolari-asti-3769463). Another source reports about $700M+ for the sector: [Para Dergi](https://www.paradergi.com.tr/girisimcilik/2026/01/27/turkiye-oyun-sektoru-700-milyon-dolari-asti). These two likely measure different scopes.
- [DATA] Mobile is the dominant platform: about $649M revenue and about 47M players, per the Gaming in Türkiye 2025 report as covered in the press. — [Mobidictum](https://mobidictum.com/tr/turkiye-oyun-sektoru-raporu-2025/)
- [DATA] Alternatives in use after the ban include Descall (Turkish-made, works on web, Windows, iOS and Android), WhatsApp and Telegram. — [Descall](https://descall.com/tr/discord-acildi-mi)

### Inferences
- A browser-based PWA fits TR well: no VPN needed, works on the PC where LoL runs, and share links travel through WhatsApp, which is the default TR messaging app.

### Gaps
- Not researched within budget: oyna.co, Mate4Game, and itemsatis community features.
- No data on LoL TR player counts or on Discord or VPN usage among TR gamers.

---

## Bottom-line implications: web vs Discord vs hybrid for TR "need a 5th now"

These are my inferences from the findings above.

1. **Discord-only is structurally capped in TR while the block lasts.** The friends' instinct that people won't use a bot is probably right for a different reason than they think. The bot format itself isn't the problem; the problem is that Discord is not legally reachable in TR without a workaround. Getting a bot added to servers and finding new users is hard because new users cannot easily join Discord.
2. **A web PWA should be the source of truth.** It needs no VPN, it runs on the PC beside the LoL client, it can send web push notifications, and its share links work in WhatsApp, Telegram, Discord, TeamSpeak and Descall.
3. **Keep the existing Discord bot as an adapter, not the core.** It should post listings with "Katıl" links into partner servers. That reaches the core of players who already use workarounds today and gives instant upside if Discord reopens (the June 2026 minister signal).
4. **Don't make Discord OAuth the only login.** Use Riot RSO, Google or email.
5. **Acquire users through creators and communities, not SEO.** Precedents: Mobalytics (streamers and pros, waitlist), LF.Group (guild and streamer partnerships, bot first), GameTree (organic). In TR that means LoL streamers, YouTubers and TikTok creators, plus community admins on Discord, WhatsApp and Telegram.
6. **Liquidity is the real risk, whatever the channel.** Stale boards and LFG shutdowns (TEAMS.gg) show it. Concentrate on peak hours, fill fast, and use notifications so a team that is waiting actually gets filled.
