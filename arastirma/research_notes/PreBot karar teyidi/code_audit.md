# PreBot code audit (independent, 9 Nov 2026)

**Baseline.** Commit `1920075` ("Tek havuz"), which landed while this audit was running (it was `3df1679` at the start). The working tree also had uncommitted edits: share button, OG tags, push copy. These were looked at only for the gesture issue (H4). `dotnet test PreBot.sln` passed: 77 core + 55 web = 132 tests. I verified the code paths by reading them. Docs were not trusted.

**Short verdict.** The core engineering is sound for an MVP. The seat claim is atomic, authz is server-side, tokens are bound to user and listing, and time handling is correct. But two design gaps (C1, C2) make the 4+1 case fail in exactly the state the product will launch in: a sparse pool. The biggest risk is still liquidity, not code.

---

## Critical

### C1. A seat can be taken with no Riot ID, and that silently kills a 1-slot listing
- **Evidence.**
  - The claim rules have no Riot ID condition: `PreService.cs:261-268`.
  - Web "Katıl" is always enabled. The Riot ID prompt appears only *after* joining: `ListingPage.razor:113-118,128`.
  - The Discord "Katıl" button auto-creates an account and claims the seat in one click, then only *suggests* `/profil`: `WebPoolModule.cs:166-203`.
  - The host push then says "Riot ID'sini kaydetmemiş; ilan sayfasından birlikte çözün" (`HostNotifier.cs:39-41`). But there is no chat to "work it out together", and the host is never pushed again when the joiner adds the ID later (`RiotIdPrompt.razor` only calls `Live.PublishChanged`).
  - A claim that fills the listing sets `Status=Filled, NextWaveAtUtc=null` (`PreService.cs:280-284`), so all waves stop.
- **Why it matters.** In the 4+1 case (`SlotsNeeded=1`), the first tapper without a Riot ID fills the listing. Notifications stop. The host, who is in the LoL client, gets an unusable push. The group waits until the host notices and presses "Gelmedi". This is the most likely real-world failure of the core loop. Discord one-click joins (with VPN users) make it more common.
- **Fix.**
  - Require a valid Riot ID to claim. Add it to the `TryClaimAsync` WHERE clause, or check it beforehand and return a new `ClaimResult.NeedsRiotId`.
  - On the web, show the inline prompt *before* the Katıl button. In Discord, open a modal asking for the Riot ID on the first Katıl.
  - As a safety net, push the host again (same tag `host-{id}`) when a seated player saves their Riot ID.

### C2. Waves stop after about 5 minutes, and late arrivals are never matched
- **Evidence.**
  - `Matcher.MaxWaves = 3` (`Matcher.cs:13`). Waves fire at t=0, about 2 and about 5 minutes. After the third wave, `nextWaveAt = null` (`PreService.cs:406`), so the listing is silent for its remaining ~10 minutes.
  - Empty waves still use up the counter. A listing opened into an empty pool burns all three waves in 5 minutes.
  - `SetAvailableAsync` (`PreService.cs:27-47`, `PoolActions.cs:24-29`) never checks open listings. A player who becomes available at minute 6 gets no push and no DM, and Discord `/musaitim` users never even see the list.
  - A reopen after a no-show or leave skips everyone ever notified for this listing (`PreService.cs:424-432`). In a small pool, the second search often has nobody left.
- **Why it matters.** At launch, players trickle in over the evening. Throwing away everyone who arrives after minute 5 turns "fills in 8 minutes" into "expired".
- **Fix.**
  - (a) Reverse match: on `SetAvailable` (web and Discord), look up open, matching listings that haven't notified this user, and notify them right away, respecting the cooldown.
  - (b) Keep a widest-range "sweep" wave every 2–3 minutes until expiry, for *new* candidates only.
  - (c) Don't advance `WavesSent` on an empty wave (just reschedule).
  - (d) On reopen, allow re-notifying wave-0 people who didn't answer, after a cooldown.

## High

