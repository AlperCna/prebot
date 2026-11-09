# In-game availability and notification timing for PreBot (TR, Ranked Flex)

Claim under test: "An 'available' player spends most of their evening inside a match (~30 min) or champ select. If the notification arrives mid-game, by the time the game ends the listing (15 min) has expired. So the part of the 'available' pool that can actually respond is small. A 'notify me after my match' or 'I'm in lobby / I'm in game' distinction is needed."

PreBot parameters assumed: availability window 30 min–3 h; notification waves of 5 / 10 / 15 people at t = 0, 3, 6 min; listing lifetime 15 min; channels are web push (PWA) and Discord DM.

Research date: 9 November 2026. Several key sites (League of Graphs, yearin.lol, PMC, Discord support, X/Twitter) returned 403, 402 or CAPTCHA to the fetch tool, so some figures come from secondary pages. This is flagged below.

---

## 1. How long is a game and a full game cycle (queue + champ select + loading + game), and how much of an evening is "in game"?

### Takeaway
The "~30 minutes" part of the claim holds up. Ranked games average about 30–31 minutes, ranging from about 28 minutes (Master+) to about 33 minutes (Iron). Champ select adds up to about 3:50 if every timer runs out in 2026, and queue times vary. That puts one full cycle at about 38–42 minutes, of which about 75–80% is spent in loading or in the game. A back-of-envelope model (below) gives these shares for a player who chains games: about 1 in 8 can be reached within the first few minutes, and about 25–45% can respond before the listing expires, depending on the wave.

