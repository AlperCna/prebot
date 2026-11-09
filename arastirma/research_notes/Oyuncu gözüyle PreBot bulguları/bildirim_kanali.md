# Notification channel for Turkish LoL players: testing the claim "the channel is in the wrong place"

Claim under test: "The channel is in the wrong place. The player is at the PC with Discord open. A web push to the phone is secondary. On iPhone you first need 'Add to Home Screen', which is serious friction. The Discord bridge (DM notifications, /pre-lazim) is built and tested but turned off in the trial. You are not using your strongest card."

Research date: 9 November 2026. Project context, from the repo README: the trial runs in "broadcast mode" on the developer's own home PC in Turkey behind a Cloudflare Quick Tunnel, and the Discord bot is off (`Discord:Enabled=false`). The earlier internal report `reports/Web uygulaması mı Discord mu.md` already chose "PWA core + Discord as optional feeder" because of the block.

## 1. Discord in Turkey: block status, access, usage, legal risk, effect on bots and DMs

### Takeaway
Discord has been blocked in Turkey since 9 October 2024 and is **still blocked as of 9 November 2026**. A June 2026 ministerial hint that it will "probably" reopen has not produced a court decision or a date. Turkish players can only reach it through a VPN, an anti-DPI tool (GoodbyeDPI Turkey build, WARP, SplitWire) or, anecdotally, certain DNS setups. No source measures how many Turkish gamers still use it. So "the player has Discord open" holds only for a self-selected, technically motivated core. It does not hold for the average new PreBot user.

