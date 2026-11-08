using Discord;
using Discord.Interactions;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Bot;

/// <summary>Slash commands and button handlers. All personal replies are ephemeral (only the user sees them).</summary>
[CommandContextType(InteractionContextType.Guild)]
public sealed class PreModule(PreService pre, ListingNotifier notifier, PreBotOptions options)
    : InteractionModuleBase<SocketInteractionContext>
{
    private const string PrivacyNotice =
        "🔒 **Gizlilik:** PreBot yalnızca Discord kimliğini, seçtiğin rol/rank'ı, müsaitlik süreni ve " +
        "istersen Riot ID'ni saklar. Bunlar yalnızca sana takım arkadaşı bulmak için kullanılır. " +
        "Müsaitlik kaydın süresi dolunca silinir. Hepsini `/verilerimi-sil` ile anında silebilirsin.";

    // ---------- Pool ----------

    [SlashCommand("musaitim", "Flex için 5. kişi olmaya hazırım: havuza gir, uygun ilanlarda haber al")]
    public async Task AvailableAsync(
        [Summary("rol", "Oynamak istediğin rol")]
        [Choice("Top", "Top"), Choice("Orman", "Jungle"), Choice("Orta", "Mid"), Choice("ADC", "Adc"),
         Choice("Destek", "Support"), Choice("Fark etmez", "Fill")]
        string role,
        [Summary("rank", "Flex rankın")]
        [Choice("Demir", "Iron"), Choice("Bronz", "Bronze"), Choice("Gümüş", "Silver"), Choice("Altın", "Gold"),
         Choice("Platin", "Platinum"), Choice("Zümrüt", "Emerald"), Choice("Elmas", "Diamond"), Choice("Master+", "MasterPlus")]
        string rank,
        [Summary("sure", "Ne kadar süre müsaitsin?")]
        [Choice("30 dakika", 30), Choice("1 saat", 60), Choice("2 saat", 120)]
        int minutes = 60)
    {
        var r = Enum.Parse<Role>(role);
        var k = Enum.Parse<Rank>(rank);
        var entry = await pre.SetAvailableAsync(Context.Guild.Id, Context.User.Id, r, k, TimeSpan.FromMinutes(minutes));

        var until = new DateTimeOffset(DateTime.SpecifyKind(entry.ExpiresAtUtc, DateTimeKind.Utc)).ToUnixTimeSeconds();
        var riotId = await pre.GetRiotIdAsync(Context.User.Id);
        var tip = riotId is null ? "\n💡 Davet edilmen kolaylaşsın diye Riot ID'ni `/profil` ile kaydet." : string.Empty;

        await RespondAsync(
            $"✅ Havuzdasın: **{TurkishNames.Of(r)}**, **{TurkishNames.Of(k)}**. <t:{until}:R> kendiliğinden çıkacaksın.\n" +
            $"Uygun bir ilan açılınca seni etiketleyeceğim (en fazla {options.PingCooldownMinutes} dakikada bir).{tip}\n\n{PrivacyNotice}",
            ephemeral: true);
    }

    [SlashCommand("musait-degilim", "Havuzdan çık, artık haber alma")]
    public async Task NotAvailableAsync()
    {
        var removed = await pre.ClearAvailableAsync(Context.Guild.Id, Context.User.Id);
        await RespondAsync(removed ? "👋 Havuzdan çıktın." : "Zaten havuzda değilsin.", ephemeral: true);
    }

    [SlashCommand("havuz", "Şu an müsait olan oyuncuları rol rol göster")]
    public async Task PoolAsync()
    {
        var summary = await pre.GetPoolSummaryAsync(Context.Guild.Id);
        if (summary.Total == 0)
        {
            await RespondAsync("Havuz şu an boş. `/musaitim` ile ilk sen gir; akşamları dolmaya başlar.", ephemeral: true);
            return;
        }

        var lines = summary.ByRole
            .Where(kv => kv.Value > 0)
            .Select(kv => $"• {TurkishNames.Of(kv.Key)}: **{kv.Value}**");
        await RespondAsync($"🟢 Şu an havuzda **{summary.Total}** kişi var:\n{string.Join("\n", lines)}", ephemeral: true);
    }

    // ---------- Listings ----------

    [SlashCommand("pre-lazim", "Eksik kişi için ilan aç; uygun ve müsait oyunculara haber verilir")]
    public async Task NeedAsync(
        [Summary("rol", "Hangi rol lazım?")]
        [Choice("Top", "Top"), Choice("Orman", "Jungle"), Choice("Orta", "Mid"), Choice("ADC", "Adc"),
         Choice("Destek", "Support"), Choice("Fark etmez", "Fill")]
        string role,
        [Summary("en-dusuk", "Kabul edeceğin en düşük rank")]
        [Choice("Demir", "Iron"), Choice("Bronz", "Bronze"), Choice("Gümüş", "Silver"), Choice("Altın", "Gold"),
         Choice("Platin", "Platinum"), Choice("Zümrüt", "Emerald"), Choice("Elmas", "Diamond"), Choice("Master+", "MasterPlus")]
        string minRank,
        [Summary("en-yuksek", "Kabul edeceğin en yüksek rank")]
        [Choice("Demir", "Iron"), Choice("Bronz", "Bronze"), Choice("Gümüş", "Silver"), Choice("Altın", "Gold"),
         Choice("Platin", "Platinum"), Choice("Zümrüt", "Emerald"), Choice("Elmas", "Diamond"), Choice("Master+", "MasterPlus")]
        string maxRank,
        [Summary("kisi", "Kaç kişi eksik?")]
        [Choice("1 kişi", 1), Choice("2 kişi", 2), Choice("3 kişi", 3)]
        int slots = 1,
        [Summary("not", "Kısa not, ör. 'mikrofon şart, eğlencesine'")]
        string? note = null)
    {
        await DeferAsync(ephemeral: true);

        var result = await pre.CreateListingAsync(
            Context.Guild.Id, Context.Channel.Id, Context.User.Id,
            Enum.Parse<Role>(role), Enum.Parse<Rank>(minRank), Enum.Parse<Rank>(maxRank), slots, note);

        if (result.Listing is null)
        {
            await FollowupAsync(
                "Zaten açık bir ilanın var. Önce onu **İlanı kapat** butonuyla kapat ya da süresinin dolmasını bekle.",
                ephemeral: true);
            return;
        }

        try
        {
            await notifier.PostListingAsync(Context.Channel, result.Listing.Id, result.PotentialMatches);
        }
        catch (Exception)
        {
            await pre.CancelAsync(result.Listing.Id, Context.User.Id);
            await FollowupAsync(
                "Bu kanala mesaj gönderemiyorum. Sunucu yöneticisinden PreBot'a bu kanalda " +
                "**Mesaj Gönder** ve **Bağlantı Yerleştir** izni vermesini iste.",
                ephemeral: true);
            return;
        }

        await notifier.RunWaveAsync(result.Listing.Id);

        var hostRiotId = await pre.GetRiotIdAsync(Context.User.Id);
        var tip = hostRiotId is null ? "\n💡 Katılanlar seni bulabilsin diye Riot ID'ni `/profil` ile kaydet." : string.Empty;
        var poolText = result.PotentialMatches == 0
            ? "Şu an havuzda uygun kimse yok; ilan açık kaldıkça yeni gelenlere haber vereceğim."
            : $"Havuzda **{result.PotentialMatches}** uygun kişi var, en uygunlardan başlayarak haber veriyorum.";
        await FollowupAsync($"📢 İlanın açıldı. {poolText}{tip}", ephemeral: true);
    }

    // ---------- Profile & privacy ----------

    [SlashCommand("profil", "Riot ID'ni kaydet (ör. Oyuncu#TR1); katıldığın ilanda sahibine gösterilir")]
    public async Task ProfileAsync([Summary("riot-id", "Örnek: Oyuncu#TR1")] string riotId)
    {
        var ok = await pre.SetRiotIdAsync(Context.User.Id, riotId);
        await RespondAsync(
            ok
                ? $"✅ Riot ID kaydedildi. Katıldığın ilanlarda ilan sahibine gösterilecek.\n\n{PrivacyNotice}"
                : "Bu bir Riot ID'ye benzemiyor. `İsim#ETİKET` biçiminde yaz, ör. `Oyuncu#TR1`.",
            ephemeral: true);
    }

    [SlashCommand("gizlilik", "PreBot hangi verini saklıyor?")]
    public Task PrivacyAsync() => RespondAsync(PrivacyNotice, ephemeral: true);

    [SlashCommand("verilerimi-sil", "PreBot'taki tüm verilerini sil")]
    public async Task DeleteMyDataAsync()
    {
        await DeferAsync(ephemeral: true);
        var result = await pre.DeleteUserDataAsync(Context.User.Id);
        foreach (var listingId in result.LeftListingIds)
        {
            await notifier.RefreshAsync(listingId);
        }

        foreach (var removed in result.RemovedListings)
        {
            await notifier.MarkRemovedAsync(removed);
        }

        await FollowupAsync("🗑️ Tüm verilerin silindi.", ephemeral: true);
    }

    // ---------- Buttons ----------

    [ComponentInteraction(ListingRenderer.JoinPrefix + "*", ignoreGroupNames: true)]
    public async Task JoinAsync(string id)
    {
        if (!int.TryParse(id, out var listingId))
        {
            return;
        }

        await DeferAsync();
        var outcome = await pre.TryClaimAsync(listingId, Context.User.Id);
        var message = outcome.Result switch
        {
            ClaimResult.Claimed or ClaimResult.ClaimedAndFilled => null,
            ClaimResult.IsHost => "Bu senin ilanın. 🙂",
            ClaimResult.AlreadyJoined => "Bu ilana zaten katıldın (ya da az önce ayrıldın).",
            ClaimResult.Full => "Maalesef biri senden önce davrandı, kadro doldu.",
            _ => "Bu ilan artık açık değil.",
        };

        if (message is not null)
        {
            await FollowupAsync(message, ephemeral: true);
            return;
        }

        await notifier.RefreshAsync(listingId);
        await notifier.AnnounceJoinAsync(listingId, Context.User.Id);

        var view = await pre.GetListingViewAsync(listingId);
        var host = view?.HostRiotId is { } riot ? $" Riot ID'si: `{riot}`." : string.Empty;
        var mine = await pre.GetRiotIdAsync(Context.User.Id);
        var tip = mine is null ? "\n💡 Riot ID'ni `/profil` ile kaydedersen ilan sahibi seni hemen davet edebilir." : string.Empty;
        await FollowupAsync(
            $"✅ Yer senin! <@{outcome.HostUserId}> seni davet edecek.{host}\n" +
            $"Gelemeyeceksen lütfen **Vazgeç**'e bas, başkası alsın.{tip}",
            ephemeral: true);
    }

    [ComponentInteraction(ListingRenderer.LeavePrefix + "*", ignoreGroupNames: true)]
    public async Task LeaveAsync(string id)
    {
        if (!int.TryParse(id, out var listingId))
        {
            return;
        }

        await DeferAsync();
        var result = await pre.LeaveAsync(listingId, Context.User.Id);
        if (result != LeaveResult.Left)
        {
            await FollowupAsync("Bu ilanda yerin yok.", ephemeral: true);
            return;
        }

        await notifier.RefreshAsync(listingId);
        await notifier.RunWaveAsync(listingId);
        await FollowupAsync("Yerini bıraktın, ilan yeniden açıldı. Haber verdiğin için teşekkürler!", ephemeral: true);
    }

    [ComponentInteraction(ListingRenderer.NoShowPrefix + "*", ignoreGroupNames: true)]
    public async Task NoShowAsync(string id)
    {
        if (!int.TryParse(id, out var listingId))
        {
            return;
        }

        await DeferAsync();
        var outcome = await pre.ReopenForNoShowAsync(listingId, Context.User.Id);
        switch (outcome.Result)
        {
            case HostActionResult.Done:
                await notifier.RefreshAsync(listingId);
                await notifier.RunWaveAsync(listingId);
                await FollowupAsync($"<@{outcome.AffectedUserId}> çıkarıldı, yeniden arıyorum.", ephemeral: true,
                    allowedMentions: AllowedMentions.None);
                break;
            case HostActionResult.NotHost:
                await FollowupAsync("Bunu yalnızca ilan sahibi yapabilir.", ephemeral: true);
                break;
            default:
                await FollowupAsync("Henüz katılan kimse yok.", ephemeral: true);
                break;
        }
    }

    [ComponentInteraction(ListingRenderer.CancelPrefix + "*", ignoreGroupNames: true)]
    public async Task CancelAsync(string id)
    {
        if (!int.TryParse(id, out var listingId))
        {
            return;
        }

        await DeferAsync();
        var outcome = await pre.CancelAsync(listingId, Context.User.Id);
        switch (outcome.Result)
        {
            case HostActionResult.Done:
                await notifier.RefreshAsync(listingId);
                await FollowupAsync("İlan kapatıldı.", ephemeral: true);
                break;
            case HostActionResult.NotHost:
                await FollowupAsync("Bunu yalnızca ilan sahibi yapabilir.", ephemeral: true);
                break;
            default:
                await FollowupAsync("Bu ilan zaten kapalı.", ephemeral: true);
                break;
        }
    }
}
