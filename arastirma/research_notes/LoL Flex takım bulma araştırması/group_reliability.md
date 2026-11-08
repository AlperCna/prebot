# Forming Reliable, Persistent Multiplayer Groups: Behavioral & Community-Design Evidence (target: LoL Ranked Flex 5-stacks, small Discord bot)

Evidence labels used throughout: **[DATA]** = quantitative or peer-reviewed study / official developer data; **[DEV-CLAIM]** = developer statement with numbers but not independently verifiable; **[QUAL]** = qualitative or observational academic work; **[ANECDOTAL]** = player or organizer posts, blogs, or vendor benchmarks without clear methods.

Scope note: research used about 17 tool calls. Several primary sources (Bungie blog, Raid-Helper docs, Riot GDC slides) could not be fetched in full. Those gaps are listed explicitly rather than filled from memory.

## Q1. Why LFG matching fails socially: no-shows/ghosting, churn, toxicity, one-off interactions

### Takeaway
I found no public show-up-rate data for game LFG tools such as Fireteam Finder, WoW Group Finder, or FFXIV PF. The closest quantitative proxy is general event data: the median no-show for free events is about 28%, and for small free events about 37%. The strongest game-specific evidence says pickup groups with strangers are instrumental and short-lived. Playing with friends is the single strongest retention factor in LoL.

### Cited Findings
- **[DATA] Event no-show baseline.** PheedLoop's check-in analysis of more than 1,070 live events found a median no-show rate of about 20%. The median was 28% for free events and 17% for paid events. Small free events (10–49 registrants) had a median of about 37%, and large free events (500+) about 24%. — [PheedLoop Event Data Lab Report #05](https://pheedloop.com/blog/event-data-lab-report-5)
  - Relevance: a free, small "5 people at 21:00" Flex session is structurally the worst case in this data (free plus small).
