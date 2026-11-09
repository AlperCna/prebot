# Passive alert subscriptions ("Bana haber ver") vs active "I'm available now" ("Şu an müsaitim") — evidence for rebalancing PreBot's supply flow

Research date: 2026-11-09. Claim under test: "'Bana haber ver' is the real weapon but sits in the background; the most realistic supply mechanism is a passive subscription ('notify me if a listing needing a Support opens'), yet the home page and flow still push 'Şu an müsaitim'."

PreBot context used for the inferences: listings live 15 minutes, first tap wins. "Şu an müsaitim" keeps a player in the pool for 30 min to 2 h. "Bana haber ver" alerts during chosen hours, with a 30-minute cooldown.

Source-quality note: much of the marketplace and gig data below is self-reported by vendors (Instawork, Heartrunner, Airship, Pushwoosh) or comes from stated-intention surveys (Localytics, Braze). The only peer-reviewed behavioural data on how fast people react to notifications is small-sample and old (Pielot et al., data collected March 2013). Some findings were seen only in search-result summaries and their pages were not fetched; these are marked "(search summary)".

---

## 1. How do LFG / teammate-finder products and gaming platforms handle "available now" vs "notify me", and which works for "need someone in 5 minutes"?

### Takeaway
The major platforms' standalone LFG boards, which rely on players actively posting or browsing, have mostly decayed or been dropped: Overwatch's LFG was cut in OW2, and Xbox Clubs (which launched alongside Xbox LFG) is being shut down as players move to Discord. Products where the need is real and time-boxed (padel open matches, sub-finding) put a passive alert at the centre, and some charge for extra alerts. No major platform ships a native "I'm available now" presence flag that notifies others, though users keep asking for one. The evidence points to "post a need + notify a subscribed pool" as the working pattern, and to a standing "available" status as something few people keep up.

