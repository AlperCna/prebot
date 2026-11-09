# Free-forever / long-free-trial VMs (IaaS) for PreBot, as alternatives to Oracle Cloud Always Free (researched 2026-11-09)

App requirements used for the suitability calls below: always-on .NET 10 Blazor Server in Docker plus Caddy, 300–600 MB RAM, a few hundred MB of persistent disk for SQLite, public IPv4 with ports 80/443, outbound HTTPS for Web Push, users in Turkey (Europe preferred). Owner: an individual in Turkey with a Visa card. Oracle signup already failed with the generic anti-fraud error.

Method note: official pages were used where they could be fetched. oracle.com, vultr.com and repost.aws returned HTTP 403 to the fetch tool, and Google's network-pricing page came back truncated. For those, the claims rely on search snippets or third-party pages and are flagged as such. Several "2026 free VPS" listicles that came up are affiliate/SEO sites (bestusavps.pages.dev, valebyte, cyfuture, itechguides, "Brand Industry", Pabau). They were treated as low-trust and are cited only where nothing better was found.

## Google Cloud: Always Free e2-micro and the $300 / 90-day trial

### Takeaway
GCP's e2-micro is the only big-cloud VM that is genuinely free forever, but it is US-only (Oregon, Iowa, South Carolina), has 1 GB RAM and 30 GB of standard (HDD) persistent disk, and only 1 GB/month of free egress, with China and Australia excluded. It can run PreBot only with swap and careful memory limits. Latency from Turkey will be transatlantic (inferred at roughly 120–170 ms, not measured). Small egress overage and possibly the external IPv4 can cost cents to a few dollars a month. Signup needs a card. Turkey uses card-only, TRY-billed self-serve accounts, and individual accounts there are subject to VAT. I found no evidence either way that Turkish individuals are excluded from the trial.