### H1. Expiry race between the two background workers when the bot runs inside the site (the recommended setup)
- **Evidence.**
  - `PoolWorker.cs:29` and `ExpiryWorker.cs:32` both call `pre.SweepAsync()` on the same 10 s period, started at the same moment.
  - Whichever runs first flips the listing to Expired and owns the returned `ExpiredListingIds`.
  - `ExpiryWorker` only calls `notifier.RefreshAsync`, which returns early for web listings (`MessageId` is null, `ListingNotifier.cs:31`).
- **Why it matters.** Roughly half of web-listing expiries will (1) never send the host the "süresi doldu / tek dokunuşla tekrar aç" push, and (2) never update the Discord feed card, which then stays "open" with a live join button. When both workers select before either updates, both write `ListingExpired`, doubling the metric.
- **Fix.** When the site hosts the bot, the sweep should run only in `PoolWorker`. Either skip the sweep in `ExpiryWorker` when `IWebPool` is registered, or give `SweepAsync` a community filter and have each worker expire only its own listings. Add a test with both workers registered.

### H2. Nothing checks that the host can be reached
- **Evidence.**
  - `NewListing.razor` has no push prompt or check; only the pool page does (`Pool.razor:22`).
  - `HostNotifier.NotifyHostAsync` (`HostNotifier.cs:16-25`) sends to push, then DM, and silently does nothing when neither exists.
  - The joiner is told "İlan sahibi seni oyunda davet edecek" whether or not the host was reached (`ListingPage.razor:106-112`, `WebPoolModule.cs:200`).
- **Why it matters.** The host is in the LoL client with the site closed. Without push, the join is invisible and the joiner waits for an invite that never comes.
- **Fix.** On the listing form, if the host has 0 push devices and no Discord, show the PushToggle inline. Make the host's Riot ID mandatory too, so the joiner can add the host themselves. On join, tell the joiner "ilan sahibine bildirim gitmedi, sen ekle: `<host Riot ID>`".

### H3. Waves pick and "use up" players who cannot be reached
- **Evidence.**
  - `Matcher.FindCandidates` (`Matcher.cs:68-90`) and `FindAlertCandidates` do not consider whether anyone can actually receive the alert.
  - `RunNextWaveAsync` still records a `ListingNotification` and sets `LastPingedAtUtc` for players with no push device, no DM route and no open tab (`PreService.cs:438-475`).
  - The `WaveSent` metric counts picks, not deliveries.
- **Why it matters.** Wave 0 has only 5 seats. Every unreachable pick delays real candidates by 2 minutes and burns their cooldown. The statistics then overstate how many people were reached, which misleads the liquidity decisions the roadmap depends on.
- **Fix.** Pass a "reachable" set into the matcher (push device, Discord DM route, or a live circuit from `LiveUpdates`). Alternatively, don't count unreachable picks toward `WaveSize`. Store delivered counts in the metrics.

### H4. Push permission prompt (and the new share sheet) runs after a server round trip, so it is unverified on iPhone
- **Evidence.** `PushToggle.razor:142` (`EnableAsync`) is a Blazor Server handler. It reaches `Notification.requestPermission()` (`prebot.js:74`) only after a SignalR round trip. The uncommitted `prebot.share` / `navigator.share` follows the same path. The E2E test ran on Edge/Windows only.
- **Why it matters.** Safari/WebKit (iOS PWA) and Firefox need *transient user activation*. A round trip to a German VPS on a Turkish mobile network may lose it, so the permission prompt or share sheet never appears. iPhone users then never get push, and push is the product.
- **Fix.** Attach a plain JS click listener (or `onclick="prebot.push.enable()"`) that calls `requestPermission` / `share` synchronously, then sends the result to .NET. Test on a real iPhone (PWA installed and logged in inside the PWA) before launch.

