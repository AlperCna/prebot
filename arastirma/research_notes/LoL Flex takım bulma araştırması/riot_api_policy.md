# Riot Games Developer API & Policies for a TR1 League of Legends Teammate-Finder (Discord bot + ASP.NET Core web app)

Research date: 2026-11-08. Note on freshness: the General Policies page shows "LAST UPDATED: MAY 29, 2025" (latest dated revision found). The Riot developer support site (support-developer.riotgames.com) returned HTTP 403 to automated fetches, so its articles were seen only as search snippets. Some Riot doc pages (e.g. the LoR policy page, footer "© 2021") are old snapshots; flagged inline.

## 1. API key types (Development, Personal, Production): permissions, expiry, rate limits, approval, requirements

### Takeaway
There are three tiers. Development keys expire every 24h and can't run a public product. Personal keys (20 req/1s, 100 req/2min) are only for "just the developer or a small private community" and can't run a public product. Production keys (starting at 500 req/10s, 30,000 req/10min) need a working prototype, a complete and verified website, and a Developer Relations review. Reviews happen roughly weekly and can take up to about 3 weeks. A public Discord bot, even a small one, needs a production key.

### Cited Findings
- **Development key:** "temporarily granted for products that are not meant for public consumption", and "They also deactivate every 24 hours… You'll need to regularly reset yours to keep it live." Also: "You may not maintain a public product with a development API key." — [Riot Developer Portal docs: Portal/Rate Limiting](https://developer.riotgames.com/docs/portal)
- **Personal key purpose:** "Personal API keys should be used for products that are intended for just the developer or a small private community. These products can be registered without the verification process, but won't be approved for rate limit increases." — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Personal key scope:** "You may request access to the Standard APIs, but not the Tournaments API." — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Personal key public-use ban:** "You may not run your application for public consumption using a personal key" — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal). A search snippet of the same docs adds that this covers "open alpha or beta tests, regardless of how long the approval process for your production key takes". The snippet also lists bots for streaming sites, boards and voice servers (i.e. a private Discord) among acceptable personal-key uses — [search snippet of Riot docs/portal](https://developer.riotgames.com/docs/portal) (the full-page fetch did not reproduce the "open alpha/beta" wording, so treat it as unverified paraphrase).
- **Personal key rate limit:** "20 requests every 1 second" and "100 requests every 2 minutes" — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Production key purpose:** "Production API keys should be used for products that are intended for large communities or the Internet as a whole." It covers "the Standard APIs and the Tournaments API." It "Typically requires a working prototype before receiving an API key." — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Production key starting rate limit:** "500 requests every 10 seconds" and "30,000 requests every 10 minutes". These limits "can be expanded if the developer is in good standing, has demonstrated a strong community benefit" — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Approval process:** the developer is "expected to have read and understood our policies". The developer picks between a larger-scale product and a personal project, then completes a form. After that, "we'll ask you to verify your product" and "our Developer Relations team will review your proposal." Also: "If your website isn't complete, we're unlikely to approve your product." And: "The process and length of time required to obtain an approved production key can vary" — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Timeline:** "Applications are typically reviewed weekly for the previous week". The FAQ adds that reviews can sometimes take up to three weeks — [Riot Developer FAQs](https://developer.riotgames.com/docs/faqs)
- **Production prerequisites:** "Production keys are reserved for fully functioning applications that can clearly demonstrate their user flows". They also need a verified website — [Riot Developer FAQs](https://developer.riotgames.com/docs/faqs)
- **Discord bots need a website:** Q: "My product is a Discord bot, do I still need a website?" A: "Yes. Discord bots still need a website so users can easily see what your product does, where to access it…" — [Riot Developer FAQs](https://developer.riotgames.com/docs/faqs)
- **Site verification (the "riot.txt" step):** "Site verification is how we validate that you own the domain where your application is hosted." After submitting a production application you receive "a text string to upload to your URL that proves you're the website owner." The FAQ does not name the file "riot.txt". (Community convention is `/riot.txt` at the domain root, but that is not confirmed by the fetched page.) — [Riot Developer FAQs](https://developer.riotgames.com/docs/faqs)
- **No multi-app rate-limit stacking:** "you may NOT have multiple applications to bypass rate limits." — [Riot Developer FAQs](https://developer.riotgames.com/docs/faqs)
- **One product per production key:** "Do not use a Production API key to run multiple projects (one product per key)" — [Riot General Policies](https://developer.riotgames.com/policies/general)
- **Key secrecy:** "Do not include your API key in your code, especially if you plan on distributing a binary". The portal warns that "your key will likely end up revoked if it isn't properly secured." — [General Policies](https://developer.riotgames.com/policies/general); [Portal docs](https://developer.riotgames.com/docs/portal)
- **Communication channel (dated 26 April 2024):** Riot began moving from in-portal "app messages" to a new support site, but "we'll still be sending approvals/rejection reasons and RSO instructions in app messages." — [Riot Developer FAQs](https://developer.riotgames.com/docs/faqs)
- **Historic Discord-bot stance (old LoR page, © 2021, not LoL-specific):** under "Examples of Unapproved Use Cases" it lists "Discord Bots – May be considered if they are particularly well-developed and Riot sees a potential for growth". It also lists "Apps a small, personal audience" [sic], "Scouting tools", "Apps that do not provide a link to a site or mockup" and "Gambling". "LFG tools" appear under approved use cases — [Riot docs: Legends of Runeterra](https://developer.riotgames.com/docs/lor). A LoL-specific equivalent page (`/policies/lol`) returned 404.
- **VALORANT contrast:** a search snippet of Riot's support site says "Personal Key Applications are currently not supported" for VALORANT. Discord bots showing community leaderboards or stats are accepted there only with player opt-in — [Riot DevRel support: VALORANT](https://support-developer.riotgames.com/hc/en-us/articles/22698769097107-VALORANT) (snippet only; page 403'd)

### Inferences
- A **personal key** is fine for development and for a bot limited to the developer's own private Discord server. Once the bot is invited to arbitrary servers or the web app is public, it is "public consumption", and a **production key** is required.
- Plan for production review: (a) a live, complete website describing the bot and web app, with user flows and screenshots; (b) the site-verification string hosted on that domain; (c) a working prototype (built with dev/personal key data); (d) a ToS and privacy policy page. These are standard expectations, but I found no Riot page that explicitly requires a ToS or privacy policy; see Gaps.
- Realistic timeline is 1 to 3+ weeks per review cycle. Rejections with feedback can add further cycles.
- Historic Riot guidance was skeptical of Discord bots. Pairing the bot with a full web app (an "LFG tool", which the old guidance listed as approved) is the strongest framing for approval.

### Gaps
- Exact current development-key rate limit is not stated on the fetched portal page. It is widely reported as identical to personal (20/1s, 100/2min), but I could not verify that here.
- I found no official statement explicitly requiring a ToS or privacy policy URL in the application form (the support site was 403).
- I found no current (2025–2026) LoL-specific "approved/unapproved use cases" list. The Discord-bot stance is from an older LoR page.
- Per-method rate limits (e.g. match-v5 per-endpoint limits) are visible only in the developer's own portal dashboard and response headers. I found no public source.

## 2. RSO (Riot Sign-On): eligibility and process

### Takeaway
RSO (Riot's OAuth2) is available only to apps that already have an **approved production application**. It is requested after production approval, and Riot sends instructions through the portal's app messages. It is the only officially endorsed account-ownership proof.

### Cited Findings
- "RSO Clients are only available for applications that have an existing, approved production application ID." — [Riot Developer FAQs](https://developer.riotgames.com/docs/faqs)
- Approval/rejection reasons and "RSO instructions" are delivered via in-portal app messages (as of the 26 April 2024 support-site transition) — [Riot Developer FAQs](https://developer.riotgames.com/docs/faqs)
- When removing Third Party Code, Riot DevRel encouraged "all developers who made use of it in the past to apply for RSO in their new or existing Production Applications on the Developer Portal." — [@RiotGamesDevRel on X](https://x.com/RiotGamesDevRel/status/1491217397965258752)
- Hextechdocs: "users are encouraged to migrate over to RSO in lieu of using this endpoint." — [Hextechdocs (obsolete third-party-code guide)](https://hextechdocs.dev/using-third-party-code-to-verify-account-ownership/)

### Inferences
- Sequence: build the MVP with icon-based verification on a personal/dev key, obtain the production key, request an RSO client, then migrate verification to an "Sign in with Riot" OAuth flow in ASP.NET Core (standard OIDC/OAuth2 handler).
- RSO is not "invite-only" in a formal sense, but it is gated behind production approval.

### Gaps
- Exact RSO request form, scopes, review time, and whether every approved production app gets RSO on request. The support site was inaccessible.

## 3. Account ownership verification without RSO (profile icon method; Third Party Code removal)

### Takeaway
The Third Party Code endpoint (`/lol/platform/v4/third-party-code/by-summoner/{id}`) was announced in Feb 2022 for deprecation "by 3/7/22". Hextechdocs confirms it was "no longer usable as of the 7th of March 2022". **The 7 March 2022 claim is verified.** Without RSO, the community workaround is the profile-icon challenge: ask the user to equip a specific (usually free/default) icon, then poll Summoner-V4 `profileIconId`. No Riot doc endorses it.

### Cited Findings
- Riot DevRel: "Due to legacy tech and security concerns, we are deprecating the Third Party Code API endpoint by 3/7/22 and encouraging all developers who made use of it in the past to apply for RSO…" — [@RiotGamesDevRel on X (Feb 2022)](https://x.com/RiotGamesDevRel/status/1491217397965258752)
- "…no longer usable as of the 7th of March 2022." — [Hextechdocs: [Obsolete] Using third-party-code to verify account ownership](https://hextechdocs.dev/using-third-party-code-to-verify-account-ownership/)
- Client-library confirmation (later date, library only): RiotWatcher v3.2.3 (28 June 2022) "removed LoL 3rd Party Code API, as has been removed by riot" — [Riot-Watcher GitHub](https://github.com/pseudonym117/Riot-Watcher)
- Open-source bots surveyed:
  - simwai/lol-ranks-bot offers "summoner name verification" (`enableVerification`) but the README does not describe the proof mechanism. It is configured with a "Riot development API key" — [lol-ranks-bot GitHub](https://github.com/simwai/lol-ranks-bot)
  - A private community bot resolves the registered Riot ID to a PUUID via Account-V1 and checks that the Discord nickname matches. That is weak and not real proof of ownership — [dnbc community bot page](https://dnbc.duckdns.org/)
- Hextechdocs does not mention the icon method as an alternative; it points only to RSO — [Hextechdocs](https://hextechdocs.dev/using-third-party-code-to-verify-account-ownership/)

### Inferences
- **Icon-challenge implementation (community practice, not Riot-documented):**
  1. Resolve the Riot ID to a PUUID (Account-V1, `europe`).
  2. Call Summoner-V4 by-puuid on `tr1` to read the current `profileIconId`.
  3. Pick a random icon from the default free set (IDs 0–28 are commonly used) that differs from the current one.
  4. Tell the user to equip it within ~5–10 minutes.
  5. Re-poll Summoner-V4 and match `profileIconId`.
  6. Store the PUUID↔Discord ID link, and optionally let the user revert the icon.

  Cost is about 2–5 API calls per verification. Summoner-V4 data can be cached for a short time, so poll on a "Verify" button press rather than continuously.
- I could not find a GitHub bot whose README explicitly documents the icon flow; see Gaps. Treat the icon method as a stopgap until RSO.

### Gaps
- I found no primary-source example repo that documents the icon flow. The search did not surface one, and that statement comes from general community knowledge.
- I found no Riot statement on whether the icon challenge is acceptable for production approval.
- Propagation delay of `profileIconId` changes in the API is not documented.

## 4. Endpoints & routing for TR1 (Account-V1, Summoner-V4, League-V4, Match-V5, Clash-V1, Spectator-V5, Champion-Mastery-V4) and deprecations

### Takeaway
TR1 platform host is `tr1.api.riotgames.com`. Regional endpoints (Account-V1, Match-V5) use `europe.api.riotgames.com`, because "The EUROPE routing value serves EUNE, EUW, ME1, TR and RU". Summoner names were replaced by Riot ID (Nov 20, 2023). On **June 20, 2025** Riot removed the summonerId/accountId-based League/Summoner endpoints, so everything should be PUUID-keyed. League-V4 returns `RANKED_FLEX_SR` entries. Match-V5 can filter by `queue=440` and returns the participant PUUIDs needed to verify that 5 players were in one match.

### Cited Findings
- Platform routing: TR1 → `tr1.api.riotgames.com`. Regional values: `americas`, `asia`, `europe`, `sea`. Account-V1 can be queried on americas/europe/asia, and Riot recommends the cluster closest to your servers — [Riot docs: League of Legends](https://developer.riotgames.com/docs/lol)
- Match-V5 routing: "The EUROPE routing value serves EUNE, EUW, ME1, TR and RU." — [Mirror of Riot match-v5 docs (riot-docs-mcp)](https://raw.githubusercontent.com/chirpon9/riot-docs-mcp/main/src/riot_docs/match-v5.md) (third-party mirror of official API reference)
- Match-V5 ids: `GET /lol/match/v5/matches/by-puuid/{puuid}/ids`. Filters: `queue` (int), `type` (ranked/normal/tourney/tutorial), `start` (default 0), `count` (default 20, **max 100**), `startTime` (epoch seconds; matches before June 16, 2021 are excluded when it is set). Participant fields include `puuid`, `riotIdGameName` and `teamId`. Metadata includes "A list of participant PUUIDs." — [Mirror of match-v5 docs](https://raw.githubusercontent.com/chirpon9/riot-docs-mcp/main/src/riot_docs/match-v5.md)
- **Riot ID migration:** starting November 20, 2023, Riot moved "away from Summoner Names to using Riot ID". Summoner-V4 by-name endpoints are deprecated and "will be removed in the future". New summoners get a random uuid-style summoner name, and the name field will eventually be removed — [Riot docs: League of Legends](https://developer.riotgames.com/docs/lol)
- **June 20, 2025 removals** (Riot DevRel): "we are removing many endpoints from the Riot API associated with AcountIDs and SummonerIDs on June 20th. Moving forward, please use the PUUID equivalents" — [@RiotGamesDevRel on X](https://x.com/RiotGamesDevRel/status/1932188110454235582). The mapping quoted from Riot's email is:
  - `/lol/league/v4/entries/by-summoner/{encryptedSummonerId}` → `/lol/league/v4/entries/by-puuid/{encryptedPUUID}`
  - `/lol/summoner/v4/summoners/by-account/{encryptedAccountId}` → `/lol/summoner/v4/summoners/by-puuid/{encryptedPUUID}`
  - `/lol/summoner/v4/summoners/{encryptedSummonerId}` → `/lol/summoner/v4/summoners/by-puuid/{encryptedPUUID}`
  - the TFT equivalents

  Source: [Riot-Watcher issue #254 (opened June 10, 2025)](https://github.com/pseudonym117/Riot-Watcher/issues/254)
- **Clash-V1:** in January 2025 Riot said it "will be removing the by summoner ID endpoint from clash-V1 with the release of the next LoL patch and replacing it with a by puuid endpoint" — [@RiotGamesDevRel on X](https://x.com/RiotGamesDevRel/status/1881759755380945009)
- **League-V4 flex:** valid queue values are `RANKED_SOLO_5x5`, `RANKED_FLEX_SR` and `RANKED_FLEX_TT`. `entries/by-puuid` returns entries for all queues. If a player is unranked or still in placements in a queue, that queueType entry is absent — [darkintaqt league-v4 guide](https://darkintaqt.com/blog/league-v4-summoner); [riot-docs-mcp league-v4 mirror](https://glama.ai/mcp/servers/@chirpon9/riot-docs-mcp/blob/bd23bbcbe155efed4c6774a7dde0bb8020d76191/src/riot_docs/league-v4.md). Note: the Riot LoL docs page itself mentions only `RANKED_SOLO_5x5` and `RANKED_TEAM_5x5` in its ranked-section text, which is outdated wording — [Riot docs: LoL](https://developer.riotgames.com/docs/lol)
- **Spectator-V5:** `GET /lol/spectator/v5/active-games/by-summoner/{encryptedPUUID}` (platform route, e.g. tr1) returns current game info. A 404 means the player is not in a game — [riot-docs-mcp spectator-v5 mirror](https://glama.ai/mcp/servers/@chirpon9/riot-docs-mcp/blob/bd23bbcbe155efed4c6774a7dde0bb8020d76191/src/riot_docs/spectator-v5.md); [apis.io Spectator API](https://apis.io/apis/riot-games/riot-games-spectator-api/) (third-party mirrors; one older mirror still shows `{encryptedSummonerId}`)
- Riot recommends "employing PUUID endpoints when available" — [Riot docs: LoL](https://developer.riotgames.com/docs/lol)
- Deprecation policy: Riot aims to "support both old and new versions of the API for 60 days", after which calls to a deprecated endpoint error — [Riot Portal docs](https://developer.riotgames.com/docs/portal)
- Queue IDs reference: the full list is in `queues.json`, linked from [Riot docs: LoL](https://developer.riotgames.com/docs/lol). 440 = Ranked Flex 5v5 (standard community knowledge; confirm in queues.json).

### Inferences
- **Endpoint plan for TR1 (all PUUID-keyed):**
  - Account-V1 `/riot/account/v1/accounts/by-riot-id/{gameName}/{tagLine}` on `europe` (Riot ID → PUUID).
  - Summoner-V4 by-puuid on `tr1` (icon, level).
  - League-V4 `entries/by-puuid` on `tr1` (Solo + Flex tier/LP).
  - Match-V5 ids + match detail on `europe` (filter `queue=440&type=ranked`).
  - Champion-Mastery-V4 by-puuid on `tr1`.
  - Spectator-V5 active-games by-summoner/{puuid} on `tr1` ("is in game" status).
  - Clash-V1 by-puuid on `tr1` (Clash team/registration).
- **Verifying that 5 players played together:** fetch the match by ID and check that all 5 PUUIDs appear in `metadata.participants`, and that they share the same `teamId` in `info.participants`. Efficient approach: take the intersection of the `queue=440` match-ID lists of the 5 PUUIDs (5 calls), then fetch only the common IDs.
- Do not store or key on summonerName, summonerId or accountId. Display `riotIdGameName#riotIdTagline`.
- Spectator is useful for "online/in-game" badges, but it is a live lookup per user (1 call each). Throttle it.

### Gaps
- I could not fetch the official API reference pages (JS-rendered), so the Spectator-V5, Clash-V1 and Champion-Mastery-V4 paths come from third-party mirrors.
- Whether summoner DTO fields `id`/`accountId` are now fully absent from responses in 2026 was not confirmed.
- Whether Spectator-V5 still returns participants' Riot IDs, and any 2025–26 changes to Spectator, were not confirmed.

## 5. Policy constraints: alternative ranking/MMR, reputation ratings, toxicity/shaming, data retention, monetization, gambling, Discord bots

### Takeaway
The General Policies (last updated May 29, 2025) ban alternatives to official ranking ("MMR or ELO calculators"), ban de-anonymization, and ban betting/gambling. They require registration and audit of every product **and every new feature**. Monetization requires an Approved/Acknowledged registration plus a free tier, and paid content must be "transformative". There is no explicit clause on reputation/behavior ratings, toxicity or data retention, and no Discord-bot-specific clause, in the current General Policies.

### Cited Findings
- Last updated: "LAST UPDATED: MAY 29, 2025" — [Riot General Policies](https://developer.riotgames.com/policies/general)
- **Ranking ban:** "Products cannot create alternatives for official skill ranking systems such as the ranked ladder." "Prohibited alternatives include MMR or ELO calculators." — [General Policies](https://developer.riotgames.com/policies/general)
- **De-anonymization:** "Products cannot de-anonymize players who cannot reasonably be identified from visible information." — [General Policies](https://developer.riotgames.com/policies/general)
- **Registration and audit:** "All products must be registered in, and audited by Riot Games through the Developer Portal". "Any new features or changes to a product must be audited through the product's page in the Developer Portal" — [General Policies](https://developer.riotgames.com/policies/general)
- **Monetization:**
  - "You may monetize your product as long as your product is registered on the Developer Portal" and "your product status is either Approved or Acknowledged."
  - "You must have a free tier of access for players, which may include advertising."
  - "Your content must be transformative if you are charging players for it."
  - Acceptable models include "Subscriptions, donations, or crowdfunding", "Entry fees for tournaments", and "Currencies that cannot be exchanged back into fiat."
  - "Your monetization cannot gouge players or be unfair."
  - "If you are unsure if your monetization platform is acceptable, contact us through the Developer Portal."
  - "You may not place advertisements in Riot properties, which include in-game, loading screens, and the Riot Client."

  Source: [General Policies](https://developer.riotgames.com/policies/general)
- **Gambling:** "Your product cannot feature betting or gambling functionality." Tournaments must "Not include any gambling", "Must have at least 20 participants", and must "Allot at least 70% of the entry fees to the prize pool" — [General Policies](https://developer.riotgames.com/policies/general)
- **Enforcement:** "Failure to meet these policies may result in either suspension or cancellation of your API access or legal recourse." — [General Policies](https://developer.riotgames.com/policies/general)
- **Reputation/toxicity/data retention/Discord bots:** the fetched General Policies contain no text on player ratings or reputation, shaming or toxicity, data retention or deletion, or Discord bots — [General Policies](https://developer.riotgames.com/policies/general)
- Older LoR-scoped guidance lists "Applications or sites that present a premium subscription system as monetization strategy, with the exception of sites suggesting a pay-per-win type of model" as approved. It also lists "Scouting tools" and "middle-man" data usage as unapproved — [Riot docs: LoR (© 2021)](https://developer.riotgames.com/docs/lor)
- 429 policy: "halt future API calls for the duration, in seconds, indicated by the Retry-After header." "Applications that are in violation of this policy may have their access disabled" — [Riot Portal docs](https://developer.riotgames.com/docs/portal)

### Inferences
- **Feature design implications:**
  - Do **not** compute an in-house MMR/ELO or a "true skill" score, and do not rank players on a custom ladder. Show official tier/LP from League-V4. Simple descriptive stats (winrate, champion pool, role frequency from Match-V5) are generally considered "aggregate stats"/training data, which the old guidance listed as approved.
  - **Reputation/behavior ratings** (e.g. "reliable teammate", thumbs-up/down) are not explicitly banned. They are a reviewer-discretion risk, because they could read as an alternative ranking or as enabling harassment or shaming. Safer options: positive-only endorsements, opt-in profiles, no public negative scores, moderation and reporting, and pre-clearance with Riot during the audit (Riot asks for new features to be audited anyway).
  - Users should **opt in** (link their own account). Don't let users look up or publicly label other players who haven't opted in, because of the de-anonymization rule and the "scouting" concerns in the older guidance.
  - Monetization: a free core LFG experience is mandatory. Premium features must be transformative, e.g. advanced filters, priority listing or analytics. No gambling, betting or wagers on match outcomes. No ads inside Riot properties. Monetization may only begin once the product status is Approved/Acknowledged.
- The absence of a data-retention clause in the General Policies does not remove obligations under Riot's Developer API Terms (not fetched) or Turkish KVKK and GDPR. Plan deletion on account unlink, and refresh data rather than archiving it indefinitely.

### Gaps
- The Riot Developer API Terms of Service (`developer.riotgames.com/terms`) were not fetched. Clauses on data caching, retention, deletion and attribution ("isn't endorsed by Riot Games" legal boilerplate) are unverified here.
- There is no current LoL-specific game policy page. `/policies/lol` returned 404, so I could not confirm whether "Discord bots" or "LFG" stances from the old LoR page still apply to LoL.
- I found no Riot statement on behavior or reputation rating systems.

## 6. Rate-limit implications: how many users can a personal vs production key support when syncing the last N matches?

### Takeaway
App limits are enforced **per region/routing value**, so `europe` (Account-V1 and Match-V5) and `tr1` (Summoner, League, Spectator, Mastery) have separate buckets. Match-V5 on `europe` is the bottleneck. A personal key (100 req/2 min ≈ 72,000 req/day per routing value) can support at most a few thousand lightly active users in theory, but it is not allowed for public use anyway. A production key (30,000 req/10 min ≈ 4.32M req/day per routing value) supports tens to hundreds of thousands of users with incremental sync. Per-method limits (not public) may bind first.

### Cited Findings
- "App rate limits are enforced per region." Method limits apply "on a per endpoint (or "method") basis for a given API key." "Service rate limits… are shared by all applications making calls to a service." — [Riot Portal docs](https://developer.riotgames.com/docs/portal)
- Personal: "20 requests every 1 second", "100 requests every 2 minutes". Production start: "500 requests every 10 seconds", "30,000 requests every 10 minutes" — [Riot Portal docs](https://developer.riotgames.com/docs/portal)
- Match-V5 ids `count` max is 100, and the `startTime` filter enables incremental sync — [match-v5 mirror](https://raw.githubusercontent.com/chirpon9/riot-docs-mcp/main/src/riot_docs/match-v5.md)
- No multi-app splitting: "you may NOT have multiple applications to bypass rate limits." — [Riot FAQs](https://developer.riotgames.com/docs/faqs)
- .NET: Camille (NuGet `Camille.RiotGames`) advertises that it "handles rate limits and large requests with ease", with retries and `MaxConcurrentRequests`, plus nightly builds that track Riot API changes — [Camille GitHub](https://github.com/MingweiSamuel/Camille)

### Inferences
These are my own calculations from the cited limits.
- **Sustained throughput per routing value:** personal is 100/120 s = 0.83 req/s ≈ 72,000/day. Production is 30,000/600 s = 50 req/s ≈ 4,320,000/day (the 500/10s burst limit is the same average).
- **Initial backfill of the last N=20 flex matches per user:** 1 ids call + 20 detail calls = 21 europe calls.
  - Personal: about 3.4 users per minute (~25 min for 100 users).
  - Production: about 140 users per minute (~4,900 users/hour), app-limit-wise.
- **Incremental sync** (1 ids call with `startTime`, plus details for new matches only). Assume a user plays ~3 flex games/day and is polled every 6 h: (4 ids + 3 details) = ~7 europe calls/user/day, plus ~1–4 tr1 calls (League-V4 refresh).
  - Personal: ~72,000 / 7 ≈ 10,000 users/day, theoretical ceiling.
  - Production: ~4.32M / 7 ≈ 600,000 users/day, theoretical ceiling.
  - Real ceilings are lower because of per-method limits, retries, Spectator polling and verification calls. Keep a 30–50% safety margin.
- **Big savings come from cache and dedup:**
  - Matches are immutable, so store each match once. Five teammates share a match, which cuts detail calls by up to 5x for a teammate-finder whose users play together.
  - Fetch match details only for `queue=440`.
  - Poll Spectator on demand, not periodically.
  - Use Account-V1 only at linking time.
- **Architecture for .NET:** a single central rate-limited worker (BackgroundService plus a queue) that honors `Retry-After` and the `X-App-Rate-Limit`/`X-Method-Rate-Limit` headers, or Camille's built-in limiter. Never call Riot directly from Discord command handlers without the shared limiter. Use distinct limiter buckets for `europe` and `tr1`.

### Gaps
- I found no public figures for per-method limits (e.g. match-v5 `matches/{id}`, spectator-v5) for production keys. These appear in the app's own portal dashboard and response headers.
- I found no public data on how readily Riot grants production rate-limit increases or how large they are.
- Camille's current version, release date and maintenance status for 2026, and whether it exposes `RegionalRoute.EUROPE` / `PlatformRoute.TR1`, were not confirmed from the README. Its example still uses the deprecated `GetBySummonerName`, which suggests the README is outdated.
