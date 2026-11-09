# Free-tier PaaS / container hosting for PreBot (.NET 10 Blazor Server, Docker, always-on), as of Nov 2026

App requirements: always-on, since a background worker runs every 10 s and sleeping breaks notifications; WebSockets/SignalR; 300–600 MB RAM; persistent SQLite or a managed DB; outbound HTTPS for Web Push; public HTTPS URL; budget 0; no card preferred; owner and users in Turkey.

Research date: 2026-11-09. Many 2026 sources are SEO/aggregator or competitor pages (SnapDeploy, agentdeals.dev, kuberns, costbench, temps.sh). They are flagged where used. Official docs were fetched where possible.

## Q1. Which platforms offer a free tier that is truly always-on and can run a Docker image (the critical filter)?

### Takeaway
Only one mainstream PaaS confirms in its own docs a free, no-card, always-on (no sleep) container tier: **Northflank Sandbox**. Every other free PaaS tier either sleeps or scales to zero (Render, Koyeb, Zeabur, Hugging Face, Cloud Run/ACA at min=0, Back4app with an hours cap), or has no free tier for new users (Fly.io, Heroku, Clever Cloud, Cloudflare Containers). Railway's free plan does not sleep, but its $1/month credit cannot keep a 300–600 MB .NET app running.

