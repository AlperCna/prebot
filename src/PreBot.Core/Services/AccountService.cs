using Microsoft.EntityFrameworkCore;
using PreBot.Core.Data;
using PreBot.Core.Domain;

namespace PreBot.Core.Services;

/// <summary>Website accounts: sign-in lookup, display names, and full account deletion.</summary>
public sealed class AccountService(IDbContextFactory<PreBotDbContext> dbFactory, TimeProvider time, PreService pre)
{
    public const int MaxDisplayNameLength = 32;

    /// <summary>Finds the account for a provider login, creating it on first sign-in.</summary>
    public async Task<UserAccount> GetOrCreateAsync(string provider, string providerKey, string displayName, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var account = await db.Accounts.SingleOrDefaultAsync(a => a.Provider == provider && a.ProviderKey == providerKey, ct);
        if (account is not null)
        {
            return account;
        }

        account = new UserAccount
        {
            Provider = provider,
            ProviderKey = providerKey,
            DisplayName = CleanName(displayName),
            CreatedAtUtc = time.GetUtcNow().UtcDateTime,
        };
        db.Accounts.Add(account);
        try
        {
            await db.SaveChangesAsync(ct);
            return account;
        }
        catch (DbUpdateException)
        {
            // Two sign-ins raced to create the same account: the other one won, use it.
            await using var retry = await dbFactory.CreateDbContextAsync(ct);
            return await retry.Accounts.SingleAsync(a => a.Provider == provider && a.ProviderKey == providerKey, ct);
        }
    }

    /// <summary>
    /// Sign-in for password accounts (invite code): the first person to use a name sets its password, after that
    /// the name only opens with it. Accounts made before passwords existed get the password on their next sign-in.
    /// </summary>
    public async Task<(SecretSignIn Result, UserAccount? Account)> SignInWithSecretAsync(
        string provider, string providerKey, string displayName, string secret, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var account = await db.Accounts.AsNoTracking().SingleOrDefaultAsync(a => a.Provider == provider && a.ProviderKey == providerKey, ct);
        if (account is null)
        {
            account = new UserAccount
            {
                Provider = provider,
                ProviderKey = providerKey,
                DisplayName = CleanName(displayName),
                CreatedAtUtc = time.GetUtcNow().UtcDateTime,
                SecretHash = AccountSecret.Hash(secret),
            };
            db.Accounts.Add(account);
            try
            {
                await db.SaveChangesAsync(ct);
                return (SecretSignIn.Created, account);
            }
            catch (DbUpdateException)
            {
                // Someone else created this name at the same moment: their password counts.
                await using var retry = await dbFactory.CreateDbContextAsync(ct);
                account = await retry.Accounts.AsNoTracking().SingleAsync(a => a.Provider == provider && a.ProviderKey == providerKey, ct);
                return AccountSecret.Verify(secret, account.SecretHash) ? (SecretSignIn.Verified, account) : (SecretSignIn.WrongSecret, null);
            }
        }

        if (account.SecretHash is null)
        {
            var hash = AccountSecret.Hash(secret);
            var claimed = await db.Accounts
                .Where(a => a.Id == account.Id && a.SecretHash == null)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.SecretHash, hash), ct);
            if (claimed == 1)
            {
                account.SecretHash = hash;
                return (SecretSignIn.Claimed, account);
            }

