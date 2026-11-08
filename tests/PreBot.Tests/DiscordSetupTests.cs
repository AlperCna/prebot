using Microsoft.Extensions.Configuration;
using PreBot.Bot;

namespace PreBot.Tests;

public class DiscordSetupTests
{
    private static IConfiguration Config(params (string Key, string? Value)[] values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values.Select(v => new KeyValuePair<string, string?>(v.Key, v.Value))).Build();

    [Fact]
    public void Bot_runs_only_with_a_token_and_can_be_switched_off()
    {
        Assert.False(DiscordSetup.IsConfigured(Config()));
        Assert.False(DiscordSetup.IsConfigured(Config(("Discord:Token", "  "))));
        Assert.True(DiscordSetup.IsConfigured(Config(("Discord:Token", "x.y.z"))));
        Assert.False(DiscordSetup.IsConfigured(Config(("Discord:Token", "x.y.z"), ("Discord:Enabled", "false"))));
        Assert.True(DiscordSetup.IsConfigured(Config(("Discord:Token", "x.y.z"), ("Discord:Enabled", "true"))));
    }
}
