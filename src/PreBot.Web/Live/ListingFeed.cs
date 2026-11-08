using PreBot.Bot;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Web.Live;

/// <summary>Somewhere a new website listing is announced (beyond the website itself).</summary>
public interface IListingFeed
{
    Task PublishAsync(Listing listing, CancellationToken ct = default);
}

/// <summary>No announcements (the Discord bot is not configured).</summary>
public sealed class NoListingFeed : IListingFeed
{
    public Task PublishAsync(Listing listing, CancellationToken ct = default) => Task.CompletedTask;
}

/// <summary>Announces website listings in the configured Discord channels with a "Sitede katıl" link.</summary>
public sealed class DiscordListingFeed(DiscordFeed feed, AccountService accounts, IConfiguration config) : IListingFeed
{
    public async Task PublishAsync(Listing listing, CancellationToken ct = default)
    {
        if (!feed.Enabled)
        {
            return;
        }

        var baseUrl = (config["PublicUrl"] ?? "http://localhost:5215").TrimEnd('/');
        var names = await accounts.GetDisplayNamesAsync([listing.HostUserId], ct);
        await feed.PostAsync(new FeedListing(
            listing.Id,
            listing.Role,
            listing.MinRank,
            listing.MaxRank,
            listing.SlotsNeeded - listing.SlotsFilled,
            listing.Note,
            names.GetValueOrDefault(listing.HostUserId, "Bir oyuncu"),
            listing.ExpiresAtUtc,
            $"{baseUrl}/ilan/{listing.Id}"), ct);
    }
}
