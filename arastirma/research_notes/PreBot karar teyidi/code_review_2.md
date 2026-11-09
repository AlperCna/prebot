# PreBot code review 2 (since c77e193 "Denetim düzeltmeleri") — 9 Nov 2026

Scope: `git diff c77e193..HEAD` (12 commits, 86 files) plus end-to-end flow. Read-only review; `dotnet test` = 87 core + 69 web tests, all green. Not exercised: real browsers (iPhone Safari), real Tailscale Funnel headers, live Discord.

## High

**H1. Invite login lets anyone with the code become any invite account, including the admin.**
- `Auth/AuthEndpoints.cs:81`: `GetOrCreateAsync(InviteProvider, displayName.ToLowerInvariant(), …)`. `AccountService.cs:16-20` returns the account that already exists for that (provider, name). There is no per-account secret.
- `Admin.razor:101-103`: admin = `Site:AdminUserIds`. In the trial (`deneme-sitesi.ps1:140,179`), invite is the only login, so the operator's admin id *is* an invite account.
- Scenario: a friend (or anyone the code leaked to) types the operator's display name. That name shows on every listing he hosts (`ListingPage.razor:81`). He is now signed in as the operator and gets `/yonetim`: he can ban everyone, close listings, and read reports. The same works against any friend: he can take their seats, cancel their listings, or press "Verilerimi sil" on their account.
- Fix: if the name is already taken, refuse it unless the browser holds a per-account secret (an HttpOnly "device key" cookie set on first login, or a PIN chosen at first login). Never allow admin ids to sign in through `InviteProvider`; require Google/Discord for admins, or a separate `Site:AdminPin`.

**H2. The invite code can be brute-forced, and the server installer always turns invite login on.**
- Codes are `flex-` plus 4 digits: `deneme-sitesi.ps1:156` gives 1000–9998; `deploy/sunucu-kurulum.sh:47` gives 1000–9999. That is about 9,000 possibilities.
- `sunucu-kurulum.sh:46-49` always writes `SITE_INVITE_CODE`, so the public Azure/VPS install has weak invite login permanently.
- The only brake is the `auth` policy: 20/min per `RemoteIpAddress` (`Program.cs:163-166`). One IPv4 address finds the code in under 7.5 h, 10 IPs in under 45 min. An IPv6 /64 gives unlimited partitions. Failed attempts are not logged.
- Together with H1, an outsider ends up as admin.
- Fix:
  - Generate at least 12 random base32 characters (`RandomNumberGenerator`), not `Get-Random`/`shuf`.
  - Add a global failed-attempt limiter for `/auth/invite-login` (for example 50 failures/hour in total, then lock for 1 h) and partition IPv6 by /64.
  - Log failures.
  - In `sunucu-kurulum.sh`, ask "enable invite login? (y/N)" instead of always generating a code.

**H3. The Tailscale Funnel stays public on port 5215, which is also the Development port, so it can expose `/auth/dev-login` to the internet.**
- `deneme-baslat.ps1:42-43` runs `tailscale funnel --bg 5215`; the comment there says it persists across reboots.
- `Properties/launchSettings.json` uses `http://localhost:5215` with `ASPNETCORE_ENVIRONMENT=Development`. `AuthEndpoints.cs:24-46` maps "sign in as any name" in Development.
- Scenario: the trial is stopped by closing the window or rebooting, and `deneme-durdur.ps1` (the only place that resets the funnel) never runs. A later `dotnet run` exposes dev-login and the developer exception page at the stable `*.ts.net` URL. Anyone can become anyone, including the admin.
- Fix:
  - Run the trial on its own port (for example 5299) in both `deneme-sitesi.ps1` and the funnel command.
  - Additionally refuse dev-login unless `Request.Host.Host` is `localhost`/`127.0.0.1`. Funnel keeps the public Host header, so this check works.
  - Consider not using `--bg`, so the funnel dies with the script.

## Medium

