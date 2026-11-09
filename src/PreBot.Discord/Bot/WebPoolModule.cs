using Discord;
using Discord.Interactions;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Bot;

/// <summary>
/// Slash commands and buttons when the bot runs inside the website: everything goes to the site's single
/// pool, so a group that opens a listing on Discord reaches players on the website (push) and vice versa.
/// Personal replies are ephemeral. Same command names as <see cref="PreModule"/>; only one of the two
/// modules is loaded (see <see cref="DiscordBotService"/>).
/// </summary>
public sealed class WebPoolModule(IWebPool pool, PreBotOptions options) : InteractionModuleBase<SocketInteractionContext>
{
    /// <summary>"Katıl" button on Discord announcements and DMs of website listings.</summary>
    public const string JoinPrefix = "web:join:";

    /// <summary>Riot ID form shown when someone without a Riot ID presses "Katıl".</summary>
    public const string RiotIdModalPrefix = "web:riot:";

    private const string PrivacyNotice =
        "🔒 **Gizlilik:** PreBot, Discord kimliğinle bir PreBot hesabı açar ve yalnızca görünen adını, seçtiğin " +
        "rol/rank'ı, müsaitlik süreni, ilanlarını ve istersen Riot ID'ni saklar; yalnızca takım arkadaşı bulmak için. " +
        "Ayrıntılar sitedeki Gizlilik sayfasında. Hepsini `/verilerimi-sil` ile anında silebilirsin. 16 yaşından küçüksen kullanma.";

    private const string BannedText = "Bu PreBot hesabı kapatıldı. Bir yanlışlık olduğunu düşünüyorsan sitedeki İletişim sayfasına yaz.";

    /// <summary>The caller's website account; throws <see cref="BannedException"/> if the operator closed it.</summary>
    private async Task<ulong> MeAsync()
    {
        var me = await pool.AccountForDiscordUserAsync(
            Context.User.Id, (Context.User as IGuildUser)?.DisplayName ?? Context.User.GlobalName ?? Context.User.Username);
        return await pool.IsBannedAsync(me) ? throw new BannedException() : me;
    }

    /// <summary>Raised for banned accounts; turned into a short reply by <see cref="DiscordBotService"/>.</summary>
    public sealed class BannedException() : Exception(BannedText);

    // ---------- Pool ----------

    [CommandContextType(InteractionContextType.Guild)]
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
        await DeferAsync(ephemeral: true);
        var me = await MeAsync();
        var r = Enum.Parse<Role>(role);
        var k = Enum.Parse<Rank>(rank);
        var entry = await pool.SetAvailableAsync(me, r, k, TimeSpan.FromMinutes(minutes));

