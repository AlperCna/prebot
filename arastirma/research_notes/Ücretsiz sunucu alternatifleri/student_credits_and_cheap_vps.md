# Student / open-source hosting credits and cheapest reliable VPS (as of Nov 2026) for PreBot

Context: .NET 10 Blazor Server in Docker + Caddy, always-on Linux VM, ~1 GB RAM, ~10 GB disk, public IPv4, ports 80/443, users in Turkey, Europe region preferred. Owner possibly a university student; Oracle Always Free signup failed. Research date: 2026-11-09. Mostly third-party sources; official pages fetched where possible (Azure, Hetzner). Flagged where unverified.

## (a) Student programs in 2026 (GitHub Student Developer Pack, Azure for Students, AWS, GCP, free domains)

### Takeaway
After DigitalOcean left the GitHub Student Pack (credit no longer redeemable after 31 Jul 2026), **Azure for Students is the only real no-card VM option**: $100/12 months, no credit card, renewable yearly while a full-time student, plus 12 months of 750 free hours each of B1s / B2pts v2 / B2ats v2 VMs — enough to run PreBot's single small VM for a full year at ~$0 and spend the $100 only on extras (IPv4, disk, egress). Heroku's student credit requires a card; AWS/GCP have no comparable self-serve no-card VM credit.