**M1. A ban does not take effect right away.**
- `UserComponentBase.cs:41` checks `BannedAtUtc` only in `OnInitializedAsync`. `PoolActions` (all methods) and `PreService.TryClaimAsync`/`CreateListingAsync` never check it. The cookie is not re-validated (no `OnValidatePrincipal`; 30-day sliding cookie, `Program.cs:95-104`).
- Effect: a banned user with a page already open can keep clicking Katıl, open listings (from an open `/ilan-ac`), and file reports until they navigate.
- `ModerationService.BanAsync` (`:59-68`) only deletes availability and alert subscriptions. The banned user's open hosted listings keep sending waves to other players, still appear in the pool and the Discord feed, and still push host updates to the banned user's devices. Seats the banned user holds in other people's listings stay taken, so those hosts wait for a banned player. Push subscriptions are kept.
- Fix:
  - In `BanAsync`: cancel open/filled hosted listings (and update the feed), release held seats through `ReleaseSeatAsync` (reason "Banned"), and delete push subscriptions.
  - Add an `IsBanned` guard in `PoolActions` (or `PreService`) for create, claim and report.
  - Add `CookieAuthenticationEvents.OnValidatePrincipal`, cached for about 1 min, that rejects banned or deleted accounts.

**M2. A banned user cannot delete their data, and deleting it would erase the ban.**
- `WebPoolModule.DeleteMyDataAsync` (`:182`) calls `MeAsync()`, which throws `BannedException` (`:34`). Site login is blocked for banned users (`AuthEndpoints.cs:82`, `ExternalLogin.cs:90`). So there is no KVKK erasure path.
- If deletion were allowed, `AccountService.DeleteAccountAsync:114` removes the row that holds `BannedAtUtc`, and the next login creates a clean account.
- `PreService.cs:791-793` also deletes every report *against* the user and every report on their listings, so a reported user can wipe the evidence by deleting their account before review.
- Fix: allow deletion for banned users, but keep a minimal tombstone (provider plus a hash of the provider key, with the ban date) and check it in `GetOrCreateAsync`. Keep reports filed by others: anonymise or keep only `ReportedUserId`/reason for a limited period, with a legitimate-interest note in the privacy text.

**M3. The "şifreli" (password-protected) lobby is not protected.**
- `Lobby.cs:17,21`: the password *is* the 4-digit suffix of the public lobby name (`PREBOT DESTEK 4821`, password `4821`). Anyone browsing custom games can read it.
- The password is also shown to every signed-in viewer of the (shareable) listing page (`ListingPage.razor:129-133`, `!insider` branch) and posted in the public Discord announcement (`ListingFeed.cs:74` → `DiscordFeed.cs:57-60`).
- `NewListing.razor` promises that only people PreBot notified can join.
- Fix:
  - Use a separate random password (`LobbyPassword`).
  - Show it only in personal notifications (push body, DM) and to insiders.
  - Leave it out of `FeedListing.LobbyHint` and the non-insider box.
  - Or drop the "şifreli" option.

**M4. "Lobiden biri geldi" closes the whole listing even when more than one seat is missing.**
- `PreService.MarkFilledViaLobbyAsync` (`:367-395`) sets `Filled` regardless of `SlotsNeeded - SlotsFilled`, records `FilledViaLobby` (counted as a fill in `StatsService.cs:53`), and leaves `SlotsFilled` unchanged.
- Scenario: a group needs 2 players and 1 arrives through the lobby. The host presses the button, the listing shows "Kadro tamam" with "0 / 2 dolu", and nobody is searching any more.
- Later, if a PreBot-seated player leaves, `ReleaseSeatAsync:860-869` reopens it with `missing = SlotsNeeded - (SlotsFilled-1)`, which counts the lobby joiner as missing.
- Fix: make the button "+1 from lobby": increment `SlotsFilled` (or a `LobbyFilled` column) and set `Filled` only when it reaches `SlotsNeeded`. Record the metric only on the final fill.

