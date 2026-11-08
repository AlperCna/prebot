# Options comparison: getting a 5th player on demand for Ranked Flex on TR1 (as of Nov 2026)

Notes legend: **[V]** = verified on the primary/official page fetched in this session. **[M]** = marketing or self-description (product site or bot listing) that was not independently tested. **[3P]** = third-party article or listing. **[I]** = my inference. Research was done on 2026-11-08 with about 18 tool calls. Several sites returned HTTP 403 to automated fetching (lolfinder.net, Discord support articles, top.gg bot pages), so those items are marked as gaps rather than guessed.

## Q0. Problem constraints (context for every option)

### Takeaway
A 4-player party cannot queue Ranked Flex. Allowed sizes are 1, 2, 3 or 5, so the group must either add a 5th player or split. That makes "find one more player fast" a hard requirement, not a nice-to-have.

### Cited Findings
- Ranked Flex allows parties of 1, 2, 3 or 5 players. Four-player parties are not allowed. [3P] — [dodge.gg Ranked Flex Guide 2026](https://www.dodge.gg/en-US/lol/news/ranked-flex-guide-2026); also stated by [theglobalgaming.com](https://theglobalgaming.com/lol/ranked-flex)
- Reason given for blocking quads: to protect the solo player who would be forced to fill the last spot. [3P] — [League wiki: Ranked game](https://wiki.leagueoflegends.com/en-us/Ranked_game)
- 2026 rank rules for Flex parties: no restriction when everyone is Diamond or below. If any member is Master+, everyone else must be at least Emerald. [3P] — [dodge.gg](https://www.dodge.gg/en-US/lol/news/ranked-flex-guide-2026). Sources disagree on how rank restrictions changed after patch 10.15. — [win.gg](https://win.gg/news/5022/lol-patch-10-dot-15-removes-rank-restriction-from-flex-queue)
- Riot's old support article URL (4406004131475) now redirects to the generic support hub, so the official party-size text could not be quoted. — [support.riotgames.com/league-of-legends](https://support.riotgames.com/league-of-legends)

### Inferences
- [I] A 5th player only has to be Diamond or below and on the TR server (or, at Master+, Emerald+). So "rank match" mostly matters for fair games and LP, not for eligibility. Matching on rank and role is a quality filter, not a hard gate.

### Gaps
- The brief says "Ranked 5s on TR runs Fri–Sun 16:00–04:00 and needs exactly 5". I found no source for a separate TR "Ranked 5s" queue or that schedule. The dodge.gg 2026 guide mentions no weekend-only 5-player mode. Treat this as user-supplied and unverified; it may be a new 2026 mode or a Clash-like event.

---

## Q1. Option A: existing tools today, no building

### Takeaway
The only verified high-traffic TR channel is the official "League of Legends Türkiye" Discord (138K members, about 21K online, LFG tag). Smaller Turkish servers exist. Of the web and bot LFG products, none was verified to support TR, and several clearly do not (premate.gg is Valorant-only, Queue Up! lists EUW/NA/EUNE). Option A works **today (0 hours)** but only as public, untargeted posting with unknown response times.

### Cited Findings
- **League of Legends Türkiye (official TR Discord):** 138,447 members and 20,958 online at fetch time. Tags: Esports, LFG, Entertainment, Gaming, General Chatting. Turkish language. Established 2020-02-17. Describes itself as the "resmi" (official) server. [V for the listing; channel structure not visible] — [discord.com/servers/league-of-legends-turkiye-678949756982132756](https://discord.com/servers/league-of-legends-turkiye-678949756982132756)
- **League of Legends TR:** about 17.6K members (the brief said ~19.5K). Pitched as a place to "search for a team or player" for LoL, TFT and Valorant. [3P/M] — [discord.com/servers/league-of-legends-tr-896475140039278592](https://discord.com/servers/league-of-legends-tr-896475140039278592)
- **A second "League of Legends Türkiye" (created Jan 2024):** about 7.2K members, pitched for finding in-game teammates. [M] — [discord.com/servers/league-of-legends-turkiye-1195405905576071168](https://discord.com/servers/league-of-legends-turkiye-1195405905576071168)
- **Stratera:** listed as one of the large Turkish LoL communities. Member count (~25K) not confirmed in this session. [M] — [discordbotlist.com/servers/stratera/join](https://discordbotlist.com/servers/stratera/join)
- **RANDOM LOL PLAYER:** a Turkish server advertising duo and team finding through voice and text channels. [M] — [discord.com/servers/random-lol-player-1205191369799245915](https://discord.com/servers/random-lol-player-1205191369799245915)
- **premate.gg:** "The Best Valorant LFG Platform". Other games are "coming soon", no LoL support, no TR mention. [V] — [premate.gg](https://premate.gg)
- **RiftQ:** began on a large EUW Discord. Its Discord communities are EUW and EUNE. No evidence of TR support found. [3P] — [discordbotlist.com RiftQ EUW](https://discordbotlist.com/servers/loleuw/upvote); [RiftQ EUNE Discord](https://discord.com/servers/riftq-league-of-legends-eune-488037455727951883)
- **Queue Up! (Duo Finder bot):** its listing says matching covers EUW, NA and EUNE, with region options such as EUW, EUNE, NA, KR. TR is not named. It is aimed at duo, not filling a 5th. [M] — [top.gg/bot/1446819621133877288](https://top.gg/bot/1446819621133877288)
- **Miko by Duoo.gg:** exists as a Discord bot listing. Region support not verified. [M] — [top.gg/bot/1096340606235717642](https://top.gg/bot/1096340606235717642)
- **LoLFinder.net:** returned HTTP 403, and search returned nothing about its Flex board or TR region. **Unverified.** — [lolfinder.net](https://lolfinder.net)
- A generic LFG blog advises including rank, role, champion pool and mic preference in posts. [3P, low authority] — [exitlag.com LFG guide](https://www.exitlag.com/blog/lfg-league-of-legends/)

### Inferences
- [I] The official TR Discord is the best Option A channel. With about 21K concurrent users it is plausible that a well-formed "+1 Flex, need ADC, Gold–Plat, mic" post gets answered within 15 minutes on weekend evenings. That is plausible, not measured.
- [I] Risks: you cannot target people, so post quality and toxicity are unknown. Posts get buried in a busy channel. Strangers may leave mid-session. There is no way to "ping available players".
- [I] Do this tonight in parallel with any build. It costs nothing.

### Gaps
- Could not see the channel list or post-reply latency on the official TR Discord (needs a logged-in view). The "<15 min" success rate is not measured anywhere I found.
- Not checked for lack of budget: GameVerse, OP.GG duo board (TR?), Mate4Game, EZLFP. LoLFinder TR support is still unknown.

---

## Q2. Option B: configure an existing Discord bot (no code)

### Takeaway
**LFG Hub** is the closest existing match. It is free, has a Turkish UI, Smart Ping (pings only members who recently played the game), Custom Ping (a role), role sign-up with a waitlist, and self-deleting temp voice. It covers roughly 60–80% of the target flow without code [I]. Gaps: posts stay open 14 days (no short auto-expiry found), and Smart Ping uses "recently played" rather than an explicit opt-in "available now" flag. Puck is a simpler alternative (thread-based /lfg with configured role pings). Raid-Helper, Apollo and Sesh are schedulers for planned events and fit poorly for "right now".

### Cited Findings
- **LFG Hub (lfghub.gg; page served in Turkish):**
  - **Smart Ping:** "Smart Ping sadece o oyunu yakın zamanda oynayanlara ulaşır" (reaches only those who recently played that game). [M] — [lfghub.gg](https://lfghub.gg)
  - Third-party listing details on Smart Ping: "never twice in 8 hours". A Spanish listing says it pings online players who played the game in the last 5 days, with an opt-out. [M via listing] — [top.gg/bot/1163043953273217044](https://top.gg/bot/1163043953273217044); [top.gg/es/bot/1163043953273217044](https://top.gg/es/bot/1163043953273217044)
  - **Custom Ping:** tags a role you set up. [M] — [lfghub.gg](https://lfghub.gg)
  - **Role sign-up:** set how many slots each role has, and players sign up for a role. **Waitlist** ("Bekleme listesi"): players move in when a slot opens. [M] — [lfghub.gg](https://lfghub.gg)
  - **Temp voice per group:** the owner can lock it, set a limit, kick or ban, pause the search, and transfer ownership. "Son oyuncu ayrılınca kanal kendini siler" (the channel deletes itself when the last player leaves). [M] — [lfghub.gg](https://lfghub.gg)
  - **Expiry:** player and team posts stay open 14 days ("14 gün açık kalır"). Invite links expire after 3 hours. Game events are deleted after 30 days. No short "need 1 for 30 min" expiry found. [M] — [lfghub.gg](https://lfghub.gg)
  - Filters per game (rank, mode, region, platform). Commands `/grup-oluştur` and `/grup-bul`. League of Legends has its own LFG page. [M] — [lfghub.gg](https://lfghub.gg)
  - **Price:** "Sonsuza dek ücretsiz" (free forever). The FAQ says it is free regardless of server size. No premium tier found. [M] — [lfghub.gg](https://lfghub.gg)
  - Caution: a different top.gg entry also uses the name "LFG Hub". Check the app ID (1163043953273217044) before inviting. [3P] — [top.gg/api/bot/1485552921532694578](https://top.gg/api/bot/1485552921532694578)
- **Puck:** lists, pings for and delists LFG posts. `/lfg` creates the post in a new thread. It "will not ping any roles out-of-the-box" until set with `/config mention set` (role plus default group type). Role buttons pre-fill slots. Needs the Mention Everyone permission to ping roles. [M] — [top.gg/bot/703068724818608138](https://top.gg/es/bot/703068724818608138); [discordbotlist.com/bots/puck](https://discordbotlist.com/bots/puck)
- **Raid-Helper:** a sign-up calendar bot built around role and class selection. Free, with premium for custom templates, recurring events and automatic archiving. Listed on about 271K servers. [3P] — [discordbotlist.com/bots/raid-helper](https://discordbotlist.com/bots/raid-helper)
- **Apollo:** free tier includes events, capacity limits, waitlists and sign-up presets (one comparison says up to 5 recurring events and 1 reminder). Premium is $7.99/month or $69.99/year. [3P; may be outdated] — [peakbot.pro Apollo vs Sesh](https://peakbot.pro/blog/apollo-vs-sesh-discord-event-bot-2026); [apollo.fyi/about](https://apollo.fyi/about)
- **Sesh:** natural-language times and RSVP polls. Repeating events are premium. [3P] — [peakbot.pro Sesh alternatives](https://peakbot.pro/blog/sesh-alternatives)
- **No "instant/now" LFG mode was found for Raid-Helper, Apollo or Sesh.** [gap noted in search] — [peakbot.pro best event bots 2026](https://peakbot.pro/blog/best-discord-event-bots-2026)
- **Discord native forum channels:** tags can be added to posts and posts filtered by tag. Pinned posts are not on a timer. [V via snippet of official article] — [support.discord.com/hc/en-us/articles/6208479917079](https://support.discord.com/hc/en-us/articles/6208479917079). A third party says the hide-after-inactivity choices are 1h, 24h, 3 days (default) and 1 week [3P, unconfirmed] — [lilachbullock.com](https://www.lilachbullock.com/discord-forum-channels/). Users report the timer cannot be disabled — [Discord feedback post](https://support.discord.com/hc/sv/community/posts/9386303504023-Forum-Posts-disable-automatic-hide-after-inactivity)

### Inferences
- [I] **Zero-code recipe (about 30–60 minutes of setup):** create self-assign roles such as `@Flex-Available`, lane roles (`@Top` … `@Sup`) and rank-band roles, using Discord Onboarding or Carl-bot reaction roles. Add a forum channel `#flex-need-1` with tags for lane and rank and a 1h hide-after-inactivity setting. Members toggle `@Flex-Available` when free. The caller posts "need 1 ADC Gold+" and mentions `@Flex-Available`. Add LFG Hub (Custom Ping set to `@Flex-Available`, temp voice) or Puck (`/config mention set @Flex-Available`).
- [I] What this recipe cannot do without code: auto-remove `@Flex-Available` after N hours, so the role goes stale and people get pinged when away. It also cannot do an intersection ping ("only available ADCs Gold+"), because Discord mentions a single role, not role A AND role B. Auto-expiry is limited to forum hide timers or LFG Hub's 14-day lifetime.
- [I] The pool is the hard part, not the tool. A 4-person group's own server has only its friends. For a stranger fill within 15 minutes they need a pool of about 20–50 active TR players [I, not measured]. Either recruit one (for example by cross-posting from the official TR Discord) or post where people already are (Option A).

### Gaps
- Not fetched: official docs for Carl-bot reaction roles (carl.gg), Discord Onboarding, Sesh and Raid-Helper pricing pages, LFG Hub's pricing page. Exact free limits for these are unverified.
- Whether LFG Hub Smart Ping honours the TR region or rank fields (versus game only) was not documented.

---

## Q3. Option C: minimal custom .NET bot (available pool + targeted pings + auto-expiry, no Riot API)

### Takeaway
This closes exactly the gaps Option B leaves: an opt-in "available for X minutes" status that expires on its own, ANDed filters (lane, rank band), DM or targeted pings, auto-expiring "need 1" posts, and first-come accept buttons. A .NET developer can reach a minimum viable version in about **1–3 evenings** [I]. Cost is near zero (a free or small VPS). Mature C# libraries exist.

### Cited Findings
- **NetCord:** MIT-licensed modern C# Discord library. The 1.0.0 beta line reached beta.11 in July 2026. It is Native AOT-ready, has hosting and services packages (NetCord.Hosting, NetCord.Services) for module-style slash commands, and documents slash-command examples. [V on NuGet/GitHub] — [nuget.org/packages/netcord.hosting](https://www.nuget.org/packages/netcord.hosting/); [github.com/NetCordDev/NetCord](https://www.github.com/NetCordDev/NetCord); [netcord.dev](https://netcord.dev/)
- **Discord.Net:** the long-standing C# wrapper. Its current release status was not confirmed in this session. Search only surfaced old mirrored docs. — [mirror of Discord.Net docs](https://code-kyxt.pcl.ac.cn/youys/Discord.Net/src/commit/d057c1b564cce32c056dfa05936b01a6a8397039/docs/index.md)
- An existing C# LoL bot on Discord.NET is a stats tool, not LFG, but it shows the stack works. [3P] — [awesome.ecosyste.ms sato-isolated/league-discord-bot](https://awesome.ecosyste.ms/projects/github.com%2Fsato-isolated%2Fleague-discord-bot)

### Inferences
- [I] Minimum viable scope:
  - `/musait sure:60 roller:ADC,SUP rank:Gold` stores the user in SQLite or in memory with an expiry.
  - `/lazim1 rol:ADC rank:Gold-Plat` posts an embed with a "Katıl" button and DMs or mentions only matching, available users.
  - The first click locks the slot. The post auto-deletes or edits after 15–20 minutes.
  - A hosted `BackgroundService` sweeps expired entries.
  - It only needs slash commands, buttons and DMs, so the GuildPresences privileged intent is not required.
- [I] Time to first value is about 1–3 evenings. Hardening (persistence, rate-limit handling, anti-spam cooldowns) takes another 1–2 weeks of spare time.
- [I] Risks: hosting uptime; DM spam leading users to block the bot; the pool problem (same as B). Discord limits bulk DMs, so prefer one mention message that lists matched users.

### Gaps
- Could not verify current Discord.Net release cadence.

---

## Q4. Option D: custom bot with Riot API verification

### Takeaway
Riot's **Personal key** tier is explicitly meant for "just the developer or a small private community". It includes "bots for … voice com servers". It needs product registration but no verification, at 20 requests per second and 100 per 2 minutes per region. That is enough to verify a Riot ID's TR1 rank on `/kayit`. A **Development key** expires every 24 hours and cannot back a live product. Adding this to Option C costs about **+1–3 days** of work plus the registration wait (no stated time) [I].

### Cited Findings
- Development key: "deactivate every 24 hours". "You may not maintain a public product with a development API key." [V] — [developer.riotgames.com/docs/portal](https://developer.riotgames.com/docs/portal)
- Personal key: 20 requests per 1 second and 100 per 2 minutes, per region. "can be registered without the verification process, but won't be approved for rate limit increases". Standard APIs only, no Tournaments API. Intended for "just the developer or a small private community". "You may not run your application for public consumption using a personal key", including open alpha or beta tests. Allowed examples include "bots for streaming sites, boards, voice com servers, etc." [V] — [developer.riotgames.com/docs/portal](https://developer.riotgames.com/docs/portal)
- Production key: starts at 500 requests per 10s and 30,000 per 10 min. Typically "requires a working prototype". Approval time varies by project and region. [V] — [developer.riotgames.com/docs/portal](https://developer.riotgames.com/docs/portal)
- Discord's native League account connection has been asked back by users on Discord's feedback forum, so you cannot rely on it for verification. [3P] — [Discord feedback: Bring back League connections](https://support.discord.com/hc/tr/community/posts/360051390394-Bring-back-League-of-Legends-account-connections)

### Inferences
- [I] Verification adds trust (no fake ranks) and auto-fills rank and lane from match history. It does not speed up finding a 5th. Do it as phase 2.
- [I] If the bot is later opened to other servers, it becomes "public consumption" and needs a production key. That matters for the portfolio path.

### Gaps
- Personal key approval turnaround is not stated by Riot.

---

## Q5. Option E: ASP.NET web app or board

### Takeaway
A web board adds friction compared with Discord (users must open a site and you need a way to notify them). It only beats a bot for discoverability by strangers and for portfolio value. Time to first value is **1–3 weeks** [I]. Best as a companion dashboard (`/musait` pool view, stats) on top of Option C, not a replacement.

### Cited Findings
- Existing LFG web products describe searchable profiles (rank, role, champion pool, availability) with contact through Discord. That suggests the web plus Discord pattern is standard. [3P] — [exitlag.com LFG guide](https://www.exitlag.com/blog/lfg-league-of-legends-lfg/)
- premate.gg is an example of a pure web LFG with lobbies filtered by rank, region, mode and mic (Valorant only). [V] — [premate.gg](https://premate.gg)

### Inferences
- [I] Without push notifications (Discord DM or web push), a board fails the under-15-minute goal, because nobody is watching it.

### Gaps
- No usage data found for web LFG boards on TR.

---

## Q6. Option F: fork or contribute to an open-source bot

### Takeaway
No suitable C# or .NET LoL LFG bot was found. The best-known LoL Discord bot, **mrtolkien/inhouse_bot**, is Python, GPL-3.0, and **unmaintained since December 2021**. It is built for in-house 10-player matchmaking, not filling a ranked Flex party. Forking would cost more than building Option C. Use it only as design reference (role queue, duo queue, button UX).

### Cited Findings
- inhouse_bot: Python (discord.py), GPL-3.0, "not maintained anymore as of December 2021". 115 stars, 73 forks, 259 commits, not archived. Features: role queue per channel, duo queuing, matchmaking aiming for about 50% predicted winrate, TrueSkill-like rating, `!won` scoring, rankings. Needs Docker, Docker Compose, the Server Members intent, custom role emoji and a database. [V] — [github.com/mrtolkien/inhouse_bot](https://github.com/mrtolkien/inhouse_bot)
- ynsrkn/inhouse-bot: an in-house custom-games bot on py-cord (Python). [3P] — [github.com/ynsrkn/inhouse-bot](https://github.com/ynsrkn/inhouse-bot)
- GitHub topic `lfg-bot` has only 2 public repos, neither for LoL or C#: Skerpsh/Silexa_Bot (Python, 0 stars, updated Oct 2026) and HarshPatel5940/DF-LFG-Bot (TypeScript, Delta Force, 0 stars). [V] — [github.com/topics/lfg-bot](https://github.com/topics/lfg-bot)
- C# LoL bots found are stats-only. XayahBot covers champion stats; Kiritsu/Aatrox is an empty starter. [3P] — [github.com/Kiritsu/Aatrox](https://github.com/Kiritsu/Aatrox); [github.com/topics/leagueoflegends](https://www.GitHub.com/topics/leagueoflegends)

### Inferences
- [I] GPL-3.0 means a fork must stay GPL if distributed. That is fine for a friend group and a constraint for a portfolio product.

### Gaps
- Did not search GitHub topics `discord-lfg`, `looking-for-group`, or "duo finder" in TypeScript or Python more broadly.

---

## Q7. Comparison, ranking and the two key questions

### Takeaway
**Fastest value: A + B tonight.** Post in the official TR Discord, and in their own server set up `@Flex-Available` plus lane and rank roles, a forum channel, and LFG Hub or Puck. **About 80% existing bot: no.** LFG Hub reaches roughly 60–80% (Smart or Custom Ping, waitlist, temp voice, free) but lacks opt-in availability that expires and short post expiry. **Best longer-term and portfolio: C, then D, then optionally E**, built in .NET, because those gaps are exactly what a small bot fills.

### Cited Findings (summary table; evidence is in Q1–Q6)

| Option | What it takes | Time to first value [I] | Cost | Fit for a matching 5th in under 15 min [I] | Main risks |
|---|---|---|---|---|---|
| A. Existing public channels | Join the official TR Discord (138K, ~21K online, LFG tag — [link](https://discord.com/servers/league-of-legends-turkiye-678949756982132756)) and post well-formed "+1" ads | 0 h | Free | Medium. Large live pool, but no targeting or filtering. Response time unmeasured | Post gets buried, stranger quality and toxicity, no web LFG tool verified for TR |
| B. Configure bots (LFG Hub / Puck + roles + forum) | Roles, forum tags, invite bot, set Custom Ping or `/config mention` ([LFG Hub](https://lfghub.gg), [Puck](https://top.gg/es/bot/703068724818608138)) | 30–90 min | Free ([LFG Hub "free forever"](https://lfghub.gg)) | Good if the pool exists. Availability roles go stale, no AND-filter pings | Small pool in a friend server, ping fatigue, 14-day post lifetime |
| C. Minimal .NET bot | NetCord or Discord.Net, slash commands and buttons, expiry sweeper, SQLite ([NetCord](https://www.github.com/NetCordDev/NetCord)) | 1–3 evenings | About $0–5/month hosting | Best targeting: opt-in "available for 60 min", lane and rank AND-filter, auto-expire | Hosting and maintenance, same pool problem |
| D. C + Riot API | Register for a Personal key, verify Riot ID and rank ([Riot portal](https://developer.riotgames.com/docs/portal)) | C + 1–3 days + approval wait | Free | Same speed as C, better trust and rank accuracy | Key policy (private only), approval time unknown |
| E. ASP.NET web board | Web app plus auth plus notifications | 1–3 weeks | Hosting | Poor alone (nobody watching). Good as a companion to C | Effort, low traffic |
| F. Fork OSS | inhouse_bot (Python, GPL-3, unmaintained since 2021 — [link](https://github.com/mrtolkien/inhouse_bot)) | Days to weeks to adapt | Free | Poor. Wrong problem (in-house 10-player), wrong language | Dead upstream, GPL |

### Inferences — ranking
1. **A + B together (now):** the only path that gives value tonight. A brings the large TR pool; B targets your own pool.
2. **C (this week):** the best ratio of value to effort for a .NET developer. It fixes stale availability, AND-filtering and short expiry.
3. **D (phase 2):** adds trust and a rank auto-fill. Do it after C proves usage.
4. **E (optional, portfolio):** a dashboard on top of C's data.
5. **F (do not fork):** read inhouse_bot for UX ideas only.
- [I] The real bottleneck is pool size and liquidity, not tooling. The first effort should be getting 20+ TR Flex players into the `@Flex-Available` pool, for example by inviting people met through Option A.
- [I] For a portfolio, C + D with optional E shows Discord interactions, background jobs, rate-limited external API integration and policy compliance. A public multi-server version would need a Riot production key.

### Gaps
- No measured data anywhere on how quickly "+1" posts get answered on TR Discords.
- Not verified: GameVerse, OP.GG duo (TR), Mate4Game, EZLFP, LoLFinder TR, Miko/Duoo region support, Carl-bot and Discord Onboarding official docs, the TR "Ranked 5s Fri–Sun" schedule.
