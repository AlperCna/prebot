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

    private readonly ITimeLimitedDataProtector _mute =
        provider.CreateProtector("PreBot.Mute.v1").ToTimeLimitedDataProtector();

    /// <summary>Personal token for the notification's "1 saat sustur" button (valid one day).</summary>
    public string CreateMute(ulong userId) => _mute.Protect(userId.ToString(), time.GetUtcNow() + TimeSpan.FromDays(1));

    /// <summary>The user a mute token belongs to, or null if it is forged or expired.</summary>
    public ulong? MuteUser(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        try
        {
            var payload = _mute.Unprotect(token, out var expiration);
            return expiration > time.GetUtcNow() && ulong.TryParse(payload, out var id) ? id : null;
        }
        catch (CryptographicException)
        {
            return null;
        }
    }

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
