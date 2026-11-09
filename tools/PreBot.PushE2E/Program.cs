// End-to-end push test against a running PreBot.Web, using a real Edge/Chrome with a throwaway profile.
// Usage: dotnet run --project tools/PreBot.PushE2E -- [baseUrl] [browserPath] [--headed]
//
// What it proves: the browser really subscribes to its vendor's push service, the server really
// delivers through it, the service worker really shows the system notification, and the
// notification's "Katıl" link really claims the seat.
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using PreBot.PushE2E;

var baseUrl = args.FirstOrDefault(a => a.StartsWith("http")) ?? "http://localhost:5215";
var browserPath = args.FirstOrDefault(a => a.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
    ?? new[]
    {
        @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
        @"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
        @"C:\Program Files\Google\Chrome\Application\chrome.exe",
    }.First(File.Exists);
var headed = args.Contains("--headed");
var port = 9300 + Random.Shared.Next(0, 600);
var profile = Path.Combine(Path.GetTempPath(), $"prebot-e2e-{Guid.NewGuid():N}");
var run = DateTime.Now.ToString("HHmmss");

var failures = 0;
async Task Step(string name, Func<Task> action)
{
    var sw = Stopwatch.StartNew();
    try
    {
        await action();
        Console.WriteLine($"  ✅ {name} ({sw.ElapsedMilliseconds} ms)");
    }
    catch (Exception ex)
    {
        failures++;
        Console.WriteLine($"  ❌ {name} ({sw.ElapsedMilliseconds} ms): {ex.Message}");
        throw;
    }
}

Console.WriteLine($"PreBot push E2E → {baseUrl}");
Console.WriteLine($"Browser: {browserPath} ({(headed ? "headed" : "headless")}), throwaway profile, port {port}");

var browserArgs = new List<string>
{
    $"--remote-debugging-port={port}",
    $"--user-data-dir=\"{profile}\"",
    "--no-first-run",
    "--no-default-browser-check",
    "--disable-features=msEdgeSidebarV2,msHubApps",
    "about:blank",
};
if (!headed)
{
    browserArgs.Insert(0, "--headless=new");
}
else
{
    browserArgs.Insert(0, "--window-size=900,800");
}

using var browser = Process.Start(new ProcessStartInfo(browserPath, string.Join(' ', browserArgs)) { UseShellExecute = false })!;
await using var cdp = new Cdp();
var exitCode = 0;

try
{
    using var http = new HttpClient();
    JsonNode? version = null;
    for (var i = 0; i < 40 && version is null; i++)
    {
        try { version = await http.GetFromJsonAsync<JsonNode>($"http://127.0.0.1:{port}/json/version"); }
        catch (HttpRequestException) { await Task.Delay(250); }
    }

    await cdp.ConnectAsync(new Uri(version!["webSocketDebuggerUrl"]!.GetValue<string>()));

    // Two isolated "players" (separate cookies), like two different people.
    async Task<Tab> NewTabAsync(string? contextId)
    {
        var target = await cdp.SendAsync("Target.createTarget",
            contextId is null ? new { url = "about:blank" } : new { url = "about:blank", browserContextId = contextId });
        var session = await cdp.SendAsync("Target.attachToTarget",
            new { targetId = target!["targetId"]!.GetValue<string>(), flatten = true });
        var sessionId = session!["sessionId"]!.GetValue<string>();
        await cdp.SendAsync("Page.enable", null, sessionId);
        await cdp.SendAsync("Runtime.enable", null, sessionId);
        return new Tab(cdp, sessionId);
    }

    async Task LoginAsync(Tab tab, string name, string returnUrl)
    {
        await tab.NavigateAsync($"{baseUrl}/giris?returnUrl={Uri.EscapeDataString(returnUrl)}");
        await tab.EvalAsync($"document.querySelector('input[name=name]').value = '{name}'; document.querySelector('form').submit(); true");
        await tab.WaitForAsync($"location.pathname === '{returnUrl}'", TimeSpan.FromSeconds(15));
    }

    var hostContext = (await cdp.SendAsync("Target.createBrowserContext"))!["browserContextId"]!.GetValue<string>();
    var player = await NewTabAsync(null);
    var host = await NewTabAsync(hostContext);
    string listingId = "";

    // The player's browser allows notifications, as if they clicked "Allow".
    await cdp.SendAsync("Browser.grantPermissions", new { origin = baseUrl, permissions = new[] { "notifications" } });

    await Step("Oyuncu giriş yapar", () => LoginAsync(player, $"E2E Oyuncu {run}", "/bildirimler"));

    await Step("'Bu cihazda bildirimleri aç' → tarayıcı push servisine abone olur, sunucu test bildirimi gönderir", async () =>
    {
        await player.WaitForTextAsync("Bu cihazda bildirimleri aç");
        await player.ClickButtonAsync("Bu cihazda bildirimleri aç");
        await player.WaitForAsync(
            "document.body.innerText.includes('Az önce bir test bildirimi gönderdim') || document.body.innerText.includes('iletilemedi') || document.body.innerText.includes('açılamadı')",
            TimeSpan.FromSeconds(30));
        var text = await player.TextAsync();
        if (!text.Contains("Az önce bir test bildirimi gönderdim"))
        {
            throw new InvalidOperationException("Sunucu push servisine iletemedi: " + text[..Math.Min(text.Length, 300)]);
        }
    });

    await Step("Test bildirimi gerçekten sistem bildirimi olarak görünür", async () =>
    {
        await player.WaitForAsync(
            "navigator.serviceWorker.ready.then(r => r.getNotifications()).then(ns => ns.some(n => n.title.includes('bildirimleri açık')))",
            TimeSpan.FromSeconds(30));
        Console.WriteLine($"       bildirimler: {await player.NotificationTitlesAsync()}");
    });

    await Step("Oyuncu 'Şu an müsaitim' der (Destek, Altın)", async () =>
    {
        await player.NavigateAsync($"{baseUrl}/havuz");
        await player.WaitForTextAsync("Şu an müsaitim");
        await player.ClickButtonAsync("Şu an müsaitim");
        await player.WaitForTextAsync("Havuzdasın");
    });

    await Step("Başka bir oyuncu ilan açar (Destek, Altın–Platin)", async () =>
    {
        await LoginAsync(host, $"E2E Host {run}", "/ilan-ac");
        await host.WaitForTextAsync("İlanı aç ve haber ver");
        await host.ClickButtonAsync("İlanı aç ve haber ver");
        await host.WaitForAsync("location.pathname.startsWith('/ilan/')", TimeSpan.FromSeconds(15));
        listingId = (await host.EvalAsync("location.pathname.split('/')[2]"))!.GetValue<string>();
        Console.WriteLine($"       ilan #{listingId}");
    });

    await Step("Oyuncuya 'Destek lazım' push bildirimi gelir", async () =>
    {
        await player.WaitForAsync(
            "navigator.serviceWorker.ready.then(r => r.getNotifications()).then(ns => ns.some(n => n.title.includes('Destek lazım')))",
            TimeSpan.FromSeconds(30));
        Console.WriteLine($"       bildirimler: {await player.NotificationTitlesAsync()}");
    });

    await Step("Kopyalanan ?katil bağlantısı başkasını ilana oturtmaz", async () =>
    {
        // A forged or forwarded link without this player's personal token must not claim a seat.
        await player.NavigateAsync($"{baseUrl}/ilan/{listingId}?katil=1");
        await player.WaitForAsync("!location.search.includes('katil')", TimeSpan.FromSeconds(10));
        await player.WaitForTextAsync("Katıl");
        var text = (await player.EvalAsync("document.body.innerText"))!.GetValue<string>();
        if (text.Contains("İlan sahibi seni oyunda davet edecek"))
        {
            throw new InvalidOperationException("Tokensız bağlantı yeri kaptı.");
        }
    });

    await Step("Bildirimdeki kişisel 'Katıl' bağlantısı yeri kapar", async () =>
    {
        var joinUrl = (await player.EvalAsync(
            "navigator.serviceWorker.ready.then(r => r.getNotifications()).then(ns => ns.find(n => n.title.includes('Destek lazım')).data.joinUrl)"))!
            .GetValue<string>();
        await player.NavigateAsync(new Uri(new Uri(baseUrl), joinUrl).ToString());
        await player.WaitForTextAsync("İlan sahibi seni oyunda davet edecek");
        await player.WaitForAsync("!location.search.includes('katil')", TimeSpan.FromSeconds(10));
    });

    await Step("İlan sahibinin ekranı canlı güncellenir: 'Kadro tamam'", () => host.WaitForTextAsync("Kadro tamam"));
}
catch (Exception ex) when (failures == 0)
{
    failures++;
    Console.WriteLine($"  ❌ Kurulum hatası: {ex.Message}");
}
finally
{
    try { await cdp.SendAsync("Browser.close", timeout: TimeSpan.FromSeconds(5)); } catch { /* already gone */ }
    if (!browser.WaitForExit(5000))
    {
        browser.Kill(entireProcessTree: true);
    }

    for (var i = 0; i < 10; i++)
    {
        try { Directory.Delete(profile, recursive: true); break; }
        catch (IOException) { await Task.Delay(500); }
        catch (UnauthorizedAccessException) { await Task.Delay(500); }
    }

    Console.WriteLine(failures == 0 ? "SONUÇ: BAŞARILI ✅" : $"SONUÇ: BAŞARISIZ ❌ ({failures})");
    exitCode = failures == 0 ? 0 : 1;
}

return exitCode;
