# Delivery Channel Comparison for an On-Demand "Need a 5th Right Now" LoL Flex Teammate Finder (TR server, as of Nov 2026)

Labels used below:
- **[VERIFIED]**: primary or official source (vendor docs, Riot or Discord docs and support pages, app store listing facts).
- **[MARKETING]**: self-reported by the product or a vendor blog with a commercial interest.
- **[SECONDARY]**: press or third-party commentary.
- **[ANECDOTAL]**: user posts.

Research covered about 18 searches and fetches. Several named products (Noobly, Plink, LoLFinder, TeamTavern, Teamplay web traction) could not be checked within budget and are listed under Gaps.

## Q1. What does evidence from existing LFG products say about which channels users actually use?

### Takeaway
The League LFG activity with real, visible scale lives inside Discord: large community servers like RiftQ plus cross-server bots like Miko/Duoo and Teamplay_LFG. Standalone apps and webs mostly report their numbers themselves and have thin independent evidence. Discord's own built-in LFG was reportedly dropped for low use, and Guilded, the Discord alternative, shut down in Dec 2025. Nobody has pulled players out of Discord for LFG at scale.

### Cited Findings
- **RiftQ (Discord-first):** The RiftQ EUW Discord server lists about 65k members and the EUNE server about 52k [VERIFIED listing counts]. It reportedly grew out of a large EUW Discord server and later added a web platform for duo/team finding [SECONDARY]. — [RiftQ EUW Discord listing](https://discord.com/servers/riftq-league-of-legends-euw-1161742778510880798); [RiftQ EUNE Discord listing](https://discord.com/servers/riftq-league-of-legends-eune-488037455727951883); [ExitLag blog](https://www.exitlag.com/blog/lfg-league-of-legends/)
- **Duoo.gg / Miko bot:**
  - The bot shares duo requests across Duoo-connected servers, verifies account ownership and assigns rank roles [VERIFIED listing features].
  - It claims "over 20,000 Discord users connected" [MARKETING].
  - — [Miko by Duoo.gg on top.gg](https://top.gg/bot/1096340606235717642)
  - A PSU roundup ranks Miko as the top LFG bot for League [SECONDARY]. — [PSU: 7 best LFG Discord bots](https://www.psu.com/news/victory-starts-here-7-best-lfg-discord-bots/)
- **Teamplay.gg (web + app + bot):**
  - It claims 23,000+ players [MARKETING]; its Discord has about 6.8k members.
  - The Teamplay_LFG bot is listed in about 289 servers (undated snapshot). Android app exists; iOS was "coming soon" at the time of the listing.
  - No independent usage data was found, and the bot had about one review on top.gg.
  - — [Teamplay.gg Discord server](https://discord.com/servers/teamplay-gg-1042150011422658750); [Teamplay_LFG on discordbotlist](https://discordbotlist.com/bots/teamplaylfg); [Teamplay Discord LFG bot page](https://teamplay.gg/discord-lfg-bot)
  - The 289-server figure is small next to the claimed 23k players, which suggests the bot is a secondary channel.
- **GameTree (native app-first):**
  - Self-reported user counts disagree: the League page says "1M+ users"; the App Store listing says "365,000+ gamers"; Google Play data via AppGoblin says "1.5M+" [MARKETING, inconsistent].
  - The app uses weekly and monthly subscriptions. App Store reviews are sparse and mixed, including crash complaints [ANECDOTAL].
  - It raised $1.7M in Dec 2023 [SECONDARY].
  - — [GameTree LoL LFG page](https://gametree.me/league-of-legends-lfg/); [GameTree App Store](https://apps.apple.com/us/app/gametree-find-gaming-friends/id1181404496); [AppGoblin](https://appgoblin.info/apps/com.gametreeapp/keywords); [AIN.ua funding](https://en.ain.ua/2023/12/05/ukrainian-founded-gametree-raises-1-7m)
  - GameTree's model is social and friend-finding (profiles, matching algorithm), not instant "seat open now" dispatch.
- **Discord's own built-in LFG:** A community support post says it "never made it past beta" and was not implemented "because it wasn't used very much" [ANECDOTAL, not an official Discord statement]. — [Discord support community post](https://support.discord.com/hc/en-us/community/posts/1500000313161-Where-is-Discord-s-LFG-Please-bring-it-back)
- **Publisher-run LFG inside Discord:** Arc Raiders added an "experimental" LFG channel to its official Discord, with tags such as Trios and mic required [SECONDARY]. — [Arc Raiders LFG channel news](https://www.arcraidersiran.com/en/news/arc-raiders-adds-lfg-channel-to-discord)
- **Guilded shutdown:**
  - Roblox announced the sunset of Guilded (a gaming-focused Discord competitor) in late Sept 2025 [VERIFIED].
  - It shut down on Dec 19, 2025, and its desktop and mobile apps then showed only a "Sunset" notice [SECONDARY].
  - — [Roblox DevForum: Update on Guilded and Communities](https://devforum.roblox.com/t/update-on-guilded-and-communities/3966775); [AlternativeTo](https://alternativeto.net/news/2025/11/guilded-to-shut-down-on-december-19-2025-as-focus-shifts-to-roblox-communities/); [Wikipedia: Guilded](https://en.wikipedia.org/wiki/Guilded)
- **Where people search:** Discord is described as the dominant LoL LFG hub. The official LoL Discord has region- and role-specific LFG channels. OP.gg and Tracker.gg have web LFG boards. [SECONDARY; vendor blogs] — [ExitLag LoL LFG](https://www.exitlag.com/blog/lfg-league-of-legends-lfg/); [parse.gl ranking](https://www.parse.gl/rankings/social-network/esports-team-and-player-management-platforms); [Official LoL Discord listing](https://discord.com/servers/league-of-legends-417825368062558219)
- **Bots are the common pattern:** Third-party bots fill the LFG gap with one-click flows, for example LFG Hub's /lfg and /create-group, and Teamplay's guided wizard [MARKETING]. — [LFG Hub](https://lfghub.gg/); [Teamplay Discord LFG bot](https://teamplay.gg/discord-lfg-bot)

### Inferences
- The products with the most visible League traction (RiftQ, Duoo/Miko) are Discord-native or Discord-first. They add a web profile or board layer rather than replacing Discord.
- Native-app-first products (GameTree) show big self-reported numbers but serve a different job: finding long-term friends, not filling a seat in real time.
- The failure modes seen here (Discord LFG beta dropped, Guilded sunset) suggest two lessons:
  - A new standalone destination struggles against Discord's network effect.
  - Even Discord could not make a generic LFG UI stick without community or server context.
- The friends' worry ("people won't type commands") is about UX, not the channel. Successful bots (Miko, LFG Hub, Teamplay) use buttons, wizards and cross-server posting rather than raw typed commands.

### Gaps
- Noobly, Plink, LoLFinder, TeamTavern and Teamplay's web vs app split: no usage data found or checked within the research budget.
- No independent (third-party analytics) usage numbers for any LoL LFG product.
- No Turkish-server-specific LFG product or Discord community sizes were found. Next step: search Discord server listings for "LoL TR" / "League of Legends Türkiye".
- No official Discord statement on why its built-in LFG beta ended; only a community post.

## Q2. How do the channels compare on friction at the "right now" moment (steps/taps to get notified and to respond; player at PC with LoL client open, phone nearby)?

### Takeaway
At the critical moment the available player is usually at their PC with the LoL client open. Discord is typically already running there, and its notifications reach both desktop and phone. Discord has the fewest steps between "group posts" and "player accepts". Web-push PWA comes second: on iOS it needs a manual home-screen install before notifications work. A plain web app needs an open tab and is worst for push. A native app has the best phone push but adds a store install and app-review overhead, and the phone isn't where the game is played.

### Cited Findings
- **iOS PWA push:**
  - Web Push on iOS/iPadOS works only for sites added to the Home Screen (iOS 16.4+), not in Safari tabs.
  - The permission prompt only works after the user installs the site and opens it from there.
  - — [OneSignal: Web push for iOS](https://documentation.onesignal.com/docs/en/web-push-for-ios); [MagicBell: PWA iOS limitations](https://www.magicbell.com/blog/pwa-ios-limitations-safari-support-complete-guide)
- **No install prompt on iOS:** iOS has no automatic install prompt (`beforeinstallprompt` is unsupported), so the user must use Share, then Add to Home Screen, manually [SECONDARY, vendor guide]. — [MagicBell guide](https://www.magicbell.com/blog/pwa-ios-limitations-safari-support-complete-guide)
- **Newer iOS changes:** iOS 18.4 added Declarative Web Push; iOS 26 opens Home Screen sites as web apps by default [SECONDARY, not checked against WebKit release notes]. — [MagicBell guide](https://www.magicbell.com/blog/pwa-ios-limitations-safari-support-complete-guide); [Edana: PWA push reliability 2026](https://edana.ch/en/2026/03/19/push-notifications-on-web-applications-pwa-is-it-really-reliable-on-ios-and-android/)
- **Missing on iOS Safari:** Background Sync and Periodic Background Sync are not supported [SECONDARY]. — [MagicBell guide](https://www.magicbell.com/blog/pwa-ios-limitations-safari-support-complete-guide)
- **Discord surfaces:**
  - Discord apps can be installed to servers, to users, or both. A user-installed app can be used in the bot's DM and private channels.
  - User-context installs support only the `applications.commands` scope by default.
  - — [Discord docs: Developing a user-installable app](https://docs.discord.com/developers/tutorials/developing-a-user-installable-app); [Discord docs: Overview of apps](https://docs.discord.com/developers/quick-start/overview-of-apps)
- **Discord Activities run everywhere:** They run "in an iframe in Discord on desktop, mobile, and web". — [Discord docs: Activities overview](https://docs.discord.com/developers/activities/overview)
- **The join step is in-game:** The last step for every channel (accepting, getting the host's Riot ID, the host invites, or friend request plus invite) happens in the LoL client. The research found no channel that can automate this without the LCU (see Q5).

### Inferences
Estimated step counts per channel. These are my own analysis, not sourced measurements.

- **Discord bot (current):**
  - Mark available: 1 slash command, or 1 button press on a persistent panel message.
  - Notified: Discord desktop toast plus mobile push, if the bot DMs or pings.
  - Respond: 1 click on an "Accept" button in the DM or channel; the bot then shows the host's Riot ID.
  - About 2 actions in total.
  - Typing commands can be largely replaced by components (buttons, select menus, modals) and a pinned "panel" message, which directly answers the friends' objection.
- **PWA with web push:**
  - First-time setup: open URL, log in, install to home screen (iOS manual), grant notification permission.
  - Each session: open the PWA, toggle "available".
  - Notified: OS push.
  - Respond: tap the notification, the PWA opens, tap Accept.
  - About 3 to 4 actions per event once installed. Setup friction is higher, and there is a known risk that iOS push stops silently.
- **Plain web app:**
  - Works only while a tab is open; the Notification API without push needs the page open.
  - The player at the PC must keep a browser tab alive next to the LoL client.
  - Worst for "notify me when I'm not looking".
- **Native mobile app:**
  - Store install, then account and push permission.
  - Most reliable phone push (FCM/APNs; general platform knowledge, not sourced here).
  - The player is at a PC, though, so they get notified on the phone, accept on the phone, then switch to the PC to add the Riot ID. That context switch adds a step compared with a desktop Discord toast.
- **Discord Activity:**
  - Launch the Activity (App Launcher or an Entry Point command), then interact in the iframe.
  - An Activity is a session UI, not a background listener; see Q4.
  - Notifications must still come from the bot side.

### Gaps
- No published usability or tap-count study comparing LFG channels was found.
- No data on what share of LoL TR players keep Discord open while playing. Run a survey or poll in a TR LoL Discord to measure it.

## Q3. How do players get notified fastest and most reliably in each channel?

### Takeaway
For PC-at-desk players, Discord bot DMs or role pings are the most practical fast channel: a single integration reaches desktop and mobile at once. Web push is good on Android and desktop Chrome/Edge but conditional and less reliable on iOS. Native push is the most reliable on phones but costs the most to build and to get installed.

### Cited Findings
- **iOS web push reliability:**
  - Only works after a Home Screen install.
  - Developers report push that "may work initially but then stop unexpectedly" [SECONDARY/ANECDOTAL].
  - A Feb 2026 Reddit snapshot reports iOS push failing after a Home Screen install [ANECDOTAL].
  - — [MagicBell guide](https://www.magicbell.com/blog/pwa-ios-limitations-safari-support-complete-guide); [OneSignal iOS web push](https://documentation.onesignal.com/docs/en/web-push-for-ios); [Reddit snapshot](https://reddit.sentinel-team.org/posts/1r7cdbs/snapshots/2026-02-18T00%3A30%3A19.469815Z)
- **Overall PWA push reliability:** A 2026 article asks whether PWA push is "really reliable on iOS and Android" and concludes it is workable with caveats on iOS [SECONDARY]. — [Edana, Mar 2026](https://edana.ch/en/2026/03/19/push-notifications-on-web-applications-pwa-is-it-really-reliable-on-ios-and-android/)
- **EU caveat:** Apple removed standalone Home Screen web apps in the EU with iOS 17.4, and push reportedly does not work for EU PWAs [SECONDARY; sources disagree on timing]. — [MagicBell guide](https://www.magicbell.com/blog/pwa-ios-limitations-safari-support-complete-guide)
  - This does not apply to Turkey, which is not in the EU.
  - My training knowledge is that Apple reversed the EU removal in March 2024; I did not verify this in this session.
- **Discord user-installed apps:**
  - Usable in DMs with the bot.
  - Discord's support page says user-installed apps "are not able to access information about your server or DMs".
  - — [Discord docs: user-installable app tutorial](https://docs.discord.com/developers/tutorials/developing-a-user-installable-app); [Discord support: Using Apps in Discord](https://support.discord.com/hc/hi-in/articles/21334461140375-Using-Apps-in-Discord)
- **Activities and notifications:** The Activities docs say nothing about Activities sending notifications. Notifications are not an Activity feature in the pages checked. — [Activities overview](https://docs.discord.com/developers/activities/overview); [How Activities Work](https://docs.discord.com/developers/activities/how-activities-work)

### Inferences
Notification speed and reliability ranking for this use case. This is my synthesis, not measured.

1. **Discord bot DM or role ping in a dedicated LFG channel:** Desktop toast arrives right where the LoL player sits, plus a mobile push. A single "Accept" button enables a first-come race. Discord delivers the notifications for you.
2. **Native app push (FCM/APNs):** Most reliable on the phone, but it notifies the device the player isn't playing on.
3. **PWA web push:** Good on Android and desktop Chromium. On iOS it is install-gated and can be flaky. A desktop browser must be running for desktop web push to show (general platform behavior, not sourced here).
4. **Plain web app:** Works only with an open tab, or with email/SMS fallbacks, which are too slow for "right now".

- A good fallback chain for a hybrid: Discord DM first, then web push, so a player who has muted Discord is still reached.

### Gaps
- No official Discord doc was fetched confirming the exact rules for bot-initiated DMs. My understanding, not verified here: the user must share a server with the bot or have the app user-installed, and must not have blocked DMs. Check the [Discord API docs](https://docs.discord.com/developers/resources/user) and rate limits before relying on DMs at scale.
- No measured end-to-end latency data (Discord DM vs web push vs FCM) was found.
- Discord rate limits on mass DMs were not researched. A broadcast to many available players at once could hit them; a role ping in a channel may scale better.

## Q4. Discord Activities / Embedded App SDK and user-installable apps: capabilities in 2026, fit for an LFG flow, notifications, limitations, approval

### Takeaway
Activities are web apps (iframe plus Embedded App SDK) that run inside Discord on desktop, mobile and web. Since Sept 2024 any developer can build one. Public discovery requires app verification, and listing is not guaranteed. They suit a richer in-Discord UI (an LFG "lobby board" with filters) but are foreground sessions with no documented notification ability. The bot (interactions, DMs, pings) must still do the notifying. User-installable apps let players use the bot from any DM or server, which lowers adoption friction.

### Cited Findings
- **What Activities are:** Activities are "web apps that run inside an iframe" and use the Embedded App SDK. They run on desktop, mobile and web. SDK commands handle authorization and authentication, Activity and user info, and updates. Events cover connection, errors, user updates and Activity instance events, and both depend on OAuth scopes. [VERIFIED] — [Discord docs: Activities overview](https://docs.discord.com/developers/activities/overview)
- **How Activities launch** [VERIFIED] — [Discord docs: How Activities Work](https://docs.discord.com/developers/activities/how-activities-work)
  - From the App Launcher via the app's Entry Point command. A default "Launch" command is created when Activities are enabled.
  - Or by any command, button or modal interaction responding with callback type `LAUNCH_ACTIVITY` (12).
  - So the existing bot could add a "Open LFG board" button that launches the Activity.
- **No notifications documented:** Neither the Activities overview nor the "How Activities Work" page mentions notifications or invite flows. — [Activities overview](https://docs.discord.com/developers/activities/overview); [How Activities Work](https://docs.discord.com/developers/activities/how-activities-work)
- **Open to all developers:** Discord opened Activities to all developers in Sept 2024 and added monetization options (SKUs, analytics) [SECONDARY]. — [PocketGamer.biz](https://www.pocketgamer.biz/discord-activities-opens-up-to-all-developers-to-make-games-on-the-platform); [Gigazine](https://gigazine.net/gsc_news/en/20240927-discord-new-monetization-feature)
- **Verification and discovery** [VERIFIED] — [Discord docs: Enabling Discovery](https://docs.discord.com/developers/discovery/enabling-discovery); [Discord dev support: How can my friends play my Activity](https://support-dev.discord.com/hc/articles/21204493235991-How-Can-My-Friends-Play-My-Activity); [App Directory: Discovery](https://support-dev.discord.com/hc/en-us/articles/9489299950487)
  - Unverified Activities are restricted to developers and testers; verified Activities can reach all users.
  - Enabling Discovery requires identity and application verification by the team owner.
  - Enabling Discovery does not guarantee an App Directory listing; it can take up to 24h to appear.
- **User-installable apps** [VERIFIED] — [Discord docs: Developing a user-installable app](https://docs.discord.com/developers/tutorials/developing-a-user-installable-app); [Discord support: Using Apps](https://support.discord.com/hc/hi-in/articles/21334461140375-Using-Apps-in-Discord)
  - Apps can be installed to a user, so the user can open a DM with the app and use its commands across contexts set by `contexts` (guild, bot DM, private channels).
  - User installs support only the `applications.commands` scope by default, and user-installed apps can't read server or DM info.
- **No LFG Activity examples:** No LFG-specific Discord Activity examples were found. LFG on Discord in practice is bots (Miko, Teamplay_LFG, LFG Hub) and channels. — [Miko top.gg](https://top.gg/bot/1096340606235717642); [LFG Hub](https://lfghub.gg/); [Teamplay LFG bot](https://teamplay.gg/discord-lfg-bot)

### Inferences
- **The Activity's role:** An Activity can be the "web app" the friends want, but hosted inside Discord. Discord handles the identity (no separate sign-up). It shows a visual board of open seats and available players with role and rank filters, and is launched from a bot button. The bot keeps doing the notifications (DM or ping with an Accept button).
- **Small-scale use needs no verification:** For a private or small-community MVP, verification is only needed to reach users beyond testers. For use inside the dev's own server(s), check whether the "testers" restriction applies to server members. The verification gate matters for public reach.
- **Activities don't fix the hard part:** The hard part is waking an idle player instantly, and an Activity doesn't help there. It is optional polish, not the core channel.
- **User-install helps adoption:** Players can carry the bot across servers and DMs without each server admin adding it, which helps the cross-community "available pool" grow.

### Gaps
- It is unclear whether Activities can be launched in DMs or group DMs for all users, and on which mobile versions; the pages fetched don't list launch surfaces. Next step: check the "Development Guides" page in Discord's Activity docs.
- Exact verification requirements in 2026 (minimum server count, identity checks) and timelines were not confirmed. Check the [Discord dev support verification article](https://support-dev.discord.com/hc/en-us/articles/8852009977879).
- No 2025–2026 Discord changelog entries on Activities or notifications were reviewed.

## Q5. Desktop companion app: is Riot restricting LCU access with an April 2027 deadline?

### Takeaway
Only partly confirmed, and the framing needs correcting. Riot's April 2027 deadline concerns **game memory access**, not the LCU. From Oct 6, 2026, Riot blocks game memory access by default for unknown third-party apps, and known approved tools have a grace period until April 2027. The LCU (client API used for lobby and invites) is separately described by Riot as unsupported and subject to change. A secondary source says LCU-based apps are not affected by the memory block, but face uncertainty from the 2027 "League Next" client rebuild. A desktop companion that auto-invites via LCU is therefore a fragile bet.

### Cited Findings
- **Memory block:** Riot Support (published Sept 29, 2026) says that starting Oct 6, 2026, Riot will begin "preventing game memory access by default for all unknown third-party applications" across League, TFT and VALORANT. Known, approved tools get a grace period until April 2027 to move to official APIs, request new API data, or retire features. Riot adds that it doesn't "want to promise that any specific application will remain functional." The page does not mention the LCU. [VERIFIED] — [Riot Support: Game Memory Access Removed for Third Party Apps](https://support.riotgames.com/en-us/riot/performance/game-memory-access-removed-for-third-party-apps)
- **No bans:** The change "does not result in bans by itself" [VERIFIED via the same Riot page, as quoted by Hexgate]. — [Hexgate summary](https://hexgate.app/blog/riot-third-party-apps/)
- **LCU is separate but unsupported:** Hexgate classes the LCU as not game memory, so it is not hit by the block. LCU apps "are still expected to work" but the API is unsupported and can change without notice. The cited basis is the Riot Vanguard FAQ. [SECONDARY; primary FAQ not fetched] — [Hexgate](https://hexgate.app/blog/riot-third-party-apps/); [Riot Vanguard FAQ for developers](https://www.riotgames.com/en/DevRel/vanguard-faq)
- **League Next uncertainty:** Commentary on the 2027 "League Next" client rebuild says LCU-reliant apps are the most exposed because the client is being rebuilt, and that Riot hasn't said whether they'll keep working [SECONDARY, speculative]. — [Hexgate: League Next](https://hexgate.app/blog/league-next/)
- **The URL the user cited:** "Changes to the LCU API Policy" is a Jan 24, 2019 post. It temporarily barred LCU apps in Korea, required developers to contact Riot before releasing new LCU apps or updates, and limited apps to approved endpoints. It has no 2027 deadline. [VERIFIED] — [Riot DevRel: Changes to the LCU API Policy (2019)](https://www.riotgames.com/en/DevRel/changes-to-the-lcu-api-policy)
- **Riot's general support page** (Feb 10, 2025) says Riot only takes issue with apps that impede competitive integrity or violate its terms, and apps built on the official Riot API "should rarely have an issue". [VERIFIED] — [Riot Support: Third-party applications](https://support.riotgames.com/en-us/riot/events/third-party-applications)
- **Official alternatives:** The official Riot API (account and rank lookup) and the Live Client Data API (in-game only) are the supported routes. — [Riot Developer Portal: LoL docs](https://developer.riotgames.com/docs/lol)

### Inferences
- The "April 2027 LCU deadline" premise mixes up the memory-access grace period with LCU policy. The correct statement: LCU is unsupported and at risk (League Next rebuild, policy that requires contacting Riot). The April 2027 deadline formally applies to memory-reading tools.
- **Low value for this flow:** The only thing a desktop companion would add is auto-invite or auto-friend-request through the LCU, which saves the step of typing a Riot ID. That is real but small, and it relies on an unsupported API with an uncertain 2027 future. It also adds Windows install and antivirus friction. Rank it last.
- **Use the official API instead:** Use the official Riot API with Riot Sign On (RSO) or account-v1/league-v4 for rank verification. That is supported, unlike the LCU.

### Gaps
- The Riot Vanguard FAQ page wasn't fetched directly to quote its LCU wording.
- No official Riot statement on LCU continuity under League Next was found.

## Q6. Ranked recommendation for this use case (TR, Ranked Flex, "need a 5th now")

### Takeaway
1. **Hybrid: Discord bot as the notification and accept engine, plus a web/PWA (or Discord Activity) as the browsing and profile surface.** This is the best fit.
2. **Discord bot alone, redesigned around buttons and panels instead of typed commands.** Close second, and the cheapest route to traction.
3. **PWA with web push.**
4. **Discord Activity, as an add-on UI only.**
5. **Native mobile app.**
6. **Plain web app.**
7. **Desktop LCU companion.**

### Cited Findings
- Visible League LFG scale sits in Discord-native communities and bots: RiftQ servers with about 65k and 52k members, and Miko's claimed 20k+ connected users [MARKETING for the Miko figure]. — [RiftQ EUW](https://discord.com/servers/riftq-league-of-legends-euw-1161742778510880798); [Miko](https://top.gg/bot/1096340606235717642)
- Competing with Discord as a separate destination has failed: Guilded shut down in Dec 2025, and Discord's own LFG beta was reportedly dropped for low use [the latter ANECDOTAL]. — [Roblox DevForum](https://devforum.roblox.com/t/update-on-guilded-and-communities/3966775); [Discord support post](https://support.discord.com/hc/en-us/community/posts/1500000313161-Where-is-Discord-s-LFG-Please-bring-it-back)
- iOS web push requires a Home Screen install and has reported reliability issues. — [OneSignal](https://documentation.onesignal.com/docs/en/web-push-for-ios); [MagicBell](https://www.magicbell.com/blog/pwa-ios-limitations-safari-support-complete-guide)
- Activities can be launched straight from a bot button (`LAUNCH_ACTIVITY`) and run on desktop, mobile and web, but have no documented notification ability. Public reach needs verification. — [How Activities Work](https://docs.discord.com/developers/activities/how-activities-work); [Enabling Discovery](https://docs.discord.com/developers/discovery/enabling-discovery)
- LCU is unsupported, and Riot is tightening third-party access (memory block from Oct 2026, grace period to April 2027). — [Riot Support](https://support.riotgames.com/en-us/riot/performance/game-memory-access-removed-for-third-party-apps)

### Inferences
1. **Hybrid (Discord bot plus web/PWA): best fit.**
   - The bot owns the "right now" moment: available-role panel, DM or role-ping with an Accept button, first-click-wins, Riot ID reveal. Discord is open on the PC next to the LoL client, and it notifies desktop and phone for free.
   - The web/PWA handles what commands do badly: profile setup, Riot account and rank verification via RSO, browsing open seats, history and ratings, and a shareable link for growth outside Discord.
   - Web push serves as an optional second notification route.
2. **Discord bot alone, button-driven: close second, cheapest.**
   - The "people won't type commands" objection is a UX problem, fixed by a persistent panel message with buttons and select menus, modals, user-install, and accept buttons in DMs.
   - Typed slash commands become optional.
   - This keeps the existing working MVP and is the fastest way to test real demand in TR LoL communities.
3. **PWA with web push.**
   - Good on Android and desktop Chromium.
   - On iOS, the manual install plus permission and flaky push hurt the "instant" promise.
   - It is a new destination, so the user pool must be built from zero (cold start).
4. **Discord Activity.**
   - A nice in-Discord visual board with no separate login, launched from the bot.
   - It can't notify on its own, and public discovery needs verification.
   - Treat it as a later UI layer for option 1 or 2, not as the core channel.
5. **Native mobile app.**
   - Best phone push, but notifies a device that isn't running LoL.
   - Store install and review overhead, two app stores, highest build cost (MAUI/Flutter/RN).
   - No evidence that app-first LoL LFG products serve the instant-fill use case (GameTree is friend-finding and subscription-based).
6. **Plain web app.**
   - No reliable background notification without a PWA or push, which defeats "notify me instantly".
7. **Desktop LCU companion.**
   - Saves only the Riot ID typing and invite step.
   - Relies on an unsupported LCU with 2027 client-rebuild risk, and adds install friction.
   - Not worth it for an MVP.

- **Liquidity is decisive:** For a "need 1 more now" product, the size of the available pool matters more than UI polish. Discord is where the pool already is, so a Discord-first hybrid starts with an advantage.

### Gaps
- No TR-region-specific data on Discord vs app usage for LoL LFG.
- No A/B or conversion data comparing bot-button UX with web UX for LFG. The developer should measure this in their own MVP: time-to-fill, accept rate, notification-to-accept latency.