        var until = new DateTimeOffset(DateTime.SpecifyKind(entry.ExpiresAtUtc, DateTimeKind.Utc)).ToUnixTimeSeconds();
        var tip = await pool.GetRiotIdAsync(me) is null ? "\n💡 Davet edilmen kolaylaşsın diye Riot ID'ni `/profil` ile kaydet." : string.Empty;
        await FollowupAsync(
            $"✅ Havuzdasın: **{TurkishNames.Of(r)}**, **{TurkishNames.Of(k)}**. <t:{until}:R> kendiliğinden çıkacaksın.\n" +
            $"Sitede ya da Discord'da uygun bir ilan açılınca sana **DM** ile haber vereceğim (en fazla {options.PingCooldownMinutes} dakikada bir). " +
            $"DM'lerin kapalıysa haber alamazsın.{tip}\n\n{PrivacyNotice}",
            ephemeral: true);
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("musait-degilim", "Havuzdan çık, artık haber alma")]
    public async Task NotAvailableAsync()
    {
        await DeferAsync(ephemeral: true);
        await pool.ClearAvailableAsync(await MeAsync());
        await FollowupAsync("👋 Havuzdan çıktın.", ephemeral: true);
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("havuz", "Şu an müsait olan oyuncuları rol rol göster")]
    public async Task PoolAsync()
    {
        var summary = await pool.GetPoolSummaryAsync();
        if (summary.Total == 0)
        {
            await RespondAsync("Havuz şu an boş. `/musaitim` ile ilk sen gir; akşamları dolmaya başlar.", ephemeral: true);
            return;
        }

        var lines = summary.ByRole.Where(kv => kv.Value > 0).Select(kv => $"• {TurkishNames.Of(kv.Key)}: **{kv.Value}**");
        await RespondAsync(
            $"🟢 Şu an havuzda (site + Discord) **{summary.Total}** kişi var:\n{string.Join("\n", lines)}",
            ephemeral: true);
    }

    // ---------- Listings ----------

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("pre-lazim", "Eksik kişi için ilan aç; uygun ve müsait oyunculara haber verilir")]
    public async Task NeedAsync(
        [Summary("rol", "Hangi rol lazım?")]
        [Choice("Top", "Top"), Choice("Orman", "Jungle"), Choice("Orta", "Mid"), Choice("ADC", "Adc"),
         Choice("Destek", "Support"), Choice("Fark etmez", "Fill")]
        string role,
        [Summary("kisi", "Kaç kişi eksik? (Flex'e 4 kişiyle girilemez)")]
        [Choice("1 kişi (4 kişiyiz)", 1), Choice("2 kişi (3 kişiyiz)", 2), Choice("3 kişi (2 kişiyiz)", 3)]
        int slots = 1,
        [Summary("en-dusuk", "Kabul edeceğin en düşük rank (boş: herkes)")]
        [Choice("Demir", "Iron"), Choice("Bronz", "Bronze"), Choice("Gümüş", "Silver"), Choice("Altın", "Gold"),
         Choice("Platin", "Platinum"), Choice("Zümrüt", "Emerald"), Choice("Elmas", "Diamond"), Choice("Master+", "MasterPlus")]
        string minRank = "Iron",
        [Summary("en-yuksek", "Kabul edeceğin en yüksek rank (boş: Elmas'a kadar)")]
        [Choice("Demir", "Iron"), Choice("Bronz", "Bronze"), Choice("Gümüş", "Silver"), Choice("Altın", "Gold"),
         Choice("Platin", "Platinum"), Choice("Zümrüt", "Emerald"), Choice("Elmas", "Diamond"), Choice("Master+", "MasterPlus")]
        string maxRank = "Diamond",
        [Summary("not", "Kısa not, ör. 'mikrofon şart, eğlencesine'")]
        string? note = null,
        [Summary("lobi", "Oyunda özel lobi de açacak mısın? Bildirimde lobi adı yazar")]
        [Choice("Hayır", "yok"), Choice("Evet, herkese açık", "acik"), Choice("Evet, şifreli", "sifreli")]
        string lobby = "yok")
    {
        await DeferAsync(ephemeral: true);
        var me = await MeAsync();
        var result = await pool.CreateListingAsync(
            me, Enum.Parse<Role>(role), Enum.Parse<Rank>(minRank), Enum.Parse<Rank>(maxRank), slots, note,
            withLobby: lobby != "yok", lobbyLocked: lobby == "sifreli", announceChannelId: Context.Channel.Id);

        if (result.RetryAfter is { } wait)
        {
            await FollowupAsync(ActionLimiter.RetryMessage(wait), ephemeral: true);
            return;
        }

        if (result.Listing is null)
        {
            var open = result.ExistingOpenListingId is int id ? $" {pool.ListingUrl(id)}" : string.Empty;
            await FollowupAsync($"Zaten açık bir ilanın var; önce onu kapat ya da süresinin dolmasını bekle.{open}", ephemeral: true);
            return;
        }

        var tip = await pool.GetRiotIdAsync(me) is null ? "\n💡 Katılanlar seni bulabilsin diye Riot ID'ni `/profil` ile kaydet." : string.Empty;
        var poolText = result.PotentialMatches == 0
            ? "Şu an havuzda uygun kimse yok; ilan açık kaldıkça yeni gelenlere haber vereceğim."
            : $"Havuzda **{result.PotentialMatches}** uygun kişi var, en uygunlardan başlayarak haber veriyorum (sitede push, Discord'da DM).";
        var lobbyText = result.Listing.LobbyCode is { } code
            ? $"\n🎮 Şimdi oyunda **Oyna → Özel Oyun → Oluştur** ile lobiyi aç: adı **{Lobby.Name(result.Listing.Role, code)}**" +
              (result.Listing.LobbyPassword is { } password ? $", şifresi **{password}**." : ".") +
              " Lobiden biri gelirse ilan sayfasında **Lobiden biri geldi**'ye bas."
            : string.Empty;
        await FollowupAsync(
            $"📢 İlanın açıldı ve bu kanalda duyuruldu. {poolText}{lobbyText}\nBiri katılınca sana DM ile haber vereceğim. " +
            $"İlanı yönetmek (kapatmak, gelmeyeni çıkarmak) için: {pool.ListingUrl(result.Listing.Id)}{tip}",
            ephemeral: true);
    }

    // ---------- Profile & privacy ----------

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("profil", "Riot ID'ni kaydet (ör. Oyuncu#TR1); katıldığın ilanda sahibine gösterilir")]
    public async Task ProfileAsync([Summary("riot-id", "Örnek: Oyuncu#TR1")] string riotId)
    {
        await DeferAsync(ephemeral: true);
        var ok = await pool.SetRiotIdAsync(await MeAsync(), riotId);
        await FollowupAsync(
            ok
                ? $"✅ Riot ID kaydedildi. Katıldığın ilanlarda ilan sahibine gösterilecek.\n\n{PrivacyNotice}"
                : "Bu bir Riot ID'ye benzemiyor. `İsim#ETİKET` biçiminde yaz, ör. `Oyuncu#TR1`.",
            ephemeral: true);
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("gizlilik", "PreBot hangi verini saklıyor?")]
    public Task PrivacyAsync() => RespondAsync(PrivacyNotice, ephemeral: true);

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("verilerimi-sil", "PreBot'taki tüm verilerini sil")]
    public async Task DeleteMyDataAsync()
    {
        await DeferAsync(ephemeral: true);
        // No ban check here: closed accounts may delete their data too (a nameless stub keeps the ban).
        await pool.DeleteAccountAsync(await pool.AccountForDiscordUserAsync(Context.User.Id, Context.User.Username));
        await FollowupAsync("🗑️ Tüm verilerin ve PreBot hesabın silindi.", ephemeral: true);
    }

    // ---------- Buttons (announcements in channels and notification DMs) ----------

    [ComponentInteraction(JoinPrefix + "*", ignoreGroupNames: true)]
    public async Task JoinAsync(string id)
    {
        if (!int.TryParse(id, out var listingId))
        {
            return;
        }

        var me = await MeAsync();
        if (await pool.GetRiotIdAsync(me) is null)
        {
            // The host needs it to invite them: ask first, then take the seat (see RiotIdAndJoinAsync).
            await RespondWithModalAsync<RiotIdModal>(RiotIdModalPrefix + listingId);
            return;
        }

        await DeferAsync(ephemeral: true);
        await ClaimAndReplyAsync(listingId, me);
    }

    [ModalInteraction(RiotIdModalPrefix + "*", ignoreGroupNames: true)]
    public async Task RiotIdAndJoinAsync(string id, RiotIdModal modal)
    {
        if (!int.TryParse(id, out var listingId))
        {
            return;
        }

        await DeferAsync(ephemeral: true);
        var me = await MeAsync();
        if (!await pool.SetRiotIdAsync(me, modal.RiotId))
        {
            await FollowupAsync("Bu bir Riot ID'ye benzemiyor. `İsim#ETİKET` biçiminde yaz, ör. `Oyuncu#TR1`, sonra tekrar **Katıl**'a bas.", ephemeral: true);
            return;
        }

        await ClaimAndReplyAsync(listingId, me);
    }

    private async Task ClaimAndReplyAsync(int listingId, ulong me)
    {
        var outcome = await pool.ClaimAsync(listingId, me);
        var message = outcome.Result switch
        {
            ClaimResult.Claimed or ClaimResult.ClaimedAndFilled => null,
            ClaimResult.IsHost => "Bu senin ilanın. 🙂",
            ClaimResult.AlreadyJoined => "Bu ilana zaten katıldın (ya da daha önce ayrıldın).",
            ClaimResult.Full => "Maalesef biri senden önce davrandı, kadro doldu.",
            ClaimResult.RateLimited => ActionLimiter.RetryMessage(outcome.RetryAfter),
            ClaimResult.NeedsRiotId => "Önce `/profil` ile Riot ID'ni kaydet, sonra tekrar **Katıl**'a bas.",
            ClaimResult.HoldsOtherSeat => "Başka bir açık ilanda yerin var; önce oradan vazgeç.",
            _ => "Bu ilan artık açık değil.",
        };

        if (message is not null)
        {
            await FollowupAsync(message, ephemeral: true);
            return;
        }

        var view = await pool.GetListingAsync(listingId);
        var host = view?.HostRiotId is { } riot
            ? $"İlan sahibinin Riot ID'si: `{riot}` — istersen sen de ekleyebilirsin."
            : "İlan sahibi Riot ID'sini kaydetmemiş.";
        await FollowupAsync(
            $"✅ Yer senin! İlan sahibine haber verdim, seni oyunda davet edecek. {host}\n" +
            $"Gelemeyeceksen lütfen ilan sayfasından **Vazgeç**'e bas, başkası alsın: {pool.ListingUrl(listingId)}",
            ephemeral: true);
    }
}
