using Discord;
using PreBot.Bot;
using PreBot.Core.Domain;

namespace PreBot.Tests;

public class DiscordFeedStatusTests
{
    private static FeedListing Listing(ListingStatus status) => new(
        7, Role.Jungle, Rank.Silver, Rank.Gold, 1, null, "Mert",
        new DateTime(2026, 11, 13, 21, 15, 0, DateTimeKind.Utc), "https://prebot.example/ilan/7", status);

    private static IEnumerable<ButtonComponent> Buttons(MessageComponent c) =>
        c.Components.SelectMany(row => ((ActionRowComponent)row).Components).OfType<ButtonComponent>();

    [Fact]
    public void Open_listing_has_a_join_button_and_a_site_link()
    {
        var (embed, components) = DiscordFeed.Render(Listing(ListingStatus.Open));

        Assert.StartsWith("🌐", embed.Title);
        var buttons = Buttons(components).ToList();
        Assert.Equal(2, buttons.Count);
        Assert.Equal(WebPoolModule.JoinPrefix + "7", buttons[0].CustomId);
        Assert.Equal("https://prebot.example/ilan/7", buttons[1].Url);
        Assert.Contains(embed.Fields, f => f.Name == "Kapanış");
    }

    [Theory]
    [InlineData(ListingStatus.Filled, "✅ Doldu")]
    [InlineData(ListingStatus.Expired, "⌛ Süresi doldu")]
    [InlineData(ListingStatus.Cancelled, "🚫 Kapatıldı")]
    public void Closed_listing_says_so_and_has_no_dead_link(ListingStatus status, string titleStart)
    {
        var (embed, components) = DiscordFeed.Render(Listing(status));

        Assert.StartsWith(titleStart, embed.Title);
        Assert.Empty(Buttons(components));
        Assert.DoesNotContain(embed.Fields, f => f.Name == "Kapanış");
    }
}
