using System.Collections.Concurrent;

namespace PreBot.Core.Services;

/// <summary>Actions that send notifications to other people, and so must not be spammed.</summary>
public enum LimitedAction
{
    /// <summary>Opening or reopening a listing (each one notifies up to ~40 players).</summary>
    CreateListing,

    /// <summary>Taking a seat (every join and give-back moves notification waves).</summary>
    Join,

    /// <summary>"Send me a test notification".</summary>
    TestPush,

    /// <summary>Reporting a listing or a player.</summary>
    Report,
}

/// <summary>
/// Per-user sliding-window quotas. HTTP rate limiting cannot see these actions: on the website they are
/// button clicks over an already open SignalR connection, not new HTTP requests.
/// </summary>
public sealed class ActionLimiter(TimeProvider time)
{
    private static readonly IReadOnlyDictionary<LimitedAction, (int Count, TimeSpan Window)> Limits =
        new Dictionary<LimitedAction, (int, TimeSpan)>
        {
            // A group that plays several games in an evening opens a few listings; 6 an hour leaves room
            // for "expired → reopen" while stopping an open/close loop from flooding the pool.
            [LimitedAction.CreateListing] = (6, TimeSpan.FromHours(1)),
            [LimitedAction.Join] = (6, TimeSpan.FromMinutes(1)),
            [LimitedAction.TestPush] = (3, TimeSpan.FromMinutes(10)),
            [LimitedAction.Report] = (5, TimeSpan.FromHours(1)),
        };

    private readonly ConcurrentDictionary<(LimitedAction, ulong), Queue<DateTimeOffset>> _history = new();

    /// <summary>Records the action if the user is within quota. Otherwise returns false and when to retry.</summary>
    public bool TryAcquire(LimitedAction action, ulong userId, out TimeSpan retryAfter)
    {
        var (count, window) = Limits[action];
        var now = time.GetUtcNow();
        var queue = _history.GetOrAdd((action, userId), _ => new Queue<DateTimeOffset>());
        lock (queue)
        {
            while (queue.Count > 0 && now - queue.Peek() >= window)
            {
                queue.Dequeue();
            }

            if (queue.Count >= count)
            {
                retryAfter = window - (now - queue.Peek());
                return false;
            }

            queue.Enqueue(now);
            retryAfter = TimeSpan.Zero;
            return true;
        }
    }

    /// <summary>Turkish "try again in ..." text for a refused action.</summary>
    public static string RetryMessage(TimeSpan retryAfter)
    {
        var text = retryAfter < TimeSpan.FromMinutes(1)
            ? $"{Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds))} saniye"
            : $"{(int)Math.Ceiling(retryAfter.TotalMinutes)} dakika";
        return $"Biraz yavaşla: {text} sonra tekrar dene.";
    }
}