### Cited Findings
- **Legal basis:** Discord was blocked under Article 8 of Law No. 5651 by the Ankara 1st Criminal Judgeship of Peace, decision dated 9 October 2024, no. 2024/12907. — [Sözcü, 5 Jun 2026](https://www.sozcu.com.tr/bakan-uraloglu-acikladi-discord-bir-sure-sonra-erisime-acilacak-p325599)
- **June 2026 minister's statement:** Transport Minister Uraloğlu said (TGRT interview) "Mahkeme kararlarıyla Roblox ve Discord'u kapattık". He added that Discord "içerik çıkarma noktasında artık Türkiye'nin istediği kriterlere gelmiş durumda" and "Muhtemelen bir müddet sonra açacağız". No date was given. — [Sözcü, 5 Jun 2026](https://www.sozcu.com.tr/bakan-uraloglu-acikladi-discord-bir-sure-sonra-erisime-acilacak-p325599)
- **Current block status:** daily tests show discord.com blocked on Türk Telekom and Turkcell Superonline home lines. The last confirmed block was **9 Nov 2026, 09:31 UTC**, with daily testing since 29 Sep 2026. An independent OONI test on Superonline on 7 Oct 2026 also found it blocked. No decision lifting the block has been published. The page notes its test covers only the website on home broadband, and that "the app, calls and mobile networks can behave differently". — [vpnAlert censorship monitor](https://vpnalert.com/tools/censorship-monitor/turkey/discord/)
- Turkish news in October 2026 still reports the block in place with no reopening date. — [Politikam, Oct 2026](https://www.politikam.com/discord-ve-roblox-turkiyede-acildi-mi-ekim-2026-itibariyla-erisim-engeli-hala-devam-ediyor-tarih-bekleniyor)
- **Data-sharing reason:** the minister said Discord had refused to share user data such as IP addresses with Turkish security units. — [vpnAlert](https://vpnalert.com/tools/censorship-monitor/turkey/discord/). Earlier coverage reported that Turkish authorities could not adequately access user information without the company's cooperation. — [CyberInsider](https://cyberinsider.com/discord-blocked-in-russia-and-turkey-amid-government-crackdowns/)
- **VPN websites blocked too:** the sites of nordvpn.com and surfshark.com fail on some Turkish networks. ExpressVPN, NordVPN, Proton VPN, Psiphon and Surfshark are listed as blocked, and installing a VPN before you need it is advised. — [vpnAlert](https://vpnalert.com/tools/censorship-monitor/turkey/discord/)
- **VPN legality:** using a VPN is legal in Turkey and no law penalizes users. What gets blocked is VPN company websites: 8 VPN sites failed to load in tests on 10 Sep and 1 Oct 2026. — [Nomad Istanbul, tested Oct 2026](https://nomadistanbul.com/kb/what-is-blocked-in-turkey-vpn/) (search-result summary; page not fetched in full)
- **Blocking method:** reportedly DNS plus SNI inspection, so changing DNS alone usually fails while obfuscated VPNs work. — [Gizmodo VPN guide](https://gizmodo.com/best-vpn/unblock-discord-turkey). This is a VPN-affiliate page, so treat it with caution. vpnAlert itself only says "the connection is cut".
- **Workarounds reported by users:** the mainline GoodbyeDPI does not work and a "GoodbyeDPI Turkey" build is needed. Results vary by ISP, and even by Ethernet vs Wi-Fi on Türk Telekom. WARP and SplitWire are suggested alternatives, with complaints about latency. Some users report the browser working while the desktop app does not. — [Technopat forum threads](https://www.technopat.net/sosyal/konu/goodbyedpi-calismiyor.3628631/)
- Some users say they reach Discord without a VPN, possibly because of their ISP's DNS, while others in the same thread say it is still blocked. — [Samsung Community TR](https://r2.community.samsung.com/t5/Di%C4%9Fer/Discord/td-p/18918945) (anecdote)
- **Users at the time of the ban:** affected users said they "just lost their entire community" (Oct 2024). — [TechRadar](https://techradar.com/pro/vpn/discord-is-banned-in-turkey-and-russia-with-users-claiming-they-just-lost-their-entire-community)
- **Discord's importance for League:** globally, League and VALORANT are among the most-played games on Discord. In 2026 Riot and Discord shipped account linking: Discord friends appear in the League friend list, party invites can be sent from Discord, and party links can be shared. These features are available only "in regions where Discord-powered features are available". — [Discord blog, 21 May 2026](https://discord.com/blog/making-it-easier-than-ever-to-connect-with-friends-in-league-val)
- **How invites arrive:** with the integration, party invites and "Ask to Join" requests arrive as messages in the Discord client with live party status. The Riot article lists no regions. — [Riot Support, 7 May 2026](https://support.riotgames.com/league-of-legends/gameplay/league-of-legends-discord-account-linking)

### Inferences
- **The block cuts both ways.**
  - (a) It makes Discord a poor *primary* channel for onboarding Turkish players. A new user cannot simply "install Discord and click the bot". VPN sites themselves are blocked, and anti-DPI tools are fragile and differ by ISP.
  - (b) The players who *do* still run Discord through a bypass are likely the most engaged, PC-centric gamers. That is plausibly the same group most likely to answer a Flex call. For them, a Discord DM may indeed be the fastest signal.
  - (c) A Discord DM reaches a Turkish user's PC only while their bypass is active and their client is connected. If the VPN or GoodbyeDPI is off at that moment, the DM sits undelivered on the PC.
- **Bot hosting:** blocking is done at Turkish ISPs, so a bot hosted outside Turkey (e.g. an EU VPS, which the project's server-alternatives report already plans) should connect to Discord's gateway normally. A bot run from the **current home setup in Turkey** would itself need a tunnel or VPN to reach Discord. This is untested. vpnAlert only tests discord.com, not the gateway or API hosts.
- **Phone side:** Discord's mobile push presumably travels via Apple and Google push services, which are not blocked. A phone might therefore show a DM notification even when the Discord app cannot connect. This is speculation and was not verified.
- **Reopening:** the June 2026 statement makes a reopening plausible at some point. A Discord channel built and kept behind a feature flag is the right hedge. Turning it on as *primary* would be premature while no court decision exists.

### Gaps
- No survey, traffic estimate or Discord statement quantifies Turkish Discord usage after the ban. VPN-routed traffic is invisible to BTK statistics, and searches in English and Turkish found nothing reliable.
- No source addresses legal risk for a developer whose free app relies on Discord or runs a Discord bot for Turkish users. No penalty for ordinary users was found, but this is not legal advice.
- Whether `gateway.discord.gg` and the API and CDN hosts are blocked the same way as discord.com from Turkish connections is not documented.
- Whether the Riot–Discord linking feature works for TR-server accounts, and whether Turkey counts as a "Discord-powered features" region, is not stated by Riot or Discord.

## 2. Discord bot DMs: reliability and limits

### Takeaway
Bot DMs are not guaranteed to land, and Discord's own docs warn against bulk DMing.
- **Delivery:** a DM succeeds only if the bot shares a server with the user, the user has not blocked the bot, and the user allows DMs from server members. There is no way to check this in advance.
- **Bulk DMs:** Discord says DMs "should generally be initiated by a user action". A bot that opens many DMs quickly "may be rate limited or blocked from opening new ones".

For PreBot's wave-based fan-out to a small opted-in pool this is manageable, but it is not a "strongest card" without caveats.

### Cited Findings
- From Discord's Create DM documentation:
  - "You should not use this endpoint to DM everyone in a server about something."
  - "DMs should generally be initiated by a user action."
  - "If you open a significant amount of DMs too quickly, your bot may be rate limited or blocked from opening new ones."
  - Source: [Discord Developer Docs – User resource (Create DM)](https://docs.discord.com/developers/resources/user)
- **Error 50007 ("Cannot send messages to this user"):** this usually means the recipient blocked the bot, shares no server with it, or has "Allow direct messages from server members" turned off. The API offers no way to check in advance whether a DM will succeed, so the failure can't be prevented, only handled. — [discord.js guide – errors](https://next.discordjs.guide/guide/legacy/popular-topics/errors); [Latenode community thread](https://community.latenode.com/t/why-cant-my-discord-bot-send-direct-messages-to-users/14023)
- The common recommended pattern is to catch the failure and fall back to a server-channel message. — [Latenode community thread](https://community.latenode.com/t/unable-to-send-private-messages-from-discord-bot-to-user/14746)
- Rate-limited requests return HTTP 429, a separate code from 50007. — search-result summary citing community sources. No authoritative per-DM rate number was found.

### Inferences
- PreBot's DMs go only to players who explicitly marked themselves "available". That fits "initiated by a user action" better than a server-wide blast. The wave design (a few players per wave) also keeps DM-open rates low. At hobby scale the documented risk is low, though not zero.
- Each DM failure (50007) should trigger a fallback to web push for that user, or a channel ping. A DM alone is not a reliable single channel.
- Privileged intents and verification at 100+ servers are probably irrelevant for a single-community bot. DMs and slash commands such as /pre-lazim need no privileged intent. This was not re-verified against current docs in this pass.

### Gaps
- Discord publishes no exact threshold for "too many DMs too quickly", and no current source on its anti-spam heuristics for bots was found.
- The current default of the "Allow DMs from server members" setting, and the share of users who turn it off, were not found.

## 3. Web push: iOS Home Screen requirement, iPhone share in Turkey, Android reliability, desktop with browser closed

### Takeaway
The iPhone friction is real but limited to roughly a quarter of Turkish phones. Web push on iOS still requires adding the site to the Home Screen (iOS 16.4+). iOS 26 only changed it so any site added to the Home Screen opens as a web app by default. On Android (about 74–77% of Turkish mobile traffic), web push needs no install step and can wake a sleeping device if the server sends `Urgency: high`. On a Windows PC, web push works as long as the browser process is alive, which Edge and Chrome background modes provide.

### Cited Findings
- **Turkey mobile OS share (StatCounter):** October 2026 was Android 76.78% and iOS 23.22%. Across May–July 2026, iOS ranged about 24–26% (July: Android 74.18% / iOS 25.81%). — [StatCounter – Mobile OS, Turkey](https://gs.statcounter.com/os-market-share/mobile/turkey) (figures from the search-tool summary of the StatCounter pages)
- **iOS requirement:** web push for web apps on iOS and iPadOS 16.4+ works only for web apps added to the Home Screen. A site open in a Safari tab cannot subscribe, and the permission prompt requires a user gesture. — [WebKit blog, "Web Push for Web Apps on iOS and iPadOS"](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)
- **iOS 26 change:** in Safari 26 (iOS/iPadOS 26), "every site can be a web app". The Add to Home Screen sheet has an "Open as Web App" switch that is on by default, so even sites without a manifest open as web apps. The Add to Home Screen step itself remains. — [WebKit – Features in Safari 26.0](https://webkit.org/blog/17333/webkit-features-in-safari-26-0/); [iDownloadBlog](https://www.idownloadblog.com/?p=1042117); [heise](https://heise.de/-10749652)
- **Android wake-up:** the Android OS listens for push messages and wakes the right app "regardless of whether the app is closed", and browsers are woken the same way. — [web.dev – Push notifications FAQ](https://web.dev/articles/push-notifications-faq)
- **Message priority:** in Doze mode, normal-priority delivery may be delayed until the device wakes. High-priority messages are attempted immediately and can wake a sleeping device. Overusing high priority can cause messages to be deprioritized. For web push, priority is set with the `Urgency` header. — [Firebase – Set message priority](https://firebase.google.com/docs/cloud-messaging/customize-messages/setting-message-priority); [Pushpad – Urgency header](https://pushpad.xyz/blog/web-push-wake-up-a-phone-from-deep-sleep-with-urgent-notifications)
- **Old Doze report:** a 2017 Chromium thread (Chrome 61, Android 8.0) reported high-priority pushes not waking a dozing phone. It is dated and does not establish current behavior. — [Chromium push-notifications-dev](https://groups.google.com/a/chromium.org/g/push-notifications-dev/c/gWsM4Hg2JZE)
- **Desktop:** on desktop, push is received while the browser is running. "The only time a push won't be received is when the browser is completely closed." On Windows "it's a little trickier to determine whether or not Chrome is running". — [web.dev – Push notifications FAQ](https://web.dev/articles/push-notifications-faq)
  - Chrome has "Continue running background apps when Google Chrome is closed". — [Windows Report](https://windowsreport.com/continue-running-background-apps-when-google-chrome-is-closed/)
  - Edge's Startup Boost keeps core Edge processes running in the background after the browser closes. — [Microsoft Support – Startup boost](https://support.microsoft.com/en-us/edge/get-help-with-startup-boost)
  - A vendor guide reports receiving PWA push via Edge even with all Edge windows closed. — [3CX](https://www.3cx.com/blog/docs/pwa-push-notifications/) (vendor claim)

### Inferences
- "Serious friction on iPhone" is correct for the iPhone segment but over-weighted as a general argument. It affects about 1 in 4 Turkish phones, and a desktop PC needs no install step at all.
- "Phone push is secondary" is half-right. When the player is at the PC, the PC browser toast matters more. But a player marked "available" may be away from the desk (kitchen, couch). In a 15-minute, first-tap-wins window, the phone then becomes the *only* reachable surface, and it needs no VPN.
- The server should send push with `Urgency: high` and a short TTL of about 15 minutes, matching the listing lifetime. Both are cheap reliability wins on Android.

### Gaps
- No current (2025–2026) measurement of Android Chrome web push latency under OEM battery managers (Xiaomi/MIUI-HyperOS, Samsung "sleeping apps", Huawei) was found. These brands are common in Turkey, but no TR-specific share was gathered here.
- Whether Chrome's "continue running background apps" setting is on by default on current Windows builds was not verified from an official source.
- EU DMA effects were not sourced in this pass. From background knowledge (unverified here), Apple's early-2024 plan to drop Home Screen web apps applied only to the EU and was reversed. Turkey is not in the EU, so it should be irrelevant, but this needs a source before use.

## 4. Windows notifications while gaming, and Discord's in-game overlay for League

### Takeaway
Windows 11 can automatically enable Do Not Disturb "when playing a game" and "when using an app in full-screen mode". When it is on, only allowed priority apps show banners. Everything else, including browser web push *and* the Discord desktop app's Windows notifications, goes silently to the notification center unless the user adds that app as a priority exception. So the claim's implicit premise, "at the PC = sees Discord", is not automatically true while League is running. Discord's in-game overlay could bypass this, but no source confirms it works in League of Legends in 2025–2026.

### Cited Findings
- **Automatic DND conditions:** under Settings > System > Notifications > "Turn on do not disturb automatically", the conditions include "When playing a game", "When using an app in full-screen mode", "When duplicating the display", "For the first hour after a Windows feature update" and a time schedule. — [Microsoft Support – Notifications and Do Not Disturb](https://support.microsoft.com/windows/8942c744-6198-fe56-4639-34320cf9444e)
- **What DND lets through:** with DND on, "you will only receive banners for alarms, reminders, and apps of your choice". Other notifications "are sent directly to the notification center". Users can add apps that are allowed to break through via "Add apps". — [Microsoft Support](https://support.microsoft.com/windows/8942c744-6198-fe56-4639-34320cf9444e)
- **Game detection:** detection works best with full-screen games, and windowed games may not trigger the rule. — [GeekRewind](https://geekrewind.com/how-to-silence-notifications-automatically-with-do-not-disturb-in-windows-11/) (third-party guide)
- **Discord's March 2025 overlay rebuild:** the new overlay no longer hooks directly into games, the hooking method that can trip anti-cheat. Discord said this should bring the overlay to "a larger portion of the most-played games". No game list was given. — [PC Gamer, Mar 2025](https://pcgamer.com/software/discord-drops-big-update-with-completely-new-in-game-overlay-and-new-dark-themes-for-the-desktop-client); [TechCrunch, 25 Mar 2025](https://techcrunch.com/2025/03/25/discord-made-its-streaming-overlay-a-lot-more-user-friendly/)
- **Discord support pages:**
  - The known-issue article says the overlay is supported "only in a limited number of games due to compatibility challenges", and that the new overlay is expected to be "compatible with nearly all games". — [Discord Support – Known Issue: Game Overlay](https://support.discord.com/hc/en-us/articles/25289844838551--Known-Issue-Game-Overlay) (from search snippet; the page returned 403 to the fetcher)
  - An older page recommends borderless fullscreen to keep the overlay. — [Discord Support – Games Overlay 101](https://support.discord.com/hc/hi-in/articles/217659737-Games-Overlay-101)
- **Vanguard:** a League broadcast tool notes its memory-reading approach is "not possible any more due to Vanguard Anti Cheat". That is a different technique from Discord's overlay, so it shows Vanguard's strictness but does not settle the overlay question. — [LeagueBroadcast GitHub](https://www.github.com/floh22/LeagueBroadcast)

### Inferences
- An "available" fifth player is usually not mid-match. If they were, they could not join within 15 minutes anyway. They are more likely in the League client, on another app, or away. The League client is a normal window, and in that state toasts from both Chrome/Edge and Discord appear normally. Suppression by game-DND matters mostly for players finishing a game or playing something else full-screen.
- Under game-DND, web push and Discord desktop notifications are equally exposed. The mitigation is the same for both: in onboarding, tell PC users to add Edge/Chrome (or Discord) to the DND priority list. A browser-based alert has no structural disadvantage here.

### Gaps
- No Discord or Riot statement confirms or denies overlay support for League of Legends under Vanguard in 2025–2026, and no recent first-hand test report was found.
- Whether League's default display mode (borderless vs fullscreen) triggers Windows' "when playing a game" detection was not documented.
- Whether the Discord desktop app on Windows renders its notifications as Windows toasts (and so is subject to DND) or through its own popup could not be confirmed from a primary source.

## 5. Other channels Turkish gamers use for "come play now" pings

### Takeaway
WhatsApp is the near-universal messaging layer in Turkey: 90% of internet users aged 16–74 used it in 2026 (TÜİK). Instagram is also high at 71.1%. Telegram is reported to be losing users. No data was found on which channel Turkish gamers specifically use for ad-hoc "come play" pings. Outside Discord, the in-game League friend list and party invites remain the native, unblocked channel.

### Cited Findings
- **TÜİK 2026 Household ICT Usage Survey:** among internet users aged 16–74, WhatsApp 90.0% (men 92.5%, women 87.6%), YouTube 77.6%, Instagram 71.1%. Internet use rose from 90.9% (2025) to 92.3% (2026). — [AA, 5 Aug 2026](https://aa.com.tr/en/turkiye/turkiyes-internet-usage-rate-rises-to-923-in-2026/4019224); [BirGün, 5 Aug 2026](https://www.birgun.net/haber/whatsapp-zirvede-tuik-internet-kullanim-verilerini-acikladi-727545)
- In the same TÜİK release, YouTube, Instagram and TikTok grew while Facebook, X, Telegram and Snapchat lost users. — [Teknoblog](https://www.teknoblog.com/turkiye-de-internet-kullanimi-yuzde-923-oldu/) (search-result summary; page not fetched; no Telegram percentage given)
- Kepios/DataReportal counts 62.3 million active social media user identities in Turkey in 2026. — [DataReportal – Digital 2026: Turkey](https://datareportal.com/reports/digital-2026-turkey)
- BTK traffic data reportedly shows WhatsApp at about 61.2% of VoIP traffic and Instagram leading messaging traffic. — [Daily Sabah](https://www.dailysabah.com/turkiye/youtube-accounts-for-40-of-turkiyes-internet-traffic/news) (search snippet only, period unclear; traffic share is not user reach)

### Inferences
- WhatsApp is where a Turkish player is reachable with no VPN, no install and on any phone. Automated WhatsApp pings would need the paid WhatsApp Business Platform plus template approval (not researched here), so for a free hobby project WhatsApp works as a *share-link* channel: a group admin posts the listing link. It is not a per-user push channel.
- The League client friend list and party invite is the zero-friction "ping" for people already on each other's friend lists. PreBot's value is reaching people who are *not* yet friends, which no native channel does.

### Gaps
- No survey on Turkish gamers' preferred coordination channel (WhatsApp groups vs Discord-over-VPN vs Instagram DMs vs in-game chat) was found.
- No TÜİK Telegram or Discord percentages were found in the reporting.

## 6. Conclusion: channel order for a 15-minute, first-tap-wins window

### Takeaway
The claim is right that a PC-side alert matters most and that iPhone install friction is real. It is wrong to call Discord the "strongest card" for Turkey in November 2026: the platform is still blocked, so it reaches only bypass users and only while their bypass is running. It also understates phone push, which is the only VPN-free channel when the player is away from the desk. The best order is **parallel fan-out**, not a ranking:
1. Web push to every registered device: PC browser plus phone.
2. A Discord DM for players who opt in to it, once the bot runs from a non-Turkish host.
3. Share links for WhatsApp groups and Discord servers.

Two fixes matter more than switching channels: (a) onboarding that adds the browser to Windows DND exceptions, and (b) iPhone Add-to-Home-Screen guidance.

### Cited Findings
- Discord still blocked as of 9 Nov 2026. — [vpnAlert](https://vpnalert.com/tools/censorship-monitor/turkey/discord/)
- iOS web push needs a Home Screen web app. — [WebKit](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)
- iPhone is about 23% of Turkish mobile traffic (Oct 2026). — [StatCounter](https://gs.statcounter.com/os-market-share/mobile/turkey)
- Windows game/full-screen DND sends non-priority app notifications silently to the notification center. — [Microsoft Support](https://support.microsoft.com/windows/8942c744-6198-fe56-4639-34320cf9444e)
- Discord warns against high-rate DM opening. — [Discord Docs](https://docs.discord.com/developers/resources/user)
- WhatsApp is used by 90% of Turkish internet users. — [AA/TÜİK](https://aa.com.tr/en/turkiye/turkiyes-internet-usage-rate-rises-to-923-in-2026/4019224)

### Inferences
- **Claim, point by point:**
  - "Player is at the PC with Discord open": true only for VPN/anti-DPI users. Unknown share, likely a minority of new users. Even then, Discord desktop toasts face the same Windows DND as browser toasts.
  - "Web push to the phone is secondary": partly true when at the desk. False when away, and phone push is the only VPN-free mobile channel.
  - "iPhone friction is serious": true for about 23–26% of phones. iOS 26 makes the result a proper web app by default but keeps the Add-to-Home-Screen step.
  - "Discord bridge off = not using the strongest card": wrong as stated for Turkey today, but there is a valid core. For the subset already running Discord via a bypass, a DM is a strong *additional* channel and costs nothing extra to send in parallel. It should be enabled as opt-in once the bot runs from a host outside Turkey. The current home host in Turkey is itself behind the block.
- **Suggested delivery logic** (design inference, not tested):
  - At listing creation, send web push with `Urgency: high`, TTL equal to the listing lifetime, and an action button to every device of each wave's players.
  - Simultaneously send a Discord DM to players who linked Discord and opted in. On a 50007 failure, rely on push only.
  - Show a copyable share link for WhatsApp groups and Discord servers.
  - Do not chain channels sequentially, because each fallback delay eats into the 15-minute window.
- **Onboarding fixes with high return:**
  - On PC, check that the browser is allowed in Windows DND priority, and keep Edge/Chrome running in the background.
  - On iPhone, a one-screen Add-to-Home-Screen guide.
  - On Android, nothing.
- **If Discord is unblocked:** re-rank Discord DMs upward. The feature flag means this is a configuration change, not new work.

### Gaps
- No real-world latency or tap-through data compares web push vs Discord DM for Turkish players. Only the PreBot trial itself could produce it, for example by logging the time from notification to first tap per channel.
- No data on the share of PreBot's actual target users who run Discord via a bypass. A one-question poll at sign-up ("Do you use Discord? Over VPN/DPI?") would close this.
