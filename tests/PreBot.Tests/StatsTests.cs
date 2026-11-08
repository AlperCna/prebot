using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Tests;

public sealed class StatsTests : IDisposable
{
    private const ulong Web = Communities.Web;
    private readonly TestDb _t = new();
    private readonly StatsService _stats;

    public StatsTests() => _stats = new StatsService(_t.Factory, _t.Time);

    public void Dispose() => _t.Dispose();

    private async Task<int> OpenAsync(ulong host) =>
        (await _t.Service.CreateListingAsync(Web, Web, host, Role.Support, Rank.Gold, Rank.Gold, 1, null)).Listing!.Id;

    [Fact]
    public async Task Empty_community_has_no_rates()
    {
        var s = await _stats.GetAsync(Web, TimeSpan.FromDays(7));

        Assert.Equal(0, s.ListingsOpened);
        Assert.Null(s.FillRate);
        Assert.Null(s.MedianMinutesToFill);
    }

    [Fact]
    public async Task Fill_rate_and_median_time_to_fill_are_measured_on_finished_listings()
    {
        // Filled after 2 minutes.
        var a = await OpenAsync(1);
        _t.Time.Advance(TimeSpan.FromMinutes(2));
        await _t.Service.TryClaimAsync(a, 10);

        // Filled after 6 minutes.
        var b = await OpenAsync(2);
        _t.Time.Advance(TimeSpan.FromMinutes(6));
        await _t.Service.TryClaimAsync(b, 11);

        // Closed without anyone joining.
        var c = await OpenAsync(3);
        await _t.Service.CancelAsync(c, 3);

        // Still open: not counted as finished.
        await OpenAsync(4);

        var s = await _stats.GetAsync(Web, TimeSpan.FromDays(7));

        Assert.Equal(4, s.ListingsOpened);
        Assert.Equal(3, s.ListingsFinished);
        Assert.Equal(2, s.ListingsFilled);
        Assert.Equal(2.0 / 3, s.FillRate!.Value, 3);
        Assert.Equal(4, s.MedianMinutesToFill!.Value, 3); // median of 2 and 6
        Assert.Equal(2, s.FastestMinutesToFill!.Value, 3);
        Assert.Equal(6, s.ActivePlayers); // 4 hosts + 2 joiners
    }

    [Fact]
    public async Task Old_listings_fall_out_of_the_window_and_other_communities_are_ignored()
    {
        await OpenAsync(1);
        await _t.Service.CreateListingAsync(guildId: 555, channelId: 1, hostUserId: 2, Role.Top, Rank.Gold, Rank.Gold, 1, null);
        _t.Time.Advance(TimeSpan.FromDays(2));

        Assert.Equal(0, (await _stats.GetAsync(Web, TimeSpan.FromDays(1))).ListingsOpened);
        Assert.Equal(1, (await _stats.GetAsync(Web, TimeSpan.FromDays(7))).ListingsOpened);
    }
}