### Cited Findings
**Northflank (Sandbox): best fit**
- Official pricing lists the Sandbox as $0 with 2 services, 2 jobs and 1 database (2 projects, 1 addon), "up to $20/mo of resources on Northflank Cloud", **"Always on, no sleeping"**, and **"No card needed"** — [Northflank pricing](https://northflank.com/pricing)
- On the same page, compute and vertical scaling are only "Limited", with no per-service CPU/RAM figure for the free tier. Volumes/persistence are listed, but no free disk allowance is stated — [Northflank pricing](https://northflank.com/pricing)
- Card requirement conflict: Northflank's own page says no card. Third-party reviews say a card is required for activation — [search summary incl. budgetforge / toolradar](https://www.budgetforge.dev/tools/northflank-pricing-2026); [freetier.co](https://freetier.co/directory/products/northflank). Treat the official page as authoritative but expect possible card/identity verification.
- Volumes are billed per GB-month on Northflank. I found no documented free-tier volume cap — [Northflank sandboxes docs](https://northflank.com/docs/v1/application/sandboxes/sandboxes-on-northflank-cloud); [resource plans & quotas](https://northflank.com/docs/v1/application/billing/resource-plans-and-quotas)
- Europe regions exist: Europe-West, Netherlands, Frankfurt. A project's region cannot be changed after creation — [Northflank Europe-West region](https://northflank.com/cloud/northflank/regions/europe-west); [Northflank sandbox docs](https://northflank.com/docs/v1/application/sandboxes/sandboxes-on-northflank-cloud)

**Koyeb: sleeps, no volumes on free**
- Free Instance: 512 MB RAM, 0.1 vCPU, 2 GB SSD, one per organization, Frankfurt or Washington D.C. only. It scales to zero after 1 h without traffic, and **"can't be used with Volumes"**. It cannot be a Worker service. Docs last updated 27 May 2026 — [Koyeb Instances docs](https://www.koyeb.com/docs/reference/instances)
- Scale-to-zero on the free instance cannot be disabled or customized. Cold start is 1–5 s — [Koyeb scale-to-zero docs](https://www.koyeb.com/docs/run-and-scale/scale-to-zero)
- The current Koyeb pricing page shows only Pro ($29/mo), Scale ($299/mo) and Enterprise. Its only explicit $0 line is "Free" Serverless Postgres (0.25 vCPU, 1 GB RAM, 1 GB storage, "5h"), available in Washington, Frankfurt or Singapore — [Koyeb pricing](https://www.koyeb.com/pricing)
- Card: a competitor page says the card-free Hobby plan was removed in Feb 2026 and a $29 pre-authorization hold is now required — [SnapDeploy state of free hosting (competitor, verified 26 Sep 2026)](https://snapdeploy.dev/state-of-free-hosting). **Unverified against Koyeb.** The 2023 Koyeb blog said a card is asked only if automatic human verification fails, and noted Koyeb has since been acquired by Mistral AI — [Koyeb blog, Oct 2023](https://www.koyeb.com/blog/sustaining-free-compute-in-a-hostile-environment)

**Render: sleeps, no disk on free**
- A free web service spins down after 15 min with no inbound traffic, counting both HTTP requests and WebSocket messages on existing connections. Spin-up takes about 1 min. The filesystem is ephemeral, so a local SQLite file is lost on redeploy, restart or spin-down. Free services cannot attach a persistent disk — [Render docs: Deploy for Free](https://render.com/docs/free)
- 750 free instance hours per workspace per month. 512 MB RAM. Free Postgres is 1 GB, expires after 30 days, then 14 days' grace. Hobby bandwidth is 5 GB. No card needed — [flaviocopes.com, checked Sep 2026](https://flaviocopes.com/hosting-free-tiers/); [SnapDeploy](https://snapdeploy.dev/state-of-free-hosting) (says the Hobby bandwidth cut from 100 GB to 5 GB came on 23 Apr 2026)
- The paid fix is an instance from $7/mo plus a disk at $0.25/GB-mo — [temps.sh Render pricing 2026 (secondary)](https://temps.sh/blog/render-pricing-2026)
- Secondary claim: the idle window was cut from 30 to 15 min on 2025-09-01 — [search summary of secondary blogs](https://livemy.app/blog/render-pricing) (unverified)

**Railway: trial, then $1/mo, too small**
- Trial is 30 days or $5 (whichever first), then the Free plan with $1 credit/month that does not roll over. Free plan: 1 replica, 0.5 GB RAM, 1 vCPU — [Railway docs: Free Trial](https://docs.railway.com/pricing/free-trial); [Railway plans](https://docs.railway.com/pricing/plans)
- No card for the trial, with verification via GitHub. "$1/month keeps only about 100 MB of RAM running." Hobby is $5/mo — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)
- Sleeping is opt-in, and Railway supports volumes — [SnapDeploy](https://snapdeploy.dev/state-of-free-hosting)

**Fly.io: no free tier for new accounts**
- Free allowances ended for new orgs on 7 Oct 2024. A new account gets a 7-day trial with 2 machine-hours, up to 10 machines and 20 GB of volumes. Trial machines stop after 5 min. A card is needed to continue. shared-cpu-1x with 256 MB costs about $2/mo full-time, going to $2.19 from 1 Oct 2026 — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/); [SnapDeploy](https://snapdeploy.dev/state-of-free-hosting); Fly's own pointer is [docs.fly.io/about/free-trial](https://docs.fly.io/about/free-trial) (not fetched)

**Zeabur: sleeps**
- Free plan needs no card. "Services automatically sleep after a period of inactivity" and wake on the next request. No SLA, no automated DB backups. Resources, volumes and regions are not stated on the page — [Zeabur Free Plan docs](https://zeabur.com/docs/en-US/pricing/free-plan)

**Hugging Face Spaces: Docker now needs a paid plan**
- Official docs: "Static Spaces are free for everyone. Gradio and Docker Spaces run on compute and require a paid plan to create: PRO for personal accounts…" CPU Basic is 2 vCPU, 16 GB RAM, free per hour, with 50 GB of **not persistent** disk. On free hardware a Space will "go to sleep… if unused" — [HF Spaces overview](https://huggingface.co/docs/hub/en/spaces-overview)
- Free hardware sleeps after 48 h idle. A secondary source dates the paid-plan rule for Docker to July 2026 — [SnapDeploy](https://snapdeploy.dev/state-of-free-hosting). Outbound traffic is allowed only on ports 80, 443 and 8080, which is fine for Web Push over 443 — [HF Spaces overview](https://huggingface.co/docs/hub/en/spaces-overview)

**Back4app Containers: unclear sleep, hours cap**
- Official page: Dockerized apps run free with "no credit card required", and limits reset monthly — [Back4app Containers](https://back4app.com/containers)
- An undated gist comment gives 256 MB RAM, 100 GB transfer and 600 active hours/mo. SnapDeploy says 256 MB / 0.25 CPU, and that the free container "lives 60 min per deploy" per template notes — [bmaupin gist](https://gist.github.com/bmaupin/d2d243218863320b01b0c1e1ca0cf5f3); [SnapDeploy](https://snapdeploy.dev/state-of-free-hosting). Both are unverified. 256 MB is below PreBot's needs either way.

**Choreo (WSO2, being renamed WSO2 Developer Platform)**
- An aggregator says the free tier allows up to 5 components and $100 credits/month — [agentdeals.dev](https://agentdeals.dev/vendor/choreo) (unverified)
- A component maps to one Kubernetes pod — [Choreo pricing](https://wso2.com/br/choreo/pricing)
- Choreo has a scale-to-zero feature (changelog, Mar 2024). Whether free components sleep was **not confirmed** — [Choreo changelog](https://wso2.com/choreo/changelog/scale-to-zero)

**Clever Cloud / Alwaysdata**
- Clever Cloud supports Docker, but its free "DEV" plans are only for some databases. Its blog argues against free tiers, and paid apps start around €5.60/mo — [Clever Cloud FAQ](https://www.clever.cloud/developers/doc/find-help/faq/); [Clever Cloud blog](https://www.clever-cloud.com/?p=2743)
- Alwaysdata free plan: about 256 MB RAM, 0.25 CPU, 1 GB SSD, for personal/non-profit use. The official comparison table shows Docker ❌, but which column that applies to is not confirmed — [Alwaysdata help: choose a plan](https://help.alwaysdata.com/en/accounts/billing/choose-its-paas); [whtop listing](https://www.whtop.com/plans/alwaysdata.com/117249)

**Heroku**
- No free tier since 28 Nov 2022. Eco is $5/mo, 512 MB, and sleeps after 30 min idle — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)

**Google Cloud Run**
- Always-free tier: 2M requests, 180k vCPU-s and 360k GiB-s/month (request-based billing). A card/billing account is required, and budget alerts do not cap spend — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)
- Min instances (needed for always-on) are billed. Under instance-based billing the full rate applies to the whole lifecycle. Third-party estimates for one warm instance are about $6.65–10/mo, before or after free-tier credits depending on the source — [Cloud Run min-instances docs](https://docs.cloud.google.com/run/docs/configuring/min-instance); [Cloud Run pricing](https://cloud.google.com/run/pricing); [oneuptime blog 2026 (secondary)](https://oneuptime.com/blog/post/2026-02-17-how-to-configure-minimum-instances-on-cloud-run-to-eliminate-cold-starts-for-production-services/markdown)

**Azure Container Apps**
- Monthly free grant per subscription: 180,000 vCPU-s, 360,000 GiB-s and 2M requests. With min replicas ≥1, idle replicas are billed at a reduced vCPU rate ($0.000003/vCPU-s vs $0.000024 active), but memory is billed at the active rate — [Azure Container Apps pricing](https://azure.microsoft.com/pricing/details/container-apps/); [Microsoft Tech Community blog](https://techcommunity.microsoft.com/blog/appsonazureblog/-/4419197)
- A card, a phone number and a $1 hold are required — [SnapDeploy](https://snapdeploy.dev/state-of-free-hosting)

**Azure App Service F1**
- 60 CPU-minutes/day and 1 GB RAM. Not supported for production. A credit or non-prepaid debit card plus a phone number are required — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)
- Free/Shared plans also have a 5-minute CPU quota. When either quota is exceeded, the app is stopped and returns HTTP 403 until reset (the daily quota resets at midnight UTC) — [Azure App Service quotas docs](https://docs.azure.cn/en-us/app-service/web-sites-monitor)
- "Always On" is disabled for F1 — [Microsoft WordPress free plan announcement](https://techcommunity.microsoft.com/t5/apps-on-azure-blog/announcing-public-preview-of-free-hosting-plan-for-WordPress-on/ba-p/3918254)
- WebSocket support exists on the free tier, with no documented connection cap found — [Microsoft blog: 10 years of free tier](https://techcommunity.microsoft.com/t5/apps-on-azure-blog/celebrating-10-years-of-azure-app-service-s-free-tier/ba-p/3621148)

**AWS**
- New accounts get the Free plan: $100 credit plus up to $100 more, and the plan closes after 6 months or when credits run out. A card is required — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/); [SnapDeploy](https://snapdeploy.dev/state-of-free-hosting) (dates the change to 15 Jul 2025)

**Cloudflare Containers / Workers**
- Containers have no free tier ("N/A") and require Workers Paid at $5/mo. That plan includes 25 GiB-h memory, 375 vCPU-min and 200 GB-h disk — [Cloudflare Containers pricing](https://developers.cloudflare.com/containers/pricing/)
- Workers Free: 100k requests/day, 10 ms CPU per request — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)

**Deno Deploy, Vercel, Netlify, DigitalOcean**
- Deno Deploy runs Deno, not arbitrary Docker, and apps scale to zero after about 20–30 s idle — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)
- Vercel Hobby is non-commercial, function-invocation based. Netlify is credit-based functions. DigitalOcean App Platform free covers static sites only, and any process starts at $5/mo — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)

**Out of PaaS scope, for the report writer's awareness**
- Oracle Cloud Always Free (VM, not PaaS): 2 OCPU and 12 GB Arm, reportedly halved from 4/24 on 15 Jun 2026 (secondary). Idle instances may be reclaimed after 7 days below 20% use. Card and phone required — [SnapDeploy](https://snapdeploy.dev/state-of-free-hosting)

### Inferences
- **Viable at $0 and always-on:** Northflank Sandbox is the only candidate. Whether a ~400 MB .NET container fits the "Limited" free compute is unconfirmed. Plausible options: run the web app as one free service, and use the free database slot (Postgres) or a volume (cost unclear) for persistence. A Frankfurt or Netherlands region is the closest to Turkey.
- **Viable with a sleep workaround but not reliable:** Render, Koyeb and Zeabur. Even if pinged awake, Render and Koyeb free give no persistent disk, so SQLite must move to an external DB. Koyeb's 0.1 vCPU is likely to make Blazor Server/.NET startup and SignalR sluggish.
- **"Free" only with a card and a small or ambiguous bill:** Azure Container Apps with min replicas = 1. By the rough calculation from the grant numbers, 0.25 vCPU / 0.5 GiB costs about $1–2/mo for vCPU, plus memory beyond 360k GiB-s (about 1.3M GiB-s/month needed). So it is **not $0**. Cloud Run with min-instances is similar at about $7–10/mo. Both need a card, which conflicts with the owner's preference.
- **Not viable:**
  - Azure F1: 60 CPU-min/day is exhausted by a 10 s background worker plus SignalR, and there is no Always On.
  - Hugging Face: Docker needs PRO, and the disk is not persistent.
  - Fly.io: trial only.
  - Heroku: paid.
  - Railway Free: $1 of credit is about 100 MB of RAM.
  - Back4app: 256 MB and an hours cap.
  - Alwaysdata: no Docker, 256 MB.
  - Clever Cloud: no free app tier.
  - Cloudflare Containers: paid and scale-to-zero oriented.
  - Vercel, Netlify and Deno Deploy: serverless functions or a JS/TS runtime, so they cannot run a long-lived .NET process, persistent SignalR WebSockets, or a 10 s background loop.

### Gaps
- **Not researched within the tool budget; no 2026 source found:**
  - Sevalla: aggregator mentions only, no verifiable free-tier numbers.
  - Leapcell: aggregator figures of 100k invocations, $0 Hobby and $5.9 Plus. Docker support and always-on not confirmed.
  - Wasmer Edge, Coolify Cloud, Dokploy Cloud, Replit Deployments, Glitch.
  - Heroku student credits, AWS App Runner.
- **Unverified prior knowledge, not confirmed in this session:**
  - Glitch ended project hosting in July 2025.
  - Coolify Cloud and Dokploy Cloud are paid control planes where you bring your own server, not free hosting.
  - Replit Deployments for always-on (Reserved VM) are paid.
  - The Heroku credit offer via the GitHub Student Pack.
  - App Runner's availability to new customers.
- Northflank free-tier per-service CPU/RAM and free volume allowance are undocumented on the pricing page. The card question has conflicting sources.
- The Koyeb $29 hold and the Feb 2026 removal of the card-free plan come only from a competitor's page.
- Choreo free-tier sleep behavior and current limits are unconfirmed.
- Azure Container Apps memory idle rate was not retrieved, so the always-on monthly cost estimate is incomplete.

## Q2. Can keep-alive pings prevent sleep, and is that against the ToS?

### Takeaway
External pings (UptimeRobot, cron-job.org, GitHub Actions) can keep Render, Koyeb and Zeabur services awake, since they wake on HTTP. No source found says this explicitly violates Render's or Koyeb's terms, but pings do not fix ephemeral disks, and providers can restart free services at any time.

### Cited Findings
- Render counts inbound HTTP requests and WebSocket messages on existing connections as activity, so a service with live SignalR traffic or periodic pings won't spin down. The filesystem is still ephemeral — [Render docs: Deploy for Free](https://render.com/docs/free)
- Pinging Render is a common practice, but Render "reserves the right to restart a free web service at any time". No source found says pings violate Render's terms — [odown blog on keeping Render awake](https://odown.com/blog/how-to-keep-a-render-free-service-from-sleeping/)
- Render's 750 free hours/workspace/month cover one service running 24/7 (about 720–744 h) — [Render docs](https://render.com/docs/free); [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)
- Koyeb free scales to zero after 1 h idle, and the setting can't be disabled. Third-party guides describe pinging to stay awake — [Koyeb scale-to-zero docs](https://www.koyeb.com/docs/run-and-scale/scale-to-zero); [cronic.pages.dev guide](https://cronic.pages.dev/blog/keeping-koyeb-free-tier-awake/)
- UptimeRobot's free plan was reportedly restricted to non-commercial use in late 2024. A 2026 post claims commercial use is allowed again. This is unverified — [UptimeRobot terms](https://uptimerobot.com/terms); [dev.to post](https://dev.to/velprove/uptimerobot-commercial-use-free-alternatives-for-business-sites-in-2026-5d75)

### Inferences
- On Render, one pinged free service can stay up all month within 750 h, but it uses 512 MB RAM and a fraction of a CPU, and SQLite must go to an external DB. A Blazor Server cold start or random restart drops all SignalR circuits, so users see "reconnecting".
- A single always-on PreBot instance would consume Render's whole free hour pool. Any second free service in the same workspace would exhaust it.
- Pinging is a gray area that may be tightened at any time. It is not a robust basis for push notifications that must fire every 10 s.

### Gaps
- No explicit ToS clause found on keep-alive pinging for Render, Koyeb, Zeabur or HF. Each provider's acceptable-use policy was not fetched.

## Q3. Free managed databases to replace local SQLite when there is no persistent disk

### Takeaway
Turso (libSQL, SQLite-compatible) is the closest drop-in for SQLite and has a generous free tier. Neon offers free serverless Postgres that scales to zero after 5 min, which is fine since it wakes on connect. Supabase free pauses after about 7 days of inactivity, which won't trigger with a 10 s worker. Render's free Postgres expires in 30 days, so avoid it. Northflank includes 1 free database slot in the Sandbox.

### Cited Findings
- **Turso:** free plan reportedly 100 databases, 5 GB storage, 500M rows read and 10M rows written per month. It was reduced from 500 DBs / 9 GB, and Developer costs $4.99/mo. These figures were not confirmed on turso.tech — [agentdeals.dev](https://agentdeals.dev/vendor/turso); [toolpick.dev](https://www.toolpick.dev/pricing/turso); [Turso pricing](https://turso.tech/pricing.md)
- **Neon:** Free plan has 100 projects, 100 CU-hours per project, about 0.5–1 GB storage per project (sources differ) and a 2 CU cap. Scale-to-zero after 5 min is fixed on Free, and limits are a hard cutoff — [Neon free plan FAQ](https://neon.com/faqs/free-plan-limits-and-quotas); [jetadmin Neon pricing, Sep 2026 (secondary)](https://www.jetadmin.io/blog/neon-pricing/)
- **Supabase:** free projects pause after low activity over a week, while paid plans never pause. Free tier allows 2 active projects — [Supabase KB](https://supabase.com/kb/faqs/platform/free-project-pausing); [automationatlas](https://automationatlas.io/answers/supabase-free-tier-limits-2026/)
- **Koyeb:** a free Serverless Postgres line is listed (0.25 vCPU, 1 GB RAM, 1 GB storage, "5h"), in Frankfurt among other regions — [Koyeb pricing](https://www.koyeb.com/pricing)
- **Render:** free Postgres is 1 GB and expires after 30 days, then 14 days' grace — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)

### Inferences
- **Turso** is the closest swap for an EF Core + SQLite app: libSQL clients and the HTTP protocol exist. EF Core support goes through community providers, so it is not a pure connection-string change.
- **Neon and Supabase** need a switch to Npgsql/EF Core Postgres, which is a well-supported path.
- **Neon CU-hours:** a 10 s background poll will likely keep Neon awake 24/7. About 730 h × 0.25 CU ≈ 183 CU-h, which exceeds 100 CU-h/month. Neon free may therefore run out unless polling hits the DB less often, or the minimum compute is 0.25 CU and the 100 CU-h figure is per project. This is an inference to verify.
- **Supabase** is not paused by a constantly active app, so it suits a 10 s worker.
- **Latency:** a host in Frankfurt plus a DB in the same EU region minimizes it.

### Gaps
- Aiven free PostgreSQL: no 2026 source found (prior knowledge suggests a free tier exists, unverified).
- Turso's official free-tier numbers and EF Core provider maturity were not verified.
- Neon's exact CU-hour math for an always-connected app was not verified.

## Q4. Which ones work for an owner and users in Turkey (signup and payment)?

### Takeaway
No source found any Turkey-specific block for these platforms. The practical issue is card requirements: Northflank (official: no card), Render, Zeabur, Back4app and Railway's trial avoid a card. Koyeb (reported $29 hold), Fly.io, Azure, Google Cloud, AWS, Oracle and Cloudflare Containers need one. EU regions (Frankfurt or Netherlands) are available on Northflank and Koyeb, and are the lowest-latency choice for users in Turkey.

### Cited Findings
- **No card needed:**
  - Northflank Sandbox, "No card needed" — [Northflank pricing](https://northflank.com/pricing)
  - Render, Railway trial, Zeabur, Back4app — [SnapDeploy](https://snapdeploy.dev/state-of-free-hosting); [Zeabur docs](https://zeabur.com/docs/en-US/pricing/free-plan); [Back4app](https://back4app.com/containers)
- **Card needed:**
  - Azure needs a credit or non-prepaid debit card plus phone. Google Cloud needs a billing account. AWS needs a card. Fly needs a card after the trial — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/)
  - Koyeb reportedly holds $29 — [SnapDeploy (competitor; unverified)](https://snapdeploy.dev/state-of-free-hosting)
- **EU regions:** Koyeb Free Instance runs in Frankfurt or Washington — [Koyeb Instances docs](https://www.koyeb.com/docs/reference/instances). Northflank has Europe-West regions — [Northflank region page](https://northflank.com/cloud/northflank/regions/europe-west)

### Inferences
- For a Turkey-based owner with no card, Northflank Sandbox in an EU region is the clear first choice. Render with a ping and an external DB is the fallback.
- Turkish bank cards generally work for international USD card payments, but prepaid or virtual cards may be rejected by Azure, which explicitly requires a non-prepaid card. This is unverified for each provider.

### Gaps
- No source found on Turkey-specific restrictions (sanctions, KYC, Stripe card acceptance, TRY billing) for any of these platforms.
- No real Reddit/HN reports from Turkish users were found within the budget.

## Q5. Honest suitability verdict for PreBot

### Takeaway
**First choice:** Northflank Sandbox, in an EU region, with the free DB slot or a volume. It is the only free, no-card, documented always-on container host.
**Fallback:** Render free with a keep-alive ping plus an external DB (Turso, Supabase or Neon). It can be kept awake, but expect restarts and SignalR disconnects.
**Paid but cheap:** Fly.io (~$2/mo for 256 MB, too small for .NET; 512 MB+ costs more), Railway Hobby ($5), Render Starter ($7 + disk), Azure Container Apps (a few $ with a card).
**Not suitable:** Azure F1, HF Spaces, Heroku, Vercel/Netlify/Deno, Cloudflare Containers, Back4app, Alwaysdata, Clever Cloud.

### Cited Findings
- Northflank: "Always on, no sleeping", "No card needed", $20/mo of resource credit, 2 services and 1 DB — [Northflank pricing](https://northflank.com/pricing)
- Render: 15-min spin-down, ephemeral filesystem, no disk on free — [Render docs](https://render.com/docs/free)
- Koyeb: 1-h scale-to-zero that can't be disabled, 0.1 vCPU, no volumes — [Koyeb Instances docs](https://www.koyeb.com/docs/reference/instances)
- HF: Docker Spaces require a paid plan, the disk is not persistent, and Spaces sleep — [HF Spaces overview](https://huggingface.co/docs/hub/en/spaces-overview)
- Azure F1: 60 CPU-min/day, 403 errors when over quota, no Always On — [flaviocopes.com](https://flaviocopes.com/hosting-free-tiers/); [Azure quotas docs](https://docs.azure.cn/en-us/app-service/web-sites-monitor)

### Inferences
- **Northflank risks:** "Limited" free compute (exact vCPU/RAM unknown; must confirm 512 MB+ is allowed), possible card or identity check, and possible volume cost. Keeping SQLite on a volume vs. using the free addon DB should be tested.
- **Koyeb free** is a poor fit even with pings: 0.1 vCPU is very low for .NET JIT startup and Blazor Server rendering.
- **SQLite on platforms without disks:** the app must switch to Postgres or Turso. That is a code change: EF Core provider swap plus migrations.
- **If no free PaaS proves workable,** the realistic $0 alternatives are outside this PaaS scope: Oracle Always Free VM (card required, reclamation risk) or self-hosting with a tunnel (already explored by the project).

### Gaps
- No hands-on 2026 report was found of a .NET Blazor Server app running on the Northflank Sandbox free tier.
- WebSocket support was not individually confirmed in docs for Northflank, Zeabur or Back4app. It is generally standard on Kubernetes-based PaaS, but unverified here.
