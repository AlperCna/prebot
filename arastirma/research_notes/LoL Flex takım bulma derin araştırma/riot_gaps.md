# Riot / LoL open gaps for a TR1 Flex team-finding Discord bot (research as of 2026-11-08)

Scope note: the coordinator later narrowed the product to an on-demand "ad board": a group that is short one or more players (e.g. 4 people need a 5th for Flex right now) posts it and finds someone fast. These notes cover that, plus "available now" signals from the Riot API, how fast a newly linked user's Flex rank and main role can be fetched, and whether Discord.Net supports the real-time features needed.

## Q1. "Ranked 5s" in 2026: official announcement, rules, schedule, TR availability, results, permanence

### Takeaway
This one is confirmed from primary sources. Riot announced Ranked 5s as an experimental weekend-only queue on 27 May 2026, and Turkey is explicitly included. It needs a full five-player premade but no fixed roster, has no rank restriction, and uses Tournament Draft. Experiment 1 ran 26 Jun–6 Sep 2026 and Experiment 2 runs 11 Sep 2026–7 Jan 2027. Riot has published no results data, and Riot says the format "isn't set in stone". Because the queue only accepts complete 5-stacks, a "find the 5th player now" bot fits it very well on TR weekend evenings.

### Cited Findings
- **Official dev blog**, "/dev: The Return of Ranked 5s", 27 May 2026, author "Riot Usury":
  - It is a Summoner's Rift queue for premade groups of five with no fixed rosters ("queue with any group of friends").
  - There is no rank restriction. Riot says it built mechanisms to offset large skill gaps within a team.
  - Draft is Tournament Draft (3 bans, 3 picks, 2 bans, 2 picks).
  - Schedule is weekends only, which the fetched page rendered as 8:00 PM–1:00 AM local time.
  - Regions listed: NA, EUW, EUNE, LAN, LAS, BR, OCE, SEA, Middle East, RU, KR, JP, TW, VN and **Turkey**.
  - Riot's quotes: "This isn't set in stone"; "When you get 5 people working together to achieve a shared goal, it's peak"; "the joy of playing with your friends is more important than tightly-constrained skill evaluation".
  - Riot will apply the smurf and boost detections it previously used in Flex, and will "monitor games to see how things pan out".
  - The blog describes no team-finding tool. Players are expected to bring their own friends.
  - [Riot dev blog](https://www.leagueoflegends.com/en-us/news/dev/dev-the-return-of-ranked-5s/)
- **Official TL;DW of the same Dev Update** (27 May 2026) calls it "experimenting with a new Ranked queue for full five-stacks". It says the queue runs for a few weeks in windowed time slots so that equally skilled teams queue at the same time. The page gives no results figures. — [Riot TL;DW Dev Update](https://www.leagueoflegends.com/en-us/news/dev/tldw-mayhem-ranked-5s-more-dev-update/)
- **Official support article "Ranked 5s Mode"**:
  - Dates: Experiment 1 ran 26 Jun 2026–6 Sep 2026. Experiment 2 runs 11 Sep 2026–7 Jan 2027.
  - Hours: "only available on weekends (Friday to Sunday), from 4pm until 4am (server time)". TR is listed at 4pm–4am TRT.
  - Party: "You'll need a full team to enter the game queue". Solo queuing is not allowed. Players of any rank, Unranked to Challenger, can join a team.
  - Placements: 5 placement matches, seeded from MMR in other modes. After that the mode uses its own MMR and LP ladder.
  - Separate from Solo/Flex: its own season overlaps the Solo/Flex seasons. Games do not count toward Solo/Flex ranked objectives or the Victorious skin, but do count toward Battle Pass XP and objectives. There is no decay.
  - Rewards: the 2026 Ranked 5s Icon for reaching any tier, a Banner for Gold or higher, and an Experiment 2 participation emote whose criteria are "still to be defined".
  - Experiment 2 adds a scouting tooltip during pick and ban.
  - Experiment 1 rewards drop from Patch 26.18 (10 Sep 2026), expected to complete by Patch 26.19.
  - The fetched page showed "last updated June 19, 2026", even though it contains Experiment 2 details, so the displayed date may be stale.
  - [Riot Support: Ranked 5s](https://support.riotgames.com/en-us/league-of-legends/events/ranked-5s)
- **The hours conflict between sources** (all secondary or differently dated):
  - EsportNow (28 May 2026) says 9:00 PM–1:00 AM local time in most regions. — [EsportNow](https://esportnow.gg/lol/news/ranked-5s-returns)
  - GameDaily also says 9 PM–1 AM. — [GameDaily via search snippet](https://gamedaily.com/games/league-of-legends-ranked-5s-return-weekend-queue-tournament-draft)
  - A secondary guide says the September update extended the window with an earlier start and later end each weekend. — [ZaplatGaming](https://zaplatgaming.com/articles/league-of-legends-ranked-5s-second-run-2026-guide?lang=en)
  - The current support page says 4pm–4am. That is consistent with the window having been widened for Experiment 2.
- **Why the original Ranked 5s was sunset** (secondary): matchmaking could not consistently find similar-skill teams, and long queues (around 20 minutes) drove players away. — [EsportNow](https://esportnow.gg/lol/news/ranked-5s-returns)

### Inferences
- Ranked 5s on TR runs Friday–Sunday 16:00–04:00 TRT until 7 Jan 2027, per the current support page. During those windows, demand to "find a 5th player now" should peak, because the queue cannot start without five people. The bot could offer a "Ranked 5s" post type alongside Flex.
- An Experiment 2 following Experiment 1 means Riot judged the first run worth repeating. Riot has not said it will become permanent.
- Riot shows no sign of building an in-client team finder for Ranked 5s. "Text the squad" puts group formation on players, which leaves room for third-party tools.

### Gaps
- No official numbers on Experiment 1 participation, queue times or reception were found, and no TR-specific data either. Riot's September 2026 dev update page itself did not appear in search results, so details about the September changes are secondary.
- No decision on what happens after 7 Jan 2027 was found.
- The official sources disagree on hours: 8–1 local in the May blog versus 4pm–4am in the support page. The likely explanation is that Experiment 2 widened the window, but no Riot primary source confirms that.

## Q2. Clash in 2026: schedule, TR1 availability, in-client team formation

### Takeaway
I found no official 2026 Clash schedule from Riot (leagueoflegends.com or support) in this pass, so everything below is secondary and inconsistent. Third-party guides describe a roughly monthly cadence that alternates Summoner's Rift and ARAM cups, but they disagree on dates.

### Cited Findings
- **(SECONDARY)** Demacia Cup (Summoner's Rift): team formation opens 12 Jan 2026, tournament 17–18 Jan. Other cups listed: ARAM Ionia Cup 21–22 Feb, Noxus Cup 21–22 Mar, ARAM Ixtal Cup 18–19 Apr. This source says it draws on Riot support pages, but I could not confirm that. — [lolnow.gg](https://lolnow.gg/lol-clash-schedule/)
- **(SECONDARY)** Another guide gives the Demacia Cup as 24–25 Jan, with registration opening 19 Jan. — [search summary of 1v9.gg / turbosmurfs](https://1v9.gg/blog/league-of-legends-lol-clash-schedule)
- **(SECONDARY, low reliability)** Guides disagree on late-2026 Clash:
  - One claims an SR Worlds Clash on 21–22 Nov 2026, with team formation from 16 Nov. — [AgataSmurf](https://agatasmurf.com/lol-clash-schedule/)
  - Another says the MSI Cup on 18–19 Jul was the last announced Clash. — [search summary, lolnow.gg](https://lolnow.gg/lol-clash-schedule/)
- The "Demacia Cup Global Invitational" (Oct 2026, hosted by TJ Sports and Huya) is an esports event, not the Clash cup. Don't confuse the two. — [Insider Gaming](https://insider-gaming.com/2026-demacia-cup-global-invitational-details/)
- The latest official Riot Clash schedule post found is old (Split 1 2023). — [Riot 2023 Clash schedule](https://www.leagueoflegends.com/en-us/news/announcements/league-of-legends-split-1-2023-clash-schedule/)

### Inferences
- Clash is a second periodic 5-stack use case, alongside Ranked 5s. A bot should pull dates from the client or official posts rather than hardcode them.

### Gaps
- No official 2026 Clash schedule was found.
- No confirmation was found that Clash runs on TR1 in 2026.
- No information was found on in-client Clash team-finder features in 2026. My background knowledge is that Clash historically had a "find team / invite" flow but no matchmaking for solo players. I did not verify this.

## Q3. Riot in-client social roadmap 2025–2026 (party finder / LFG for LoL PC)

### Takeaway
I found no Riot announcement of a party finder, LFG or "find a team" feature for League on PC in 2025–2026. The official 2026 social changes I found concern behavior and safety, such as reporting DMs. Riot's other game Wild Rift does have an official Party Finder, which shows Riot has built the concept, but only for mobile.

### Cited Findings
- **Official Dev Update TL;DW**, 27 May 2026:
  - "Starting this Act" players can report abusive communication in their DMs.
  - A new system will make it easier to track which features and restrictions a player has access to (Honor rewards, chat and queue restrictions).
  - The update does not mention a party finder, Clash or Flex.
  - [Riot TL;DW](https://www.leagueoflegends.com/en-us/news/dev/tldw-mayhem-ranked-5s-more-dev-update/)
- **Official:** Wild Rift has a "Party Finder" for joining or hosting parties. This is the mobile game, not LoL PC. — [Riot Support: Wild Rift Party Finder](https://support.riotgames.com/en-us/wild-rift/gameplay/party-finder)
- **Official:** The Ranked 5s dev blog assumes players bring their own five, and no Riot-side finder is described. — [Riot dev blog](https://www.leagueoflegends.com/en-us/news/dev/dev-the-return-of-ranked-5s/)
- **(SECONDARY)** Community LFG for LoL is mostly on the official LoL Discord, Tracker.gg and OP.gg LFG sections. — [ExitLag blog](https://www.exitlag.com/blog/lfg-league-of-legends/)
- **(SECONDARY, undated)** A Dot Esports item mentions a "looking for friends" client feature. The date could not be confirmed. — [Dot Esports](https://dotesports.com/?p=441849)

### Inferences
- As of Nov 2026 I found no evidence of a native LoL PC LFG that would make a Discord "ad board" redundant. Watch for one, though: Riot has the Wild Rift precedent, and Ranked 5s gives it a motive.

### Gaps
- No Riot dev-blog quotes on a social-panel or LFG roadmap for LoL PC were found.
- No quotes from Riot social or client roadmap posts (Riot Client social) were found.

## Q4. Riot Developer API Terms and General Policies: retention, attribution, deletion, messaging

### Takeaway
The API Terms set no explicit caching or retention period. They do require two things:
1. Act on end-user deletion requests that Riot passes along as lists of identifiers.
2. Delete all Game Information when the terms are terminated.

One clause matters directly for this bot: the Terms prohibit "sending messages to or communicating with … other users of the Game through Game Information obtained from the Riot Games API". The bot must therefore contact only Discord users who opted in themselves, never players it discovered through the API. The General Policies require a visible "isn't endorsed by Riot Games" notice and ban products that de-anonymize players or create alternatives to the ranked ladder.

### Cited Findings
- **API Terms (developer.riotgames.com/terms)**. The fetched page shows "LAST UPDATED: DECEMBER 9, 2013", although it includes a GDPR section, so it has evidently been amended since.
  - **Deletion (GDPR section):** "When Riot Games receives a request from an end user to delete their personal data, we will be passing the request to all active developers by sharing a list of identifiers (e.g. accountId) for the end users through Riot Channels." No time limit is stated.
  - **Prohibited:** "Sending messages to or communicating with (or attempting to do the foregoing) other users of the Game through Game Information obtained from the Riot Games API."
  - **Prohibited:** collecting any additional information about the Game or its users that the API does not provide or permit.
  - **Prohibited:** giving third parties access to the API.
  - **Termination:** "…cease using the Materials, including without limitation any Game Information in Your possession, as well as delete all of the Game Information in Your possession."
  - **Caching and retention:** there is no clause on caching, retention periods or storing match history.
  - **Key approval:** Riot may "in its sole discretion" register an Application, and "reserves the right to delay the provision or activation of any Production Key".
  - **Development key limit:** the Terms state 10 calls per 10 seconds. That figure is outdated compared with current portal limits.
  - [Riot API Terms](https://developer.riotgames.com/terms)
- **General Policies**:
  - **Required notice:** "[Your product] isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games…", placed "in a location that is readily visible to players".
  - **Riot logos and trademarks:** use only "where such use is inevitable".
  - **De-anonymization:** "Products cannot de-anonymize players who cannot reasonably be identified from visible information."
  - **Ranked ladder:** "Products cannot create alternatives for official skill ranking systems such as the ranked ladder."
  - **Registration and audit:** all products must be registered and audited in the Developer Portal. "Any new features or changes to a product must be audited."
  - **Keys:** one product per production key, and the key must not be embedded in code.
  - [Riot General Policies](https://developer.riotgames.com/policies/general)
- **Developer Portal docs:** the trademark line is "League of Legends and Riot Games are trademarks, service marks, and registered trademarks of Riot Games, Inc." The docs give no caching guidance. — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)

### Inferences
- **Storage:** storing a user's linked PUUID, rank snapshot and recent-role summary looks permissible, since there is no retention cap. Even so, I'd recommend:
  - a deletion path when a user unlinks;
  - a process for Riot-forwarded deletion lists keyed by account or PUUID;
  - a short TTL, refreshing rank on demand instead of building a long-term match archive.
- **Pings and DMs:** pinging or DMing Discord members who registered with the bot and opted into LFG alerts uses Discord data and opt-in, not "Game Information obtained from the API" used to reach game users. Still, keep the wording and flows clearly opt-in. The bot must not message players found through API lookups, such as recent teammates from match history.
- **Seriousness or behavior labels:** a Flex "MMR estimate" or a bot-computed "skill score" risks the "alternatives to the ranked ladder" clause, and the MMR/ELO-calculator ban was already established. Show the official tier/LP only.

### Gaps
- The Terms page has no exact Attribution section text. The Survival clause references one, but no body was shown.
- The developer.riotgames.com/policies/lol page returned 404 on 2026-11-08, so the game-specific policies could not be checked.
- The Legal Jibber Jabber / fan content policy was not re-fetched in this pass. It concerns fan content and IP use, not the API.

## Q5. Production key approvals for Discord bots in practice

### Takeaway
Riot requires a working, near-complete product. For a Discord bot, that includes a website describing the bot with Terms of Service and a Privacy Policy. A GitHub repo is not accepted as a substitute. Developers report long waits:
- Riot's own wording: "can vary".
- One 2026 community blog: about 5–8 months.
- GitHub issues: pending 38 days, about 6 months, and 7+ months.

The official docs say personal keys may serve "a small private community", including "voice com servers". A personal key is therefore a legitimate starting point for a single TR Discord server, and the realistic path while a production application is pending.

### Cited Findings
- **Riot support article "Production Key Applications"** (via search snippet; direct fetch returned 403):
  - Riot cannot accept GitHub repositories or source code in place of a functioning application or site.
  - Discord bots still need a website showing what the product does, where to access it, and where the ToS and Privacy Policy are.
  - Production keys are for completed or near-complete apps.
  - [Riot DevRel support](https://support-developer.riotgames.com/hc/en-us/articles/22801383038867-Production-Key-Applications)
- **Developer Portal docs:**
  - **Personal key scope:** personal keys are for "products that are intended for just the developer or a small private community". Listed uses include bots for "streaming sites, boards, voice com servers, etc.".
  - **Personal key restrictions:** personal keys "won't be approved for rate limit increases", give no Tournaments API access, and can't run public apps, including open alpha or beta.
  - **Production keys:** "Typically requires a working prototype".
  - **Review criteria:** DevRel favors products that help players improve, and incomplete websites are unlikely to be approved.
  - [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Rejection example:** GitHub issue #1139. The rejection said the link "does not work or leads to a blank page". The applicant disputed this and noted there is no way to reply in the portal's Project Discussion tab. — [GitHub #1139](https://github.com/riotgames/developer-relations/issues/1139)
- **Wait-time examples** (GitHub issue titles):
  - "Pending Review 38 days (App ID: 875612, TILT)" — [#1197](https://github.com/RiotGames/developer-relations/issues/1197)
  - "Pending Review ~6 months (App ID: 835226, LOL Guided Review)" — [#1207](https://github.com/RiotGames/developer-relations/issues/1207)
  - "Pending Review for 7+ months, 3 support tickets unanswered (App ID: 798301)" — [#1192](https://github.com/RiotGames/developer-relations/issues/1192)
  - Pending follow-up (lolpt.gg, submitted Dec 2025) — [#1127](https://github.com/riotgames/developer-relations/issues/1127)
  - Portal error "Selected app type is not available" — [#1194](https://github.com/RiotGames/developer-relations/issues/1194)
- **(SECONDARY)** DarkIntaqt blog, 19 Aug 2026:
  - Production keys take "around 5 to 8 months" depending on the game, and need a valid domain.
  - Apps for "personal research" are usually not accepted.
  - The GitHub repo is "barely monitored by Rioters".
  - Each key has its own encryption, so PUUIDs differ between personal and production keys. When upgrading, ask Riot to link the encryption to the personal app ID.
  - [DarkIntaqt](https://darkintaqt.com/blog/riot-app-application)
- **(SECONDARY)** Hextechdocs gives older estimates of about 10 business days, and elsewhere about 20 business days. — [hextechdocs](https://hextechdocs.dev/getting-started-with-the-riot-games-api/)

### Inferences
- **Plan:** launch on one TR community server with a personal key (this matches "small private community / voice com servers"). Meanwhile, build the website with ToS and Privacy Policy and apply for production when going multi-server or public.
- **Encryption across keys:** encrypted PUUIDs are specific to each key, so store data in a way that survives re-encryption. Store Riot ID plus PUUID, and plan a re-resolve migration, or ask Riot to link the apps.
- **LFG eligibility:** no source states that LFG or team-finding is disallowed. It is not mentioned either way.

### Gaps
- No top.gg LoL bot advertising a "Riot-approved production key" was verified.
- No Reddit threads with first-hand Discord-bot approval stories were found in this pass.
- No official list of rejection reasons was found beyond the website and completeness requirements.

## Q6. Profile-icon verification: open-source implementations and DevRel comment

### Takeaway
The icon-change ownership check is a widely used pattern. Challengermode's support docs document it: temporarily set a specified summoner icon, and the system checks it. This pass did not surface a concrete GitHub Discord-bot implementation that verifies through `profileIconId`, and I found no Riot DevRel statement on it.

### Cited Findings
- **(SECONDARY, third-party platform)** Challengermode verifies a LoL account by having the user temporarily change their Summoner Icon. The system checks it, and the user can switch back afterward. — [Challengermode support](https://support.challengermode.com/en/game-specific/how-to-verify-a-summoner-1)
- **RTBGG/lol-ranks-bot (GitHub, Node.js):**
  - It "verifies" League accounts and assigns rank roles, keeping them updated periodically.
  - The README does not state the ownership mechanism and refers to summoner names and a "Riot development API key" (`riotToken`).
  - [GitHub RTBGG/lol-ranks-bot](https://github.com/RTBGG/lol-ranks-bot)
- **Developer Portal docs** say nothing about profile-icon verification or third-party code. — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)

### Inferences
- The icon is readable through Summoner-V4 by PUUID (`profileIconId`), which makes the icon check feasible without RSO:
  1. Ask the user to equip one of the default icons, ID 0–28, which every account owns.
  2. Poll once or twice.
  3. Confirm the match.
- RSO remains the only Riot-sanctioned ownership proof, and it requires production approval (already established).

### Gaps
- No GitHub code search was possible with these tools. Run a direct GitHub code search for "profileIconId" with "verify" to find implementations.
- No DevRel comment on icon verification was found.

## Q7. Riot policy on rating, reviewing, "reputation" or reporting players in third-party apps

### Takeaway
I found no explicit Riot rule about player rating, reputation tags or reporting in third-party apps. The relevant constraints are general:
- no de-anonymizing players;
- no alternatives to the ranked ladder;
- no using API data to message game users;
- no harassment or defamation.

A teammate-rating feature would also need DevRel audit as a "new feature".

### Cited Findings
- **General Policies:**
  - "Products cannot de-anonymize players who cannot reasonably be identified from visible information."
  - "Products cannot create alternatives for official skill ranking systems such as the ranked ladder."
  - New features must be audited through the Developer Portal.
  - [Riot General Policies](https://developer.riotgames.com/policies/general)
- **API Terms:** prohibited uses include harassing or defaming others and violating privacy (paraphrased from the restrictions list), and messaging Game users through API Game Information. — [Riot API Terms](https://developer.riotgames.com/terms)
- **Riot's own social direction:** Riot itself is adding DM reporting and a restriction-tracking system (27 May 2026), which keeps behavior enforcement in-client. — [Riot TL;DW](https://www.leagueoflegends.com/en-us/news/dev/tldw-mayhem-ranked-5s-more-dev-update/)

### Inferences
- A bot-internal, Discord-scoped "no-show / reliability" flag would be the lower-risk design:
  - it is moderated by server admins, not public;
  - it is keyed to Discord users, not Riot IDs;
  - it is not shown as a skill rating.
- Public per-Riot-ID reputation tags, like Porofessor-style tags, would carry higher risk. They would also need explicit disclosure in the production application.

### Gaps
- No DevRel statement specifically on Porofessor or LeagueOfGraphs-style tags, or on "rate your teammates" apps, was found.

## Q8. Riot API support for "available now", and speed of fetching a new user's Flex rank and role

### Takeaway
The Riot API has no presence or online-status endpoint for third parties, so "available" must come from the user, for example via a button or status in Discord. Spectator-V5 by PUUID can only tell you whether someone is currently in a game: it returns 404 when they are not, and is occasionally wrong. That is useful to auto-expire a "looking" post or mark someone "in game", not to detect "online".

Onboarding a new user takes about 3 calls for identity and rank, plus 1+N calls for role from recent Flex matches. With a personal key (20/1s, 100/2min per region), that is a few seconds per user. Batch spikes are limited by the 100-per-2-minute budget, mostly on the EUROPE regional cluster where Match-V5 lives.

### Cited Findings
- **Spectator-V5:**
  - Endpoint: GET `/lol/spectator/v5/active-games/by-summoner/{encryptedPUUID}` on the platform host (TR1). It returns the current game, or 404 if the player is not in a game. Sources: a third-party doc mirror and the riven client library metadata. — [Glama mirror of Riot docs](https://glama.ai/mcp/servers/@chirpon9/riot-docs-mcp/blob/bd23bbcbe155efed4c6774a7dde0bb8020d76191/src/riot_docs/spectator-v5.md); [riven meta.rs](https://docs.rs/crate/riven/latest/source/src/meta.rs)
  - A developer-relations bug report describes Spectator V5 returning 404 while a player was in a game (Arena). — [search summary referencing RiotGames/developer-relations issues](https://github-redirect.dependabot.com/RiotGames/developer-relations/issues)
  - Spectator-V4 was deprecated in April 2024. — same search summary
- **Rate limits:**
  - Personal key: 20 requests per 1s and 100 per 2min. Production starts at 500 per 10s and 30,000 per 10min. Limits are enforced per region.
  - Personal keys "won't be approved for rate limit increases".
  - [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **(SECONDARY)** At 100 per 120 s, a personal key allows about 72,000 requests per day per routing region. — [DarkIntaqt](https://darkintaqt.com/blog/riot-app-application)

### Inferences
- **Call budget per newly linked TR1 user:**
  1. Account-V1 by-riot-id (EUROPE): 1 call.
  2. Summoner-V4 by-puuid (TR1): 1 call, for the icon and verification.
  3. League-V4 entries by-puuid (TR1): 1 call, which returns the RANKED_FLEX_SR entry.
  4. Match-V5 match IDs by puuid with queue=440 (EUROPE): 1 call.
  5. Match-V5 match details (EUROPE): N calls, for example N=10 to derive the main role from each participant's `teamPosition`.
- **That totals about 14 calls**, about 12 of them on EUROPE.
  - **Personal key:** about 8 full onboardings per 2 minutes on EUROPE.
  - **Production:** thousands of onboardings.
- **Mitigations:**
  - Cache role summaries.
  - Fetch only 5 matches, or let users self-declare their role and verify it lazily.
  - Refresh rank only when a user posts or answers an ad (League-V4, 1 call on TR1).
- **Unverified detail:** the `teamPosition` field in Match-V5 comes from background knowledge and was not verified in this pass.
- **Method limits:** Match-V5 and Spectator also have per-endpoint method rate limits that may be lower. They are returned in response headers.
- **Using Spectator:** polling it for everyone in a lobby is cheap (1 TR1 call per user). It could, for example, auto-close an ad when the poster's group enters a Flex game (queueId 440 in the response).

### Gaps
- I did not retrieve the exact method rate limits for Match-V5 and Spectator-V5 from the official portal.
- No official doc confirms the absence of presence data. That conclusion comes from the absence of any presence endpoint in the API sources reviewed.

## Q9. .NET library status (Nov 2026): Camille, RiotSharp, Discord.Net, NetCord, DSharpPlus, and Discord.Net feature coverage

### Takeaway
- **Discord.Net** is actively maintained: 3.20.2 shipped 8 Oct 2026, targeting net8/9/10. Its Interaction Framework supports slash commands, buttons and select menus, modals, ephemeral responses, autocomplete, DM contexts and thread channel types. That covers everything the ad-board needs.
- **NetCord** is very active but still beta-only: 1.0.0-beta.30 on 7 Nov 2026, net10.0.
- **DSharpPlus** has 4.5.2 stable (Apr 2026) and active 5.0 nightlies.
- **Camille** (`Camille.RiotGames`) has only 3.0.0 nightlies, the latest dated 24 Jun 2025. That is about 17 months without a published build, so treat it as stale.

### Cited Findings
- **Discord.Net NuGet:**
  - Latest stable 3.20.2 (10/8/2026). Earlier versions: 3.20.1 (6/7/2026), 3.20.0 (6/6/2026), 3.19.1 (3/12/2026), 3.19.0 (3/3/2026).
  - Targets net8.0, net9.0 and net10.0. About 4.8M downloads.
  - [NuGet Discord.Net](https://www.nuget.org/packages/Discord.Net)
- **Discord.Net Interaction Framework docs:**
  - SlashCommandAttribute, component interaction commands (buttons and select menus, whose values are passed in), modal commands (`IModal`), autocomplete, and `RespondAsync(..., ephemeral: true)`.
  - The `IThreadChannel` parameter type and the `IDMChannel` type.
  - DM availability is moving from `EnabledInDmAttribute` to `CommandContextType` / `InteractionContextType.BotDm`.
  - [Discord.Net docs](https://docs.discordnet.dev/guides/int_framework/intro.html)
- **NetCord NuGet:** 1.0.0-beta.30 (11/7/2026), beta.29 (11/2/2026), beta.28 (10/1/2026), beta.27 and beta.26 (9/26/2026). No stable release. Targets net10.0. About 2.8M downloads. — [NuGet NetCord](https://www.nuget.org/packages/NetCord)
- **DSharpPlus:** stable 4.5.2 (last updated 14 Apr 2026). 5.0.0 nightlies were still publishing in May 2026, e.g. nightly-02587 dated 4 May 2026. — [NuGet DSharpPlus 5.0 nightly](https://www.nuget.org/packages/DSharpPlus/5.0.0-nightly-02585) (search summary)
- **Camille:**
  - The `Camille.RiotGames` NuGet package lists only prereleases. The latest is 3.0.0-nightly-2025-06-24-d2b94715b7 (6/24/2025). Targets .NET 6 and .NET Standard 2.1, with net8.0 also listed. About 51.8K total downloads. — [NuGet Camille.RiotGames](https://www.nuget.org/packages/Camille.RiotGames)
  - The old `MingweiSamuel.Camille` package stops at 2.8.0 stable plus nightlies, with the last dated nightly in 2022. — [NuGet MingweiSamuel.Camille](https://packages.nuget.org/packages/MingweiSamuel.Camille)
  - GitHub repo: branch `release/3.x.x`, 268 commits, 101 stars, no GitHub Releases.
  - The repo advertises nightlies that reflect Riot API changes within 24 hours, automatic retries and rate limiting. Its README example shows `GetAllChampionMasteriesByPUUID`.
  - The README does not mention EUROPE routing or TR1 explicitly.
  - [GitHub Camille](https://github.com/MingweiSamuel/Camille)

### Inferences
- **Camille and by-PUUID endpoints:** Camille's codegen nightly of June 2025 came out right around the 20 Jun 2025 summonerId removal, so it likely includes the by-PUUID League-V4 endpoint. Verify this, and if the generated surface lags, fall back to a thin HttpClient wrapper. The Riot endpoints this bot needs are only about 5, so a hand-rolled typed client with a token-bucket limiter is low-risk.
- **Recommended Discord library:** Discord.Net, as the stable, maintained choice. NetCord is a strong modern alternative if beta status is acceptable and the project targets .NET 10.

### Gaps
- RiotSharp status was not checked in this pass. Background knowledge says it has been unmaintained for years and predates Riot ID and PUUID changes; that is unverified.
- I did not verify that Camille 3.0 nightlies include League-V4 `entries/by-puuid` and Account-V1 `by-riot-id`. Check the generated source in the repo.
- Discord thread creation and role pings: these are standard Discord API features that Discord.Net exposes through channel and message APIs, but they are not confirmed from the page fetched here.
