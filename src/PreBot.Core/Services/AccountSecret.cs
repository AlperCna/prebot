using System.Security.Cryptography;

namespace PreBot.Core.Services;

/// <summary>
/// Password for "name + invite code" accounts. Without it, anyone who knows the shared invite code could sign
/// in as anyone just by typing their name. Stored as salted PBKDF2-SHA256; the plain text is never kept.
/// </summary>
public static class AccountSecret
{
    public const int MinLength = 6;
    public const int MaxLength = 64;
    private const int Iterations = 100_000;
    private const int SaltBytes = 16;
    private const int HashBytes = 32;

    public static string Hash(string secret)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltBytes);
        var hash = Rfc2898DeriveBytes.Pbkdf2(secret, salt, Iterations, HashAlgorithmName.SHA256, HashBytes);
        return $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string secret, string? stored)
    {
        var parts = (stored ?? "").Split('.');
        if (parts.Length != 4 || parts[0] != "v1" || !int.TryParse(parts[1], out var iterations) || iterations < 1)
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(secret, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

/// <summary>Outcome of signing in with a name and password.</summary>
public enum SecretSignIn
{
    /// <summary>New account, the password was set now.</summary>
    Created,

    /// <summary>Account from before passwords existed: the password was set now.</summary>
    Claimed,

    /// <summary>Existing account, the password matched.</summary>
    Verified,

    /// <summary>The name belongs to someone else, or the password is wrong.</summary>
    WrongSecret,
}
