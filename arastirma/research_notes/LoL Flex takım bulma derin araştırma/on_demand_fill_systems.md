# On-demand slot-filling systems ("need 1 more right now") vs fixed scheduled teams: design lessons for a TR LoL Flex Discord bot

Scope note: the original brief covered fixed 5-player scheduled teams. Mid-task the user changed scope. They want an on-demand "ad board": a group that is short (for example 4 friends who need a 5th for Flex) posts once and fills the slot within minutes. Valorant Premier and LoL Clash are therefore covered only briefly, as a contrast. Most of the effort went into real-time LFG and party-finder systems.

Source quality legend: **[Official]** is a primary developer or support page. **[Community docs]** is a fan wiki or API wiki. **[Press]** is a news site. **[Anecdote]** is a forum post or Reddit thread. **[Self-reported]** is a bot or app's own marketing listing. **[Snippet]** means only the search-engine summary was seen because the page blocked fetching (402/403), so it is less verified.

---

## Q1. Contrast case: how do Riot's fixed-team modes (Valorant Premier, LoL Clash) work, and what does that say about on-demand fill?

### Takeaway
Premier and Clash are built around a persistent roster with an owner or captain, a bench of 5–7 players, and a few fixed weekly windows. Their "substitute" mechanics show that the hard problem, even inside fixed-team modes, is filling one missing seat at the last minute. Riot handles that with a bench and lock-in windows, not with a live finder. Neither mode has an in-client "find 1 more player now" board, as far as the sources found show.

