using System.Net;
using System.Text.Json;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Web.Push;

/// <summary>What the service worker receives and turns into a system notification.</summary>
/// <param name="OpenedUrl">The service worker posts here when the notification is opened (trial measurement).</param>
/// <param name="InGameUrl">"Maçtayım, sonra" button: the service worker posts here.</param>
/// <param name="Quiet">Replaces an earlier notification with the same tag without sound or vibration.</param>
public sealed record PushPayload(
    string Title, string Body, string Url, string? JoinUrl, string Tag, string? MuteUrl = null,
    string? OpenedUrl = null, string? InGameUrl = null, bool Quiet = false);

/// <summary>Sends Web Push messages to every registered device of the given users.</summary>
public sealed class PushSender(
    IHttpClientFactory httpClients,
    VapidKeys keys,
    SubscriptionService subscriptions,
    ILogger<PushSender> logger)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    /// <summary>Returns how many devices accepted the message.</summary>
    public Task<int> SendAsync(IEnumerable<ulong> userIds, PushPayload payload, TimeSpan timeToLive, string? topic, CancellationToken ct = default) =>
        SendAsync(userIds, _ => payload, timeToLive, topic, ct);

    /// <summary>Same, with a payload per user (e.g. a personal join link). Returns how many devices accepted it.</summary>
    public async Task<int> SendAsync(IEnumerable<ulong> userIds, Func<ulong, PushPayload> payloadFor, TimeSpan timeToLive, string? topic, CancellationToken ct = default)
    {
        var devices = await subscriptions.GetPushSubscriptionsAsync(userIds, ct);
        if (devices.Count == 0)
        {
            return 0;
        }

        var client = new PushServiceClient(httpClients.CreateClient("webpush"))
        {
            DefaultAuthentication = new VapidAuthentication(keys.PublicKey, keys.PrivateKey) { Subject = keys.Subject },
        };
        var bodies = devices.Select(d => d.UserId).Distinct()
            .ToDictionary(id => id, id => JsonSerializer.Serialize(payloadFor(id), Json));

        var results = await Task.WhenAll(devices.Select(async device =>
        {
            var subscription = new PushSubscription { Endpoint = device.Endpoint };
            subscription.SetKey(PushEncryptionKeyName.P256DH, device.P256dh);
            subscription.SetKey(PushEncryptionKeyName.Auth, device.Auth);
            var message = new PushMessage(bodies[device.UserId])
            {
                // High urgency + short TTL: "your group needs you now" is worthless 2 minutes later.
                Urgency = PushMessageUrgency.High,
                TimeToLive = (int)timeToLive.TotalSeconds,
                Topic = topic,
            };

            try
            {
                await client.RequestPushMessageDeliveryAsync(subscription, message, ct);
                return (device.Id, Ok: true);
            }
            catch (PushServiceClientException ex) when (ex.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone)
            {
                // The browser unsubscribed or the subscription expired: forget it.
                await subscriptions.RemovePushSubscriptionAsync(device.Endpoint, ct);
                return (device.Id, Ok: false);
            }
            catch (PushServiceClientException ex)
            {
                // 413: payload too big, 429: push service rate limit (Retry-After), 5xx: service trouble.
                logger.LogWarning("Push to device {DeviceId} refused: {Status} {Reason}", device.Id, (int)ex.StatusCode, ex.Message);
                return (device.Id, Ok: false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Push to device {DeviceId} failed", device.Id);
                return (device.Id, Ok: false);
            }
        }));

        var delivered = results.Where(r => r.Ok).Select(r => r.Id).ToList();
        await subscriptions.MarkPushDeliveredAsync(delivered, ct);
        logger.LogInformation("Push sent: {Delivered}/{Devices} devices accepted", delivered.Count, devices.Count);
        return delivered.Count;
    }

    public Task<int> SendTestAsync(ulong userId, CancellationToken ct = default) =>
        SendAsync(
            [userId],
            new PushPayload("PreBot bildirimleri açık", "Uygun bir ilan açılınca böyle haber vereceğim.", "/havuz", null, "test"),
            TimeSpan.FromMinutes(5),
            topic: null,
            ct);

    /// <param name="joinToken">Personal token from <see cref="JoinTokens"/>; without it the link only opens the listing.</param>
    /// <param name="lobbyHint">"Oyundaysan: Özel oyuna katıl → ..." when the host also opened a custom game lobby.</param>
    public static PushPayload ForListing(
        int listingId, Role role, Rank min, Rank max, int missing, string? joinToken = null, string? lobbyHint = null,
        string? muteToken = null) => new(
        $"{TurkishNames.Of(role)} lazım",
        (missing == 1
            ? $"4 kişi seni bekliyor · {TurkishNames.RankRange(min, max)}. İlk basan alır."
            : $"{TurkishNames.RankRange(min, max)} · {missing} kişi eksik. İlk basan alır.")
            + (lobbyHint is null ? "" : $"\n{lobbyHint}"),
        $"/ilan/{listingId}",
        joinToken is null ? null : $"/ilan/{listingId}?katil={Uri.EscapeDataString(joinToken)}",
        $"listing-{listingId}",
        muteToken is null ? null : $"/api/sustur?t={Uri.EscapeDataString(muteToken)}",
        joinToken is null ? null : $"/api/bildirim/acildi?t={Uri.EscapeDataString(joinToken)}",
        joinToken is null ? null : $"/api/bildirim/oyundayim?t={Uri.EscapeDataString(joinToken)}");
}
