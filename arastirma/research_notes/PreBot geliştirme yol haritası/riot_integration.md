# Riot Games Developer Platform Integration for PreBot (LoL LFG, TR1), as of Nov 2026

Scope: what PreBot (open-source, non-commercial .NET 10 PWA, "we're 4 in Ranked Flex, need a 5th") can integrate from Riot's developer platform, what policy allows, and concrete designs. Research date: 2026-11-09. Most Riot doc pages are undated, so they are treated as current but flagged where that is uncertain.

## 1. API key types, application process, and whether an LFG product is allowed

### Takeaway
LFG tools are explicitly listed by Riot as an approved League use case. PreBot is a public website, so the policy-correct path is a Production key: register the product, verify the site (upload a text string to your domain), have live ToS and Privacy pages, and show a working app. Reviews run weekly and can take up to about 3 weeks. A Personal key (20 req/s, 100 req/2 min) is meant for small or personal projects, and the portal says it is not for public use. Policy also bans MMR/ELO calculators and alternative ranking, and bans de-anonymizing hidden players.

### Cited Findings
**Key types**
- Development key: issued automatically at login, for prototyping only, "deactivate every 24 hours" — [Riot Dev Portal docs](https://developer.riotgames.com/docs/portal)
- Personal key: you get it by registering a product. There is no verification step and no rate-limit increase. Standard APIs only, no Tournaments. Limits are 20 requests per 1 s and 100 requests per 2 min. Per the docs (as summarized by the fetch tool), it is not for public use, "including open alpha or beta tests". — [Riot Dev Portal docs](https://developer.riotgames.com/docs/portal)
- Production key: for public products with large communities. Standard plus Tournaments APIs. Starts at 500 requests per 10 s and 30,000 per 10 min, per region, and can be raised for developers in good standing. — [Riot Dev Portal docs](https://developer.riotgames.com/docs/portal)
- "Personal keys are meant for smaller-size, personal projects"; "Production keys are meant for larger-size, professional projects." — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- Each production project needs its own registration and approval. Reviewers look for products that help players measurably and for complete websites; incomplete sites are unlikely to be approved. A working prototype is typically required. — [Riot Dev Portal docs](https://developer.riotgames.com/docs/portal)

**Production requirements (FAQ)**
- "For production keys, we need to see a functioning website/application or a prototype", in order to "understand what your app does, view your Terms of Service and Privacy Policy" — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- "Production keys are reserved for fully functioning applications that can clearly demonstrate their user flows." No website usually means no production key; early projects should apply for a personal key. — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- "we are unable to accept Github repositories and source code in lieu of a functioning application/site." Discord bots also need a website. Native apps must be hosted on a domain you own. — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- Site verification: "you'll receive a text string to upload to your URL that proves you're the website owner"; "We cannot grant production keys to applications without a verified website"; there are no alternative ways to prove ownership. — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- Review timing: "Applications are typically reviewed weekly for the previous week"; "sometimes the review process can take up to three weeks"; replies to messages can take up to two weeks. — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- One product or website per API key. A key is approved only for the use case and game in its description. You may not register multiple applications to get around rate limits. — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- Unofficial claim: approval took anywhere from 12 hours to 3–4 months. This is a community gist, not Riot. — [Henrik-3 gist](https://gist.github.com/Henrik-3/d6b631fb7c61821bc16b17cd347a3811)

**Policy: allowed and forbidden**
- Riot's approved League use cases include "LFG tools", self player stats, tournaments, pre-game static data overlays, aggregate stats without specific players, and official ladder leaderboards. — [Riot LoL docs/policies](https://developer.riotgames.com/docs/lol)
- Game Integrity: products cannot alter the goal of the game or create unfair advantages. Products should not remove game decisions, though they may highlight them. "Alternatives to official ranking systems are prohibited, including MMR and ELO calculators." "Products cannot de-anonymize players who cannot reasonably be identified from visible information." — [Riot General Policies](https://developer.riotgames.com/policies/general)
- LoL-specific rules: products cannot identify or analyze players the game deliberately hides, and cannot use information absent from the client that gives an edge. Augment and Arena win rates may not be displayed. Custom match history can be shown publicly only if the player opts in; otherwise only that player can see it, through RSO. — [Riot LoL docs/policies](https://developer.riotgames.com/docs/lol)
- All products must be registered and audited through the Developer Portal, and any new feature must also be audited there. Use HTTPS. Never put the API key in code, especially distributed binaries. — [Riot General Policies](https://developer.riotgames.com/policies/general)
- Monetization requires Approved or Acknowledged status and a free tier. Betting, gambling, and crypto/blockchain are banned. Ads may not appear inside Riot properties. — [Riot General Policies](https://developer.riotgames.com/policies/general)
- Required boilerplate: "[Your product] isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing Riot Games properties." It must be shown where players can readily see it. — [Riot General Policies](https://developer.riotgames.com/policies/general)
- Deprecation policy: old and new versions are typically supported in parallel for about 60 days. Calls to removed endpoints return errors. — [Riot Dev Portal docs](https://developer.riotgames.com/docs/portal)

### Inferences
- PreBot fits the explicitly approved "LFG tools" category. Showing the official tier/division/LP from league-v4 is in line with "official ladder" use. Showing a computed "true MMR" or an "elo estimate" would violate policy, as would hiding the official rank and replacing it with a PreBot score.
- PreBot is already public, so running it on a Personal key goes against the wording that personal keys are not for public use. Two realistic paths: (a) use a personal key only while developing and testing with a small closed group, then (b) apply for Production once a public site exists on a domain the project owns. That site needs a /terms page, a /privacy page, the Riot disclaimer, and a demo flow. GitHub-only submissions are rejected.
- The Production application should describe the exact endpoints and features (Riot ID lookup, rank display, RSO verification). Adding features later, such as match-v5 checks, needs a re-audit note in the portal.
- Open source is compatible with policy as long as the key stays out of the repo, for example in user-secrets or environment variables. The project already keeps the Discord token out of the repo, and the Riot key should follow the same pattern.

### Gaps
- I found no explicit Riot developer-policy clause on "boosting" or on LFG products specifically, beyond LFG being an approved use case. Riot's ToS forbids account sharing and boosting, but I did not fetch the ToS text here. A PreBot rule against "boost/elo job" listings would be prudent, but it is not a sourced API requirement.
- Whether Riot approves a TR-only niche LFG app for production (the "large communities" wording) is not documented. Approval decisions are case by case.
- The exact numeric method-level limits (per endpoint) are not on the docs pages I fetched. They come back in response headers (X-Method-Rate-Limit) and the portal's app page.

## 2. RSO (Riot Sign On) for verifying account ownership

### Takeaway
RSO is OAuth2/OIDC from Riot and the proper way to prove someone owns a Riot account. It is available only to apps that already have an approved Production key; approved apps get an invitation link to apply for an RSO client. After login you call `/riot/account/v1/accounts/me` with the user's access token and receive their PUUID, gameName and tagLine. Riot's old third-party-code verification endpoint is absent from the current API schema, so without RSO the only verification left is a community "change your profile icon" trick that reads `profileIconId` from summoner-v4.

### Cited Findings
- RSO is available only with Production keys. Players log in through the authorize link with `openid offline_access` scopes and are returned to your `redirect_uri`. Then you call `/riot/account/v1/accounts/me` on the regional cluster closest to your servers; all clusters return identical data. — [Riot LoL docs](https://developer.riotgames.com/docs/lol)
- "RSO Clients are only available for applications that have an existing, approved production application ID." "All newly approved production applications will receive a link inviting them to apply for RSO access." Multiple apps share one RSO client. — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- RSO implementation guidance is provided in the Developer Portal after an application is approved. That statement comes from the Riftbound page, but it describes the general RSO process. — [Riot Riftbound docs](https://developer.riotgames.com/docs/riftbound)
- Endpoints that need an RSO token: `GET /riot/account/v1/accounts/me` and `GET /lol/summoner/v4/summoners/me` (both need the `openid` scope), plus `GET /lol/match/v5/matches/replays`. — [riotapi-schema OpenAPI (MingweiSamuel)](https://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json)
- In the current OpenAPI schema, no `third-party-code` endpoint exists. The only "code" endpoints belong to tournament-v5. — [riotapi-schema OpenAPI](https://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json)
- SummonerDTO still exposes `profileIconId`, `puuid`, `summonerLevel` and `revisionDate`. `id` (summonerId) is marked deprecated and "will be removed". — [riotapi-schema OpenAPI](https://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json)
- Riot's RSO support article ([link](https://support-developer.riotgames.com/hc/en-us/articles/22801670382739-RSO-Riot-Sign-On)) returned HTTP 403 to automated fetch. It appears in search results, but its details were not verified.

### Inferences
- Recommended staged design:
  1. **Now (no key or personal key):** keep self-declared Riot IDs, but validate that the account exists with `account-v1 by-riot-id` and normalize capitalization.
  2. **Interim "soft verification":** ask the user to set a specific, randomly chosen default profile icon (one every account owns, IDs 0–28), then poll `summoner-v4 by-puuid` until `profileIconId` matches, within a 5–10 minute TTL. This is a widely used community pattern, but I found no Riot source that endorses it. It proves control of the account cheaply and touches no forbidden data.
  3. **After Production approval:** apply for an RSO client. Use the "Riot ile bağlan" (connect with Riot) flow: authorization code from `https://auth.riotgames.com/authorize`, then token exchange server-side, then `accounts/me`, then store the PUUID and show a "verified" badge. ASP.NET Core's generic OpenIdConnect/OAuth handler can implement it.
- RSO depends on the Production approval chain (Production key, then invite, then RSO application). Realistically that means weeks to months, so treat icon verification as the near-term deliverable.
- Store the PUUID as the stable primary identity. Riot IDs can change, and the PUUID survives name changes and region transfers ([darkintaqt blog](https://darkintaqt.com/blog/ids), secondary source).

### Gaps
- Exact RSO URLs (token endpoint `https://auth.riotgames.com/token`, the `cpid` parameter, etc.) and the RSO application form are not on public pages I could fetch. They are in the portal after approval. The authorize URL above comes from general knowledge, not a fetched source in this session.
- I found no Riot statement about whether the icon trick is allowed or forbidden. It uses only public summoner data, so it is unlikely to conflict with policy, but this is unverified.
- Riot published no 2026 change to RSO availability that I could find.

## 3. Endpoints for PreBot on TR1 / EUROPE, with deprecations through 2026

### Takeaway
Use PUUID everywhere. The routing split:
- **Regional host `europe.api.riotgames.com`:** account-v1 and match-v5.
- **Platform host `tr1.api.riotgames.com`:** summoner-v4, league-v4, champion-mastery-v4, spectator-v5 and lol-status-v4.

On 2025-06-20 Riot removed the summonerId and accountId lookup endpoints. Summoner-name `by-name` lookups are gone, and SummonerDTO.id is deprecated.

### Cited Findings
**Routing**
- Platform hosts follow the pattern `tr1.api.riotgames.com`. The regional hosts are `americas`, `asia`, `europe` and `sea`. — [Riot LoL docs](https://developer.riotgames.com/docs/lol)

**Endpoints (exact paths)**
| Purpose | Endpoint | Host |
|---|---|---|
| Riot ID to PUUID | `GET /riot/account/v1/accounts/by-riot-id/{gameName}/{tagLine}` | europe |
| PUUID to Riot ID (refresh name) | `GET /riot/account/v1/accounts/by-puuid/{puuid}` | europe |
| RSO identity | `GET /riot/account/v1/accounts/me` | europe |
| Icon, level | `GET /lol/summoner/v4/summoners/by-puuid/{encryptedPUUID}` | tr1 |
| Rank (all queues) | `GET /lol/league/v4/entries/by-puuid/{encryptedPUUID}` (filter `queueType == "RANKED_FLEX_SR"`) | tr1 |
| Mastery | `/lol/champion-mastery/v4/champion-masteries/by-puuid/{encryptedPUUID}`, `.../top?count=N`, `.../by-champion/{championId}`, `/lol/champion-mastery/v4/scores/by-puuid/{encryptedPUUID}` | tr1 |
| Match ids | `GET /lol/match/v5/matches/by-puuid/{puuid}/ids?startTime&endTime&queue&type&start&count` | europe |
| Match detail | `GET /lol/match/v5/matches/{matchId}` (TR ids look like `TR1_...`) | europe |
| Live game | `GET /lol/spectator/v5/active-games/by-summoner/{encryptedPUUID}` (404 means not in game) | tr1 |
| Server status | `GET /lol/status/v4/platform-data` | tr1 |

Sources: [riotapi-schema OpenAPI](https://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json) (paths, query params, routing type); [Riot LoL docs](https://developer.riotgames.com/docs/lol) (accounts/me).

- match-v5 `ids` query params: `startTime` and `endTime` (epoch seconds), `queue` (int), `type` (ranked/normal/tourney/tutorial), `start` (default 0), `count` (default 20, max 100). — [riotapi-schema OpenAPI](https://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json)
- spectator-v5 CurrentGameInfo includes `gameId`, `gameStartTime` (ms), `gameLength` (s), `gameQueueConfigId` (optional), and `participants[]` with `puuid` (null when the player is anonymous), `riotId`, `teamId` and `championId`. There is no summonerId or summonerName. — [riotapi-schema OpenAPI](https://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json)
- match-v5 ParticipantDto requires `puuid` and `teamId`. `riotIdName` is deprecated in favor of `riotIdGameName`. — [riotapi-schema OpenAPI](https://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json)

**Deprecations**
- Removed 2025-06-20, with replacements:
  - `/lol/league/v4/entries/by-summoner/{encryptedSummonerId}` → `/lol/league/v4/entries/by-puuid/{encryptedPUUID}`
  - `/lol/summoner/v4/summoners/by-account/{encryptedAccountId}` and `/lol/summoner/v4/summoners/{encryptedSummonerId}` → `/lol/summoner/v4/summoners/by-puuid/{encryptedPUUID}`
  - The same changes were made to the TFT equivalents.
  — [Riot-Watcher issue #254 (quotes Riot email)](https://github.com/pseudonym117/Riot-Watcher/issues/254); [RiotGamesDevRel on X](https://x.com/RiotGamesDevRel/status/1932188110454235582)
- Summoner `by-name` endpoints are deprecated, and front ends must use gameName + tagLine. Summoner names are no longer player-facing. — [Riot LoL docs](https://developer.riotgames.com/docs/lol); [Summoner Name to Riot ID FAQ](https://developer.riotgames.com/docs/summoner-name-to-riot-id-faq)
- SummonerDTO `id` is deprecated ("will be removed"). — [riotapi-schema OpenAPI](https://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json)
- spectator-v4 was replaced by spectator-v5 (Camille commit, April 2024). — [Camille commits](https://github.com/MingweiSamuel/Camille/commits/release/3.x.x)

### Inferences (concrete PreBot designs)
- **D1 Riot ID validation at listing/join time.** Call `by-riot-id` on europe and store the PUUID. On 404, show "Riot ID bulunamadı" ("Riot ID not found"). This removes typos for the host, who currently copies the ID by hand. It costs 1 call, cached for about 24 h.
- **D2 Verified Flex rank badge.** Call `league-v4 entries/by-puuid` on tr1, choose the `RANKED_FLEX_SR` entry, and show tier, rank, LP and W/L. Show "Unranked" when it is missing. Cache 10–30 min. When self-declared and API rank differ, display the API rank. This is "official ladder" data, so it is allowed.
- **D3 Role/champ hint (optional).** Fetch `champion-mastery-v4 .../top?count=3` and show it on the joiner card. Cache about 6 h.
- **D4 Auto-close or "in game" detection.** When a listing is "Full" or "Waiting", poll `spectator-v5 active-games/by-summoner/{hostPuuid}` every 60–120 s, for the host only, to save rate limit. If it returns a game with `gameQueueConfigId == 440` (Ranked Flex) and the joiner's PUUID is among the participants on the same `teamId`, mark the listing "Oyunda" ("in game") and close it. An anonymous `puuid` of null means the player can't be matched, so fall back to manual close. Do not display other participants' details (de-anonymization rule).
- **D5 Did the group actually play / no-show stats.** After a listing closes, call `match-v5 by-puuid/{hostPuuid}/ids?queue=440&startTime={listingClosedAt}&count=5` on europe, fetch the newest match, and check whether the joiner's PUUID is on the host's team. Use the result only for internal statistics, such as the doluluk ("fill rate") statistics page and reliability counters. Per the LoL policy, showing match history publicly needs player opt-in, so keep any public "played together" or "no-show" indicator opt-in or aggregate.
- **D6 Maintenance banner.** Poll `lol/status/v4/platform-data` on tr1 every 5 min. If `maintenances` or `incidents` is non-empty, show a banner on the PWA and in the Discord announcement.
- Queue id 440 = Ranked Flex SR and 420 = Ranked Solo/Duo come from Riot's queues.json static data. That file was not fetched here, so verify against `https://static.developer.riotgames.com/docs/lol/queues.json`. The 2026 "Ranked 5s" queue id is undocumented (see section 7).

### Gaps
- The LeagueEntryDTO schema for LoL (whether `summonerId` still appears in responses and whether `puuid` is included) was not visible in the fetched schema chunks. Code should not depend on `summonerId`.
- The lol-status-v4 PlatformDataDto fields were not verified in this session. The field names above (`maintenances`, `incidents`) come from general knowledge.

## 4. Rate limits, caching, and .NET libraries

### Takeaway
Personal key limits are 20 requests per 1 s and 100 per 2 min. Production starts at 500 per 10 s and 30,000 per 10 min per region. Limits are per region, so tr1 and europe count separately, and every method also has its own limits. Camille (`Camille.RiotGames`, by MingweiSamuel) is still the only actively maintained .NET wrapper, but it is slow-moving: the last NuGet nightly was June 2025 and a commit landed in August 2026. RiotSharp is effectively abandoned.

### Cited Findings
- The rate-limit figures above come from Riot's own docs; production limits can be expanded for developers in good standing. — [Riot Dev Portal docs](https://developer.riotgames.com/docs/portal)
- A 429 response carries `Retry-After` (seconds). `X-Rate-Limit-Type` appears only when the edge enforces the limit; underlying services may return 429 without it. — [Riot Dev Portal docs](https://developer.riotgames.com/docs/portal)
- Registering multiple applications to get around limits is forbidden. — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- **Camille:**
  - Package `Camille.RiotGames`.
  - Latest NuGet version is `3.0.0-nightly-2025-06-24-d2b94715b7`, published 2025-06-24; there are no stable 3.x releases, only prereleases.
  - Targets .NET 6.0 and .NET Standard 2.1, with net8.0 dependency groups.
  - About 51.8K total downloads.
  — [NuGet Camille.RiotGames](https://www.nuget.org/packages/Camille.RiotGames)
- Camille's README says it "handles rate limits", "automatically retries failed requests", and exposes `MaxConcurrentRequests` and `Retries`. — [Camille GitHub](https://github.com/MingweiSamuel/Camille)
- Recent Camille commits on release/3.x.x:
  - 2026-08-22: QueueType UNKNOWN fallback (#127)
  - 2025-11-10: Fix codegen (#124)
  - 2025-02-19: query param fix
  - 2024: Riot ID / spectator-v5 updates
  — [Camille commits](https://github.com/MingweiSamuel/Camille/commits/release/3.x.x)
- Conflict: the August 2026 commit is newer than the latest NuGet nightly (June 2025), so nightly publishing may have stopped. Endpoints after June 2025 may require building from source. — [NuGet](https://www.nuget.org/packages/Camille.RiotGames) vs [Camille commits](https://github.com/MingweiSamuel/Camille/commits/release/3.x.x)
- RiotSharp: latest NuGet 4.0.0 (V4 API, .NET Standard 1.3) with a 5.0.0-alpha from June 2021; a fork (`xXLAOKOONXx.RiotSharp`) last updated March 2023. Riot.NET (`RiotDotNET`) has only a 2023 nightly with an incomplete to-do list. — [NuGet RiotSharp](https://www.nuget.org/packages/RiotSharp); [NuGet RiotDotNET](https://www.nuget.org/packages/RiotDotNET/)

### Inferences
- **Library recommendation for .NET 10:** either (a) use Camille prerelease (netstandard2.1 and net8 assets run on .NET 10), or (b) write about 6 endpoint calls by hand with `HttpClient` + `IHttpClientFactory` and a simple rate limiter. For (b), .NET has a built-in `System.Threading.RateLimiting` with a `SlidingWindowRateLimiter` per host, and Polly can retry on 429 using Retry-After. PreBot uses so few endpoints that (b) avoids depending on a slowly published library, especially since the June 2025 removal broke old summonerId paths.
- **Budget on a personal key** (100 requests per 2 min is the binding limit):
  - One join validation is about 2 calls (account plus league, the latter cached).
  - Spectator polling for the host only, every 120 s, across 10 open listings is about 10 calls per 2 min.
  - That is workable for TR-scale traffic only with aggressive caching.
- **Suggested cache TTLs** (in-memory/IDistributedCache, keyed by PUUID):
  - account by-riot-id: 24 h
  - league entries: 15 min
  - mastery: 6 h
  - status: 5 min
  - match details: forever (immutable)
  - Data Dragon: per patch version
- Never call the Riot API from the PWA client. Proxy through the ASP.NET backend so the key stays server-side, as policy requires.

### Gaps
- Default method limits for each endpoint (for example, spectator-v5 and match-v5 have their own lower method limits) were not retrieved. Read them from the `X-Method-Rate-Limit` and `X-App-Rate-Limit` response headers at runtime.
- No official Riot-maintained .NET SDK exists that I found.

## 5. Data Dragon / Community Dragon assets and the legal disclaimer

### Takeaway
Data Dragon is Riot's official static asset CDN; its latest version on the docs page was 16.22.1, and the ranked emblems ship as `ranked-emblems-latest.zip`. Using Riot assets in a free fan project falls under "Legal Jibber Jabber": it must be non-commercial, may not use Riot logos or trademarks in names or domains, and needs a conspicuous disclaimer. The API policies add their own required non-endorsement boilerplate. Show both.

### Cited Findings
- Data Dragon facts from the docs page:
  - Latest version is 16.22.1, shipped as a .tgz.
  - Check `versions.json` and `realms` for each region's version.
  - It includes champions, items, summoner spells and profile icons.
  - The latest ranked emblems are in `ranked-emblems-latest.zip`; older emblems, positions and tier icons are in separate zips.
  — [Riot LoL docs](https://developer.riotgames.com/docs/lol)
- The Legal Jibber Jabber disclaimer has two parts: "[The title of your Project] was created under Riot Games' "Legal Jibber Jabber" policy using assets owned by Riot Games." and "Riot Games does not endorse or sponsor this project." Display it conspicuously, for example on the website. — [Riot Legal](https://www.riotgames.com/en/legal)
- Legal Jibber Jabber terms:
  - The license is noncommercial, personal, non-transferable and revocable.
  - No crowdfunding, business entity or paywalls without a written license.
  - Exceptions: passive ads, and commercial API-based projects that follow the API terms.
  - No Riot logos or trademarks without a license, including in domain names, social handles and search keywords.
  - Projects must add something original.
  — [Riot Legal](https://www.riotgames.com/en/legal)
- API boilerplate: "[Your product] isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing Riot Games properties." — [Riot General Policies](https://developer.riotgames.com/policies/general)

### Inferences
- **Footer for PreBot:** "PreBot isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing Riot Games properties. League of Legends and Riot Games are trademarks or registered trademarks of Riot Games, Inc." Also add the Legal Jibber Jabber line when Riot assets (emblems, champion icons) are used. A Turkish translation can sit next to it, but keep the English original.
- **Naming:** "PreBot" is fine. Avoid "LoL" or "League" in the domain name. The research folder name contains "LoL", but that is internal and not a problem.
- **Assets:** rank emblems from the official zip and champion squares from `ddragon.leagueoflegends.com/cdn/{ver}/img/champion/{Key}.png`. Self-host or cache them in the PWA; there is no need to hotlink.
- **Role (position) icons:** available in the separate "positions" zip on the Data Dragon section of the docs.

### Gaps
- Community Dragon (raw.communitydragon.org) is a community, unofficial mirror. I found no Riot statement on its licensing, so treat its assets as Riot IP under the same Legal Jibber Jabber terms and prefer official Data Dragon zips.
- The exact current download URL for `ranked-emblems-latest.zip` was not captured. It is linked from the docs page.

## 6. LoL client (LCU) API: auto-invites, lobby reading, Vanguard

### Takeaway
The LCU API is local-only and "not officially supported for use with third party applications". Riot asks developers to register which LCU endpoints they use. Per Riot's FAQ, apps built on the LCU and in-game APIs "are still expected to work" under Vanguard, with no support guarantees. A PWA cannot reach the LCU at all; it would take a separate native desktop companion. I found no explicit Riot rule allowing or banning automated lobby invites.

### Cited Findings
- On the LCU: "This service is not officially supported for use with third party applications"; "We provide no guarantees of full documentation, service uptime, or change communication". Developers must report LCU use through the Developer Portal, saying which endpoints and how. — [Riot League Client APIs](https://developer.riotgames.com/league-client-apis.html); [Riot LoL docs](https://developer.riotgames.com/docs/lol)
- Riot also requires LCU-based products to be registered through the Developer Portal. — [Riot General Policies](https://developer.riotgames.com/policies/general)
- Vanguard FAQ: "Apps developed using the LCU and in-game APIs are still expected to work", but "the LCU and in-game APIs are not owned by Developer Relations and we cannot offer support or guarantee updates". — [Riot Dev FAQ](https://developer.riotgames.com/docs/faqs)
- The Game Client API (port 2999) is "only available locally for native applications". The Live Client Data API may not provide "game-session-specific information that would be previously unknown to the player". — [Riot League Client APIs](https://developer.riotgames.com/league-client-apis.html)
- General restrictions: no unfair advantage, no analysis of deliberately hidden players, and apps that dictate player decisions are unapproved. — [Riot League Client APIs](https://developer.riotgames.com/league-client-apis.html); [Riot General Policies](https://developer.riotgames.com/policies/general)

### Inferences
- An opt-in desktop companion, such as a small .NET tray app run by the host, could read the local LCU lockfile and send a lobby invite to the joiner's PUUID or Riot ID. That would not give a gameplay advantage, so it seems unlikely to break the Game Integrity rules. But it would rely on an unsupported, undocumented API, need registration in the portal, and could break on any patch. Low value for the cost: the host already gets a one-click "copy Riot ID" button. Recommend deferring it, or at most offering a tiny optional companion later.
- A safer UX improvement that needs no LCU: deep-copy the Riot ID with the right formatting, and show "Add friend → paste" instructions in Turkish.

### Gaps
- I found no Riot statement that specifically permits or forbids automating friend requests or lobby invites through the LCU. I also found no 2025–2026 report of Vanguard bans for LCU-only tools. Third-party tools like Blitz and Porofessor use the LCU, but that was not verified in this session.

## 7. Flex queue party rules (and the new Ranked 5s)

### Takeaway
Riot's support page, dated 2026-05-07, confirms that Ranked Flex allows parties of 1, 2, 3 or 5, not 4. A 4-player group must find a 5th, which is exactly PreBot's use case. There are no Flex rank restrictions for Diamond and below. To queue with Master+ players you must be at least Emerald, so the rule "Master+ in party requires everyone ≥ Emerald" is confirmed. Separately, Riot is running an experimental "Ranked 5s" mode in 2026: 5-player premades only, no rank restrictions, weekends only, and TR is included. That is highly relevant to PreBot.

### Cited Findings
- Flex rules from Riot's support page:
  - Parties of 1, 2, 3 or 5 can queue; "parties of 4 will have to stick to unranked matches".
  - No Flex rank restrictions for Diamond and below.
  - Queuing with Master+ requires being at least Emerald.
  - Large rank gaps may produce harder matchups.
  - Page date: May 7, 2026.
  — [Riot Support: Ranked Tiers, Divisions and Queues](https://support.riotgames.com/en-us/league-of-legends/gameplay/ranked-tiers-divisions-and-queues)
- The same 1/2/3/5 sizes and the Master/GM/Challenger-needs-Emerald+ rule appear in a third-party guide checked against Riot sources on 2026-09-25. It reports no Flex-specific patch changes in 2025–2026. — [pairly.gg Flex rules](https://pairly.gg/blog/lol-flex-queue-rules-rank-restrictions-party-size)
- Solo/Duo, for comparison:
  - Duo is limited to adjacent tiers.
  - No duo at Grandmaster or Challenger.
  - From Emerald to Diamond II, you can't duo with Master-MMR players.
  - KR has its own Master+ rule.
  — [Riot Support](https://support.riotgames.com/en-us/league-of-legends/gameplay/ranked-tiers-divisions-and-queues)
- Third-party guides report that patch 26.15 reopened Master duo with close-rank partners. Not verified against Riot patch notes. — [pairly.gg duo rules](https://pairly.gg/blog/lol-duo-queue-rules-which-ranks-can-duo)
- **Ranked 5s:**
  - Experiment 1 ran 2026-06-26 to 2026-09-06; Experiment 2 runs 2026-09-11 to 2027-01-07.
  - Weekends only (Fri–Sun), 16:00–04:00 server time, which for TR is 16:00–04:00 TRT.
  - Five-player premades only, with no rank restrictions from Unranked to Challenger.
  - Tournament Draft, 5 placement games, no decay.
  - Separate season; does not progress Solo/Duo or Flex objectives or the Victorious skin.
  - TR is among the 15 shards.
  — [Riot Support: Ranked 5s](https://support.riotgames.com/en-us/league-of-legends/events/ranked-5s)

### Inferences
- **Match-rule validation in PreBot** (using the verified ranks from D2):
  - Flex listings: warn only if a Master/GM/Challenger is in the group and the joiner is below Emerald, or the reverse.
  - Otherwise allow any rank, and show an informational "rank farkı büyük" ("large rank gap") hint only.
  - Use the league-v4 `RANKED_FLEX_SR` tier for this check. Riot's rule is presumably based on the Flex rank, but the docs do not say whether it uses the Flex or the Solo rank; see Gaps.
- **New listing type, "Ranked 5s (hafta sonu)" ("weekend"):** it needs exactly 5 and has no rank limits, so PreBot could support "4/5 for Ranked 5s" listings, shown only in the TRT Fri–Sun 16:00–04:00 window until 2027-01-07. That is strong product fit and should go on the roadmap. The queue id for D4/D5 detection is unknown; detect it through spectator `gameQueueConfigId` once observed, and add it to queues.json handling.
- The 4-player rule should be stated in PreBot's UI/FAQ ("Flex'te 4 kişi sıraya giremez", "4 players can't queue in Flex"), since it is the reason the product exists.

### Gaps
- Riot's page does not say whether the Flex Master+/Emerald check uses Flex rank, Solo rank, or the highest. The graphic on the page was not readable.
- I found no official Ranked 5s queue id or API `queueType` string, and no statement on whether league-v4 returns a separate Ranked 5s entry.
- Whether Ranked 5s continues after 2027-01-07 is not announced.