### Cited Findings
- **Game length by rank.** These come from ranked Solo/Duo data for October 2024, excluding surrenders. Iron 33:03, Bronze 32:32, Silver 32:00, Gold 31:33, Platinum 31:03, Emerald 30:23, Diamond 29:25, Master 28:31, Grandmaster 28:11, Challenger 28:01. A December 2024 report, whose surrender handling is not stated, shows about 1:30–2:00 less per tier: Iron 30:57, Gold 30:06, Challenger 25:52. Both datasets are from 2024; the page has no 2025-specific data. — [turbosmurfs.gg analysis](https://turbosmurfs.gg/?p=8567)
- **A "2026 / current patch" dashboard** gives about 30:28 average for Ranked Solo/Duo across all regions. Its per-rank figures (Challenger 28:01, Iron 33:03) are identical to the October 2024 snapshot above, so they are probably not fresh 2026 data. Treat with caution. — [leagueoflegendstools.com](https://leagueoflegendstools.com/statistics/average-game-length/)
- **Pro play 2026 (context only, not solo queue).** LPL averaged 32:25 over 674 games and LCK 32:13 over 502 games, between 14 Jan and 13 Sep 2026. — [Outdrafted LPL](https://outdrafted.lol/q/average-game-length-lpl); [Outdrafted LCK](https://outdrafted.lol/q/average-game-length-lck)
- **Riot's 2026 pacing changes.** Riot's 1 Dec 2025 season preview says that from January 2026 games start about 35 seconds faster, with minions spawning at 0:30. It also says Swiftplay is about 8 minutes shorter than a standard game. — reported in [turbosmurfs.gg](https://turbosmurfs.gg/?p=8567), citing Riot's 1 Dec 2025 preview
- **Champ select.** Riot is "cutting down on animations and timers in Champ Select by roughly 30 seconds". This was tested in NA/OCE and went worldwide in patch 26.1. In Master+, a dodge counts as a full loss. — [Riot, "Dev: Ranked 2026", 1 Dec 2025](https://leagueoflegends.com/en-ph/news/dev/dev-ranked-2026/)
- **Ranked draft timers** add up to about 4:20 if every timer runs out. With the 2026 cut that is about 3:50 (the page's own arithmetic). — [turbosmurfs.gg](https://turbosmurfs.gg/?p=8567)
- **Flex queue time.** Riot reported Ranked Flex queues at "~7 min on average for 3 person parties". It also said there were "not enough 1s and 2s to fill them out". This is dated (spring 2022) and global, not TR-specific. — [Riot, "The State of Competitive – Spring 2022"](https://www.leagueoflegends.com/en-au/news/dev/the-state-of-competitive-spring-2022/)
- **Older Riot figure:** about 3-minute queues across all players and about 8.5 minutes for Master+, measured off-peak. The era is undated (probably 2016–2018). The same Riot material cites an 8-minute champ select as a past pain point (2018). — search summary of [Riot matchmaking dev post 2018](https://nexus.leagueoflegends.com/en-us/2018/03/dev-making-matchmaking-better/) (page not fetched directly)
- **Riot says queue-time changes made "back in September" produced faster queues**, without giving numbers. — search summary of [Riot Ranked 2026 coverage](https://www.gamegrin.com/news/league-of-legends-ranked-changes-coming-2026-season-1-act-1-for-demacia) (not fetched directly)

### Inferences
- **Cycle model for one back-to-back Ranked Flex game in 2026.** Every input is an assumption:
  - Queue: about 2–7 min (midpoint 4)
  - Champ select: about 3–4 min (3.5)
  - Loading screen: about 1–2 min (1.5; unsourced)
  - Game: about 30 min
  - Post-game screen and re-queue: about 1 min (unsourced)

  Total is about 40 min:
  - In loading or game: about 31.5 min (about 79%)
  - In champ select: about 3.5 min (about 9%)
  - In lobby, queue or post-game: about 5 min (about 12%)

  The claim that players spend "most of the evening in a match or champ select" is supported, at about 88% of the time, *if* the player chains games without breaks. Section 2 shows that many sessions are short, so this is an upper bound.
- **Chance of responding before the listing expires.** Assume a player who is in game when notified has a remaining game time spread evenly between 0 and 30 min. They then need about 1.5 min for the post-game screen and to see the notification. The chance they can still respond is about (15 − t_wave − 1.5) / 30:
  - Wave 1 (t = 0): about 45%
  - Wave 2 (t = 3): about 35%
  - Wave 3 (t = 6): about 25%

  A player in champ select at notification time has about 0% chance unless they dodge, which costs LP and is a full loss at Master+. A player in lobby or queue has about 100% chance. Weighted by the cycle shares, the reachable share is about 48% of wave 1, about 40% of wave 2 and about 32% of wave 3. Those who do respond from in-game arrive late, about 7 minutes after notification on average. That is a problem when the group wants a 5th player quickly.
- **Absolute numbers.** "Small share" does not automatically mean "nobody". With 12% of players reachable immediately, the chance that at least one of N notified players is immediately reachable is about 47% for N = 5, about 85% for N = 15 and about 98% for N = 30. This counts only *reachable*, not *willing*. If only 30% of reachable people actually accept, the 30-person cumulative pool falls to about 67%. For a small TR Flex pool, the claim is directionally right. Wave 3 (people 16–30) has the worst timing, because recipients get only about 9 minutes of listing life.
- The 12% "idle" share assumes back-to-back play. If PreBot users who mark "available" are actually waiting idle for a Flex 5-stack rather than playing solo queue meanwhile, the reachable share is much higher. PreBot's own data could settle this: time from notification to response, split by response and non-response.

### Gaps
- No primary 2025 or 2026 patch-by-patch average game duration was retrieved. League of Graphs, lolalytics and op.gg could not be fetched (403). The 2024 figures are the most reliable available. They are likely still accurate to within about 1–2 minutes, given only small 2026 pacing changes.
- No current (2025–2026) official Ranked Flex queue-time figure exists, and none for the TR server at all. TR Flex queue time is probably longer than the global figure at off-peak hours (inference only).
- No sourced loading-screen duration or post-game screen duration was found. These are assumptions in the model.
- Flex-specific game duration (as opposed to Solo/Duo) was not found.

---

## 2. Typical session behavior: games per session, breaks between games

### Takeaway
The only rigorous source found is an academic study from 2018 on highly active Solo Queue players. It defined a session as consecutive games with breaks of at most 15 minutes. The average session lasted about 52 minutes, roughly 1.5 games. Many "sessions" are therefore 1–2 games, separated by breaks of more than 15 minutes. That gives more idle (reachable) time than the back-to-back model in section 1.

### Cited Findings
- Sapienza et al. (2018) analyzed about 242,000 Solo Queue matches by 16,665 of the most active players. The session definition was "no more than a 15-minute break between matches". Their summary table gives an average of 132 sessions and 239.7 matches per player, an average match duration of 33.8 minutes and an average session play time of 52.0 minutes. Performance deteriorates over a session, though experience mitigates this. — [Sapienza et al. 2018, PDF](https://iris.uniupo.it/bitstream/11579/181344/2/sapienza-et-al-2018-individual-performance-in-team-based-online-games.pdf); [PMC version](https://pmc.ncbi.nlm.nih.gov/articles/PMC6030337) (both are via a search summary; the PMC page returned a CAPTCHA to the fetch tool)
- 239.7 matches across 132 sessions works out to about 1.8 matches per session. The 52-minute average session time divided by the 33.8-minute match length gives about 1.5. These ratios are derived here, not reported by the study. — derived from [Sapienza et al. 2018](https://iris.uniupo.it/bitstream/11579/181344/2/sapienza-et-al-2018-individual-performance-in-team-based-online-games.pdf)

### Inferences
- If typical play is 1–2 games followed by a break of more than 15 minutes, someone "available" for 2 hours is probably idle for a meaningful part of that window, not 88% busy. The real reachable share probably falls between the back-to-back estimate (about 12% immediately) and much higher figures for players who are genuinely waiting. Only PreBot telemetry can pin this down.
- Players who mark themselves "available" specifically for a Flex group may behave differently from the study's sample (2018, Solo Queue, very active players). Treat the study as indicative only.

### Gaps
- There are no 2023–2026 public data on games per session, the distribution of breaks between games, or evening play patterns on TR.
- The study's distribution (share of 1-game versus 3-or-more-game sessions) could not be retrieved because the PMC page was blocked.

---

## 3. Riot API: can a third-party app detect that a player is in an active game? (Spectator-V5, key types, rate limits, policy, 2025–2026 changes)

### Takeaway
Yes, technically. `GET /lol/spectator/v5/active-games/by-summoner/{encryptedPUUID}` returns the live game, including `gameStartTime` and `gameLength` (seconds elapsed). It returns 404 when there is no game. This makes it possible to estimate how long a game has left. Policy is the blocker for a public app: a personal key "may not" be used for public consumption, so PreBot needs a production key. Spectator only covers a game *after* it starts, not lobby, queue or champ select. Riot also briefly switched Spectator-V5 off in October 2025, and live data now respects streamer mode.

### Cited Findings
- **Endpoint and data.** The endpoint is `GET /lol/spectator/v5/active-games/by-summoner/{encryptedPUUID}`. It returns `gameId`, `gameStartTime` ("epoch milliseconds"), `gameLength` ("the amount of time in seconds that has passed since the game started"), `gameQueueConfigId` (the queue type) and `participants`. A participant's PUUID is "null when the player is anonym". The response-code list shows 404 as "Data not found" and 429 as rate limit exceeded. — [Riot API reference mirror (riot-docs-mcp, spectator-v5.md)](https://raw.githubusercontent.com/chirpon9/riot-docs-mcp/main/src/riot_docs/spectator-v5.md)
- Third-party documentation describes the endpoint as "Get current game info (404 if not in game)". — search summary of [jentic Riot API listing](https://jentic.com/apis/developer.riotgames.com/riot-games)
- **Spectator-V5 launch.** Riot DevRel announced Spectator-V5, which uses PUUID instead of SummonerId, as available "with your approved League of Legends or Development API Key" (March 2024). — [RiotGamesDevRel on X](https://twitter.com/RiotGamesDevRel/status/1764780016640852222) (via search snippet)
- **Development key.** It "deactivate[s] every 24 hours", and "You may not maintain a public product with a development API key." — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Personal key.** It is for "products that are intended for just the developer or a small private community". Examples include bots for streaming sites or voice servers, your own stats on a personal site, and personal research. Rate limit is 20 requests per 1 s and 100 requests per 2 min, per region. Riot states: "You may not run your application for public consumption using a personal key", including open alpha and beta tests. Registration needs no verification, but personal keys get no rate-limit increases. — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Production key.** It is for "large communities or the Internet as a whole". Starting limits are 500 requests per 10 s and 30,000 requests per 10 min, per region. It "typically requires a working prototype before receiving an API key", and each project needs its own registration. Riot states: "The process and length of time required to obtain an approved production key can vary". No duration is given. — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Approval criteria.** Riot looks at whether the use case is good and approved, and whether the developer shows they will deliver it. It contacts approved developers through portal messaging. — search summary of [Riot application process page](https://developer.riotgames.com/application-process.html) (not fetched directly)
- **General policies.**
  - "Do not use a Production API key to run multiple projects (one product per key)"
  - "Products cannot de-anonymize players who cannot reasonably be identified from visible information"
  - "Products cannot create alternatives for official skill ranking systems"
  - Products must offer a free tier
  - All products must be registered and audited, and new features need auditing too

  — [Riot Developer General Policies](https://developer.riotgames.com/policies/general)
- **October 2025 shutdown and reversal.**
  - 17 Oct 2025: Riot DevRel announced it would deactivate Spectator-V5 for League "to prevent third party applications from deanonymizing players", in coordination with new anonymity features. — [RiotGamesDevRel on X](https://x.com/RiotGamesDevRel/status/1979263978787246391) (via search snippet; X returned 402)
  - About 20–21 Oct 2025: Riot "reenabled the entire Spectator API (including the getCurrentGameInfoByPuuid endpoint)". — [League of Legends on X](https://x.com/LeagueOfLegends/status/1980434308268687850) (via search snippet)
  - Riot added that live spectate options on third-party sites would be disabled and replaced by replay-file access, that featured games would be disabled, and that live game data on third-party sites "will respect players' streamer mode settings". — [League of Legends on X](https://x.com/LeagueOfLegends/status/1980434309736771638) (via search snippet)
- **Replay endpoint and RSO.** A DevRel post said the League replay endpoint would be replaced on 23 Oct 2026 by one requiring RSO authorization. The year shown in the snippet was ambiguous and is unverified. — search summary of [RiotGamesDevRel on X](https://x.com/RiotGamesDevRel)
- **Streamer mode (patch 25.20, 6 Oct 2025).** "Hide My Name" hides Riot ID and Game Name "to all other players in champ select, in game, and in third party apps". "Hide My Everything" also hides account progression. — [Riot, "Dev: Account Linking and Streamer Mode"](https://www.leagueoflegends.com/en-us/news/dev/dev-account-linking-and-streamer-mode/)
- A fan guide reports that since patch 25.20 Riot's API stopped serving player identity data to third-party apps for Streamer Mode users during a live match. This is secondary and unconfirmed. — search summary of [loltheory streamer mode guide](https://blog.loltheory.gg/lol-streamer-mode/)
- **Champ select is off limits for third-party info.** From patch 12.22 (late 2022), "Apps that reveal player names or stats in Champ Select will be asked to remove this feature immediately" (Solo/Duo and Flex). Riot's rationale is that "a lot of incentive to be negative is removed after you hit the loading screen". — [Riot via devtrackers.gg](https://devtrackers.gg/leagueoflegends/p/749a443a-apps-that-reveal-player-names-or-stats-in-champ-select-need-to-remove-these-informations-starting-patch-12-22)

### Inferences
- **Feasible for a small, free, non-commercial PreBot.** The legitimate route is a production key, which requires a registered product and a working prototype, with no guaranteed timeline. Using a personal key for PreBot would breach "You may not run your application for public consumption using a personal key", because PreBot serves the public TR Flex player base, not a "small private community". A dev key is worse still, since it expires every 24 h and is explicitly banned for public products.
- **Rate-limit budget.** The cheapest pattern is to check Spectator *on demand*, only for the candidates about to be notified. That is about 30 calls per listing (5 + 10 + 15), plus a few re-checks.
  - Production key (about 50 requests per second sustained): fine.
  - Personal key (100 per 2 min, about 0.83 per second): roughly 3 listings per 2 minutes. Technically workable for a tiny pilot, but not policy-compliant for a public app.
  - Continuous polling of every "available" user, for example every 2 minutes, costs N/120 requests per second. 300 available users is about 2.5 requests per second, fine on production but over the personal limit.
  - Per-method limits for Spectator-V5 were not found and could be lower than the app limit.
- **Using `gameLength`.** It makes "notify after match" smarter. A player at 25:00 elapsed will likely finish within about 3–8 minutes and is still useful for a 15-minute listing. A player at 3:00 is about 27 minutes away and should be skipped or put on an after-match list. This is an inference from the field definition plus the duration data in section 1.
- **Blind spot.** Spectator cannot see lobby, queue, champ select or loading-screen states (it covers only an active game; Riot itself frames the loading screen as the boundary). So "in champ select" will look "not in game". This matters because a champ-select player is effectively unavailable for about 35 more minutes.
- **Fragility.** Riot switched the endpoint off without notice in October 2025 and only reversed course days later. Any PreBot feature built on Spectator needs a fallback, such as manual status or the Match-V5 approach in section 4.
- **Streamer mode risk.** Lookups for players with streamer mode on may fail or return a null PUUID, so PreBot could treat them as "not in game". Whether a lookup *by the user's own PUUID* still works under "Hide My Name" is unverified (see Gaps). Querying a user's live game also should not expose that user to *others* beyond what they opted into, because of the "cannot de-anonymize" policy.

### Gaps
- Spectator-V5 per-method rate limits were not found.
- Whether `active-games/by-summoner/{puuid}` still returns data for a player using "Hide My Name / Hide My Everything" (since Oct 2025) is unknown. Riot's statement that results "respect streamer mode settings" does not specify the behavior. This needs an empirical test with a dev key.
- No data on actual production-key approval times (days or weeks) was found. Reddit and the Riot developer Discord were not reachable or not searched successfully. Riot gives no SLA.
- The current developer policy page for League on the Riot support site ([support-developer.riotgames.com](https://support-developer.riotgames.com/hc/en-us/articles/22698698001939-League-of-Legends)) returned 403. Any League-specific rules there on live-game use or notifications could not be read.
- The exact date and status of the replay endpoint RSO change (23 Oct 2026?) is unverified.

---

## 4. Riot Sign On (RSO) for account linking, and detecting match end via Match-V5

### Takeaway
RSO is available only to production-key holders and needs a separate onboarding initiated by Riot. A small app cannot simply turn it on. Spectator and Match lookups do not need RSO, only the player's PUUID. Match end can be detected in two ways: Spectator going from 200 to 404, or a new match ID appearing in the player's Match-V5 history. The Match-V5 delay after game end was not found.

### Cited Findings
- RSO: "This access is only available to developers with Production Level API Keys." If approved, Riot contacts the developer through portal messaging to start RSO integration. After login, the RSO client calls `/riot/account/v1/accounts/me` on the americas, europe or asia cluster to identify the user. The supported auth methods are Client Secret Basic and Private Key JWT. — [Riot Developer Docs – League of Legends](https://developer.riotgames.com/docs/lol)
- PUUIDs are key-specific: a PUUID obtained with a dev key won't work with a production key. — search summary of [hextechdocs / Riot API getting started](https://hextechdocs.dev/getting-started-with-the-riot-games-api/)
- **Polling pattern.** Score bots treat a status change from "running" to "finished" as the end-of-match trigger, for example PandaScore's Discord score-bot polling `/matches`. — search summary of [PandaScore Discord score-bot docs](https://developers.pandascore.co/docs/discord-score-bot)
- **League bots announcing results after games** are an established pattern, for example FeedWatch, which posts results right after a game ends, and an open-source League team bot. — search summary of [FeedWatch](https://discordbotlist.com/bots/feedwatch); [league-of-legends-team-discord-bot](https://github.com/PatricNox/league-of-legends-team-discord-bot)
- The League Client API is "not officially supported for use with third party applications". — [Riot Developer Docs – League of Legends](https://developer.riotgames.com/docs/lol) (relevant to section 6)

### Inferences
- **Without RSO, PreBot can still link a Riot ID.** It can resolve the Riot ID to a PUUID (account-v1 by-riot-id, a standard endpoint not re-verified in this session). It can then verify ownership with a common pattern, such as asking the user to set a specific summoner icon temporarily. Calling Riot's RSO a requirement for PreBot is wrong. It is a production-tier extra.
- **Detecting match end:**
  - (a) Spectator transition: 200, then 404. This needs polling roughly every 1–2 minutes during the expected end window, which `gameLength` makes predictable.
  - (b) A new ID at the top of Match-V5 `matches/by-puuid/{puuid}/ids`. This is likely slower, because the match record usually appears some time after the game ends (delay unverified).

  Option (a) is better for a quick "your match ended, the group is still waiting" push.
- **The October 2025 replay and RSO direction** suggests Riot is moving more per-player data behind RSO. PreBot should not assume Spectator access stays RSO-free indefinitely.

### Gaps
- No source was found on the typical delay between game end and the match appearing in Match-V5.
- No source was found on whether Riot grants RSO to small, free, non-commercial projects in practice, or how long that takes.

---

## 5. Discord: can a bot see "Playing League of Legends", and does League provide in-lobby / champ-select / in-game Rich Presence in 2025–2026?

### Takeaway
A Discord bot can read a member's activities, including "Playing {game}" plus `state`, `details` and `timestamps`, from `PRESENCE_UPDATE`. It needs the privileged `GUILD_PRESENCES` intent, and it only sees members of servers it shares with them. Below 100 servers the intent is a dashboard toggle; verified apps (100+ servers) need approval. This works only if users join a PreBot Discord server and share their activity. League's native Discord presence exists, but its exact state strings could not be verified from an official source. Riot's official Discord integration, live in beta since April 2026, is friend-to-friend for US, Canada and Brazil, not a bot API. TR was not included in any source found.

### Cited Findings
- `GUILD_PRESENCES (1 << 8)` covers `PRESENCE_UPDATE`. It is a privileged intent that must be toggled in app settings, and it is "turned off by default on all API versions". "For verified apps (required for apps in 100+ guilds), the intent must also be approved." The same page also states "Apps with fewer than 10,000 users can access privileged intents by enabling them in the Developer Portal". This user-count threshold is a separate or newer criterion and is worth re-checking. A privileged intent that is not enabled or approved closes the gateway with code 4014. — [Discord Docs – Gateway](https://docs.discord.com/developers/events/gateway)
- **Presence Update payload.** It contains `activities` ("User's current activities"), `client_status` and `status`. An activity has:
  - `name`
  - `type` (0 = Playing, shown as "Playing {name}")
  - `state` ("User's current party status")
  - `details` ("What the player is currently doing")
  - `timestamps` ("Unix timestamps for start and/or end of the game")
  - `application_id`

  "You *must* specify the `GUILD_PRESENCES` intent in order to receive Presence Update events." Without it, presences "will only contain your bot and users in voice channels". — [Discord Docs – Gateway Events](https://docs.discord.com/developers/events/gateway-events)
- **League's native presence.** The open-source League RPC project replaces it and notes that "in order for league's native rich presence to be disabled" League RPC must launch the game, which confirms a native rich presence exists. League RPC itself shows separate states for "client, lobby, custom game, queue, champ select, in game, TFT and spectating", read from the local League client. — [Its-Haze/league-rpc (GitHub)](https://github.com/Its-Haze/league-rpc)
- A Discord support community thread complains that League's Rich Presence uses old splash art and has no images for champions after Samira. This implies the native presence shows champion art in game; details were unverified because the page returned 403. — search summary of [Discord support community post](https://support.discord.com/hc/ko/community/posts/1500001361022-Update-League-of-Legends-Rich-Presence)
- **Official Riot × Discord integration.** It shipped with patch 26.8 (14 Apr 2026) as a beta "across US, Canada and Brazil starting on April 16th". It lets players invite Discord friends to a party, share party links and "see which of your friends play other Riot games". Riot plans "expanding to additional regions in a few patches". — [Riot, Patch 26.8 notes](https://www.leagueoflegends.com/en-gb/news/game-updates/league-of-legends-patch-26-8-notes/)
- A secondary outlet (7 May 2026) says linked friends show whether they are "in queue, in champ select, or in a game", and that your League status appears on your Discord profile. Linking uses OAuth 2.0 and works from Discord Settings > Connections, the Riot account site or the League client. Regions are US/CA/BR, with "other regions over the coming months". Turkey is not mentioned. — [esportnow.gg](https://esportnow.gg/lol/news/league-of-legends-discord-integration-beta)
- **Conflicting dates.** A Discord blog post gives rollout dates of Brazil 7 May and US/Canada 20 May. That conflicts with Riot's 16 April date and may refer to a different (VALORANT, earlier) rollout. — search summary of [Discord blog](https://discord.com/blog/making-it-easier-than-ever-to-connect-with-friends-in-league-val)
- No source confirming a Europe or Turkey rollout as of the searches was found. — search over [Fragster](https://www.fragster.com/league-of-legends-patch-26-8-mel-and-dr-mundo-nerfed-discord-integration-launches-in-beta/), [exitlag](https://www.exitlag.com/news/league-of-legends-patch-26-8/) and others
- **Presence-based LFG bots already exist.** LFGScout monitors and notifies when guild members start playing a specific game, using Discord activity data. It detects game start but not match end. — search summary of [LFGScout](https://github.imc.re/DJStompZone/LFGScout)

### Inferences
- **Feasible for PreBot today.** If PreBot runs or uses a Discord server that its users join (it currently uses DMs only), a bot with `GUILD_PRESENCES` can see "Playing League of Legends". It can also see the native presence's `details`/`state` text, if League populates it with phases such as champ select or in game. While PreBot is in fewer than 100 servers this requires no verification. It is easy to build, but coverage depends on users joining the server, enabling Discord's activity sharing and running the Discord desktop app.
- **"Playing League of Legends" alone is too coarse.** It is true whenever the client is open, including idle in lobby. Only the `details`/`state` strings or timestamps distinguish lobby from game, and their format is undocumented and can change without notice.
- **Riot's 2026 integration** helps humans (friends see each other's status) but gives a third-party bot no structured "in game" signal beyond normal Discord presence. It is also not confirmed for TR.

### Gaps
- The exact strings League's native Discord Rich Presence publishes in 2026 (for example "In Lobby", "In Queue", "Champion Select", "In Game" with timer) could not be verified from an official source. Testing it directly would settle this: watch `PRESENCE_UPDATE` payloads on a test server while cycling through lobby, queue, champ select and game.
- Whether the Riot × Discord integration has reached EU or TR by November 2026 is unknown.
- How the "fewer than 10,000 users" rule interacts with the "100+ guilds verification" rule for privileged intents could not be confirmed from a dated Discord announcement.

---

## 6. Local options: League Client (LCU) API

### Takeaway
The LCU API is a local, unsupported interface on the player's own PC. A website or PWA cannot legitimately read it. Only a locally installed companion app can, as League RPC, Blitz and Porofessor do. Riot asks for such apps to be registered.

### Cited Findings
- On the League Client API, Riot says: "This service is not officially supported for use with third party applications" and gives "no guarantees of full documentation, service uptime, or change communication". Developers should register the app or note their LCU endpoint use in the Developer Portal. — [Riot Developer Docs – League of Legends](https://developer.riotgames.com/docs/lol)
- General policy: products using the League Client API must follow its documentation and register through the Developer Portal. — [Riot Developer General Policies](https://developer.riotgames.com/policies/general)
- League RPC (2026) reads the local League client (via `lcu-driver` / `lcu-gopher`) to show client, lobby, queue, champ select and in-game states. This shows phase detection is technically possible from a local app. — [Its-Haze/league-rpc](https://github.com/Its-Haze/league-rpc)

### Inferences
- A browser page cannot read the local client. The LCU runs on localhost with a per-session password from the client's lockfile and a self-signed certificate, so a website cannot reach it (general technical knowledge, not re-verified here).
- A small desktop companion app (or a fork of an existing RPC tool) could report the exact phase (lobby, queue, champ select, in progress, end of game) to PreBot's server. It would need LCU registration, installation and trust, and every user would have to run it. That is heavy for a small free app.
- The champ-select policy (no showing *other players'* names or stats in champ select) would not block reporting a user's *own* phase, but this would need a policy check.

### Gaps
- No Riot statement was found on whether a companion app that relays a user's own client phase to a website is explicitly allowed.

---

## 7. Design patterns from other games and apps: "notify after match", queue-pop style alerts, in-game status in LFG tools

### Takeaway
Established patterns are presence-triggered alerts ("X started playing"), polling for state changes that fire on "running to finished", post-game result bots, and queue-pop notifiers. Riot's own 2026 Discord integration builds invites around live status (in queue, champ select, in game) plus a one-click Join. No League premade finder found in this research combines Spectator checks with "notify after match". PreBot would be early but not exotic.

### Cited Findings
- **Presence-triggered alerts:** LFGScout notifies a guild when members start playing a specific game. — search summary of [LFGScout](https://github.imc.re/DJStompZone/LFGScout)
- **State-change polling:** PandaScore's score-bot polls match status and fires on running to finished. — search summary of [PandaScore docs](https://developers.pandascore.co/docs/discord-score-bot)
- **Post-game notifications:** FeedWatch posts match results immediately after a game ends. — search summary of [FeedWatch](https://discordbotlist.com/bots/feedwatch)
- **Queue-end alerts:** An Overwatch community tool notifies via a Discord bot when the queue pops or ends. — search summary of [over.gg queue notification tool](https://www.over.gg/12944/simple-queue-notification-tool)
- **Status-aware invites (Riot 2026):** invites are sent as Discord messages with live status and a Join button. Open parties can be joined by link; closed parties show "Ask to Join". — [esportnow.gg](https://esportnow.gg/lol/news/league-of-legends-discord-integration-beta); feature list confirmed in [Riot Patch 26.8 notes](https://www.leagueoflegends.com/en-gb/news/game-updates/league-of-legends-patch-26-8-notes/)
- **Flex context:** Riot says Flex struggles because there are "not enough 1s and 2s" to fill premades (2022). A 4-stack looking for one solo player is exactly the scarce resource PreBot is trying to reach. — [Riot, State of Competitive Spring 2022](https://www.leagueoflegends.com/en-au/news/dev/the-state-of-competitive-spring-2022/)

### Inferences
These are design options for PreBot, all derived from the findings above.

1. **Three-state self-report, at no API cost.** "Ready now (lobby)", "In a game – notify me after" and "Available later". The in-game state auto-expires after about 35–40 minutes, or after a Spectator 404 if a key is available. This works for every TR user regardless of key tier or Discord setup.
2. **"I'm in game, ping me after" action on the push or DM itself.** This turns a missed notification into a registration on an after-match list, rather than a silent non-response.
3. **Spectator check at send time** (production key only). Before each wave, look up candidates:
   - Not in game: notify now.
   - In game with `gameLength` ≥ about 20–22 min: notify with an ETA, because they may finish inside the listing window.
   - In game and early: skip, and add to the after-match list with a re-check near the predicted end.

   This also partly fixes wave 3, by filling it with idle players first.
4. **Revisit the listing lifecycle** rather than only the notification. The 15-minute lifetime with a last wave at 6 minutes leaves wave-3 recipients about 9 minutes. Options:
   - Front-load the waves by reachability, so idle players come first.
   - Allow a "standby/extend" step where the group can wait about 10 more minutes for a named after-match candidate.
   - Send a "your match ended – this group is still open" push only if the listing is still unfilled.
5. **Optional Discord-presence signal** for users who join a PreBot server. Treat "Playing League of Legends" plus `details` and timestamps as a hint, never as ground truth.
6. **Instrument before building.** Log, per notification, time-to-open, time-to-response and outcome. That shows whether non-responders cluster at about 15–30 minutes, as the in-game hypothesis predicts, or simply never respond (low intent rather than bad timing).

### Gaps
- No LoL-specific LFG or premade-finder product that shows live in-game status or "notify after match" was found in the searches, so there are no outcome data (response-rate uplift) from comparable products.
- No published data on push-notification response latency for game-related LFG apps was found.
