using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Web.Push;

/// <summary>
/// When a listing fills or closes, the "Support lazım" notification on the devices of everyone who was told
/// about it is replaced (same tag, same push topic) by a quiet "Doldu" / "Kapandı". Nobody taps an old
/// "Katıl" for nothing, and a push still waiting at the push service is replaced before it is ever shown.
/// </summary>
public sealed class ListingClosedPush(PreService pre, PushSender push)
{
    public async Task SendAsync(int listingId, CancellationToken ct = default)
    {
        var view = await pre.GetListingViewAsync(listingId, ct);
        if (view is null || view.Listing.GuildId != Communities.Web || view.Listing.Status == ListingStatus.Open)
        {
            return;
        }

        var told = await pre.GetNotifiedUserIdsAsync(listingId, ct);
        if (told.Count == 0)
        {
            return;
        }

        await push.SendAsync(told, ForClosed(view.Listing), TimeSpan.FromMinutes(10), $"listing{listingId}", ct);
    }

    public static PushPayload ForClosed(Listing l)
    {
        var role = TurkishNames.Of(l.Role);
        var (title, body) = l.Status switch
        {
            ListingStatus.Filled => ($"Doldu: {role} ilanı", "Kadro tamamlandı, bu sefer olmadı. Sıradaki uygun ilanda yine haber vereceğim."),
            ListingStatus.Expired => ($"Kapandı: {role} ilanı", "İlanın süresi doldu. Sıradaki uygun ilanda yine haber vereceğim."),
            _ => ($"Kapandı: {role} ilanı", "İlan sahibi ilanı kapattı. Sıradaki uygun ilanda yine haber vereceğim."),
        };
        return new PushPayload(title, body, "/havuz", null, $"listing-{l.Id}", Quiet: true);
    }
}
