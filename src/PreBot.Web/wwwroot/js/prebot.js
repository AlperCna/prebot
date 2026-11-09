// Small browser helpers called from Blazor components.
window.prebot = {
    // Flash the tab title and play a short beep so a player in another tab/window notices an alert.
    attention(text) {
        const original = document.title;
        let on = false;
        let count = 0;
        clearInterval(window.prebot._flash);
        window.prebot._flash = setInterval(() => {
            document.title = on ? original : text;
            on = !on;
            if (++count > 12 || document.hasFocus()) {
                clearInterval(window.prebot._flash);
                document.title = original;
            }
        }, 800);

        try {
            const ctx = new (window.AudioContext || window.webkitAudioContext)();
            const osc = ctx.createOscillator();
            const gain = ctx.createGain();
            osc.frequency.value = 880;
            gain.gain.setValueAtTime(0.15, ctx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.4);
            osc.connect(gain).connect(ctx.destination);
            osc.start();
            osc.stop(ctx.currentTime + 0.4);
        } catch {
            // Browsers may block audio before the first click; the title flash still works.
        }
    },

    async copy(text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch {
            return false;
        }
    },

    // Opens the phone's share sheet when there is one. Returns "shared", "copied" or "failed".
    async share(title, text, url) {
        if (navigator.share) {
            try {
                await navigator.share({ title, text, url });
                return 'shared';
            } catch (e) {
                if (e && e.name === 'AbortError') {
                    return 'cancelled';
                }
            }
        }

        return (await prebot.copy(`${text} ${url}`)) ? 'copied' : 'failed';
    },

    // ---------- Web Push ----------
    push: {
        supported() {
            return 'serviceWorker' in navigator && 'PushManager' in window && 'Notification' in window;
        },

        // Everything the UI needs to show the right hint for this device.
        async state() {
            const ua = navigator.userAgent;
            const ios = /iPhone|iPad|iPod/.test(ua);
            const standalone = window.matchMedia('(display-mode: standalone)').matches || navigator.standalone === true;
            const supported = window.prebot.push.supported();
            let endpoint = null;
            if (supported) {
                const reg = await navigator.serviceWorker.getRegistration('/');
                const sub = reg ? await reg.pushManager.getSubscription() : null;
                endpoint = sub ? sub.endpoint : null;
            }
            return {
                supported,
                ios,
                standalone,
                permission: supported ? Notification.permission : 'unsupported',
                endpoint,
            };
        },

        // Asks permission (must run from a click) and subscribes this browser.
        async subscribe(publicKey) {
            if (!window.prebot.push.supported()) {
                return { error: 'unsupported' };
            }
            // On iPhone the permission prompt only appears if it is requested inside the tap itself.
            // The click listener at the bottom of this file starts it right in the tap; we just wait here.
            const pending = window.prebot.push._pending;
            window.prebot.push._pending = null;
            const permission = await (pending || Notification.requestPermission());
            if (permission !== 'granted') {
                return { error: permission };
            }

            const reg = await navigator.serviceWorker.register('/sw.js', { scope: '/' });
            await navigator.serviceWorker.ready;
            const key = window.prebot.push._toBytes(publicKey);

            let sub = await reg.pushManager.getSubscription();
            if (sub && !window.prebot.push._sameKey(sub, key)) {
                await sub.unsubscribe(); // server keys changed: old subscription can't be used
                sub = null;
            }
            if (!sub) {
                sub = await reg.pushManager.subscribe({ userVisibleOnly: true, applicationServerKey: key });
            }

            const json = sub.toJSON();
            return { endpoint: json.endpoint, p256dh: json.keys.p256dh, auth: json.keys.auth, device: window.prebot.push._device() };
        },

        async unsubscribe() {
            const reg = await navigator.serviceWorker.getRegistration('/');
            const sub = reg ? await reg.pushManager.getSubscription() : null;
            if (!sub) {
                return null;
            }
            const endpoint = sub.endpoint;
            await sub.unsubscribe();
            return endpoint;
        },

        _toBytes(base64url) {
            const padded = (base64url + '='.repeat((4 - (base64url.length % 4)) % 4)).replace(/-/g, '+').replace(/_/g, '/');
            return Uint8Array.from(atob(padded), (c) => c.charCodeAt(0));
        },

        _sameKey(sub, key) {
            const current = sub.options && sub.options.applicationServerKey;
            if (!current) {
                return true;
            }
            const a = new Uint8Array(current);
            return a.length === key.length && a.every((v, i) => v === key[i]);
        },

        _device() {
            const ua = navigator.userAgent;
            const os = /Android/.test(ua) ? 'Android' : /iPhone|iPad|iPod/.test(ua) ? 'iPhone' : /Windows/.test(ua) ? 'Windows' : /Mac/.test(ua) ? 'Mac' : 'Cihaz';
            const browser = /Edg\//.test(ua) ? 'Edge' : /SamsungBrowser/.test(ua) ? 'Samsung' : /OPR\//.test(ua) ? 'Opera'
                : /Firefox/.test(ua) ? 'Firefox' : /Chrome/.test(ua) ? 'Chrome' : /Safari/.test(ua) ? 'Safari' : 'Tarayıcı';
            return `${os} · ${browser}`;
        },
    },
};

