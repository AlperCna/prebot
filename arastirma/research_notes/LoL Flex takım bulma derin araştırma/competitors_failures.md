# LoL LFG competitor gaps, stat-app duo features, LFG startup failures and user sentiment

Research date: 2026-11-08. All "retrieved" dates are 2026-11-08 unless stated otherwise. Scope note from coordinator applied throughout: the product idea is an on-demand "ad board" (e.g., 4 players need a 5th for Ranked Flex *right now*), so each tool is judged on fast slot-filling: time-to-fill, "available now" status, notifications, post expiry, role slots. Earlier scan's findings (Teamplay.gg, GameTree, Noobly, GamerLink, E-Pal/TapIn, Premate.gg, Duoo.gg/Miko, Queue Up!, Gankster, DOR, Mate4Game) are not repeated.

Method caveats: reddit.com and web.archive.org page fetches were blocked for this researcher, and several sites (lolfinder.net, tracker.gg, OP.GG help center, Discord support) returned HTTP 403 to automated fetches. Where that happened, details come from search-engine snippets or third-party pages and are flagged as such. Many LFG "guides" found are written by competing LFG vendors (GameVerse, TapIn, ExitLag, Pairly, PaxJax, Kaki Gaming), so they are treated as marketing claims, not verified facts.

## Q1. Do LoLFinder.net, GameVerse, TapTeam, GankPls (and FindPre) exist, and what do they do? Any other unlisted competitors?

### Takeaway
LoLFinder.net and GameVerse are real, active 2025–2026 LoL listing boards with dedicated Flex/5-stack pages. TapTeam exists only as a just-launched, unproven app. I found no trace of GankPls; it is probably a garbled "Gankster". The bigger gap in the earlier scan is a set of other active boards. RiftQ (EUW) comes closest to an "open slots + roles" board with fresh posts, and TeamTavern has match alerts and 30-day post expiry. None of the boards I checked shows a real-time "available now" status.

