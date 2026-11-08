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
};
