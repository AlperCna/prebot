using Discord;
using PreBot.Bot;
using PreBot.Core.Domain;

namespace PreBot.Tests;

public sealed class DiscordFeedTests : IDisposable
{
    private readonly TestDb _t = new();

    public void Dispose() => _t.Dispose();

    [Fact]
    public void Announcement_links_back_to_the_site_and_shows_the_essentials()
    {
        var (embed, components) = DiscordFeed.Render(new FeedListing(
            42, Role.Support, Rank.Gold, Rank.Platinum, 1, "mikrofon şart", "Mert",
            new DateTime(2026, 11, 13, 21, 15, 0, DateTimeKind.Utc), "https://prebot.example/ilan/42"));

        Assert.Contains("Destek", embed.Title);
        Assert.Contains("mikrofon şart", embed.Description);
        Assert.Contains(embed.Fields, f => f.Name == "Rank" && f.Value == "Altın – Platin");
        Assert.Contains(embed.Fields, f => f.Name == "Eksik" && f.Value == "1 kişi");
        Assert.Contains(embed.Fields, f => f.Name == "Kapanış" && f.Value.StartsWith("<t:"));

        var buttons = components.Components.SelectMany(row => ((ActionRowComponent)row).Components).OfType<ButtonComponent>().ToList();
        Assert.Equal(WebPoolModule.JoinPrefix + "42", buttons[0].CustomId); // take the seat right from Discord
        var link = buttons[1];
        Assert.Equal(ButtonStyle.Link, link.Style);
        Assert.Equal("https://prebot.example/ilan/42", link.Url);
    }

    [Theory]
    [InlineData(null, new ulong[0])]
    [InlineData("", new ulong[0])]
    [InlineData("123", new ulong[] { 123 })]
    [InlineData("123, 456;123  abc", new ulong[] { 123, 456 })]
    public void Feed_channel_setting_is_tolerant(string? value, ulong[] expected) =>
        Assert.Equal(expected, new DiscordOptions { FeedChannels = value }.FeedChannelIds);

    [Fact]
    public async Task Web_and_discord_workers_each_take_only_their_own_due_waves()
    {
        const ulong discordServer = 555;
        var web = (await _t.Service.CreateListingAsync(Communities.Web, Communities.Web, 1, Role.Mid, Rank.Gold, Rank.Gold, 1, null)).Listing!.Id;
        var discord = (await _t.Service.CreateListingAsync(discordServer, 9, 2, Role.Mid, Rank.Gold, Rank.Gold, 1, null)).Listing!.Id;

        Assert.Equal([web], await _t.Service.GetDueWaveListingIdsAsync(onlyCommunity: Communities.Web));
        Assert.Equal([discord], await _t.Service.GetDueWaveListingIdsAsync(exceptCommunity: Communities.Web));
        Assert.Equal(2, (await _t.Service.GetDueWaveListingIdsAsync()).Count);
    }
}
