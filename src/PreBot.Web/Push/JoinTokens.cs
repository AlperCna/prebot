using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace PreBot.Web.Push;

/// <summary>
/// One-tap join links in notifications. The link carries an encrypted, time-limited token bound to one
/// listing and one user, so a copied or forwarded link never seats anyone else.
/// </summary>
public sealed class JoinTokens(IDataProtectionProvider provider, TimeProvider time)
{
    /// <summary>A join link is useless once the listing is closed; listings stay open at most 15 minutes.</summary>
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(20);

    private readonly ITimeLimitedDataProtector _protector =
        provider.CreateProtector("PreBot.JoinLink.v1").ToTimeLimitedDataProtector();

    public string Create(int listingId, ulong userId) =>
        _protector.Protect($"{listingId}:{userId}", time.GetUtcNow() + Lifetime);

    public bool IsValid(string? token, int listingId, ulong userId)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        try
        {
            var payload = _protector.Unprotect(token, out var expiration);
            return expiration > time.GetUtcNow() && payload == $"{listingId}:{userId}";
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
}