### H5. Flex rank rule can be broken; the host can't see the joiner's rank or role
- **Evidence.**
  - `WidenedRange` (`Matcher.cs:42-49`) lowers the min by 1–2 tiers even when `MaxRank == MasterPlus`. A host who correctly set Emerald–Master+ (because the group has a Master) gets Platinum and Gold players notified in waves 1–2.
  - `TryClaimAsync` doesn't check rank or role at all, so any listing on the open board can be joined by anyone.
  - `ListingSlot` stores no declared role or rank, and the host push and page show only name and Riot ID (`HostNotifier.cs:36-41`, `ListingPage.razor:83-93`).
- **Why it matters.** The invite works, but the party can't queue ("must be at least Emerald"). That counts as a failed evening, and a no-show is the wrong label for it.
- **Fix.** If the range includes Master+, clamp the widened min at Emerald. Reject claims outside the widest allowed range. Snapshot the joiner's declared role and rank onto `ListingSlot` and show them, plus an op.gg link, in the host push and page.

### H6. One player can hold seats in several listings, and there are no moderation tools
- **Evidence.**
  - `TryClaimAsync` (`PreService.cs:261-268`) doesn't check whether the user already holds an active seat elsewhere.
  - Discord and Google accounts are free, and the only limit is 6 joins per minute per account (`ActionLimiter.cs`).
  - There is no admin, ban or report code anywhere (grep: none), although Terms say "Bu tür hesaplar kapatılabilir".
- **Why it matters.** Every 1-slot listing a troll or ghost joins becomes Filled and silent (see C1). A handful of alt accounts can grief the whole TR pool in an evening. Law 5651 notice-and-takedown also needs a way to remove content.
- **Fix.** Allow one active seat per user (WHERE clause plus a partial unique index). Add a minimal admin page: list listings and accounts, delete or ban (`UserAccount.BannedAtUtc` checked in claim/create). Add a host "rapor et" button.

## Medium

- **M1. Global refresh fan-out has no throttle.** `LiveUpdates.PublishChanged` (`LiveUpdates.cs:17`) makes *every* open circuit run its `LoadAsync`; `Pool.razor` alone runs about 8 queries per refresh (`UserComponentBase.cs:58-60`). It is triggered by unlimited actions: SetAvailable / "+1 saat", Riot ID save, profile save (`PoolActions.cs:24-35`). A scripted SignalR client can amplify one click into N×8 SQLite reads. **Fix:** coalesce `PublishChanged` (e.g. 500 ms debounce) and rate-limit SetAvailable.
- **M2. No retry on transient push failures.** On 429, 5xx or timeout, `PushSender.cs:66-76` only logs, while the player is already recorded as notified and cooled down. **Fix:** one retry with jitter inside the 120 s TTL, and record the `ListingNotification` only when a device accepted it (or keep the user eligible for the next wave).
- **M3. IPv6 behind Docker can merge all clients into one IP.** If the VPS gets an AAAA record, Docker publishes `80/443` (`docker-compose.yml:53-56`) and IPv6 clients usually reach the IPv4-only Caddy container through docker-proxy. Caddy then sees the gateway (172.30.0.1) as the client, so all IPv6 users share one bucket: auth 20/min (`Program.cs:165`) and global 600/min. Turkish mobile carriers do use IPv6. **Fix:** verify on the VPS. Either skip the AAAA record, enable IPv6 on the compose network, or run Caddy with `network_mode: host` and add the host address to `ReverseProxy__KnownNetworks`.
- **M4. Off-site backup can be wiped by the sync.** `rclone sync` (`deploy/backup-offsite.sh:16`) mirrors deletions: if `/data/backups` is emptied (volume recreated, disk issue), the off-site copies are deleted on the next run. The volume name `prebot_prebot-data` also assumes the repo directory is named `prebot`. **Fix:** use `rclone copy` (or `sync --backup-dir` / `--max-delete 3`), derive the volume name from `COMPOSE_PROJECT_NAME`, and document plus test a restore.
- **M5. KVKK texts don't match the code.**
  - "Günlükler … en fazla 30 gün" (`Privacy.razor:57`), but Docker rotation is by size (`docker-compose.yml:45`, 10 MB × 5), which at low traffic keeps logs for months.
  - The cross-border section (`Privacy.razor:80-90`) only says some providers are abroad. Since the 2024 amendment (Law 7499), a transfer to a German VPS needs a basis under md. 9, such as a standard contract notified to the Authority. The status report lists this as a lawyer item. It remains open.
  - Retention purge runs only inside the backup job (`Backup.cs:32`): with `Backup:Enabled=false`, the "90 gün" promise is never applied.
  - Discord-created accounts (`WebPoolModule.cs:23-25`, `WebPoolForDiscord.AccountForDiscordUserAsync`) never record `TermsAcceptedAtUtc` or a 16+ confirmation, unlike web logins (`ExternalLogin.cs`).
  - **Fix:** add a time-based `--log-opt` or app-side pruning, move the purge into `PoolWorker` (daily), and require an accept step (ephemeral button) on first Discord interaction.
