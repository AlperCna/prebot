# Trust, verification and host selection in PreBot (claims A and B)

Scope: test two player-perspective claims about PreBot (TR, Ranked Flex). Claim A is "op.gg is not one tap, ranks are self-declared, add a direct op.gg link". Claim B is "first-come-first-served with no host choice; the only exit is 'Gelmedi, yeniden ara', which wrongly adds a no-show". Sources are the PreBot code, live sites, Riot developer docs, and LFG and marketplace research. Research date: 2026-11-09.

## What do profile URLs look like on TR, how are Riot IDs encoded, and which sites do TR players use?

### Takeaway
OP.GG takes the Riot ID in the path as `op.gg/lol/summoners/tr/{gameName}-{tagLine}`, with spaces sent as `%20` and the `#` replaced by `-`. U.GG uses region `tr1` and lowercase names. A deep link needs no API key and no Riot approval. I found no reliable source on which site TR players use most. OP.GG is widely called the most used site worldwide, and Turkey is not among its top-5 traffic countries.

### Cited Findings
- **OP.GG pattern (checked on a live page).** `https://op.gg/lol/summoners/kr/Hide%20on%20bush-KR1` loads a summoner profile. The page title is "Hide on bush#KR1 - Summoner stats", and the internal tab links use the same `/lol/summoners/{region}/{name with %20}-{tag}` path. — [OP.GG](https://op.gg/lol/summoners/kr/Hide%20on%20bush-KR1)
- **OP.GG Turkish UI prefix.** OP.GG also serves pages with a Turkish-UI language prefix in front of the same path, e.g. `op.gg/tr/lol/summoners/kr/Fix-Happy/style`. — [OP.GG TR locale](https://op.gg/tr/lol/summoners/kr/Fix-Happy/style)
  - That makes a TR-server profile in the Turkish UI `https://op.gg/tr/lol/summoners/tr/{name}-{tag}`.
- **OP.GG multi-search.** OP.GG has a Turkish-locale multi-search page. — [OP.GG multisearch](https://op.gg/tr/lol/multisearch)
- **OP.GG "find a teammate" button.** Turkish-locale OP.GG TFT profile pages show a "Takım arkadaşı bul" button, so OP.GG itself pushes teammate finding. — [OP.GG TFT TR page](https://op.gg/tr/tft/summoners/euw/Lope-1990) (search snippet only, not fetched)
- **A wrong region gives a 404.** `https://op.gg/lol/summoners/tr/Hide%20on%20bush-KR1` returned HTTP 404 because that account is not on TR. A stale or mistyped Riot ID will therefore show a dead page, not a search. — [OP.GG (404 test)](https://op.gg/lol/summoners/tr/Hide%20on%20bush-KR1)
- **U.GG pattern.** U.GG uses `https://u.gg/lol/profile/tr1/{gamename}-{tagline}/overview` with region code `tr1`. Names and tags appear lowercase, and spaces are `%20`. Non-ASCII characters are UTF-8 percent-encoded: "V ì#TR1" becomes `v%20%C3%AC-tr1`. — [U.GG example 1](https://u.gg/lol/profile/tr1/trabzonlu%20061-tr1/overview); [U.GG example 2](https://u.gg/lol/profile/tr1/v%20%C3%AC-tr1/overview)
- **LoLProfile pattern.** LoLProfile uses `https://lolprofile.net/summoner/tr/{Name}-{TAG}`. Some indexed links encode the hyphen as `%2D`. — [LoLProfile](https://lolprofile.net/summoner/tr/Seadero-TR1); [LoLProfile %2D variant](https://lolprofile.net/summoner/tr/Umidobequit%2DTR1)
- **Lolytics pattern.** Lolytics uses `lolytics.com/lol/challenges/{Name}-{TAG}?region=tr`. — [Lolytics](https://www.lolytics.com/lol/challenges/Wiru-TR1?region=tr)
- **League of Graphs.** It has TR-server champion leaderboards under `/rankings/summoners/{champion}/tr`. I could not check its profile URL, because the fetch returned 403. — [League of Graphs TR rankings](https://www.leagueofgraphs.com/rankings/summoners/smolder/tr)
- **OP.GG popularity.** One 2026 roundup calls OP.GG "the most widely used" League stats site. — [ExitLag](https://www.exitlag.com/blog/best-league-of-legends-reviews-stats-sites/)
- **OP.GG traffic.** Similarweb (October 2026 data) shows 53.2M visits over the last three months. Its top countries are Korea, the US, Brazil, Taiwan and China; Turkey is not shown, and the full list is behind a login. — [Similarweb](https://www.similarweb.com/website/op.gg/)
  - This conflicts with Semrush, which reported about 62.5M visits in October 2025 for a single month. — [Semrush](https://www.semrush.com/website/op.gg/overview/) — Both are estimates; treat the volumes as rough.
- **Riot's linking rules.** Riot's general policies do not address linking to third-party sites. The closest rule is that products "cannot de-anonymize players who cannot reasonably be identified from visible information". — [Riot Developer Policies](https://developer.riotgames.com/policies/general)

### Inferences
- **How to build the link.** Split the stored Riot ID on its **last** `#`. Percent-encode the name and the tag **separately**: `Uri.EscapeDataString` in C#, `encodeURIComponent` in JS. Join them with `-`.
  - Never put a raw `#` in the URL: the browser treats everything after it as a fragment, so the tag is lost.
  - Turkish letters (ı, İ, ş, ğ, ç, ö, ü) become UTF-8 percent-encoding automatically, as the U.GG `%C3%AC` example shows.
- **Case.** Keep the original case for OP.GG.
  - If U.GG (lowercase) is ever added, use `ToLowerInvariant()`, never culture-sensitive `ToLower()`. Under tr-TR culture, .NET turns "I" into dotless "ı", which yields a different and broken URL (the "Turkish-I" problem; general .NET knowledge, not sourced here).
- **Recommended primary link:** `https://op.gg/lol/summoners/tr/{EscapeDataString(name)}-{EscapeDataString(tag)}`.
  - A Turkish-UI variant is `https://op.gg/tr/lol/summoners/tr/...`.
  - On the web, open it in a new tab (`target="_blank" rel="noopener"`). In Discord, use a link button next to the existing buttons.
- **Is a deep link allowed?** I found no rule against it.
  - Riot's policy is silent. The data is the player's own Riot ID, which they typed into PreBot for exactly this purpose, so it raises no de-anonymization concern. It is shown only to listing participants (the README says Riot IDs are visible only inside the listing).
  - Linking to public profile URLs is standard practice: Porofessor, Discord bots and OP.GG's own multi-search all work on Riot IDs.
- **One link is enough, for now.** It should go to OP.GG, the site the claim names and the one players recognize. A second link (U.GG or LoLProfile) adds clutter for little gain until there is TR-specific data on site preference.

### Gaps
- **Unverified URL patterns.** The live profile URL patterns for deeplol.gg, leagueofgraphs, porofessor and mobalytics were not checked: fetches returned 403 or only a page title, and searches returned no TR profile URLs. From prior knowledge they are likely the following; verify before use:
  - `deeplol.gg/summoner/tr/Name-TAG`
  - `leagueofgraphs.com/summoner/tr/Name-TAG`
  - `porofessor.gg/live/tr/Name-TAG`
  - `mobalytics.gg/lol/profile/tr/name-tag/overview`
- **log.lol.** No site called "log.lol" turned up in searches. The claim may mean another site (e.g. lolprofile.net, dpm.lol or lolvvv); this needs clarification.
- **TR site preference.** No reliable data on which stats site TR players prefer. Similarweb's per-country share is behind a login, and no Turkish forum survey was found.
- **Multi-search format.** The OP.GG multi-search query-string format was not checked.
- **OP.GG terms of use.** Not read for deep-linking clauses. Hyperlinking is generally not restricted, but this was not confirmed against op.gg's ToS.

## Which verification options exist: RSO, Riot API rank lookup, icon-change checks, and what do similar products do?

### Takeaway
Any **automatic** verification needs a Riot API key that is legal for a public product, which means a **production key**. Personal and development keys are explicitly banned for public use. RSO (OAuth) is available only to production-key holders, and the older "third-party code" ownership check has been removed in favour of RSO. Icon-change verification is the common community pattern and needs no RSO, but it still makes API calls. The deep link from the previous section is the only option that needs nothing from Riot.

### Cited Findings
- **RSO purpose.** RSO "allows players to safely link their Riot Account to other applications." — [Riot docs (LoL)](https://developer.riotgames.com/docs/lol)
- **RSO access.** RSO is "only available to developers with Production Level API Keys". After approval, Riot reaches out "in the developer portal app messaging to kick off the RSO integration process." — [Riot docs (LoL)](https://developer.riotgames.com/docs/lol)
- **RSO flow.** RSO runs through `auth.riotgames.com/authorize` with `scope=openid+offline_access`. The signed-in account is identified with `/riot/account/v1/accounts/me`. — [Riot docs (LoL)](https://developer.riotgames.com/docs/lol)
- **Riot ID lookups.** Riot ID to PUUID: `/riot/account/v1/accounts/by-riot-id/{gameName}/{tagLine}`. PUUID to Riot ID: `/riot/account/v1/accounts/by-puuid/{puuid}`. The by-name summoner endpoints are deprecated. — [Riot docs (LoL)](https://developer.riotgames.com/docs/lol)
- **Development keys.**
  - They expire every 24 hours.
  - "You may not maintain a public product with a development API key." — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Personal keys.**
  - Limits are 20 requests per second and 100 per 2 minutes.
  - They are meant for "the developer or a small private community".
  - "You may not run your application for public consumption using a personal key," including open alpha/beta tests. — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Production keys.**
  - Starting limits are 500 requests per 10 seconds and 30,000 per 10 minutes.
  - Approval "typically requires a working prototype".
  - Reviewers look for player benefit.
  - "The process and length of time required to obtain an approved production key can vary" (no timeline is given). — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **General policy rules.**
  - All products must be "registered in, and audited by Riot Games through the Developer Portal".
  - Products "cannot create alternatives for official skill ranking systems" (e.g. MMR or ELO calculators).
  - Products must show the legal boilerplate visibly.
  - Monetized products need a free tier. — [Riot Developer Policies](https://developer.riotgames.com/policies/general)
- **Third-party code removed.** A community docs site states that the Third Party Code API "has been removed" and users are "encouraged to migrate over to RSO". It cites a Riot tweet that was not retrieved. — [Hextechdocs](https://hextechdocs.dev/tag/third-party-code/)
  - That endpoint let a player type a code into the client settings, which the app then read through the API to prove ownership. — [LeagueAPI wiki](https://github.com/Kelmatou/LeagueAPI/wiki/Verification-Code)
- **Icon-change verification in production.** Challengermode, a tournament platform, has the user set a specific summoner icon in the client. The system detects the change and confirms ownership, and the user can change the icon back afterwards. — [Challengermode support](https://support.challengermode.com/pt/game-specific/how-to-verify-a-summoner)
- **Discord rank bots.**
  - lol-ranks-bot is open source. The user starts verification and the bot assigns a rank role; a fork refreshes ranks periodically. Its setup uses a Riot development key. — [lol-ranks-bot (mirror of the GitHub repo)](https://gh.r-mj.com/simwai/lol-ranks-bot)
  - Other bots list similar features: Miko by Duoo.gg advertises rank verification per owned account, and Orianna Bot assigns roles by ranked tier. — [top.gg Miko listing](https://top.gg/api/bot/1096340606235717642); [Orianna Bot](https://top.gg/bot/244234418007441408)
- **Riot-ID matching instead of icons.** One private community bot resolves the Riot ID to a PUUID through Account-V1 and checks that the user's Discord nickname matches. — [community bot](https://dnbc.duckdns.org/)

### Inferences
- **Options for PreBot, cheapest first:**
  1. **OP.GG deep link.** No key, no approval, ships in hours. The host verifies with their own eyes.
  2. **Show the real Flex rank from the API.** Needs a production key: PreBot is public, so a personal key is not allowed. Flow: Account-V1 Riot ID to PUUID, then the League-V4 entries for that PUUID. This replaces "self-declared" with "Riot-sourced", but it does **not** prove ownership: anyone can type someone else's Riot ID.
  3. **Icon-change ownership check.** Needs the same production key (it reads profileIconId through Summoner-V4 by PUUID). One-time friction of about 1 minute; proves ownership. No source found that Riot forbids it; Challengermode, a sizeable commercial platform, uses it.
  4. **RSO.** Best UX and strongest proof, but needs a production key **plus** a separate RSO onboarding with Riot. That is the slowest path and has no published timeline.
- **The production key is the gating item for options 2 to 4.** Riot wants a working prototype first, and PreBot already is one. Applying early costs little and starts the clock.
- **Don't build an implicit skill rating.** A rank shown from the API is fine; OP.GG and others display official ranks. PreBot must not compute its own "real elo" score: Riot policy bans alternatives to official ranking, including MMR/ELO calculators.
- **Ownership is a smaller problem for this use case.** The host invites the exact Riot ID shown, so an impostor's typed ID would invite someone else. The real risk is a player **lying about rank**, not about identity.
  - So for PreBot, a deep link (manual check) or an API rank (automatic check) solves most of the trust problem. Ownership proof (icon or RSO) is a later, lower priority.

### Gaps
- **League-V4 by-PUUID endpoint.** Its existence and exact path (`/lol/league/v4/entries/by-puuid/{puuid}`) were not confirmed on Riot's docs page; the fetched page does not mention it. Verify at developer.riotgames.com/apis.
- **Production key and RSO approval times.** No data on real approval times for TR-focused LFG products. Riot publishes none, and no developer reports were found.
- **Policy on icon-change checks.** No explicit Riot statement allowing or forbidding them was found.
- **What LoL-specific LFG sites do.** LoLFinder, duo sites and RiftGuard were not examined in detail; no primary pages were fetched for them.

## Matching models: first-come, application/accept, or instant join with host veto? Evidence on speed vs quality

### Takeaway
Major LFG tools let the **host choose per listing** between instant join and an application step; Bungie's Fireteam Finder is the clearest example. Marketplace research shows host rejections are costly: Airbnb guests rejected once were 51% less likely to book that trip, and rejections often drive users off the platform. Pre-filtering cuts rejections a lot. The best fit for PreBot is to **keep instant join (speed) and add a cheap, non-punitive host veto**, plus better pre-filters (verified rank where possible). An optional "accept first" mode would suit hosts who care more about quality than speed.

### Cited Findings
- **Bungie Fireteam Finder.** Listing creators choose whether players "automatically join" or must apply. Creators can accept or reject applications based on the listing's requirements, and applicants see "Application Pending". — [Bungie Help: Fireteam Finder Guide](https://help.bungie.net/hc/cs/articles/25787853699220-Fireteam-Finder-Guide) (from search snippets; the English page returned 403)
- **Rejection cost (Fradkin, MIT IDE, Airbnb data).**
  - Rejections are "a particularly large transaction cost" and "often cause guests to leave the platform altogether."
  - "Guests who are initially rejected are 51% less likely to book a specific trip on the platform than those who are accepted." — [MIT IDE Research Brief 07/2017 (Fradkin)](https://ide.mit.edu/wp-content/uploads/2017/07/IDE-Research-Brief_0717.pdf)
- **Filters and availability tracking (same brief).**
  - Without availability tracking and filtering, "searches leading to initially successful matches would fall by 68% and rejections would increase by 140%."
  - Congestion rejections (several people going for the same listing at once) are "relatively rare". Screening rejections (host preferences) are modelled separately.
  - A ranking that accounts for the chance of a screening rejection would raise the share of searchers who tried to book and were accepted "to 34% from 24%." — [MIT IDE Research Brief (Fradkin)](https://ide.mit.edu/wp-content/uploads/2017/07/IDE-Research-Brief_0717.pdf)
- **Share of rejected inquiries (secondhand).** A paper citing Fradkin reports that 49% of Airbnb inquiries were rejected or ignored. — [Columbia working paper](https://business.columbia.edu/sites/default/files-efs/citation_file_upload/ssrn-2427960.pdf) (secondary citation)
- **Labor-market evidence.** In an online labor market, rejections causally lower the chance that a job opening gets filled. — [Horton, "Buyer Uncertainty about Seller Capacity"](https://john-joseph-horton.com/papers/scs.pdf) (search snippet)
- **Airbnb declines.** Declining a request before confirmation does not itself hurt placement, but "your search result placement may be impacted if you frequently decline requests". — [Airbnb Help 259](https://www.airbnb.com/help/article/259/what-if-i-feel-uncomfortable-with-a-guest)
  - Hosts may cancel a confirmed booking "without fees or other consequences" only for qualifying reasons and may be asked for evidence. — [Airbnb Help 259](https://www.airbnb.com/help/article/259/what-if-i-feel-uncomfortable-with-a-guest)
- **Instant Book penalty-free cancellations.** Forum posts claim Instant Book hosts get up to 3 penalty-free cancellations for concerns about a guest. One post says the number is unlimited, and the current official text was not found. — [Airbnb Community](https://community.withairbnb.com/t5/Support-with-your-bookings/Unhappy-with-instant-booking/m-p/1767598) (unverified)
- **Vendor conversion figures for Instant Book.** Plum Guide says "up to 21% better conversion". An Expedia figure relayed by Your.Rentals says about 35% higher conversion. Both come from vendors with commercial interests; neither is Airbnb data or independently checked. — [Plum Guide](https://help.plumguide.com/en/articles/13169027-instant-booking-vs-request-to-book-the-benefits); [Your.Rentals](https://your.rentals/blog/instant-booking-for-vacation-rental-hosts/)
- **Conditional instant book.** Hostaway describes "conditional" instant book, which only lets guests who meet requirements book instantly, as a middle ground. — [Hostaway](https://www.hostaway.com/blog/airbnb-instant-book/)

### Inferences
- **PreBot today is effectively instant book, with a hidden veto.** The veto ("Gelmedi") is mislabelled and punishes the joiner (next section).
  - The research supports keeping instant join for speed, since dropping the approval step is the main driver of conversion.
  - It also supports cutting **screening rejections** with pre-filters, which is the Airbnb filter lesson: a Riot-sourced Flex rank, or at least a rank range on the listing that is enforced against the API-verified rank.
- **Recommended model: "instant join + short host veto".**
  - The first tapper takes the seat as today.
  - The host sees the joiner's OP.GG with one tap and can press **"Uygun değil, sıradakine geç"** during a short veto window, before the invite or within about 2–3 minutes.
  - The seat reopens at once, the bot keeps notifying, and the vetoed player is not notified again for that listing.
- **Optional per-listing mode, as in Bungie.** "Önce onayla" (applications) for hosts who value quality over speed, e.g. high-elo 4-stacks.
  - This costs fill speed: each applicant waits for a human decision, and Fradkin's data says rejections cost users more than slower fills do.
  - Keep instant join as the default.
- **Keep the declined player's experience gentle.** Rejected users churn (51% lower booking), so a vetoed joiner should get a neutral message ("Grup başka bir oyuncuyla devam etti; diğer ilanlara bak") plus immediate visibility of other open listings. Do not say "rejected".

### Gaps
- **No LFG fill-speed data.** I found no published A/B or academic data on fill speed or quality for gaming LFG (Destiny 2, Valorant or Overwatch LFG, Discord bots, GamerLink, LFG.gg); none was fetched.
- **Airbnb Instant Book cancellation rules.** The official current count of penalty-free host cancellations under Instant Book was not confirmed.

## Reputation and no-show design: separating "did not show" from "host rejected", abuse risks, display

### Takeaway
PreBot currently records **every** host exit as a no-show. That pollutes the per-player badge shown to future hosts ("daha önce N kez gelmedi") and the public "katılıp gelmeyen %" statistic. The fix is to split the reasons in the data model, not just the button label. Host vetoes should be counted against the **host** for rate-limiting, Airbnb-style, not against the joiner.

### Cited Findings
- **The button and what it does (code).** The host button "Gelmedi, yeniden ara" calls `ReopenForNoShowAsync`. That releases the **most recently claimed** active seat with `SeatReleaseReason.NoShow`; the host cannot pick which joiner. — [PreService.cs:352-381](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Core\Services\PreService.cs); [ListingPage.razor:188](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Web\Components\Pages\ListingPage.razor); [ListingRenderer.cs:77](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Discord\Bot\ListingRenderer.cs)
- **How the badge is counted (code).** No-show counts come from all slots with `ReleaseReason == SeatReleaseReason.NoShow`. Hosts see "daha önce N kez gelmedi", with the tooltip "Daha önceki ilanlarda ilan sahipleri 'Gelmedi' dedi". — [ModerationService.cs:110-120](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Core\Services\ModerationService.cs); [ListingPage.razor:103-105](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Web\Components\Pages\ListingPage.razor)
- **The public statistic (code).** The Stats page computes "katılıp gelmeyen" % from the same NoShow seat releases. — [StatsService.cs:69](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Core\Services\StatsService.cs); [Stats.razor:38-39](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Web\Components\Pages\Stats.razor)
- **The README's description (code).** The README describes the button only as "Gelmedi: 'Gelmedi, yeniden ara' → yer boşalır, yeniden aranır"; there is no other host exit besides closing the listing. — [README.md:22](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\README.md)
- **A separate report path (code).** There is a separate report option "Katıldı ama gelmedi" (`ReportReason.NoShow`) that goes to admins. — [ListingPage.razor:234](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Web\Components\Pages\ListingPage.razor); [Admin.razor:126](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Web\Components\Pages\Admin.razor)
- **Airbnb's guardrails against host abuse of declines.** Frequent declines may hurt the host's placement. Penalty-free cancellations need a qualifying reason and possibly evidence. — [Airbnb Help 259](https://www.airbnb.com/help/article/259/what-if-i-feel-uncomfortable-with-a-guest)

### Inferences
- **Today a "not a fit" decision and a real no-show look the same.** Pressing "Gelmedi" for the first reason permanently lowers the joiner's reputation and inflates the public no-show statistic. Both signals become unreliable: a "3 kez gelmedi" badge may mean "3 groups didn't like my OP.GG".
- **Data model.** Add a release reason such as `SeatReleaseReason.HostDeclined` ("Uygun değil").
  - Count only `NoShow` in `GetNoShowCountsAsync` and in the "katılıp gelmeyen" statistic.
  - Track `HostDeclined` separately, as its own product metric (decline rate per listing and per host).
- **UI.** Put per-joiner controls next to each joiner's Riot ID, instead of one button that always hits the latest joiner:
  - "OP.GG" link.
  - "Uygun değil, sıradaki" (no reputation effect).
  - "Gelmedi" (counts as a no-show). Allow it only after an invite could plausibly have happened, e.g. a few minutes after the join, so it can't be used as an instant veto.
- **Abuse guards.**
  - On hosts: cap "Uygun değil" per listing (e.g. 2–3), and show hosts a soft warning when their decline rate is high. This mirrors Airbnb's placement penalty.
  - On joiners: do not re-notify a declined player for the same listing, and keep reasons private.
  - Consider showing a no-show badge only after ≥2 no-shows from **different** hosts, which limits a single hostile host's power.
- **Positive signals beat negative ones.** "N kez oynadı" (completed games through PreBot), or a simple host-confirmed "geldi" tick, gives hosts a positive trust signal and makes no-show counts read in context.
  - This is a design inference; no source was fetched on honor or commend systems.

### Gaps
- **Reputation designs elsewhere.** No primary sources were fetched on how other LFG tools show reliability or separate kicks from no-shows: Riot Honor, Overwatch endorsements, Discord LFG bot "reliability" scores, GamerLink ratings.
- **No evidence on how often hosts misuse "Gelmedi" today.** PreBot's own event data (NoShow releases within minutes of a join) could answer this.

## Do TR players routinely check OP.GG before accepting a 5th?

### Takeaway
Plausible, but **not established by evidence**. Searches of Ekşi Sözlük, Technopat and Turkish phrasing ("op.gg at", "op.gg'sine baktım") found no relevant threads. English-language LFG guides do recommend checking match history or rank before accepting a stranger. "A 4-stack *always* checks OP.GG" should be presented as a reasonable assumption, not a fact.

### Cited Findings
- **LFG guides recommend checking stats first.** An LFG guide says stat-integrated platforms let you check "recent match history, role performance, and champion pool" before accepting a duo request, and names OP.GG among LFG sources. — [ExitLag LFG guide](https://www.exitlag.com/blog/lfg-league-of-legends/)
- **Check accounts before queueing.** Duo-finding guides advise checking both accounts before the first queue and cross-checking Solo/Duo against recent role distribution. — [Tapin.gg](https://tapin.gg/blogs/find-league-of-legends-duo-partner)
- **Flex rank is a weak signal of team skill.** A Turkish-language guide on Flex vs Solo/Duo says this and suggests asking recruits about their five-stack history. — [GuildOrder (TR)](https://guildorder.com/tr/games/league/guides/flex-vs-solo-duo-queue)
- **No Turkish forum evidence found.** Turkish forum searches (Ekşi Sözlük, Technopat) on "flex 5. kişi op.gg" or "op.gg at" returned only unrelated entries and OP.GG profile pages. — [search result example: Ekşi entry](https://eksisozluk.com/entry/22224439) (irrelevant hit, cited to show what came back)

### Inferences
- **Even without an "always", the link is worth adding.** Checking a stranger's stats before risking LP for four people is common advice. The cost of a one-tap link is close to zero, so the recommendation does not depend on proving the "always".
- **The Flex-rank caveat strengthens the case for a link.** If Flex rank is a weak signal, hosts will want to see the OP.GG page (Solo/Duo rank, recent games, champion pool), not only a rank number, verified or not. A link carries more information than an API-fetched rank badge alone.

### Gaps
- **No primary TR evidence.** No Reddit, Ekşi, Technopat or Donanımhaber threads quantify how often TR 4-stacks check OP.GG.
  - The fastest way to close this is first-party: log OP.GG link clicks after shipping the link, or ask 5–10 PreBot hosts.

## Are claims A and B correct, and what exactly should change?

### Takeaway
**Claim A: mostly confirmed.** The only trust aid in the UI is a copy button plus a hint to look up the ID "op.gg gibi bir sitede". Ranks are self-declared, and a deep link is the cheapest fix. "Always checks" is unproven, and "most effective" is overstated: a link speeds up checking but does not stop a mis-ranked player from taking the seat first.
**Claim B: confirmed in substance, with one correction.** The host *can* effectively choose today by pressing "Gelmedi", but only through a mislabelled button that punishes the joiner. It also always removes the latest joiner and pollutes both the reputation badge and the public statistic.
The two fixes are coupled. Shipping the OP.GG link **without** the separate "Uygun değil" button would increase unfair no-show marks, because hosts would screen more and the only way to act on a bad screen is "Gelmedi".

### Cited Findings
- **The hint text (code).** The host-facing hint reads: "Ranklar oyuncuların kendi beyanı, doğrulanmıyor. Emin olmak istersen Riot ID'sine op.gg gibi bir sitede bakabilirsin." — [ListingPage.razor:143](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Web\Components\Pages\ListingPage.razor)
- **The Riot ID badge (code).** The badge is a copy-only button ("Kopyala: oyunda Arkadaş ekle'ye yapıştır") with no external link. — [RiotIdBadge.razor](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Web\Components\Shared\RiotIdBadge.razor)
- **"Gelmedi" behaviour (code).** It releases the latest seat as NoShow, which feeds the per-user badge and the public statistic (see previous sections). — [PreService.cs](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Core\Services\PreService.cs); [ModerationService.cs](C:\Users\Alper\OneDrive\Masaüstü\LoL Pre Bot Projesi\src\PreBot.Core\Services\ModerationService.cs)
- **Verified ranks need a production key.** Personal and development keys can't run public products, so any automatic rank verification requires one. — [Riot Developer Portal docs](https://developer.riotgames.com/docs/portal)
- **Rejections are costly, filters cut them.** Rejected users are much less likely to convert, and filters reduce rejections substantially. — [MIT IDE Research Brief (Fradkin)](https://ide.mit.edu/wp-content/uploads/2017/07/IDE-Research-Brief_0717.pdf)

### Inferences
- **Concrete changes, in order:**
  1. **OP.GG link next to every Riot ID** on both the web and the Discord embed, for all participants. The joiner also risks LP and may want to check the group.
     - Format: `https://op.gg/lol/summoners/tr/{EscapeDataString(name)}-{EscapeDataString(tag)}`, split on the last `#`, case preserved.
     - Replace the hint text with something like "Ranklar beyan; OP.GG'ye tek dokunuşla bak."
     - Keep the copy button, which is still needed for the in-client invite.
  2. **Ship the "Uygun değil, sıradaki" button in the same release as the link**, per joiner.
     - Reason `HostDeclined`, excluded from no-show counts and from "katılıp gelmeyen %".
     - Declined player: no re-notification for that listing, and a neutral message.
     - Per-listing cap, plus host decline-rate tracking.
  3. **Make "Gelmedi" per joiner and time-gated.** It stays the only action that counts as a no-show.
  4. **Apply for a Riot production key now**, since approval time is unknown and a prototype exists. Then:
     - Show a Riot-sourced Flex and Solo/Duo rank tagged "doğrulandı", replacing self-declared ranks.
     - Optionally let listings set an enforced minimum or maximum rank (pre-filter, which cuts screening rejections).
  5. **Later, and optional:** icon-change or RSO ownership proof, and a per-listing "Önce onayla" (application) mode for quality-first hosts.
- **What not to do.** Do not switch the default to request-to-join. The evidence favours instant join for speed. The problem is the missing **non-punitive veto**, not first-come-first-served itself.

### Gaps
- **Reaction to the veto mechanics.** Not tested with real TR users: how quickly hosts actually decide, and whether a 2–3 minute veto window fits the client friend-add/invite flow.
- **Production-key approval for an LFG product.** Unknown whether Riot would approve one that shows ranks. Policy allows showing official ranks, but approval is discretionary and timelines are unpublished.
