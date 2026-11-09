namespace PreBot.Core.Domain;

/// <summary>
/// Links to public stats pages for a Riot ID, so a host can check a stranger in one tap instead of copying the
/// Riot ID into another site. No API key is needed for a link.
/// </summary>
public static class ProfileLinks
{
    /// <summary>OP.GG profile on the Turkish server, e.g. https://op.gg/lol/summoners/tr/Ali%20Veli-TR1.</summary>
    public static string? OpGg(string? riotId)
    {
        // Split on the last '#': a raw '#' in a URL would cut off everything after it.
        var hash = riotId?.LastIndexOf('#') ?? -1;
        if (riotId is null || hash <= 0 || hash == riotId.Length - 1)
        {
            return null;
        }

        var name = Uri.EscapeDataString(riotId[..hash].Trim());
        var tag = Uri.EscapeDataString(riotId[(hash + 1)..].Trim());
        return $"https://op.gg/lol/summoners/tr/{name}-{tag}";
    }
}