- **M6. Real first names become public by default.** The display name defaults to the Google given name (`ExternalLogin.cs:86`). It is shown to all users on listing cards and posted to Discord feed channels (`ListingFeed.cs`). **Fix:** ask for a nickname on first login, or default to the Riot game name.
- **M7. Service worker click edge case.** `client.navigate(target)` (`sw.js:47`) rejects for an uncontrolled window (e.g. after a hard reload), and then nothing opens. **Fix:** wrap it in try/catch and fall back to `clients.openWindow`.
- **M8. Join notifications go stale.** `requireInteraction: true` keeps join pushes on screen (`sw.js:21`), and nothing replaces them when the listing fills, closes or expires. Players tap stale alerts and get "artık açık değil". **Fix:** on fill or cancel, send a low-urgency push with the same tag `listing-{id}` saying "Doldu".

## Low

- "+1 saat" sets expiry to now+60 instead of extending it, so it can *shorten* a 2-hour slot (`Pool.razor:36` → `PreService.cs:44`).
- Check-then-insert race on "one open listing per host" (`PreService.cs:123-154`); the UI `_busy` flag mostly hides it.
- `/auth/callback` trusts the `provider` query parameter instead of the authenticated scheme (`AuthEndpoints.cs:61-70`). Not exploitable in practice (id spaces differ), but derive it from `AuthenticationProperties.Items`.
- Cookie `SecurePolicy.SameAsRequest` (`Program.cs:100`): set `Always` outside Development so a forwarded-headers misconfiguration can't produce non-Secure cookies.
- CSP is report-only with no `report-uri`, and `connect-src ws: wss:` is wider than needed.
- `EnableAsync` sends a test push without the TestPush limiter (`PushToggle.razor:150`).
- `ActionLimiter` is in-memory and never pruned, and it resets on restart.
- The profile page can't clear a Riot ID. Deleting an account frees seats but doesn't push those hosts (`Profile.razor:103`).
- The UI hard-codes "15 dakika" (`NewListing.razor:75`) instead of reading `ListingMinutes`.
- The default VAPID subject is `mailto:…@example.com` (`VapidKeys.cs`, compose). Use a real address; Apple is strict about the subject.
- The gitleaks allowlist path `src/PreBot.Web/keys/*.xml` hides future key commits there (`.gitignore` mitigates). The old dev key is still in the public history; it is unused in production.
- Discord embed notes allow masked links (phishing) (`DiscordFeed.cs` description).
- Data Protection keys and the VAPID private key sit unencrypted in `/data` and `/data/backups`, and off-site secrecy depends on the operator configuring rclone `crypt`.
- Mobile Blazor circuits are dropped after 3 minutes of sleep (default retention), so a reload is needed. This is acceptable because push carries the flow.

## Verified OK (in code, not docs)

