using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Web.Push;

/// <summary>
/// Tells the listing's host what happened, on their phone/PC even when the site is closed — the host is
/// usually in the LoL client by then. Without this they would not see who joined or their Riot ID.
/// </summary>
public sealed class HostNotifier(PreService pre, AccountService accounts, PushSender push)
{
    private static readonly TimeSpan JoinTtl = TimeSpan.FromMinutes(10);

    public async Task JoinedAsync(int listingId, ulong joinerUserId, CancellationToken ct = default)
    {
        var view = await pre.GetListingViewAsync(listingId, ct);
        if (view is null || view.Listing.GuildId != Communities.Web)
        {
            return;
        }

        var l = view.Listing;
        var name = (await accounts.GetDisplayNamesAsync([joinerUserId], ct)).GetValueOrDefault(joinerUserId, "Bir oyuncu");
        var riotId = view.ActiveSlots.FirstOrDefault(s => s.UserId == joinerUserId)?.RiotId;
        var title = l.Status == ListingStatus.Filled ? $"✅ Kadro tamam! {name} katıldı" : $"🎉 {name} katıldı!";
        var body = riotId is null
            ? "Riot ID'sini kaydetmemiş; ilan sayfasından birlikte çözün."
            : $"Riot ID: {riotId} · Oyunda Arkadaş ekle → lobine davet et.";

        await push.SendAsync([l.HostUserId], new PushPayload(title, body, $"/ilan/{l.Id}", null, $"host-{l.Id}"), JoinTtl, null, ct);
    }

    public async Task LeftAsync(int listingId, ulong leaverUserId, CancellationToken ct = default)
    {
        var view = await pre.GetListingViewAsync(listingId, ct);
        if (view is null || view.Listing.GuildId != Communities.Web)
        {
            return;
        }

        var name = (await accounts.GetDisplayNamesAsync([leaverUserId], ct)).GetValueOrDefault(leaverUserId, "Katılan oyuncu");
        await push.SendAsync(
            [view.Listing.HostUserId],
            new PushPayload($"😕 {name} vazgeçti", "İlan yeniden açıldı, uygun oyunculara tekrar haber veriyorum.",
                $"/ilan/{listingId}", null, $"host-{listingId}"),
            JoinTtl, null, ct);
    }

    public async Task ExpiredAsync(int listingId, CancellationToken ct = default)
    {
        var view = await pre.GetListingViewAsync(listingId, ct);
        if (view is null || view.Listing.GuildId != Communities.Web || view.Listing.Status != ListingStatus.Expired)
        {
            return;
        }

        await push.SendAsync(
            [view.Listing.HostUserId],
            new PushPayload("⌛ İlanın süresi doldu", "Bu sefer kimse katılmadı. Tek dokunuşla tekrar açabilirsin.",
                $"/ilan/{listingId}", null, $"host-{listingId}"),
            TimeSpan.FromMinutes(30), null, ct);
    }
}
