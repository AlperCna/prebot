using PreBot.Bot;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Web.Live;

/// <summary>Somewhere a website listing is announced (beyond the website itself).</summary>
public interface IListingFeed
{
    /// <summary>A new listing was opened.</summary>
    Task PublishAsync(Listing listing, CancellationToken ct = default);

    /// <summary>A listing changed (filled, reopened, closed, expired): bring its announcements up to date.</summary>
    Task UpdateAsync(int listingId, CancellationToken ct = default);
}

/// <summary>No announcements (the Discord bot is not configured).</summary>
public sealed class NoListingFeed : IListingFeed
{
    public Task PublishAsync(Listing listing, CancellationToken ct = default) => Task.CompletedTask;

    public Task UpdateAsync(int listingId, CancellationToken ct = default) => Task.CompletedTask;
}

/// <summary>Announces website listings in the configured Discord channels with a "Sitede katıl" link.</summary>
public sealed class DiscordListingFeed(
    DiscordFeed feed,
    PreService pre,
    AccountService accounts,
    AnnouncementStore announcements,
    IConfiguration config) : IListingFeed
{
    public async Task PublishAsync(Listing listing, CancellationToken ct = default)
    {
        if (!feed.Enabled)
        {
            return;
        }

        var posted = await feed.PostAsync(await ToFeedListingAsync(listing, ct), ct);
        foreach (var p in posted)
        {
            await announcements.SaveAsync(listing.Id, p.ChannelId, p.MessageId, ct);
        }
    }

    public async Task UpdateAsync(int listingId, CancellationToken ct = default)
    {
        var saved = await announcements.GetAsync(listingId, ct);
        if (saved.Count == 0)
        {
            return;
        }

        var view = await pre.GetListingViewAsync(listingId, ct);
        if (view is null || view.Listing.GuildId != Communities.Web)
        {
            return;
        }

        await feed.UpdateAsync(
            await ToFeedListingAsync(view.Listing, ct),
            saved.Select(a => new PostedAnnouncement(a.ChannelId, a.MessageId)),
            ct);
    }

    private async Task<FeedListing> ToFeedListingAsync(Listing l, CancellationToken ct)
    {
        var baseUrl = (config["PublicUrl"] ?? "http://localhost:5215").TrimEnd('/');
        var names = await accounts.GetDisplayNamesAsync([l.HostUserId], ct);
        return new FeedListing(
            l.Id, l.Role, l.MinRank, l.MaxRank, l.SlotsNeeded - l.SlotsFilled, l.Note,
            names.GetValueOrDefault(l.HostUserId, "Bir oyuncu"), l.ExpiresAtUtc, $"{baseUrl}/ilan/{l.Id}", l.Status);
    }
}
