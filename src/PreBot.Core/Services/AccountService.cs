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

    /// <summary>KVKK: deletes all bot data for the user, then the account itself.</summary>
    public async Task<DeleteUserDataResult> DeleteAccountAsync(ulong userId, CancellationToken ct = default)
    {
        var result = await pre.DeleteUserDataAsync(userId, ct);
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var id = (long)userId;
        await db.Accounts.Where(a => a.Id == id).ExecuteDeleteAsync(ct);
        return result;
    }

    public static string CleanName(string? name)
    {
        var cleaned = string.Join(' ', (name ?? "").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        return cleaned.Length > MaxDisplayNameLength ? cleaned[..MaxDisplayNameLength] : cleaned;
    }
}
