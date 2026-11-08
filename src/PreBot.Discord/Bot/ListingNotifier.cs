using Discord;
using Discord.WebSocket;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Bot;

/// <summary>Everything that talks to Discord channels: posting, refreshing, and pinging for a listing.</summary>
public sealed class ListingNotifier(DiscordSocketClient client, PreService pre, ILogger<ListingNotifier> logger)
{
    public async Task<ulong> PostListingAsync(IMessageChannel channel, int listingId, int potentialMatches, CancellationToken ct = default)
    {
        var view = await pre.GetListingViewAsync(listingId, ct)
            ?? throw new InvalidOperationException($"Listing {listingId} not found right after creation.");
        var (embed, components) = ListingRenderer.Render(view, potentialMatches);

        var message = await channel.SendMessageAsync(
            embed: embed,
            components: components,
            allowedMentions: AllowedMentions.None);
        await pre.AttachMessageAsync(listingId, message.Id, ct);
        return message.Id;
    }

    /// <summary>Re-renders the listing message so it shows the latest state. Never throws.</summary>
    public async Task RefreshAsync(int listingId, CancellationToken ct = default)
    {
        try
        {
            var view = await pre.GetListingViewAsync(listingId, ct);
            if (view?.Listing.MessageId is not { } messageId)
            {
                return;
            }

            var channel = await GetChannelAsync(view.Listing.ChannelId);
            if (channel is null)
            {
                return;
            }

            var (embed, components) = ListingRenderer.Render(view);
            await channel.ModifyMessageAsync(messageId, m =>
            {
                m.Embed = embed;
                m.Components = components;
                m.AllowedMentions = AllowedMentions.None;
            });
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Could not refresh listing {ListingId}", listingId);
        }
    }

    /// <summary>
    /// Sends the next notification wave if it is due: one short reply under the listing that mentions
    /// only the chosen players (no role or @everyone pings).
    /// </summary>
    public async Task RunWaveAsync(int listingId, CancellationToken ct = default)
    {
        var wave = await pre.RunNextWaveAsync(listingId, ct);
        if (wave is null || wave.UserIds.Count == 0)
        {
            return;
        }

        try
        {
            var view = await pre.GetListingViewAsync(listingId, ct);
            if (view?.Listing.MessageId is not { } messageId)
            {
                return;
            }

            var channel = await GetChannelAsync(view.Listing.ChannelId);
            if (channel is null)
            {
                return;
            }

            var l = view.Listing;
            var mentions = string.Join(" ", wave.UserIds.Select(id => $"<@{id}>"));
            var text = $"{mentions}\n📣 **{TurkishNames.Of(l.Role)}** lazım ({TurkishNames.RankRange(l.MinRank, l.MaxRank)}). " +
                       "Müsaitsen ilandaki **Katıl** butonuna bas. İlk basan alır.";

            await channel.SendMessageAsync(
                text,
                allowedMentions: new AllowedMentions(AllowedMentionTypes.None) { UserIds = wave.UserIds.ToList() },
                messageReference: new MessageReference(messageId, failIfNotExists: false));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Could not send wave {Wave} for listing {ListingId}", wave.Wave, listingId);
        }
    }

    public async Task AnnounceJoinAsync(int listingId, ulong joinedUserId, CancellationToken ct = default)
    {
        try
        {
            var view = await pre.GetListingViewAsync(listingId, ct);
            if (view?.Listing.MessageId is not { } messageId)
            {
                return;
            }

            var channel = await GetChannelAsync(view.Listing.ChannelId);
            if (channel is null)
            {
                return;
            }

            var riotId = view.ActiveSlots.FirstOrDefault(s => s.UserId == joinedUserId)?.RiotId;
            var who = riotId is null
                ? $"<@{joinedUserId}> katıldı. Riot ID kayıtlı değil, kendisine sor."
                : $"<@{joinedUserId}> katıldı. Davet et: `{riotId}`";

            await channel.SendMessageAsync(
                $"<@{view.Listing.HostUserId}> 🎉 {who}",
                allowedMentions: new AllowedMentions(AllowedMentionTypes.None)
                {
                    UserIds = [view.Listing.HostUserId, joinedUserId],
                },
                messageReference: new MessageReference(messageId, failIfNotExists: false));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Could not announce join for listing {ListingId}", listingId);
        }
    }

    public async Task MarkRemovedAsync(DeletedListingMessage removed)
    {
        try
        {
            var channel = await GetChannelAsync(removed.ChannelId);
            if (channel is null)
            {
                return;
            }

            await channel.ModifyMessageAsync(removed.MessageId, m =>
            {
                m.Embed = new EmbedBuilder()
                    .WithTitle("🗑️ Bu ilan kaldırıldı")
                    .WithDescription("İlan sahibi verilerinin silinmesini istedi.")
                    .WithColor(Color.DarkGrey)
                    .Build();
                m.Components = new ComponentBuilder().Build();
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not mark removed listing message {MessageId}", removed.MessageId);
        }
    }

    private async Task<IMessageChannel?> GetChannelAsync(ulong channelId)
    {
        if (client.GetChannel(channelId) is IMessageChannel cached)
        {
            return cached;
        }

        return await client.Rest.GetChannelAsync(channelId) as IMessageChannel;
    }
}