### Cited Findings
**LoLFinder.net: exists (verified via snapshot, search index and third-party description)**
- The Wayback Machine availability API returns a snapshot of `https://lolfinder.net/` dated 2025-08-26 (timestamp 20250826121649), so the domain was live in Aug 2025. Direct fetches of lolfinder.net returned HTTP 403 (bot protection), and the archived copy could not be opened by this tool. — [Wayback availability API](http://archive.org/wayback/available?url=lolfinder.net)
- The site has a dedicated Flex page ("Find a partner for Flex Queue in League of Legends") and region sub-pages (e.g., `/en/flex/region/euw`). According to search snippets, you can post a party or filter by region, rank and role and message the authors of matching parties. Snippets also say listings carry role, rank and voice/no-mic preferences, and the site repeats the Flex rule that a party can be 1, 2, 3 or 5 players. — [LoLFinder Flex](https://lolfinder.net/en/flex); [LoLFinder EUW Flex](https://lolfinder.net/en/flex/region/euw)
- GameVerse (a competitor) describes LoLFinder as a "listing board": "post a party or filter listings by region, rank, and role, then message authors". Its only stated weakness is that you scroll listings by hand. — [GameVerse blog, rules checked 2026-10-22](https://gameverse.app/blog/find-duo-league-of-legends)
- Name collision: a 2014 Mobafire forum post announced an unrelated "LoL Finder" app (live game / summoner search) at lol.uxlayer.com. It is not the same product. — [Mobafire forum, 2014-01-13](https://www.mobafire.com/league-of-legends/forum/general/26931)
- Not verified: post freshness, user counts, operator identity, and whether posts expire or show online status.

**GameVerse (gameverse.app): exists, active in 2026**
- Multi-game teammate matcher (claims "12+ games"). It has a LoL team finder for a "5-stack for Clash or Ranked Flex" (modes: Clash, Ranked Flex, Normal Draft 5-stack), a separate LoL duo finder, filters for mindset (tryhard/casual), comms (voice required vs. ping-only) and region (EUW, EUNE, NA, KR, BR, LAN, OCE), built-in messaging, an Android app and a Discord bot. It says it is free with "no paywall, no credit card", and the footer reads © 2026. — [GameVerse LoL team finder](https://gameverse.app/find-players/league-of-legends-team)
- It does not match by role: players negotiate lanes in chat. The page says nothing about available-now status, notifications, post expiry or reputation. No user counts are given. Operator not named. — [GameVerse LoL team finder](https://gameverse.app/find-players/league-of-legends-team)
- GameVerse's own blog admits it "does not verify ranks". (Self-description, marketing source.) — [GameVerse blog](https://gameverse.app/blog/find-duo-league-of-legends)

**TapTeam: exists, but only as a brand-new, unproven listing**
- The only trace is a Peerlist launch profile. It describes TapTeam as a gaming app to find players by game, schedule, playstyle and communication preferences, links to tapteam.app, and says "launched this week" with no votes or discussion (snippet, launch date not visible). No press, reviews or store listing found. — [Peerlist profile](https://peerlist.io/rein12)

**GankPls: not found**
- An exact-match search for "GankPls" returned nothing relevant. The closest hits were Gankster (Tel Aviv, founded 2022, teammates + scrims + tournaments). This is probably a misremembered name. — [search hits incl. startupim Gankster record](https://startupim.com/company/gankster/raw)
- New Gankster product not in the earlier scan: "Gankster Duos: Gamer Friends" (iOS), a swipe-style duo matcher with a "Play Now" option for instant matches. It is rated 4.1 from 81 reviews (search snippet, retrieved 2026-11-08). — [App Store id6755520323](https://apps.apple.com/app/id6755520323)

**FindPre**
- Not searched in this pass for budget reasons. It remains unverified (see Gaps).

**Other LoL Flex/5-stack boards the earlier scan missed (all checked 2026-11-08)**
- **RiftQ (riftq.com), EUW-centric.** The Flex page header says "304 Teams and Players are looking for you!" (4 pages, 80 per page). The newest post was "15 minutes ago", most were 1–8 hours old, and the oldest visible was "1 day ago". Each post shows rank icon, an **open-spots count**, **role icons for roles sought** (Top/Jungle/Mid/Bot/Support/Fill), "mic verified" badges, free text, a timestamp and a "send" button. Filters cover language, elo, lane, **spots left**, voice-only, and solo vs. team posts. Posting requires Discord login plus a linked Riot account, and EUW use requires joining its Discord. No online-now status or expiry shown. © 2026, operator not named. — [RiftQ EUW Flex](https://riftq.com/euw/flex)
  - Conflict: a search-engine snippet claimed many RiftQ Flex posts were ~750 days old. The live fetch on 2026-11-08 showed posts from 15 minutes to 1 day old, so the snippet appears stale.
- **TeamTavern (teamtavern.net).** Free LoL board with player/group/community posts, filterable by rank, role, goal (Ranked, Clash, Casual, Scrims), location, language, age, Clash tier, **"usually online" hours** and mic. **It alerts you when a new post matches yours.** Player/group posts **expire after 30 days**, community posts after 90. At fetch time it showed 43 active posts, the visible ones aged 17 hours to 13 days. — [TeamTavern LoL](https://www.teamtavern.net/games/league-of-legends)
- **PaxJax (paxjax.com).** Commercial LFG with rank/role/region filters and "saved standing groups for Flex and Clash" (search snippet; vendor's own 2026 blog). — [PaxJax LoL](https://paxjax.com/games/league-of-legends/); [PaxJax blog 2026](https://paxjax.com/blog/how-to-find-people-to-play-league-of-legends-with-in-2026/)
- **TeamUp.pro.** LoL clans/teams/LFG listings (search snippet only). — [teamup.pro](https://teamup.pro/lol-teamfinder)
- **LFG.GG.** Matches on role/playstyle/rank. Free, with a paid Premium tier for unlimited messages (per Kaki Gaming guide, 2026-09-25). — [Kaki Gaming](https://kakigaming.com/blog/lol-duo-finder)
- **Also named (not checked in depth):** Pairly.gg duo finder ([pairly.gg/duo](https://pairly.gg/duo)), AnySquad (named by Kaki Gaming), 1v9.gg ([1v9 blog](https://1v9.gg/blog/lol-duo-finder)), Teamgap.gg Overwolf overlay ([Overwolf](https://overwolf.com/app/hafen_co_ltd-teamgap-gg)), Arena teammate-finder iOS app ([App Store](https://apps.apple.com/app/id6740140093)), Duos – Gamer Dating & LFG ([App Store](https://apps.apple.com/us/app/-/id1600771296)), Gamerplug ([App Store](https://apps.apple.com/mx/app/gamerplug/id6752116866)), Gameram, eBlitz, Lita.
- **Riot's own NA "Player Recruitment" forum board is archived**, with old "Flex team" posts. — [Riot boards](https://boards.na.leagueoflegends.com/en/c/team-recruitment/ExJYJoOv-flex-team)

### Inferences
- The LoL Flex niche is crowded with **listing boards** (LoLFinder, GameVerse, RiftQ, TeamTavern, PaxJax, TeamUp, LFG.GG). Almost all are asynchronous. On RiftQ, posts hours old are "fresh"; on TeamTavern, posts are typically days old and last 30 days. Neither model fits "need a 5th in the next 5 minutes."
- RiftQ is the closest existing analogue to the user's idea: "spots left" filter, role-slot icons, Riot-account verification and mic verification. It still lacks an "available now"/live status, push notifications and short post expiry, and it is regionally Discord-gated (EUW).
- Gankster Duos' "Play Now" instant-match is the only explicit real-time feature found in this pass, but it is for duos, not filling a 5th Flex slot.

### Gaps
- LoLFinder.net's post freshness, traffic, operator and expiry rules: 403 to fetch, archive copy unreadable by this tool. A manual browser visit is needed.
- FindPre was not researched.
- TapTeam's launch date, platform and traction are unknown beyond one Peerlist page.
- No traffic or user numbers for any of the new boards (RiftQ's "304" is a self-displayed count).

## Q2. Current (2025–2026) LFG/duo features of the big LoL stat apps, Discord and Riot

### Takeaway
OP.GG is the only big stat site with a real duo board ("Find Duo"/"OP.GG Duo"). It shows posters' stats, bans external links and boosting, and a 2026 guide reports it can be quiet, with the newest posts days old on smaller servers. The OP.GG desktop app can invite to a lobby only if both players have it installed. I found no LFG feature in U.GG, Mobalytics or Blitz. Discord's native "LFG Channels" exist only as a limited experiment, and Riot's 2026 Discord integration lets players share party links into Discord. Riot's own client still has no LFG board. Riot's Flex rules (parties of 1/2/3/5 only; a 4-stack cannot queue ranked Flex) create exactly the "need a 5th" moment.

### Cited Findings
**OP.GG**
- OP.GG's free "Find Duo" board (per Kaki Gaming guide, updated 2026-09-25):
  - Filters by queue and tier, and posts show recent champions, win rate and KDA.
  - The board bans payments, account trades/boosting and links to other platforms. Players connect via in-game friend request.
  - The SEA board (listed under "Singapore") is "much quieter than Korea's", and "the newest ranked posts can be days old". (Third-party; Kaki Gaming sells its own duo service.)
  - Source: [Kaki Gaming](https://kakigaming.com/blog/lol-duo-finder)
- OP.GG desktop app help article "How to find a duo on the OP.GG app" says, via search snippet (the page returned 403): "add friend" sends a friend request, "invite" invites them to your lobby, and **"the invite feature only works if the other user also has the OP.GG Desktop App installed."** — [OP.GG for desktop help center](https://opggfordesktop.zendesk.com/hc/en-us/articles/48999582601753-How-to-find-a-duo-on-the-OP-GG-app)
- A 1v9.gg blog (undated, competitor) says OP.GG's "revamped" Duo Finder filters by rank, position and region and has a bulletin board for duo requirements. — [1v9.gg](https://1v9.gg/blog/how-to-find-your-perfect-lol-duo-partner)
- GameVerse (competitor) characterizes OP.GG Duo as "stat-based partner pages" that start "from stats rather than compatibility". — [GameVerse blog](https://gameverse.app/blog/find-duo-league-of-legends)
- OP.GG Talk (talk.op.gg) is a Korean community forum where duo topics are discussed (e.g., a thread on allowable duo tier gaps). It is not a matching tool. — [talk.op.gg](https://talk.op.gg/s/lol/free/1219869/%EB%93%80%EC%98%A4%20%ED%8B%B0%EC%96%B4%EC%B0%A8%EC%9D%B4%20%EC%96%B4%EB%94%94%EA%B9%8C%EC%A7%80%20%EA%B0%80%EB%8A%A5%ED%95%9C%EA%B0%80%EC%9A%94)
- I could not determine the exact current URL of OP.GG's duo board (`op.gg/lol/duo` and `op.gg/lol/find-duo` both returned 404 on 2026-11-08).

**U.GG, Mobalytics, Blitz.gg, Porofessor, League of Graphs**
- Searches found no duo/LFG feature for Mobalytics, U.GG or Blitz.gg. Coverage describes stats, builds, overlays and a "personal assistant" (Mobalytics). — [Mobalytics Overwolf listing](https://www.overwolf.com/app/mobalytics-mobalytics); [esports.net tool roundup](https://www.esports.net/news/lol/best-league-of-legends-tools/)
- One roundup says Porofessor has a "player finder" usable to look up players to play with. This is weak, possibly a misread of player lookup, and unverified. — [RealSport101](https://realsport101.com/article/the-best-companion-apps-for-lol)
- League of Graphs was not checked.

**tracker.gg**
- A third-party guide says Tracker.gg and OP.GG "both maintain League of Legends LFG sections" that let users browse by rank, role and region. A direct fetch of tracker.gg/lfg returned 403, so this is unverified. — [ExitLag blog](https://www.exitlag.com/blog/lfg-league-of-legends/)

**Discord**
- Discord support article "How to Use the LFG Channels to Find Game Sessions on Discord" (via search snippet; the page returned 403):
  - A forum-style LFG channel where members can post and browse **live game invites** for select supported games.
  - "Currently available in the official ARC Raiders server", "part of a limited experiment and is only visible in select servers", and available only to members who have the game installed.
  - Source: [Discord Support](https://support.discord.com/hc/nl/articles/41832532728215-How-to-Use-the-LFG-Channels-to-Find-Game-Sessions-on-Discord)
- A Discord feedback thread "Official LFG" shows long-standing user demand for built-in LFG like Xbox/PSN. — [Discord feedback](https://support.discord.com/hc/en-us/community/posts/360062348071-Official-LFG)
- Generic Discord forum channels support tags, which servers use as a DIY LFG workaround. — [GamesBeat on Forum Channels](https://gamesbeat.com/discord-adds-forum-channels-for-more-specific-uses)

**Riot client**
- Riot's Discord integration launched in beta with LoL patch 26.8 (article dated 2026-04-15). It lets players "invite Discord friends directly into their party", **share party links to Discord servers**, and see friends active in other Riot games. It launched first in US, Canada and Brazil. No LFG feature is mentioned. — [Fragster](https://www.fragster.com/league-of-legends-patch-26-8-mel-and-dr-mundo-nerfed-discord-integration-launches-in-beta/); [esports-news.co.uk](https://esports-news.co.uk/?p=188683)
- A full client overhaul is slated for 2027. — [esports.gg](https://esports.gg/news/league-of-legends/lol-new-client-map-updates-2027); [GamesRadar](https://www.gamesradar.com/games/league-of-legends/league-of-legends-is-getting-a-new-client-entirely-new-visuals-and-a-bit-of-new-gameplay-in-a-massive-2027-update-reportedly-codenamed-league-next)
- Riot's "Party Finder" exists only in Wild Rift (mobile), not PC LoL. — [Wild Rift support](https://support-wildrift.riotgames.com/hc/articles/4405460661907)
- **Ranked Flex party rules:**
  - Ranked Flex accepts parties of 1, 2, 3 or 5, so **a party of four cannot queue** and must find a fifth or play normals.
  - Reported rank rules: no Flex restrictions for Diamond and below, and Master+ partners require at least Emerald.
  - Sources are third-party, so verify in-client: [Pairly](https://pairly.gg/blog/lol-flex-queue-rules-rank-restrictions-party-size); [Unplayed](https://unplayed.gg/blog/lol-flex-queue-explained); [LoLFinder Flex](https://lolfinder.net/en/flex)
- **Riot's duo restrictions (2026):**
  - Patch 16.1: duo disallowed up to Grandmaster, per Riot's Matt Leung-Harrison on X. — [X post](https://x.com/RiotPhroxzon/status/2012091893530927146)
  - Duo queue disabled for Apex tiers in Solo/Duo for patch 16.9 after a ranked reset (article dated 2026-04-27). — [Sheep Esports](https://www.sheepesports.com/us/lol/articles/how-the-temporary-removal-of-duo-queue-will-impact-the-ranked-reset/en)
  - Riot is also cracking down on duo abuse. — [PCGamesN](https://www.pcgamesn.com/league-of-legends/duo-queue-restrictions-matchmaking-penalties)

### Inferences
- None of the major stat apps offers a real-time "fill the 5th slot" flow. OP.GG's board is duo-oriented and stats-first, its anti-link rule blocks Discord handoff, and its in-app invite needs both sides on OP.GG Desktop.
- Riot's 2026 Discord party-link sharing lowers friction for "post the lobby link into a Discord LFG channel". That strengthens Discord as the default competitor, but it also gives a third-party board a cheap "join lobby" handoff.
- Discord's native LFG channel experiment (live invites, ARC Raiders only) is the biggest platform risk. If it expands to LoL community servers, it would directly overlap the "post a live invite, fill fast" concept.
- Tightened Solo/Duo rules in 2026 may push more groups into Flex, which supports demand, but no source quantifies this.

### Gaps
- The current OP.GG duo board URL, whether it supports Flex 5-stack recruiting with role slots, and its post volume per region (TR in particular) are not verified.
- tracker.gg LFG status (403), League of Graphs, and Porofessor's actual feature are unverified.
- The Discord LFG Channels article's publish date and whether it has expanded beyond ARC Raiders are unknown. The "as of August 2026" phrasing came from a TapIn explainer, not from Discord. — [TapIn](https://tapin.gg/blogs/what-is-lfg-in-gaming)

## Q3. Post-mortems and shutdowns of LFG / gamer-matching startups, and the100.io lessons

### Takeaway
I found no founder-written post-mortem on Indie Hackers, Hacker News or Medium for a LoL/LFG startup. The two clearest 2025–2026 data points are TEAMS.gg and Guilded:
- **TEAMS.gg** shut down after 6 years and 1.5M connection requests because ads and subscriptions (tried since 2023) never covered costs. This is a monetization failure despite usage.
- **Guilded** ($90M Roblox acquisition) was sunset on 2025-12-19 when the acquirer folded it into Roblox Communities. This shows platform/owner dependence.

The100.io, the archetype of scheduled sessions + karma, still runs, but its own homepage now shows only 218 weekly active players and 4 games with sessions in the last 7 days. Its iOS app has not been updated since Dec 2020, and Bungie's in-game Fireteam Finder now duplicates its core.

### Cited Findings
**TEAMS.gg: shut down (exact date unknown, between 2023 and 2026-11-08)**
- The shutdown page reads "6 years and 1,500,000 connection requests later, TEAMS is closing down." It says the team "experimented with ads and subscriptions to keep TEAMS alive" since 2023, and that these "haven't been enough to cover the costs of running the site." The site "has now been switched off". No alternatives recommended. — [teams.gg](https://teams.gg/)
- Earlier coverage: matched on availability, commitment, skill, ambition, language and location, with "more than 125,000 users" with profiles (2023 figure). It covered Valorant, Apex, Fortnite, Rocket League and Overwatch 2, plus anti-toxicity measures for under-16s. — [PCGamesN](https://www.pcgamesn.com/teams-gg-multiplayer-games); [esports-news.co.uk 2022-06-21](https://esports-news.co.uk/2022/06/21/teams-gg-apex-legends-fortnite-help-players-avoid-toxicity); [MCV/UK](https://mcvuk.com/business-news/teams-gg-the-dream-team-solution/)

**Guilded: acquired, then killed**
- Timeline:
  - Founded by Eli Brown (ex-Facebook/Xbox), launched 2017-03-17. — [Wikipedia](https://en.wikipedia.org/wiki/Guilded)
  - Raised over $7M (May 2020). — [Wikipedia](https://en.wikipedia.org/wiki/Guilded)
  - Acquired by Roblox on 2021-08-16 for $90M. — [Wikipedia](https://en.wikipedia.org/wiki/Guilded)
  - In 2024, Roblox account-linking became mandatory, and users left over privacy and platform-neutrality concerns. — [Wikipedia](https://en.wikipedia.org/wiki/Guilded)
  - Sunset announced 2025-09-29 (AlternativeTo says 2025-09-28). — [Wikipedia](https://en.wikipedia.org/wiki/Guilded); [AlternativeTo](https://alternativeto.net/news/2025/11/guilded-to-shut-down-on-december-19-2025-as-focus-shifts-to-roblox-communities/)
  - Website and apps closed on 2025-12-19, and mobile apps were removed weeks later. — [Wikipedia](https://en.wikipedia.org/wiki/Guilded)
- Features: clan-oriented scheduling, integrated calendars, whisper groups, Kanban boards and docs. — [Wikipedia](https://en.wikipedia.org/wiki/Guilded)
- Stated reason: to "unify social features into 'Roblox Communities'". — [Wikipedia](https://en.wikipedia.org/wiki/Guilded); [Roblox DevForum "Update on Guilded and Communities"](https://devforum.roblox.com/t/update-on-guilded-and-communities/3966775)
- An export tool was offered before shutdown. — [AlternativeTo](https://alternativeto.net/news/2025/11/guilded-to-shut-down-on-december-19-2025-as-focus-shifts-to-roblox-communities/)

**The100.io: alive but shrunken**
- Operated by Resonant Ventures LLC (© 2026). Tagline: "Game night for grown-ups." — [the100.io](https://www.the100.io/)
- Mechanics (self-description on the homepage):
  - Users set a recurring weekly play time and join sessions with open slots, or get matched into a group.
  - Full games use a **waitlist, invited in join order**, plus **reserve/backup players**.
  - **Reminders and a karma system "to reduce no-shows"** (no penalty details given).
  - Players keep crews across games. LFG and Looking-For-Clan sections, a Discord scheduling bot, and an iPhone app.
  - Source: [the100.io](https://www.the100.io/)
- **Current activity shown on its own homepage (2026-11-08): "218 players active this week" and "4 games with sessions in the last 7 days"** across a list of ~13 supported games (Destiny 2, Helldivers 2, Warframe, Marathon, etc.). — [the100.io](https://www.the100.io/)
- iOS app:
  - Users are placed into permanent groups of ~100 players. LFG posts can be private, group-only or public, with reminders, realtime chat, a Discord bot, and calendar sync for "supporters". — [App Store](https://apps.apple.com/us/app/the100-io-destiny-2-groups/id972830288)
  - Rated 4.7 from 1.2K ratings. First version 2016-02-16, **last update 2.2.0 on 2020-12-10**. — [App Store](https://apps.apple.com/us/app/the100-io-destiny-2-groups/id972830288)
  - Reviews praise finding "like-minded players" (2022-03-22) and a raid group still played with (2019-10-11). — [App Store](https://apps.apple.com/us/app/the100-io-destiny-2-groups/id972830288)
- Monetization: free, with optional "supporter" perks; no prices shown. — [App Store](https://apps.apple.com/us/app/the100-io-destiny-2-groups/id972830288)
- Clan pages describe karma as a social norm ("give Karma to other Guardians if you have enjoyed playing with them"). One clan says scheduling events raises a group's activity score, and the100 then auto-adds more people. That is user-described, not official. — [the100 group 3023](https://www.the100.io/groups/3023); [the100 search results incl. group pages](https://www.the100.io/groups/2185)
- Platform cannibalization: Bungie's in-game Fireteam Finder (2023) was framed as an official alternative to third-party sites such as The100.io, DestinyLFG and GamerLink. GamesRadar noted that DestinyLFG users would "be familiar with most of what Bungie is trying to do". — [Destiny Game Wiki](https://d2.destinygamewiki.com/wiki/Fireteam_Finder); [GamesRadar](https://www.gamesradar.com/destinys-finally-getting-an-official-lfg-tool-next-week)
- No traffic data or statements from the100 or DestinyLFG about Fireteam Finder's impact were found. — [Destiny Game Wiki](https://d2.destinygamewiki.com/wiki/Fireteam_Finder)

**Plink: alive, monetization-heavy**
- Swipe-to-match teammate app by PLINK INCORPORATED, rated 3.5 from ~50K ratings. It updated frequently through 2025–2026 (1.46.2 on 2025-05-14 to 1.48.8 "May 7"), every note reading "Small UI/UX fixes".
- Subscriptions: "BATTLE PASS" / PLINK Pro at $2.99–$19.99.
- A 2020-06-02 review complained of a paywall "the first 2 minutes of the app".
- No real-time/online-now matching is described.
- Source: [App Store](https://apps.apple.com/us/app/-/id1306783602)
- An aggregator reports bot accounts and scammers hurting the experience (unverified aggregator). — [mwm.ai](https://mwm.ai/apps/plink-team-up-chat-play/1306783602)
- Plink originated in Crytek's CRYCASH token ecosystem. — [Crytek press](https://press.crytek.com/crycash-provides-early-access-to-plink-alpha-to-all-token-sale-contributors)

**Founder-cited reasons, generic and adjacent**
- CB Insights' analysis of 431 VC-backed shutdowns: "ran out of capital" is listed in 70% but is usually the final cause. Root causes were poor product-market fit (43%), bad timing (29%) and unsustainable unit economics (19%). — [CB Insights](https://www.cbinsights.com/research/report/startup-failure-reasons-top/)
- Indie Hackers threads on two-sided cold start: "people with real audiences don't usually team up with apps that have no users", and "test demand before building". These are anecdotes, not LFG-specific. — [Indie Hackers](https://www.indiehackers.com/post/im-really-stuck-with-my-app-can-someone-help-me-figure-out-what-to-do-f1c3d2feac)

### Inferences
- Failure modes evidenced in 2025–2026:
  - **Monetization**: TEAMS.gg had volume (1.5M requests) but ads/subscriptions did not pay for hosting.
  - **Platform/owner dependence**: Guilded was killed by its acquirer's strategy, and the100 was undercut by Bungie's native LFG.
  - **Single-game lifecycle**: the100's fortunes track Destiny.
  - **Product stagnation**: no iOS update since 2020.
  - Discord's live-invite LFG experiment and Riot's Discord party links are the LoL equivalents of the Bungie risk.
- The100's design (open slots, join-order waitlist, backups, reminders, karma) is directly reusable for fast slot-filling: a "backup/reserve" list is effectively a pre-qualified pool to ping when a slot opens. Its scheduled-weekly framing is the opposite of the user's on-demand model, and its tiny current activity shows that scheduled-group liquidity also decays without a hit game.
- Plink shows the monetization path many survivors take (aggressive subscription paywalls), at the cost of ratings (3.5★) and trust.
- **Why real-time LFG boards die**, synthesized from Q3/Q4 evidence:
  1. Liquidity collapses at off-hours and on smaller servers (OP.GG SEA board posts "days old").
  2. Stale posts with long expiry (TeamTavern's 30 days) make boards look alive while nobody is actually available.
  3. High-volume channels (Discord) bury posts in about a minute.
  4. No verification invites smurfs and dodgers.
  5. Monetization rarely covers costs (TEAMS.gg).
  6. Platform owners clone the feature (Bungie, Discord).

### Gaps
- No founder-written post-mortem was found for any LoL-specific LFG tool, DestinyLFG, LFG.pub or a dead Discord LFG bot. Searches on Indie Hackers/HN/Medium returned only generic posts.
- TEAMS.gg's exact shutdown date and the100's historical peak user numbers were not found (the archive could not be read).
- No data was found on The100's revenue or "supporter" conversion.

## Q4. Real user sentiment about LFG tools (Reddit and elsewhere)

### Takeaway
Reddit itself could not be accessed (fetch blocked, and search engines returned no Reddit threads for the queries tried), so direct r/leagueoflegends / r/summonerschool sentiment is **not captured**. The proxies available are consistent:
- Discord LFG posts get buried within about a minute and attract smurfs/dodgers.
- Smaller boards look dead or days-stale.
- Reddit LFG is low-volume.
- Users like scheduled/persistent groups when they work (the100 reviews).
- Users dislike paywalls (Plink).

### Cited Findings
- GameVerse (competitor) blog, rules checked 2026-10-22. Treat as vendor framing.
  - **Discord LFG**: active at all hours, but posts "scroll away within about a minute", replies include "smurfs and people who dodge champ select", there is no rank verification, and "no memory, so you must repost each time".
  - **Reddit LFG posts**: "low volume" and nobody stops rank misstatement.
  - **In-game randoms → add friend**: free, but one game is a weak test of fit.
  - Source: [GameVerse blog](https://gameverse.app/blog/find-duo-league-of-legends)
- Kaki Gaming (2026-09-25): OP.GG's SEA board is "much quieter than Korea's" and the newest ranked posts "can be days old". It recommends "recent games and post-game lobby" as the best free source of same-rank players. — [Kaki Gaming](https://kakigaming.com/blog/lol-duo-finder)
- Dead-channel anecdotes from other game communities:
  - A Steam user found a Discord "seems dead with most of the posts being from several years ago". — [Steam theHunter discussion](https://steamcommunity.com/app/518790/discussions/0/3809531058949695149)
  - A WoW forum user said RBG LFG communities with hundreds of members "rarely ever see any group being posted… no answers". — [Blizzard EU forums](https://eu.forums.blizzard.com/en/wow/t/active-rbg-communitydiscord/254538)
  - An itch.io user said "my last LFG was largely ignored". — [itch.io](https://itch.io/post/8893320)
- A vendor guide flags "most channels are dead while the member count is enormous" as a red flag, and says specific posts (mode, region) get better replies. — [TapIn Fortnite LFG guide 2026](https://spacex.tapin.gg/blogs/fortnite-lfg-discord-alternatives-2026)
- An AV Club commenter called the100.io "the only one I've really used" among LFG services. — [AV Club](https://avclub.com/why-does-online-gaming-have-to-be-so-anxiety-inducing-1819007386)
- the100 App Store reviews are praise-heavy, with one feature complaint: wanting to see more than one game at a time (2023-06-07). — [App Store](https://apps.apple.com/us/app/the100-io-destiny-2-groups/id972830288)
- Plink review complaint about an immediate paywall and hard cancellation (2020-06-02). — [App Store](https://apps.apple.com/us/app/-/id1306783602)
- A Bungie forum post (2022) asked for an LFG that works "actively instead of passively". It is an echo of the push-vs-browse problem. — [via Destiny Fireteam Finder search results](https://www.bungie.net/ko/Forums/Post/260579162)

### Inferences
- The consistent complaint pattern maps onto the user's idea. Users want posts that do not get buried (Discord), boards that are not stale (OP.GG SEA, long-expiry boards), active matching rather than passive browsing, and some verification (rank/mic) without a paywall.
- Liquidity is the key risk, and it is heavily region- and hour-dependent. Even OP.GG's board is quiet outside Korea. For a Turkish/TR-server launch (implied by the project folder name), this suggests seeding inside existing Discord communities rather than expecting standalone traffic. This is an inference; no TR-specific data was found.

### Gaps
- No direct Reddit threads ("best way to find flex team", "LFG sites are dead", "finding a 5 stack") were retrieved. reddit.com fetch was blocked and search returned none. A manual Reddit search is recommended.
- No quantitative sentiment data (surveys, ratings across LoL LFG apps) was found.
- No TR-server-specific sentiment or board activity was measured.

## Appendix-style summary for the report writer: fast slot-filling fit (included under the coordinator's scope note)

### Takeaway
None of the tools checked combines all five slot-filling primitives: "available now" status, role-slot posting, push notification to matching idle players, short auto-expiry, and Riot-verified rank. RiftQ has role slots + verification, TeamTavern has match alerts, Gankster Duos has "Play Now" for duos, Discord's experiment has live invites, and the100 has waitlist/backups.

### Cited Findings
- RiftQ: open-spots count, role icons, spots-left filter, Riot account + mic verification; no online-now or expiry shown. — [RiftQ](https://riftq.com/euw/flex)
- TeamTavern: match alerts, "usually online" hours, 30-day expiry (stale-prone). — [TeamTavern](https://www.teamtavern.net/games/league-of-legends)
- GameVerse: Flex/Clash 5-stack finder, no role matching, no available-now/expiry/notification mentioned. — [GameVerse](https://gameverse.app/find-players/league-of-legends-team)
- OP.GG Find Duo: stats on posts, no external links allowed, invite requires both on OP.GG Desktop, quiet off-Korea. — [Kaki Gaming](https://kakigaming.com/blog/lol-duo-finder); [OP.GG help](https://opggfordesktop.zendesk.com/hc/en-us/articles/48999582601753-How-to-find-a-duo-on-the-OP-GG-app)
- Gankster Duos: "Play Now" instant match (duo). — [App Store](https://apps.apple.com/app/id6755520323)
- Discord LFG Channels: live game invites, limited experiment (ARC Raiders). — [Discord Support](https://support.discord.com/hc/nl/articles/41832532728215-How-to-Use-the-LFG-Channels-to-Find-Game-Sessions-on-Discord)
- the100.io: open slots, join-order waitlist, reserve/backup players, reminders, karma vs. no-shows. — [the100.io](https://www.the100.io/)

### Inferences
- Differentiation space for the user's idea is a "live slot" post (e.g., "4/5, need Support, Gold–Plat, mic, expires in 15 min") pushed to players who have toggled "available now". It could borrow the100's backup-pool and karma (no-show) concepts, RiftQ's Riot verification, and Riot's 2026 Discord party-link handoff. The main threats are Discord's native LFG expansion and off-hours liquidity.

### Gaps
- No tool publishes time-to-fill metrics. TapIn's "average wait under two minutes" claim (earlier scan's E-Pal/TapIn) is marketing and unverified. — [TapIn blog](https://spacex.tapin.gg/blogs/find-your-ideal-lol-duo-teammate-on-tapingg)
