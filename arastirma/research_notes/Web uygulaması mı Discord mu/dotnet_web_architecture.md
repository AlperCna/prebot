# .NET Web Architecture for a Real-Time LoL "Need a 5th Player Now" Web App (as of November 2026)

Scope: a junior .NET developer with limited time, reusing an existing .NET 10 class library (domain + EF Core/SQLite service with atomic seat-claim logic). Research date: 2026-11-08.

> **CRITICAL FINDING THAT AFFECTS SEVERAL DECISIONS:** Discord has been blocked in Türkiye since 9 October 2024, and Turkish press reports the block was still in place at the end of October 2026, with no confirmed reopening date ([Politikam, 30 Oct 2026](https://www.politikam.com/discord-ve-roblox-turkiyede-acildi-mi-ekim-2026-itibariyla-erisim-engeli-hala-devam-ediyor-tarih-bekleniyor)). "Login with Discord" sends the user's browser to `https://discord.com/oauth2/authorize` ([Discord OAuth2 docs](https://docs.discord.com/developers/topics/oauth2)). If the target users are in Türkiye, Discord login (and a Discord-bot-only product) will not work for anyone who is not using a VPN. See the Authentication section.

---

## 1. Frontend choice: Blazor Web App render modes (.NET 10) vs React/Vue SPA + ASP.NET Core API

### Takeaway
**Recommended default: a single Blazor Web App (.NET 10).** Use **Interactive Server** for the live pages (pool, listings, join buttons) and **Static SSR** for the landing, ToS and privacy pages. Add a hand-written web manifest and a small service worker for "installable + push", rather than using the standalone WASM PWA template. Interactive Server gives the smallest payload, direct use of the existing Core/EF library with no API layer, and server-pushed UI updates over the circuit. .NET 10 also fixes the biggest mobile pain point (losing the circuit when a phone switches apps) with circuit state persistence. React/Vue + API only pays off if the main goal is frontend portfolio value, and it roughly doubles the work.

### Cited Findings
- .NET 10 Blazor Web App render modes: Static Server (static SSR, no interactivity), Interactive Server (Blazor Server, server-side), Interactive WebAssembly (CSR), and Interactive Auto ("Interactive SSR using Blazor Server initially and then CSR on subsequent visits after the Blazor bundle is downloaded"). Prerendering is enabled by default for interactive components. — [MS Learn: Blazor render modes (.NET 10)](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-10.0)
- Interactive Server handles UI events "using a real-time connection with the browser called a circuit"; the circuit is closed when no Interactive Server components remain on the page, freeing server resources. — [MS Learn: render modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-10.0)
- WebAssembly and Auto components "must be built from a separate client project" that sets up the WASM host. This adds a second project and forces an API boundary. — [MS Learn: render modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-10.0)
- Auto mode "never dynamically changes the render mode of a component already on the page"; it makes an initial decision at render time. — [MS Learn: render modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-10.0)
- Static SSR should be used only for pages that need it (for example, pages reading/writing HTTP cookies). For pages that work interactively, "you shouldn't force them to use static SSR, as it's less efficient and less responsive." — [MS Learn: render modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-10.0)
- Blazor Server benefits: download size "significantly smaller" with a faster load; full .NET API access; normal .NET debugging; app code isn't served to clients. Limitations: "Higher latency usually exists. Every user interaction involves a network hop"; "no offline support"; scaling to many users needs server resources; an ASP.NET Core server is required (no CDN/serverless). — [MS Learn: Blazor hosting models](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-10.0)
- Blazor WASM limitations: "Download size is larger, and components take longer to load"; "Code sent to the client can't be protected from inspection and tampering"; a server API is needed to access server resources. — [MS Learn: hosting models](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-10.0)
- Each browser tab gets its own circuit. Graceful close frees resources immediately. On a non-graceful disconnect (network drop), "Blazor Server stores disconnected circuits for a configurable interval to allow the client to reconnect." — [MS Learn: hosting models](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-10.0)
- **.NET 10 mobile/reconnect improvement:** "Blazor Web Apps can now persist a user's session (circuit) state when the connection to the server is lost for an extended period of time or proactively paused, as long as a full-page refresh isn't triggered." Covered scenarios include tab throttling, **mobile app switching**, network interruptions and pausing inactive circuits. The template now includes a CSP-friendly `ReconnectModal` component and a new `retrying` reconnect state. — [MS Learn: What's new in ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0?view=aspnetcore-10.0)
- .NET 10 adds a declarative persistent-component-state attribute (removing boilerplate during prerendering) and `RestoreBehavior.SkipLastSnapshot` "to ensure fresh data after reconnection". The second is relevant for a live pool. — [MS Learn: What's new in ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0?view=aspnetcore-10.0)
- .NET 10 WASM changes are about preloading, fingerprinting and caching (`ResourcePreloader`, boot config inlined into `dotnet.js`, removal of `BlazorCacheBootResources`). The release notes do not claim a specific download-size reduction. — [MS Learn: What's new in ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0?view=aspnetcore-10.0)
- .NET 10 ASP.NET Core Identity supports **passkeys** (WebAuthn/FIDO2), and the Blazor Web App template provides passkey management and login out of the box. — [MS Learn: What's new in ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0?view=aspnetcore-10.0)
- **PWA template:** the "Progressive Web Application" option exists for **Blazor WebAssembly App** (`dotnet new blazorwasm --pwa`). The docs also describe converting an existing WASM app by adding `manifest.webmanifest`, `service-worker.js` and icons. — [MS Learn: Blazor PWA](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/?view=aspnetcore-10.0)
- Push notifications "are independent of Blazor WebAssembly. Sending a push notification is implemented by the backend server, which can use any technology. Receiving and displaying a push notification on the client is implemented in the service worker JavaScript (JS) file." The server side uses the third-party [`WebPush`](https://www.nuget.org/packages/WebPush) NuGet package with VAPID keys. The reference sample is Blazing Pizza. — [MS Learn: Blazor PWA push notifications](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/push-notifications?view=aspnetcore-10.0)
- PWA offline caveats: offline support "adds significant complexity", and auth flows must bypass the cached `index.html`, for example by excluding `/signin-google`-style callback paths in the service worker. — [MS Learn: Blazor PWA](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/?view=aspnetcore-10.0)
- **iOS web push:** supported since iOS/iPadOS 16.4, but only for web apps **added to the Home Screen**, and the permission request must come from direct user interaction (for example a subscribe button). Regular Safari tabs cannot request push permission. — [WebKit: Web Push for Web Apps on iOS and iPadOS](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/); [9to5Mac](https://9to5mac.com/2023/02/16/iphone-web-app-new-features-ios-16-4/)
- Recommended sustained UI latency for Blazor Server is 250 ms or less. — [MS Learn: Host and deploy server-side Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server/?view=aspnetcore-10.0)
- React was described as dominant on the web as of January 2026 (vendor blog, market-share framing). — [Leobit: Blazor vs React](https://leobit.com/blog/blazor-vs-react/)

### Inferences
- **Interactive Server fits this app.** The UI is a small number of buttons plus a live list. All state that matters (seats, availability) lives on the server anyway, and the seat-claim logic is already a server-side EF Core service. Interactive Server lets Razor components inject the existing Core service directly, with no REST API, DTO layer, or client auth token handling.
- **"Live pool" without a separate hub:** in Interactive Server mode, a singleton "PoolNotifier" service (C# events or `Channel<T>`) can raise "pool changed" events, and each component calls `InvokeAsync(StateHasChanged)`. The circuit's SignalR connection carries the UI diff. An explicit SignalR hub is only needed later, for non-Blazor clients (mobile app, Discord bot, WASM).
- **Interactive Auto/WASM are not worth it for v1.** They need a second project plus an HTTP API and auth for the client, and the existing EF Core/SQLite code cannot run in the browser. That roughly doubles the surface area for a junior developer.
- **PWA + push with Blazor Web App:** the PWA template is WASM-only, but push is plain browser JS (service worker + Push API) plus server-side `WebPush`. So a Blazor Web App can add `manifest.webmanifest`, a minimal service worker that handles only `push`/`notificationclick` (no offline caching), and a JS-interop subscribe button. This gives installability (needed on iOS) and push without WASM or offline complexity. This is an inference from the docs above; Microsoft does not document it as a template path.
- **SEO/landing:** a Static SSR (or prerendered) landing page is crawlable HTML. Riot's reviewers also need a visible site with ToS/privacy, so the landing, ToS and privacy pages should be static SSR.
- **Mobile reconnect risk:** before .NET 10 this was the main argument against Blazor Server for phone users. With .NET 10 circuit pause/resume plus the reconnect modal, it is acceptable for a "check the pool, tap join" app. For the "5th player needed NOW" moment, rely on **push notifications** (which work while the app is closed) rather than an open circuit.
- **React/Vue + API alternative:** choose it only if the developer wants frontend skills on the CV and accepts about 1.5–2x the build time (new language/tooling, API design, CORS/auth tokens, and `@microsoft/signalr` client wiring).

### Gaps
- No Microsoft-published number for a minimal .NET 10 Blazor WASM download size was found in the docs fetched.
- No official Microsoft guide exists for "PWA + push in a Blazor Web App (Interactive Server)". The recommendation is an inference from the WASM PWA and push docs.

---

## 2. Real-time: SignalR hubs, scaling on one VPS, Azure SignalR

### Takeaway
**Recommended default:** a single ASP.NET Core process on one VPS, with no Azure SignalR and no Redis backplane. Use the Blazor circuit for live UI. Add one small `PoolHub` only if non-Blazor clients (the Discord bot, a future mobile app) need live events. Proxy WebSockets correctly through Caddy/nginx.

### Cited Findings
- Sticky sessions are **not** required when "Hosting on a single server in a single process". — [MS Learn: SignalR hosting and scaling](https://learn.microsoft.com/en-us/aspnet/core/signalr/scale?view=aspnetcore-10.0)
- SignalR connections are persistent and consume TCP connections and memory per connection. Running out of connections causes socket and connection-reset errors. — [MS Learn: SignalR scale](https://learn.microsoft.com/en-us/aspnet/core/signalr/scale?view=aspnetcore-10.0)
- Scale-out needs Azure SignalR Service or a Redis backplane. "The Redis backplane is the recommended scale-out approach for apps hosted on your own infrastructure." Azure SignalR is recommended for apps hosted *on Azure*. — [MS Learn: SignalR scale](https://learn.microsoft.com/en-us/aspnet/core/signalr/scale?view=aspnetcore-10.0)
- Blazor circuit memory: "Each circuit uses approximately 250 KB of memory for a minimal Hello World-style app … If you expect your app to support 5,000 concurrent users, consider budgeting at least 1.3 GB of server memory to the app (or ~273 KB per user)." Memory is "likely the first resource that the app exhausts." — [MS Learn: Host and deploy server-side Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server/?view=aspnetcore-10.0)
- "A Blazor app on Azure App Service doesn't require Azure SignalR Service". On App Service, WebSockets must be turned on (the default is Off). — [MS Learn: Host and deploy server-side Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server/?view=aspnetcore-10.0)
- Blazor works best with WebSockets. Long Polling is a fallback, and a console warning appears if it is used. — [MS Learn: Host and deploy server-side Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server/?view=aspnetcore-10.0)
- Nginx for SignalR needs `proxy_set_header Upgrade $http_upgrade; proxy_set_header Connection $connection_upgrade; proxy_http_version 1.1;`. For Blazor, use `location /` and drop `proxy_buffering off`. — [MS Learn: SignalR scale (nginx)](https://learn.microsoft.com/en-us/aspnet/core/signalr/scale?view=aspnetcore-10.0); [MS Learn: Blazor server hosting (nginx changes)](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server/?view=aspnetcore-10.0)

### Inferences
- At the realistic scale of a Turkish LoL LFG MVP (tens to a few hundred concurrent users), circuit memory is about 250–500 KB each, so 500 users is roughly 125–250 MB. A 4 GB VPS is far more than enough.
- Caddy proxies WebSockets transparently with a one-line `reverse_proxy`, which is easier for a junior developer than the nginx config above. (General Caddy knowledge; no Caddy doc was fetched in this session.)

### Gaps
- No benchmark was found for Blazor circuits on a 2 vCPU/4 GB VPS specifically.

---

## 3. Authentication: Discord OAuth, Riot Sign-On, alternatives

### Takeaway
**Recommended default (revised because of the Türkiye block):** do **not** make Discord the only login. Use ASP.NET Core cookie auth with **Google login as primary** and **Discord login as secondary** for non-Turkish or VPN users. Optionally add **.NET 10 Identity passkeys** for people who want neither. Plan to add **Riot Sign-On (RSO)** only after a production key is approved. For Discord, use the `AspNet.Security.OAuth.Discord` package (actively maintained, net10.0) with only the `identify` scope.

### Cited Findings
- **Discord is blocked in Türkiye:** the access ban took effect on 9 October 2024. "Ekim 2026'nın sonu itibarıyla Discord'a Türkiye'den erişim engeli devam ediyor" (the block continues as of end of October 2026). There is no confirmed reopening date; talks with the Ministry of Transport and Infrastructure are reported. — [Politikam, 30 Oct 2026](https://www.politikam.com/discord-ve-roblox-turkiyede-acildi-mi-ekim-2026-itibariyla-erisim-engeli-hala-devam-ediyor-tarih-bekleniyor). Original reason reported as "child sexual abuse and obscenity" — [NTV](https://www.ntv.com.tr/turkiye/discorda-erisim-engeli,nnlgpHdb40Kh1XAiLzrJVQ). Many Turkish results are SEO-style articles, and no official BTK statement on reopening was found.
- Discord OAuth2 authorize URL: `https://discord.com/oauth2/authorize`. The `identify` scope "allows `/users/@me` without `email`"; the separate `email` scope adds email. — [Discord Developer Docs: OAuth2](https://docs.discord.com/developers/topics/oauth2)
- **AspNet.Security.OAuth.Discord:** latest version **10.1.0** (published 8 Nov 2026); 10.0.0 (11 Nov 2025, about 151k downloads); 9.4.1 (Sep 2025). It targets **net10.0** and has about 1.3M total downloads (about 904/day). — [NuGet: AspNet.Security.OAuth.Discord](https://www.nuget.org/packages/AspNet.Security.OAuth.Discord)
- **RSO** is only available to "applications that have an existing, approved production application ID"; it uses SSO with OpenID. — [Riot Developer FAQ](https://developer.riotgames.com/docs/faqs)
- Discord MAU estimates for 2026 vary widely (about 200M–430M), and Discord no longer publishes exact numbers. There is no reliable "percentage of gamers who have Discord" figure. One third-party stat says 78% of users use it for non-gaming activities (unverified). — [AffMaven](https://affmaven.com/discord-statistics/); [DemandSage](https://demandsage.com/discord-statistics/); [Statista topic page](https://www.statista.com/topics/9816/discord)

### Inferences
- The OAuth redirect to `discord.com` will fail for Turkish users unless they use a VPN or DNS workaround, so Discord-only login would exclude much of the target audience. The same logic weakens a **Discord-bot-only** product for Türkiye. This is a strong argument for the web app being the primary product, with the Discord bot kept as an optional notifier for communities that can reach Discord.
- Google login (`Microsoft.AspNetCore.Authentication.Google`, a first-party package) is the most universal fallback for Turkish gamers. Passkeys in the .NET 10 Identity template avoid passwords entirely. Plain email/password is not recommended: it brings password reset, email sending, breach liability and KVKK data scope.
- Keep the `identify` scope only. It gives Discord user ID, username and avatar with no email, which minimises personal data.
- **Implementation note for Blazor:** the OAuth challenge and cookie sign-in must run through HTTP endpoints (minimal API `/login`, `/logout`) or static SSR pages, not inside an interactive circuit. This matches the render-modes doc's note that cookie-dependent pages need the request/response cycle ([MS Learn: render modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-10.0)).

### Gaps
- No reliable data was found on Discord account penetration among Turkish gamers, or on VPN usage for Discord in Türkiye after the block.
- The aspnet-contrib GitHub repo was not fetched directly. Maintenance status is inferred from the NuGet release cadence (a release on 8 Nov 2026 for .NET 10).

---

## 4. Riot API integration: personal vs production key, and account verification

### Takeaway
A public website **cannot** run on a personal key. A production key requires a working, near-complete website with visible ToS and privacy policy, so **building the web app is the path to a production key** (and later RSO). **Recommended default:** develop with a dev/personal key privately, launch publicly only after the production key is approved, and design the MVP so Riot data is optional. In the meantime, the user can type their Riot ID manually, with no API calls.

### Cited Findings
- Key types: **Development** keys are for prototyping and "deactivate every 24 hours". **Personal** keys: "You may not run your application for public consumption using a personal key," and this includes open alpha/beta tests. **Production** keys are "for products that are intended for large communities or the Internet as a whole." — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- Rate limits: personal 20 req/1 s and 100 req/2 min; production (starting) 500 req/10 s and 30,000 req/10 min (per region). — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- Production key: "Typically requires a working prototype"; "If your website isn't complete, we're unlikely to approve your product." — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- FAQ: "We need to see a website to understand what your app does, view your Terms of Service and Privacy Policy"; "Production keys are for applications that are in a completed or near-complete state"; GitHub repos are not accepted in lieu of a functioning site; applications "are typically reviewed weekly" and review "can take up to three weeks". — [Riot Developer FAQ](https://developer.riotgames.com/docs/faqs)
- Site verification: after applying you "receive a text string to upload to your URL" to prove domain ownership, and there are no alternative methods. — [Riot Developer FAQ](https://developer.riotgames.com/docs/faqs); [Riot support: Verification for Production Applications](https://support-developer.riotgames.com/hc/en-us/articles/22801461443091-Verification-for-Production-Applications)
- General policies: all products must be registered and audited via the Developer Portal; one product per production key; a free tier is required if monetised; no betting; "Products cannot de-anonymize players"; "Products cannot create alternatives for official skill ranking systems". A legal boilerplate ("isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games") must be readily visible. — [Riot General Policies](https://developer.riotgames.com/policies/general)
- Third-party apps must display Riot IDs (not summoner names) in frontend fields. — [Riot LoL docs](https://developer.riotgames.com/docs/lol) (via search summary)
- Third-party claims on approval time conflict (a blog says 5–8 months; another guide says up to a month), versus Riot's FAQ statement of weekly reviews and up to three weeks. — [darkintaqt blog](https://darkintaqt.com/blog/riot-app-application); [Riot FAQ](https://developer.riotgames.com/docs/faqs)

### Inferences
- **Icon verification flow (common community pattern, not found in Riot's official docs):**
  1. The user enters a Riot ID.
  2. The server resolves the PUUID (Account-V1) and shows a randomly chosen default profile icon.
  3. The user sets that icon in the client and clicks "Verify".
  4. The server re-reads Summoner-V4 `profileIconId` and marks the account verified on a match, with an expiry of about 10 minutes.

  In Blazor Interactive Server this is one component with a countdown and a "Check" button. RSO replaces it once available.
- Because a public site needs a production key, the MVP should **launch without live Riot API calls** (manual Riot ID plus self-reported rank/role). Turn on verification and rank lookup after approval. The live site with ToS, privacy policy and boilerplate is what gets the key approved.

### Gaps
- No official Riot documentation for summoner-icon ownership verification was found. It is a community pattern, so verify on the Riot developer Discord or support site.
- Whether an LFG/matchmaking-style product raises policy issues was not explicitly addressed. The closest rule is "no alternatives for official skill ranking systems".

---

## 5. Database: SQLite vs PostgreSQL

### Takeaway
**Recommended default: keep SQLite for the MVP** (the existing library already uses it). Enable WAL mode and a busy timeout, keep write transactions short, run a single app instance, and back up the DB file. Because the code is EF Core, moving to PostgreSQL (Npgsql) later is mostly a provider swap plus new migrations, so plan it for when you need more than one app instance or see sustained write contention.

### Cited Findings
- SQLite allows only one writer at a time (global write lock). Under concurrent writes, transactions wait or fail with "database is locked". — [tenthousandmeters: SQLite concurrent writes](https://tenthousandmeters.com/blog/sqlite-concurrent-writes-and-database-is-locked-errors/); [OneUptime, Sep 2026](https://oneuptime.com/blog/post/2026-09-08-fix-sqlite-database-is-locked-concurrent-writes/view)
- WAL lets reads proceed during a write but doesn't remove writer serialisation. The common fix combines WAL, a busy timeout (3–5 s suggested) and short write transactions. — [OneUptime](https://oneuptime.com/blog/post/2026-09-08-fix-sqlite-database-is-locked-concurrent-writes/view); [darkthread (zh)](https://blog.darkthread.net/Blog/sqlite-multithreading-issue)
- Jellyfin's move to EF Core with SQLite hit locking issues; it used EF Core interceptors to serialise or retry writes. — [Jellyfin: SQLite locking](https://jellyfin.org/posts/SQLite-locking/)
- Community NuGet packages offering EF Core SQLite write queues exist (for example EntityFrameworkCore.Sqlite.Concurrency 10.0.x). Their performance claims are vendor-provided. — [NuGet: EntityFrameworkCore.Sqlite.Concurrency](https://feed.nuget.org/packages/EntityFrameworkCore.Sqlite.Concurrency)

### Inferences
- The workload is mostly reads (live list), with occasional small writes (toggle available, create listing, claim seat). At MVP scale SQLite with WAL is fine. The atomic seat-claim logic is a natural fit for SQLite's single-writer model: the claim transaction serialises by design.
- In Blazor Server, use `IDbContextFactory<T>` and create a short-lived context per operation. Do not hold a single DbContext for the life of a circuit, because DbContext is not thread-safe.
- Store the SQLite file on a Docker **volume**, back it up nightly, and apply migrations at startup (or with a migration bundle) on a single instance.

### Gaps
- No fetched Microsoft doc gives SQLite concurrency limits for EF Core web apps. Findings come from blogs and engineering posts.

---

## 6. Hosting & cost

### Takeaway
**Recommended default:** one small Linux VPS (Hetzner Cloud shared-vCPU, or a comparable EU provider if Hetzner's cheap line is sold out) running Docker Compose with the app container plus **Caddy** (automatic Let's Encrypt HTTPS), and a `.com` or `.com.tr` domain. Expect roughly €6–7/month for the server plus about $10–15/year for a .com (or a few dollars to roughly 1,300 TL/year for .com.tr depending on registrar). Avoid Azure App Service Free (F1) for this app. B1 or Railway are fallbacks if the developer does not want to run a server.

### Cited Findings
- Hetzner raised prices on 1 April 2026 (up to about 37%) and again on 15 June 2026. The June round was much larger for some families (CPX/CCX), applies to new orders and rescales, and is attributed to DRAM/NAND costs. — [Tom's Hardware](https://www.tomshardware.com/tech-industry/hetzner-to-raise-prices-by-up-to-37-percent-from-april-1); [heise](https://heise.de/-11333037); [wz-it blog](https://wz-it.com/en/blog/hetzner-price-increase-june-2026-cpx-ccx-alternatives/)
- Hetzner's cost-optimized page (fetched Nov 2026) lists **CX23: 2 vCPU / 4 GB / 40 GB / 20 TB, €5.99/mo incl. IPv4**; CAX11 (ARM) €6.49; CX33 (4 vCPU / 8 GB) €8.99. **All are marked "currently unavailable"** in that snapshot. — [Hetzner Cloud Cost-Optimized](https://www.hetzner.com/cloud/cost-optimized/). The main cloud page shows Cost-Optimized from €5.99 and Regular Performance from €6.49 (shared). — [Hetzner Cloud](https://www.hetzner.com/cloud/)
- **Conflict:** wz-it reports CX23 at €5.49 and CAX11 at €5.99 after June 2026, versus Hetzner's own page at €5.99/€6.49. The difference may be IPv4 inclusion or a later change. wz-it also notes CX/CAX was "Currently not available" as of 1 Oct 2026. — [wz-it](https://wz-it.com/en/blog/hetzner-price-increase-june-2026-cpx-ccx-alternatives/) vs [Hetzner](https://www.hetzner.com/cloud/cost-optimized/)
- **Azure App Service Linux:** F1 Free has 60 CPU minutes/day and 1 GB RAM; "Free and shared service plans do not support SSL"; no SLA and not for production. B1 has 1 core, 1.75 GB RAM and 10 GB storage (price not rendered on the page fetched). — [Azure App Service Linux pricing](https://azure.microsoft.com/en-us/pricing/details/app-service/linux/)
- Azure App Service needs WebSockets turned on (default Off) and session affinity for Blazor Server. — [MS Learn: Host and deploy server-side Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server/?view=aspnetcore-10.0)
- **Railway:** Hobby is $5/mo including $5 of usage. Usage costs about $20/vCPU-month, $10/GB-RAM-month and $0.15/GB-month for volumes; Hobby allows 2 custom domains. — [Railway pricing](https://railway.com/pricing)
- Fly.io raised RAM pricing about 20% in 2026, per a third-party blog. — [buildmvpfast](https://www.buildmvpfast.com/blog/fly-io-pricing-2026-ram-increase-vs-hetzner) (search title only; not fetched)
- **.com.tr:** since TRABİS (14 Sep 2022), .com.tr/.net.tr/.org.tr are generally first-come-first-served with no documents needed. Prices vary widely by registrar and campaign: about $1.69–1.91 for the first year at some resellers; Hostinger lists .tr at 889 TL first year and 1,334 TL/year renewal. — [karekod.org (Sep 2026)](https://www.karekod.org/blog/domain-fiyatlari/); [Com Laude](https://comlaude.com/removal-of-restrictions-for-com-tr/); [TRABİS announcements](https://www.trabis.gov.tr/announcements); [Hostinger TR](https://www.hostinger.com/tr/tld/tr-domain)

### Inferences
- **Always-on SignalR + push** means the process must never sleep. A VPS is the cheapest reliable option. Railway Hobby with a ~0.5 GB container running 24/7 would cost about $5 RAM + vCPU share, so roughly $5–10/mo after the included credit, and it needs a volume for SQLite. Azure F1 is unusable (CPU quota, no SSL); Azure B1 is typically tens of USD/month (verify in the calculator).
- On a VPS the SQLite file, the Data Protection keys (persist them to a volume so auth cookies survive restarts) and the VAPID private key all live on a Docker volume or in environment secrets.
- Run the server in an EU region (Nuremberg/Falkenstein/Helsinki) to keep latency to Türkiye reasonable, under the 250 ms UI-latency target.

### Gaps
- An exact Azure B1 Linux monthly price was not rendered on the page fetched.
- The current .com retail price was not fetched. Exact .com.tr TL pricing at major Turkish registrars (Natro, İsimtescil, Turhost) was not verified.
- Hetzner availability is volatile. Check the console on purchase day.

---

## 7. Build-time estimate and suggested project structure

### Takeaway
**Recommended default:** one solution with three projects: `Core` (existing), `Web` (Blazor Web App, Interactive Server), and an optional `DiscordNotifier` worker. A realistic MVP for a junior developer working part-time is about **4–7 weeks** (roughly 60–110 hours). The estimate is an inference; no source was found.

### Cited Findings
- Blazor Server components have "direct access to server and network resources", which avoids building a separate API. — [MS Learn: hosting models](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-10.0)
- Interactive WASM/Auto require a separate client project. — [MS Learn: render modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-10.0)
- Push requires VAPID keys, a subscription endpoint storing endpoint/p256dh/auth per user, a service worker `push` handler, and the server-side `WebPush` package. — [MS Learn: Blazor PWA push](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/push-notifications?view=aspnetcore-10.0)

### Inferences
- **Suggested structure:**
  ```
  LolPre.sln
  ├─ src/LolPre.Core/            (existing: domain, EF Core DbContext, SeatClaimService)
  ├─ src/LolPre.Web/             (Blazor Web App, .NET 10, Interactive Server global or per-page)
  │   ├─ Components/Pages/       Home (static SSR landing), Pool (live), Listing/{id}, Profile, Privacy, Terms
  │   ├─ Auth/                   AuthEndpoints.cs (/login/google, /login/discord, /logout) + cookie config
  │   ├─ Realtime/               PoolNotifier (singleton events) [+ optional PoolHub for bot/mobile]
  │   ├─ Push/                   PushSubscription entity, /push/subscribe endpoint, PushSender (WebPush)
  │   └─ wwwroot/                manifest.webmanifest, sw.js (push + notificationclick only), icons
  ├─ src/LolPre.DiscordNotifier/ (optional Worker Service: subscribes to Core events/PoolHub, posts to channels)
  └─ tests/LolPre.Core.Tests/    (seat-claim concurrency tests)
  ```
- **Effort breakdown (part-time junior, hours):**

  | Item | Hours |
  |---|---|
  | Template setup + Core wiring + DbContextFactory | 4–8 |
  | Google/Discord OAuth + cookie + profile | 10–16 |
  | Available toggle + live pool (notifier pattern) | 8–14 |
  | Create/join listing using existing seat-claim | 8–14 |
  | Web push (VAPID, SW, subscribe UX, iOS install hint) | 10–18 |
  | Landing/ToS/privacy/Riot boilerplate | 4–6 |
  | Docker + Caddy + VPS + domain + backups | 6–12 |
  | Polish/testing/bugs | 10–20 |
  | **Total** | **≈ 60–110 hours** |

  At 15 h/week that is about 4–7 weeks. Riot production key review (weekly, up to 3 weeks per the FAQ) runs in parallel after launch-readiness.
- Keep the Discord bot as an **optional notifier**. It consumes the same Core events, so it can be dropped for Turkish users without affecting the web app.

### Gaps
- No empirical data was found on junior developer build times for comparable Blazor apps. The estimate is the researcher's judgment.

---

## 8. Portfolio value in the Turkish junior .NET job market (Blazor vs React)

### Takeaway
There is no reliable Türkiye-specific data comparing Blazor and React demand. Global signals suggest React is far more common in job ads, while Blazor is a niche plus for .NET shops. **Recommended default:** build the MVP in Blazor (fastest to ship, showcases ASP.NET Core, EF Core, SignalR, OAuth and Docker), and present the deployed, real-user product as the portfolio piece. If frontend demand matters, a later React client against a small API would be an additional, separate skill signal.

### Cited Findings
- A Turkish junior full-stack posting (The British School İstanbul, July 2026) exists on techcareer.net, but the snippet does not specify Blazor or React. — [techcareer.net](https://www.techcareer.net/jobs/detail/junior-yazlm-gelistirme-uzman-4500975)
- React is described as dominant on the web as of Jan 2026 (vendor blog). — [Leobit](https://leobit.com/blog/blazor-vs-react/)
- Blazor is positioned as attractive for C#/.NET teams because it uses one language for backend and frontend (vendor opinion). — [Technijian](https://technijian.com/software-development/react-vs-blazor-for-enterprise-apps-which-framework-should-your-team-build-on-in-2026/)

### Inferences
- For Turkish junior .NET roles, employers usually screen on C#, ASP.NET Core, EF Core and SQL first. A live, deployed real-time app with OAuth, SignalR, Docker and a Riot production key is likely more valuable than the choice of frontend framework.

### Gaps
- No Kariyer.net or LinkedIn TR counts for "Blazor" vs "React" were obtainable through search; those sites were not fetchable. Manual keyword searches on Kariyer.net, LinkedIn Jobs (Türkiye) and techcareer.net are recommended to get real counts.