// Register the service worker early so notification clicks work even before the user opens settings.
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/sw.js', { scope: '/' }).catch(() => { /* not critical */ });
}

// Things Safari (iPhone) only allows during the user's tap, before any round trip to the server.
document.addEventListener('click', (event) => {
    // "Bildirimleri aç": ask for permission inside the tap; prebot.push.subscribe() picks up the answer.
    const enable = event.target.closest('[data-push-enable]');
    if (enable && window.prebot.push.supported() && Notification.permission === 'default') {
        window.prebot.push._pending = Notification.requestPermission();
    }

    // Copy buttons (Riot ID, lobby name): copy inside the tap, show a short confirmation.
    const copy = event.target.closest('[data-copy]');
    if (copy) {
        window.prebot.copy(copy.dataset.copy).then((ok) => {
            const hint = copy.querySelector('.copy-hint');
            if (ok && hint) {
                const original = hint.innerHTML;
                hint.textContent = 'kopyalandı';
                setTimeout(() => { hint.innerHTML = original; }, 2500);
            }
        });
    }

    // "Paylaş": open the share sheet directly from the tap (no server round trip in between).
    const share = event.target.closest('[data-share-url]');
    if (share) {
        event.preventDefault();
        window.prebot.share(share.dataset.shareTitle || 'PreBot', share.dataset.shareText || '', share.dataset.shareUrl)
            .then((result) => {
                if (result === 'copied' || result === 'shared') {
                    const original = share.innerHTML;
                    share.textContent = result === 'copied' ? 'Bağlantı kopyalandı' : 'Paylaşıldı';
                    setTimeout(() => { share.innerHTML = original; }, 3000);
                }
            });
    }
}, true);

// "Uygulamayı yükle": Chrome/Edge on Android and PC offer installing the site as an app (own window,
// notifications even with the browser closed on PC). The button stays hidden where this isn't offered.
window.addEventListener('beforeinstallprompt', (event) => {
    event.preventDefault();
    window.prebot._installPrompt = event;
    document.querySelectorAll('[data-install]').forEach((b) => { b.hidden = false; });
});
// Blazor's enhanced navigation re-renders the layout, which hides the button again: show it once more.
window.addEventListener('load', () => {
    window.Blazor?.addEventListener?.('enhancedload', () => {
        if (window.prebot._installPrompt) {
            document.querySelectorAll('[data-install]').forEach((b) => { b.hidden = false; });
        }
    });
});
window.addEventListener('appinstalled', () => {
    window.prebot._installPrompt = null;
    document.querySelectorAll('[data-install]').forEach((b) => { b.hidden = true; });
});
document.addEventListener('click', async (event) => {
    const button = event.target.closest('[data-install]');
    if (!button || !window.prebot._installPrompt) {
        return;
    }
    const prompt = window.prebot._installPrompt;
    window.prebot._installPrompt = null;
    await prompt.prompt();
    button.hidden = true;
});