### Cited Findings
- Always Free Compute Engine: 1 "non-preemptible e2-micro VM instance" per month, only in us-west1 (Oregon), us-central1 (Iowa) and us-east1 (South Carolina). The limit is time-based (hours in the month, summed across those regions), not per instance. — [Google Cloud docs: Free cloud features](https://docs.cloud.google.com/free/docs/free-cloud-features)
- Disk: 30 GB-months of *standard* persistent disk. Egress: 1 GB/month outbound from North America to all destinations except China and Australia. GPUs/TPUs are always charged. — [Google Cloud docs: Free cloud features](https://docs.cloud.google.com/free/docs/free-cloud-features)
- $300 Welcome credit valid for 90 days. You "must provide a credit card or other payment method", and some countries also require bank-account verification. The authorization is a temporary hold. There is no automatic upgrade: if you don't upgrade manually, the billing account closes when the credit is spent or after 90 days, followed by a 30-day grace period before resources are deleted. Trial restrictions: no GPUs, no Marketplace, no quota increases, no Windows Server VMs. — [Google Cloud docs: Free cloud features](https://docs.cloud.google.com/free/docs/free-cloud-features)
- Trial eligibility depends on never having been a paying Google Cloud / Maps / Firebase customer and not having had the trial before. The page lists no excluded countries. — [Google Cloud docs: Free cloud features](https://docs.cloud.google.com/free/docs/free-cloud-features)
- Turkey: the self-serve Cloud Billing payment method is listed as credit/debit card in TRY. — [Google Cloud billing currency/payment methods table](https://docs.cloud.google.com/billing/docs/resources/currency?hl=bn) (via search snippet)
- Turkey: Google support says VAT is now charged on individual (non-business) accounts in Turkey, and individuals should migrate their billing account so VAT is applied. — [Google Cloud support answer 9051878](https://support.google.com/cloud/answer/9051878?hl=ES) (via search snippet)
- Old, unofficial: a 2018 Google Groups thread said business accounts are not eligible for the free trial, while individual accounts without a prior billing account are. — [Google Groups thread](https://groups.google.com/g/google-cloud-dev/c/zLxtdeCMTHM) (dated)
- One anecdote of a Turkish user hitting payment errors with Turkish bank cards when paying for Gemini API usage. This is a single report, not policy. — [search summary of discuss.google.dev / Google Groups results](https://discuss.google.dev/t/this-action-couldnt-be-completed-or-cbat-23/162780) (unverified which thread)
- Third-party sources put internet egress beyond the free 1 GB at about $0.12/GB. — [agentdeals.dev GCP Free Tier 2026](https://agentdeals.dev/gcp-free-tier-2026); [bestusavps Google Cloud free](https://bestusavps.pages.dev/deals/google-cloud-free) (third-party, lower trust)
- External IPv4 on a free e2-micro: the sources conflict.
  - Older Always Free docs, quoted on Server Fault, said in-use external IPs on non-preemptible instances were covered. — [Server Fault archive](https://kiwix.lema.org/serverfault.com_en_all_2023-11/questions/1012277/google-cloud-external-ip-charges-for-free-tier)
  - DoiT's article "No more free external IPs on Google Cloud" describes Google charging for all external IPv4. — [DoiT blog](https://www.doit.com/blog/no-more-free-external-ips-on-google-cloud-how-much-will-it-cost-you)
  - Reports say a static/reserved or stopped-VM IP is billed. — [discuss.google.dev](https://discuss.google.dev/t/how-to-find-what-resources-exceed-free-tier/182904)
  - The official VPC pricing page could not be read (truncated fetch).
- A 2025 article claiming e2-micro is free in 27 regions, including Europe, contradicts Google's own docs and should be disregarded. — [sivaro.in (contradicted by Google docs)](https://sivaro.in/articles/gcp-free-tier-limits-2025-what-still-works-and-what-doesnt/)

### Inferences
- RAM: e2-micro is 1 GB, with 2 shared vCPUs at a fractional baseline. That last part is from general knowledge and was not re-verified this session. PreBot at 300–600 MB, plus Caddy at about 20–50 MB, plus dockerd/containerd at about 50–100 MB, plus the OS at about 150–250 MB leaves almost no headroom. It is feasible with 1–2 GB of swap on the 30 GB disk, `DOTNET_GCHeapHardLimit` or a container `mem_limit`, and Server GC disabled (`DOTNET_gcServer=0`). Expect occasional stalls under memory pressure, and the HDD-class standard disk makes swapping slow.
- Egress: a few users on Blazor Server/SignalR plus Web Push will likely use a few GB/month. Above 1 GB, at about $0.12/GB, that is cents to under $1/month. So "free" is likely "near-free", not strictly $0.
- Latency: the nearest free region is us-east1 (South Carolina), which is suboptimal for Turkey but acceptable for a lobby/notification app. The $300 trial can run a VM in a European region (e.g., europe-west3 Frankfurt) for 90 days, but that is temporary only.
- Card: the card is mandatory, and a Visa debit/credit card with 3-D Secure and international online payments enabled is the standard requirement. I found no systematic reports of Turkish Visa cards being rejected by GCP.

### Gaps
- Current official rule on whether the in-use ephemeral external IPv4 of the free e2-micro is billed. The VPC pricing page was not readable, so check the billing report after the first days.
- Exact current egress rate from North America to EMEA (Premium vs Standard tier).
- Whether Google's trial or free tier currently accepts Turkish *individual* accounts without issues. No 2025–2026 Turkish forum reports were found.

## AWS Free Tier in 2026: the credit-based Free plan

### Takeaway
Accounts created on or after 15 July 2025 no longer get 12 months of free t2/t3.micro. They get a "Free plan" with $100 signup credit, plus up to $100 more for completing activities, which expires after 6 months or when the credit runs out, whichever comes first. Eligible EC2 types include t3.micro, t3.small, t4g.micro and t4g.small, plus c7i-flex/m7i-flex.large, all paid from credits. Public IPv4 costs $0.005/h (about $3.60/month), and I found no source saying the new plan exempts it. At expiry the account is suspended, not billed. That makes it roughly 6 months of free hosting in a European region (Frankfurt, Milan, etc.), not a permanent solution.

### Cited Findings
- Free plan: up to 6 months or until credits are exhausted, with a subset of services and basic support. Credits: $100 at signup plus up to $100 more for exploring foundational services ("up to $200 total"). The plan cannot be extended. — [AWS Free Tier FAQs](https://aws.amazon.com/free/free-tier-faqs/)
- At expiry AWS suspends the account, retains data for 90 days (you can upgrade to restore access), then permanently erases it. — [AWS Free Tier FAQs](https://aws.amazon.com/free/free-tier-faqs/)
- Payment method: "A payment method isn't required to sign up for most new customers", but AWS may request one to verify identity. — [AWS Free Tier FAQs](https://aws.amazon.com/free/free-tier-faqs/)
- Paid plan: full access, usage beyond credits is charged, and a monthly spend limit can be set from $20. — [AWS Free Tier FAQs](https://aws.amazon.com/free/free-tier-faqs/)
- Free-plan-eligible EC2 instances: t3.micro, t3.small, t4g.micro, t4g.small, c7i-flex.large, m7i-flex.large. — [AWS Free Tier: Compute](https://aws.amazon.com/free/compute/)
- On the Free plan you are not charged if you exceed credits; AWS shuts the account down instead. Accounts created before 15 July 2025 keep the legacy 12-month offer. — [infratally: AWS Free Tier in 2026](https://infratally.com/articles/aws-free-tier-2026/); [AWS Builder Center](https://builder.aws.com/content/3BlPcDGddaB6WkPvUf1LIy1rzIm/everything-you-need-to-know-to-get-started-with-aws-free-tier)
- "The six-month clock runs regardless of how many credits you have left." — [infratally (Apr 2026)](https://infratally.com/articles/aws-free-tier-2026/)
- Public IPv4: $0.005/h per address, whether attached or idle, since February 2024 (about $3.60/month). Legacy 12-month accounts get 750 h/month of free public IPv4. I found no source confirming that allowance for the new Free plan. — [infratally](https://infratally.com/articles/aws-free-tier-2026/); [AWS legacy free tier FAQ](https://aws.amazon.com/free/legacy/free-tier-faqs)
- Data transfer out: 100 GB/month free to the internet (then $0.09/GB). — [infratally](https://infratally.com/articles/aws-free-tier-2026/)
- Real-world: in one free-tier t4g.small write-up, about 60% of the cost was the IPv4 address ($3.75). A Reddit comment says charges still accrue on the new plan and are simply paid from credits first. — [search summary citing Classmethod CUR article](https://dev.classmethod.jp/en/articles/ec2-t4g-small-free-tier-2026/); [Reddit snapshot](https://reddit.sentinel-team.org/posts/1rpn9gm/snapshots/2026-03-11T04%3A58%3A06.070402Z) (forum, weaker)
- Turkey: a Turkish user on AWS re:Post reported their card's $1 authorization being declined even though the card worked for other international USD services, with signup stuck in "incomplete" status. — [AWS re:Post](https://www.repost.aws/questions/QUI2fgsj9WQqOLr2EZtnPnww/account-stuck-in-incomplete-status-for-5-days-phone-payment-verification-failing-no-resolution-from-support)
- AWS says declines can stem from unaccepted card type, transaction limits, or issuer 3-D Secure requirements. Its suggested fix is to ask the issuer about 3-D Secure and retry. — [AWS Knowledge Center: credit card declined](https://aws.amazon.com/ru/premiumsupport/knowledge-center/credit-card-declined/)
- An AWS notice quoted on re:Post says Turkish Lira is the only billing/payment currency for AWS Turkey accounts, with prices still in USD. — [AWS re:Post: AWS Turkey billing currency](https://www.repost.aws/questions/QUpoZnQFThRqeh80OqIxlQIg/aws-turkey-billing-and-payment-currency) (page returned 403 to the fetch tool, so this is from the search snippet)

### Inferences
- Cost model: the hourly rates here are from general knowledge, not verified this session.
  - t3.micro (2 vCPU, 1 GB) in eu-central-1 is about $0.012/h, roughly $9/month.
  - t4g.micro is about $6–7/month. t4g.small (2 GB) is about $12–13/month.
  - Add IPv4 (about $3.60/month) and a 20 GB gp3 volume (about $1.6–2/month).
  - A t4g.micro or t3.micro setup therefore costs about $11–14/month in credits. $100 covers about 6–8 months, so the 6-month cap is the binding limit and the second $100 is effectively unneeded.
  - A t4g.small (2 GB RAM, much more comfortable for .NET) costs about $17–18/month, so $100 lasts about 5.5 months. Earning the extra credits covers the full 6 months.
- .NET 10 has first-class linux-arm64 support, so t4g (Graviton) works if the Docker image is multi-arch (`--platform linux/arm64`).
- AWS has the best European region options for Turkey among the free/credit options: Frankfurt, Milan, Stockholm, and Bahrain/UAE are nearer.
- Risk is low: there is no surprise bill on the Free plan, because the account suspends. The plan is 6 months, so a migration is needed after that.

### Gaps
- An official statement on whether Free-plan accounts get any free IPv4 hours. None was found, so assume billed against credits.
- Exact 2026 per-hour EC2 prices per region (not fetched).
- Frequency of Turkish Visa rejections at AWS: only one anecdote was found.

## Microsoft Azure free account

### Takeaway
Azure gives $200 of credit for 30 days, plus 12 months of 750 h/month each of B1s, B2pts v2 (Arm) and B2ats v2 (AMD) burstable VMs (all about 1 GiB RAM), usable in any region including West Europe. Turkey is listed as a supported country. Requirements are a phone number, a non-prepaid credit or debit card, and a Microsoft or GitHub account. To keep the 12-month freebies after day 30 you must convert to pay-as-you-go. Public IPv4 may no longer be free (a third-party source says all public IPs are charged from 1 Oct 2025). At least one Turkish user hit a card-verification failure loop.

### Cited Findings
- First 30 days: $200 credit for any service except third-party Marketplace. Unused credit is lost after 30 days. From day 30 to month 12, only limited quantities of some services are free. — [Microsoft Learn: Create free services](https://learn.microsoft.com/azure/cost-management-billing/manage/create-free-services) (updated 2026-03-25)
- Free VMs: "B1S, B2pts v2 (ARM-based), and B2ats v2 (AMD-based) burstable VMs that are usable for up to 750 hours per month". They can be created in any region where B-series is offered. Microsoft's own example: "a customer in the U.S. can provision a B1S ... VM in West Europe" for free. — [Microsoft Learn: Create free services](https://learn.microsoft.com/azure/cost-management-billing/manage/create-free-services)
- To get free tiers, create resources from the "Free services" page. Resources created elsewhere don't always default to free configurations. — [Microsoft Learn: Create free services](https://learn.microsoft.com/azure/cost-management-billing/manage/create-free-services)
- Requirements: phone number, a non-prepaid credit or debit card, and a Microsoft or GitHub account. A temporary $1 authorization may be placed. Available in 140 countries/regions, and "Turkey" appears in the list. To continue beyond 30 days or after the credit is used up, you must move to pay-as-you-go. — [Azure account / purchase options](https://azure.microsoft.com/en-us/pricing/purchase-options/azure-account)
- "Customers who try Azure free must move to pay as you go within 30 days to continue receiving 12 months free services." These are for new customers only. — [Azure free services](https://azure.microsoft.com/en-us/pricing/free-services)
- After 12 months, running services continue and are billed at PAYG rates. — [search summary of Azure free services/FAQ](https://azure.microsoft.com/free/free-account-faq)
- Turkey appears in the free-account terms' currency table with USD billing. — [Azure free account terms (MS-AZR-0044P)](https://azure.microsoft.com/en-gb/offers/ms-azr-0044p/)
- Disks: an archived (2022) Microsoft page listed 2× 64 GB P6 SSD managed disks, plus 1 GB snapshot and 2M I/O, free for 12 months. Current terms were not confirmed. — [Wayback 2022 Azure free services](https://webcf.waybackmachine.org/web/20220208065848/https://azure.microsoft.com/en-us/pricing/free-services)
- The Marketplace "free account VM" listing names B1, B2ATS and B2PTS for Linux. — [Azure Marketplace free account VM](https://marketplace.microsoft.com/en-us/product/azure-services/microsoft.freeaccountvirtualmachine)
- B2ats v2 = 2 vCPU / 1 GiB RAM, according to a third-party guide; not confirmed on an official page this session. A third-party source also says that from 1 Oct 2025 all public IPs are charged (previously a Basic dynamic IP could be free). — [agentdeals Azure Free Tier 2026](https://agentdeals.dev/azure-free-tier-2026) / [search summary](https://xtec.pages.dev/cloud/azure/virtual-machine) (third-party, unverified)
- Deallocated VMs still incur disk charges if disks are outside the free allowance (about $9.60/month per P6, per a third-party guide). — [search summary, third-party](https://agentdeals.dev/azure-free-tier-2026)
- Turkey signup problem (Aug 2024): a Turkey-based user could not pass card verification ("check credentials", address fields highlighted) with several cards, including a roommate's. The advice was to check that profile/address matches the card, check for an existing Microsoft account with the same details, follow the declined-card guide, try another card, or contact @AzureSupport. No accepted resolution was recorded. — [Microsoft Q&A](https://learn.microsoft.com/en-us/answers/questions/1883628/verification-by-credit-card-does-not-work-for-free)

### Inferences
- Suitability: a B2ats v2 or B2pts v2 (2 vCPU, about 1 GiB) in West Europe or North Europe, or Italy North / Poland Central for lower latency to Turkey, could host PreBot for 12 months, with the same 1 GB RAM caveats as GCP (swap required).
- Converting to PAYG is required after 30 days. From then on, anything outside the free quantities is billed. The main hidden-cost risks are:
  - the public IPv4 (likely about $3–4/month if the Oct 2025 change is real)
  - non-free disk SKUs
  - egress above the free allowance (amount unconfirmed)
- Net: Azure is likely the best *12-month* European option if the Turkish card passes verification, but probably not $0 if public IP is now billed.
- Azure for Students ($100, no card) could be relevant if the owner is a student, but it was not researched this session.

### Gaps
- Current official status of the free public IP allowance and the free bandwidth amount. Not readable on the official pages fetched.
- Official RAM spec of B2pts v2 / B2ats v2 (general knowledge says 1 GiB).
- Whether Turkish cards fail Azure verification systematically: only one 2024 thread was found.

## Oracle Cloud: retry options for the signup error, and whether a retry risks a ban

### Takeaway
There is no official fix for "An error occurred while creating your account". It is widely reported as an automated fraud/risk rejection that can occur even after successful card verification. Oracle allows only one free account per person and forbids creating or *attempting* multiple free accounts. Repeated attempts trigger rate limits and "forbidden" errors, so retries should be few, deliberate, and made with different variables (card, network, exact details). Oracle's Always Free Arm allowance was also quietly halved on 15 June 2026 to 2 OCPU / 12 GB. That is still far more than PreBot needs, so Oracle remains the best free option if signup ever succeeds.

### Cited Findings
- Users report the generic error on two attempts despite a successfully validated credit card, and request manual review. — [Oracle Community: "Free tier signup blocked: generic error after 2 attempts – need manual review"](https://community.oracle.com/CustomerConnect/discussion/958295/free-tier-signup-blocked-generic-error-after-2-attempts-need-manual-review) (page timed out on fetch; title and snippet only)
- Another user corrected a truncated email and then got "forbidden"/rate-limit errors on later attempts. — [Oracle Community: signup blocked after email correction](https://community.oracle.com/CustomerConnect/discussion/961457/free-tier-signup-blocked-after-email-correction-incident-260523-000782)
- Users report the verification amount being taken from the card yet account creation still failing. — [Oracle Community](https://community.oracle.com/CustomerConnect/discussion/825917/unable-to-create-oracle-free-tier-account-even-after-verification-token-is-getting-debited); [Oracle Forums: payment accepted but account creation failed](https://forums.oracle.com/ords/apexds/post/payment-accepted-but-free-tier-account-creation-failed-0273)
- OCI uses very aggressive fraud detection: unusual IP, browser setup or billing format can block signup automatically. This is commentary, not Oracle-confirmed. — [ITNEXT: Why creating an OCI free tier account is so hard](https://itnext.io/why-creating-an-oci-free-tier-account-is-so-hard-and-what-you-can-do-instead-7a06b83ec576)
- Oracle FAQ (Turkish edition): only one Free Trial / Always Free account per person, and creating *or attempting to create* multiple free accounts is prohibited. Card verification uses a temporary authorization that the bank releases in 3–5 days. Contact and billing information must be accurate and valid. A card closed to international online transactions is a possible cause of rejection. — [Oracle Cloud Free Tier SSS (TR)](https://www.oracle.com/tr/cloud/free/faq/) (via search summary; oracle.com returned 403 to the fetch tool)
- Oracle Turkey support numbers: technical +90 212 329 6820, billing +90 312 459 8963 (Free Tier support is limited). — [Oracle TR help/contact](https://www.oracle.com/tr/corporate/contact/help.html) (via search summary)
- "Workaround" reported in commentary: create a regular PAYG account and use only Always Free resources. Unverified whether individuals can sign up directly as PAYG. — [ITNEXT](https://itnext.io/why-creating-an-oci-free-tier-account-is-so-hard-and-what-you-can-do-instead-7a06b83ec576)
- Free→PAYG upgrades are often reported stuck "in progress" for days or weeks, and a Reddit post says the SalesHelp email was discontinued. — [Oracle Community: unable to upgrade to PAYG](https://community.oracle.com/customerconnect/discussion/670817/unable-to-upgrade-to-payg-always-get-the-same-error-message); [r/oraclecloud mirror](https://redlib.hbubli.cc/r/oraclecloud)
- 2026 Always Free change: Ampere A1 was cut from 4 OCPU / 24 GB to 2 OCPU / 12 GB (3,000→1,500 OCPU-hours, 18,000→9,000 GB-hours) effective 15 June 2026, with no blog post or email. Free-only accounts over the limit had instances shut down until resized. Some support agents told users PAYG accounts keep 4/24 for free, but this is unconfirmed. — [InfoQ (Jul 2026)](https://infoq.com/news/2026/07/oracle-cloud-free-tier-limits/); [Linuxiac](https://linuxiac.com/oracle-quietly-cuts-free-tier-ampere-a1-resources-in-half/)
- Conflicting date: a Japanese article says enforcement starts 18 August 2026. — [labmemo](https://labmemo.com/?p=55273) (conflicts with InfoQ)
- Other sources say the AMD micro instances (2× VM.Standard.E2.1.Micro, 1 GB each) and 200 GB block storage are unchanged. — [braindetox (Jun 2026)](https://braindetox.kr/en/posts/oracle_always_free_tier_reduced_2026.html) (secondary)
- Idle Always Free instances can be reclaimed. Oracle evaluates CPU, network and (for Arm) memory use over 7 days. — [space-node: Oracle Always Free limits 2026](https://space-node.net/blog/oracle-cloud-always-free-limits-2026) (third-party)

### Inferences
- What plausibly helps, all community-derived and with no guarantee:
  - Wait several days to weeks between attempts. The rate limits are per identity and network.
  - Use a *credit* card rather than a debit or virtual card, with international online payments and 3-D Secure enabled.
  - Make the name and billing address match the card exactly, in Latin characters without Turkish diacritics, and enter the address in the format your bank uses.
  - Use a clean residential connection with no VPN, proxy or shared/CGNAT mobile IP, and a normal browser profile without privacy extensions that block Oracle's risk-scoring scripts.
  - Use a non-disposable email (Gmail or an own domain).
  - Pick a European home region with known capacity (e.g., Frankfurt or Amsterdam). This cannot be changed later.
  - Most reliable: open a chat/ticket through Oracle's sales or support contact, or call the Turkish billing line, citing the failed attempts and asking for manual review.
- Ban risk: the FAQ forbids attempting multiple free accounts. Retrying with the *same* identity after an error is what everyone does and is not documented as bannable. Retrying with a different identity, name or someone else's card to get a second account would violate the terms, and accounts found to be duplicates can be terminated. Keep it to one person, one account, and a few retries.
- Even after the 2026 cut, 2 OCPU / 12 GB Arm is far more than PreBot needs. The two 1 GB AMD micros alone would also work, with swap.

### Gaps
- No official Oracle explanation of the error, and no confirmed success recipe. No 2026 Turkish-forum (Technopat / DonanımHaber) success reports were found. The searches returned only official Oracle TR pages.
- Whether individuals can currently sign up directly for PAYG, skipping Free Tier.
- Official confirmation of whether PAYG tenancies keep 4/24.

## Other "free" VM providers in 2026: realistic or not

### Takeaway
Beyond Oracle and GCP's e2-micro, nothing in 2026 is a reliable, *permanent*, IPv4-capable free VM.
- Alibaba Cloud and Huawei Cloud advertise 12-month free trials. Alibaba's are for small instances, mostly in Asia, and its official docs conflict on the terms.
- IBM, Linode/Akamai, DigitalOcean and Vultr offer short credit trials of 30–60 days, and DigitalOcean's has reportedly shrunk.
- Hetzner, Scaleway and OVH have no free VM tier. None was found in this research.
- EUserv's "VS2-free" is IPv6-only, invite or manual-approval gated, historically flaky, and its 2026 availability is unconfirmed.
- Generic "free VPS" listicles are mostly affiliate content and should not be trusted.

### Cited Findings
- **Alibaba Cloud.**
  - The official blog says individual accounts get a free 1 vCPU / 1 GB ECS for 12 months (or 2c/2GB for 3 months), and enterprise accounts get 2c/2GB for 12 months. — [Alibaba Cloud blog: 12-month ECS free trial](https://www.alibabacloud.com/blog/jump-start-your-digital-transformation-with-a-12-month-ecs-free-trial_595665) (older post)
  - Alibaba's help docs describe a different, newer structure: a CNY 300 free ECS credit for verified individuals and CNY 660 for verified enterprises. The two conflict, so treat the blog tiers as possibly outdated. — [Alibaba help: Try products for free / benefit rules](https://help.aliyun.com/en/document_detail/2881463.html)
- **Huawei Cloud.** Customers registering after 1 Jan 2025 get free usage of specified products for 12 months, once per product. Whether an ECS VM is included was not confirmed. — [Huawei Cloud free packages](https://activity-intl.huaweicloud.com/en-us/free_packages/index.html)
- **IBM Cloud.** A community comparison lists $200 credit for 30 days and no always-free VM. Another comparison says IBM Virtual Servers have no free trial (conflicting). — [cloudcommunity Cloud-Free-Tier-Comparison (search snippet; repo now 404)](https://github.com/cloudcommunity/Cloud-Free-Tier-Comparison); [spotsaas comparison](https://www.spotsaas.com/compare/ibm-cloud-virtual-servers-vs-alibaba-elastic-compute-service)
- **Linode / Akamai.** $100 promotional credit expiring after 60 days, one per person. — [bestusavps Linode coupon (Mar 2026)](https://bestusavps.pages.dev/deals/linode-coupon) (affiliate site, but consistent across sources)
- **DigitalOcean.**
  - Affiliate sites still say $200 for 60 days. — [devopscube](https://devopscube.com/get-free-digital-ocean-credits/)
  - A more recent source says the new-account credit is now $5 (one per user/payment method). The author did not test it. — [Findstack: "DigitalOcean's Free Credit Is $5 Now, Not $200"](https://findstack.com/resources/digitalocean-free-credit)
  - Older DO promo pages describe $100 for 60 days with a card required. — [try.digitalocean.com](https://try.digitalocean.com/more-vcpus)
  - Conflicting; unverified.
- **Vultr.**
  - Promo credit of about $250–300 valid about 30 days, with a credit card or PayPal required. — [vultrfreetrial.pages.dev](https://vultrfreetrial.pages.dev/); [agentdeals DO vs Vultr](https://agentdeals.dev/compare/digitalocean-vs-vultr) (affiliate, low trust)
  - Vultr's separate "Free Tier" program is granted at random to eligible applicants in limited quantities. The official page returned 403, so specs and duration were not verified. — [search summary of Vultr free-tier references](https://bestusavps.pages.dev/deals/free-vps-trials)
- **EUserv VS2-free.**
  - IPv6-only. 2020-era specs: 1 core at 1 GHz, 1 GB RAM, 10 GB disk. — [LinuxQuestions](https://www.linuxquestions.org/questions/linux-server-73/how-can-i-get-access-to-my-free-vps-on-euserv-ipv6-4175657417-new/); [51sec blog 2021](https://blog.51sec.org/2021/10/euserv-ipv6-only-vps-usage.html)
  - Requires an invite code / manual approval. — [LowEndTalk comments](https://lowendtalk.com/discussion/comment/3141677/)
  - Users report connectivity problems. — [EUserv forum 2020](https://forum.euserv.com/index.php/topic,1584.0.html)
  - Users rely on public NAT64 to reach IPv4 hosts. — [LowEndSpirit](https://lowendspirit.com/discussion/comment/51256)
  - No 2026 confirmation that it is still offered.
- **General.** Several 2026 roundups conclude that truly reliable forever-free VPS hosting is rare, and that Oracle Always Free and GCP e2-micro are the only real permanent candidates; paid VPS from about $4–7/month is the usual fallback. — [valebyte: Free VPS vs Cheap VPS 2026](https://valebyte.com/en/blog/free-vps-vs_-cheap-vps-the-whole-truth-about-2026-free-tiers/); [space-node: Oracle alternatives 2026](https://space-node.net/blog/oracle-cloud-free-tier-alternatives-vps-2026) (vendor/affiliate blogs, moderate trust)

### Inferences
- **Alibaba / Huawei / Tencent.** Even where 12-month trials exist, they typically require real-name/ID verification and a card. Instances are 1 vCPU / 1 GB, often in Asian regions, though Alibaba does have Frankfurt and London. Usefulness for PreBot depends on whether the free SKU is offered in a European region, which is unverified. They are worth a look only as a 12-month fallback.
- **Credit trials (Linode / DO / Vultr / IBM).** These cover 1–2 months at most. They are fine for testing, not for an always-on community app.
- **EUserv / IPv6-only.** PreBot needs a public IPv4 for players. A Cloudflare-proxied DNS record (AAAA to an IPv6 origin) or a Cloudflare Tunnel, which the project already has scripts for, could serve IPv4 users from an IPv6-only box. Outbound Web Push to IPv4-only endpoints would need NAT64. Given EUserv's reliability history and gated signup, it is not recommended as the primary host.
- **"Gratisvps", random "free VPS" sites, and Telegram/forum giveaways.** No reliable evidence of legitimacy was found. Treat as unreliable or potential scams: they collect data, vanish without notice, and have no SLA.
- **Cloudzy.** No free tier was found; it is a paid VPS seller.

### Gaps
- No reliable 2026 data was found for:
  - Tencent Cloud free trial specs and regions
  - Yandex Cloud trial terms for Turkey (historically a grant of credits for 60 days; not verified)
  - Scaleway, OVH and Hetzner free offers (believed to be none; not sourced this session)
  - IBM Cloud's current free-tier page
- No sourced reports of Turkish cards being specifically rejected by Alibaba, Huawei, DO, Vultr or Linode.
- The Vultr Free Tier program's current specs and whether new applicants are still accepted (the official page returned 403).
- Turkish forum (Technopat / DonanımHaber) experience reports on any of these. The searches surfaced none.

## Summary table (for the report writer; every cell traces to the cited findings above, and "?" means unverified)

| Provider | Free compute | Duration | RAM | Region for Turkey users | Card | Hidden costs / notes | PreBot fit |
|---|---|---|---|---|---|---|---|
| Oracle Always Free | A1 Arm 2 OCPU/12 GB (since Jun 2026) + 2× AMD 1 GB | Forever | 12 GB | EU regions (home region fixed at signup) | Yes (auth hold) | Idle reclaim; signup anti-fraud; one account per person | Best if signup succeeds |
| GCP e2-micro | 1× e2-micro, 30 GB standard PD | Forever | 1 GB | US only (us-east1/central1/west1) | Yes | 1 GB egress then ~$0.12/GB; external IPv4 billing unclear | Workable with swap; US latency |
| GCP trial | $300 credit | 90 days | any | EU OK | Yes | Must upgrade manually after | Temporary |
| AWS Free plan | Credits $100 (+$100) for t3/t4g micro/small etc. | 6 months max | 1–2 GB | EU (Frankfurt, Milan…) | Usually not required at signup | IPv4 $0.005/h from credits; account suspends at end | Good for 6 months |
| Azure free | $200/30 days + 750 h each of B1s/B2pts v2/B2ats v2 | 12 months (PAYG required after 30 days) | ~1 GiB | Any region incl. West Europe | Yes, non-prepaid; phone | Public IP possibly billed since Oct 2025 (unverified); Turkish card verification issue reported | Good for 12 months, maybe not $0 |
| Alibaba | 1c/1GB ECS 12 months (blog) vs CNY 300 credit (docs) | 12 months? | 1 GB | ? | Yes + ID verification | Conflicting docs | Uncertain |
| Huawei | 12-month free products (VM?) | 12 months | ? | ? | ? | VM inclusion unverified | Uncertain |
| IBM | $200 credit | 30 days | — | — | Yes | — | Testing only |
| Linode/Akamai | $100 credit | 60 days | — | EU OK | Yes | — | Testing only |
| DigitalOcean | $200/60 days vs $5 now (conflict) | ≤60 days | — | EU OK | Yes | — | Testing only |
| Vultr | ~$250–300 promo / random "Free Tier" | ~30 days | — | EU OK | Card/PayPal | — | Testing only |
| EUserv VS2-free | 1 core, 1 GB, 10 GB | "Forever" (unconfirmed 2026) | 1 GB | Germany | ? | IPv6-only, invite, flaky | Not recommended |
