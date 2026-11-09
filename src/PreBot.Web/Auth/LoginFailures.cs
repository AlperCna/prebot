using System.Collections.Concurrent;

namespace PreBot.Web.Auth;

/// <summary>
/// Counts failed invite sign-ins (wrong invite code or wrong password), per client IP and per account name, so
/// guessing the code or a friend's password takes far too long. Successful sign-ins are not counted.
/// </summary>
public sealed class LoginFailures(TimeProvider time, ILogger<LoginFailures> logger)
{
    public static readonly TimeSpan IpWindow = TimeSpan.FromMinutes(15);
    public const int IpLimit = 10;
    public static readonly TimeSpan NameWindow = TimeSpan.FromHours(1);
    public const int NameLimit = 10;

    private readonly ConcurrentDictionary<string, Queue<DateTimeOffset>> _failures = new();

    public bool IsBlocked(string? ip, string? name) =>
        Count("ip:" + ip, IpWindow) >= IpLimit || (name is not null && Count("name:" + name, NameWindow) >= NameLimit);

    public void Record(string reason, string? ip, string? name)
    {
        logger.LogWarning("Invite sign-in failed ({Reason}) from {Ip}", reason, ip);
        Add("ip:" + ip);
        if (name is not null)
        {
            Add("name:" + name);
        }

        if (_failures.Count > 10_000)
        {
            // Drop keys whose failures are all older than the longest window.
            var cutoff = time.GetUtcNow() - NameWindow;
            foreach (var (key, queue) in _failures)
            {
                lock (queue)
                {
                    if (queue.Count == 0 || queue.Last() < cutoff)
                    {
                        _failures.TryRemove(key, out _);
                    }
                }
            }
        }
    }

    private int Count(string key, TimeSpan window)
    {
        if (!_failures.TryGetValue(key, out var queue))
        {
            return 0;
        }

        var cutoff = time.GetUtcNow() - window;
        lock (queue)
        {
            return queue.Count(t => t >= cutoff);
        }
    }

    private void Add(string key)
    {
        var queue = _failures.GetOrAdd(key, _ => new Queue<DateTimeOffset>());
        var now = time.GetUtcNow();
        lock (queue)
        {
            queue.Enqueue(now);
            while (queue.Count > 50)
            {
                queue.Dequeue();
            }
        }
    }
}