### Cited Findings
**Azure for Students (official)**
- $100 Azure credit, "no credit card required", valid one year; "You can renew each year that you're an active student to get an additional $100." — [Microsoft Learn: What is Azure for Students? (updated 2026-08-03)](https://learn.microsoft.com/en-us/azure/education-hub/about-azure-for-students)
- Eligibility: must affirm age 18+, full-time student at an accredited degree-granting 2- or 4-year institution, and verify via institution email address; not for MOOCs/for-profit trainings; one subscription per person — [Microsoft Learn](https://learn.microsoft.com/en-us/azure/education-hub/about-azure-for-students)
- Official offer page: free for 12 months (new customers) "750 hours each of B1s, B2pts v2 (Arm-based), and B2ats v2 (AMD-based) burstable VMs" (Linux or Windows); also 750 h PostgreSQL/MySQL Flexible B1MS, 1 Standard Container Registry (100 GB); always-free: App Service up to 10 apps (1 GB, 1 h/day), Container Apps 180,000 vCPU-s / 360,000 GiB-s / 2M requests per month, Functions 1M requests. Credit can't be used for Marketplace offers. When credit ends, Microsoft asks whether to move to pay-as-you-go; if not, "subscription and products are disabled". No country restriction mentioned. — [azure.microsoft.com/free/students](https://azure.microsoft.com/en-us/free/students)
- The page did not explicitly state "per month" for the 750 hours in the fetched text; Microsoft's general free-account VM page says "750 hours ... free every month for 12 months" for B2ts/B2pts/B2ats v2 — [Azure Windows VM pricing](https://azure.microsoft.com/en-us/pricing/details/virtual-machines/windows/) (via search snippet)
- Renewal: opens up to 30 days before the 12-month anniversary, requires re-verifying student status in Education Hub; community reports of renewal refusal (one June 2026 case told "not eligible to renew") — [MS Q&A: not eligible to renew](https://learn.microsoft.com/en-in/answers/questions/5909370/you-are-not-eligible-to-renew-azure-for-students); [MS Q&A: extend student credits](https://learn.microsoft.com/en-us/answers/questions/5831889/need-to-extend-student-credits) (community answers, not official policy)
- "Azure for Students Starter" requires no card but gives only free-tier services, no $100 credit (search summary of MS Q&A) — [MS Q&A](https://learn.microsoft.com/en-us/answers/questions/5833148/i-cant-update-my-subscription-for-students)
- Pay-as-you-go prices (third-party trackers, not official; conflicting): B1s ~$0.012/h West Europe, ~$0.011/h Sweden Central (≈$8–9/month at 730 h); B2ats v2 ~$0.011/h West Europe (another tracker: ~$14.60/mo), Sweden Central ~$13.80/mo — [azurespeed B1s](https://www.azurespeed.com/AzureVmPricing/Standard_B1s); [cloudprice compare](https://cloudprice.net/compare/Standard_B1s-vs-Standard_B2ats_v2); [CloudOptimo B2ats v2](https://costcalc.cloudoptimo.com/azure-pricing-calculator/vm/Standard-B2ats-v2)

**GitHub Student Developer Pack (2026 state)**
- DigitalOcean left the Pack: $200 student credit offer no longer available after 31 Jul 2026; already-redeemed credits usable until 1 Aug 2026 — [GitHub community discussion #201240](https://github.com/orgs/community/discussions/201240). One third-party site claims a different cutoff (19 May–9 Jul 2026) and another claims all credits were removed 1 Aug 2026 — [aistudentdiscount](https://aistudentdiscount.com/digitalocean-github-student-developer-pack-credits/) (conflicting, lower reliability)
- Hosting/domain offers checked 6 Sep 2026 — [santoshpaudel.me: GitHub Student Pack Free vs Trial](https://www.santoshpaudel.me/blog/github-student-pack-free-vs-trial):
  - Azure for Students: $100 + free tiers, 12 months, no card.
  - Heroku for GitHub Students: $13/month credit for 24 months ($312 total); **requires valid credit/debit card**; after it ends card is charged list price; past participants can't reapply.
  - Appwrite Education: 2 Pro-equivalent projects (~$40/mo value) until graduation (BaaS, not a VM).
  - MongoDB Atlas: $50 credits; card or PayPal required.
  - Namecheap: free .ME domain for 1 year + SSL (1 year) bundle; renews ~$15/yr.
  - Name.com: free domain on 25+ TLDs (incl. .dev, .app) — term unverified.
  - .TECH (get.tech): one .TECH domain for 1 year — details unverified (403).
  - "The Pack has no VPS offer"; Vercel never in Pack.
- Another guide says Azure for Students is now the main remaining cloud credit in the Pack — [search summary of reconscribe / perkstack guides](https://reconscribe.com/free-credits/students/)
- Official partner list: [education.github.com/pack](https://education.github.com/pack) / [Partners FAQ repo](https://github.com/github-education-resources/Student-Developer-Pack-Current-Partners-FAQ) (not fetched directly — verify)

**AWS / Google Cloud education**
- AWS Educate: training/labs need no card; Starter Account with capped resources only when invited by an educator; credit amounts reported as $35 individual / up to $100 via member institution — affiliate aggregator, unverified — [AWS Builder Center article](https://builder.aws.com/content/3J0F6sCj6NWuOC3jqP6ZYMRyxrc/aws-educate-and-free-credit-programs-for-students-whats-actually-available-in-2026); [aistudentdiscount AWS Educate](https://aistudentdiscount.com/aws-educate-student-credits/)
- Google Cloud for Education credits go to institutions/courses (faculty grants), not self-serve; the $300 new-customer trial requires a billing method — [Berkeley Student Tech](https://studenttech.berkeley.edu/node/440); [reskilll blog 2026](https://blogs.reskilll.com/free-cloud-credits-hackathons-aws-gcp-azure-student-programs-2026/)

### Inferences
- **Best zero-cost path if the owner is a full-time university student**: Azure for Students, Linux B1s (1 vCPU / 1 GiB) or B2ats v2 / B2pts v2 (2 vCPU / 1 GiB — RAM figure from general knowledge, verify) in West Europe / North Europe / Sweden Central. 750 h ≥ 744 h in a 31-day month, so one VM can run 24/7 inside the free hours for 12 months. The $100 then only covers chargeable extras (public IPv4 static/standard IP roughly $3–4/month, managed disk beyond any free allowance, egress beyond the free allowance). Rough estimate: even if the free VM hours did NOT apply, $100 / ~$9 per month (B1s) ≈ 11 months; with free hours, $100 should comfortably last the full 12 months. (Prices for IP/disk/egress not verified in this research.)
- 1 GiB RAM is tight for Docker + .NET 10 Blazor Server + Caddy; add a 1–2 GB swap file. B2pts v2 is Arm64 — the Docker image would need an arm64 build (.NET supports linux-arm64).
- Turkish university e-mail (…edu.tr) should work for verification since no country exclusion appears; not confirmed specifically for Turkey.
- Set a budget alert / avoid any pay-as-you-go upgrade; when credit runs out the subscription is disabled (no surprise charges since no card).
- Heroku student credit ($13/mo) could run a Basic/Eco dyno but needs a card and Heroku dynos are not a VM (no Caddy needed, though; container deploy possible). Secondary option.
- Free domain: Namecheap .me (1 yr) or .tech / name.com via the Pack — useful for Caddy HTTPS instead of a raw IP.

### Gaps
- Official Azure prices for public IPv4, P6 managed disk and egress free allowances under the Students offer were not fetched; official B1s/B2ats v2 hourly price in West Europe/Sweden Central not confirmed (only third-party trackers, which conflict).
- Whether education.github.com/pack currently lists additional hosting partners added after Sep 2026 — not verified directly.
- Azure for Students acceptance of Turkish institutions not explicitly confirmed.
- AWS Educate credit amounts unverified on an official AWS page.

## (b) Open-source / non-profit hosting programs

### Takeaway
No realistic free-VM program exists for a tiny new OSS project in late 2026: DigitalOcean's Open Source Credits program was reported sunset in Oct 2026, Fly.io has no free tier for new accounts and no published OSS credit program, and Render's program is for startups. Microsoft for Startups/Nonprofits and Cloudflare Project Galileo don't fit (PreBot is not a company/NGO; Galileo is DDoS protection, not hosting).

### Cited Findings
- DigitalOcean is sunsetting Open Source Credits: no new applications, renewals or extensions; existing grants honored; inbox no longer monitored (reported Oct 2026; no official DO blog post found) — [It's FOSS](https://itsfoss.com/news/digitalocean-open-source-credits-end/); [Linux Magazine](https://www.linux-magazine.com/Online/News/DigitalOcean-Ends-Open-Source-Credits-Program); [wisevoter (2026-10-12)](https://wisevoter.com/world/us/2026/10/12/digitalocean-ends-open-source-credits)
- Fly.io: no free tier for new accounts since Oct 2024; a community thread asking about OSS credits has no clear answer — [flaviocopes free tiers (checked Sep 2026)](https://flaviocopes.com/hosting-free-tiers/); [Fly.io community](https://community.fly.io/t/credits-for-open-source/13595)
- Render's credits program targets startups, tiers valid one year from approval; no OSS program found — [Better Stack: DO Startups vs Render for Startups](https://betterstack.com/community/comparisons/digitalocean-vs-render-startups/)

### Inferences
- Microsoft for Startups (Founders Hub) requires a business/startup; Microsoft nonprofit grants require a registered non-profit — not applicable to an individual's hobby project (general knowledge, not re-verified this session).
- Cloudflare Project Galileo is for at-risk civil-society orgs and provides security, not compute; Cloudflare Tunnel (free) is already used by the owner for the home-hosted trial.
- GitHub doesn't sponsor runtime hosting for OSS; GitHub Sponsors could fund a cheap VPS once the project has users.

### Gaps
- Fly.io / Render / Koyeb / Railway OSS-specific programs: no official pages found confirming or denying in 2026.
- DigitalOcean sunset not confirmed on an official DO page.

## (c) Cheapest reliable paid VPS in Europe (Nov 2026)

### Takeaway
2026 saw big price rises (Hetzner +30–40% on 15 Jun 2026 and its cheapest CX/CAX plans often **sold out** in Oct–Nov 2026). The cheapest trustworthy options now are **OVHcloud VPS-1** (~€4.49/mo, €3.81/mo on 12-month prepay, 2 vCPU/4 GB/40 GB NVMe, IPv4, unlimited EU traffic, daily backup), **Hetzner CX23/CAX11** (€5.99/€6.49 incl. IPv4 when in stock), **netcup VPS Lite 1** (€4.92 net, 6-month minimum), **Contabo** (€4.40–5.50 but weaker reputation), and IONOS promo pricing ($5 then $14). True "1 GB for $1–2" exists only via promos (IONOS, RackNerd Black Friday ~$10–11/year, US-located mostly).

### Cited Findings
**Hetzner (Germany/Finland)**
- Official cost-optimized page (fetched 2026-11-09): CX23 2 vCPU/4 GB/40 GB NVMe **€5.99/mo incl. IPv4** (excl. VAT); CAX11 (Arm) 2/4/40 €6.49; CX33 4/8/80 €8.99; CAX21 €10.99. **All plans shown "currently unavailable. Please check back later"** (older hardware, limited stock). Locations NBG1/FSN1/HEL1; 20 TB traffic; hourly billing to monthly cap — [Hetzner Cost-Optimized](https://www.hetzner.com/cloud/cost-optimized)
- Price change 15 Jun 2026: CX23 €3.99→€5.49, CAX11 €4.49→€5.99 (excl. €0.50 IPv4); CPX22 €7.99→€19.49 (+144%), CCX13 €15.99→€42.99. As of 21 Oct 2026 all EU CX/CAX plans showed "not available" — [VPSDEX (2026-10-21)](https://vpsdex.com/en/activity/hetzner-cloud-pricing-2026/); [Northflank breakdown](https://northflank.com/blog/hetzner-cloud-server-price-increases)
- Oct 2026 snapshot: CPX22 2/4/80 €19.99 incl. IPv4 (was €6.49 at start of 2026) — [HostingSift on DEV (2026-10-23)](https://dev.to/hostingsift/is-hetzner-still-the-cheapest-vps-we-priced-24-alternatives-october-2026-271g)

**Other European providers (HostingSift snapshot 23 Oct 2026, EUR excl. VAT unless noted)** — [DEV/HostingSift](https://dev.to/hostingsift/is-hetzner-still-the-cheapest-vps-we-priced-24-alternatives-october-2026-271g)
- OVHcloud VPS-1: 2 vCPU/4 GB/40 GB NVMe, €4.49/mo (€3.81/mo on 12-month prepay; renewal same), daily backup, 500 Mbps unlimited EU traffic.
- OVHcloud VPS-2: 4/8/75 NVMe €8.49 (€7.21 prepaid).
- Contabo Cloud VPS 4: 4/8/100 GB SSD €5.50 (€4.40 on 24-month prepay), 200 Mbit/s; Contabo Cloud VPS 10 last seen Aug 2026 at €3.60/mo (12-month) — unreliable record.
- netcup VPS Lite 1 G12.5s: 2/4/80 GB SSD €4.92 net (€5.86 incl. 19% VAT), **6-month minimum**; VPS 500 G12.5: 2/4/64 GB €7.98/mo month-to-month. Lite plans: 750 Mbps port throttled to 100 Mbps if 24-h average traffic exceeds threshold.
- IONOS VPS M+: $5 for first 3 months on 12-month term, renews $14 (regular raised $11→$14 on 18 Sep 2026).
- Hostinger KVM 2: 2/8/100 NVMe $8.79/mo on 24-month term, renews $14.99.
- IONOS smallest VPS (2025): 1 vCPU/1 GB/10 GB SSD at $2/month — TechRadar 2025, **may be outdated** — [TechRadar](https://techradar.com/news/cheap-vps-hosting-deals); 2026 guide: VPS S+ $2/mo first 3 months then $5/mo for rest of 1-year term — [virtarix (via search snippet)](https://virtarix.com/blog/vps-guides/cheap-cloud-vps/)
- OVH VPS-1 "starts at $4.54/month", region/tax dependent — [essamamdani 2026](https://essamamdani.com/blog/hetzner-alternatives-2026)

**LowEnd deals (RackNerd)**
- RackNerd Black Friday 2025 (LowEndBox): KVM VPS from $10.60/year incl. 1 GB plans — [LowEndBox](https://lowendbox.com/?p=51180); New Year 2026 promo from $11.29/year — [prjs-astro blog](https://prjs-astro.pages.dev/blog/2026-03-03_racknerd_1). A $10.18/year 768 MB/1 vCPU/15 GB plan is widely cited on low-quality blogs — [bestusavps](https://bestusavps.pages.dev/deals/racknerd-deals) (unreliable; contains internally inconsistent dates). Black Friday 2026 deals not yet announced as of 9 Nov 2026 (expected mid-Nov). RackNerd locations are mainly US; EU locations exist in some promos (unverified).

**Turkish providers (TL)**
- Comparison (prices checked 9 Sep 2026): Natro XCloud VPS Eco 2 vCPU/2 GB/40 GB SSD ₺389/mo (20% KDV incl.), Istanbul; Cenuta NVMe VDS-1 2 vCPU/4 GB/50 GB NVMe ₺249 (KDV incl.), Bursa/Istanbul; Turhost Bulut Sunucu SSD-1 2 dedicated vCPU/4 GB/60 GB NVMe ₺449 (KDV incl.), Istanbul (Radore & TGN); for comparison OVH VPS Starter ₺210, Hetzner CAX11 ₺212 (excl. KDV). Ping TR→Germany ~40–58 ms vs Istanbul 3–12 ms — [verinokta VPS fiyatları](https://verinokta.com/karsilastirma/vps-fiyatlari/) (dates on page conflict)
- Natro XCloud Mini 1 vCPU/1 GB/20 GB SSD ₺241.99/mo (25 Sep 2026 comparison); Güzel Hosting TR-VPS-2 1 CPU/1 GB/30 GB SSD ₺447.90/mo; Cenuta VPS I 1/1 GB/25 GB NVMe ₺139.90/mo (page possibly ~4 years old); UcuzaVDS Cheap Virtual Server 1 (1 core/1 GB/40 GB NVMe, Istanbul) ₺89.99/mo (undated) — [search summary: cenuta.com](https://www.cenuta.com/vps-server); [ucuzavds.com.tr](https://www.ucuzavds.com.tr/en/); [karekod.org](https://www.karekod.org/blog/hosting-fiyatlari-2025-vps-vds-sunucu-fiyati/)

**Taxes / payment from Turkey**
- Foreign provider without Turkish establishment: for non-KDV-registered individuals, the provider is responsible for declaring Turkish KDV on electronic services (özelge summary, not authoritative); for businesses, buyer self-declares via KDV-2 — [muhasebetr](https://www.muhasebetr.com/yazarlarimiz/aylinkorkmazokyay/004/); [ffkpartnerhukuk 2026 guide](https://www.ffkpartnerhukuk.com.tr/dijital-hizmet-sunan-yabancilarin-vergi-yukumlulukleri-2026-guncel-rehber/)
- Turkish providers' TL prices commonly listed with 20% KDV included (Natro, Cenuta, Turhost) — [verinokta](https://verinokta.com/karsilastirma/vps-fiyatlari/)

### Inferences
- Ranked cheapest trustworthy options for PreBot (all far exceed 1 GB/10 GB needs):
  1. **OVHcloud VPS-1** — ~€3.81–4.49/mo, EU (France/Germany/Poland), IPv4 + backups included, good reputation, generally in stock. Best value now.
  2. **Hetzner CX23 / CAX11** — €5.99 / €6.49 incl. IPv4, excellent reputation, ~40–55 ms from Turkey, but frequently sold out; check Console daily or try other EU locations.
  3. **netcup VPS Lite 1** — €4.92 net, solid German provider, but 6-month minimum (~€30 upfront) and throttling rule.
  4. **IONOS VPS S+/XS** — $2 promo then ~$5/mo; good for a 1-year cheap run, watch renewal pricing.
  5. **Contabo** — cheap on prepay but known for weaker performance/support (reputation caveat; no source fetched this session).
  6. **Turkish VPS** (Natro ~₺242–389, Cenuta ₺139–249, UcuzaVDS ₺90) — lowest latency, TL billing, local payment (havale/EFT, Turkish cards) likely, but reliability varies; prefer established brands (Natro, Turhost).
  7. **RackNerd BF 2026** — ~$10–11/year, mostly US locations (higher latency ~150+ ms to Turkey); fine for a hobby app if latency acceptable. Wait for mid/late-Nov announcements on LowEndBox.
- Hetzner bills non-EU private customers without German VAT in general (general knowledge, unverified here); Turkish KDV may be added by registered foreign providers — budget ~20% on top of excl.-VAT prices, plus bank FX fees.
- Hetzner, OVH, netcup and IONOS generally accept Visa/Mastercard and often PayPal; Turkish debit cards must be enabled for internet/foreign transactions (general knowledge — not verified in this session).

### Gaps
- No source confirmed which foreign providers accept Turkish cards/Papara/PayPal-from-Turkey in 2026 (PayPal does not operate in Turkey since 2016 — general knowledge). Hetzner's identity verification for Turkish accounts not researched.
- Scaleway Stardust, netcup piko/nano, Hostinger KVM 1 current 2026 prices not found.
- Exact Turkish KDV treatment for individuals buying foreign VPS in 2026 — only blog/özelge summaries, no GİB primary source.
- Payment methods on Turkish provider sites not fetched (likely card/havale; unverified). İHS, Radore direct retail prices not found.
- RackNerd Black Friday 2026 offers not yet published as of 2026-11-09.
