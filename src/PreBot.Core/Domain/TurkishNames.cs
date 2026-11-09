namespace PreBot.Core.Domain;

/// <summary>Turkish display names shown to users.</summary>
public static class TurkishNames
{
    /// <summary>
    /// Role names as Turkish players write them in LFG posts ("sup lazım", "jungle lazım"), one set everywhere.
    /// The client's own words (Üst, Orman, Orta, Alt, Destek, Doldur) appear next to them where there is room.
    /// </summary>
    public static string Of(Role role) => role switch
    {
        Role.Top => "Top",
        Role.Jungle => "Jungle",
        Role.Mid => "Mid",
        Role.Adc => "ADC",
        Role.Support => "Support",
        Role.Fill => "Fill",
        _ => role.ToString(),
    };

    /// <summary>Label for pickers: the player word plus the client word, e.g. "Jungle (Orman)".</summary>
    public static string OptionOf(Role role) => role switch
    {
        Role.Top => "Top (Üst)",
        Role.Jungle => "Jungle (Orman)",
        Role.Mid => "Mid (Orta)",
        Role.Adc => "ADC (Alt)",
        Role.Support => "Support (Destek)",
        Role.Fill => "Fill (fark etmez)",
        _ => role.ToString(),
    };

    public static string Of(Rank rank) => rank switch
    {
        Rank.Iron => "Demir",
        Rank.Bronze => "Bronz",
        Rank.Silver => "Gümüş",
        Rank.Gold => "Altın",
        Rank.Platinum => "Platin",
        Rank.Emerald => "Zümrüt",
        Rank.Diamond => "Elmas",
        Rank.MasterPlus => "Master+",
        _ => rank.ToString(),
    };

    public static string RankRange(Rank min, Rank max) =>
        min == max ? Of(min) : $"{Of(min)} – {Of(max)}";
}
