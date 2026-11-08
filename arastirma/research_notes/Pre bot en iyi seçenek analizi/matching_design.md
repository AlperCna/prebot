# Matching, Notification & Trust Design for an On-Demand "Need 1 More" LoL Flex (TR) Discord Bot

Research date: 2026-11-08. Labels: **[DATA]** = measured or official fact, **[OFFICIAL]** = platform/law text, **[OPINION]** = design guidance, a vendor claim or an informal source, **[INFERENCE]** = my reasoning, not sourced. Each item flagged as unverified is listed under Gaps.

## 1. Matching model: who gets the slot, and broadcast vs. sequential dispatch

### Takeaway
Real LFG systems give the poster two modes: manual review of applicants (the default) and an opt-in auto-accept (WoW Premade Groups). Ride-hailing moved away from "nearest driver, first come" toward short batching windows with timed offers. For a bot filling 1 slot from a small TR pool, the best default is a hybrid. Send a small targeted batch first, expand the broadcast over time, and give the slot to the first person who accepts, with the poster able to veto. Make "poster chooses" an optional mode.

### Cited Findings
- [OFFICIAL] WoW's Group Finder API has an `autoAccept` flag. When it is true, "new applicants will be automatically invited to join the group". Otherwise the leader reviews applicants. — [warcraft.wiki.gg UpdateListing](https://warcraft.wiki.gg/wiki/API:C_LFGList.UpdateListing); [GetActiveEntryInfo](https://warcraft.wiki.gg/wiki/API_C_LFGList.GetActiveEntryInfo)
- [OFFICIAL] WoW limits auto-accept. A separate function, `CanActiveEntryUseAutoAccept`, added in 8.0.1, decides whether a listing may use it, so auto-accept is not allowed for every activity. — [warcraft.wiki.gg CanActiveEntryUseAutoAccept](https://warcraft.wiki.gg/wiki/API_C_LFGList.CanActiveEntryUseAutoAccept)
- [OFFICIAL] WoW's `CreateListing` became hardware-event protected in 7.2.0. A human click is needed to create a listing, which blocks automated listing spam. — [warcraft.wiki.gg CreateListing](https://warcraft.wiki.gg/wiki/API_C_LFGList.CreateListing)
- [OFFICIAL] Uber says it moved from instantly matching each rider with the closest driver, which "sometimes led to long wait times for others", to batched matching. It evaluates nearby riders and drivers together over a few seconds. — [Uber Marketplace: Matching](https://www.uber.com/ae/en/marketplace/matching/)
- [OPINION, unverified] Third-party system-design write-ups describe a driver accept window of about 15 s. They also describe Uber trying multi-driver offers where the first acceptance wins, a trade-off against sequential offers, which are fairer but slower. These are not Uber primary sources. — [DEV Community](https://dev.to/ishaanthedev/designing-uber-a-real-time-ride-matching-system-at-scale-pc9); [techinterview.org dispatch LLD](https://www.techinterview.org/post/3233472823/lld-ride-hailing-dispatch/?format=md)
- [OPINION] One design guide proposes offering to the top 3 candidates with staggered starts, a middle ground between broadcast and strictly sequential offers. — [techinterview.org driver matching LLD](https://www.techinterview.org/post/3233471614/lld-driver-matching/)
- [DATA, thesis] A 2025 UvA master's thesis models offer sending as an optimal-stopping problem. It reports that cancellations rise sharply over the first 30–40 matching attempts (about 5 minutes) and then level off, so most of the cost of waiting comes early. — [UvA thesis presentation](https://student.uva.nl/events/2025-presentation-master-s-thesis-yi-mao-fan-psychological-methods)

### Inferences
- [INFERENCE] Options compared:
  - **(a) FCFS broadcast with auto-accept.** Fastest fill. Highest risk of a toxic or mismatched teammate. Pings many people, but only one wins, so the rest get a "slot already filled" experience that adds to notification fatigue.
  - **(b) Poster chooses from applicants.** Best fit and trust. Slower, and the poster has to stay attentive. Applicants who are not picked feel rejected.
  - **(c) Sequential or batched timed offers, ride-hailing style.** Fewer wasted pings and fairer to the pool. Slow if each timeout is long.
- [INFERENCE] Recommended default: batched waves.
  - Wave 1: DM the top 3–5 scored candidates with a 60–90 s accept window. The 15 s ride-hailing window is too short for players who may be alt-tabbed.
  - Wave 2: if nobody accepts, widen to about 10 candidates. Then post a channel or role mention.
  - The first acceptor gets the slot, and the poster has a short veto window of about 30 s.
  - Offer "manual select" as a per-post toggle, the way WoW offers autoAccept as an option.
- [INFERENCE] Copy WoW's hardware-event idea. Require an interactive button press to create a post, to prevent macro-spam.

### Gaps
- I found no Uber or Lyft primary engineering post that gives offer timeout values or a sequential-vs-broadcast A/B result. The 15 s figure is third-party only.
- I found no published fill-time data for WoW auto-accept vs. manual applicant review.

## 2. Matching criteria and progressive widening

### Takeaway
Rank barely matters for legality in Flex. Riot allows any Iron–Diamond mix, and only Master+ parties require everyone to be Emerald or higher. Rank band is therefore a quality preference, not a hard constraint. The standard technique is to start strict and widen thresholds as the request ages. Microsoft's TrueMatch research found that starting pre-expanded in low-population regions halved wait times, which is relevant for a small TR pool.

### Cited Findings
- [OFFICIAL] Riot: Flex has no rank restrictions for Diamond and below. To play with Master/Grandmaster/Challenger, everyone else must be at least Emerald. — [Riot Support: Ranked Tiers, Divisions, and Queues](https://support.riotgames.com/league-of-legends/gameplay/ranked-tiers-divisions-and-queues); conflicting one-rank range claimed by a less reliable aggregator ([strafe.com](https://www.strafe.com/news/read/ranked-restrictions-league-of-legends-2025-guide-causes-fixes-queue-rules/)). Rely on Riot's page.
- [DATA, secondary] Reports describe 2026 ranked changes: harsher dodging penalties, LP protection and autofill updates. — [GosuGamers](https://www.gosugamers.net/lol/news/77660-league-of-legends-will-roll-out-2026-ranked-changes-soon-with-harsher-dodging-and-lp-protection); [AltChar](https://www.altchar.com/game-news/league-of-legends-ranked-2026-changes-explained-autofill-updates-aegis-of-valor-dodging-rules-and-faster-queue-times-asZ9b4U04yVM)
- [OFFICIAL] PlayFab matchmaking "expansion": a rule loosens its threshold while a ticket waits. Example: a skill difference of 0.2 that grows by 0.1 every 5 s, up to 0.5. — [Microsoft Learn: How matchmaking works](https://learn.microsoft.com/en-ca/xbox/playfab/multiplayer/matchmaking/how-matchmaking-works)
- [OFFICIAL] Apple GameKit advises: "widen the skill range as the wait time, or age of the match request, increases." — [Apple GameKit docs](https://developer.apple.com/documentation/gamekit/finding-players-with-similar-skill-levels.md)
- [DATA, vendor research] Microsoft TrueMatch: "Previous systems expand the allowance only after a player has waited a long time... TrueMatch starts the search pre-expanded, cutting wait time in half in low population regions." Associated with Josh Menke's GDC 2020 talk. — [Microsoft Research: TrueMatch](https://www.microsoft.com/en-us/research/project/truematch/)
- [OPINION, practitioner] The Awesomenauts developer warns that a naive growing search range in a small player pool "probably results in really long waiting times... or even in never finding a full match at all." — [Game Developer: Designing efficient systems for expressive matchmaking](https://www.gamedeveloper.com/marketing/designing-efficient-systems-for-expressive-matchmaking)

### Inferences
- [INFERENCE] Treat criteria in three groups:
  - **Hard filters** (never relaxed): server = TR; queue = Flex; not blocked by either side; not currently in game or on cooldown; age bracket (see section 7); and, only if any party member is Master+, Emerald or higher (Riot rule).
  - **Strong preferences:** the missing role, then the poster's rank band, then voice/language (TR vs EN).
  - **Weak preferences:** reliability score, past positive co-play.
- [INFERENCE] Suggested score: `role_fit (0/0.5 for "fill"/1) × w1 + rank_proximity × w2 + voice_match × w3 + reliability × w4`.
  - Suggested widening schedule: t=0 means exact role and ±1 tier.
  - t=2 min: ±2 tiers, "fill" players accepted.
  - t=5 min: any Diamond-or-below rank, voice becomes optional.
  - Because TR is a small pool, consider TrueMatch-style pre-expansion: start at ±1 tier plus "fill" already allowed.
- [INFERENCE] Rank width needs judgment. Riot allows any mix, but large gaps create in-game frustration. Show the rank gap transparently to the poster instead of hiding candidates.

### Gaps
- I found no published LoL-specific data on how rank gap inside premade Flex parties affects win rate or toxicity.
- I could not access GDC Vault talk content (Menke 2018/2020, Overwatch 2017) to confirm their widening schedules.

## 3. The "available now" pool: TTL, re-confirmation, auto-removal

### Takeaway
I found no hard evidence for an optimal availability TTL. LFG Hub's "recent players" window (played in the last 5 days, online now) is the closest analogue. Discord can detect "Playing League of Legends" only through the privileged, very heavy GUILD_PRESENCES intent, which needs approval once the bot is in 100+ servers. A 60-minute TTL with a one-tap extend prompt is a reasonable default, but it rests on inference, not data.

### Cited Findings
- [OPINION, bot listing] LFG Hub's Smart Ping targets "online players who played the game in the past 5 days", combining presence with recency. Members can opt out. — [top.gg LFG Hub (es)](https://top.gg/es/bot/1163043953273217044); [top.gg LFG Hub](https://top.gg/bot/1163043953273217044)
- [OFFICIAL/secondary docs] GUILD_PRESENCES is privileged. It delivers online/idle/DND/offline status and activities such as "Playing". Bots in 100+ servers must go through Discord verification and an intent application. Declaring the intent without enabling it closes the connection with code 4014. — [discord.js guide: Gateway Intents](https://discordjs.guide/popular-topics/intents); [JDA GatewayIntent](https://docs.jda.wiki/net/dv8tion/jda/api/requests/GatewayIntent.html)
- [OFFICIAL] Discord warns presence is "a very heavy intent" (about 99% of gateway traffic). Its guidance says many bots don't need privileged intents, because REST and interaction payloads cover on-demand data. — [Discord docs: You might not need a privileged intent](https://docs.discord.com/developers/gateway/you-might-not-need-a-privileged-intent); [discord.js guide](https://discordjs.guide/popular-topics/intents)

### Inferences
- [INFERENCE] Options:
  - **30 min:** fresh, but users must re-flag often.
  - **60 min:** about one or two Flex games plus queue time.
  - **120 min:** stale by the end, and pings go to people who left.
- [INFERENCE] Recommended default: 60 min. At T-10 min, send one ephemeral or DM "Still available? [+60 min] [Remove]" prompt. Auto-remove when:
  - (a) the user accepts a slot, with re-entry after a cooldown or a "back" button;
  - (b) presence shows League in game, only if the bot has the presence intent;
  - (c) the user goes offline, if presence is available;
  - (d) the TTL expires.
- [INFERENCE] Without the presence intent, use proxies. Remove a user after they accept a match, and assume a roughly 35–45 min game before re-prompting.
- [INFERENCE] The Riot API (spectator/active-game endpoint) could detect "in game". Weigh this against Riot policy on processing other players' data and the API key class (see section 7).

### Gaps
- I found no published data on TTL length vs. stale-ping rate for "available now" features (Xbox LFG, Steam, Bumble BFF and others were not reached in this session's budget).
- I did not verify the exact current Discord verification threshold or approval criteria for the presence intent against Discord's primary support article.

## 4. Notification fatigue: frequency caps, DM vs. channel vs. role mention

### Takeaway
The direction of the evidence is consistent: more pushes lead to more opt-outs. But good per-week thresholds are not public, and at least one dataset (Pushwoosh, news apps) found almost no correlation. The most concrete gaming precedent is LFG Hub's Smart Ping. It pings only recent, online players, never the same person twice within 8 hours, and lets members opt out. That is a sound default cap to adapt.

### Cited Findings
- [OPINION, bot listing] LFG Hub Smart Ping "never pings the same player twice within 8 hours", targets recent players only, and is optional. — [top.gg LFG Hub](https://top.gg/bot/1163043953273217044); [top.gg LFG Hub (it)](https://top.gg/it/bot/1163043953273217044)
- [DATA, aggregated] MIDiA Research, using data from 5 notification vendors covering more than 65,000 apps: "the greater the frequency of push campaigns the higher that opt out rates will go" (directional, no threshold). — [MIDiA Research](https://www.midiaresearch.com/blog/chart-of-the-week-over-half-of-app-user-enable-push-notifications)
- [DATA, contradicts] A Pushwoosh analysis of 213 news apps found almost no correlation between push frequency and opt-in rate (r = −0.07). — [Pushwoosh](https://www.pushwoosh.com/blog/push-notifications-news-apps/)
- [DATA, vendor] Airship: app users who received at least one push in their first 90 days had about 3× higher retention than users who received none. Relevant, well-targeted pings help. — [Airship App Retention Benchmarks (PDF)](https://grow.urbanairship.com/rs/313-QPJ-195/images/WP_App_Retention_Rates_Benchmarks.pdf)
- [OPINION, informal n=14] In a Braze staff survey, 25% said they opted out because of too many notifications. The authors say the results are not statistically significant. — [Braze](https://www.braze.com/resources/articles/opt-out-of-push-notifications-why-users-do-it)
- [DATA, HCI] Pielot et al. (2014) found a median of 63.5 notifications per day per user in a one-week in-situ study of 15 users. — [MobileHCI 2014 PDF](https://ic.unicamp.br/~oliveira/doc/MHCI2014_An-in-situ-study-of-mobile-phone-notifications.pdf)
- [DATA, HCI] Pielot & Rello (2017), "Productive, Anxious, Lonely", a 24 h no-notification challenge. Participants reported better focus, yet all of them re-enabled notifications afterwards. People value relevant notifications but suffer from volume. — [Pielot & Rello 2017 (arXiv)](https://www.arxiv.org/pdf/1612.02314v1)

### Inferences
- [INFERENCE] Channel options:
  - **DM:** highest attention and most intrusive. Use it only for opted-in "available now" users with a cap.
  - **Channel post without mention:** zero fatigue, low reach.
  - **@role mention:** broad reach, and fatigue spreads to everyone in the role. Use it as the escalation step only, rate-limited per server, e.g., at most 1 role ping per 15–30 min.
- [INFERENCE] Recommended defaults:
  - Only users who opted in (pressed "available") get DMs, at most 1 DM invite per user per 15–20 min while available.
  - Opted-in users get at most about 6 invites per availability session.
  - "Passive" members (recent players, not flagged available) follow an LFG-Hub-style rule: at most 1 ping per 8 h, with a one-click "stop pinging me / snooze 24 h" on every ping.
  - Track opt-out rate per ping as the guardrail metric.
- [INFERENCE] Every invite message should carry a mute/snooze button. This reduces hard opt-outs (leaving the server or role).

### Gaps
- I found no published data on Discord-specific DM or role-mention opt-out rates, and no OneSignal frequency/opt-out benchmark in this pass.
- I found no peer-reviewed study that sets an optimal per-day ping cap for real-time matching invites.

## 5. Post lifetime, expiry and stale posts

### Takeaway
I could not verify the commonly cited "WoW auto-delists after 30 minutes" or any FFXIV Party Finder duration from primary sources in this pass. The design principle still holds. A "need 1 now" post loses value within minutes. The cancellation data in section 1 points the same way: most of the cost accrues in the first ~5 min. Posts should expire quickly and stale posts should be cleaned up visibly.

### Cited Findings
- [DATA, thesis] Cancellations rise sharply over the first 30–40 matching attempts (about 5 minutes), then level off. — [UvA thesis](https://student.uva.nl/events/2025-presentation-master-s-thesis-yi-mao-fan-psychological-methods)
- [OFFICIAL] WoW lets the leader update a listing in place with `UpdateListing` (same settings, including autoAccept), instead of re-posting. — [warcraft.wiki.gg UpdateListing](https://warcraft.wiki.gg/wiki/API:C_LFGList.UpdateListing)
- [OPINION] FFXIV Party Finder guides describe category caps (50 listings per page) and say listing conventions change across patches. They do not state a duration. — [Thonky PF guide](https://www.thonky.com/final-fantasy-xiv/party-finder); [Console Games Wiki](https://ffxiv.consolegameswiki.com/wiki/Party_Finder)

### Inferences
- [INFERENCE] Recommended default: a "need 1 more" post lives 15 min.
  - At 10 min, the poster gets "Still looking? [+15] [Close]".
  - Hard maximum 45 min.
  - The post closes automatically when filled. Edit the original message to "FILLED" or "EXPIRED" and disable its buttons rather than deleting it, so the channel history stays understandable.
  - Allow edit-in-place (role/rank change) instead of re-posting, like WoW's UpdateListing, to prevent duplicate spam.
  - Limit each poster to one active post.

### Gaps
- WoW's 30-minute delist and FFXIV's PF duration are unverified. Primary patch notes or Blizzard/Square Enix support pages were not found in this pass.

## 6. No-show / flake handling after accepting; reputation design

### Takeaway
Gaming evidence favors positive, low-stakes reputation over public negative ratings. Overwatch's endorsements plus LFG coincided with abusive-chat matches falling 26.4% (Americas). Riot's Honor saw early reported-behaviour changes, though critics question the metric. For flakes, the established pattern is a short confirm window plus automatic promotion of the next candidate, with reliability kept as a private soft score.

### Cited Findings
- [DATA, official via press] Jeff Kaplan, July 2018: after endorsements and LFG launched, competitive matches with abusive chat fell 26.4% (Americas) and 16.4% (Korea). Daily players behaving abusively fell 28.8% and 21.6%. These were early figures. — [PC Gamer](https://pcgamer.com/overwatch-endorsements-and-looking-for-group-features-are-already-working); [Blizzard Watch](https://blizzardwatch.com/2018/07/19/endorsements-lfg-bringing-player-toxicity-overwatch)
- [DATA, official via press] Later, Blizzard's Natasha Miller said at GDC that disruptive-behaviour matches dropped by about 40%. — [PC Gamer](https://www.pcgamer.com/overwatchs-endorsement-system-has-cut-disruptive-behavior-by-40-percent/)
- [DATA/OPINION] Riot's initial Honor results (2012) reported global changes in reported bad behaviour in the first week or ten days. — [Surrender@20](https://www.surrenderat20.net/2012/10/initial-honor-results.html). A critique argues that the falling report rate is not proof that behaviour improved. — [Game Developer](https://www.gamedeveloper.com/business/-quot-honor-pls-quot-or-why-riot-s-behaviour-policing-techniques-don-t-really-work)
- [DATA, secondary] Riot reworked Honor in 2018 and later added positive-play rewards and champ-select reporting. Champ-select reports are about 3.2% of reports. — [Dot Esports](https://dotesports.com/league-of-legends/news/riot-working-to-improve-league-behavioral-systems); [RealSport101](https://realsport101.com/article/league-of-legends-behavioral-system-changes-champ-select-reporting-new-honor-rewards)
- [OPINION, secondary blog] A 2026 analysis cites Riot saying most flagged players are "net neutral to positive", which argues against permanent public labels. — [twoaveragegamers.com](https://www.twoaveragegamers.com/is-league-of-legends-toxic-an-honest-answer-backed-by-data-2026/)
- [OPINION, unverified] Ride-hailing designs return an unanswered offer to the pool and move to the next driver when the accept window expires. — [techinterview.org](https://www.techinterview.org/post/3233472823/lld-ride-hailing-dispatch/?format=md)

### Inferences
- [INFERENCE] Recommended flow:
  1. Accept, which reserves the slot.
  2. Within 2 min, a ready-check DM asks for the Riot ID to send in an invite ("I'm in client, invite sent?"). The poster confirms "joined" or "no-show".
  3. If there is no confirmation within 3–5 min, release the slot, auto-promote the next best candidate from the same wave or a waitlist, and notify the poster.
- [INFERENCE] Reliability is a private soft score, e.g., an exponentially decayed show rate over the last 10–20 accepts. It is used only to rank or lower candidates, never shown publicly.
  - Repeated no-shows trigger a temporary cooldown (e.g., 30 min, then 24 h).
  - Use optional positive "endorse / would play again" feedback (Overwatch model). Do not use public 1–5 star ratings: retaliation risk, and they are personal data under KVKK.
  - Allow forgiveness: the score decays over time.

### Gaps
- I found no published accept→show rates from gig platforms or gaming LFG tools in this pass. DoorDash and Uber acceptance-rate policies were not fetched.
- I found no primary Blizzard post on long-term (post-2019) endorsement effects.

## 7. Trust & safety minimum: blocks, reports, minors, KVKK, Riot policy

### Takeaway
Riot's developer policy (last updated May 29, 2025) explicitly bans alternatives to the ranked ladder, including MMR/ELO calculators, and bans de-anonymizing players. The bot must therefore use only official tier/division, never estimate MMR, and never reveal hidden identities. KVKK (Law 6698) requires notice at collection (Art. 10), a lawful basis (Art. 5), erasure when the purpose ends or on request (Art. 7/11), and responses to requests within 30 days, free of charge (Art. 13). Blocks and reports are table stakes.

### Cited Findings
- [OFFICIAL] Riot: "Products cannot create alternatives for official skill ranking systems such as the ranked ladder. Prohibited alternatives include MMR or ELO calculators." — [Riot Developer Policies](https://developer.riotgames.com/policies.html)
- [OFFICIAL] Riot: "Products cannot de-anonymize players who cannot reasonably be identified from visible information." — [Riot Developer Policies](https://developer.riotgames.com/policies.html)
- [OFFICIAL] Riot: all products must be registered and audited through the Developer Portal. The policy page was last updated May 29, 2025. — [Riot Developer Policies](https://developer.riotgames.com/policies.html)
- [OFFICIAL] KVKK Art. 3: explicit consent means "freely given, specific and informed consent". Personal data is "any information relating to an identified or identifiable natural person". Discord IDs and Riot IDs linked to a person therefore count. — [KVKK law text (EN), kvkk.gov.tr](https://www.kvkk.gov.tr/Icerik/6649/Personal-Data-Protection-Law)
- [OFFICIAL] KVKK Art. 4: data is stored only for the period required by legislation or the purpose. Art. 5: processing needs explicit consent unless an exception applies, such as necessity for the controller's legitimate interests without harming the subject's rights. — [kvkk.gov.tr](https://www.kvkk.gov.tr/Icerik/6649/Personal-Data-Protection-Law)
- [OFFICIAL] KVKK Art. 7: erase, destroy or anonymize data when the reasons for processing no longer exist, either ex officio or on request. Art. 11 rights include learning whether data is processed, correction, erasure, and objecting to outcomes based solely on automated analysis. The last one matters for an automated reliability score. — [kvkk.gov.tr](https://www.kvkk.gov.tr/Icerik/6649/Personal-Data-Protection-Law)
- [OFFICIAL] KVKK Art. 10: inform data subjects at collection about the controller's identity, the purpose, recipients, the method and legal basis, and their rights. Art. 12: take technical and organizational security measures and report breaches "within the shortest time". — [kvkk.gov.tr](https://www.kvkk.gov.tr/Icerik/6649/Personal-Data-Protection-Law)
- [OFFICIAL] KVKK Art. 13: answer data-subject requests "at the latest within thirty days and free of charge". Art. 9 (amended 2024): transfers abroad need an adequacy decision or safeguards such as Board-approved standard contracts, with a limited occasional-transfer exception. This is relevant when hosting the bot DB outside Turkey or relying on Discord/US servers. — [kvkk.gov.tr](https://www.kvkk.gov.tr/Icerik/6649/Personal-Data-Protection-Law)
- [OFFICIAL/secondary] If a request is not answered, the subject may complain to the KVK Board after the 30-day period, within 60 days of the application (Art. 14). — [hukukihaber.net](https://www.hukukihaber.net/kisisel-verileri-koruma-kurulunun-kurula-sikyette-bulunma-suresi-ile-veri-ihlali-bildirimine-iliskin-kamuoyu-duyurulari-hakkinda-degerlendirme)
- [OFFICIAL] Art. 16: register with the Data Controllers' Registry (VERBİS) before processing, subject to Board exemptions. The law text has no child-specific article. Art. 5(2)(b)/6(3)(c) cover persons whose consent is not legally valid. KVKK publishes separate "KVKK Çocuk" guidance materials. — [kvkk.gov.tr](https://www.kvkk.gov.tr/Icerik/6649/Personal-Data-Protection-Law)

### Inferences
- [INFERENCE] Minimum T&S feature set:
  - **Mutual block list**, enforced as a hard filter in both directions and invisible to the blocked user.
  - **/report** with a reason code, routed to server mods.
  - **Automatic exclusion from matching** for a user after N independent reports from distinct users in X days, pending mod review.
  - **Minors:** collect a self-declared age bracket (<18 / 18+) at opt-in, and by default don't match 18+ posters with <18 users unless both opt in. Discord's own ToS minimum age is 13. Store only the bracket, not a birthdate.
  - **Data minimization:** store Discord user ID, Riot ID, self-declared role, rank and voice preferences, block list, reliability counters, and timestamps.
  - **Data handling and rights:** show a short Turkish aydınlatma metni (privacy notice) on first opt-in. Provide /verilerimi-sil (delete my data) for immediate deletion, with an answer well within 30 days. Purge inactive users after, e.g., 12 months. Document where data is hosted (Art. 9).
- [INFERENCE] Riot compliance:
  - Show official tier/division only. No "estimated MMR" or "real Elo" shown or used.
  - Don't reveal identities of players met in champ select or ranked lobbies.
  - Look up data only for players who registered themselves. Never DM or contact players discovered only through the API.
  - Register the product in the Riot Developer Portal.
- [INFERENCE] The reliability score is automated profiling. Under Art. 11 users can object to outcomes based solely on automated analysis, so give an appeal path to a human mod.

### Gaps
- The Riot policy page as fetched does not explicitly address messaging or contacting players found via the API, nor data retention. The "no messaging players found via API" constraint could not be confirmed verbatim. Check the Riot General Policies / API Terms directly.
- I did not verify VERBİS exemption thresholds for small non-commercial controllers, or KVKK Board guidance on children's consent age, in this pass.
- I did not fetch Discord's Developer Policy requirements (privacy policy, data deletion) for bots.

## 8. Metrics to track

### Takeaway
I found no LFG-specific metric standard. The ride-hailing and matchmaking literature centres on wait time vs. match quality, and the notification literature centres on opt-out as a guardrail. The core dashboard should balance speed (time-to-fill), success (fill rate, show rate) and health (opt-outs, repeat usage).

### Cited Findings
- [OFFICIAL] Uber frames batching as reducing long waits for some riders while keeping overall efficiency. Wait-time distribution, not only the mean, is the target. — [Uber Marketplace: Matching](https://www.uber.com/ae/en/marketplace/matching/)
- [OFFICIAL] PlayFab and GameKit both frame matchmaking as trading threshold strictness against wait time. — [Microsoft Learn](https://learn.microsoft.com/en-ca/xbox/playfab/multiplayer/matchmaking/how-matchmaking-works); [Apple GameKit](https://developer.apple.com/documentation/gamekit/finding-players-with-similar-skill-levels.md)
- [DATA] TrueMatch reports wait time as its headline outcome, halved in low-population regions. — [Microsoft Research](https://www.microsoft.com/en-us/research/project/truematch/)
- [DATA] Opt-out rate rises with push frequency (directional). — [MIDiA](https://www.midiaresearch.com/blog/chart-of-the-week-over-half-of-app-user-enable-push-notifications)

### Inferences
- [INFERENCE] Metric set, logged per post:
  - **Time-to-fill:** report p50/p90, not only the mean.
  - **Fill rate:** filled ÷ posted, also split by role and hour.
  - **Pool size:** available users at post time and those matching after hard filters.
  - **Funnel:** invites sent, accept rate, accept→show rate, no-show rate.
  - **Widening:** the stage at which each fill happened (shows whether criteria are too strict).
  - **Repeat usage:** posters and acceptors returning within 7 and 30 days.
  - **Notification health:** pings per user per day, snooze/opt-out rate per 100 pings, blocks/reports per 100 matches.
- [INFERENCE] Pool size at post time is the main predictor to watch. If p90 time-to-fill is high when pool <10, rely on earlier role-ping escalation and pre-expanded criteria.

### Gaps
- I found no public benchmark values for LFG fill rate or time-to-fill (e.g., from LFG Hub, Discord LFG bots or WoW Premade Groups).