            account = await db.Accounts.AsNoTracking().SingleAsync(a => a.Id == account.Id, ct);
        }

        return AccountSecret.Verify(secret, account.SecretHash) ? (SecretSignIn.Verified, account) : (SecretSignIn.WrongSecret, null);
    }

    /// <summary>Records the first time the user accepted the terms; later logins keep the original time.</summary>
    public async Task MarkTermsAcceptedAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = time.GetUtcNow().UtcDateTime;
        await db.Accounts
            .Where(a => a.Id == (long)userId && a.TermsAcceptedAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.TermsAcceptedAtUtc, now), ct);
    }

    public async Task<UserAccount?> FindAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var id = (long)userId;
        return await db.Accounts.AsNoTracking().SingleOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<bool> RenameAsync(ulong userId, string displayName, CancellationToken ct = default)
    {
        var name = CleanName(displayName);
        if (name.Length < 2)
        {
            return false;
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var id = (long)userId;
        return await db.Accounts.Where(a => a.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.DisplayName, name), ct) > 0;
    }

    /// <summary>Display names for a set of user ids. Unknown ids (e.g. Discord-only users) are left out.</summary>
    public async Task<IReadOnlyDictionary<ulong, string>> GetDisplayNamesAsync(IEnumerable<ulong> userIds, CancellationToken ct = default)
    {
        var ids = userIds.Select(u => (long)u).Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<ulong, string>();
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Accounts.AsNoTracking()
            .Where(a => ids.Contains(a.Id))
            .ToDictionaryAsync(a => (ulong)a.Id, a => a.DisplayName, ct);
    }

    /// <summary>A refused DM is tried again after this long (the user may have opened their DMs meanwhile).</summary>
    public static readonly TimeSpan DmRetryAfter = TimeSpan.FromDays(7);

    /// <summary>Discord user ids (snowflakes) of the given website users that signed up through Discord.</summary>
    public async Task<IReadOnlyDictionary<ulong, ulong>> GetDiscordIdsAsync(IEnumerable<ulong> userIds, CancellationToken ct = default) =>
        await GetDiscordIdsAsync(userIds, dmOpenOnly: false, ct);

    /// <param name="dmOpenOnly">Leave out accounts whose DMs were refused recently (see <see cref="DmRetryAfter"/>).</param>
    public async Task<IReadOnlyDictionary<ulong, ulong>> GetDiscordIdsAsync(IEnumerable<ulong> userIds, bool dmOpenOnly, CancellationToken ct = default)
    {
        var ids = userIds.Select(u => (long)u).Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<ulong, ulong>();
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var retryBefore = time.GetUtcNow().UtcDateTime - DmRetryAfter;
        var rows = await db.Accounts.AsNoTracking()
            .Where(a => ids.Contains(a.Id) && a.Provider == "discord")
            .Where(a => !dmOpenOnly || a.DmClosedAtUtc == null || a.DmClosedAtUtc <= retryBefore)
            .Select(a => new { a.Id, a.ProviderKey })
            .ToListAsync(ct);
        return rows
            .Where(r => ulong.TryParse(r.ProviderKey, out _))
            .ToDictionary(r => (ulong)r.Id, r => ulong.Parse(r.ProviderKey));
    }

    /// <summary>Remembers that Discord refused (or accepted again) a DM to this Discord user.</summary>
    public async Task SetDiscordDmClosedAsync(ulong discordUserId, bool closed, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var key = discordUserId.ToString();
        var at = closed ? time.GetUtcNow().UtcDateTime : (DateTime?)null;
        await db.Accounts
            .Where(a => a.Provider == "discord" && a.ProviderKey == key && (closed || a.DmClosedAtUtc != null))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.DmClosedAtUtc, at), ct);
    }

    /// <summary>
    /// KVKK: deletes all bot data for the user, then the account itself. A banned account keeps only a
    /// nameless stub (login id + ban date), so deleting it is not a way around the ban.
    /// </summary>
    public async Task<DeleteUserDataResult> DeleteAccountAsync(ulong userId, CancellationToken ct = default)
    {
        var result = await pre.DeleteUserDataAsync(userId, ct);
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var id = (long)userId;
        await db.Accounts.Where(a => a.Id == id && a.BannedAtUtc != null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(a => a.DisplayName, DeletedName)
                .SetProperty(a => a.SecretHash, (string?)null), ct);
        await db.Accounts.Where(a => a.Id == id && a.BannedAtUtc == null).ExecuteDeleteAsync(ct);
        return result;
    }

    public const string DeletedName = "Silinmiş hesap";

    public static string CleanName(string? name)
    {
        var cleaned = string.Join(' ', (name ?? "").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        return cleaned.Length > MaxDisplayNameLength ? cleaned[..MaxDisplayNameLength] : cleaned;
    }
}
