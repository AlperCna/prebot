# Engineering Best Practices: .NET "Need 1 More Player" Discord Bot (LoL TR community), as of Nov 2026

Research note scope: library choice, Discord platform limits, architecture, Riot API client, hosting, testing/CI. Research was capped at about 25 tool calls. Some areas (DM limits, AutoMod, Riot icon verification specifics, Match-V5 field semantics, CI) are covered by inference and marked as such. Every factual claim has a URL. Claims without a source are under Inferences or Gaps.

## 1. .NET Discord library choice (Discord.Net vs NetCord vs DSharpPlus)

### Takeaway
**Recommended default: Discord.Net 3.20.x.** It is the only one of the three with a current *stable* release line that is actively shipped (3.20.2 on 2026-10-08). It has the largest download base, supports Components V2 through `ComponentBuilderV2`, and targets net8/9/10. **NetCord (1.0.0-beta.30, .NET 10 only)** is the strongest modern alternative, with very frequent releases and first-party Generic Host packages, but it is still prerelease. **DSharpPlus** splits into an old stable line (4.5.3, netstandard2.0) and a v5 that is still nightly-only, so it is the weakest default.

### Cited Findings
- **Discord.Net:** latest stable is 3.20.2 (2026-10-08). Earlier releases: 3.20.1 (2026-06-07), 3.20.0 (2026-06-06), 3.19.1 (2026-03-12), 3.19.0 (2026-03-03). It has 4.8M total downloads and targets net8.0, net9.0 and net10.0. — [NuGet Discord.Net](https://www.nuget.org/packages/Discord.Net)
- Discord.Net Interactions is published as a separate package (3.20.1 listed). — [NuGet Discord.Net.Interactions](https://www-0.nuget.org/packages/Discord.Net.Interactions)
- Discord.Net supports **Components V2** through a `ComponentBuilderV2` class in Discord.Net.Core, and has an official guide for it. — [ComponentBuilderV2 API](https://docs.discordnet.dev/api/Discord.ComponentBuilderV2.html); [Components V2 guide](https://docs.discordnet.dev/guides/components_v2/intro.html)
- The v3.18 breaking-changes guide covers type changes to `ActionRowBuilder`/`ComponentBuilder` that came with Components V2. Anyone reading pre-3.18 tutorials should check it. — [Discord.Net v3.18 breakings](https://docs.discordnet.dev/guides/breakings/v3.18.html)
- Discord.Net throws an exception when a V2 component structure is invalid, which gives fast-fail validation. — [Discord.Net commit cf66ab4](https://github.com/discord-net/Discord.Net/commit/cf66ab4)
- **NetCord:** latest is 1.0.0-beta.30 (2026-11-07). Earlier releases: beta.29 (2026-11-02), beta.28 (2026-10-01), beta.25–27 (late Sept 2026). It has 2.8M total downloads, targets net10.0 only, and describes itself as "The modern and fully customizable C# Discord library". — [NuGet NetCord](https://www.nuget.org/packages/NetCord)
- NetCord "requires .NET 10 or higher". Installing it requires the `--prerelease` flag. It ships `NetCord.Hosting` (".NET Generic Host extensions"), `NetCord.Hosting.Services` and `NetCord.Hosting.AspNetCore` (for HTTP interactions). — [NetCord installation guide](https://netcord.dev/guides/getting-started/installation.html)
- Older search snapshots show NetCord.Hosting and NetCord.Services at earlier beta numbers, so the sub-packages may be versioned separately. — [NuGet NetCord.Hosting](https://www.nuget.org/packages/netcord.hosting/)
- **DSharpPlus:** latest stable is 4.5.3 (2026-08-24) and targets netstandard2.0 only. The latest v5 build is 5.0.0-nightly-02615 (2026-11-08). The listing shows only nightlies for v5, with no stable v5. It has about 2.2M total downloads. — [NuGet DSharpPlus](https://www.nuget.org/packages/DSharpPlus)
- DSharpPlus policy: stable versions get no breaking API changes without a major bump, while nightlies can change without notice. Critical fixes are backported to stable after soak tests. — [NuGet DSharpPlus 5.0.0-nightly-02578](https://packages.nuget.org/packages/DSharpPlus/5.0.0-nightly-02578)

### Inferences
- Discord.Net ships stable releases about every 1–4 months and targets LTS .NET 8 as well as .NET 10. For a hobby bot whose maintainer wants "install and forget" stability, that is the safest choice. NetCord's ~weekly betas mean breaking API changes are still possible before 1.0.
- NetCord is a good choice if the developer wants .NET 10 only, first-class Generic Host/DI (`NetCord.Hosting`), and an HTTP-interactions mode through ASP.NET Core. It is a reasonable pick for a greenfield bot, as long as the developer pins an exact beta version.
- DSharpPlus 4.5.3 (netstandard2.0) is in maintenance mode. v5 is nightly-only, so it is not recommended for new projects in Nov 2026.
- Discord.Net has no first-party Generic Host package. The common pattern is a `BackgroundService` or `IHostedService` that owns `DiscordSocketClient` plus `InteractionService` registered in DI. Community packages exist, but none was verified in this research.

### Gaps
- Community size (Discord server member counts, GitHub stars and contributors) was not fetched for any of the three libraries.
- Whether NetCord has documented Components V2 and modal support was not verified on its docs pages. The installation page fetched does not cover it.
- The exact Discord.Net release where V2 support landed was not found. It appears to be 3.18, based on the breaking-changes doc.

## 2. Discord platform rules that shape the design

### Takeaway
Design around these constraints:
- **Interaction timing:** acknowledge within 3 seconds. Use a deferred response for anything that touches the DB or Riot API. The interaction token then lasts 15 minutes.
- **REST rate limits:** read the rate-limit headers rather than hard-coding values. The global limit is 50 req/s, and interaction endpoints are exempt from it. Avoid 401/403/429 storms, because 10,000 invalid requests in 10 minutes gets the IP temporarily banned.
- **custom_id:** at most 100 characters, so encode only a slot ID in it.
- **Privileged intents:** a slash-command and button bot needs none. Presence is privileged, so detecting "playing League" through it adds an approval burden and is not needed for an MVP.

### Cited Findings
- **Global REST limit:** 50 requests/second per bot. Interaction endpoints are not bound by it. Per-route buckets are reported via `X-RateLimit-Bucket`, and the headers are `X-RateLimit-Limit/Remaining/Reset/Reset-After/Global/Scope` plus `Retry-After`. — [Discord Rate Limits](https://docs.discord.com/developers/topics/rate-limits)
- Discord says "don't hard-code rate limits, parse headers". Limits are tracked separately per top-level resource (channel/guild/webhook). — [Discord Rate Limits](https://docs.discord.com/developers/topics/rate-limits)
- **Invalid request limit:** more than 10,000 invalid requests (401/403/429) in 10 minutes temporarily bans the IP. 429s with `X-RateLimit-Scope: shared` don't count. To stay under it, stop requests on an invalid token, check permissions before acting to avoid 403s, and don't hit exhausted buckets. — [Discord Rate Limits](https://docs.discord.com/developers/topics/rate-limits)
- **Interaction response timing:** an initial response is required within 3 seconds, or the token is invalidated. Tokens are valid for 15 minutes for follow-ups and edits to the original response. — [Discord Receiving & Responding](https://docs.discord.com/developers/interactions/receiving-and-responding)
- **Deferred response types:**
  - `DEFERRED_CHANNEL_MESSAGE_WITH_SOURCE` (type 5) shows a loading state.
  - `DEFERRED_UPDATE_MESSAGE` (type 6) is for component interactions and shows no loading state. It suits button clicks that edit the LFG post.
  - The ephemeral flag is `1<<6` (64). After a type-5 defer, the ephemeral setting from the defer is kept.
  - Source: [Discord Receiving & Responding](https://docs.discord.com/developers/interactions/receiving-and-responding)
- Interactions received over the gateway must be responded to via HTTP. Interaction webhooks share normal webhook rate-limit properties. The 5-follow-up cap applies only to user-installed apps not installed in the server. — [Discord Receiving & Responding](https://docs.discord.com/developers/interactions/receiving-and-responding)
- **Component limits:**
  - `custom_id` is 1–100 characters and must be unique per message.
  - Buttons: up to 5 per action row, label max 80 characters.
  - String select: max 25 options, option label/value max 100 characters, one select per action row.
  - Modal text input: up to 4000 characters, label max 45 characters.
  - Modals also support Radio Group (2–10 options) and Checkbox Group (1–10).
  - Source: [Discord Component Reference](https://docs.discord.com/developers/components/reference)
- **Components V2:** enabled with flag `IS_COMPONENTS_V2` (`1<<15`), which can't be removed once set. `content` and `embeds` stop working, so use Text Display and Container instead. A message can hold up to 40 total components. — [Discord Component Reference](https://docs.discord.com/developers/components/reference)
- **Privileged intents:**
  - `GUILD_MEMBERS`: member add/update/remove events and List Guild Members.
  - `GUILD_PRESENCES`: `PRESENCE_UPDATE` events.
  - `MESSAGE_CONTENT`: message text, embeds and so on. Messages the app sends, DMs with the app, messages that mention the app, and context-menu targets are exempt.
  - Sending an unapproved intent closes the gateway with code 4014.
  - Source: [Discord Gateway](https://docs.discord.com/developers/events/gateway)
- **Approval:** apps that qualify for verification need approval for privileged intents, and verified apps in 100+ servers need it after verification. The fetched page also says apps with fewer than 10,000 users can enable intents directly, while larger apps go through review and reapply annually. — [Discord Gateway](https://docs.discord.com/developers/events/gateway). *Caution:* the 10,000-user wording came through the fetch summarizer and differs from the long-standing "100 servers" rule. Verify on the live page.
- **Gateway limits:** 1,000 IDENTIFY calls per 24 hours. Exceeding it terminates sessions and resets the bot token. Gateway sends are capped at 120 events per 60 seconds per connection, and exceeding that disconnects immediately. — [Discord Gateway](https://docs.discord.com/developers/events/gateway)
- **Developer Policy (data storage, DMs, presence use):** the official page (support-dev.discord.com) returned HTTP 403 to automated fetch and could not be verified. Third-party bot privacy policies cite user deletion rights under Discord's developer rules, and one cites a ban on using message content for AI training. These are not authoritative. — [Modmail privacy policy](https://docs.modmail.dev/frequently-asked-questions/privacy-policy); [NoxBot privacy](https://noxbot.vercel.app/privacy-policy)

### Inferences
- **Button handling:** every button click should immediately `DeferAsync()` (type 6), then do the DB transaction, then edit the original message. This stays within 3 seconds even under DB or Riot latency.
- **custom_id design:** use a short, versioned scheme such as `lfg:v1:join:{slotId}` (well under 100 characters). Keep all state server-side, keyed by `slotId`, and never trust state encoded in custom_id.
- **Intents:** an LFG bot built on slash commands, buttons, selects and modals needs only non-privileged intents (`Guilds`; possibly `GuildVoiceStates` for voice-channel checks). Avoid `MESSAGE_CONTENT` entirely.
- **Presence for "playing LoL":** it would require `GUILD_PRESENCES`, which is privileged. That means extra review once the bot scales or verifies, plus a high event volume and privacy sensitivity. Rich Presence for LoL is also often absent or inconsistent. Recommendation: don't use presence. Use explicit "/available" opt-in plus Riot API data.
- **Role pings:** prefer a single opt-in "LFG" role mentioned at most once per post, with a per-user cooldown (for example, one post per 5–10 minutes). Use `allowed_mentions` to restrict pings to that role, so user-provided text can't trigger @everyone or other mass mentions. Sourced Discord docs on allowed_mentions or AutoMod were not fetched.
- **DMs:** treat them as best-effort. Many users have server DMs off, which surfaces as a 403. Repeated 403s count toward the invalid-request ban, so cache "DMs closed" per user and fall back to an ephemeral or in-channel notice.

### Gaps
- No sourced DM-specific rate limits; Discord does not publish them in the fetched pages.
- No sourced role-mention limits or AutoMod interplay (AutoMod can block bot messages matching keyword or mention-spam rules).
- The bot verification requirement (mandatory at 100 servers) was not directly fetched. It is referenced indirectly by the gateway page's "verified apps in 100+ servers".
- The authoritative Developer Policy and Terms text on data retention, deletion and privacy policy requirements was blocked (403).

## 3. Architecture for a small bot

### Takeaway
**Recommended default:**
- A single-process .NET Generic Host worker: one `BackgroundService` runs the gateway client, plus a `PeriodicTimer`-driven expiry service.
- **SQLite + EF Core** for persistence, with the "available now" pool modelled as rows with an `ExpiresAtUtc` column and a periodic sweep. Upgrade to PostgreSQL only if there are multiple instances or a dashboard.
- Concurrency handled with an optimistic, conditional UPDATE (or a unique constraint) so only one user can claim the last slot.
- No Redis, Hangfire or Quartz at this scale.

### Cited Findings
- NetCord ships Generic Host integration packages (`NetCord.Hosting`, `NetCord.Hosting.Services`) and an ASP.NET Core package for HTTP interactions. The Generic Host worker pattern is first-party there. — [NetCord installation guide](https://netcord.dev/guides/getting-started/installation.html)
- Interactions received over the gateway must be answered via HTTP, but no public HTTP endpoint is needed. A gateway-only worker suffices, and ASP.NET is only needed if you choose HTTP-interaction mode. — [Discord Receiving & Responding](https://docs.discord.com/developers/interactions/receiving-and-responding)
- 1,000 IDENTIFY calls per 24 hours: a crash-looping container that re-identifies each restart can exhaust this and get the token reset. — [Discord Gateway](https://docs.discord.com/developers/events/gateway)

### Inferences (engineering judgment; no external source fetched)
- **Process model:** a single worker process (`Host.CreateApplicationBuilder` + `AddHostedService`). Add a tiny ASP.NET minimal-API health endpoint only if the host platform needs HTTP health checks (Fly, Railway, Render). Otherwise, use a Docker HEALTHCHECK on a file or heartbeat.
- **"Available now" pool:**
  - Store each entry as a DB row `(UserId, GuildId, Role, Rank, CreatedAt, ExpiresAtUtc)`. Read with `WHERE ExpiresAtUtc > now`, so correctness never depends on the sweep timing.
  - A `BackgroundService` with `PeriodicTimer(TimeSpan.FromSeconds(30))` deletes expired rows and edits or closes their Discord messages.
  - An in-memory `ConcurrentDictionary` cache is optional. Redis is overkill for a single instance.
  - Hangfire or Quartz only add value for persisted cron jobs across restarts. Here, rebuilding the timers from DB on startup is enough.
- **Database:**
  - SQLite (file on a Docker volume) means zero ops and is enough for a single community.
  - PostgreSQL is better if hosting provides it for free or if multiple replicas are planned.
  - Either way, use EF Core migrations applied at startup (`Database.MigrateAsync()`), and keep the provider swappable.
- **Idempotency and races (two people accept the last slot):**
  - Use an atomic conditional update: `UPDATE Slots SET FilledBy=@u, Version=Version+1 WHERE Id=@id AND FilledBy IS NULL`. Then check rows-affected. With EF Core, use `ExecuteUpdateAsync` or a concurrency token (`[Timestamp]`/`Version`).
  - Alternatively, use a unique index on `(PostId, SlotIndex)` in a `PostParticipants` table and catch the `DbUpdateException`.
  - Also dedupe by interaction ID (unique) to make retries safe.
  - On loss, send an ephemeral "slot already filled" reply.
  - Serialize per-post work with `SemaphoreSlim` keyed by post ID as an additional in-process guard.
- **Logging and monitoring:**
  - Serilog or `Microsoft.Extensions.Logging` with console JSON output, forwarding the library's log events (Discord.Net `client.Log`).
  - Log every 429 or `Retry-After`.
  - Track gateway disconnect/reconnect counts.
  - An external uptime ping (e.g. a heartbeat URL) is enough at this scale. OpenTelemetry is optional.

### Gaps
- No primary sources were fetched for EF Core concurrency tokens, `PeriodicTimer`, or Hangfire/Quartz comparisons. The recommendations above are standard .NET practice but unsourced here. The report writer may cite learn.microsoft.com (EF Core "Handling Concurrency Conflicts"; `System.Threading.PeriodicTimer`) if allowed.

## 4. Riot API client in .NET

### Takeaway
**Recommended default:** Camille (`Camille.RiotGames`), unless the developer wants zero prerelease dependencies. It is auto-generated nightly from Riot's spec, thread-safe, and handles rate limits and retries automatically. The alternative is a hand-written typed `HttpClient` + `Microsoft.Extensions.Http.Resilience` (Polly v8) with per-region token buckets that honor `Retry-After`. That is reasonable because the bot needs only about 4 endpoints (Account-V1, Summoner-V4, League-V4, Match-V5). On key type, a TR community bot qualifies for a **Personal API key** (20 req/1 s, 100 req/2 min), not production. Cache aggressively.

### Cited Findings
- **Camille:**
  - Package `Camille.RiotGames`, installed with `--prerelease`.
  - "Fast, asynchronous, thread-safe", "automatically retries failed requests", and "handles rate limits".
  - Automatic nightly releases reflect Riot API changes within 24 hours.
  - Configured via `RiotGamesApiConfig.Builder` (`MaxConcurrentRequests`, `Retries`).
  - 268 commits, 101 stars, `release/3.x.x` branch. No dated releases are shown on GitHub.
  - Source: [Camille GitHub](https://github.com/MingweiSamuel/Camille)
- Camille's rate limiter is published separately as `MingweiSamuel.TokenBucket`. — [NuGet MingweiSamuel.TokenBucket](https://feed.nuget.org/packages/MingweiSamuel.TokenBucket)
- **Riot key types:**
  - Development keys deactivate every 24 hours.
  - Personal keys are for "private use or small communities", need no verification, allow 20 req/1 s and 100 req/2 min, and can't be used for public products.
  - Production keys start at 500 req/10 s and 30,000 req/10 min and require a working prototype and review.
  - Source: [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Riot rate-limit types:**
  - Application limits are per key, per region.
  - Method limits are per endpoint, per key, per region.
  - Service limits are shared by all apps.
  - Some underlying-service 429s lack the `X-Rate-Limit-Type` header.
  - On a 429, stop for the `Retry-After` seconds. Violations can disable API access.
  - Source: [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Headers:** `X-App-Rate-Limit` lists `limit:seconds` windows (e.g. `20:1,100:120`). `X-App-Rate-Limit-Count` and `X-Method-Rate-Limit(-Count)` report usage, and each window resets independently. — [HextechDocs rate limiting (community)](https://hextechdocs.dev/rate-limiting/); [Riot rate-limiting (archived)](https://developer.riotgames.com/rate-limiting.html)
- During service-level 429s the method count header may be missing, a signal that the limit is infrastructure-side. — [riot-lol-api issue #26](https://github.com/Neamar/riot-lol-api/issues/26)
- Product registration is reviewed for policy compliance, "measurable benefit to players" and completeness. — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)

### Inferences
- **Routing:**
  - Account-V1 (Riot ID → PUUID) and Match-V5 use the *regional* routing value `EUROPE` for TR accounts.
  - Summoner-V4 and League-V4 use the *platform* value `TR1`.
  - Rate limits are per region, so keep **one token bucket per routing value** (EUROPE and TR1 separately), each with both windows (1 s and 120 s).
  - The EUROPE vs TR1 mapping is standard Riot practice but was not re-verified on a fetched page.
- **Hand-written client recipe:**
  - A typed `HttpClient` with a `DelegatingHandler` that checks the per-route token bucket before sending.
  - On a 429, read `Retry-After` and pause that bucket. Use `AddResilienceHandler` (Polly v8) for transient 5xx with jittered backoff.
  - Never retry 400, 401, 403 or 404.
  - Parse `X-App-Rate-Limit` on the first response to auto-configure the windows.
- **Caching:**
  - Riot ID → PUUID is effectively immutable per account (cache for days; refresh on name-change errors).
  - League entries (rank) can be cached for 10–30 minutes.
  - Match details are immutable, so cache them forever.
  - Store results in the same SQLite/Postgres DB, or use `IMemoryCache`.
- **Icon verification flow (ownership proof):**
  1. User runs `/link` with a Riot ID.
  2. Bot resolves the PUUID (Account-V1/EUROPE), then reads the Summoner (TR1) and records the current `profileIconId`.
  3. Bot picks a random *default* icon different from the current one and stores `(userId, puuid, expectedIconId, expiresAt = now+10min)`.
  4. User changes the icon in client and clicks "Verify". The bot re-fetches the Summoner with a cache bypass.
  5. On a match, the bot marks the account as linked and deletes the pending challenge.
  - The verify button needs a defer (type 6), since the Riot call may exceed 3 seconds.
- **Main role from Match-V5:**
  1. Fetch match IDs with `queue=440` (Ranked Flex) and `count≈20` from `/lol/match/v5/matches/by-puuid/{puuid}/ids` (EUROPE).
  2. For each match, find the participant by PUUID and tally `teamPosition` (TOP/JUNGLE/MIDDLE/BOTTOM/UTILITY; empty for remakes).
  3. Take the mode, and the second-most-frequent as the secondary role.
  - About 21 requests per user, so rate-limit and cache.
  - With a Personal key (100 per 2 min), compute this lazily and asynchronously in the background, not inside the interaction.
  - Also allow manual role override.
- **Policy:** confirm that the bot's use case fits a Personal key (a single community). Otherwise register the product.

### Gaps
- Camille's latest NuGet version and date were not fetched. Its GitHub page shows no release dates, so maintenance recency is unverified.
- No fetched Riot page explicitly documents `teamPosition` vs `individualPosition` semantics, or the EUROPE/TR1 routing table.
- Riot's policy on third-party account verification flows was not fetched.

## 5. Hosting options (cheap/free, always-on)

### Takeaway
**Recommended default: a small Hetzner Cloud VPS (EU region; low latency to Turkey and to Discord/Riot EU) running Docker Compose.** It costs roughly €4–7/month after the 2026 price increases. Check the live price, because sources conflict and some cost-optimized plans showed as unavailable. **Free alternative:** Oracle Cloud Always Free (Ampere A1, now 2 OCPU / 12 GB since 2026-06-15), which is plenty for a bot but carries capacity and account-reclamation risk. **PaaS (Railway ~$5/month Hobby, Fly.io pay-as-you-go ~$2–5/month)** suits developers who want git-push deploys. Render's free tier sleeps, so it is unsuitable for a gateway bot.

### Cited Findings
- **Hetzner, wave 1:** a global price increase effective 2026-04-01. CX22 (2 vCPU / 4 GB, EU) went from €3.29 to €4.49 (+36%), and existing customers were affected. — [igor'sLAB](https://www.igorslab.de/en/hetzner-to-significantly-increase-prices-for-cloud-and-dedicated-servers-from-april-2026/); [Hetzner price adjustment (official, DE)](https://docs.hetzner.com/de/general/infrastructure-and-availability/price-adjustment/)
- **Hetzner, wave 2:** effective 2026-06-15 for new orders and rescales. Reported CAX11 €4.49 → €5.99 and CX23 €3.99 → €5.49 (aggregator figures). CX22 is described as legacy, with CX23 as its successor. — [agentdeals Hetzner 2026](https://agentdeals.dev/hetzner-pricing-2026); [webhosting.today](https://webhosting.today/2026/06/18/hetzners-price-increases-reached-209-the-30-headline-applied-to-a-different-tier/); [Capital & Compute](https://capitalandcompute.net/blog/hetzner-price-increase-2026/)
- **Conflict:** the official German table reportedly lists CAX11 at €5.34 → €7.13, possibly including VAT. One source reports cost-optimized plans as unavailable in Sept–Oct 2026. — [Hetzner price adjustment (DE)](https://docs.hetzner.com/de/general/infrastructure-and-availability/price-adjustment/); [privatedevops](https://privatedevops.com/news/hetzner-june-2026-cloud-price-increase-what-to-do)
- **Oracle Cloud Always Free:** the Ampere A1 allowance dropped from 4 OCPU / 24 GB to 2 OCPU / 12 GB, effective 2026-06-15 (1,500 OCPU-hours and 9,000 GB-hours per month). — [InfoQ](https://infoq.com/news/2026/07/oracle-cloud-free-tier-limits/); [terminalbytes](https://terminalbytes.com/oracle-cloud-free-tier-changes-2026)
- Oracle's AMD micro option (2 VMs, each 1/8 OCPU and 1 GB) is noted as suitable for small bots. Third-party guides report a risk that A1 instances over the allowance are deleted after trial expiry. — [space-node Oracle limits 2026](https://space-node.net/blog/oracle-cloud-always-free-limits-2026)
- **Railway:** the Hobby plan is about $5/month with included usage credit and is always-on. One source reports a mandatory credit card since March 2026. — [devtoolpicks](https://devtoolpicks.com/blog/railway-vs-render-vs-fly-io-solo-developers-2026); [temps.sh Railway pricing 2026](https://temps.sh/blog/railway-pricing-2026.md)
- **Render:** the free tier sleeps after idle (30–60 s wake). Paid always-on starts around $7/month, and a background worker is priced like a web service. — [devtoolpicks](https://devtoolpicks.com/blog/railway-vs-render-vs-fly-io-solo-developers-2026); [agentdeals Railway vs Render](https://agentdeals.dev/railway-vs-render)
- **Fly.io:** per-second Machines billing, with a small always-on instance at roughly $2–5/month. Sources conflict on whether a Hobby credit still exists. — [budgetforge Fly.io 2026](https://www.budgetforge.dev/tools/fly-io-pricing-2026); [sota.io comparison](https://sota.io/blog/railway-vs-render-vs-fly-pricing-2026)

### Inferences
- A gateway bot needs a persistent WebSocket, so anything that sleeps (Render free, scale-to-zero Container Apps or Fly with auto-stop) is unsuitable. On Fly, set `auto_stop_machines = false` and `min_machines_running = 1`.
- A VPS + Docker Compose (bot + optional Postgres, `restart: unless-stopped`, volume for SQLite) is the cheapest predictable option. Combine it with a restart backoff so a crash-loop doesn't burn the 1,000/day IDENTIFY budget.
- Use the official `mcr.microsoft.com/dotnet/runtime` (or `runtime-deps` for self-contained/AOT) base image in a multi-stage Dockerfile. NetCord requires .NET 10 images. Building on ARM64 (CAX/Oracle A1) works with .NET multi-arch images.

### Gaps
- Contabo, Azure App Service / Container Apps free tiers, and current official Fly, Railway and Render pricing pages were not fetched. All PaaS figures are from third-party 2026 comparisons, not provider pages.
- Exact current Hetzner prices including or excluding VAT are unconfirmed.

## 6. Testing approach and minimal CI

### Takeaway
**Recommended default:**
- **xUnit (or NUnit) unit tests** on a pure domain layer (slot or pool logic, expiry, role inference from `teamPosition` tallies, custom_id parsing), kept free of Discord types.
- **Integration tests** for EF Core using SQLite in-memory or Testcontainers-Postgres, especially for the "two people claim the last slot" race.
- **Riot client tests** against canned JSON with a fake `HttpMessageHandler` (429 + `Retry-After` behavior).
- **CI:** one GitHub Actions workflow running `dotnet restore/build/test` and a Docker build on push.

### Cited Findings
- Discord.Net throws on invalid V2 component structures, so builder output can be exercised in unit tests to catch layout errors before runtime. — [Discord.Net commit cf66ab4](https://github.com/discord-net/Discord.Net/commit/cf66ab4)
- Riot's documented 429 + `Retry-After` contract, and application vs method limit types, define the behaviors the HTTP client tests should assert. — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- Discord's documented 3-second ack and 15-minute token window define timing behaviors to test (e.g. that the handler defers before doing I/O). — [Discord Receiving & Responding](https://docs.discord.com/developers/interactions/receiving-and-responding)

### Inferences
- Keep handlers thin. Interaction handlers should translate into application-service calls (`ClaimSlot(postId, userId, interactionId)`), so 90% of the logic is testable without a gateway connection.
- Run the concurrency test with two parallel claims against a real DB provider (SQLite file or Testcontainers Postgres). The EF InMemory provider does not enforce concurrency or uniqueness semantics.
- Use a separate "dev" Discord application and test guild with guild-scoped command registration, which updates instantly, versus global registration in production.
- **Minimal CI:** `actions/checkout`, `actions/setup-dotnet` (10.x), `dotnet test`, then `docker build`. Optionally push to GHCR and deploy via SSH or watchtower to the VPS. Add Dependabot for NuGet, which matters because NetCord and Camille are prerelease and fast-moving.

### Gaps
- No external sources were fetched on .NET testing tooling (xUnit, Testcontainers, GitHub Actions setup-dotnet). The recommendations are standard practice but unsourced in this note.
