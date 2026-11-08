using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PreBot.PushE2E;

/// <summary>Minimal Chrome DevTools Protocol client (one browser WebSocket, flattened sessions).</summary>
public sealed class Cdp : IAsyncDisposable
{
    private readonly ClientWebSocket _ws = new();
    private readonly ConcurrentDictionary<int, TaskCompletionSource<JsonNode?>> _pending = new();
    private readonly CancellationTokenSource _cts = new();
    private int _nextId;
    private Task? _receiveLoop;

    public async Task ConnectAsync(Uri browserWsUrl)
    {
        await _ws.ConnectAsync(browserWsUrl, _cts.Token);
        _receiveLoop = Task.Run(ReceiveLoopAsync);
    }

    public async Task<JsonNode?> SendAsync(string method, object? parameters = null, string? sessionId = null, TimeSpan? timeout = null)
    {
        var id = Interlocked.Increment(ref _nextId);
        var tcs = new TaskCompletionSource<JsonNode?>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pending[id] = tcs;

        var message = new JsonObject { ["id"] = id, ["method"] = method };
        if (parameters is not null)
        {
            message["params"] = JsonSerializer.SerializeToNode(parameters);
        }

        if (sessionId is not null)
        {
            message["sessionId"] = sessionId;
        }

        var bytes = Encoding.UTF8.GetBytes(message.ToJsonString());
        await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, _cts.Token);

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout ?? TimeSpan.FromSeconds(30)));
        if (completed != tcs.Task)
        {
            _pending.TryRemove(id, out _);
            throw new TimeoutException($"CDP {method} timed out");
        }

        return await tcs.Task;
    }

    private async Task ReceiveLoopAsync()
    {
        var buffer = new byte[1 << 20];
        var sb = new StringBuilder();
        try
        {
            while (_ws.State == WebSocketState.Open && !_cts.IsCancellationRequested)
            {
                var result = await _ws.ReceiveAsync(buffer, _cts.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }

                sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                if (!result.EndOfMessage)
                {
                    continue;
                }

                var node = JsonNode.Parse(sb.ToString());
                sb.Clear();
                if (node?["id"] is { } idNode && _pending.TryRemove(idNode.GetValue<int>(), out var tcs))
                {
                    if (node["error"] is { } error)
                    {
                        tcs.TrySetException(new InvalidOperationException($"CDP error: {error.ToJsonString()}"));
                    }
                    else
                    {
                        tcs.TrySetResult(node["result"]);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (WebSocketException)
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        try
        {
            if (_ws.State == WebSocketState.Open)
            {
                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
        }
        catch (WebSocketException)
        {
        }

        if (_receiveLoop is not null)
        {
            await Task.WhenAny(_receiveLoop, Task.Delay(1000));
        }

        _ws.Dispose();
    }
}

/// <summary>One browser tab: navigate, run JavaScript, wait for conditions.</summary>
public sealed class Tab(Cdp cdp, string sessionId)
{
    public async Task NavigateAsync(string url)
    {
        await cdp.SendAsync("Page.navigate", new { url }, sessionId);
        await WaitForAsync("document.readyState === 'complete'", TimeSpan.FromSeconds(20));
    }

    public async Task<JsonNode?> EvalAsync(string expression)
    {
        var result = await cdp.SendAsync("Runtime.evaluate",
            new { expression, awaitPromise = true, returnByValue = true }, sessionId);
        if (result?["exceptionDetails"] is { } ex)
        {
            throw new InvalidOperationException($"JS error: {ex["exception"]?["description"] ?? ex.ToJsonString()}");
        }

        return result?["result"]?["value"];
    }

    public async Task<string> TextAsync() => (await EvalAsync("document.body ? document.body.innerText : ''"))?.GetValue<string>() ?? "";

    public async Task<string> UrlAsync() => (await EvalAsync("location.href"))?.GetValue<string>() ?? "";

    /// <summary>Polls a JavaScript boolean expression until true.</summary>
    public async Task WaitForAsync(string condition, TimeSpan timeout)
    {
        var until = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < until)
        {
            try
            {
                if ((await EvalAsync($"!!({condition})"))?.GetValue<bool>() == true)
                {
                    return;
                }
            }
            catch (InvalidOperationException)
            {
                // page navigating; try again
            }

            await Task.Delay(250);
        }

        throw new TimeoutException($"Timed out waiting for: {condition}");
    }

    public async Task WaitForTextAsync(string text, int seconds = 20)
    {
        try
        {
            await WaitForAsync($"document.body && document.body.innerText.includes({JsonSerializer.Serialize(text)})", TimeSpan.FromSeconds(seconds));
        }
        catch (TimeoutException)
        {
            var page = (await TextAsync()).ReplaceLineEndings(" ⏎ ");
            throw new TimeoutException($"'{text}' görünmedi. URL: {await UrlAsync()} | Sayfa: {page[..Math.Min(page.Length, 400)]}");
        }
    }

    /// <summary>Waits until an enabled button with this text exists (the page went live), then clicks it.</summary>
    public async Task ClickButtonAsync(string text)
    {
        var find = $"[...document.querySelectorAll('button')].find(x => x.textContent.includes({JsonSerializer.Serialize(text)}) && !x.disabled)";
        await WaitForAsync(find, TimeSpan.FromSeconds(20));
        await EvalAsync($"{find}.click(); true");
    }

    /// <summary>Titles of system notifications currently shown by this site's service worker.</summary>
    public async Task<string> NotificationTitlesAsync() =>
        (await EvalAsync("navigator.serviceWorker.ready.then(r => r.getNotifications()).then(ns => ns.map(n => n.title).join(' | '))"))
            ?.GetValue<string>() ?? "";
}
