using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using PreBot.Core.Domain;

namespace PreBot.Bot;

/// <summary>A website listing, as announced in Discord.</summary>
public sealed record FeedListing(
    int Id,
    Role Role,
    Rank MinRank,
    Rank MaxRank,
    int SlotsMissing,
    string? Note,
    string HostName,
    DateTime ExpiresAtUtc,
    string JoinUrl,
    ListingStatus Status = ListingStatus.Open,
    string? LobbyHint = null);

/// <summary>Where an announcement was posted.</summary>
public sealed record PostedAnnouncement(ulong ChannelId, ulong MessageId);

/// <summary>
/// Announces website listings in chosen Discord channels, with a "Katıl" button (takes the seat right from
/// Discord) and a link to the site. No pings: it is an announcement, not a notification wave.
/// When the listing fills or closes, the same message is edited, so nobody clicks a dead link.
/// </summary>
public sealed class DiscordFeed(DiscordSocketClient client, IOptions<DiscordOptions> options, ILogger<DiscordFeed> logger)
{
    public bool Enabled => options.Value.FeedChannelIds.Length > 0;

    public static (Embed Embed, MessageComponent Components) Render(FeedListing l)
    {
        var open = l.Status == ListingStatus.Open;
        var role = TurkishNames.Of(l.Role);
        var title = l.Status switch
        {
            ListingStatus.Open => $"🌐 Flex için {role} lazım",
            ListingStatus.Filled => $"✅ Doldu: {role} bulundu",
            ListingStatus.Expired => $"⌛ Süresi doldu: {role} ilanı",
            _ => $"🚫 Kapatıldı: {role} ilanı",
        };

        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithColor(open ? new Color(0xC8AA6E) : new Color(0x5B6578))
            .WithDescription(l.Note is null ? "PreBot sitesinde açılan ilan." : $"> {l.Note}")
            .AddField("Rank", TurkishNames.RankRange(l.MinRank, l.MaxRank), inline: true);

        if (open)
        {
            var expires = new DateTimeOffset(DateTime.SpecifyKind(l.ExpiresAtUtc, DateTimeKind.Utc)).ToUnixTimeSeconds();
            embed.AddField("Eksik", $"{l.SlotsMissing} kişi", inline: true)
                .AddField("Kapanış", $"<t:{expires}:R>", inline: true);
            if (l.LobbyHint is not null)
            {
                embed.AddField("🎮 Özel lobi", l.LobbyHint);
            }
        }

        embed.AddField("İlan sahibi", l.HostName, inline: true)
            .WithFooter(ListingRenderer.Disclaimer);

        var components = new ComponentBuilder();
        if (open)
        {
            components.WithButton("Katıl", WebPoolModule.JoinPrefix + l.Id, ButtonStyle.Success, new Emoji("✋"));
            components.WithButton("Sitede aç", url: l.JoinUrl, style: ButtonStyle.Link, emote: new Emoji("🎮"));
        }

        return (embed.Build(), components.Build());
    }

    /// <summary>
    /// Posts the listing to every feed channel, and to <paramref name="alsoChannelId"/> (where /pre-lazim was
    /// used). Never throws; returns where it was posted.
    /// </summary>
    public async Task<IReadOnlyList<PostedAnnouncement>> PostAsync(FeedListing listing, ulong? alsoChannelId = null, CancellationToken ct = default)
    {
        var posted = new List<PostedAnnouncement>();
        var channels = options.Value.FeedChannelIds
            .Concat(alsoChannelId is { } extra ? [extra] : [])
            .Distinct()
            .ToList();
        if (channels.Count == 0 || client.ConnectionState != ConnectionState.Connected)
        {
            return posted;
        }

        var (embed, components) = Render(listing);
        foreach (var channelId in channels)
        {
            try
            {
                var channel = await GetChannelAsync(channelId);
                if (channel is null)
                {
                    logger.LogWarning("Feed channel {ChannelId} not found or not a text channel", channelId);
                    continue;
                }

                var message = await channel.SendMessageAsync(embed: embed, components: components, allowedMentions: AllowedMentions.None);
                posted.Add(new PostedAnnouncement(channelId, message.Id));
                logger.LogInformation("Announced website listing {ListingId} in channel {ChannelId} (message {MessageId})",
                    listing.Id, channelId, message.Id);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Could not post listing {ListingId} to feed channel {ChannelId}", listing.Id, channelId);
            }
        }

        return posted;
    }

    /// <summary>Re-renders earlier announcements with the listing's current state. Never throws.</summary>
    public async Task UpdateAsync(FeedListing listing, IEnumerable<PostedAnnouncement> announcements, CancellationToken ct = default)
    {
        if (client.ConnectionState != ConnectionState.Connected)
        {
            return;
        }

        var (embed, components) = Render(listing);
        foreach (var a in announcements)
        {
            try
            {
                var channel = await GetChannelAsync(a.ChannelId);
                if (channel is null)
                {
                    continue;
                }

                await channel.ModifyMessageAsync(a.MessageId, m =>
                {
                    m.Embed = embed;
                    m.Components = components;
                    m.AllowedMentions = AllowedMentions.None;
                });
                logger.LogInformation("Updated announcement of listing {ListingId} ({Status}) in channel {ChannelId}",
                    listing.Id, listing.Status, a.ChannelId);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // The message may have been deleted by a moderator: nothing to update.
                logger.LogWarning(ex, "Could not update announcement {MessageId} of listing {ListingId}", a.MessageId, listing.Id);
            }
        }
    }

    private async Task<IMessageChannel?> GetChannelAsync(ulong channelId) =>
        client.GetChannel(channelId) as IMessageChannel
        ?? await client.Rest.GetChannelAsync(channelId) as IMessageChannel;
}
