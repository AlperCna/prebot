using System.Security.Cryptography;

namespace PreBot.Core.Domain;

/// <summary>
/// Custom game lobby that goes with a listing. Many groups already open a public custom game in the LoL
/// client ("FLEX 5. LAZIM") and wait for someone to join. PreBot can't open it (it is a website), but it
/// gives the lobby a recognizable name, and its notifications tell players who are in the client right now
/// to join that lobby directly. Both ways run at the same time; whichever is faster fills the listing.
/// </summary>
public static class Lobby
{
    /// <summary>A short random code that makes the lobby name unique (and doubles as its password).</summary>
    public static string NewCode() => RandomNumberGenerator.GetInt32(1000, 10000).ToString();

    /// <summary>The name to type in the client, ASCII only (e.g. "PREBOT DESTEK 4821").</summary>
    public static string Name(Role role, string code) => $"PREBOT {Word(role)} {code}";

    /// <summary>One line for notifications: where to find the lobby (and its password).</summary>
    public static string Hint(Role role, string code, bool locked) =>
        $"Oyundaysan: Özel oyuna katıl → {Name(role, code)}" + (locked ? $" (şifre {code})" : "");

    private static string Word(Role role) => role switch
    {
        Role.Top => "TOP",
        Role.Jungle => "ORMAN",
        Role.Mid => "ORTA",
        Role.Adc => "ADC",
        Role.Support => "DESTEK",
        _ => "FILL",
    };
}
