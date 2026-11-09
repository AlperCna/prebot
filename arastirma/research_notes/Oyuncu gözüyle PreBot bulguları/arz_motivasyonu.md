# Supply-side motivation of the "5th player" in LoL Ranked Flex (TR focus) — testing the PreBot claim

Claim under test: "Supply-side motivation is weak: a 4-player group urgently needs a 5th, but why would a solo player join? A solo player can already queue Flex alone with fast queues. Joining a stranger 4-stack is often a disadvantage (the 4 talk on voice, you're the outsider, you get blamed when losing)."

Research date: 2026-11-09. Research limits: Reddit was not reachable with the tools used (both the search domain filter and direct fetch were refused), and League of Graphs and nerfplz returned HTTP 403. Player-sentiment evidence therefore leans on Riot statements, Turkish forums/Discord listings, and cross-game forums (Steam, VLR.gg) rather than r/leagueoflegends threads.

## 1. Current (2025–2026) Ranked Flex rules, rewards, and queue times

### Takeaway
Ranked Flex still allows parties of 1, 2, 3 or 5 (never 4). There are no rank limits from Iron to Diamond. Flex MMR was pulled toward Solo/Duo MMR for 2026, but Flex "will never pull Solo up." The biggest 2026 change for PreBot is Riot's **Ranked 5s** experiment, which is live on TR. It is a weekend-only, full-premade-only ranked queue with its own rewards, and a solo player cannot enter it at all. No published Flex queue-time figure exists for TR. Riot's last official number (2022) was about 7 minutes on average for 3-person parties, and third-party guides describe the Flex pool as smaller and slower than Solo/Duo, especially off-peak.

