using Microsoft.Extensions.Time.Testing;
using PreBot.Core.Services;

namespace PreBot.Tests;

public sealed class ActionLimiterTests
{
    private readonly FakeTimeProvider _time = new();
    private readonly ActionLimiter _limiter;

    public ActionLimiterTests() => _limiter = new ActionLimiter(_time);

    [Fact]
    public void Seventh_listing_within_an_hour_is_refused_then_allowed_again()
    {
        for (var i = 0; i < 6; i++)
        {
            Assert.True(_limiter.TryAcquire(LimitedAction.CreateListing, 1, out _));
            _time.Advance(TimeSpan.FromMinutes(5));
        }

        Assert.False(_limiter.TryAcquire(LimitedAction.CreateListing, 1, out var wait));
        Assert.Equal(TimeSpan.FromMinutes(30), wait); // the first one leaves the window at t=60

        _time.Advance(wait);
        Assert.True(_limiter.TryAcquire(LimitedAction.CreateListing, 1, out _));
    }

    [Fact]
    public void Limits_are_per_user_and_per_action()
    {
        for (var i = 0; i < 6; i++)
        {
            Assert.True(_limiter.TryAcquire(LimitedAction.Join, 1, out _));
        }

        Assert.False(_limiter.TryAcquire(LimitedAction.Join, 1, out _));
        Assert.True(_limiter.TryAcquire(LimitedAction.Join, 2, out _));
        Assert.True(_limiter.TryAcquire(LimitedAction.CreateListing, 1, out _));
    }

    [Theory]
    [InlineData(20, "Biraz yavaşla: 20 saniye sonra tekrar dene.")]
    [InlineData(150, "Biraz yavaşla: 3 dakika sonra tekrar dene.")]
    public void Retry_message_is_turkish_and_rounded_up(int seconds, string expected) =>
        Assert.Equal(expected, ActionLimiter.RetryMessage(TimeSpan.FromSeconds(seconds)));
}
