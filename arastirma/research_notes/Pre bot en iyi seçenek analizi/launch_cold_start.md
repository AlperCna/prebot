# Launch, Cold Start and Validation Playbook for a TR "Need 1 More Player" (Pre) LoL Discord Bot

Scope note: research done 2026-11-08 with a budget of about 15 tool calls. Many results were secondary summaries or vendor blogs; each finding is labelled **[DATA]** (a measured or reported number), **[ANECDOTE]** (one founder or user's report), or **[OPINION]** (advice or interpretation). The queueing figures are **my own calculations** from standard Poisson and binomial formulas, not numbers taken from a source.

---

## 1. Fastest validation in days: concierge, Wizard-of-Oz, fake-door and smoke tests

### Takeaway
No source has a Discord-specific validation benchmark. The most-cited general anchors for fake-door click-through are under 2% = noise, over 5% = good and over 10% = excellent, all from vendor guides. A real-time LFG product is better validated by watching actual fills: run a hand-operated ("concierge") pre channel during peak hours and measure fill rate and time-to-fill, not clicks.

### Cited Findings
- **[OPINION]** "Smoke test" and "fake door" are now used interchangeably. Historically, a smoke test was a fake landing page for a whole product and a fake door was a non-working button inside an existing product. — [causo.ai guide](https://hub.causo.ai/guides/fake-door-smoke-testing-validate-demand-2026)
- **[OPINION]** Thresholds vary by source. One guide suggests about 5% click-through for in-product tests shown to relevant users. Another lists over 2% as minimum, over 5% good and over 10% excellent. A third says anything under 2% is noise. Guides advise setting the threshold *before* the test, ideally against a control or comparable feature. — [koji.so fake door guide](https://www.koji.so/docs/fake-door-testing-guide); [koji.so smoke test guide](https://www.koji.so/docs/smoke-test-product-validation); [preuve.ai](https://preuve.ai/blog/fake-door-test.md); [Kromatic Real Startup Book – landing page smoke test](https://kromatic.com/real-startup-book/2-evaluative-market-experiments/value-proposition-test/landing-page-smoke-test/)
- **[OPINION]** Commitment signals rank by strength: a "notify me" click is weak, email plus specifics is medium, and card capture or pre-payment is strong. Passing a smoke test validates acquisition, not retention ("Demand validates acquisition; retention validates value"). — [causo.ai guide](https://hub.causo.ai/guides/fake-door-smoke-testing-validate-demand-2026)
- **[OPINION]** Founders act as the "invisible bridge" between the two sides early on (manual matchmaking). — Chen framework as summarised by [vcbeast glossary](https://vcbeast.com/venture-capital-glossary/cold-start-problem)
- **[ANECDOTE]** Zillow had brokers type in mortgage quotes by hand every day because no automated system existed. This is a classic "do things that don't scale" or Wizard-of-Oz pattern. — [Lenny's Newsletter, Kickstarting a marketplace pt.1](https://www.lennysnewsletter.com/p/how-to-kickstart-and-scale-a-marketplace)
- **[ANECDOTE]** An Indie Hackers post titled "Validation journey for a Discord bot" reports about $30 of ads → about 70–80 servers, a YouTuber video in week one, and that bot-list votes (not the listing itself) drove discovery. These figures come from search snippets only; the page returned HTTP 403 when fetched. — [Indie Hackers](https://www.indiehackers.com/post/validation-journey-for-a-discord-bot-17de6caaaa)

### Inferences
- For a "pre" (need +1) tool, the riskiest assumption is **liquidity** (is someone available within minutes?), not "do people want it?". Fake-door clicks do not test liquidity, so concierge testing is the better first experiment.
- Suggested 3–5 day concierge test (my design, informed by the sources above):
  1. Day 0: open a `#pre-ara` channel in one partner server, or your own server. The founder posts something like "Bu akşam 21:00–24:00 pre eşleştirme: rol + rank yaz, ben eşleştiriyorum" ("Tonight 21:00–24:00 pre matchmaking: post your role + rank, I'll match you"). Matching can be done by hand, or with a 50-line bot that only collects `/pre rol:sup rank:gold` and pings the founder.
  2. Log every request: timestamp, role, rank, filled (y/n), minutes to fill.
  3. Decision metrics (thresholds are my proposal, since no source gives LFG-specific ones): **fill rate** (filled ÷ requests) of at least 50% at peak; **median time-to-fill** of 10 minutes or less; **repeat requesters** (people who asked on 2 or more distinct nights) of at least 20% of unique users; and at least 20–30 requests per peak evening. If these are met within 3 evenings, build the automated version.
- A fake-door test is still cheap as a *secondary* signal. Pin a message with a "Pre bul" button that writes to a waitlist, and compare clicks against the server's online count. Treat under 2% of online users as noise, per the vendor anchors.

### Gaps
- No Discord-bot-specific validation benchmarks found (for example, typical slash-command adoption per server).
- The Indie Hackers post could not be read in full (403); its figures are snippet-level and unverified.

---

## 2. Launch strategies compared: own server, partner TR LoL servers, bot lists, streamers, university clubs

### Takeaway
For liquidity, a **partnership with one large, active TR LoL server** (option 2), backed by the founder's friends as seed supply (option 1), is the strongest first move. It puts players in one place at once, which marketplace evidence favours. Public bot lists (option 3) drive *server installs*, not *players in the same pool*, and listing alone brings little traffic. Streamers (option 4) give spikes but not sustained density. University clubs (option 5) are good small "atomic networks" but seasonal and small.

### Cited Findings
- **[DATA, self-reported]** PixxieBot's developer said 90% of invites come from top.gg, although the bot is listed on several lists. top.gg says "More votes = listing higher in search results" and suggests vote rewards or vote-locked commands; promoted slots can be bought via top.gg Auctions. — [top.gg blog](https://blog.top.gg/top-5-tips-to-organically-grow-your-discord-bot)
- **[OPINION]** top.gg advises standing out with one distinctive feature rather than being "multifunctional" (examples: Hydra for music, Ticket Tool for tickets). — [top.gg blog](https://blog.top.gg/top-5-tips-to-organically-grow-your-discord-bot)
- **[ANECDOTE]** Listing alone brought little traffic; votes drove discovery; about $30 of ads gave about 70–80 servers; a YouTuber video in week one helped (snippet only). — [Indie Hackers](https://www.indiehackers.com/post/validation-journey-for-a-discord-bot-17de6caaaa)
- **[ANECDOTE]** Dank Memer started in 2017 as a meme bot built with a friend. Growth went from about 2,000 servers/day at the start of 2020 to about 10,000/day by year-end, which the team credits to users sharing it with friends during lockdown. The bot later reached about 2M servers. Growth was word of mouth between servers, not admin partnerships. — [Dank Memer blog: 2020 in conclusion](https://dankmemer.lol/blogs/2020:-in-conclusion); [Dank Memer blog: 2 million servers later](https://dankmemer.lol/blogs/2-million-servers%20later…)
- **[DATA, unverified date]** Raid-Helper (an event and sign-up bot for MMO guilds) is listed as used in about 271,000 servers. Its early growth history could not be verified; the only origin source found was an auto-generated page with inconsistent claims. — [discordbotlist.com Raid-Helper](https://discordbotlist.com/bots/raid-helper)
- **[DATA, platform rule, 2020–2023 sources]** Unverified bots cannot join more than 100 servers; verification can be requested from 75 servers. Bots in more than 100 servers need approval for privileged intents (members, presence, message content). — [Discord blog: The future of bots on Discord](https://discord.com/blog/the-future-of-bots-on-discord); [PiunikaWeb on MEE6 losing verification (2023)](https://piunikaweb.com/2023/03/02/discord-mee6-bot-lost-verification-heres-what-you-need-to-know/)
- **[DATA, snapshot]** "League of Legends Türkiye" (discord.com server 1195405905576071168) showed **7,228 members, 949 online** in a search snapshot. Listing sites show very different numbers for other "LoL Türkiye" servers (e.g., 137,605 members / 18,854 online), so counts must be checked live. — [discord.com server page](https://discord.com/servers/league-of-legends-turkiye-1195405905576071168); [discordbotlist LoL Türkiye](https://discordbotlist.com/servers/lolturkiye-1894/join)
- **[DATA]** League of Legends TR and Stratera (Dora "Stratera" Özsoy's official server) are listed in Discord discovery and on discordbotlist. Several smaller duo-focused servers already exist ("SoloQ", created 25 Nov 2025; "Random LoL Player", which has "duo arıyorum" (looking-for-duo) channels). — [LoL TR discovery](https://discord.com/servers/league-of-legends-tr-896475140039278592); [Stratera on discordbotlist](https://discordbotlist.com/servers/stratera/join); [SoloQ server](https://discord.com/servers/soloq-1442928338590240820); [Random LoL Player](https://discord.com/servers/random-lol-player-1205191369799245915)
- **[OPINION]** What admins and owners check before adding a bot:
  - Minimal permissions; an Administrator request is a red flag.
  - A public home: website, support server, docs.
  - A verified badge.
  - Signs of maintenance.
  - Not spamming pings ("members will mute the server").
  - Being testable in a private channel first.

  — [peakbot.pro: how to tell if a bot is safe](https://peakbot.pro/blog/how-to-tell-if-a-discord-bot-is-safe); [mava.app 2026 guide](https://www.mava.app/blog/add-a-bot-to-discord)
- **[OPINION]** Chen: Tinder's early growth came through USC fraternities and sororities via campus parties, then spread to other colleges. This is evidence for "university clubs" as atomic networks. — [Lenny's Newsletter: The Atomic Network (Chen excerpt)](https://www.lennysnewsletter.com/p/atomic-network); [foodondemand.com interview summary](https://foodondemand.com/01062022/andrew-chen-examines-the-cold-start-problem-for-platforms-like-uber-snackpass/)

### Inferences
Comparison matrix (my synthesis):

| Option | Liquidity (players in one pool at the same time) | Speed to first fills | Effort for a solo dev | Main risk |
|---|---|---|---|---|
| 1. Own server + friends | Low (maybe 10–30 people) | Same day | Very low | Pool too small; dies after novelty |
| 2. Partner with one big TR LoL server (LoL TR / Stratera / LoL Türkiye) | **High**: hundreds online at peak, and an existing "looking for duo/team" behaviour | 1–7 days (admin approval) | Medium (outreach, trust, moderation features) | Admin says no; bot seen as spam |
| 3. Public bot list (top.gg / discordbotlist) | **Fragmented**: many small servers each with tiny pools, unless a cross-server pool exists | Slow (votes, ranking) | Medium (listing, votes, support server) | Installs without fills; 100-server verification cap |
| 4. TR LoL streamers | Spiky, high for one evening | Fast if a streamer agrees | High (access, negotiation) | Spike, then churn; hard for an unknown dev to reach |
| 5. University e-sports clubs | Medium-low, but dense and trusting | 1–2 weeks | Medium | Small; exam and holiday seasonality |

- **Best for liquidity: option 2**, seeded by option 1. Uber started with a time-bound atomic network ("5pm at the Caltrain station"), and Lenny's interviewees mostly constrained by geography or category. Both point to concentrating in **one** existing dense pool rather than spreading across many servers ([Lenny – Atomic Network](https://www.lennysnewsletter.com/p/atomic-network); [Lenny – Kickstart pt.1](https://www.lennysnewsletter.com/p/how-to-kickstart-and-scale-a-marketplace)).
- **Admin pitch (inferred from the admin-concern sources):**
  - Minimal permissions (slash commands plus the one channel it posts in).
  - No DMs and no mass pings; `@here` only when opted in.
  - A built-in report and blacklist command, auto-deleting stale requests, and rate limits per user.
  - It *reduces* clutter in their existing LFG channel and keeps their members on their server.
  - The admin can turn it off at any time.
  - A free/no-ads promise, and a dashboard/log showing fills ("X takım tamamlandı" / "X teams filled") as proof of value.
- Bot lists become useful **later**, once a cross-server pool exists (so a 30-member server still gets fills from the shared pool), or for portfolio "installs" numbers.

### Gaps
- No case study found of a bot maker approaching *Turkish* server admins; there is no data on admin acceptance rates.
- Current live member/online counts for League of Legends TR (~19.5K) and Stratera (~25K) were not verified in this session; those figures came from the brief.
- MEE6's early-growth story could not be verified (only low-quality pages found). Raid-Helper's and LFG Hub's origin stories were not found.
- No data found on Turkish LoL streamer partnership costs or response rates.

---

## 3. Cold-start tactics for real-time marketplaces / LFG

### Takeaway
The evidence favours a small, dense, **time-boxed** network: a specific place plus a specific hour, like Uber's "5pm at Caltrain". It also favours seeding the scarce ("hard") side, which most marketplaces did first, and doing unscalable manual work early. For a pre bot this means: one server, fixed "pre saati" (pre-hour) windows, the founder's friends as standby fillers, and a visible live pool counter.

### Cited Findings
- **[OPINION, from book]** The atomic network is "the smallest network needed that can stand on its own"; "Your product's first atomic network is probably smaller and more specific than you think." Chen's advice: launch a dead-simple value proposition and do whatever it takes, even unscalable things. — [Lenny's Newsletter: The Atomic Network](https://www.lennysnewsletter.com/p/atomic-network)
- **[ANECDOTE]** Uber's earliest network was tiny and time-bound ("5pm at the Caltrain station at 5th and King St."). It later grew to the city's 7x7 square miles. Uber used an internal tool to send drivers to busy moments in real time. — [Lenny's Newsletter: The Atomic Network](https://www.lennysnewsletter.com/p/atomic-network)
- **[DATA, anecdotal recall]** Uber's launch playbook targeted 30+ drivers and ETAs under 15 minutes before launch. OpenTable found 50–100 concentrated restaurants per city gave enough choice. — [Lenny's Newsletter, Kickstart pt.1](https://www.lennysnewsletter.com/p/how-to-kickstart-and-scale-a-marketplace)
- **[DATA, interview count]** Most of Lenny's 17 interviewed marketplaces constrained at launch, by geography or category; Thumbtack was the exception. Most concentrated early effort on supply. Eventbrite focused only on supply and found that supply generated demand. About 40% had supply/demand imbalance varying by geography or category. — [Lenny's Newsletter, Kickstart pt.1](https://www.lennysnewsletter.com/p/how-to-kickstart-and-scale-a-marketplace); [Lenny's pt.2 Supply vs Demand](https://www.lennysnewsletter.com/p/how-to-kickstart-and-scale-a-marketplace-9ee); [Lenny's: supply- or demand-constrained](https://www.lennysnewsletter.com/p/how-to-know-if-youre-supply-or-demand)
- **[OPINION/ANECDOTE]** Minimum concurrent users: Zoom needs 2–3 people and Slack 5–10 at the same time (Chen interview). Tinder needed about 200–500 people on a campus and seeded them with campus parties. — [foodondemand.com (Chen interview)](https://foodondemand.com/01062022/andrew-chen-examines-the-cold-start-problem-for-platforms-like-uber-snackpass/); [Lenny – Atomic Network](https://www.lennysnewsletter.com/p/atomic-network)
- **[OPINION]** Prioritise the "hard side", the scarce side, which often needs incentives early. "Neighbourhood liquidity matters far more than a diluted national presence." — [Antoine Buteau: lessons from Andrew Chen](https://www.antoinebuteau.com/lessons-from-andrew-chen/); [vectree summary](https://vectree.io/c/dynamics-of-acquiring-and-retaining-hard-side-vs-soft-side-users-in-initial-network-liquidity)
- **[OPINION]** Slack used an invite-only launch plus early-adopter buzz; Uber used ice-cream promotions for local press. — [Lenny – Atomic Network](https://www.lennysnewsletter.com/p/atomic-network)

### Inferences
- **Hard side here = fillers**: the solo player willing to be the 5th, especially in scarce roles (support/jungle) and in rank-compatible bands. The "demand" side is the 4-stack posting a pre request. Seed the fillers first: founder plus 10–20 friends on a "yedek" (standby) role who get pinged when a request matches.
- **Concentrate in time:** do not promise 24/7. Announce "Pre saati: Cuma–Pazar 21:00–01:00" (Fri–Sun) so arrivals cluster; this raises the arrival rate λ during the window (see §4). Outside the window, the bot replies "Şu an havuz boş; bu akşam 21:00'de X kişi bekleniyor" ("The pool is empty right now; X people are expected at 21:00 tonight"), which is the "come back at 21:00" message.
- **Show live pool size** ("Havuzda: 7 kişi — 2 sup, 1 jgl", i.e. "In pool: 7 people — 2 support, 1 jungle") so would-be fillers see the network exists. This is analogous to Uber showing nearby cars; it is an inference, not sourced.
- **Cross-server pool** only after one server works. Chen's "build one, then two, then ten" logic says replicate the first atomic network rather than start fragmented ([Lenny – Atomic Network](https://www.lennysnewsletter.com/p/atomic-network)).
- **Incentives for the hard side:** small non-monetary rewards such as a "Kurtarıcı" (rescuer) role or leaderboard for fillers, mirroring "hard side often needs incentives" (inference).

### Gaps
- The Ranked 5s / Flex windows on TR (the brief's "Fri–Sun 16:00–04:00") could not be verified against a Riot source in this session. Riot's current TR queue schedule should be checked directly before marketing "pre saati" around it.
- No published LFG-bot case study with fill-rate or time-to-fill numbers was found (LFG Hub and similar bots have no public metrics).

---

## 4. Measuring and deciding quickly: queueing math and realistic TR pool sizes

### Takeaway
By simple probability, with each pooled player having about an 8% chance of matching both role and rank, you need about **30 simultaneously available fillers** to fill a request roughly 90% of the time. If only the role must match (about 20%), around **10–15** suffice. A large TR LoL server with about 900+ online at peak could plausibly sustain this *only if* a few percent of online members opt into the pool. That is why concentration in one big server matters.

### Cited Findings
- **[DATA, snapshot]** LoL Türkiye (7,228 members) showed 949 online, about 13% of members online at snapshot time. — [discord.com server page](https://discord.com/servers/league-of-legends-turkiye-1195405905576071168)
- **[DATA, anecdotal]** Chen: Slack needs 5–10 concurrent users and Zoom 2–3; the minimum viable network depends on how many simultaneous people a use requires. — [foodondemand.com](https://foodondemand.com/01062022/andrew-chen-examines-the-cold-start-problem-for-platforms-like-uber-snackpass/)
- **[DATA, anecdotal]** Uber's pre-launch threshold was 30+ drivers and ETAs under 15 minutes. This is a useful analogue for "fill within 10–15 minutes". — [Lenny's Newsletter, Kickstart pt.1](https://www.lennysnewsletter.com/p/how-to-kickstart-and-scale-a-marketplace)
- Standard references for the formulas below: [Poisson distribution (Wikipedia)](https://en.wikipedia.org/wiki/Poisson_distribution); [Little's law (Wikipedia)](https://en.wikipedia.org/wiki/Little%27s_law). These pages were not fetched this session; the formulas are textbook.

### Inferences (my calculations)
**Model A – snapshot pool.** N fillers are waiting and each independently matches with probability p. P(at least one match) = 1 − (1−p)^N.
- Role-only match (p = 0.20):
  - N=5 → 67%
  - N=10 → 89%
  - N=15 → 96.5%
- Role + rank band (p ≈ 0.20 × 0.40 = 0.08):
  - N=10 → 57%
  - N=20 → 81%
  - N=30 → 92%
  - N=40 → 96%
- Rule of thumb: N ≈ 2.3/p for 90%. That is about 12 fillers for a role-only match and about 29 for role + rank.

**Model B – Poisson arrivals.** Matching fillers arrive at rate λ·p per hour. P(fill within t) = 1 − e^(−λ·p·t).
- For 90% within 10 min (t = 1/6 h) you need λ·p ≥ 13.8/h. That means about **69 filler arrivals/hour** with role-only matching, or about **173/hour** with role + rank matching.
- For 50% within 10 min you need λ·p ≥ 4.2/h. That means about 21/h (role-only) or about 52/h (role + rank).

**Little's law.** L = λ·W. If fillers arrive at 30/h and stay in the pool for 30 minutes on average, about 15 are in the pool at any time. That is roughly enough for role-only matching (Model A, about 96%).

**Realistic TR pool (inference).** A server with about 950 online at peak would need about 1.5–3% of online members to actively sit in the pool (15–30 people) for good fill rates.
- Own server with friends (10–30 people online): role-only matching may work on a good night, but rank + role matching will fail often.
- Hence:
  - Loosen matching at launch (role-only, or a wide rank band such as ±1 tier), and let the requester accept "any rank".
  - Concentrate arrivals into announced windows, which raises λ.
  - Use standby friends to raise N.

**Decision rule (proposed).** After 3 peak evenings:
- If median time-to-fill ≤ 10 min and fill rate ≥ 50% → automate and pitch a second server.
- If fill rate < 30% → loosen matching or move to a bigger server before writing more code.

### Gaps
- No empirical data on the share of a TR LoL Discord's online members who look for a pre at any moment.
- No data on the TR role distribution of fillers (support/jungle scarcity is assumed, not sourced).
- Peak-hour online counts for League of Legends TR and Stratera were not captured.

---

## 5. Portfolio angle: what junior .NET hiring managers value in a side project

### Takeaway
Evidence, mostly blog and forum opinion, consistently says a deployed project with **real users plus measurable outcomes** beats tutorial clones. Reviewers look at 2–4 pinned repos with a clear README, live demo, clean code and sane commit history. There is credible pushback that some enterprise hiring managers ignore side projects entirely. No Turkey-specific recruiter evidence (Kariyer.net, LinkedIn TR) was found.

### Cited Findings
- **[OPINION]** Recruiters and hiring managers don't count contribution squares; they look at 2–4 pinned repos with clean READMEs, real functionality and live demos. A README should say what the project does, the stack, how to run it, and give a screenshot or demo link. — [dev.to: GitHub README that gets junior devs past the recruiter scan](https://dev.to/pickuma/the-github-readme-that-gets-junior-developers-past-the-recruiter-scan-4c6a); [Code Labs Academy](https://codelabsacademy.com/en/blog/github-portfolio-junior-developers-what-to-include-remove)
- **[OPINION]** A strong junior portfolio has one bigger "anchor" project plus supporting ones. Nobody expects perfect commits, but ongoing carelessness is noticed. — [Code Labs Academy](https://codelabsacademy.com/en/blog/github-portfolio-junior-developers-what-to-include-remove); [dev.to jsgurujobs](https://dev.to/jsgurujobs/portfolio-code-that-gets-you-hired-what-recruiters-actually-see-in-your-github-h9n)
- **[ANECDOTE]** A candidate was rejected because projects were "too simple / resemble a simple TODO app". — [freeCodeCamp forum](https://forum.freecodecamp.org/t/rate-my-portfolio/422687)
- **[OPINION]** A tutorial project answers "Can this person follow step-by-step instructions?", while the hiring manager asks "Can this person solve problems I have not anticipated yet?" Usage metrics, user feedback and iteration stories make hiring managers "lean forward". — [levelop.dev](https://levelop.dev/blog/the-developer-portfolio-that-lands-interviews-beyond-the-to-do-app); [Hashnode: your portfolio projects need real users](https://hashnode.com/forums/thread/your-portfolio-projects-need-real-users-not-just-code); [Develop Yourself podcast: Build a product, not a portfolio](https://www.buzzsprout.com/1819601/episodes/18568054-300-build-a-product-not-a-portfolio)
- **[OPINION, counterpoint]** A self-described hiring manager on Reddit said they don't care about side projects or users for enterprise hires. Others noted that a unique no-user project can beat a generic one with users, and that active users bring support burden. — [Reddit snapshot](https://reddit.sentinel-team.org/posts/1ppwm6e/snapshots/2025-12-18T19%3A50%3A19.739708Z)
- **[UNRELIABLE STAT]** "80% of top employers want clean, documented code" and "80% of hiring managers value personal projects" circulate without verifiable primary surveys; do not use them. — [placementpreparation.io](https://www.placementpreparation.io/blog/github-profile-portfolio-guide/) (claim origin unclear)

### Inferences
- For a .NET junior, the pre bot becomes a strong anchor project if the README or case study shows:
  1. **Real-usage metrics**: requests, fill rate, median time-to-fill, servers, weekly active users. This is exactly what the validation in §1 and §4 produces, so validation work doubles as portfolio content.
  2. **An architecture write-up**: Discord.NET or NetCord gateway → matching service → PostgreSQL/Redis, with a queueing design note.
  3. **Tests** for the matching logic (xUnit), **CI** (GitHub Actions) and a **deployment** (Docker on a small VPS or a cloud tier).
  4. A "what I learned / what I changed after user feedback" section.
- Pros and cons of the launch options for the portfolio: a partner server gives the most impressive *usage* numbers; top.gg gives a "used in N servers" vanity metric. Both are worth stating honestly.

### Gaps
- No Turkish-market evidence found (Kariyer.net, LinkedIn TR, Turkish dev communities such as Kodlama.io, Teknoseyir or Ekşi threads) on how TR hiring managers weigh side projects. This needs a dedicated search.
- No .NET-specific recruiter source found; the advice is stack-agnostic.

---

## 6. Recommended path for a solo .NET developer with little time

### Takeaway
Validate liquidity by hand in a single dense pool before you build much code. Start with one existing big TR LoL server, seed its filler side with friends, and run only during announced peak windows. Automate only after the fill-rate and time-to-fill thresholds are met. Bot lists and cross-server pools come second.

### Cited Findings
- Constrain first, then expand. — [Lenny's Newsletter, Kickstart pt.1](https://www.lennysnewsletter.com/p/how-to-kickstart-and-scale-a-marketplace)
- Smallest network, unscalable tactics, then replicate: "build one, then two". — [Lenny's Newsletter: The Atomic Network](https://www.lennysnewsletter.com/p/atomic-network)
- Supply-first: most of Lenny's interviewed marketplaces focused early effort on supply. — [Lenny's pt.2](https://www.lennysnewsletter.com/p/how-to-kickstart-and-scale-a-marketplace-9ee)
- A single distinctive feature over multifunction, and top.gg as the main later discovery channel. — [top.gg blog](https://blog.top.gg/top-5-tips-to-organically-grow-your-discord-bot)
- Admin trust: minimal permissions, a support server and docs, no ping spam. — [peakbot.pro](https://peakbot.pro/blog/how-to-tell-if-a-discord-bot-is-safe)
- 100-server cap for unverified bots. — [Discord blog](https://discord.com/blog/the-future-of-bots-on-discord)

### Inferences (recommended plan, my synthesis)
1. **Days 1–2:**
   - Build a minimal bot: `/pre rol rank` (post a request), `/yedek` (join the standby pool), live pool embed, auto-expire after 20 minutes, report command.
   - Use only slash commands and no privileged intents, which avoids the message-content intent and eases admin trust.
   - Recruit 10–20 friends as standby fillers.
2. **Days 2–4:**
   - DM admins of 2–3 large TR LoL servers with a short pitch: a 3-evening trial in one channel, minimal permissions, no DMs or mass pings, removable at any time, and a fill report afterwards.
   - Run a concierge/hybrid test in whichever server says yes first, or in your own server as a fallback.
3. **Days 3–6:**
   - Run "Pre saati" on 3 evenings (for example Fri–Sun 21:00–01:00).
   - Use loose matching (role-only or a wide rank band).
   - Measure requests per evening, fill rate, median time-to-fill and repeat users.
4. **Day 7 decision:**
   - Fill rate ≥ 50% and median ≤ 10 min → harden (tests, CI, Docker deploy), add a second server, then a cross-server pool and a top.gg listing.
   - Below 30% → loosen matching, raise standby seeding, or move to a bigger server; do not add features.
5. **Throughout:** log the metrics into the README or case study for the portfolio.

### Gaps
- None of the recommended thresholds (50% fill, 10-minute median, 20% repeat) are sourced benchmarks for LFG bots; they are proposals adapted from Uber's "ETA < 15 min" analogue and general marketplace logic.
