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
        renotify: true,
        requireInteraction: !!data.joinUrl,
        vibrate: [200, 100, 200],
        data: { url: data.url || '/havuz', joinUrl: data.joinUrl || null, muteUrl: data.muteUrl || null },
        // Action buttons work on Chrome/Edge/Firefox; Safari ignores them and just opens the listing.
        actions: data.joinUrl
            ? [{ action: 'join', title: 'Katıl' }, { action: data.muteUrl ? 'mute' : 'skip', title: data.muteUrl ? '1 saat sustur' : 'Geç' }]
            : [],
    };

    event.waitUntil(self.registration.showNotification(data.title || 'PreBot', options));
});

self.addEventListener('notificationclick', (event) => {
    event.notification.close();
    const { url, joinUrl, muteUrl } = event.notification.data || {};
    if (event.action === 'skip') {
        return;
    }

    if (event.action === 'mute' && muteUrl) {
        event.waitUntil(fetch(muteUrl, { method: 'POST' }).catch(() => {}));
        return;
    }

    const target = new URL(event.action === 'join' && joinUrl ? joinUrl : url || '/havuz', self.location.origin).href;

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