**M5. Lobby defaults and reopen break the lobby flow.**
- `NewListing.razor:106` defaults to `_lobby = "acik"`, so every web listing advertises a custom lobby. Discord defaults to `"yok"` (`WebPoolModule.cs:122`).
- Most hosts never open that lobby, so notified players are sent to look for a lobby that does not exist (push body, DM, feed) and the notification gets longer.
- `PoolActions.ReopenAsync:142` calls `CreateListingAsync(…withLobby…)`, which makes a **new** code (`:57`). The lobby the host already has open in the client no longer matches the reopened listing's notifications.
- Fix: default to "Hayır" (or remember the host's last choice), and reuse `l.LobbyCode` on reopen.

**M6. Client IP behind the tunnels decides whether rate limits work. Verify it.**
- `Program.cs:131-139` trusts loopback for `X-Forwarded-For`/`X-Forwarded-Proto`. Both tunnels connect from 127.0.0.1.
- If Tailscale Funnel sends no `X-Forwarded-For`, every visitor shares the partition `127.0.0.1`. The global 600/min and auth 20/min limits (`:159-166`) then become shared limits: one brute-forcer locks out all logins and `/api/sustur`.
- If it forwards a client-supplied `X-Forwarded-For` unchanged, the IP is spoofable and the limit can be bypassed (feeds H2).
- `AssumeHttps` (`:179-186`) exists exactly because Funnel's header behaviour was uncertain.
- Fix: log `RemoteIpAddress` and the raw `X-Forwarded-For` for a few Funnel requests and confirm. If they are not trustworthy, add an invite-login limiter that does not depend on the IP (see H2).

## Low

- **L1. A report can name any user id.** `ListingPage.razor:366-367` parses `_reportTarget` from a client-sent `<select>` value. Blazor accepts any string, and `ModerationService.ReportAsync` does not check that the target is the host or a slot holder of that listing. A forged report about an unrelated user then gets a one-click "hesabını kapat" button (`Admin.razor:46-49`). Fix: check server-side that the target ∈ {host} ∪ seat holders (including released seats).
- **L2. Mute token issues.**
  - `/api/sustur` (`Program.cs:229-237`) → `PreService.MuteAsync` → `EnsurePlayerAsync` recreates a `Player` row for a user who deleted their data (the token stays valid for 24 h on a notification still on the device). Fix: no-op if the account does not exist.
  - The token travels in the query string, so it lands in Caddy/tunnel access logs (replay only mutes the owner, so the impact is small). Fix: send it as a POST body.
  - The endpoint shares the `auth` bucket with logins; on CGNAT, mute taps compete with sign-ins.
- **L3. "+1 saat" now stacks without limit.** `Pool.razor` sends `MinutesLeft(a) + 60`, and `PreService.SetAvailableAsync:27-46` has no cap. Ten taps keep a player "available" for 10 h, so the pool fills with people who left long ago. Fix: clamp the duration to 3 h on the server.
- **L4. JS rewrites DOM that Blazor owns.** `prebot.js:167-188` replaces `textContent` and then restores `innerHTML` on Blazor-rendered nodes (`.copy-hint` with an `<Icon>`, and the share button). The page re-renders every 20 s and on every pool change. It works today because those children are static, but any future dynamic child will desync Blazor's node references. Fix: toggle a CSS class or `data-state` attribute and show the confirmation text through CSS `::after`, or use a sibling `aria-live` element that Blazor does not render. Gestures are fine: `writeText`/`navigator.share`/`requestPermission` are all called synchronously inside the capture-phase tap, and there are no inline handlers (CSP OK).
- **L5. The install button disappears after the first enhanced navigation.** `MainLayout.razor:9` (static SSR) renders `data-install hidden`. `prebot.js:194-198` unhides it only on `beforeinstallprompt`, which fires once per document. Blazor's enhanced navigation re-syncs the layout DOM and restores `hidden`. Fix: `Blazor.addEventListener('enhancedload', …)` should re-unhide while `prebot._installPrompt` is set, or mark the button `data-permanent`.
- **L6. Discord single-pool details.**
  - Any Discord user who presses Katıl or runs any command gets a site account (`WebPoolModule.cs:30-35`, `DiscordBridge.cs:153-158`). `TermsAcceptedAtUtc` stays null, and the privacy notice is shown only after `/musaitim` and `/profil`. Fix: show the short notice on the first Katıl or `/pre-lazim` reply as well, and record a consent timestamp.
  - There is no `/sustur` on Discord.
  - `PreService.ReachableAsync:607-614` counts every Discord account as reachable, even when its DMs are closed (`_dmClosed` is only in memory) or the bot is disconnected (`DiscordBridge.cs:92-95`). Those users use up wave slots and inflate "X uygun kişi".
  - No duplicate push+DM was found (`ReachableWithoutPushAsync` excludes push users).
- **L7. Trial scripts.**
  - `deneme-durdur.ps1:124`: `tailscale funnel reset` wipes **all** serve/funnel configuration on the machine. Fix: `tailscale funnel --https=443 off`.
  - `deneme-baslat.ps1:44`: when there is no match, `.Matches[0]` on null throws under `ErrorActionPreference=Stop` before the friendly message.
  - `site.log` never contains the app's logs: `deneme-sitesi.ps1:190` uses `Start-Process -NoNewWindow`, which bypasses the `*>` redirection at `baslat:26`.
  - The fallback kill pattern `*.\PreBot.Web.dll*` (`durdur:109-110`) stops *any* PreBot published build started that way, not only the trial.
  - `deneme-tuneli.ps1:210-216` accepts any valid Authenticode signer. Fix: also require `SignerCertificate.Subject -like '*Cloudflare*'`.
  - The profile shows `#123` (`Profile.razor:33`), but `-Yonetici #123` is a PowerShell comment and `"#123"` never matches `IsAdmin`. Fix: strip `#` in `IsAdmin`.
  - The invite code is printed and saved to `davet-mesaji.txt` (intended; no other secrets are printed).
- **L8. `sunucu-kurulum.sh`.**
  - `:49`: `sed` with user input breaks or injects on `|`, `&`, `\` (for example "Alper & Co").
  - `:13`: `$USER` can be unset under `set -u` (cloud-init/sudo).
  - `:27-31`: iptables ACCEPT rules are added again on every run.
  - `:54-58`: the GHCR package must be public, or `pull` fails and the build runs on a 1 GB VM (the swap file mitigates this).
- **L9. AssumeHttps (`Program.cs:179-186`): no real weakening found.** It runs after `UseForwardedHeaders`, so it only overrides the scheme. If it were wrongly enabled on a public plain-HTTP binding, HTTPS redirection would be skipped and Secure cookies set over http. Browsers drop them and OAuth `redirect_uri` breaks, so it fails closed. Fix: log a startup warning when it is on and `ASPNETCORE_URLS` is not loopback-only.
- **L10. Housekeeping.** `src/PreBot.Web/keys/` (DataProtection keys) and `prebot-web.db-wal` sit in the OneDrive-synced source tree. They are gitignored, but the keys are uploaded to OneDrive, which `Program.cs:28-29` warns against. Fix: delete them, and set `DataDirectory` for every non-Development run.

## Verified OK
- Admin actions: handlers are only rendered when `IsAdmin`, Blazor only dispatches rendered handler ids, and `LoadAsync` is gated. A test covers non-admins.
- Host-only server checks: `MarkFilledViaLobbyAsync`, `CancelAsync`, `ReopenForNoShowAsync`, `ReopenAsync` (also checks `GuildId == Web`).
- JoinTokens: separate DataProtection purposes for join and mute (no cross-use), bound to listing:user, expiry checked. Auto-join runs only in the interactive render, once, and not for host, seated or left users; the `?katil` URL is stripped afterwards.
- Mute: a token only mutes its owner. The Notifications page mutes only `UserId`. Muted and banned users are excluded from waves, alert subscriptions and newcomer matches across all channels (InApp, Push, DM). Host updates still arrive, as intended.
- Invite login: antiforgery enforced (form binding plus `<AntiforgeryToken/>`, covered by tests), consent box required server-side, constant-time comparison, `SafeReturnUrl`, banned check, `auth` rate limit, endpoint absent when no code is set.
- Discord: every mutating command goes through `MeAsync`, so bans are enforced; `BannedException` gets a proper reply. `ExternalLogin` blocks banned accounts.
- Lobby code from a CSPRNG. Stats count a listing as filled once (minimum over ListingFilled/FilledViaLobby).
- Account deletion also removes reports and mute state (see M2 for the downside).
- UI redesign: every `<Icon Name>` used is defined; no inline `on*=` handlers or inline scripts were added; report notes are HTML-encoded.
- Push payloads stay well under 4 KB with both tokens; `muteUrl` is serialised camelCase as `sw.js` expects.
- CI publishes images only on master pushes after the test and docker jobs, with `packages: write` only. `backup-offsite.sh` now uses `rclone copy` instead of `sync`, which is safer.
- `dotnet test`: 156/156 pass.
