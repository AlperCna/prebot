namespace PreBot.Core.Domain;

/// <summary>Turkish display names shown to users.</summary>
public static class TurkishNames
{
    public static string Of(Role role) => role switch
    {
        Role.Top => "Top",
        Role.Jungle => "Orman",
        Role.Mid => "Orta",
        Role.Adc => "ADC",
        Role.Support => "Destek",
        Role.Fill => "Fark etmez",
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
