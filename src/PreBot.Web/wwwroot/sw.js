// PreBot service worker: only handles push notifications (no offline caching on purpose —
// the pool is live data, a cached page would show stale listings).

self.addEventListener('install', () => self.skipWaiting());
self.addEventListener('activate', (event) => event.waitUntil(self.clients.claim()));

self.addEventListener('push', (event) => {
    let data;
    try {
        data = event.data ? event.data.json() : {};
    } catch {
        data = { title: 'PreBot', body: event.data ? event.data.text() : '' };
    }

    const options = {
        body: data.body || '',
        icon: '/icons/icon-192.png',
        badge: '/icons/badge-72.png',
        tag: data.tag || 'prebot',
        // A quiet update ("Doldu") replaces the earlier notification without buzzing again.
        renotify: !data.quiet,
        silent: !!data.quiet,
        requireInteraction: !!data.joinUrl,
        // Chrome refuses a vibration pattern on a silent notification.
        ...(data.quiet ? {} : { vibrate: [200, 100, 200] }),
        data: {
            url: data.url || '/havuz',
            joinUrl: data.joinUrl || null,
            muteUrl: data.muteUrl || null,
            openedUrl: data.openedUrl || null,
            inGameUrl: data.inGameUrl || null,
        },
        // Action buttons work on Chrome/Edge/Firefox (most show two); Safari ignores them and just opens the listing,
        // where the same choices are buttons on the page.
        actions: data.joinUrl ? listingActions(data) : [],
    };

    event.waitUntil(self.registration.showNotification(data.title || 'PreBot', options));
});

// Katıl first, then "in a match, tell me afterwards", then mute — as many as this browser can show.
function listingActions(data) {
    const actions = [{ action: 'join', title: 'Katıl' }];
    if (data.inGameUrl) {
        actions.push({ action: 'ingame', title: 'Maçtayım, sonra' });
    }
    if (data.muteUrl) {
        actions.push({ action: 'mute', title: '1 saat sustur' });
    }
    const max = (self.Notification && Notification.maxActions) || 2;
    return actions.slice(0, max);
}

const post = (url) => fetch(url, { method: 'POST' }).catch(() => {});

self.addEventListener('notificationclick', (event) => {
    event.notification.close();
    const { url, joinUrl, muteUrl, openedUrl, inGameUrl } = event.notification.data || {};
    if (event.action === 'skip') {
        return;
    }

    if (event.action === 'mute' && muteUrl) {
        event.waitUntil(post(muteUrl));
        return;
    }

    if (event.action === 'ingame' && inGameUrl) {
        event.waitUntil(post(inGameUrl));
        return;
    }

    const target = new URL(event.action === 'join' && joinUrl ? joinUrl : url || '/havuz', self.location.origin).href;

    // Counted once on the server: did the player see it, and how long after it was sent?
    if (openedUrl) {
        event.waitUntil(post(openedUrl));
    }

    event.waitUntil((async () => {
        const windows = await self.clients.matchAll({ type: 'window', includeUncontrolled: true });
        for (const client of windows) {
            if (new URL(client.url).origin === self.location.origin && 'focus' in client) {
                await client.focus();
                return client.navigate(target);
            }
        }
        return self.clients.openWindow(target);
    })());
});