### Cited Findings
- **Overwatch:** Overwatch 1 had an in-game LFG system that Overwatch 2 did not carry over. Blizzard said "usage of the LFG system had declined over time" and that it wanted to explore new ways of forming groups — [esports.gg](https://esports.gg/news/overwatch/overwatch-2-lfg-is-unlikely-to-happen-soon).
- **Overwatch:** Dexerto reports a second reason for removing it, "social and disruptive behavior problems". A Blizzard developer (Winter) said LFG lacked protections and that the feature "hasn't been a priority so far" — [Dexerto](https://www.dexerto.com/overwatch/overwatch-2-blizzard-removed-aspects-no-sense-2161280/).
- **Overwatch:** Players dispute the low-usage claim. Forum posters say it was used heavily at lower ranks and was useful for quickly finding voice-chat players. No published usage data backs either side — [Blizzard EU forums](https://eu.forums.blizzard.com/en/overwatch/t/bring-lfg-back/37141); [Blizzard US forums](https://us.forums.blizzard.com/en/overwatch/t/please-add-lfg-back-into-ow2/890055).
- **Overwatch:** OW2 has no direct replacement for LFG — [CyberPost](https://cyberpost.co/?p=130940) (search summary).
- **Xbox:** Xbox LFG launched in 2016 alongside Clubs and was described as a gaming "want-ad" to the community — [Xbox Wire, 2016](https://news.xbox.com/en-us/2016/09/06/clubs-looking-for-group-and-more-roll-out-to-xbox-live-today-in-preview/).
- **Xbox:** By 2022 Xbox LFG was reported as "filled with spam posts by bot accounts" — [PiunikaWeb, Jul 2022](https://piunikaweb.com/2022/07/20/xbox-looking-for-group-filled-with-spam-posts-by-bot-accounts/).
- **Xbox:** In February 2026 Microsoft announced it would shut down Xbox Social Clubs. Coverage said users increasingly ignore the feature and turn to Discord. The fate of LFG posts specifically was not confirmed in what I found — [XboxEra, Feb 2026](https://xboxera.com/2026/02/14/microsoft-to-shut-down-xbox-social-clubs-this-april); [Gaming Bible, Apr 2026](https://www.gamingbible.com/news/platform/xbox/xbox-removes-popular-feature-gamers-mourn-090774-20260421).
- **Steam:** Steam has no native "looking to play" flag. Users have asked for an opt-in subscription notification so they are alerted when others want to play a game. That is a demand for passive "notify me", not for a status — [Steam Community discussion](https://steamcommunity.com/discussions/forum/10/4767710913128814889).
- **Discord:** Discord has no native LFG presence status. A user feature request asks to "flag yourself as Looking for Group" with a status colour and selected games, and I found no sign it was implemented — [Discord support community](https://support.discord.com/hc/nl/community/posts/360041631631--Suggestion-Flag-Yourself-as-Looking-for-Group).
- **Discord:** Third-party bots fill the gap. Puck "lists, pings for, and delists" LFG posts, and its settings include a default time before an LFG post is archived. This is the "listing + ping + expiry" pattern — [discordbotlist: Puck](https://discordbotlist.com/bots/puck/upvote).
- **Playtomic (padel/tennis):** "Priority alerts" send push notifications about available matches. A standard account gets one alert; Premium gets unlimited. The passive alert is something they charge for — [Playtomic player help](https://playerhelp.playtomic.com/hc/en-gb/articles/19831492072337-How-to-manage-the-notifications-from-Playtomic-App) (search summary; exact page wording not fetched).
- **Playtomic:** An open match with only 1–3 players is cancelled automatically 1 hour before start. The deadline makes the alert time-critical — [Playtomic: Cancellation policy for Open Matches](https://playerhelp.playtomic.com/hc/en-gb/articles/19831672824465).
- **Padel Mates (another padel app):** An app-store reviewer complains that notifications are noisy and wants alerts only for things like "waiting-list openings, chat messages, or relevant updates – not every single community post." The complaint is about irrelevant alerts, not alerts in general — (search summary of an App Store review, [apps.apple.com](https://apps.apple.com/gb/app/id1531797995)).
- **SubHook (amateur sports subs):** Managers post a broadcast with date, time, position needed and location, and players "browsing the feed can respond in seconds." This is a marketing claim with no data — [App Store: SubHook](https://apps.apple.com/us/app/subhook/id6760349965).
- **Spond:** Spond's "availability requests" are scheduled polls. An admin asks who can make a date, answers are sorted into Available and Unanswered, and automatic reminders go to non-responders. It is built for planning ahead, not for minutes-scale filling — [Spond help](https://help.spond.com/en/articles/25743-overview-availability-requests); [Spond blog](https://www.spond.com/news-and-blog/spond-availability-requests/).
- **TeamSnap:** An old (2011) TeamSnap blog post suggests coaches tell players they won't start if they don't mark their availability. Getting people to keep an active availability state up to date takes pressure — [TeamSnap blog](https://www.teamsnap.com/blog/customer-service/how-to-get-your-players-to-mark-their-availability).
- **Shiftgig (gig shifts):** When a shift is already claimed, a worker can still choose "notify me" to get it if it reopens. This is a passive waitlist layered on first-come-first-served — [gigworker.com: Shiftgig](https://gigworker.com/shiftgig/) (search summary).

### Inferences
- Across gaming, no major platform has kept a standing "I'm available" presence flag alive. Where LFG survives, it is listings plus notifications to opted-in people (Discord bots, Playtomic alerts). This supports the claim's core: the passive subscription is the scalable supply source.
- The weak point in the claim: for a "need someone in 5 minutes" case, a passive subscriber has to notice the alert, decide, and open the app (see Q3 latency data). An active-pool player is already primed. The two mechanisms do different jobs: subscriptions supply breadth, the active pool supplies speed. Playtomic's 1-hour cutoff gives alerts much more slack than PreBot's 15-minute listings do.
- Playtomic charging for more than one alert suggests users value alerts. It also suggests the operator limits the number of alerts per user, which works as a frequency cap.

### Gaps
- I found no usage or response-rate data for gaming LFG notifications (Xbox LFG, Overwatch LFG, Discord LFG bots, GamerLink, Guilded, LFG.co, Teamplay). PlayStation and Valorant LFG were not researched in depth, and I found no native Valorant LFG.
- I found no data comparing time-to-fill between "available pool" and "subscription alerts" inside any gaming product.
- TeamSnap's current sub-request feature could not be confirmed.

---

## 2. What do on-demand marketplaces (ride-hailing, sub apps, emergency shift-fill, volunteer dispatch) show about "go online" vs opt-in notification blasts?

### Takeaway
Real-time marketplaces that must fill demand in minutes keep an "online" pool and use notifications or scheduling only to top it up. DoorDash allows "Dash Now" only when a zone is busy, and Uber's surge pull is modest because surges fade before drivers arrive. Shift apps that blast many opted-in workers report fill times in hours, not minutes. They also produce a well-documented failure: workers tap the alert and find the shift "already taken", which breeds frustration and fatigue. Emergency volunteer-responder systems show minutes-scale blasting can work. They do it by alerting many nearby people at once (up to 30 per case in the SAMBA trial) and tolerating low individual acceptance.

### Cited Findings
- **DoorDash:** Dashers can "Dash Now" only when their zone is shown as busy (red). Greyed-out zones are closed to on-demand dashing — [DoorDash help: Dash Now home page](https://help.doordash.com/en-ca/dashers/article/how-to-use-the-dash-now-home-page); [Ridesharing Driver](https://www.ridesharingdriver.com/doordash-schedule-hours/).
- **DoorDash:** Scheduling reserves a time and zone in advance, even when the zone is full. Scheduling opens five days ahead, and DoorDash cancels a scheduled dash if the Dasher doesn't log in within 30 minutes of the start time — [DoorDash Dasher blog](https://dasher.doordash.com/en-us/blog/dasher-schedule); [Ridesharing Driver](https://www.ridesharingdriver.com/doordash-schedule-hours/).
- **DoorDash:** When too many Dashers are online at once there may not be enough orders for all of them, and scheduling is how DoorDash manages that. One secondary source says scheduled Dashers get priority, but this is not confirmed in DoorDash's own text — [Ridesharing Driver](https://www.ridesharingdriver.com/doordash-schedule-hours/).
- **Uber:** A study of Uber data reported that surge pricing is "only modestly effective at luring drivers" and that "the vast majority of surges are short-lived." Uber disputed this — [San Francisco Chronicle](https://preview-prod.w.sfchronicle.com/business/article/Report-says-Uber-surge-pricing-has-a-twist-some-6597012.php).
- **Uber:** A natural experiment using a surge-display outage found surge visibility attracts drivers toward surging areas and explains 10–60% of drivers' self-positioning decisions across 10 cities — [Simons Institute talk abstract](https://simons.berkeley.edu/talks/signaling-surge-pricing-evidence-natural-experiment-uber).
- **Uber:** Drivers report frustration at "chasing the surge" only for it to disappear — [Jalopnik](https://jalopnik.com/we-think-uber-and-lyfts-new-surge-fares-screw-drivers-a-1835952856).
- **Instawork (self-reported):** Instawork claims a 97% fill rate within 24 hours and an average time-to-fill under 12 hours — [Contrary Research](https://research.contrary.com/company/instawork). Its own comparison pages list a "3 hours average position fill time" and a 95%+ fill rate — [Instawork compare page](https://www.instawork.com/compared-to/shiftsmart) (search summary; exact page for this figure not verified).
- **Instawork:** Over a holiday period, average time to fill dropped "below 10 hours for every occupational group" — [Instawork blog](https://blog.instawork.com/these-are-the-hottest-shifts-for-flexible-work-during-the-holidays).
- **Instawork:** Shifts posted at 5 pm local time or later get the highest fill rates. Timing relative to when workers are free matters — [Instawork blog: best time to post shifts](https://www.instawork.com/blog/when-is-the-best-time-of-day-to-post-shifts).
- **ShiftKey:** Facilities that invite specific professionals, or a group of them, to claim open shifts "find more coverage — especially for last-minute shifts." Targeted invites beat open broadcast for urgent fills. This is vendor advice with no numbers — [ShiftKey blog](https://www.shiftkey.com/resources/blog/six-ways-to-fill-more-last-minute-shifts).
- **Traba:** Invitations go to many workers at once, shifts fill first-come-first-served, and an invite doesn't guarantee a spot. Workers may be waitlisted and told if a spot opens — [Traba help](https://help.traba.work/en/articles/9096196-shift-invitations-how-do-they-work); [Traba: why waitlisted](https://help.traba.work/en/articles/9101208-why-did-i-get-waitlisted-on-the-shift-even-when-i-tried-to-sign-up-right-away).
- **Indeed Flex:** Shifts are offered to every eligible worker in the area, so another worker may accept first. Workers are advised to keep notifications on — [Indeed Flex article](https://indeedflex.co.uk/?p=4484) (search summary; page returned 403 when fetched).
- **Instawork app reviews (undated):** One user writes "I immediately click the notification and the job is already taken" and "Every day I get 20 to 30 notifications about an available job." Another writes "When a gig comes up it's gone with in seconds." A third says the volume of notifications "is sometimes overwhelming" — [JustUseApp: Instawork reviews](https://justuseapp.com/en/app/1123819773/instawork-gigs-jobs/reviews).
- **SAMBA trial (Sweden, Dec 2018 – Jan 2020):** For each suspected cardiac arrest, the system located up to 30 volunteer responders within a 1.3 km radius. Median time from call to volunteer dispatch was about 3 minutes — [JAMA Cardiology](https://jamanetwork.com/journals/jamacardiology/fullarticle/10.1001/jamacardio.2022.4362).
  - Correction: secondary summaries call the trial's "72%" a response rate. In the paper it is the survey answering rate among volunteers who had already accepted an alert, not an alert acceptance rate.
- **Heartrunner (vendor, 2023, Sweden):** In 97.4% of cases one or more volunteers accepted the alert, and volunteers arrived before the ambulance in 41% of cases. This counts cases with at least one acceptance, not individual acceptance — [Heartrunner research page](https://heartrunner.com/research/).
- **Danish citizen-responder pilot:** Fewer responders accepted alarms at certain times, and the program raised the number alerted per case from 10 to 20 to increase acceptance. The full text could not be fetched — [JMIR mHealth 2023](https://mhealth.jmir.org/2023/1/e41551/PDF) (search summary).
- **Liquidity definition:** NFX's James Currier: "Liquidity is the probability of selling something you list, or finding what you are looking for" — [Point Nine: WTF is Marketplace Liquidity?](https://writing.pointnine.com/p/wtf-is-marketplace-liquidity) (quote seen via search summary).
- **Time to fill:** Point Nine's essay frames "time to fill" as a metric to measure per time slot. It says that where the platform picks the match, supply must enter availability so buyers can transact instantly — [Point Nine](https://writing.pointnine.com/p/wtf-is-marketplace-liquidity).
- **Marketplace metrics:** a16z's glossary lists fill rate (match rate), market depth and time to find a match as core metrics — [a16z Marketplace Glossary](https://a16z.com/the-marketplace-glossary/).

### Inferences
- The closest analogy to PreBot (15-minute listing, first tap wins, need in minutes) is the volunteer-responder model, not the shift-fill model. It works because it alerts many nearby, opted-in people at once (10 → 20 → up to 30) and accepts low individual response. For PreBot that means one listing should go to enough subscribers to fill at a modest per-person tap rate.
- The cost of "blast to many, first tap wins" is shown by Instawork and Indeed Flex: most alerted people lose the race. The "already taken" feeling, repeated, is the main driver of alert fatigue in these apps, more than raw alert count. PreBot's first-tap-wins rule has exactly this risk.
- DoorDash and Uber show the hybrid operators actually run: an online pool for instant matching, plus advance scheduling or alerts to bring more people online when demand is high. Neither drops "go online". So the claim is right that the subscription should get more weight, but it overreaches if it means removing or burying the active pool.
- Instawork's 5 pm finding fits PreBot's "chosen hours" subscription design. Alerts work best when they line up with when people are already free. That is an argument for asking subscribers for time windows (as PreBot already does) and for smart defaults such as evening slots.

### Gaps
- No independent, audited time-to-fill or per-notification acceptance figures exist for Instawork, ShiftKey, Snag, TeamSnap or Plei. Snag and Plei were not found.
- No public ride-hailing data compares online-pool vs push-recruited supply in minutes.
- The individual-level acceptance rate for Heartrunner and the Danish pilot is unverified.
- The a16z, NFX and Lenny's Newsletter essays were not fetched directly. The liquidity framing comes via Point Nine and the a16z glossary.

---

## 3. What are push notification response rates, fatigue drivers and frequency caps (industry and gaming)?

### Takeaway
Gaming is the weakest push category: the lowest opt-in floor in Airship's 2025 data and broadcast CTRs under 1% in Pushwoosh's. Highly relevant, time-sensitive alerts perform far better (live-event alerts above 53% open, and transactional pushes excluded from benchmarks because their CTRs are "extreme"). The top stated reason for opting out is frequency, and stated tolerance falls off sharply above about one push a week for marketing-style messages. Personal, socially meaningful notifications get seen within minutes and feel positive, while impersonal bulk alerts are deferred and annoy. So a PreBot alert must read as "a specific group needs you, a Support, now", not as a generic broadcast.

### Cited Findings
- **Airship 2026 benchmark (data Jan–Dec 2025, 3B+ users, 15 verticals):** Median opt-in is 53.3% on Android and 48.85% on iOS — [Airship](https://www.airship.com/mobile-app-push-notification-benchmarks-for-2026/).
- **Airship, gaming:** "Gambling & Gaming" has the lowest Android opt-in floor (10th percentile) at 21.35%. Other gaming figures are only in the charts — [Airship](https://www.airship.com/mobile-app-push-notification-benchmarks-for-2026/).
- **Airship, open rates:**
  - iOS median direct open rate fell 13.71% year over year.
  - Entertainment's Android floor is 0.45% direct open.
  - Live-streaming alerts for one sports body (USTA) reached above a 53% open rate, showing high relevance and time-sensitivity.
  - Lifts reported: personalisation +37% average, zero-party data +82%.
  
  — [Airship](https://www.airship.com/mobile-app-push-notification-benchmarks-for-2026/)
- **Airship, frequency:** Top-decile senders push about 179–188 notifications per user per month and show 72% higher session rates. But Airship's consumer survey names messages arriving "too frequently" as the top reason users opt out — [Airship](https://www.airship.com/mobile-app-push-notification-benchmarks-for-2026/).
- **Pushwoosh CTR (Q4 2024 – Q2 2025, broadcast and segmented pushes only):**
  - Action games: iOS 0.46%, Android 0.82%.
  - Hypercasual games: iOS 0.82%, Android 1.05%. Both game categories are below the all-industry average.
  - E-commerce: iOS 3.05%, Android 3.78%.
  - Transactional pushes were excluded because their CTRs "can achieve extreme values."
  
  — [Pushwoosh](https://www.pushwoosh.com/blog/push-notification-benchmarks/)
- **Localytics (2017 survey of 1,000 US smartphone users, via Qualtrics; stated intentions):**
  - 10% say they would disable notifications for an app sending 1 push a week.
  - 37% would disable at 2–5 a week. An earlier Localytics / Research Now survey put this at 46%.
  - 22% would stop using the app at 2–5 a week (down from 31% in 2015), and nearly one-third at 6–10 a week.
  
  — [MarketingProfs (2018)](https://www.marketingprofs.com/charts/2018/33486/how-many-mobile-push-notifications-are-too-many); [eMarketer chart](https://www.emarketer.com/chart/215696/volume-of-push-notifications-that-prompt-us-smartphone-users-stop-using-app-oct-2015-nov-2017-of-respondents)
- **Braze (2016):** Braze cites research that 60% of iOS and 40% of Android users opt out of push. An informal poll of 14 Braze staff ranked reasons as irrelevant or non-personal messages (30%), too many notifications (25%), default distrust (25%) and no interest in the brand (20%). Braze itself says this poll is not statistically significant — [Braze](https://www.braze.com/resources/articles/opt-out-of-push-notifications-why-users-do-it.md).
- **Braze:** The same article cites a 2015 Delvv survey in which 78% of consumers said most push notifications they get are irrelevant — [Braze](https://www.braze.com/resources/articles/opt-out-of-push-notifications-why-users-do-it.md).
- **Pielot, Church & de Oliveira (MobileHCI 2014; 15 Android users, 1 week, March 2013):**
  - Participants received 63.5 notifications a day on average.
  - Median time from arrival to viewing was 6.6 min on weekdays and 3.5 min on weekends for messengers, 3.8 and 7.0 min for social networks, and up to 27.7 min for email on weekends.
  - The screen was off when 69.2% of notifications arrived.
  - Notifications were viewed within minutes whether or not the phone was in silent mode.
  
  — [Pielot et al. 2014 (PDF)](https://ic.unicamp.br/~oliveira/doc/MHCI2014_An-in-situ-study-of-mobile-phone-notifications.pdf)
- **Pielot et al. 2014, emotions:** More email and social-network notifications correlated with feeling stressed, interrupted and annoyed. More messenger notifications correlated with feeling connected. When overwhelmed, participants chose to defer email and social notifications. The authors conclude that in personal contexts "strategies around communicating (un)availability and managing expectations appear more suited" — [Pielot et al. 2014](https://ic.unicamp.br/~oliveira/doc/MHCI2014_An-in-situ-study-of-mobile-phone-notifications.pdf).
- **Pielot et al. (CHI 2016; 10,372 notifications, 20 users):** Sender–recipient relationship, notification priority, and the type and complexity of the ongoing task affected response time — [UCL Discovery (CHI16.pdf)](https://discovery.ucl.ac.uk/1502364/1/CHI16.pdf); [Birmingham research portal](https://research.birmingham.ac.uk/en/publications/my-phone-and-me-understanding-peoples-receptivity-to-mobile-notif/).
- **OneSignal:** Secondary sources say OneSignal warns that more than a few messages a week erodes trust for most use cases and recommends frequency caps rather than a single number. I did not find this in OneSignal's own docs — [MobiLoud summary](https://www.mobiloud.com/blog/push-notification-statistics) (search summary).

### Inferences
- With a 15-minute listing, a viewing median of about 3.5–7 minutes is only reached when the alert feels personal or social. Generic "app" alerts sit closer to the 20+ minute email tail. An alert phrased as a named group needing a Support at a given rank, with a countdown and squad avatars, is more likely to behave like a messenger notification than like a marketing push. This is an inference from the sender-relationship and priority findings, not a tested result.
- Because gaming broadcast CTR is below 1%, PreBot cannot rely on blasting all subscribers like a marketing push. Its edge is that each alert is a transactional, matching event, the category Pushwoosh excluded for having "extreme" CTRs. That edge only holds while alerts stay specific (role, rank, time window) and rare enough to keep meaning something.
- Survey tolerance (about 10% disable at 1 a week, 37% at 2–5 a week) is for marketing pushes. Wanted alerts tolerate more. Still, a subscriber in a popular role (for example Support) during peak hours could easily get several alerts an hour even with the 30-minute cooldown, which is well above any benchmark comfort zone.

### Gaps
- I found no gaming-specific or LFG-specific push open-rate data from Airship, OneSignal, Braze or CleverTap. Airship's gaming medians are in charts I could not read.
- I found no 2025–2026 measured (not survey) data linking per-user alert frequency to opt-out.
- Web push for PWAs (open rates, and iOS needing home-screen install before web push works) was not researched here and matters for PreBot's PWA channel. It should be checked separately.
- The CHI 2016 paper's exact response-time figures were not extracted.

---

## 4. What are the norms for Discord role pings in LFG servers (@here, LFG roles, ping fatigue, limits)?

### Takeaway
Discord LFG role pings are blunt tools. They bypass users' mute settings, and the only per-user escape (suppressing role pings) is all-or-nothing. Busy servers see LFG roles pinged dozens of times a day, and moderators fight misuse. Community leaders warn that overuse makes players tune out. The common fixes are narrow opt-in roles, channel limits, and bots that post, ping once and auto-expire listings.

### Cited Findings
- **Mute bypass:** Role mentions are not affected by server mute, server notification settings set to "Nothing", suppressing @everyone/@here, or muting the channel. Users have asked for years for a way to silence them — [Discord support community](https://support.discord.com/hc/tr/community/posts/360056254152-Ability-to-Disable-Mute-Role-Pinging-Notifications); [Discord support community](https://support.discord.com/hc/sv/community/posts/360056444792-Ability-to-mute-role-mentions).
- **All-or-nothing suppression:** "Suppress role pings" suppresses all role pings, including ones the user actually wants — [Discord support community](https://support.discord.com/hc/tr/community/posts/360056254152-Ability-to-Disable-Mute-Role-Pinging-Notifications).
- **Volume and misuse:** A GTA server's LFG role was reportedly pinged at least 40 times a day. Moderators report new members pinging LFG roles outside LFG channels and doing it again "5-10 mins" after being told. They asked for per-channel role-ping permissions, which I found no evidence Discord has shipped — [Discord support community: per-channel role pings](https://support.discord.com/hc/fr/community/posts/4422739545367-Make-custom-roles-be-pingable-in-specific-channels).
- **Expiring pings:** An LFG admin proposed "Temporary Pings Only" roles, where pings are cancelled after X hours. This is a request, not a feature — [Discord support community](https://support.discord.com/hc/ko/community/posts/9075076945943-Role-pings-which-expire-after-a-certain-time-limit).
- **Norms come later:** A gaming community rolling out pingable LFG roles limited them to #gaming-chat and #gaming-matchmaking and admitted "we will have to shake out the social norms and etiquette as we go" — [Dark Jedi Brotherhood](https://www.darkjedibrotherhood.com/news/reports-fist-supplemental-report-pingable-lfg-roles-in-discord-now-testing).
- **Overuse warning:** An indie studio's community guidance warns that overusing pings frustrates players and leads them to ignore you — [Akupara Games](https://www.akuparagames.com/?p=3966) (search summary).
- **Bot pattern:** Puck lists, pings for and delists LFG posts, with a default archive time and the option to restrict which channels it posts in — [discordbotlist: Puck](https://discordbotlist.com/bots/puck/upvote).

### Inferences
- For PreBot's Discord bot, a broad role such as "@Flex-Support" pinged for every listing would recreate the 40-a-day problem. Users cannot mute it except by suppressing all role pings, or by leaving the role, which means losing supply. Per-user bot DMs, or a role per role-and-rank band with a server-side cooldown, give users finer control.
- Listings that expire on their own (PreBot's 15 minutes) match the Puck pattern. Editing the original ping message to "FILLED" or "EXPIRED" spares later readers a dead ping and the "already taken" letdown.

### Gaps
- I found no measured data (reaction rates, role-leave rates) on LFG role pings in Discord servers, and no Turkish LoL Discord norms.
- I found no official Discord guidance on @here etiquette or limits. The findings are user requests and community practice.
- Reddit discussions did not surface in searches.

---

## 5. Verdict on the claim, and concrete recommendations to rebalance the flow

### Takeaway
The claim is largely right on direction and partly wrong on framing. The evidence supports making "Bana haber ver" the main supply source: standing "available" statuses are rarely kept up, and successful products centre need-driven alerts, with some charging for extra alerts. But passive alerts are slower than a primed active pool, and with first-tap-wins they create a race most alerted people lose, which is where fatigue comes from. The better design is a hybrid. Onboard everyone into a narrow, time-windowed subscription. Let one tap on an alert both claim the slot and optionally put the user into "available" for a short window. Then guard the subscription with per-user caps, quiet hours, snooze, and honest "filled" feedback.

### Cited Findings (evidence behind each recommendation)
- **Make the subscription the default onboarding step.** Standing LFG/presence features have decayed or never shipped: Overwatch LFG was dropped citing declining usage, Discord and Steam have no native looking-to-play flag, and users ask for subscription notifications. Alert-led products monetise alerts (Playtomic) — [esports.gg](https://esports.gg/news/overwatch/overwatch-2-lfg-is-unlikely-to-happen-soon); [Steam Community](https://steamcommunity.com/discussions/forum/10/4767710913128814889); [Playtomic help](https://playerhelp.playtomic.com/hc/en-gb/articles/19831492072337-How-to-manage-the-notifications-from-Playtomic-App) (search summary). Airship reports onboarding campaigns can lift opt-in up to 40% above category average — [Airship](https://www.airship.com/mobile-app-push-notification-benchmarks-for-2026/).
- **Keep an active pool for minutes-scale fills.** DoorDash keeps on-demand "Dash Now" and scheduling side by side. Surge-type alerts to offline supply are only modestly effective because demand spikes are short-lived — [DoorDash help](https://help.doordash.com/en-ca/dashers/article/how-to-use-the-dash-now-home-page); [SF Chronicle](https://preview-prod.w.sfchronicle.com/business/article/Report-says-Uber-surge-pricing-has-a-twist-some-6597012.php).
- **Cascade the blast instead of alerting everyone.** Volunteer-responder systems alert a bounded nearby set (up to 30 per case) and widen it when acceptance is low (10 → 20 in the Danish pilot). ShiftKey says targeted invites cover last-minute shifts better — [JAMA Cardiology](https://jamanetwork.com/journals/jamacardiology/fullarticle/10.1001/jamacardio.2022.4362); [JMIR mHealth](https://mhealth.jmir.org/2023/1/e41551/PDF) (search summary); [ShiftKey](https://www.shiftkey.com/resources/blog/six-ways-to-fill-more-last-minute-shifts).
- **Soften first-tap-wins losses.** Blast-to-many apps produce "already taken" frustration and "20 to 30 notifications" a day. Traba and Shiftgig add waitlists and "notify me" re-offers — [JustUseApp: Instawork reviews](https://justuseapp.com/en/app/1123819773/instawork-gigs-jobs/reviews); [Traba help](https://help.traba.work/en/articles/9101208-why-did-i-get-waitlisted-on-the-shift-even-when-i-tried-to-sign-up-right-away); [gigworker.com](https://gigworker.com/shiftgig/) (search summary).
- **Cap frequency and make alerts specific.** Frequency is the top opt-out reason, survey tolerance drops sharply above 1 a week for generic pushes, and relevance and personalisation give the largest lifts (+37% to +82%) — [Airship](https://www.airship.com/mobile-app-push-notification-benchmarks-for-2026/); [MarketingProfs](https://www.marketingprofs.com/charts/2018/33486/how-many-mobile-push-notifications-are-too-many); [Braze](https://www.braze.com/resources/articles/opt-out-of-push-notifications-why-users-do-it.md).
- **Phrase alerts as personal and social, and let users signal unavailability.** Personal messages are viewed in about 3.5–7 minutes and feel connecting, while impersonal volume causes stress and deferral. The authors recommend communicating (un)availability — [Pielot et al. 2014](https://ic.unicamp.br/~oliveira/doc/MHCI2014_An-in-situ-study-of-mobile-phone-notifications.pdf).
- **Time windows matter.** Shifts posted at 5 pm or later fill best. DoorDash peaks cluster at meal times — [Instawork blog](https://www.instawork.com/blog/when-is-the-best-time-of-day-to-post-shifts); [Ridesharing Driver](https://www.ridesharingdriver.com/doordash-schedule-hours/).
- **On Discord, avoid broad role pings.** Role pings bypass mutes and get overused. Use expiring, bot-managed listings — [Discord support community](https://support.discord.com/hc/tr/community/posts/360056254152-Ability-to-Disable-Mute-Role-Pinging-Notifications); [Puck](https://discordbotlist.com/bots/puck/upvote).

### Inferences (proposed PreBot changes; design hypotheses to A/B test, not proven results)
1. **Reorder the home and onboarding flow.** The first call to action after role and rank setup should be "Haber ver: Support aranınca, 19:00–00:00" with smart defaults (the user's role, their rank band, evening hours). "Şu an müsaitim" stays as a secondary "I'm on right now" action. This keeps the claim's main point without dropping the fast pool.
2. **Make one tap from the alert do the work.** The alert opens straight into a "Katıl" (join) confirmation, with no browsing. After a win or a loss, offer "15 dk müsait kal" (stay available for 15 minutes), so a subscriber who responded is briefly turned into an active-pool member. This links the two mechanisms instead of treating them as rivals.
3. **Cascade instead of blast.** Notify active-pool players first, then a ranked slice of subscribers, for example by best rank fit and past responsiveness, then widen after about 2–3 minutes if unfilled. This copies the volunteer-responder pattern. It cuts "already taken" losses and spreads alerts more evenly across subscribers.
4. **Handle losing honestly.** If someone taps after the slot is filled, tell them immediately ("Doldu, 40 sn önce"), give them priority on the next matching alert, and don't count that alert against their cooldown. Silently counting race losses is likely the biggest source of fatigue.
5. **Cap per user and adapt.** Beyond the 30-minute cooldown, add a daily cap per subscriber (for example 3–6) and lengthen the cooldown for users who ignore N alerts in a row. Exact numbers need testing, since no benchmark exists for wanted, transactional LFG alerts.
6. **Snooze and quiet hours.** Add a one-tap "Şu an meşgulüm" (busy right now) on the alert itself (1 h / tonight), apply the chosen hours strictly, and add a default night quiet window. This is the "communicate unavailability" approach from Pielot et al.
7. **Make alert copy specific and social.** For example: "Gold–Plat Flex ekibi Support arıyor — 4/5, 12 dk kaldı" (a Gold–Plat Flex team is looking for a Support, 4/5 filled, 12 minutes left). Include the role, rank, remaining time and how full the squad is, so the alert reads like a message from people, not a broadcast.
8. **On Discord, avoid one big role.** Prefer bot DMs or narrow role-and-rank-band roles with a server cooldown. Edit ping messages to "DOLDU" (filled) or "SÜRESİ DOLDU" (expired) when listings close.
9. **Measure per mechanism.** Track:
   - alert → open rate and open → tap rate;
   - tap → win rate (share of race losses);
   - time-to-fill split by fill source (active pool vs subscription);
   - unsubscribe or snooze rate after the Nth alert per day.
   
   These are the metrics needed to settle the claim with PreBot's own data, since public benchmarks do not cover this case.

### Gaps
- No public data directly compares passive-subscription and active-status supply for time-to-fill in any LFG product. The verdict rests on analogies (delivery, gig shifts, emergency volunteers) and general notification research.
- No benchmark exists for the "right" per-user cap on wanted, time-critical matching alerts. The caps above are starting points, not evidence-backed thresholds.
- Not researched: Turkish-market push behaviour, iOS PWA web-push constraints, and Discord-DM vs web-push response differences. These could change the recommendations for PreBot's channels.
