# Self-hosting PreBot at home (Turkey, Nov 2026): free stable HTTPS URL options, hardware and power

Research date: 2026-11-09. Scope: .NET 10 Blazor Server (WebSockets/SignalR, background workers, SQLite) on a home machine, today exposed with a Cloudflare Quick Tunnel whose URL changes on restart and breaks Web Push. Research budget was limited (~20 tool calls); items marked "unverified" were not checked against primary sources.

## Tunnels with a stable free hostname (Cloudflare named tunnel, Tailscale Funnel, ngrok, zrok, Pinggy, others)

### Takeaway
The two realistic free options with a stable HTTPS URL and no domain purchase are **Tailscale Funnel** (stable `machine.tailnet-name.ts.net`, no interstitial, beta, undisclosed bandwidth cap) and **ngrok's free static dev domain** (stable, but a browser interstitial, a 1 GB/month data cap and 20,000 HTTP requests/month, which a SignalR app with several players can hit). The most robust long-term option is a **named Cloudflare Tunnel on a free domain** (eu.org or DigitalPlat dpdns.org, see next section). zrok and Pinggy free tiers are weaker: zrok has an interstitial plus reported Safe Browsing flags, and Pinggy free URLs are random with a 60-minute timeout.

### Cited Findings

**Cloudflare Quick Tunnel (current setup) vs named tunnel**
- Quick Tunnels have a hard limit of 200 in-flight proxied requests; going over returns HTTP 429. The limit "only applies to Quick Tunnels. To avoid this limit, sign up for a Cloudflare account and create a Cloudflare Tunnel." — [Cloudflare Docs: Quick Tunnels](https://developers.cloudflare.com/cloudflare-one/networks/connectors/cloudflare-tunnel/do-more-with-tunnels/trycloudflare/)
- Quick Tunnels do not support Server-Sent Events; Cloudflare says they are for testing/development only and recommends a remotely-managed (named) tunnel for production. — [Cloudflare Docs: Quick tunnels](https://developers.cloudflare.com/tunnel/get-started/quick-tunnels/); [Cloudflare Sandbox SDK tunnels](https://developers.cloudflare.com/sandbox/sdk/api/tunnels/)
- WebSockets do work over Quick Tunnels (third-party guide); this matches the owner's experience that the current setup works. — [StackHarbor guide](https://stackharbor.com/en/knowledge-base/cf-quick-tunnel-trycloudflare-guide/)
- To put a zone on Cloudflare, the apex must sit one level below a suffix on the Public Suffix List; this is why `you.eu.org` or `you.dpdns.org` can be added as its own zone, and everything needed for a tunnel's domain is on the Free plan. — [Cloudflare Zero Trust learning path: add a site](https://developers.cloudflare.com/learning-paths/clientless-access/initial-setup/add-site/index.md)

**Tailscale Funnel**
- "Tailscale Funnel is available for all plans" (including free Personal) and "is currently in beta." — [Tailscale Docs: Funnel](https://tailscale.com/docs/features/tailscale-funnel.md)
- Funnel URLs are under the tailnet's domain: "Funnel can only use DNS names in your tailnet's domain (`tailnet-name.ts.net`)." This name is stable across restarts (it is the machine's MagicDNS name). — [Tailscale Docs: Funnel](https://tailscale.com/docs/features/tailscale-funnel.md)
- Public ports are limited to 443, 8443, and 10000; the local target port is unrestricted. — [Tailscale Docs: Funnel](https://tailscale.com/docs/features/tailscale-funnel.md); [SSD Nodes](https://www.ssdnodes.com/learn/tailscale-funnel-limits-and-ports)
- "Traffic sent over a Funnel is subject to non-configurable bandwidth limits." No number is published; a Tailscale staffer on Reddit confirmed a limit exists but is not announced. — [Tailscale Docs](https://tailscale.com/docs/features/tailscale-funnel.md); [r/PleX thread](https://redlib.hbubli.cc/r/PleX/comments/1igtim2/bypass_cgnat_plex_no_vps_needed/maxitnq/?context=3); [Stack Overflow mirror](https://kiwix.lema.org/stackoverflow.com_en_all_2023-11/questions/76091568/tailscale-funnel-bandwidth-limit)
- TLS: Tailscale creates valid Let's Encrypt certificates automatically; the local Tailscale client terminates TLS, and "Funnel relay servers do not decrypt the traffic." Requesting certificates too often can hit Let's Encrypt rate limits ("waiting 34 hours"). Public DNS can take up to 10 minutes to appear. Requirements: Tailscale ≥1.38.3, MagicDNS, HTTPS enabled, `funnel` node attribute in the policy file. — [Tailscale Docs: Funnel](https://tailscale.com/docs/features/tailscale-funnel.md)
- Transport is a TCP proxy via Funnel relay servers (so a WebSocket upgrade should pass through); a homelab guide lists WebSocket support as "Yes", but Tailscale's own docs do not explicitly mention WebSockets. — [Tailscale Docs](https://tailscale.com/docs/features/tailscale-funnel.md); [homelab-guide](https://homelab-guide.pages.dev/homelab-tailscale-funnel)
- No interstitial/warning page is described for Funnel in the official docs (contrast ngrok/zrok). — [Tailscale Docs: Funnel](https://tailscale.com/docs/features/tailscale-funnel.md)

**ngrok free**
- Free plan gives one automatically assigned dev domain (e.g. `your-assigned-name.ngrok-free.app`) that "doesn't change when you restart the agent"; custom/reserved/wildcard domains need a paid plan. — [ngrok Docs: Free plan limits](https://ngrok.com/docs/pricing-limits/free-plan-limits)
- Monthly quotas: 1 GB data transfer out, 20,000 HTTP requests, 5,000 TCP connections; rate limit 4,000 HTTP req/min; up to 3 online endpoints; 1 user. "Free endpoints have no timeout—they can stay online indefinitely." — [ngrok Docs: Free plan limits](https://ngrok.com/docs/pricing-limits/free-plan-limits)
- Third-party claims conflict: one glossary says free sessions are capped at 2 hours; another says only 1 active endpoint. These contradict the official docs and look outdated. — [Pangea glossary](https://pangea.app/glossary/ngrok); [InstaTunnel / DEV](https://dev.to/instatunnel/the-great-ngrok-migration-why-developers-are-moving-on-in-2026-8m4)
- Interstitial: shown "in front of all HTML browser traffic on the free tier"; clicking "Visit" sets a cookie that suppresses it for that domain for 7 days. Bypass by sending the `ngrok-skip-browser-warning` header, using a non-standard User-Agent, or upgrading. It does not affect API/programmatic access. — [ngrok Docs: Free plan limits](https://ngrok.com/docs/pricing-limits/free-plan-limits)
- ngrok's docs say nothing about WebSockets or service workers. The common failure for service workers behind an HTML-returning proxy is "Failed to register a ServiceWorker... unsupported MIME type ('text/html')". No source confirms this happens with the ngrok interstitial specifically. — [ngrok Docs](https://ngrok.com/docs/pricing-limits/free-plan-limits); [DFINITY forum example](https://forum.dfinity.org/t/service-worker-the-script-has-an-unsupported-mime-type-text-html/5941)
- The interstitial is a known pain point that blocks fetch/XHR requests in other products' integrations. — [Atlassian developer community](https://community.developer.atlassian.com/t/ngrok-interstitial-page-blocks-requests/61146)

**zrok (OpenZiti / NetFoundry, zrok.io)**
- Shares are ephemeral unless you create a "reserved" share, which keeps a stable share token/URL. — [zrok getting started](https://docs.zrok.io/docs/getting-started); [NetFoundry: sharing reserved](https://netfoundry.io/docs/zrok/1.0/concepts/sharing-reserved)
- Free-tier limits conflict across sources: an Aug 2026 third-party blog says 5 GB per rolling 24h (shares disabled when exceeded until reset), 25 environments, 50 share backends; an older OpenZiti forum reply said 10 GB/24h; an older forum answer said free accounts get only 2 reserved shares. The official limits page redirected and did not render for me. — [51sec blog, Aug 2026](https://blog.51sec.org/2026/08/zrok-secure-open-source-platform-for.html); [OpenZiti forum: bandwidth](https://openziti.discourse.group/t/is-there-any-upload-or-download-limit-for-free-accounts/2862); [OpenZiti forum: reserved addresses](https://openziti.discourse.group/t/question-about-reserved-addresses-limit-and-public-udp-tunnelling/5366)
- Free-tier public shares show an interstitial on first visit (timer resets weekly). It is cookie/JavaScript based; bypass with a CLI user-agent or the `skip_zrok_interstitial` header; paid tiers have none. Users report it blocking JavaScript-fetched resources. — [NetFoundry: interstitial page](https://netfoundry.io/docs/zrok/self-hosting/interstitial-page); [zrok is growing up (blog)](https://blog.openziti.io/zrok-is-growing-up); [OpenZiti forum: interstitial blocks JS fetching](https://openziti.discourse.group/t/interstitial-blocks-javascript-fetching-images/2965)
- One forum post reports Google Safe Browsing flagging `*.share.zrok.io` shares as "Deceptive site ahead" in Chrome (date unclear, no confirmed fix). — [OpenZiti forum](https://openziti.discourse.group/t/issue-with-google-safe-browsing-flagging-share-zrok-io-subdomains-as-unsafe-phishing/6017)

**Pinggy**
- Most sources say the free tier has a 60-minute tunnel timeout and random subdomains that change on reconnect; persistent subdomains are paid (Pro roughly $2–3/month, sources differ). One Pinggy homepage snippet advertises a "free persistent URL for 7 days", which conflicts. — [freetier.co: Pinggy](https://freetier.co/directory/products/pinggy); [makerstack review 2026](https://makerstack.co/reviews/pinggy-review/); [pinggy.io](https://pinggy.io/)

### Inferences
- **Best no-domain option: Tailscale Funnel.** A stable `*.ts.net` HTTPS origin with a real Let's Encrypt cert, no interstitial, and a TCP-level relay make it the closest free drop-in for the current quick tunnel. Risks: beta status, an undisclosed bandwidth cap (likely fine for a few players' text/SignalR traffic), and relay latency (relays are probably not in Turkey; unverified). The tailnet name is randomly assigned (e.g. `tail1234.ts.net`) but stable. Verify that SignalR negotiates WebSockets rather than silently falling back to long polling (check the browser network tab).
- **ngrok free is a poor fit for PreBot.** Blazor Server keeps one WebSocket per tab and pushes UI diffs over it. 1 GB/month out is very tight for a group of players with all-day use, and 20,000 HTTP requests/month covers page loads, static assets and the SignalR negotiate calls. The interstitial is also a problem: the first browser visit gets an HTML warning, and a Web Push service worker or the PWA manifest fetched before the cookie is set could receive HTML instead of JS/JSON. The 7-day cookie means it recurs weekly. Background push delivery itself goes from the push service to the browser and does not pass through ngrok, but the service worker's update checks (`sw.js` re-fetch) could hit the interstitial. This failure mode is inferred, not confirmed.
- **zrok free has the same interstitial class of problem** plus possible Safe Browsing warnings, which is bad for a link shared with friends. Self-hosting zrok removes the interstitial but needs a public server (a VPS), which is then just "frp on a VM".
- **Not researched in this pass (unverified):** localtonet, playit.gg (oriented toward game TCP/UDP), LocalXpose, Serveo, bore, Pangolin, and frp/rathole on a free VM (e.g. Oracle Cloud Always Free). From general knowledge, bore and frp need your own server for a stable public address; Serveo has historically been unreliable; Pangolin is a self-hosted (needs VPS) WireGuard tunnel + reverse proxy. Treat these as unverified.

### Gaps
- Tailscale Funnel's actual bandwidth number and relay locations: not published.
- Official WebSocket statement for Tailscale Funnel: not found (only secondary-source "yes").
- zrok current official free limits (the official limits page did not render) and whether free reserved shares are still capped at 2.
- localtonet / playit.gg / LocalXpose / Serveo / bore free-tier details for 2026: no searches done or results found.
- No direct test or report of ngrok's interstitial breaking `sw.js` registration or Web Push subscription.

## Free domains usable with Cloudflare (to enable a named Cloudflare Tunnel)

### Takeaway
In 2026 the free domains that can be delegated to Cloudflare nameservers (needed for a named tunnel) are **eu.org** (free, permanent, but approval can take days to months) and **DigitalPlat FreeDomain dpdns.org** (fast, on the PSL, officially recommends Cloudflare; us.kg is reportedly suspended for new registrations). **is-a.dev does not allow NS delegation to Cloudflare** (it does offer a `proxied: true` flag, but that does not give you a zone for a tunnel). Freenom is gone, and DuckDNS cannot be added to Cloudflare.

### Cited Findings
- **eu.org:** Registration asks for nameservers up front, but Cloudflare only accepts an already-delegated domain. The usual workaround is to register with a free DNS host (e.g. ClouDNS, Hurricane Electric) as nameservers, then switch to Cloudflare after approval. Adding it to Cloudflare early gives "xxxx.eu.org is not a registered domain". Users report it "works perfectly with cloudflare once approved". — [InfinityFree forum guide](https://forum.infinityfree.com/t/how-to-get-a-free-eu-org-domain/88508); [Google Sites guide (promotional)](https://sites.google.com/view/theslowbookmark/how-to-get-a-free-eu-org-domain-forever-cloudns-cloudflare-method)
- **eu.org approval time** is anecdotal and highly variable: "up to a month" (Jan 2024 guide), 3 months for one user, a few days for others. In a 2025 thread, a request submitted in January 2025 was still pending in March. No official figure. — [InfinityFree: Time taken to get EU.ORG approval](https://forum.infinityfree.com/t/time-taken-to-get-eu-org-domain-approval/92362); [page 2](https://forum.infinityfree.com/t/time-taken-to-get-eu-org-domain-approval/92362?page=2)
- **DigitalPlat FreeDomain:** It operates `*.qzz.io`, `*.dpdns.org`, `*.us.kg`, `*.xx.kg`. Its DNS-hosting tutorial strongly recommends Cloudflare: add the domain to Cloudflare Free, then set Cloudflare's assigned nameservers in the DigitalPlat dashboard. The project claims 500,000+ domains registered. — [domain.digitalplat.org](https://domain.digitalplat.org); [DigitalPlat tutorial mirror: 1.2 DNS hosting](https://vat.pages.dev/DigitalPlatDev/FreeDomain/blob/main/documents/tutorial/getting-started/1.2-dns-hosting.md); [51sec: DigitalPlat + Cloudflare](https://blog.51sec.org/2024/12/free-short-domain-and-easy-integration.html?m=0)
- **us.kg** is reportedly suspended for new registrations due to abuse; dpdns.org is described as the primary offering (third-party directory, not confirmed by DigitalPlat). — [agentdeals.dev: DigitalPlat](https://agentdeals.dev/vendor/digitalplat)
- **dpdns.org is on the Public Suffix List** and Cloudflare accepts dpdns.org domains as zones (Chinese-language forum). Vercel reportedly refuses new dpdns.org domains, which matters only for Vercel, not Cloudflare. Cloudflare can lag PSL updates for newer suffixes (dev.cv example). — [NodeSeek thread](https://www.nodeseek.com/post-797903-1); [Vercel community](https://community.vercel.com/t/cannot-adding-dpdns-org-domains-that-is-in-public-suffix-list/8492); [Cloudflare community: dev.cv](https://community.cloudflare.com/t/cannot-add-hasan-dev-cv/939724)
- **is-a.dev:** NS records are approved only in narrow cases (e.g. a SaaS provider requires delegation), not for DNS providers like Cloudflare or deSEC. Self-hosting and convenience are listed as disallowed reasons, and they may deny NS records at their discretion. A register mirror says "We currently do not support Cloudflare (for NS)". Alternative: `"proxied": true` in the subdomain JSON enables Cloudflare proxying of an A/CNAME record. — [is-a.dev FAQ](https://docs.is-a.dev/faq/); [is-a.dev domain structure](https://docs.is-a.dev/domain-structure/); [GitHub register mirror](https://github.com/qloha/register)

### Inferences
- **Recommended path: DigitalPlat dpdns.org** (fast, Cloudflare-recommended), then a named Cloudflare Tunnel (`cloudflared tunnel create` + DNS route `prebot.yourname.dpdns.org`). This gives a permanent HTTPS origin, removes the 200-concurrent-request cap, and keeps Cloudflare's edge in/near Turkey. **eu.org** is the more reputable, longer-lived backup but slow to approve, so apply now in parallel.
- **Risks of free domains:** abuse-driven suspensions have already hit us.kg, and a provider policy change or a renewal requirement could break the origin, which would invalidate Web Push subscriptions again. Keep a fallback plan, e.g. let the app re-subscribe users on a new origin.
- **is-a.dev with `proxied: true`** cannot point at a Cloudflare Tunnel, since a tunnel needs a CNAME to `<uuid>.cfargotunnel.com` inside a zone in your own Cloudflare account. Inferred from how tunnels route; not verified for is-a.dev specifically.
- **No-domain named tunnel:** I found no Cloudflare mechanism to get a stable public hostname for a named tunnel without your own zone. The `<uuid>.cfargotunnel.com` name is not publicly browsable (general knowledge, unverified in this pass).
- **DuckDNS** only provides A/AAAA/TXT records under duckdns.org (no NS delegation), so it is useful only with port forwarding, which CGNAT usually blocks. **Freenom** stopped offering free domains in 2023. Both are from general knowledge; I did not re-verify them in this pass.

### Gaps
- Official, current (2026) DigitalPlat rules: renewal period, whether a KYC/GitHub verification is required, and any per-account domain cap. The GitHub docs were seen only via mirrors.
- pp.ua status (requires phone/Telegram verification historically): not researched.
- eu.org official stance on current backlog: not found.

## Web Push requirements on ts.net / tunnel domains

### Takeaway
Web Push only needs a secure context (HTTPS with a valid cert) and a stable origin. Both `*.ts.net` (Let's Encrypt via Tailscale) and a Cloudflare-tunnelled custom domain qualify. The thing that breaks subscriptions is an origin change, which is exactly the Quick Tunnel problem. Interstitial-based tunnels (ngrok/zrok free) add a risk of service-worker script fetches being answered with HTML.

### Cited Findings
- Service workers require a secure context (HTTPS); a common registration failure is "not running your application in a secure context". — [MDN: Using Service Workers](https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API/Using_Service_Workers)
- Tailscale Funnel serves only over TLS with automatically provisioned valid HTTPS certificates. — [Tailscale Docs: Funnel](https://tailscale.com/docs/features/tailscale-funnel.md)
- If a proxy returns HTML for the SW script, registration fails with "unsupported MIME type ('text/html')". — [DFINITY forum](https://forum.dfinity.org/t/service-worker-the-script-has-an-unsupported-mime-type-text-html/5941)
- ngrok's free interstitial appears in front of all HTML browser traffic; it can be skipped only via a header or a non-standard User-Agent, neither of which a normal browser visitor sends. — [ngrok Docs](https://ngrok.com/docs/pricing-limits/free-plan-limits)

### Inferences
- Service worker scope and push subscriptions are tied to the exact origin (scheme+host+port). Moving from trycloudflare to a stable host requires every user to re-subscribe once. Funnel on 443 keeps a clean origin; ports 8443/10000 work but form a different origin.
- The Google Safe Browsing flags reported for `*.share.zrok.io` could also affect shared-tunnel domains generally. A dedicated domain (dpdns/eu.org) or `ts.net` is likely safer, though I have no direct evidence about ts.net reputation.
- iOS Safari Web Push requires the site to be installed to the Home Screen (general knowledge, not re-verified); this is independent of the tunnel choice.

### Gaps
- No source specifically confirming Web Push working through Tailscale Funnel or through `*.ngrok-free.app` (expected to work for the origin, but untested).
- Whether ts.net or dpdns.org domains have any reputation or blocklist issues in Turkey or in browsers: not found.

## Home hardware: power cost, reliability, Linux/ARM64 .NET, Android/Termux

### Takeaway
Electricity is cheap relative to the value here. A low-power always-on box (Pi or mini PC at roughly 3–10 W) costs on the order of 10–35 TL/month at Turkey's 2026 residential tariffs, an old laptop about 30–50 TL, and a gaming desktop left on 24/7 roughly 100–200 TL+ (my calculations, not sourced). .NET 10 officially ships Arm64 Linux builds (LTS to Nov 2028), so a Raspberry Pi 4/5 on 64-bit Debian-based OS is viable. Android/Termux is not an officially supported .NET target.

### Cited Findings
- **Electricity tariff, April 2026:** EPDK raised residential tariffs effective 4 April 2026 (published in the Resmi Gazete the same day). For high-consumption users, the energy unit price went from 1.615460 to 1.895808 TL/kWh and the distribution fee from 1.836166 to 2.4249 TL/kWh, about 4.32 TL/kWh before taxes. Low-tier users had no increase on the energy price, which the article does not state. — [memurlar.net, 6 Apr 2026](https://www.memurlar.net/haber/1163908/elektrige-yapilan-yuzde-25-zam-sonrasinda-asgari-aile-faturasi-744-tl-yi-asti.html)
- Calculator/secondary figures conflict: one site gives a 2026 tiered price of 2.92 TL (0–240 kWh) and 4.32 TL (240+ kWh); DonanımHaber gives 2.59 TL and 3.92 TL. These probably reflect different dates or tax inclusion. — [hemenhesap](https://www.hemenhesap.com/hesap/elektrik-fatura-hesaplama); [DonanımHaber](https://www.donanimhaber.com/1-kw-kac-tl-iste-kw-birim-fiyatlari-ve-elektrik-tarife-hesaplama--193641)
- The 2026 annual high-consumption threshold is 4,000 kWh/year (about 333 kWh/month); a new limit for 2027 has been announced. — [DonanımHaber / search snippets](https://www.donanimhaber.com/elektrik-faturalarinda-devlet-destegine-yeni-duzenleme--197970); [CNBC-e](https://www.cnbce.com/enerji/elektrik-faturasinda-820-tl-detayi-2027de-yeni-tuketim-siniri-belli-oldu-h40665)
- **.NET 10 on Arm64 Linux:** .NET 10 (released Nov 2025, LTS, supported to 2028-11-14) has Arm64 Linux builds. Microsoft's package feed publishes .NET 10 for x64 and Arm64. Debian 13 is listed as supporting .NET 10/9/8 (from a docs PR diff). Official linux-arm64 SDK binaries (e.g. 10.0.402) are downloadable. — [AWS Graviton .NET](https://aws.github.io/graviton/dotnet.html); [dotnet/docs PR 49703](https://github.com/dotnet/docs/pull/49703/files); [dotnet/docs PR 49713](https://github.com/dotnet/docs/pull/49713/files); [Microsoft download: SDK 10.0.402 linux-arm64](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-10.0.402-linux-arm64-binaries-targz)
- .NET 10 is packaged in Ubuntu 24.04 LTS archives. — [Ubuntu Discourse](https://discourse.ubuntu.com/t/net-10-is-now-available-on-ubuntu-24-04-lts/73896)
- Microsoft's supported-OS metadata lists Linux, Windows, macOS, and iOS/tvOS, but no Android-as-host (Termux) entry. — [dotnet supported-os.json](https://builds.dotnet.microsoft.com/dotnet/release-metadata/8.0/supported-os.json)

### Inferences
- **Power math** (assumed wattages, not sourced; 720 h/month; using about 2.9–4.3 TL/kWh before taxes):
  - Raspberry Pi 5 at about 4 W gives about 2.9 kWh/month, roughly 8–13 TL.
  - Mini PC (N100-class) at about 8–10 W gives about 6–7 kWh/month, roughly 20–31 TL.
  - Old laptop, screen off, at about 12–15 W gives about 9–11 kWh/month, roughly 26–47 TL.
  - Desktop gaming PC idling at about 50–80 W gives about 36–58 kWh/month, roughly 105–250 TL. It can also push the household over the 333 kWh/month high-consumption threshold.
  - Add taxes/fees (around 10% per the memurlar example) on top.
- **Reliability on Windows:** You need to disable sleep/hibernate, set Active Hours or pause updates, run the app and `cloudflared`/`tailscale` as Windows services (both support it), and set the BIOS to "power on after AC loss" for power cuts. A laptop's battery acts as a built-in UPS for short outages, but the modem/ONT also needs power (a small router UPS helps). These are general practices, not sourced here.
- **Raspberry Pi path:** Use Raspberry Pi OS 64-bit (Debian 12/13-based) with `dotnet publish -r linux-arm64 --self-contained`. Use an SSD or high-endurance card for SQLite (SD-card wear). The Pi itself is not explicitly on Microsoft's supported list; it is supported indirectly via Debian Arm64 (inference). Both `cloudflared` and `tailscale` have official Arm64 Linux builds (general knowledge).
- **Android + Termux:** Not recommended. .NET does not officially support Android as a server host. Community workarounds run a Debian userland via proot-distro and install linux-arm64 .NET there; they work for some but are fragile. Android also kills background processes (Doze/battery optimisation), and phones are risky to keep on a charger 24/7 (battery swelling). This is unverified with sources in this pass.

### Gaps
- An authoritative EPDK November 2026 residential tariff table (all-in TL/kWh including taxes) was not retrieved; figures may have changed after April 2026 (e.g. a July/October update).
- Official Microsoft statement on Raspberry Pi OS support for .NET 10, and measured Blazor Server memory/CPU on a Pi 4 vs Pi 5.
- Power-outage frequency statistics for Turkish regions: not researched.
- Termux + .NET 10 community success reports for 2026: not found.

## CGNAT in Turkey (Türk Telekom, Superonline, Vodafone, TurkNet): port forwarding and static IP cost

### Takeaway
CGNAT is common on Turkish fiber, especially after fiber migrations. Users often get out of it for free by asking customer service for a "gerçek IP" or dynamic public IP. A static IP is a paid monthly add-on with no clear published 2026 retail price. Even with a public IP, a dynamic address plus port 443 forwarding is more fragile than a tunnel. Because tunnels need only outbound connections, CGNAT does not matter for Cloudflare Tunnel or Tailscale Funnel.

### Cited Findings
- A Türk Telekom 2022 wholesale protocol allows assigning virtual IPs via CGNAT where real public IPs are not available. — [Türk Telekom Toptan PTS ek protokol (PDF)](https://toptan.turktelekom.com.tr/media/adpewsb2/pts-ek-protokol-010422.pdf)
- Forum users report being moved into a CGNAT pool after switching to Türk Telekom fiber (port forwarding stopped working) and getting out by asking support for a dynamic real IP. — [DonanımHaber: Türk Telekom fiber CGNAT nasıl çıkılır](https://forum.donanimhaber.com/turk-telekom-fiber-cgnat-nasil-cikilir--162137943); [DonanımHaber: TT port açmak](https://forum.donanimhaber.com/turktelekom-port-acmak--145979779)
- A third-party guide (natchecker) recommends first requesting a dynamic public IPv4 from Türk Telekom before paying for a static IP. Otherwise it suggests IPv6, port-forwarding VPNs, or a VPS reverse tunnel, and says individual vs corporate prices differ. — [natchecker: Türk Telekom CGNAT](https://natchecker.com/tr/blog/turk-telekom-cgnat-cikarma-statik-ip-olmadan)
- **How to detect CGNAT:** the WAN IP in the modem differs from the IP shown by whatismyip/speedtest. One user tip: if after a 10–15-minute modem power-off you don't get an IP starting 88/85/176, request a real IP from customer service. Some staff deflect and just reset the modem. — [Technopat / forum aggregation](https://www.technopat.net/sosyal/etiket/statik-ip/?amp=1); [natchecker: how to check CGNAT](https://natchecker.com/tr/blog/how-to-check-cgnat)
- No extra fee for port opening itself; the fee is for a static IP, which is available for a monthly extra payment. One Superonline user found ports opened after switching to Superonline's own modem. — [Technopat statik IP tag](https://www.technopat.net/sosyal/etiket/statik-ip/?amp=1); [Technopat Superonline tag](https://www.technopat.net/sosyal/etiket/superonline/)
- TurkNet and Kablonet users have long-running threads about leaving the CGN pool, so the practice is widespread across ISPs. — [DonanımHaber: TurkNet CGN](https://forum.donanimhaber.com/turknet-cgn-ip-havuzundan-cikmak-istiyorum--143570056); [DonanımHaber: Kablonet CGN](https://forum.donanimhaber.com/kablonet-cgn-havuzundan-cikmak--126380237-9); [Technopat: TurkNet port forwarding](https://www.technopat.net/sosyal/konu/concerns-on-turknet-port-forwarding.2224421/?amp=1)
- Wholesale only (not a consumer price): a 2022 Türk Telekom wholesale document lists a "Bölgesel Statik IP Yönetim Ücreti" of 5,631.09 TL per circuit/month among optional fees. BTK approved updated TT wholesale tariffs effective 1 July 2026. — [TT PTS uygulama esasları (PDF)](https://toptan.turktelekom.com.tr/media/4m2dgio3/tt-pts-hizmeti-uygulama-esaslari-150422.pdf); [BTK decision PDF 2026](https://www.btk.gov.tr/s3/web-btk-site/7708c26c-5161-4919-8b3d-daa31d4ac8fb/2026/06/7c74f9cc-9cbe-48e0-a39a-53a9e30b2968.pdf)

### Inferences
- **For PreBot, CGNAT is a non-issue if a tunnel is used.** Port forwarding would also require handling dynamic IPs (DDNS), certificates, exposing the home IP, and possible ISP blocking of inbound 80/443. A tunnel is simpler and safer for a small group.
- **A static IP is not worth paying for here.** Retail prices are not published; forum reports imply a monthly fee. A free tunnel plus a free domain removes the need.

### Gaps
- Official 2026 retail static IP prices for Türk Telekom, Superonline, Vodafone and TurkNet: not found. You would have to call the ISP or check its own static-IP page.
- Whether Turkish ISPs block inbound ports 80/443 on residential lines: no source found.
- IPv6 availability per ISP in 2026 (IPv6 would allow direct reachability, but many Turkish mobile networks lack IPv6): not researched.