- **Seat claim.** A single conditional `UPDATE … SlotsFilled+1 WHERE open/not expired/not full/not host/not previously seated` runs inside a transaction (Microsoft.Data.Sqlite defaults to `BEGIN IMMEDIATE`), with a unique `(ListingId, UserId)` index as backstop. Exactly one player gets the last seat.
- **Waves.** A conditional `WavesSent` update prevents double-sends. The web and Discord workers split due waves by community. A unique `(ListingId, UserId)` index stops anyone hearing about the same listing twice.
- **Authz.** Cancel, no-show and reopen check the host server-side, and every action uses the server-resolved `UserId`, so there is no IDOR. Riot IDs are visible only to insiders. Discord-server listings are hidden on the site.
- **Join tokens.** They are Data Protection time-limited (20 min) and bound to listing and user. Auto-join runs only in the interactive render and only once. A copied link seats nobody.
- **Login.** `SafeReturnUrl` + `LocalRedirect` (no open redirect). Logout is a POST with antiforgery. Dev login exists only in Development (tested). Google scopes are `openid profile`; Discord's is `identify`.
- **Deleted accounts.** Account ids use AUTOINCREMENT, so ids are never reused. A cookie for a deleted account is signed out by `UserComponentBase`.
- **Proxy and hosts.** Forwarded headers are trusted only from `172.30.0.0/24`, and Caddy overwrites `X-Forwarded-For`. `AllowedHosts "${DOMAIN};localhost"` works with the curl healthcheck (port ignored). Caddyfile syntax (snippet, `{$DOMAIN}`, `encode`, `reverse_proxy`, WebSockets) is valid. `DOMAIN` is required by the compose file.
- **Time.** The database stores UTC, and SQLite's string comparisons are order-safe. `TurkeyTime` (with a +3 fallback) is used only for alert windows, daily counters and the 05:00 backup. The midnight-wrapping `InHours` is correct.
- **Push.** 404/410 subscriptions are deleted. Urgency is High, TTL 120 s, and Topic is set per listing. The payload is encrypted and the body is small. Endpoints must be https.
- **Backup.** `VACUUM INTO` gives a consistent copy under WAL. Retention is 7 daily / 4 Sundays / 6 months, with a heartbeat.
- **CI.** Gitleaks, `-warnaserror` Release build, migrations drift check, tests, and a Docker smoke test of `/health` and `/`.

---

## Can it solve the problem?

**Technically yes; in practice only under narrow conditions.** The loop is "4 friends → listing → push → 1 tap → host gets Riot ID → in-game invite". It works when:
- at least about 5–15 *push-enabled, Riot-ID'd* players are available at that hour,
- the host has push on,
- the joiner is an honest, Flex-legal rank.

Count of taps from push to the in-game invite:
- Joiner: 1 tap on Android (the "Katıl" action), 2 on iPhone (Safari ignores actions), then accept the friend request and the invite in the client.
- Host: reads the Riot ID on the phone, *types* it into the PC client, waits for the friend request to be accepted, then invites.

That is about 1–3 minutes, which is fine.

The failure modes that remain even when the code "works":
- the pool is empty at that hour (liquidity),
- the first tapper is a ghost, has no Riot ID, or has an illegal rank, and the listing goes silent (C1, H5, H6),
- a player arrives after minute 5 and is never matched (C2),
- the host or iPhone users never got push working (H2, H4),
- the group gives up and plays Normal or ARAM, or splits 2+2 or 3+1. Flex allows those party sizes, so the alternative is always one click away.

## Top 5 missing things

1. **A Riot ID gate on join**, a host re-push when it is added, and later RSO/Riot API rank verification (C1, H5).
2. **Matching for the listing's whole lifetime**: reverse match on "müsaitim" (web and Discord), periodic waves for new candidates until expiry, and reachability-aware picks (C2, H3).
3. **A liquidity plan in the product**: an announced "pre saati", `Bana haber ver` defaulting to it, seed fillers, honest live counters, and a fallback suggestion when nobody comes.
4. **Trust and safety basics**: one active seat per user, the joiner's declared rank, role and op.gg link shown to the host, no-show counts, and admin ban/remove tools (H5, H6).
5. **A real-device and production checklist before launch**: iPhone PWA (install, login inside the PWA, push permission, tap-through), Xiaomi/Samsung battery killers, Windows with Chrome closed, plus the compose stack on the VPS (IPv6 client IPs, OAuth redirect URIs, backup restore), and fixing the H1 worker race.
