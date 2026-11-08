using System.Net;
using System.Text.Json;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Web.Push;

/// <summary>What the service worker receives and turns into a system notification.</summary>
public sealed record PushPayload(string Title, string Body, string Url, string? JoinUrl, string Tag);

/// <summary>Sends Web Push messages to every registered device of the given users.</summary>
public sealed class PushSender(
    IHttpClientFactory httpClients,
    VapidKeys keys,
    SubscriptionService subscriptions,
    ILogger<PushSender> logger)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    /// <summary>Returns how many devices accepted the message.</summary>
    public async Task<int> SendAsync(IEnumerable<ulong> userIds, PushPayload payload, TimeSpan timeToLive, string? topic, CancellationToken ct = default)
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
        var body = JsonSerializer.Serialize(payload, Json);

        var results = await Task.WhenAll(devices.Select(async device =>
        {
            var subscription = new PushSubscription { Endpoint = device.Endpoint };
            subscription.SetKey(PushEncryptionKeyName.P256DH, device.P256dh);
            subscription.SetKey(PushEncryptionKeyName.Auth, device.Auth);
            var message = new PushMessage(body)
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
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Push to device {DeviceId} failed", device.Id);
                return (device.Id, Ok: false);
            }
        }));

        var delivered = results.Where(r => r.Ok).Select(r => r.Id).ToList();
        await subscriptions.MarkPushDeliveredAsync(delivered, ct);
        return delivered.Count;
    }

    public Task<int> SendTestAsync(ulong userId, CancellationToken ct = default) =>
        SendAsync(
            [userId],
            new PushPayload("🔔 PreBot bildirimleri açık", "Uygun bir ilan açılınca böyle haber vereceğim.", "/havuz", null, "test"),
            TimeSpan.FromMinutes(5),
            topic: null,
            ct);

    public static PushPayload ForListing(int listingId, Role role, Rank min, Rank max, int missing) => new(
        $"📣 {TurkishNames.Of(role)} lazım!",
        $"{TurkishNames.RankRange(min, max)} · {missing} kişi eksik. İlk basan alır.",
        $"/ilan/{listingId}",
        $"/ilan/{listingId}?katil=1",
        $"listing-{listingId}");
}
