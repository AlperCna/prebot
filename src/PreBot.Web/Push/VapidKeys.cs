using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace PreBot.Web.Push;

/// <summary>
/// The site's Web Push identity (VAPID key pair). Production: set Push:PublicKey / Push:PrivateKey / Push:Subject
/// in configuration (secrets). Development: generated once and kept in vapid-keys.json (git-ignored).
/// Changing the keys invalidates every existing browser subscription.
/// </summary>
public sealed class VapidKeys
{
    public VapidKeys(IConfiguration config, DataDirectory data, ILogger<VapidKeys> logger)
    {
        Subject = config["Push:Subject"] ?? "mailto:prebot@example.com";
        var publicKey = config["Push:PublicKey"];
        var privateKey = config["Push:PrivateKey"];

        if (string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey))
        {
            var file = Path.Combine(data.Path, "vapid-keys.json");
            if (File.Exists(file))
            {
                var stored = JsonSerializer.Deserialize<Stored>(File.ReadAllText(file))!;
                (publicKey, privateKey) = (stored.PublicKey, stored.PrivateKey);
            }
            else
            {
                (publicKey, privateKey) = Generate();
                File.WriteAllText(file, JsonSerializer.Serialize(new Stored(publicKey, privateKey)));
                logger.LogWarning("Generated new VAPID keys in {File}. Set Push:PublicKey/PrivateKey in production.", file);
            }
        }

        PublicKey = publicKey;
        PrivateKey = privateKey;
    }

    public string PublicKey { get; }
    public string PrivateKey { get; }
    public string Subject { get; }

    /// <summary>P-256 key pair in the base64url format browsers and the push library expect.</summary>
    public static (string PublicKey, string PrivateKey) Generate()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var p = ecdsa.ExportParameters(includePrivateParameters: true);
        var uncompressed = new byte[65];
        uncompressed[0] = 0x04;
        p.Q.X!.CopyTo(uncompressed, 1);
        p.Q.Y!.CopyTo(uncompressed, 33);
        return (WebEncoders.Base64UrlEncode(uncompressed), WebEncoders.Base64UrlEncode(p.D!));
    }

    private sealed record Stored(string PublicKey, string PrivateKey);
}
