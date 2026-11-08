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
    string JoinUrl);

/// <summary>
/// Announces website listings in chosen Discord channels with a link back to the site, so players who
/// are still on Discord can find them. No pings: it is an announcement, not a notification wave.
/// </summary>
public sealed class DiscordFeed(DiscordSocketClient client, IOptions<DiscordOptions> options, ILogger<DiscordFeed> logger)
{
    public bool Enabled => options.Value.FeedChannelIds.Length > 0;

    public static (Embed Embed, MessageComponent Components) Render(FeedListing l)
    {
        var expires = new DateTimeOffset(DateTime.SpecifyKind(l.ExpiresAtUtc, DateTimeKind.Utc)).ToUnixTimeSeconds();
        var embed = new EmbedBuilder()
            .WithTitle($"🌐 Flex için {TurkishNames.Of(l.Role)} lazım")
            .WithColor(new Color(0xC8AA6E))
            .WithDescription(l.Note is null ? "PreBot sitesinde açılan ilan." : $"> {l.Note}")
            .AddField("Rank", TurkishNames.RankRange(l.MinRank, l.MaxRank), inline: true)
            .AddField("Eksik", $"{l.SlotsMissing} kişi", inline: true)
            .AddField("Kapanış", $"<t:{expires}:R>", inline: true)
            .AddField("İlan sahibi", l.HostName, inline: true)
            .WithFooter(ListingRenderer.Disclaimer)
            .Build();

        var components = new ComponentBuilder()
            .WithButton("Sitede katıl", url: l.JoinUrl, style: ButtonStyle.Link, emote: new Emoji("🎮"))
            .Build();

        return (embed, components);
    }

    /// <summary>Posts the listing to every feed channel. Never throws.</summary>
    public async Task PostAsync(FeedListing listing, CancellationToken ct = default)
    {
        if (!Enabled || client.ConnectionState != ConnectionState.Connected)
        {
            return;
        }

        var (embed, components) = Render(listing);
        foreach (var channelId in options.Value.FeedChannelIds)
        {
            try
            {
                var channel = client.GetChannel(channelId) as IMessageChannel
                    ?? await client.Rest.GetChannelAsync(channelId) as IMessageChannel;
                if (channel is null)
                {
                    logger.LogWarning("Feed channel {ChannelId} not found or not a text channel", channelId);
                    continue;
                }

                var message = await channel.SendMessageAsync(embed: embed, components: components, allowedMentions: AllowedMentions.None);
                logger.LogInformation("Announced website listing {ListingId} in channel {ChannelId} (message {MessageId})",
                    listing.Id, channelId, message.Id);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Could not post listing {ListingId} to feed channel {ChannelId}", listing.Id, channelId);
            }
        }
    }
}