### Cited Findings
- **Premier roster size:** "Premier teams have 5-7 players." The creator becomes Team Owner. The Owner can assign one Captain who can invite and remove players, and the party leader queues the team and locks map picks. A player can be on only one roster at a time. (Riot support article dated 2025-07-07.) **[Official]** — [Riot Support: Premier Team Creation & Management](https://support.riotgames.com/en-us/valorant/gameplay/premier-team-creation-management)
- **Premier eligibility and division lock:** players need a verified account and at least one lifetime ranked placement. Any behavior penalty makes a player "ineligible to join a Premier Team or queue up for Premier matches for 30 days". After the first match, a team "cannot invite players who would move the team's current division". Removing a player after the first match carries "no guarantee that you can re-add them." (2025-07-07) **[Official]** — [Riot Support](https://support.riotgames.com/en-us/valorant/gameplay/premier-team-creation-management)
- **Premier schedule:** one map per week. Teams and individuals are capped at 2 matches per week. Matches are played in in-client "queue windows", and the article advises queuing early in a window. Enrollment is no longer required; teams can start any time during a Stage. (2025-07-07) **[Official]** — [Riot Support](https://support.riotgames.com/en-us/valorant/gameplay/premier-team-creation-management). The older Ignition-stage FAQ (2023) said teams that missed the enrollment week could not play, so Riot has since removed that friction. — [Premier Ignition Stage FAQ](https://playvalorant.com/en-us/news/game-updates/premier-ignition-stage-faq/)
- **Premier stage length:** the Spanish-language support page describes "up to 2 matches per week for 7 consecutive weeks", plus 1 more week for playoff teams. Exact times are published in the client. (Undated support page.) **[Official, via snippet]** — [Riot Support (es-es): Premier joining/leaving teams](https://support.riotgames.com/es-es/valorant/gameplay/premier-joining-leaving-teams)
- **Premier participation data:** the only number found is that by 2024 Premier had promoted "over 40 teams" into Challengers. No player or team participation counts were found. **[Official]** — [Riot Competitive Operations: VALORANT](https://competitiveops.riotgames.com/en-US/VALORANT)
- **Clash lock-in and subs:** lock-in opens 30 minutes before matches start. If the full team does not lock in, the team does not take part in that day's matches. A substitute can be used for a single day, but only after the lock-in phase begins. The sub must be in the same tier and hold their own Clash ticket. Older 2018 sources gave different sub rules, such as subs only on days 2–3 or one sub per day. **[Community docs / Press]** — [LoL Wiki: Clash](https://leagueoflegends.fandom.com/wiki/Clash); [Dignitas: Overview of LoL Clash](https://dignitas.gg/articles/an-overview-of-league-of-legends-clash-tournaments)

### Inferences
- Even Riot's own "fixed five" products need an escape hatch for a missing fifth player: Premier's 7-player bench and Clash's day-of sub. This supports the user's pivot. The common real-world need is "fill 1 seat now", not "maintain a roster".
- Riot's division and MMR locks (Premier) and same-tier rule (Clash) show that rank compatibility is the main constraint on who can fill a seat. A Flex fill bot should also make rank compatibility explicit. Flex has its own duo/party rank-spread restrictions, which another researcher should verify.

### Gaps
- No Riot data was found on how many Clash teams fail to form or forfeit at lock-in, or on Premier participation numbers. Searches returned only promotion counts.
- No Premier no-show or forfeit policy was found in official docs. Searches surfaced only VCT and Game Changers forfeits, which are not relevant.
- No current official Riot support article on Clash subs was retrieved. The Clash rules above come from a fan wiki and Dignitas, and the dates of the current rules are unconfirmed.

---

## Q2. What mechanics do on-demand LFG systems use to get posts answered fast?

### Takeaway
The mature in-game finders (FFXIV Party Finder, WoW Premade Groups, Destiny 2 Fireteam Finder, Overwatch 1 LFG) share a core design. A host lists an activity with typed role slots and requirements, applicants are filtered and then approved or auto-accepted, and stale listings expire automatically. WoW's 30-minute auto-delist and FFXIV's "notify me when a matching listing appears" alerts address the two biggest liquidity issues: stale posts and seekers who have to keep watching the board.

### Cited Findings
- **FFXIV Party Finder: typed slots.** Leaders can assign specific jobs, classes or roles to each slot, leave slots open to any class, or remove slots. The duty sets the maximum slot count. Listings also carry an objective (Practice, Duty Completion, Loot), a free-text description, a minimum average item level, a home-server-only toggle, language tags, and an optional 4-digit password for private listings. **[Community docs]** — [FFXIV ConsoleGamesWiki: Party Finder](https://ffxiv.consolegameswiki.com/wiki/Party_Finder)
- **FFXIV Party Finder: push notifications to seekers.** Notifications are on by default. They tell the player how many groups match their saved Advanced Search, which can filter by duty, open job slot and item level, and the alert interval is configurable. Seekers can also filter by tag and hide blacklisted recruiters. Each category shows up to 50 listings per page. **[Community docs]** — [FFXIV ConsoleGamesWiki: Party Finder](https://ffxiv.consolegameswiki.com/wiki/Party_Finder)
- **WoW Premade Groups: auto-expiry.** The listing API says a listing's expiration "Currently starts at 1800 seconds (30 minutes)". It adds: "If no player joins the group within this time the group is delisted for inactivity." The same API exposes an `autoAccept` flag ("If new applicants are automatically invited"). The page's change log covers patches 8.1.0 (2018) through 10.2.7 and 11.0.7 (2024). **[Community docs]** — [warcraft.wiki.gg: C_LFGList.GetActiveEntryInfo](https://warcraft.wiki.gg/wiki/API:C_LFGList.GetActiveEntryInfo)
- **WoW: apply then accept, with friction.** A Blizzard forum thread reports that accepting a group of applicants into a premade raid removes the listing, so the leader must relist. **[Anecdote, undated]** — [Blizzard Forums: Group joins raid, removes group listing](https://us.forums.blizzard.com/en/wow/t/group-joins-raid-removes-group-listing/1711919). Third-party addons exist mainly to fix the applicant and listing workflow. GroupFinder highlights applied, declined, delisted and full states. Auto Sign Up mass-applies and skips groups whose role slot for you is full, groups already applied to, and delisted groups. JustFilters clears old applicants on relist and recommends relisting every 6 minutes. **[Self-reported]** — [CurseForge: GroupFinder](https://www.curseforge.com/wow/addons/groupfinder); [CurseForge: Auto Sign Up](https://www.curseforge.com/wow/addons/auto-sign-up); [CurseForge: JustFilters](https://www.curseforge.com/wow/addons/justfilters/files/8990324)
- **Destiny 2 Fireteam Finder: timeline.** It was announced for Lightfall (2023) and then delayed. A beta and stress test ran from about 30 Nov 2023, and it later became permanent. Gamingbolt reported "over 60 bug fixes and improvements" after the beta, with voice chat and lobby invites in the works. **[Official, via snippet / Press]** — [Bungie: Dev Insights – Fireteam Finder Beta First Look (Nov 2023)](https://www.bungie.net/7/en/News/article/fireteam_finder_beta_preview); [Insider Gaming: release delayed, coming as beta](https://insider-gaming.com/destiny-2-fireteam-finder-release-delayed-but-coming-as-a-beta/); [Gamingbolt: Fireteam Finder exits beta](https://gamingbolt.com/destiny-2-fireteam-finder-exits-beta-voice-chat-and-lobby-invites-in-the-works/amp); [PC Gamer: Destiny 2 getting in-game LFG](https://pcgamer.com/destiny-2-is-finally-getting-in-game-lfg-and-loadout-systems-with-lightfall)
- **Destiny 2 Fireteam Finder: flow.** It is a listing system "without automatic matchmaking". Hosts create a listing with an activity, tags and requirements. Seekers browse and then "Join or apply to group". A February 2024 This Week in Destiny post listed "better feedback when you apply to lobbies" as a planned fix, which implies applicants were left unsure whether they had been seen. **[Community docs / Official, via snippet]** — [Destiny Game Wiki: Fireteam Finder](https://d2.destinygamewiki.com/wiki/Fireteam_Finder); [Bungie Help: Fireteam Finder Guide](https://help.bungie.net/hc/gl/articles/25787853699220-Fireteam-Finder-Guide) (page blocked fetching; content seen only via snippet)
- **Overwatch 1 LFG (2018): requirements filter tied to reputation.** It launched with the Endorsement system. Group leaders could set a minimum Endorsement level from 1 to 5 for joiners. LFG was pitched as giving players "more control around your gameplay experience". (June 2018 developer update.) **[Press]** — [AIPT: Looking for Group and Endorsements (2018-06-06)](https://aiptcomics.com/2018/06/06/new-functionality-comes-to-overwatch-looking-for-group-and-endorsements/); [PC Gamer: Overwatch getting LFG and tools to reward good behavior](https://pcgamer.com/overwatch-is-getting-a-looking-for-group-feature-and-tools-to-reward-good-behavior)

### Inferences
- The recurring pattern for fast fills is: 1) typed open slots (for LoL: Top, Jungle, Mid, ADC or Support, plus rank range); 2) a machine-readable requirement filter so seekers only see posts they qualify for; 3) pushing alerts to seekers who opted in, so they do not have to watch the board; 4) auto-expiry so the board shows only live demand.
- WoW players' heavy use of addons for auto-apply, state tracking and relist reminders suggests that a manual apply-and-accept loop is the slowest part of the flow. For a "need 1 now" case, a one-tap "I'm in" with optional host auto-accept would probably beat a multi-step application. This is an inference, not measured.
- A 30-minute default expiry is one reference point (WoW). For a "right now, within minutes" Flex fill, a shorter TTL, for example 10–20 minutes with a one-click "bump/extend", fits the intent better. This is a design suggestion, not sourced.

### Gaps
- FFXIV Party Finder's listing duration (auto-expiry time) was not found in any fetched source.
- The exact Destiny 2 Fireteam Finder listing lifetime, application cap and auto-approve option could not be verified. The Bungie Help and Bungie.net pages blocked fetching or rendered empty.
- Overwatch 1 LFG specifics, such as role-slot types, whether applicants needed approval, and listing expiry, could not be confirmed. The Overwatch wiki returned 402.

---

## Q3. What data exists on adoption, time-to-fill and fill rate?

### Takeaway
Hard numbers are almost nonexistent. The best data point is Blizzard's 2018 claim that six-stack grouping "doubled" in the first week after Overwatch LFG launched, alongside a drop in abusive chat. No source was found that publishes time-to-fill or fill-rate data for any LFG board.

### Cited Findings
- **Overwatch LFG early impact:** Jeff Kaplan reported that "instances of players grouping in six-stacks has doubled in the first week". He also said abusive behavior had "gone down dramatically" in the Americas and Korea since LFG and Endorsements launched (mid-2018). These are developer self-reported figures, seen through press-coverage snippets; the full article text could not be fetched. **[Press, via snippet]** — [PC Gamer: Overwatch endorsements and LFG features are already working](https://www.pcgamer.com/overwatch-endorsements-and-looking-for-group-features-are-already-working); [MassivelyOP: Overwatch's new LFG system is a huge hit](https://massivelyop.com/?p=175768)
- **Liquidity concentrates on one hub:** FFXIV's non-Japanese players rely almost entirely on Party Finder for high-end duties. Players have clustered in one data center per region (Aether for NA, Light for EU, Mana for JP), and others may need to travel there to find groups "in a reasonable time". **[Community docs]** — [FFXIV ConsoleGamesWiki: Party Finder](https://ffxiv.consolegameswiki.com/wiki/Party_Finder)
- **Destiny 2: anecdotal low liquidity.** A Steam user wrote that Fireteam Finder "is almost always empty for me in every activity" and that joining "only works sometimes". **[Anecdote, undated]** — [Steam Community discussion](https://steamcommunity.com/app/1085660/discussions/0/651430500729138697)

### Inferences
- The FFXIV data-center clustering is a natural experiment in liquidity. When the pool is split, players move voluntarily to the largest hub. For a TR bot, this argues for one shared cross-server pool or channel for all TR Flex seekers, rather than many small per-server boards that each look empty. This also matches the general two-sided-market logic that an empty board deters both sides.
- The Overwatch figure suggests that a low-friction in-product finder can substantially increase premade play, but the number is old (2018) and self-reported.

### Gaps
- No published time-to-fill, fill-rate or abandonment metrics were found for FFXIV PF, WoW Premade Groups, D2 Fireteam Finder, Overwatch LFG, or any Discord LFG bot.
- No data was found on how usage varies by hour of day for any LFG board, so the off-hours emptiness problem is supported only by anecdote.

---

## Q4. What kills LFG boards, and why was Overwatch's LFG removed?

### Takeaway
The documented failure modes are low usage and liquidity (an empty board leads to fewer posters), toxicity and gatekeeping by hosts, unclear application feedback, and server or engineering complexity. Overwatch is the clearest case of a big LFG feature being cut. Blizzard retired it with the Overwatch 2 launch (Oct 2022), and the reasons reported in the press were declining usage and social or disruptive behavior. Players have campaigned for its return since.

### Cited Findings
- **Overwatch 2 removal:** the October 2022 Overwatch 2 patch notes listed LFG among retired features, alongside loot boxes, general chat and player levels. **[Press]** — [Gameranx: Overwatch 2 getting rid of LFG](https://gameranx.com/updates/id/376070/article/overwatch-2-seems-to-be-getting-rid-of-the-looking-for-a-group-feature/)
- **Stated reasons conflict.** Dexerto (2023-05-30), relaying a Reddit thread, says developers cited "social and disruptive behavior problems". **[Press citing Reddit]** — [Dexerto](https://www.dexerto.com/overwatch/overwatch-2-blizzard-removed-aspects-no-sense-2161280). Other secondary coverage says Blizzard pointed to declining LFG usage over time and a wish to try other ways of forming groups, and that a return was "unlikely to happen soon". **[Press, via snippet; the primary Blizzard quote was not verified]** — [esports.gg: Overwatch 2 LFG unlikely to happen soon](https://esports.gg/news/overwatch/overwatch-2-lfg-is-unlikely-to-happen-soon). A forum post quotes a server engineer saying LFG was "very complicated on the server, too". **[Anecdote, second-hand]** — [Blizzard EU Forums: Find Group Overwatch 1](https://eu.forums.blizzard.com/en/overwatch/t/find-group-overwatch-1/35573)
- **Player demand persisted after removal:** there are repeated forum threads ("Day 23 of asking for LFG back") and a Change.org petition. **[Anecdote]** — [Blizzard Forums: Day 23 of asking for LFG back](https://us.forums.blizzard.com/en/overwatch/t/day-23-of-asking-for-lfg-back/738777); [Change.org petition](https://www.change.org/p/bring-back-the-looking-for-groups-feature-in-overwatch-2)
- **Gatekeeping and toxicity on open boards (WoW):** players describe Mythic+ LFG as a "toxic quagmire" for players without an established history, because hosts reject applicants who lack a score or record. **[Anecdote]** — [Blizzard Forums: Mythic Plus and LFG are a Toxic Quagmire](https://us.forums.blizzard.com/en/wow/t/mythic-plus-and-lfg-are-a-toxic-quagmire-for-players-that-were-not-established-in-bfa/835826)
- **Overwatch "LFG failed because we're hypocrites":** a player thread argues that hosts posted strict requirements while applicants ignored them, and that players' behavior wrecked the board. **[Anecdote]** — [Blizzard Forums thread](https://us.forums.blizzard.com/en/overwatch/t/lfg-failed-because-we-re-hypocrites/307531?page=2)
- **Destiny pre-Fireteam-Finder:** random LFG group quality was "sketchy at best". Players reported being kicked near the end of content so a friend could take their spot. **[Press]** — [GamesBeat: Destiny fans want better matchmaking, so they're making it themselves](https://gamesbeat.com/?p=119170)
- **Unclear application status:** Bungie listed "better feedback when you apply to lobbies" as a Fireteam Finder improvement in February 2024, which means applicants were left uncertain. **[Official, via snippet]** — [Bungie: Fireteam Finder beta preview](https://www.bungie.net/7/en/News/article/fireteam_finder_beta_preview)
- **Ping spam:** Discord users have asked for a customizable role-ping cooldown and limits on who can ping a role. Discord does not natively rate-limit role pings per role, which is why LFG bots add their own throttles. **[Anecdote, Discord feedback forum]** — [Discord Support Community: Customizable Role-Ping Cooldown](https://support.discord.com/hc/vi/community/posts/1500000433002-Customizable-Role-Ping-Cooldown)

### Inferences
- The main ways LFG fails relate to liquidity (empty board), trust (toxic or no-show strangers, gatekeeping), feedback (an applicant never hears back), and noise (pings or stale posts). A Flex fill bot needs an explicit answer for each one.
- Overwatch's 2018 LFG was paired with a reputation system (Endorsements) and early behavior gains were reported. Its later removal was blamed partly on behavior, so reputation gates alone may not keep a board healthy over the long run. This is an inference with weak evidence.
- No-shows after accepting were not documented with data for any system. They show up only as general "quality of randoms" complaints.

### Gaps
- No primary Blizzard statement on why OW2 dropped LFG was retrieved; the "declining usage" and "behavior problems" claims are secondary.
- No source gave quantitative no-show or ghosting rates after joining an LFG group.

---

## Q5. Discord LFG and scheduling bots: which features are standard and which are rare?

### Takeaway
Current Discord LFG bots already provide slash-command group posts, role-slot sign-up buttons, waitlists, auto-created temporary voice channels, and role pings, so these are table stakes. The rarer and more interesting features are "smart" targeted pings (only players who recently played that game, rate-limited per user), moderation history per player, and privacy guarantees.

### Cited Findings
- **LFG Hub:** `/create-group` (or `/lfg`) creates a group post with its own voice channel, which deletes itself when the last player leaves. Players sign up per role. When a role is full they join a waitlist and move in when a seat opens. "Smart Ping" pings only players who recently played that game and never pings the same person twice within 8 hours. "Custom Ping" pings a server role. Moderators can see a player's full group history after a report. The bot claims it does not read other channels, never records voice, and deletes game-activity data after 30 days. **[Self-reported, via snippet; the page blocked fetching]** — [top.gg: LFG Hub](https://top.gg/bot/1163043953273217044)
- **Puck:** posts LFG groups with role buttons that players press to pre-fill spots. It can be configured to ping a role and to set a default group type per role, and it needs "Mention Everyone" permission to ping roles. **[Self-reported, via snippet]** — [top.gg: Puck](https://top.gg/es/bot/703068724818608138)
- **Recommended content of a good LFG post:** communication preference (mic required, Discord voice, text or ping only) and rank. **[Press / vendor blog]** — [ExitLag: How to find a group on Discord](https://www.exitlag.com/blog/lfg-how-to-find-a-group-on-discord/)
- **Reputation as a differentiator in LFG apps:** Premate.gg (Valorant) markets post-game teammate ratings, so that toxic players get low ratings and are avoided. **[Self-reported]** — [Premate.gg: toxic-free Valorant](https://premate.gg/en/guides/toxic-free-valorant)

### Inferences
- Role-slot buttons, waitlists, temporary voice channels and role pings are already offered by generic LFG bots, so a TR Flex bot cannot stand out on these alone.
- LFG Hub's "recent players of this game, max once per 8h" ping is the closest existing analogue to an "available now" push pool. A stronger version for LoL would let players opt into a "müsaitim / available now" state for a set time (for example the next 60 minutes) with role and rank, then ping only matching available players. This is inference; no bot with exactly this LoL-aware design was confirmed.
- Scheduling bots (Raid-Helper, Sesh, Apollo) were in the original brief but are less relevant after the scope change. Their RSVP and recurring-event features fit planned sessions, not instant fills. Their docs were not researched in this pass.

### Gaps
- Feature claims for LFG Hub and Puck come from top.gg search snippets, because the pages could not be fetched. Server counts and usage numbers were not available.
- No LoL-specific Discord fill bot with Riot API rank verification was found or reviewed. GitHub "Clash bot" projects were not researched after the scope change.
- No research was done on Turkey-specific LFG communities or Discord servers' activity patterns.

---

## Q6. Synthesis: which features are table stakes and which differentiate a TR "need 1 more for Flex now" Discord bot?

### Takeaway
Typed role slots, rank and requirement fields, one-click join or apply, auto-expiry of stale posts, auto voice channels and role pings are table stakes; the major finders and existing Discord bots already have them. Differentiation is likely to come from availability-aware targeted push (FFXIV-style alerts, LFG Hub-style smart pings, adapted to LoL), Riot-API-verified rank and role so Flex party restrictions and requirements are checked automatically, fast accept/decline with clear applicant feedback, and reputation and no-show tracking. All of these serve one cross-server TR pool, which concentrates liquidity.

### Cited Findings
- **Typed slots plus requirements** are standard in FFXIV Party Finder and WoW Premade Groups, and Overwatch LFG had a minimum Endorsement level. — [FFXIV wiki](https://ffxiv.consolegameswiki.com/wiki/Party_Finder); [AIPT 2018](https://aiptcomics.com/2018/06/06/new-functionality-comes-to-overwatch-looking-for-group-and-endorsements/)
- **Auto-expiry** is standard in WoW (30-minute inactivity delist). — [warcraft.wiki.gg](https://warcraft.wiki.gg/wiki/API:C_LFGList.GetActiveEntryInfo)
- **Push alerts to seekers** for matching listings exist in FFXIV, on by default with a configurable interval. — [FFXIV wiki](https://ffxiv.consolegameswiki.com/wiki/Party_Finder)
- **Targeted, rate-limited pings** (LFG Hub Smart Ping, at most once per 8 hours per user) and **waitlists plus temporary voice** are available in existing Discord bots. — [top.gg: LFG Hub](https://top.gg/bot/1163043953273217044)
- **Applicant feedback** was a gap that Bungie had to fix in Fireteam Finder (Feb 2024). — [Bungie](https://www.bungie.net/7/en/News/article/fireteam_finder_beta_preview)
- **Concentrated liquidity** shows in FFXIV players moving to one data center per region. — [FFXIV wiki](https://ffxiv.consolegameswiki.com/wiki/Party_Finder)
- **Reputation and behavior gating** paired with LFG was tied to doubled six-stacks and lower abuse in Overwatch in 2018 (self-reported). Behavior issues were later cited in its removal. — [PC Gamer](https://www.pcgamer.com/overwatch-endorsements-and-looking-for-group-features-are-already-working); [Dexerto](https://www.dexerto.com/overwatch/overwatch-2-blizzard-removed-aspects-no-sense-2161280)

### Inferences
**Table stakes (expected; missing them hurts, having them does not differentiate):**
1. A slash-command or button post: "4/5, need: Support, rank Gold–Plat, mic yes/no, TR server, Flex".
2. Typed open role slots (Top, Jungle, Mid, ADC, Support or Fill) and a rank-range field.
3. A one-tap "Join" or "Apply" button, with host accept/decline and an optional auto-accept toggle (WoW `autoAccept`).
4. Auto-expiry of posts (WoW reference: 30 min; for "now" fills, likely 10–20 min with a "bump/extend" button). The post auto-closes or edits to "FULL" when the slot fills.
5. An auto-created temporary voice channel, or a link to the host's voice.
6. Role pings, with rate limiting to avoid spam.
7. A waitlist or bench for a second applicant if the first drops out.

**Likely differentiators (rare or absent in generic bots, and they address the documented failure modes):**
1. **An "available now" opt-in pool:** players mark themselves available for X minutes with role and rank. A new post pings only matching available players, and each player is rate-limited. This combines FFXIV-style matching alerts with LFG Hub's smart ping and addresses the empty-board and noisy-ping problems.
2. **Riot API-verified rank and main role:** Flex rank is auto-checked against the post's range and Flex party rules, so hosts are not gatekept by self-reported ranks. This addresses rank mismatch and WoW-style gatekeeping complaints.
3. **Clear applicant feedback loop:** the bot DMs "accepted / declined / post expired" and shows a live count of viewers or pings sent. This addresses the Destiny "better feedback when you apply" problem.
4. **No-show and reliability tracking:** after accept, the bot asks for confirmation within N minutes, with lightweight post-game "showed up / no-show" flags and a per-user reliability score visible to hosts. Moderators get the history, as in LFG Hub, which addresses trust. No source measured no-show rates, so this rests on inference.
5. **One cross-server TR Flex pool:** the bot posts to a shared hub across partner servers, or keeps a cross-server index, instead of isolated per-server boards. This follows the FFXIV liquidity concentration lesson.
6. **Off-hours handling:** when the pool is thin, the bot shows "N players available now in your rank" before posting, and offers a short scheduled fallback ("post for 21:00 tonight"). This addresses empty boards at off-hours, which is supported only by anecdote.
7. **Graduation to recurring groups:** after a successful fill, a "play again / add to friends group" prompt. This links the on-demand flow back to the fixed-team concepts in Q1, where Premier and Clash rosters show demand for persistent fives.

### Gaps
- None of the differentiators above are backed by measured outcomes. They are design inferences from the documented failure modes.
- LoL Flex party rank-restriction rules and the Riot API scopes needed for rank and role verification were not researched here, and should be verified separately.
- Apex, Valorant and Fortnite "fill teammates" queue toggles and Dota 2 or CS2 party finders were not researched in this pass because of the tool-call budget. These are matchmaking fill options, not boards, and are a lower priority for a Discord ad-board design.
