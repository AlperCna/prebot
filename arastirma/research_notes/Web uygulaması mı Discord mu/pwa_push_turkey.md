# PWA / Web Push Feasibility for an Instant "A Game Needs You Now" Alert (Turkey, Nov 2026)

Labels used: **[DATA]** = measured or official data or spec. **[VENDOR]** = claim from a push vendor or marketing source. **[ANECDOTE]** = user reports or forum posts. **[OPINION]** = commentary or my own judgement.
Research date: 2026-11-08. About 20 tool calls were made, so coverage is targeted rather than exhaustive.

## 1. Web Push platform support and limitations (2026)

### Takeaway
Web Push works on Android Chrome and Samsung Internet and on desktop Chrome, Edge, Firefox and Safari without installing anything. On iPhone and iPad it works only after the user adds the site to the Home Screen (iOS 16.4+). iOS 26 made that step easier by defaulting "Add to Home Screen" to a web app, but it still takes several taps. Notification action buttons are not supported on Safari (macOS or iOS), so a true "one-tap accept" button is only possible on Chromium and Firefox. On iOS the tap opens the app instead. The DMA does not affect Turkey.

### Cited Findings
- **[DATA]** iOS/iPadOS Web Push is limited to web apps added to the Home Screen. The manifest `display` must be `standalone` or `fullscreen`, otherwise the site is saved as a bookmark. — [WebKit: Web Push for Web Apps on iOS and iPadOS](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)
- **[DATA]** On iOS, notification permission can only be requested in response to a direct user action (for example a "subscribe" button). Users manage it per app in Notification Settings. — [WebKit](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)
- **[DATA]** iOS web app notifications follow Focus modes, so Focus/DND suppresses them like native apps unless the user allows the app. A Focus setting applies to the same web app on all of the user's devices. Badging (`setAppBadge`) is supported. — [WebKit](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)
- **[DATA]** No Apple Developer Program membership is needed for web push on iOS. Servers must allow `*.push.apple.com` endpoints. Third-party iOS browsers can add to Home Screen only with Apple's browser entitlement. — [WebKit](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)
- **[DATA, secondary]** A WebKit web push always shows a visible notification. Silent background refresh via push is not possible on iOS. — summarized in [MagicBell PWA iOS limitations guide](https://www.magicbell.com/blog/pwa-ios-limitations-safari-support-complete-guide). The primary WebKit post I fetched does not mention silent push, so treat this as secondary.
- **[DATA]** iOS 26 / iPadOS 26: "Add to Home Screen" from Safari's share sheet now creates a web app by default. An "Open as web app" toggle can turn this off. A manifest is no longer required, and Apple says any website can become a Home Screen web app. The article does not mention push changes. — [heise: iOS 26 changed web app behaviour on the home screen](https://heise.de/-10749652); also [iDownloadBlog](https://www.idownloadblog.com/?p=1042117)
- **[OPINION/ANECDOTE]** An Apple Developer Forums post argues iOS 26 is a step back because adding a PWA now takes at least five taps. — [Apple Developer Forums thread 801602](https://developer.apple.com/forums/thread/801602)
- **[DATA]** Declarative Web Push lets a site subscribe and show notifications without a service worker. It shipped in iOS/iPadOS 18.4 for Home Screen web apps and reached macOS in Safari 18.5 (macOS 15.5). — [WebKit Features in Safari 18.4](https://webkit.org/blog/16574/digital-credentials-api/); [Meet Declarative Web Push (WebKit)](https://webkit.org/?p=16535); [MacTech on Safari 18.5](https://www.mactech.com/2025/05/12/with-safari-18-5-included-in-macos-15-5-apple-added-declarative-web-push)
- **[DATA]** Notification `actions` (buttons) support per caniuse (Nov 2026): Chrome desktop 53+, Edge 18+, Samsung Internet 6.2+, Chrome for Android (supported), Firefox desktop listed as 152+. **Not supported in Safari desktop (through 27.3) or Safari iOS (through 27.2).** — [caniuse: Notification actions](https://caniuse.com/mdn-api_notification_actions). Note: caniuse's Firefox version figure looked unusual and should be double-checked on the page.
- **[DATA]** `showNotification` options include `actions`, `requireInteraction` (keep the notification on screen until the user acts), `silent`, `vibrate`, `renotify` + `tag`. It needs a secure context (HTTPS) and is Baseline "widely available" since March 2023. — [MDN: showNotification](https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorkerRegistration/showNotification)
- **[DATA]** Desktop caveat: in Chrome/Edge (not installed as a PWA), notifications appear only while the browser process is running. Otherwise they show the next time the browser opens. An installed PWA should receive them even when the app isn't running. — [Microsoft Learn: Blazor PWA push notifications](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/push-notifications)
- **[DATA]** Android delivery: FCM normal-priority messages can be held during Doze. High-priority messages are delivered immediately and can wake the device, but should produce user-visible notifications or they can be downgraded. Overusing high priority can delay important messages. Web push sets the equivalent with the `Urgency: high` header. — [Firebase: Set and manage Android message priority](https://firebase.google.com/docs/cloud-messaging/android-message-priority?authuser=6); [Firebase blog: Ensure FCM notifications reach users (2025)](https://firebase.blog/posts/2025/04/fcm-on-android)
- **[VENDOR]** Pushwoosh says aggressive OEM battery optimisation (Xiaomi, Huawei, Samsung) is the most common cause of Android push delays. — [Pushwoosh help: delayed time-sensitive notifications](https://help.pushwoosh.com/hc/en-us/articles/31226802486301-Why-are-my-time-sensitive-scheduled-notifications-being-delayed-and-how-can-I-improve-delivery-precision)
- **[ANECDOTE]** A developer blog measured 30–60 minutes of push delay for a non-whitelisted app on a Xiaomi Android 13 device after 3 hours idle. This is a single test. — [xckevin: Android Push Delivery: FCM, Vendor Channels, and Doze](https://xckevin.github.io/en/blog/android-push-service-fcm-vendor-channels/)
- **[DATA]** EU DMA: Apple planned to remove Home Screen web apps in the EU in iOS 17.4, then reversed this on 1 March 2024 and kept the existing capability. The issue was EU-only throughout. — [MacRumors](https://www.macrumors.com/2024/03/01/apple-walks-back-decision-to-disable-eu-web-apps/); [9to5Mac](https://9to5mac.com/2024/03/01/apple-home-screen-web-apps-ios-17-eu/)

### Inferences
- Turkey is not an EU member state, so DMA-specific iOS behaviour (including the reverted 17.4 removal and alternative browser engines) does not apply to Turkish App Store accounts. Turkish iOS users get the standard global web app and push behaviour. (Inference: no source specifically about Turkey was found.)
- For the "respond with one tap" requirement:
  - Android Chrome, Samsung Internet and desktop Chromium can show inline "Join / Pass" buttons.
  - iOS and macOS Safari users must tap the notification body, which opens the PWA, and then press Join. That is effectively two taps.
- Chrome on Android sends web push through FCM. With `Urgency: high` and a visible notification, delivery is normally in seconds. On Xiaomi, Oppo and Huawei-style ROMs with aggressive battery managers, Chrome itself may be restricted. The app should tell users to exempt Chrome from battery optimisation and set autostart. This is unverified for Chrome specifically (see Gaps).
- `requireInteraction: true` plus `renotify` with a `tag` per match request is the right pattern on Chromium. Short TTL headers (for example 60–120 s) stop stale "game needs you" alerts from arriving late.

### Gaps
- I found no primary source on web push changes in **iOS 27** (Sept 2026). caniuse lists Safari 27.x but still no action-button support. WebKit's Safari 26/27 release notes were not checked directly.
- I found no measured latency study comparing web push with native push end to end (p50/p95 seconds). Delivery-time numbers are not available from primary sources.
- I found no source explaining how Chrome on Xiaomi/HyperOS handles web push under MIUI battery saver specifically.
- Sound customisation: the Notifications API has no custom sound option widely supported (the `sound` option is not implemented by major browsers). This is from my knowledge and was not verified with a citation this session.

## 2. Turkish device and browser market (StatCounter)

### Takeaway
Turkey is about 77% Android and 23% iOS on mobile, and Chrome holds about 79% of mobile browsing. Desktop is about 86% Windows. So roughly three quarters of Turkish mobile users can get web push with no install step, while the iOS quarter needs Add to Home Screen.

### Cited Findings
- **[DATA]** Mobile OS share in Turkey, October 2026: Android 76.78%, iOS 23.22%. — [StatCounter: Mobile OS Turkey](https://gs.statcounter.com/os-market-share/mobile/turkey)
- **[DATA, via search snippets of StatCounter pages]** Earlier 2026 figures: May about 74.5% Android / 25.5% iOS, June about 75.7% / 24.3%, July about 74.2% / 25.8%. — [StatCounter mobile-tablet Turkey](https://gs.statcounter.com/os-market-share/mobile-tablet/turkey). These monthly values came from search-result summaries, not a direct page read.
- **[DATA]** Mobile browser share in Turkey, October 2026: Chrome 78.85%, Safari 15.76%, Samsung Internet 2.82%, Opera 1.25%, Yandex Browser 0.9%, UC 0.18%. — [StatCounter: Mobile browser Turkey](https://gs.statcounter.com/browser-market-share/mobile/turkey)
- **[DATA]** Desktop OS share in Turkey, October 2026: Windows 86.39%, Linux 5.83%, macOS 4.43% + "OS X" 3.29% (about 7.7% Apple combined), Chrome OS 0.05%. — [StatCounter: Desktop OS Turkey](https://gs.statcounter.com/os-market-share/desktop/turkey)

### Inferences
- League of Legends runs only on Windows and macOS, and Riot's anti-cheat makes Windows the norm. So Turkish LoL players on desktop are overwhelmingly Windows + Chrome/Edge. Desktop web push there supports action buttons and is mature. The catch is that the browser must be running, or the PWA installed (Edge and Chrome can install PWAs).
- Safari's mobile share (15.8%) is lower than iOS share (23.2%). Some iOS users browse with Chrome for iOS, which cannot do web push unless the app is added to the Home Screen from Safari or an entitled browser.
- Yandex Browser is under 1% in Turkey, so it is not a priority to test.

### Gaps
- I found no StatCounter or other data on **gamers specifically** in Turkey. The Steam Hardware Survey is global and not broken down by country. Riot does not publish TR-server device data.

## 3. Opt-in, engagement and PWA install benchmarks

### Takeaway
Generic web push opt-in is low: about 10% of prompts are granted on desktop Chrome and 21% on Android (Google data, about 2020). Vendors report 10–15% site-wide opt-in with good prompts. A matchmaking app is different: the user asks to be alerted, so in-context opt-in should be much higher, but no published data proves this. Data on how many users install iOS PWAs is essentially missing.

### Cited Findings
- **[DATA]** Google telemetry: notifications make up 74% of all Chrome permission prompts and are the least granted. The grant rate is about 10% on desktop and 21% on Android. Chrome's "quiet UI" cut deny rates about 30% at a cost of about 5 points of grant rate. — [Google Research: "Shhh...be quiet!" (USENIX Security 2021)](https://research.google/pubs/shhhbe-quiet-reducing-the-unwanted-interruptions-of-notification-permission-prompts-on-chrome/); [USENIX presentation](https://www.usenix.org/conference/usenixsecurity21/presentation/bilogrevic)
- **[DATA]** Chrome automatically quiets prompts for sites in the bottom 5% of grant rates, or after a user denies 3 prompts in 28 days. — [Chromium blog: quieter permission UI](https://blog.chromium.org/2020/01/introducing-quieter-permission-ui-for.html)
- **[DATA]** Chrome publishes per-origin notification Accept/Block rates in CrUX (since dataset 202001). — [Chrome Developers: notification permission data in CrUX](https://developer.chrome.com/blog/notification-permission-data-in-crux?hl=en)
- **[VENDOR]** Gravitec (cited by Sleeknote) reports typical web push opt-in of 10–15% with well-placed prompts. — [Sleeknote push statistics 2026](https://sleeknote.com/?p=138964)
- **[VENDOR]** PushPushGo: 8.21% average CTR for automated web push campaigns, and 5.09% for abandoned-cart pushes. — [PushPushGo: 31 stats about web push](https://pushpushgo.com/en/blog/31-stats-about-web-push)
- **[VENDOR]** iZooto: some sites see 60–70% acceptance on non-native (soft) prompts, others 10–20%. This is a small vendor sample. — [iZooto Chrome permission UX report](https://www.izooto.com/chrome-notification-permission-ux-report)
- **[VENDOR, native app push for comparison]** Pushwoosh (600+ apps, CTR period Q4 2024–Q2 2025, opt-in period 2024) gives CTRs for action games of 0.46% on iOS and 0.82% on Android, and for hypercasual games 0.82% iOS and 1.05% Android. iOS opt-in for games is "significantly below average", while Android is slightly below benchmark. — [Pushwoosh push notification benchmarks](https://www.pushwoosh.com/blog/push-notification-benchmarks/)
- **[VENDOR, secondary]** Batch 2025 (via aggregators): average native opt-in is 61%. iOS fell from 58% to 56%, and Android from 85% to 67% after the Android 13 runtime permission. — summarized in [Business of Apps](https://www.businessofapps.com/?p=101648) / [Sleeknote](https://sleeknote.com/?p=138964). Not verified against Batch's primary report.
- **[VENDOR]** Airship 2025 benchmark report exists, but its figures were not retrieved. — [Airship marketer's guide to push benchmarks](https://www.airship.com/blog/a-marketers-guide-to-push-notification-benchmarks/)
- **[VENDOR/OPINION]** Apple itself cited "very low user adoption of Home Screen web apps" when it proposed removing them in the EU in 2024. — quoted in [MacRumors](https://www.macrumors.com/2024/03/01/apple-walks-back-decision-to-disable-eu-web-apps/)

### Inferences
- Marketing CTR benchmarks (about 1–8%) don't fit this use case well. A "your group needs you now" alert sent to someone who just marked themselves available is a transactional, expected message, which behaves more like a chat message than a promotion. Expect far higher open rates. This is my opinion, and there is no data for it.
- iOS is the weakest point. About 23% of Turkish mobile users must (a) open the site in Safari, (b) Share, then Add to Home Screen, (c) open the installed app, and (d) grant permission. Each step loses users, and Apple's own statement suggests Home Screen web app adoption is low. Plan for a large share of iOS users never reaching push-ready status. An in-app install walkthrough is essential.

### Gaps
- I found no reliable published number for the share of iOS users who add a PWA to the Home Screen, and no 2025–2026 OneSignal web push opt-in/CTR benchmark (OneSignal pages were not retrieved).
- Web push unsubscribe/revoke rates: no reliable source found this session.
- No Turkey-specific opt-in data.

## 4. Native app push vs Web Push vs Discord DM/mention

### Takeaway
On Android, native push and Chrome web push use the same FCM transport, so speed is similar. Native apps get better OEM battery-whitelist handling, full-screen or time-sensitive options and custom sounds. On iOS, native push (APNs) is far more capable: Time Sensitive interruption level, action buttons, custom sounds, and no Home Screen install step. Discord is convenient, but by Discord support's own account it suppresses mobile push while the desktop client is active. That is a real risk for players sitting at their PC.

### Cited Findings
- **[DATA]** iOS web push uses Apple's push service (`*.push.apple.com`), the same APNs infrastructure as native apps. — [WebKit](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)
- **[DATA]** FCM high priority is meant for time-sensitive, user-visible content and is delivered immediately even in Doze. — [Firebase message priority](https://firebase.google.com/docs/cloud-messaging/android-message-priority?authuser=6)
- **[ANECDOTE + support statement relayed by users]** Discord: when logged into several sessions, only one client receives push notifications. Users say support called this "by design", and mobile push for DMs and mentions does not arrive while the desktop client is open. — [Discord Support community: Will not receive push notifications until desktop client is closed](https://support.discord.com/hc/fr/community/posts/360052612473-Will-not-receive-push-notifications-until-desktop-client-is-closed); [Discord feedback: Let us receive push notifications on mobile while logged into desktop](https://support.discord.com/hc/de/community/posts/360040208611-Let-us-receive-push-notifications-on-mobile-while-logged-into-a-desktop-client)
- **[ANECDOTE]** Users report that Discord's "Push Notification Inactive Timeout" setting does not reliably restore mobile push (for example, set to 1 minute with no pushes). — [Discord Support community: Push Notification Inactive Timeout no longer working](https://support.discord.com/hc/sv/community/posts/360051256154-Push-Notification-Inactive-Timeout-no-longer-working); [Allow 0 minute AFK timeout request](https://support.discord.com/hc/da/community/posts/360039981112-Allow-0-minute-setting-for-Push-Notification-AFK-timeout)
- **[VENDOR, unverified]** A third-party guide describes Discord delaying or suppressing mobile push while desktop or web is active. — [TRTC blog: Discord push notifications gaming guide](https://trtc.io/blog/details/discord-push-notifications-gaming-guide)

### Inferences
- For a player at their PC waiting for a game, the Discord desktop app does show the DM/mention (desktop toast plus sound) while mobile is suppressed. For an "available now" player who is AFK from the PC but has Discord desktop open, Discord may fail to reach their phone. Web push sent to every subscribed device (desktop browser + phone) avoids this, but the app must deduplicate (for example by closing notifications by `tag` once the user responds).
- Discord's advantage is that nearly all LoL players already have it installed with permissions granted, so there are zero onboarding steps. Web push's advantage is control (TTL, urgency, action buttons, deep link to an "Accept" page) and no dependence on Discord's suppression logic.
- **[OPINION]** A hybrid is likely best: the PWA as primary with web push, plus an optional Discord DM fallback through a bot for users who never enable push (especially iOS users who never install).

### Gaps
- No primary, quantitative latency comparison (seconds) between Web Push, native FCM/APNs and Discord DMs was found.
- No official Discord documentation on push routing rules was found, only support-forum relays.

## 5. .NET implementation (VAPID, service worker, Blazor PWA, cost)

### Takeaway
It is easy and free. Microsoft's own Blazor PWA push docs (.NET 10 default, updated Nov 2026) use the `WebPush` NuGet package with VAPID keys. `Lib.Net.Http.WebPush` 3.3.1 (MIT) is a well-maintained alternative with an ASP.NET Core companion. You need HTTPS, a service worker with `push` and `notificationclick` handlers, and subscription storage. No FCM or APNs accounts or Apple Developer membership are needed for web push.

### Cited Findings
- **[DATA]** Blazor WebAssembly PWA template: `dotnet new blazorwasm -o MyBlazorPwa --pwa`. It provides a manifest, icons, `service-worker.js` (dev) and `service-worker.published.js` (cache-first, offline only when published). The doc lists push notifications as a PWA capability. — [Microsoft Learn: Blazor PWA](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/)
- **[DATA]** The template does not wire up push itself. Microsoft's push guide adds the following:
  - **Client:** JS that calls `pushManager.subscribe({ userVisibleOnly: true, applicationServerKey })` and sends the endpoint, `p256dh` and `auth` to the server.
  - **Server:** an endpoint that stores the subscription, and code that sends with the **`WebPush`** NuGet package (`WebPushClient`, `VapidDetails`, `PushSubscription`).
  - **Service worker:** `push` (calls `showNotification`) and `notificationclick` (calls `clients.openWindow`) handlers.
  - **Key handling:** keep the private key in Secret Manager or Key Vault.

  — [Microsoft Learn: Push notifications for Blazor PWAs](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/push-notifications)
- **[DATA]** In that guide's example, permission is requested on the Checkout page, where the reason to subscribe is clear. Microsoft recommends asking "only when it's clear why they would want to subscribe." — [Microsoft Learn: Push notifications for Blazor PWAs](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/push-notifications)
- **[DATA]** `Lib.Net.Http.WebPush` 3.3.1 is an MIT-licensed Web Push protocol client with VAPID and message encryption (RFC 8291). The companion `Lib.AspNetCore.WebPush` provides ASP.NET Core integration. — [NuGet: Lib.Net.Http.WebPush](https://www.nuget.org/packages/Lib.Net.Http.WebPush); [NuGet: Lib.AspNetCore.WebPush](https://feed.nuget.org/packages/Lib.AspNetCore.WebPush); [project docs](https://tpeczek.github.io/Lib.Net.Http.WebPush)
- **[DATA]** There is also an Azure Functions extension for the same library. — [NuGet: Lib.Azure.Functions.Worker.Extensions.WebPush](https://www.nuget.org/packages/Lib.Azure.Functions.Worker.Extensions.WebPush)
- **[DATA]** `Persilsoft.WebPush.Blazor` is a separate Blazor helper for client-side subscription. — [NuGet: Persilsoft.WebPush.Blazor](https://packages.nuget.org/packages/Persilsoft.WebPush.Blazor)
- **[DATA]** iOS web push requires no Apple Developer Program membership. — [WebKit](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)
- **[DATA]** `showNotification` and service workers require a secure context (HTTPS). — [MDN: showNotification](https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorkerRegistration/showNotification)

### Inferences
- **Cost:** browser push services (FCM for Chrome/Edge-on-Android, Mozilla autopush, Apple web push) do not charge senders. The only costs are hosting and HTTPS (for example a free Let's Encrypt certificate). A native app would add an Apple Developer fee ($99/yr, from general knowledge, not cited here), a Google Play one-time fee, and separate APNs/FCM setup. Native push sending itself is also free.
- Server side, set the `TTL` (for example 60–120 s) and `Urgency: high` headers. Handle HTTP 404/410 responses by deleting expired subscriptions. Store multiple subscriptions per user (phone + PC). Microsoft's sample keeps only one subscription per user, which is the wrong choice here.
- Blazor Server or SSR apps (not WASM) can still be PWAs with a hand-written manifest and service worker, since push is independent of Blazor (per the Microsoft doc's statement that the mechanism is independent of Blazor WebAssembly).

### Gaps
- Current NuGet version and maintenance status of the `WebPush` (web-push-csharp) package were not checked this session.
- Declarative Web Push support in the .NET libraries (sending the declarative JSON payload format) was not verified.

## 6. UX best practices for the permission prompt

### Takeaway
Ask only at the moment the user clicks "I'm available now", with a soft pre-prompt that explains the benefit. Never ask on page load. On iOS, first walk the user through Add to Home Screen, because permission can't be requested until the app is installed and must follow a user tap.

### Cited Findings
- **[DATA]** Chrome auto-quiets sites with low grant rates (bottom 5%), so prompting cold on page load can permanently degrade the prompt for all users. — [Chromium blog](https://blog.chromium.org/2020/01/introducing-quieter-permission-ui-for.html)
- **[DATA]** iOS requires the permission request to come from a direct user interaction. — [WebKit](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)
- **[DATA]** Microsoft recommends asking when the reason is clear (their example is the Checkout page). — [Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/push-notifications)
- **[DATA]** Once a user allows or blocks, the browser won't ask again. Resetting requires the user to change site settings manually. — [Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app/push-notifications)
- **[VENDOR]** Soft (custom) pre-prompts reach 60–70% acceptance on some sites. — [iZooto](https://www.izooto.com/chrome-notification-permission-ux-report)

### Inferences (recommended flow)
1. The user taps "Şu an müsaitim" ("I'm available now"). Show an in-page explainer: "Eşleşme bulunduğunda saniyeler içinde bildirim alırsın" ("You'll get a notification within seconds when a match is found"). Then the button triggers the native prompt from that same tap.
2. iOS detected and not standalone (`display-mode: standalone` false): show an illustrated "Share, then Add to Home Screen" guide (iOS 26 defaults to web app mode), and ask for permission after first launch from the icon.
3. Android Xiaomi, Oppo or Samsung: after opt-in, show a one-time tip to disable battery optimisation for Chrome and allow notifications on the lock screen.
4. Send a test push right after subscribing ("Bildirimler çalışıyor ✓", "Notifications are working ✓") to confirm delivery and catch silent failures.
5. Fallback: if the user denies, keep an on-page live status via SignalR/WebSocket while the tab is open, plus an optional Discord DM fallback.
6. Make the "available" state expire automatically (for example after 30–60 minutes) so pushes stay relevant and users don't revoke permission.

### Gaps
- No A/B data was found specifically on contextual versus page-load prompts for gaming or matchmaking apps.
