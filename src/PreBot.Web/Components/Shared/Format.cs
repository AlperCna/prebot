using PreBot.Core.Domain;

namespace PreBot.Web.Components.Shared;

public static class Format
{
    public static IReadOnlyList<Role> Roles { get; } = Enum.GetValues<Role>();

    public static IReadOnlyList<Rank> Ranks { get; } = Enum.GetValues<Rank>();

    public static string TimeLeft(DateTime expiresAtUtc)
    {
        var left = DateTime.SpecifyKind(expiresAtUtc, DateTimeKind.Utc) - DateTime.UtcNow;
        if (left <= TimeSpan.Zero)
        {
            return "süresi doldu";
        }

        return left.TotalMinutes < 1 ? "1 dk'dan az" : $"{(int)Math.Ceiling(left.TotalMinutes)} dk";
    }

    public static string RoleLabel(Role role) => TurkishNames.Of(role);

    /// <summary>Picker label: player word plus the client's word, e.g. "Jungle (Orman)".</summary>
    public static string RoleOption(Role role) => TurkishNames.OptionOf(role);
}
