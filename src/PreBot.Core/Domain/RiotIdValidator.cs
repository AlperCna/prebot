using System.Text.RegularExpressions;

namespace PreBot.Core.Domain;

/// <summary>Format check for Riot IDs: game name 3–16 characters, tag line 3–5 letters/digits.</summary>
public static partial class RiotIdValidator
{
    [GeneratedRegex(@"^[^#]{3,16}#[\p{L}\p{N}]{3,5}$")]
    private static partial Regex Pattern();

    public static bool TryNormalize(string? input, out string riotId)
    {
        riotId = (input ?? string.Empty).Trim();
        if (!Pattern().IsMatch(riotId))
        {
            return false;
        }

        var hash = riotId.IndexOf('#');
        riotId = $"{riotId[..hash].Trim()}#{riotId[(hash + 1)..].Trim()}";
        return riotId.IndexOf('#') >= 3;
    }
}