### Cited Findings
**Party size and rank rules**
- Ranked Flex accepts parties of 1, 2, 3 or 5. A group of 4 cannot queue ranked Flex and must play unranked, add a 5th, or split up. — [Pairly.gg, checked 25 Sep 2026](https://pairly.gg/blog/lol-flex-queue-rules-rank-restrictions-party-size); [Dodge.gg, 23 Mar 2026](https://www.dodge.gg/en-US/lol/news/ranked-flex-guide-2026); [GuildOrder, 18 May 2026](https://guildorder.com/tr/games/league/guides/flex-vs-solo-duo-queue)
- Rank limits: "there are no Flex rank restrictions for Diamond and below." If any party member is Master/GM/Challenger, every other member must be at least Emerald. — Riot Support wording as quoted by [Pairly.gg](https://pairly.gg/blog/lol-flex-queue-rules-rank-restrictions-party-size), citing [Riot Support: ranked tiers, divisions and queues](https://support.riotgames.com/en-us/league-of-legends/gameplay/ranked-tiers-divisions-and-queues) and [Riot Support: apex tiers](https://support.riotgames.com/en-us/league-of-legends/gameplay/master-grandmaster-and-challenger-the-apex-tiers). Pairly says it could not open the Riot pages directly, so these are secondhand quotes.
- Riot Support's stated purpose of Flex's party rule: Flex "is meant to foster an atmosphere of teamwork." Riot also warns that "creating parties with large rank disparities... may result in more difficult matchups." — as quoted by [Pairly.gg](https://pairly.gg/blog/lol-flex-queue-rules-rank-restrictions-party-size)
- Historical note: rank restrictions on Flex parties were lifted in patch 10.15 (2020). — [Sportskeeda](https://www.sportskeeda.com/esports/the-importance-flex-queue-going-under-used-league-legends). Riot's 2020 goal was to "allow players to play with anyone on their team, not just those within nearby ranked tiers." — [Riot /dev, 2020](https://www.leagueoflegends.com/en-us/news/dev/dev-updates-on-2020-ranked-matchmaking/)

**MMR / LP changes (2026)**
- Riot /dev Ranked 2026 (posted 2025-12-01): "we're going to be bringing Flex ranks more in-line with Solo/Duo ranks." Flex "will never pull Solo up, however." The motivating problem was "playing Flex with your friends in a Gold lobby and being absolutely stomped by someone who's Gold in Flex." Riot's goal is to make Flex "feel a little more fair, while still allowing you to play with friends (within reason)." — [Riot /dev: Ranked 2026](https://www.leagueoflegends.com/en-us/news/dev/dev-ranked-2026)
- Timing conflict: the Patch 26.1 coverage says this applies from the 2026 season start (8 Jan 2026), but [Amber.gg](https://amber.gg/blog/league-of-legends/lol-ranked-2026-changes) says the "Flex rank separation" changes activated with Patch 26.9 in April 2026. This was not resolved against Riot primary text.
- Same /dev post: duo queue was re-enabled at all ranks in Solo/Duo, including Challenger, for most regions. The post also cut about 30 seconds of champ-select timers, added harsher dodge rules "for quicker queue times," and introduced Aegis of Valor (double LP or full LP-loss protection on autofilled games). — [Riot /dev: Ranked 2026](https://www.leagueoflegends.com/en-us/news/dev/dev-ranked-2026)
- Premade LP: Dodge.gg says premades get the same LP as solo players, with loss mitigation when matchmaking puts a player at a clear disadvantage. — [Dodge.gg](https://www.dodge.gg/en-US/lol/news/ranked-flex-guide-2026). This partly conflicts with [Elo Ascend](https://www.eloascend.com/blog/mmr-blueprint), which cites Riot's MMR page as saying queuing with friends can mean "slightly more difficult opponents or reduced LP gains." Unresolved.

**Rewards**
- Since 2025, the Victorious skin requires 15 ranked wins, which can come from Solo/Duo, Flex, or both. Riot Support: "can happen in Solo/Duo, Flex, or a combination of the two." — [Pairly.gg](https://pairly.gg/blog/lol-flex-queue-rules-rank-restrictions-party-size); [GuildOrder](https://guildorder.com/tr/games/league/guides/flex-vs-solo-duo-queue). GuildOrder adds that chroma tier follows the higher of the two queue ranks.
- Flex tier earns its own end-of-season border and icon, and rewards are distributed per split. — [Dodge.gg](https://www.dodge.gg/en-US/lol/news/ranked-flex-guide-2026)

**Ranked 5s (new in 2026, includes TR)**
- Riot Support: "You'll need a full team to enter the game queue." Solo queueing is not allowed, and the mode is kept as a full premade queue. The page mentions no feature for finding teammates or filling a 5th. — [Riot Support: Ranked 5s](https://support.riotgames.com/en-us/league-of-legends/events/ranked-5s)
- Schedule: weekends only, Friday to Sunday, "from 4pm until 4am (server time)." Experiment 1 ran 26 Jun–6 Sep 2026. Experiment 2 runs 11 Sep 2026–7 Jan 2027. Riot: "By only enabling Ranked 5s over the weekend, we can guarantee faster matchups." — [Riot Support: Ranked 5s](https://support.riotgames.com/en-us/league-of-legends/events/ranked-5s). Third-party hours conflict: [LolTheory](https://blog.loltheory.gg/ranked-5s-lol/) says Experiment 1 used 8 PM–1 AM and was later extended. Riot's page is authoritative.
- Servers include TR. There are no rank restrictions ("From Unranked to Challenger"). The mode has a separate MMR/LP ladder, 5 placement games, no decay, and Tournament Draft (3 bans, 3 picks, 2 bans, 2 picks). — [Riot Support: Ranked 5s](https://support.riotgames.com/en-us/league-of-legends/events/ranked-5s)
- Rewards: 2026 Ranked 5s Icon (reach any tier), Ranked 5s Banner (Gold+), and an Exclusive Ranked 5s Emote in Experiment 2 for participation (criteria "still to be defined"). The mode counts toward Battle Pass objectives and XP, but not toward Solo/Flex ranked rewards. — [Riot Support: Ranked 5s](https://support.riotgames.com/en-us/league-of-legends/events/ranked-5s)
- Riot's reason for running it as an experiment: a permanent Ranked 5s mode produced many unbalanced games because too few teams were queuing. — summarized in [Yahoo/News coverage of the May 2026 dev update](https://tech.yahoo.com/gaming/articles/league-legends-brings-back-ranked-011209937.html)

**Queue times**
- Riot (Spring 2022): "~7 min on average for 3 person parties" in Ranked Flex, while "the total population in Ranked Flex remains healthy." Queue times and match quality had "diverged significantly," with "2-4 division discrepancies between teams." — [Riot /dev: State of Competitive, Spring 2022](https://www.leagueoflegends.com/en-us/news/dev/the-state-of-competitive-spring-2022/)
- In a 2021 Riot-response thread, a commenter reported 2–15 minute Flex waits. — [devtrackers.gg (2021)](https://devtrackers.gg/leagueoflegends/p/2dd7c2f1-if-riot-wants-flex-to-be-taken-seriously-it-needs-some-serious-work)
- "Flex's pool is smaller, especially mid-week and outside peak hours." The matchmaker widens MMR brackets to compensate, so games can be lopsided. — [GuildOrder, May 2026](https://guildorder.com/tr/games/league/guides/flex-vs-solo-duo-queue). Dodge.gg: Flex pops are slower than Solo/Duo, "especially five-stacks," and gives no figures. — [Dodge.gg](https://www.dodge.gg/en-US/lol/news/ranked-flex-guide-2026)
- Extreme cases in small regions: a SEA Challenger team waited 113 minutes for a Flex match that never came. — [Spilled.gg](https://spilled.gg/challenger-team-sea-minutes-flex-queue-match/). During a 2023 matchmaking experiment, EUNE players reported waits up to 30x longer. — [GamesRadar](https://www.gamesradar.com/league-of-legends-players-say-theyre-waiting-up-to-30x-longer-for-a-game-after-matchmaking-changes) (2023, not current)

### Inferences
- "A solo can queue Flex alone" is **true**. "With fast queues" is **plausible but unverified** for TR. Solos are the easiest unit for the matchmaker to slot, so solo Flex at common ranks (Iron–Emerald) is probably quick at peak hours. Off-peak and at high ranks, waits get longer on a small server like TR.
- Ranked 5s changes supply-side economics on weekends. It is a ranked mode with its own rewards that a solo **cannot play at all** unless they join a 5-person party. Riot provides no in-client tool to find that 5th. Weekend 16:00–04:00 TR time is when a 4-stack's need for a 5th is greatest. It is also when a solo who wants to try Ranked 5s has an actual reason to join strangers.
- The 2026 Flex MMR alignment weakens "Flex as an easy alternative rank" as a motive. Flex LP still gives a separate tier, split rewards, and Victorious-skin wins.

### Gaps
- No TR-specific Flex queue-time data, and no 2025–2026 Riot figure for Flex queue times at any party size.
- Riot's own Flex 4-stack rationale (the 2016 dynamic-queue era) was not found in primary form. See Q3 for the secondhand version.
- The Patch 26.1 notes page fetched only partially. The exact Flex wording in the patch notes was not confirmed.
- Not confirmed whether Ranked 5s continues after 7 Jan 2027 (Riot has not announced).

## 2. Do 5-stacks get MMR/LP advantages or penalties? Is the solo filler advantaged or disadvantaged?

### Takeaway
Riot has never published a 5-stack win rate. Its matchmaking is designed to neutralize the premade advantage: 5-stacks are matched against 5-stacks first, and an MMR offset is applied when that is not possible. LP is reported to be the same for premade members (one third-party source disagrees). Mechanically, the filler shares the team's result and LP. The filler is **not** mechanically disadvantaged versus the other four, and is arguably advantaged over a solo Flex player, who is often the random filling a lobby against coordinated stacks. The disadvantage in the claim is social, not mechanical.

### Cited Findings
- 5-stacks went "from less than 20% of Flex games to greater than 35%," making "full premades the most popular party size in all of Flex Queue." Riot attributed this to Clash's launch and more time at home. Riot then switched Flex matchmaking "to be more similar to Clash's," which "prioritizes balance across a much broader range of MMR." — [Riot /dev: Updates on 2020 Ranked & Matchmaking](https://www.leagueoflegends.com/en-us/news/dev/dev-updates-on-2020-ranked-matchmaking/) (2020)
- Riot (CodeofBear, 2021): Flex has "sort of [an] identity crisis living somewhere between solo queue, normals, and Clash" and is settling into a "semi-serious competitive team or competitive solo practice mode." The matchmaker was "not being strict enough with envelope expansion" across premade sizes. The biggest problem was 3-person parties landing in matches with premade 5s. — [devtrackers.gg (2021)](https://devtrackers.gg/leagueoflegends/p/2dd7c2f1-if-riot-wants-flex-to-be-taken-seriously-it-needs-some-serious-work)
- Riot (2022): the root of Flex's "premade problem" was "the high number of 3 person parties that want to play together," and "our inability to match premades in a compatible way is a detriment to the system." — [Riot /dev: State of Competitive, Spring 2022](https://www.leagueoflegends.com/en-us/news/dev/the-state-of-competitive-spring-2022/). A search-tool summary of the same post said imbalanced-MMR duos had win-rate spreads of about 10% in both directions. This was not re-verified in the fetched text.
- 5-stacks are matched primarily against other 5-stacks. If none of similar MMR is available, they can face smaller parties with an "MMR offset" applied. Premades are said to receive the same LP. — [Dodge.gg, Mar 2026](https://www.dodge.gg/en-US/lol/news/ranked-flex-guide-2026) (third party, no Riot citation)
- Secondhand Riot statement (Drew Levin, as summarized from a 2026 "Red Post Roundup"): in Solo/Duo, duoing is effectively free in Silver and Gold, and compensation starts around Emerald as a slightly harder lobby with no extra LP tax. Riot monitors this to within half a percentage point of expected win rate per skill range. — [nerfplz Red Post Roundup #4 (Aug 2026)](https://www.nerfplz.com/2026/08/red-post-roundup-4-riot-explains-duo.html). The page returned 403 and the content comes only from a search snippet. It concerns duos in Solo/Duo, not Flex 5-stacks.
- GuildOrder argues that a coordinated 5-stack plays above the level of five solos of the same rank, which is why Flex was split from Solo/Duo. It claims the 4-stack block exists because the matchmaker "can't reliably find a fifth solo player willing to accept the disadvantage." It also warns that a high Flex rank can hide "a 4-stack carry with one passenger." — [GuildOrder, May 2026](https://guildorder.com/tr/games/league/guides/flex-vs-solo-duo-queue) (third-party opinion)
- A 3-stack plus two strangers can play worse than a random Solo lobby because "the two strangers can't hear the call going on in Discord." Whether Flex feels easier "depends on whether your stack actually communicates." — [Boosting Market (2026)](https://boostingmarket.com/blogs/lol-solo-queue-vs-flex-queue/) (commercial boosting blog, low authority)

### Inferences
- In a 5-stack, all five players get the same win or loss and, per the more consistent sources, the same LP. The filler's main mechanical risk is the group's own skill and coordination, which the filler can judge beforehand if PreBot shows ranks.
- A solo player queuing Flex alone gets slotted into lobbies that may contain 2- and 3-stacks, or even face a 5-stack, which is the situation Riot itself described as broken (2021–2022). Joining a 5-stack moves the filler to the "coordinated side." This "if you can't beat them, join them" argument is an inference, not measured data.
- A filler who is much lower-ranked than the 4-stack may face harder opponents ("large rank disparities... may result in more difficult matchups"). That is a real but rank-dependent cost.

### Gaps
- No published 5-man premade win rate (Riot, op.gg, u.gg, League of Graphs) was found. League of Graphs returned 403.
- No data on the filler's individual performance or LP outcomes within 4+1 groups.
- The Elo Ascend vs. Dodge.gg conflict on premade LP reduction is unresolved.

## 3. What do players say about joining random 4-stacks as the 5th?

### Takeaway
The negative side of the claim has strong support. Riot removed 4-stacks from Valorant competitive in 2021 explicitly because the solo player in a 4+1 had "a relatively poor play experience" and generated toxicity reports, and community explanations of LoL's Flex 4-stack ban give the same reason. Cross-game forums repeatedly describe the 5th being ignored or blamed. But this evidence concerns **matchmaker-forced** 4+1s. PreBot's 5th is **self-selected and opts in**, often with voice access. On the positive side, TR players do seek groups: TR Discord LFG servers have thousands of members, and solo-side "Flex'e pre lazım" posts exist. Reddit sentiment could not be accessed directly.

### Cited Findings
**Negative (supports the claim)**
- Valorant Patch 3.10 (Nov 2021) removed 4-stacks from Competitive while allowing 5-stacks at all ranks. Riot's rationale: "these types of groups tend to leave the remaining solo queue player with a relatively poor play experience, and we believe that removing this option will result in a significant reduction of overall toxicity reports." — [VALORANT Patch Notes 3.10](https://playvalorant.com/en-us/news/game-updates/valorant-patch-notes-3-10/); quoted in [Sportskeeda](https://www.sportskeeda.com/valorant/valorant-patch-3-10-changes-competitive-queuing-removal-4-man-stack)
- Community explanation of LoL's Flex rule: Riot doesn't want "only one player stuck in a game with four friends that can potentially be toxic to him." — [LeagueFeed](https://leaguefeed.net/solo-queue-vs-flex-queue/) (secondhand, not a Riot quote). On GameFAQs, players describe the rule as avoiding "fifth-man scenarios, which are generally unpleasant for the singular individual, and/or potentially unpleasant for the group." — [GameFAQs](https://gamefaqs.gamespot.com/boards/954437-league-of-legends/74703289)
- Pairly's paraphrase of Riot Support: a 4-stack would leave one stranger as the odd player out on a team of friends, which conflicts with Flex's teamwork goal. — [Pairly.gg](https://pairly.gg/blog/lol-flex-queue-rules-rank-restrictions-party-size)
- Cross-game anecdotes (CS2 Steam forums): "a large majority of 4-man queues always place any blames on the 5th guy," plus reports of the solo being kicked late in a match. — [Steam CS2 discussions](https://steamcommunity.com/app/730/discussions/0/1778261844037145611). These come from a search-tool summary of several CS2 threads, and attribution to this exact thread was not verified.

**Positive or mixed (weakens the claim)**
- In the Valorant 4-stack debate, some players argued that a 4-stack at least plays as a coordinated team, even when toxic, and friend groups complained that they could no longer queue. — [VLR.gg thread](https://www.vlr.gg/51064/4-stack)
- Flex lets premade teams use Discord voice, and is pitched as a way to learn "team coordination, voice comms, and group strategies," which solo queue lacks. — [WhatIsEsports](https://whatisesports.xyz/solo-queue-vs-flex/) (low-authority guide)
- TR solo-side demand for a group exists, for example a Donanımhaber thread titled "TR Server Flex e pre lazım" (2 Oct 2017; a solo asking for a Flex premade; 584 views, 5 replies). — [Donanımhaber](https://forum.donanimhaber.com/tr-server-flex-e-pre-lazim--129212783) (old, small)
- TR LFG Discord communities: "League of Legends TR (#LOLTR)," about 17,596 members (listing snapshot), for finding teams or players on the Turkish server. "League of Legends Türkiye," about 7,228 members (created Jan 2024), aimed at finding in-game teams. — [Discord: League of Legends TR](https://discord.com/servers/league-of-legends-tr-896475140039278592); [Discord: League of Legends Türkiye](https://discord.com/servers/league-of-legends-turkiye-1195405905576071168)

### Inferences
- The claim's "outsider / blamed" point is **well-founded as a risk** and is the same logic Riot used to ban 4-stacks. But Riot's evidence (toxicity reports) comes from strangers assigned by the matchmaker, where the solo did not choose the group, has no voice access, and has no information about the group. PreBot's flow differs on all three points (the filler sees the listing, chooses to tap "Katıl," and can presumably join the group's voice). The outsider effect should be smaller, though not zero.
- The existence of TR Discord LFG servers with tens of thousands of members suggests a pool of players who **want** to be in groups. Not all of them are "fillers," and member counts overstate active users.

### Gaps
- No r/leagueoflegends or r/summonerschool threads could be read (Reddit blocked for the tools). The positive filler experience ("I joined a random 4-stack and it was great") is under-evidenced here, but not disproven.
- No Ekşi Sözlük or Technopat entries specifically on "flex 5. kişi" were found. Searches returned unrelated results.
- No survey data on how fillers rate their experience.

## 4. What incentives do LFG and reputation systems use for the "filler" side, and what evidence exists on what works?

### Takeaway
The best-documented evidence is Overwatch's 2018 endorsements plus LFG tool. Blizzard reported about a 40% drop in matches with disruptive behavior, with 50–70% of players giving endorsements. That figure is self-reported by Blizzard and dates from 2019. LoL's own Honor system was stripped of its material rewards in 2025 (now Battle Pass XP), and players reacted negatively. Smaller LFG apps (Premate, Gankster, Krew) use post-game ratings and host reputation. No source measures whether these incentives specifically increase the supply of fillers.

### Cited Findings
- Overwatch: at GDC 2019, Blizzard's Natasha Miller reported that matches with disruptive behavior fell about 40% (measured by reports per match), with 50–70% of players handing out endorsements. The drop was credited to endorsements together with the Looking for Group feature. — [PC Gamer](https://www.pcgamer.com/overwatchs-endorsement-system-has-cut-disruptive-behavior-by-40-percent)
- Earlier (2018), Jeff Kaplan reported that abusive chat in competitive fell by more than 26% in the Americas and 16% in Korea after LFG and endorsements launched. — [Inven Global](https://labs.invenglobal.com/articles/5645/jeff-kaplan-reports-abusive-chat-on-the-decline-thanks-to-lfg-and-endorsement-system)
- Mechanism: endorsement levels decay if you stop earning endorsements ("if you slip up... your level is going to slip"). Overwatch's LFG let players filter groups by endorsement level, role, and communication style. — [PC Gamer](https://www.pcgamer.com/overwatchs-endorsement-system-has-cut-disruptive-behavior-by-40-percent)
- Caveats: Digital Trends noted that a 40% drop still leaves many toxic matches, and that fewer reports might reflect players giving up on reporting. — [Digital Trends](https://digitaltrends.com/gaming/overwatch-toxicity-down-40-percent-thanks-to-endorsement-system)
- LoL Honor rework (2025): the Honor Shop and Tokens were removed and Honor capsules and orbs were replaced by Battle Pass XP. Higher Honor gives more Battle Pass XP and fewer restrictions. Patch 25.04 made Honor update continuously. — [AltChar](https://www.altchar.com/game-news/riot-overhauls-league-of-legends-honor-system-honor-shop-and-tokens-to-be-removed-aUMRx1j18lMt); [Patch 25.04 notes](https://www.leagueoflegends.com/en-gb/news/game-updates/patch-25-04-notes/). Fans and content creators reacted angrily to the nerf. — [GamesRadar](https://www.gamesradar.com/games/moba/league-of-legends-fans-fury-grows-as-riot-nerfs-another-way-to-get-free-skins-this-time-as-a-reward-for-good-behavior)
- Pre-revamp (Dec 2024): Honor Level 1 disabled chat and made ranked split rewards unavailable. — [LolTheory: Honor system](https://blog.loltheory.gg/honor-system-lol/) (may be outdated after 25.04)
- Premate.gg (Valorant/CS2 LFG): players rate teammates after every game, and consistently high-rated players "rise to the top" and are more likely to be picked. Ratings are anonymous and accounts are verified Riot accounts. — [Premate FAQ](https://premate.gg/faq)
- Gankster: end-of-match reviews, only available if someone in the group has the app installed. — [Gankster](https://gankster.gg/find-valorant-players/)
- PUBG: a 0–5 reputation level is displayed next to the username. Positive ratings from teammates are not possible. — [Mein-MMO](https://mein-mmo.de/en/this-is-how-pubg-will-ensure-that-its-toxic-players-behave-nicely,641278/)
- Krew (squad finder): host ratings, where "bad hosts don't build reputation, good ones do." — [Product Hunt: Krew](https://www.producthunt.com/products/krew-find-gaming-squad)
- Riot itself incentivized group play on TR. The "Play Together – Türkiye" FTW challenge (Ramadan campaign, 12 Mar–2 Apr; TR1 only; any rank) gave one point per friend per game (up to 4), leaderboard rewards for the top 1,000 (physical merch for the top 10), and custom emotes for everyone who played 2+ games. — [Riot FTW: Play Together – Türkiye](https://ftw.riotgames.com/challenges/play-together-turkiye) (year not shown on page)

### Inferences
- The relevant lesson from Overwatch is that rating the **group** (the host side) matters as much as rating the filler. The filler's main fear is a toxic 4-stack, so a two-sided reputation system (group endorsement or host score, like Krew) speaks to the supply-side objection directly.
- Material rewards for good behavior in LoL were cut in 2025. A third-party product like PreBot cannot grant in-game rewards, so its incentives must be social or informational: reputation badges, "plays with voice," "chill/tryhard" tags, repeat-group or friend features, and ranking or leaderboard visibility.
- The "Play Together – Türkiye" challenge shows that Riot TR treats playing with more friends as something worth rewarding, so similar campaigns could create spikes in filler demand and supply.

### Gaps
- No evidence on whether reputation systems increase **filler willingness** (supply), as opposed to reducing toxicity.
- No data on Guilded, LFG.gg, GamerLink, or Destiny 2 LFG app reputation outcomes. Searches did not surface usable primary information.
- The year of the TR "Play Together" campaign was not confirmed. The dates 12 Mar–2 Apr fit Ramadan 2024, but this is an inference.

## 5. Which player segments want to be the 5th, and how common is Flex play vs Solo/Duo?

### Takeaway
The only hard figure on Flex composition is Riot's 2020 statement that 5-stacks were more than 35% of Flex games, the most common party size. In 2022 Riot said total Flex population was "healthy" and dominated by many 3-person parties. No current Flex-vs-Solo/Duo population share (global or TR) could be verified. Segments with a plausible reason to fill can be identified from game rules, but are not measured: weekend Ranked 5s seekers (who cannot otherwise play), Flex/Victorious progress seekers, team-play or voice-comms seekers, solos tired of being the random against stacks, and players building a recurring group.

### Cited Findings
- 5-stacks were more than 35% of Flex games in 2020, up from less than 20%, the "most popular party size in all of Flex Queue." — [Riot /dev 2020](https://www.leagueoflegends.com/en-us/news/dev/dev-updates-on-2020-ranked-matchmaking/)
- 2022: "the total population in Ranked Flex remains healthy," with "the high number of 3 person parties that want to play together." Riot's proposed Solo-Only mode would have sent premades to "Ranked Flex or Organized 5's modes." — [Riot /dev: State of Competitive, Spring 2022](https://www.leagueoflegends.com/en-us/news/dev/the-state-of-competitive-spring-2022/)
- Riot frames Flex as a "semi-serious competitive team or competitive solo practice mode." — [devtrackers.gg (2021)](https://devtrackers.gg/leagueoflegends/p/2dd7c2f1-if-riot-wants-flex-to-be-taken-seriously-it-needs-some-serious-work)
- Flex has its own population and a different rank distribution from Solo/Duo. Secondary articles say Flex has a higher share of Silver players than Solo/Duo, but their figures conflict by year and method. — [Gfinity](https://www.gfinityesports.com/guides/league-of-legends-ranked-distribution-explained) (no reliable current numbers)
- GuildOrder: Flex solo queue "plays loosely," and its LP "doesn't matter" to players who only care about individual rank. — [GuildOrder](https://guildorder.com/tr/games/league/guides/flex-vs-solo-duo-queue) (opinion)
- Ranked 5s requires a full five and has no solo entry. It runs on TR on weekends, with an icon, a Gold+ banner, and a participation emote. — [Riot Support: Ranked 5s](https://support.riotgames.com/en-us/league-of-legends/events/ranked-5s)
- Flex wins count toward the 15-win Victorious skin. — [Pairly.gg](https://pairly.gg/blog/lol-flex-queue-rules-rank-restrictions-party-size)

### Inferences
Candidate filler segments (inferred from rules and context, not measured):
1. **Ranked 5s seekers (weekends, 2026):** solos who want the new mode or its icon, banner, or emote have **no way to play except joining a 5**. This is the clearest supply-side motivation found.
2. **Players seeking team play or voice:** solos who prefer coordinated play over Solo/Duo randomness. Supported by Flex's "teamwork" framing and TR Discord LFG communities.
3. **Solos avoiding the random-vs-stacks experience:** Riot's 2021–2022 statements show that solos and small parties end up in mismatched lobbies against stacks. Joining a stack is a way out.
4. **Progress or cosmetics seekers:** Victorious-skin wins, Flex split rewards, Battle Pass XP (Ranked 5s counts toward it), and Clash-style practice.
5. **Recurring-group builders:** players who want to become a regular 5th. The "friend-making" motive is plausible, but no source measures it.
6. Players who would **not** fill: rank-focused Solo/Duo grinders (Flex "never pulls Solo up"), players who dislike voice, and low-ranked players intimidated by higher-ranked 4-stacks.

### Gaps
- No current (2025–2026) Flex vs Solo/Duo population share for TR or globally. League of Graphs returned 403, and op.gg/u.gg ladder counts were not retrieved.
- No data on what share of Flex 5-stacks are friends versus strangers recruited via LFG.
- No measured segmentation of TR players by motivation.

## 6. Overall verdict: how true is the claim, and what actually motivates a 5th?

### Takeaway
The claim is **partly true and partly overstated**. True: supply is structurally weaker than demand. A 4-stack is blocked from ranked until it finds a 5th, while a solo always has the fallback of queuing Flex or Solo/Duo alone. The 4+1 outsider dynamic is real enough that Riot designed it out of both Flex and Valorant. Overstated: (a) since 2026, Ranked 5s gives solos a ranked mode they **cannot access without joining a group**, and TR is included; (b) the "disadvantage" is mostly social risk, not a mechanical penalty, because the filler shares the team's result and LP and moves to the coordinated side of matchmaking; (c) evidence of harm comes from matchmaker-forced 4+1s, while PreBot's fillers opt in; (d) "fast queues" for solo Flex is unverified for TR and weaker off-peak. On the fallback point (solos can always queue alone), PreBot must offer something solo queue does not: a coordinated voice team, access to Ranked 5s, and a safe, reputation-checked group.

### Cited Findings
- 4-stacks are blocked in Flex (1/2/3/5 only). — [Pairly.gg](https://pairly.gg/blog/lol-flex-queue-rules-rank-restrictions-party-size)
- Riot removed 4-stacks in Valorant because the solo in a 4+1 had "a relatively poor play experience" and generated toxicity reports. — [VALORANT Patch Notes 3.10](https://playvalorant.com/en-us/news/game-updates/valorant-patch-notes-3-10/); [Sportskeeda](https://www.sportskeeda.com/valorant/valorant-patch-3-10-changes-competitive-queuing-removal-4-man-stack)
- Ranked 5s: full team required, weekends 16:00–04:00 server time, TR included, own rewards. — [Riot Support: Ranked 5s](https://support.riotgames.com/en-us/league-of-legends/events/ranked-5s)
- 5-stacks are matched against 5-stacks first, and premades reportedly get the same LP. — [Dodge.gg](https://www.dodge.gg/en-US/lol/news/ranked-flex-guide-2026)
- The Flex pool is smaller off-peak and mid-week. — [GuildOrder](https://guildorder.com/tr/games/league/guides/flex-vs-solo-duo-queue)
- Overwatch-style LFG plus endorsements was associated with fewer disruptive matches (Blizzard-reported, 2019). — [PC Gamer](https://www.pcgamer.com/overwatchs-endorsement-system-has-cut-disruptive-behavior-by-40-percent)

### Inferences
Where the claim **holds**:
- Asymmetric urgency: the 4-stack's alternative is "no ranked" (or Normals), while the solo's alternative is "ranked alone." Supply will be the thin side of the market, and PreBot should expect more listings than takers outside peak and weekend windows.
- The outsider and blame risk is real and is the main reason fillers decline. Riot's own design decisions confirm the risk.

Where the claim is **wrong or overstated**:
- "Why would a solo join?" has concrete answers in 2026: Ranked 5s access (solo-locked), coordinated voice play, escaping random-vs-premade lobbies, Flex/Victorious/Battle Pass progress, and finding a recurring group.
- "Often a disadvantage" applies to matchmaker-forced 4+1s. With an opt-in filler, visible group info (ranks, roles, voice yes/no, mood), and reputation, much of the risk can be screened in advance.
- No evidence shows a mechanical LP or MMR penalty for being the 5th in a 5-stack.

Design implications for PreBot (inferences):
- Pitch the filler side as "play Ranked 5s / play with a voice team tonight," not "help someone out."
- Make the **group's** reputation visible to the filler: host or group endorsements after the match, no-show and kick reports, and "were you treated well?" feedback. This targets the outsider and blame fear directly (Overwatch/Krew logic).
- Show ranks, the needed role, voice requirement, and tone (chill/tryhard) on each listing so fillers can self-select. Mismatched rank gaps can mean "more difficult matchups" (Riot Support, via Pairly).
- Concentrate notification campaigns on Ranked 5s windows (Fri–Sun 16:00–04:00 TR) and peak evenings, when the pool of available solos is largest.
- Offer "add as regular / play again" after a good game to turn one-off fillers into recurring groups.

### Gaps
- No direct measurement of filler willingness or filler satisfaction on TR (or anywhere) was found. A small PreBot user survey or listing-fill-rate telemetry would be the primary evidence.
- Reddit player testimony (both positive and negative) could not be accessed. It should be reviewed manually if the report needs player quotes.