- **[ANECDOTAL] Meetup-style free events.** One organizer reported 175 RSVPs and about 75 attendees, roughly 43% attendance. Unsourced vendor benchmarks claim 30–50% no-show for free events. — [jpmens.net "To meet up or not to meetup"](https://jpmens.net/2019/09/14/to-meet-up-or-not-to-meetup/); [Nunify event attendance rate](https://nunify.com/blogs/event-attendance-rate); [TicketFairy no-show estimator](https://ticketfairy.com/tools/no-show-rate-estimator)
- **[ANECDOTAL/unverified detail] Fees as a commitment device.** A 2026 Skift Meetings piece is headlined "The $100 Experiment That Slashed No-Show Rates to Just 10%". I saw only the headline. Its method and baseline are unverified. — [Skift Meetings](https://meetings.skift.com/2026/09/02/the-100-experiment-that-slashed-no-show-rates-to-just-10/)
- **[DATA] Friends are the strongest LoL retention factor.** A CSCW 2014 study (Shores et al., GroupLens/University of Minnesota) used 18 million LoL matches over 3 months from Duowan (China). Playing with friends made both new and experienced players more likely to play another match. It kept newer players from taking long breaks. For experienced players, it was *the only factor* that kept them from quitting for good. — [GroupLens blog summary of CSCW 2014 paper](https://grouplens.org/blog/deviant-behavior-in-league-of-legends/)
- **[DATA] Toxicity and churn for newcomers.** In the same study, non-toxic newer players were more likely to take a break after games with toxic teammates. Ranked matches had higher toxicity indices than normals. Carry roles predicted higher toxicity than support or tank roles. — [GroupLens](https://grouplens.org/blog/deviant-behavior-in-league-of-legends/)
- **[DATA] Premades can be toxic to outsiders.** Players queued with friends got *more* negative feedback from their non-friend teammates than the authors expected. — [GroupLens](https://grouplens.org/blog/deviant-behavior-in-league-of-legends/)
  - Relevance: a partial premade (e.g., 3 friends plus 2 LFG fills, the typical Flex scenario) is a known friction point for the fills.
- **[DATA] Losing drives toxicity reports.** In Kwak, Blackburn & Han (CHI 2015), 10M+ Tribunal reports on 1.46M toxic players showed that reported matches had a win ratio "clearly below" 50%. For intentional feeding and assisting the enemy, the win ratio was under 15%. The paper did *not* analyze premades or friends, citing lack of data. — [arXiv 1504.02305 (CHI'15)](https://arxiv.org/abs/1504.02305); [ar5iv full text](https://ar5iv.labs.arxiv.org/html/1504.02305)
- **[DEV-CLAIM] Toxicity is mostly ordinary players having bad days.** Riot (Jeffrey Lin, GDC 2013) found severe, persistent toxicity rare. Much of the toxicity players see comes from normally fine players having a "bad day", and negativity spreads by a ripple effect. — [Game Developer: GDC Riot experimentally investigates online toxicity](https://gamedeveloper.com/design/gdc-riot-experimentally-investigates-online-toxicity); [GDC Vault session](https://gdcvault.com/play/1017940/The-Science-Behind-Shaping-Player)
- **[QUAL] PUGs are instrumental and one-off.** Eklund & Johansson (DiGRA 2010) did participant observation of WoW pickup groups. Communication was held to a minimum and dungeons were run at high speed, so interaction was mainly instrumental. Sociability was higher when several guildmates were in the group. — [DiGRA: "Social Play? A study of social interaction in temporary group formation (PUG) in WoW"](https://dl.digra.org/index.php/dl/article/view/504)
- **[QUAL] Automated cross-server Dungeon Finder changed temporary-group sociability.** An Eludamos (2013) article uses the Dungeon Finder as a case where the tool "radically changed the premises" for temporary collaboration. — [Eludamos Vol 7 No 1](https://eludamos.org/index.php/eludamos/article/download/vol7no1-2/7-1-2-html?inline=1)
- **[DEV-CLAIM] Blizzard acknowledged that automation reduced meeting people.** The Warlords of Draenor lead designer described the new Group Finder as a step back toward WoW being "a place where you can actually meet new people". — [PCGamesN](https://pcgamesn.com/world-of-warcraft-warlords-of-draenor/warlords-of-draenor-group-finder-will-return-world-of-warcraft-to-being-a-place-where-you-can-actually-meet-new-people)
- **[DEV-CLAIM] Bungie rejects random matchmaking for hard content.** Bungie said random pairing "does not work" for Destiny's hard activities. It built Guided Games, where a clan opens a slot for a solo player, and later an in-game Fireteam Finder that replaced web/app LFG. — [Stevivor on clans and Guided Games](https://stevivor.com/news/destiny-2s-matchmaking-just-got-whole-lot-easier-thanks-clans-guided-games); [CasualGamer Fireteam Finder guide](https://casualgamer.com/d2-fireteam-finder-guide/)
- **[ANECDOTAL] Guided Games had a supply-side problem.** Forum posts say hosts (clans) had little or no incentive to open slots, and one post reports waiting over an hour for a raid match. — [Bungie.net forum thread](https://bungie.net/fr/Forums/Post/260113224?page=0); [Destructoid](https://www.destructoid.com/?p=432035)

### Inferences
- The failures form a chain. A free event plus a small group (5) plus strangers predicts the worst no-show rates. The first loss with strangers raises toxicity, which drives newcomers to churn before any social tie forms. A bot should optimize for *repeat co-play with the same people* (tie formation), not for one-off match fill.
- Flex 5-stacks are fragile in a specific way. One no-show kills the whole session, because Flex needs exactly 1, 2, 3, or 5, not 4. With a 20–37% per-person no-show baseline and independent absences, the chance that all 5 show is only about 0.63⁵–0.8⁵ ≈ 10–33%. This is a back-of-envelope calculation, not measured data. It is the core argument for a bench/substitute mechanism and for over-subscribing a slot (6–7 RSVPs for 5 seats).
- Partial premades (friends plus fills) carry a known risk of in-group/out-group toxicity. A bot could prefer forming whole 5-stacks from a pool over attaching strangers to an existing 3-stack, or explicitly label "existing duo/trio + fills".

### Gaps
- I found no published show-up or no-show rates for Destiny 2 Fireteam Finder, WoW Group Finder or Raid scheduling, FFXIV Party Finder, Valorant or Overwatch LFG, or Discord LFG experiments. Bungie's official design-blog text on Fireteam Finder could not be retrieved, and no Bungie no-show data turned up.
- I found no peer-reviewed data on guild raid attendance rates or what improved them.
- Discord's own LFG experiments: nothing found in this pass.
- Riot's claim that "players in premades are less toxic / report less" could not be verified. Kwak et al. explicitly did not analyze friends.

## Q2. Reliability/reputation mechanics: what works, what backfires

### Takeaway
Lightweight *positive* reputation (Overwatch endorsements) is the best-documented case: 40% fewer matches with disruptive behavior, with 50–70% of players using it. That figure is a developer claim, and endorsements launched together with LFG. Positive reputation degrades into a skill signal if users can award it for performance. Riot hit exactly this with Honor. For no-shows specifically, the evidence favors commitment devices (paid or at-stake commitments, reminders) over after-the-fact penalties, but direct gaming data is thin.

### Cited Findings
- **[DEV-CLAIM, moderately strong] Overwatch endorsements plus LFG.** At GDC 2019, Blizzard research scientist Natasha Miller reported a 40% decline in matches with disruptive behavior, measured by reports per match. The window ran from the launch of endorsements *and* LFG (June 2018). 50–70% of players had used endorsements. A player survey indicated people perceived endorsements as the cause. Weeks after launch, Jeff Kaplan had cited abusive chat in competitive down more than 26% (Americas) and 16% (Korea). — [PC Gamer](https://www.pcgamer.com/overwatchs-endorsement-system-has-cut-disruptive-behavior-by-40-percent); [Digital Trends](https://digitaltrends.com/gaming/overwatch-toxicity-down-40-percent-thanks-to-endorsement-system); [MCV/UK](https://mcvuk.com/overwatchs-endorsement-system-has-reduced-antisocial-behaviour-says-blizzard/); [Byteside](https://byteside.com/2019/03/blizzard-says-overwatch-toxicity-down-40-percent-endorsements-positivity)
  - Caveat: this is a confounded before/after with no published control group. LFG launched simultaneously, so the causal split between endorsements and LFG is unknown.
- **[DEV-CLAIM] Riot Honor drifted into a skill award.** Riot's Behavioral Systems team (Aug 2024) said Honor had become a way to acknowledge strong play rather than good behavior. They also said withholding seasonal rewards for bad behavior "wasn't having the intended impact". The change: honor multiple teammates and enemies. — [Riot /dev: Honor and Behavioral Systems Update (Aug 22, 2024)](https://www.leagueoflegends.com/en-us/news/dev/dev-honor-and-behavioral-systems-update/)
- **[DEV-CLAIM] Riot is shifting toward repairing harm for victims.** Riot reports 13.5M "transgressions repaired" and 40.8M awards given, e.g., LP consolation and autofill protection for victims of disruptive players. Its griefing detection was only about 70% accurate against a 95% target. — [Riot /dev (2024)](https://www.leagueoflegends.com/en-us/news/dev/dev-honor-and-behavioral-systems-update/)
- **[DEV-CLAIM] Honor 2017 design.** All players start at level 2 and gain honor for being honored or simply well-behaved. Honor resets each season, and bad behavior loses levels. — [RiftFeed Honor explained](https://riftfeed.gg/more/honor-system-explained)
- **[DEV-CLAIM] Honor gaming in 2012.** At the 2012 launch, a week of data showed 0.21% of players reported for honor trading and 0.63% actively trying to trade honor. This is small but non-zero collusion. — [Surrender@20 "Initial Honor Results"](https://www.surrenderat20.net/2012/10/initial-honor-results.html)
- **[DEV-CLAIM] Riot's feedback experiments.** Riot ran experiments showing players their positive or negative behavior stats or prompting self-reflection, versus controls (Lin, GDC 2013). — [Game Developer](https://gamedeveloper.com/design/gdc-riot-experimentally-investigates-online-toxicity); [Scientific American](https://www.scientificamerican.com/article/can-a-video-game-company-tame-toxic-behavior/)
- **[DATA] Payment as commitment.** Paid events have a lower median no-show than free events (17% vs 28%). — [PheedLoop](https://pheedloop.com/blog/event-data-lab-report-5)
- **[ANECDOTAL] Bot-level mechanics in the wild.** Discord raid bots use bench/overflow "passes" and DM reminders 10 minutes before start (Pyrobot). Some bots post nudges only when needed (Baldio). — [Pyrobot FAQ](https://luminix-pyrobot.pages.dev/faq); [Baldio Discord bot guide](https://wowutils.com/viserio-cooldowns/guide/discord-bot)
- **[DEV-CLAIM] Guided Games (sherpa model).** Guided Games pair a clan or experienced team with solo players, but hosts lacked incentives, which created a supply bottleneck. — [Stevivor](https://stevivor.com/news/destiny-2s-matchmaking-just-got-whole-lot-easier-thanks-clans-guided-games); [Bungie forum (anecdotal)](https://bungie.net/fr/Forums/Post/260113224?page=0)

### Inferences
- **Design pattern: behavior-only endorsement.** After each session, let members give "would play again" or "showed up on time" endorsements that are explicitly *not* about skill. Make it multi-select, so you can endorse all 4 teammates (Riot's 2024 fix). Display it as a reliability badge. Never make it a single "MVP" pick, which turns into a skill award (Riot's lesson).
- **Show-up rate as the primary reputation metric.** Compute it mechanically from RSVP vs attendance (e.g., voice-channel presence or a check-in reaction at start time), not from peer votes. This avoids honor-trading collusion and is objective.
- **Prefer reward and priority over punishment.** Riot found withholding rewards ineffective. A bot can give reliable players *priority* (first pick of slots, auto-confirm, captain eligibility). It can quietly deprioritize low show-up players: put them on the bench by default, or require reconfirmation. A public "shame" list is not supported by the evidence.
- **Repair the victims.** Riot's "repair" approach transfers to bots: when a no-show breaks a stack, the bot immediately pings the bench and gives the 4 who showed up priority for the next slot.
- **Sherpa/captain systems need an incentive for the host side.** Guided Games failed on host supply. A captain role in a bot should carry visible status (badge, role color), and captains should get scheduling control.
- **Commitment devices without money.** A real-money deposit is out of scope and likely inappropriate for a Discord bot. Non-monetary stakes can mimic it: a reliability score at risk, losing priority, or a "late-cancel" window, e.g., free cancellation until 2 hours before, then it counts as a no-show. These are inferences; I found no game-specific trial data.

### Gaps
- I found no controlled study isolating the effect of RSVP reminders or no-show penalties in *gaming* sessions. Healthcare appointment-reminder literature was not searched due to budget.
- Overwatch endorsements' long-term effect after 2019, and the 2022 Overwatch 2 changes, were not examined.
- I found no data comparing penalty-first vs reward-first LFG reliability systems.
- Valorant/Riot's in-client LFG or "Riot social" features: no usable evidence found.

## Q3. Community formats that create persistent teams

### Takeaway
Repeat co-play with the same people (friendship and guild ties) measurably improves both performance and retention. Persistent formats such as guilds, in-house leagues, and fixed weekly nights work because they turn strangers into repeat partners inside one bounded community. The evidence strongly supports "friends/ties" as the mechanism. Evidence on *specific* format features (fixed schedule, MMR within community, stakes) is mostly practitioner knowledge.

### Cited Findings
- **[DATA] Friendship improves team performance.** Mason & Clauset analyzed millions of ad hoc Halo: Reach teams (about 2.45M games) plus a player survey. Friendship improved individual and team performance even after controlling for skill. Individuals did better the more friends they played with. Offline friends had a stronger effect than online friends. Players structure their play around opportunities to play with friends. — [arXiv 1203.2268 "Friends FTW!"](https://arxiv.org/abs/1203.2268v2); [MIT Technology Review](https://www.technologyreview.com/2012/03/14/187171/playing-with-friends-makes-you-better-at-video-games/); [The Register](https://www.theregister.com/2012/03/13/halo_friends_frag_harder/)
- **[DATA] Friendship does not substitute for skill.** Being friends doesn't compensate for poor play, so skill-banding still matters. — [arXiv 1203.2268](https://arxiv.org/abs/1203.2268v2)
- **[DATA] Friends drive LoL retention.** Playing with friends was the strongest LoL retention factor at all tenure levels. — [GroupLens / CSCW 2014](https://grouplens.org/blog/deviant-behavior-in-league-of-legends/)
- **[QUAL] Guildmates socialize more.** Guildmates grouping together showed more sociable interaction than PUGs. — [DiGRA Eklund & Johansson](https://dl.digra.org/index.php/dl/article/view/504)
- **[QUAL] Collaboration spans a spectrum.** Nardi & Harris (CSCW 2006, WoW) describe collaboration from lightweight encounters with strangers to highly organized groups of well-known friends. Light stranger encounters are a pathway into guilds. — [Nardi & Harris 2006 PDF](https://people.eng.unimelb.edu.au/vkostakos/courses/socialweb10F/reading_material/8/nardi06.pdf)
- **[DEV-CLAIM] Destiny 2 builds groups through clans.** Bungie routes matchmaking for hard content through clans and Guided Games, built around persistent social units. — [Stevivor](https://stevivor.com/news/destiny-2s-matchmaking-just-got-whole-lot-easier-thanks-clans-guided-games)
- **[DATA/tool] In-house leagues use an internal ladder.** LoL in-house bots run within-community MMR. mrtolkien/inhouse_bot keeps per-role TrueSkill-like ratings, a server-wide ranking/leaderboard, and match history, and it matchmakes toward a 50% predicted win rate. This is the "MMR within a community" pattern. — [GitHub mrtolkien/inhouse_bot](https://github.com/mrtolkien/inhouse_bot)

### Inferences
- The causal mechanism to design for is **tie formation through repeated exposure**, the same people again. Format features (fixed weekly slot, community ladder, captains) are the means of getting repeat exposure. A bot should *bias matching toward previous successful teammates*: people who co-played and both endorsed "would play again". It should also offer to "lock in" a recurring 5-stack once a group has played together 2–3 times.
- **Fixed weekly slot.** It turns an ad hoc LFG post into a recurring event with a known roster, which addresses both cold start (density at one time) and reliability (habit, accountability).
- **Skill banding still matters** (Mason & Clauset). Flex rules restrict premade rank spread, so the bot must enforce LoL's Flex rank eligibility. Within that range, it should prefer ties over perfect skill balance.
- **Clash.** Riot's scheduled team tournament is the closest Riot-native analog of "scheduled team play with stakes". No retention data was found in this pass.

### Gaps
- I found no published retention data on LoL Clash, collegiate/amateur leagues, CS PUG systems (e.g., FACEIT hubs or 10-man pugs), or in-house Discord communities. Organizer write-ups were not located within budget.
- Kim, Engel, Woolley, Lin, McArthur & Malone (CSCW 2017, "What Makes a Strong Team? Using Collective Intelligence to Predict Team Performance in League of Legends") is a highly relevant paper on LoL team composition and communication. I did not retrieve it in this pass, so no findings from it are claimed here.
- Social-network analysis of LoL behavior spread was announced for Riot's GDC 2015 talk, but I did not retrieve its results. — [GDC Vault 2015 session listing](https://gdcvault.com/play/1022160/contactUs)

## Q4. Signup friction and cold start

### Takeaway
Chen's "atomic network" principle applies directly: start with the smallest self-sustaining network and focus on density over size. For a Flex bot, the atomic unit is about 10–20 people in one Discord server who can play in one recurring time slot. That is enough to reliably fill one 5-stack plus a bench, before expanding to more slots or servers.

### Cited Findings
- **[Expert framework] The atomic network.** It is "the smallest network needed that can stand on its own". It should be much smaller and more specific than founders think. Focus on density, not market size. Do unscalable things to build momentum, then repeat the playbook (next team, campus, city). Uber started with "5pm at the Caltrain station at 5th and King St." Slack worked with under 10 people at one company. — [Lenny's Newsletter: "The Atomic Network" (Andrew Chen)](https://www.lennysnewsletter.com/p/atomic-network)
- **[Expert framework] Anti-network effects.** Networked products suffer anti-network effects early: a thin network actively repels users. — [Medium summary of The Cold Start Problem](https://medium.com/twosapp/the-cold-start-problem-how-to-start-and-scale-network-effects-by-andrew-chen-813f0668c70f)
- **[Expert framework] Google+ as the counterexample.** It launched big without establishing atomic networks, and failed. — [Charter: The Cold Start Problem](https://www.charterworks.com/the-cold-start-problem-andrew-chen/)
- **[DATA] Small events have the worst no-shows.** Small free events show the highest no-show rates (about 37%), so thin networks are also the least reliable. — [PheedLoop](https://pheedloop.com/blog/event-data-lab-report-5)
- **[ANECDOTAL] An empty board kills manual grouping.** A WoW forum poster tried to form groups manually in a low-density setting and formed 0 groups. — [Blizzard EU forums thread](https://eu.forums.blizzard.com/en/wow/t/random-dungeon-finder-and-looking-for-dungeon-tool/366175/128)

### Inferences
- **Constrain time:** start with 1–2 fixed weekly slots (e.g., Fri/Sat 21:00 TR time) instead of "post anytime". Concentrating demand creates density at the moment of need, which is Chen's "specific moment".
- **Constrain community:** launch in one server or community where members already share context and norms. The bot is a layer on an existing atomic network, not a stranger marketplace.
- **The hard side of a Flex LFG is the full 5.** Each extra person needed multiplies no-show risk. Mitigations: accept partial groups (duo/trio, which is valid in Flex) and merge them; over-subscribe slots; keep a standing bench.
- **Minimize signup friction:** one slash command or button to RSVP; optional profile fields (roles, rank) captured once and reused; a single "I'm in this week" button on a recurring post. Exact-friction data from LFG products was not found. This is standard product practice, unvalidated here.
- **Seed manually.** Chen's "do things that don't scale" applies: an organizer personally fills the first several sessions until habits form.

### Gaps
- I found no gaming-specific cold-start case studies, e.g., how an LFG Discord or in-house league reached critical mass, with numbers.
- I found no data on signup-form length vs completion for gaming LFG.

## Q5. Existing Discord bots (LoL in-house, scheduling): features to borrow

### Takeaway
Open-source LoL in-house bots already solve the role queue, ready check, balancing, rating, and result-validation problems. Scheduling bots (Raid-Helper, Sesh, Apollo) solve recurring events, RSVP, and reminders. No bot found combines *recurring Flex 5-stack formation* with *reliability tracking*, which is the gap a small bot could fill. That is inferred from limited search, not an exhaustive market scan.

### Cited Findings
- **[Tool] mrtolkien/inhouse_bot** (Python, Docker; unmaintained since Dec 2021):
  - one queue per channel, server-wide ratings, and `!queue <role>` role queue;
  - a duo option, `!queue role @user other_role`;
  - a ✅ reaction ready check before a game starts;
  - matchmaking that favors the longest-waiting players and targets a 50% predicted win rate;
  - per-role, TrueSkill-like ratings with a conservative MMR estimate;
  - `!won` and `!cancel`, which require validation from 6 players;
  - a player can't requeue while a game is unscored;
  - `!history`, `!rank`, `!ranking`, and admin reset/score overrides.
  - No penalty for declining a ready check is documented.
  - [GitHub](https://github.com/mrtolkien/inhouse_bot); [awesome.ecosyste.ms entry](https://awesome.ecosyste.ms/projects/github.com%2Fmrtolkien%2Finhouse_bot)
- **[Tool] DorianAarno/InHouseQueue** (Python/disnake): organizes 5v5 in-house customs for LoL, with Valorant and Overwatch variants. A hosted version is listed on top.gg. — [GitHub](https://github.com/DorianAarno/InHouseQueue); [top.gg In House Queue](https://top.gg/fr/bot/1001168331996409856)
- **[Tool] XFazze/bjorn** (discord.py): queue with team balancing, manual team creation, teams generated from a voice channel, MMR tracking, and match history for LoL customs. — [Gitea bjorn](https://gitea.com/fabian_okbro/bjorn)
- **[Tool] ynsrkn/inhouse-bot**: scrapes local custom-game match history, which requires the LoL client to be open. — [GitHub](https://github.com/ynsrkn/inhouse-bot)
- **[Tool] Raid-Helper.** A calendar/event bot with reaction or button sign-ups. Premium adds custom templates and recurring events. — [TechCult guide](https://techcult.com/how-to-use-raid-helper-bot-in-discord); [discordbotlist Raid-Helper](https://discordbotlist.com/bots/raid-helper)
- **[Tool] RaidHelper** (a separate top.gg bot): web dashboard, participant limits, sign-up management, automatic reminders and announcements. — [top.gg](https://top.gg/bot/1515083220167819385)
- **[Tool] Pyrobot.** Bench/overflow "passes" and DM reminders 10 minutes before start. — [Pyrobot FAQ](https://luminix-pyrobot.pages.dev/faq)
- **[Tool] Baldio (WoW).** Posts nudges only when someone needs one, so it isn't noisy. — [Baldio guide](https://wowutils.com/viserio-cooldowns/guide/discord-bot)

### Inferences: transferable design patterns for a small Flex Discord bot
1. **Recurring slot post:** a weekly event (e.g., "Flex Night Thu 21:00") with buttons for Main / Bench / Can't. It concentrates density (Q4) and builds habit (Q3).
2. **Role-aware RSVP:** reuse inhouse_bot's role queue. Each signup carries a primary and secondary role, and the bot assembles a 5-stack with role coverage.
3. **Over-subscribe and bench:** target 5 Main plus 2 or more Bench. Auto-promote from the bench on a cancel. This is justified by the roughly 10–33% all-5-show probability at baseline no-show rates (Q1 inference).
4. **Two-step confirmation:** RSVP early, then a ready check (✅) about 1 hour or 15 minutes before start. Treat no reaction as soft-drop and promote the bench. Inhouse_bot's ready check and Pyrobot's 10-minute DM are the precedents.
5. **Late-cancel window:** free cancel until a cutoff (e.g., T-2h), after which it counts toward a lower show-up rate. This is a non-monetary commitment device.
6. **Mechanical reliability score:** show-up percentage from RSVP vs check-in, displayed as a badge. Use it for priority (first slot pick, captain eligibility), not public shaming. Riot's 2024 note found reward-withholding ineffective; Honor drifted to skill.
7. **Post-session behavior endorsement:** multi-select "would play with again" for each teammate, separate from skill. It feeds a tie graph.
8. **Tie-biased matching:** prefer assembling players with mutual "play again" endorsements. Offer to "lock" a recurring premade after 2–3 good sessions, converting LFG into a persistent team (Mason & Clauset; Shores et al.).
9. **Captain role:** one member per stack owns the lobby invite, confirms attendance, and gets visible status. This addresses the host-incentive failure of Destiny's Guided Games.
10. **Flex eligibility guard:** validate rank spread for Flex premades, and check rank/role via the Riot API if desired. Skill banding still matters.
11. **Low-noise reminders:** a DM only to people who haven't confirmed, as in Baldio's approach.
12. **Victim repair:** if a no-show breaks a stack, give the remaining players priority for the next slot and ping the bench immediately (Riot's "repair" approach).

### Gaps
- Raid-Helper's official docs (bench, absence, and late-signup features), Sesh, and Apollo features were not verified in this pass, so their exact features are not claimed.
- Star counts, activity, and licenses of the GitHub bots were not checked.
- No existing bot was found that specifically targets LoL *Ranked Flex* 5-stack formation with reliability scoring. Absence of evidence from a limited search is not proof none exists.
