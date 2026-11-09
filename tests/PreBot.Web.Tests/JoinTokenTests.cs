using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Time.Testing;
using PreBot.Core.Domain;
using PreBot.Web.Push;

namespace PreBot.Web.Tests;

public sealed class JoinTokenTests
{
    private readonly FakeTimeProvider _time = new(DateTimeOffset.UtcNow);
    private readonly JoinTokens _tokens;

    public JoinTokenTests() => _tokens = new JoinTokens(new EphemeralDataProtectionProvider(), _time);

    [Fact]
    public void Token_only_works_for_its_own_listing_and_user()
    {
        var token = _tokens.Create(listingId: 7, userId: 42);

        Assert.True(_tokens.IsValid(token, 7, 42));
        Assert.False(_tokens.IsValid(token, 7, 43)); // someone else opened a forwarded link
        Assert.False(_tokens.IsValid(token, 8, 42)); // another listing
    }

    [Fact]
    public void Missing_forged_or_expired_tokens_are_rejected()
    {
        var token = _tokens.Create(7, 42);

        Assert.False(_tokens.IsValid(null, 7, 42));
        Assert.False(_tokens.IsValid("1", 7, 42)); // the old "?katil=1" link
        Assert.False(_tokens.IsValid(token + "x", 7, 42));

        _time.Advance(JoinTokens.Lifetime + TimeSpan.FromMinutes(1));
        Assert.False(_tokens.IsValid(token, 7, 42));
    }

    [Fact]
    public void Listing_notification_has_a_personal_join_link_only_when_given_a_token()
    {
        var withToken = PushSender.ForListing(7, Role.Support, Rank.Gold, Rank.Gold, 1, _tokens.Create(7, 42));
        var withoutToken = PushSender.ForListing(7, Role.Support, Rank.Gold, Rank.Gold, 1);

        Assert.StartsWith("/ilan/7?katil=", withToken.JoinUrl);
        Assert.DoesNotContain("katil=1", withToken.JoinUrl);
        Assert.Null(withoutToken.JoinUrl);
    }

    [Fact]
    public void Notification_tells_players_in_the_client_which_lobby_to_join()
    {
        var push = PushSender.ForListing(7, Role.Support, Rank.Iron, Rank.Diamond, 1, null, Lobby.Hint(Role.Support, "4821", null));
        Assert.Contains("Özel oyuna katıl → PREBOT SUP 4821", push.Body);
    }

    [Fact]
    public void One_missing_seat_says_four_players_are_waiting()
    {
        Assert.StartsWith("4 kişi seni bekliyor", PushSender.ForListing(7, Role.Mid, Rank.Iron, Rank.Diamond, 1).Body);
        Assert.Contains("2 kişi eksik", PushSender.ForListing(7, Role.Mid, Rank.Iron, Rank.Diamond, 2).Body);
    }
}
