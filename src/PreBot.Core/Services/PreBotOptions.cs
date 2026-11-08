namespace PreBot.Core.Services;

/// <summary>Tunable timings. Bound from the "PreBot" section of appsettings.json.</summary>
public sealed class PreBotOptions
{
    /// <summary>How long a new listing stays open.</summary>
    public int ListingMinutes { get; set; } = 15;

    /// <summary>When a seat is given back (left / no-show), the listing stays open at least this long.</summary>
    public int ReopenMinutes { get; set; } = 10;

    /// <summary>Minimum time between two notifications to the same person.</summary>
    public int PingCooldownMinutes { get; set; } = 15;

    /// <summary>Maximum seats a single listing can ask for (Flex parties: 1, 2, 3 or 5).</summary>
    public int MaxSlotsPerListing { get; set; } = 3;

    /// <summary>How often the background worker expires entries and sends due waves.</summary>
    public int SweepIntervalSeconds { get; set; } = 20;
}
