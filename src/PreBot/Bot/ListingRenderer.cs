using System.Text;
using Discord;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Bot;

/// <summary>Builds the public listing message (embed + buttons) from the current listing state.</summary>
public static class ListingRenderer
{
    public const string JoinPrefix = "pre:join:";
    public const string LeavePrefix = "pre:leave:";
    public const string NoShowPrefix = "pre:noshow:";
    public const string CancelPrefix = "pre:cancel:";

    public const string Disclaimer =
        "PreBot, Riot Games tarafından onaylanmamıştır. • Master+ biriyle partide herkes en az Zümrüt olmalı.";

    public static (Embed Embed, MessageComponent Components) Render(ListingView view, int? potentialMatches = null)
    {
        var l = view.Listing;
        var missing = l.SlotsNeeded - l.SlotsFilled;

        var (title, color) = l.Status switch
        {
            ListingStatus.Open => ($"🔎 Flex için {missing} kişi aranıyor", Color.Green),
            ListingStatus.Filled => ("✅ Kadro tamam, iyi oyunlar!", Color.Blue),
            ListingStatus.Expired => ("⌛ İlanın süresi doldu", Color.LightGrey),
            _ => ("🚫 İlan iptal edildi", Color.DarkGrey),
        };

        var description = new StringBuilder($"<@{l.HostUserId}> grubuna katıl.");
        if (view.HostRiotId is not null)
        {
            description.Append($"\nİlan sahibi Riot ID: `{view.HostRiotId}`");
        }

        if (l.Note is not null)
        {
            description.Append($"\n> {l.Note}");
        }

        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithColor(color)
            .WithDescription(description.ToString())
            .AddField("Rol", TurkishNames.Of(l.Role), inline: true)
            .AddField("Rank", TurkishNames.RankRange(l.MinRank, l.MaxRank), inline: true)
            .AddField("Doluluk", $"{l.SlotsFilled}/{l.SlotsNeeded}", inline: true);

        if (l.Status == ListingStatus.Open)
        {
            embed.AddField("Kapanış", $"<t:{new DateTimeOffset(DateTime.SpecifyKind(l.ExpiresAtUtc, DateTimeKind.Utc)).ToUnixTimeSeconds()}:R>", inline: true);
            if (potentialMatches is not null)
            {
                embed.AddField(
                    "Havuzda uygun kişi",
                    potentialMatches == 0 ? "Şu an yok, yeni gelenlere haber verilecek" : potentialMatches.Value.ToString(),
                    inline: true);
            }
        }

        if (view.ActiveSlots.Count > 0)
        {
            var joined = string.Join("\n", view.ActiveSlots.Select(s =>
                s.RiotId is null ? $"<@{s.UserId}> (Riot ID kayıtlı değil)" : $"<@{s.UserId}> — `{s.RiotId}`"));
            embed.AddField("Katılanlar", joined);
        }

        embed.WithFooter(Disclaimer);

        var components = new ComponentBuilder();
        if (l.Status is ListingStatus.Open or ListingStatus.Filled)
        {
            components.WithButton("Katıl", JoinPrefix + l.Id, ButtonStyle.Success, disabled: l.Status != ListingStatus.Open)
                .WithButton("Vazgeç", LeavePrefix + l.Id, ButtonStyle.Secondary)
                .WithButton("Gelmedi, yeniden ara", NoShowPrefix + l.Id, ButtonStyle.Secondary)
                .WithButton("İlanı kapat", CancelPrefix + l.Id, ButtonStyle.Danger);
        }

        return (embed.Build(), components.Build());
    }
}
