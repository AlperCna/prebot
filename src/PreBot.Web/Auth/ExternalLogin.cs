using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using PreBot.Core.Services;

namespace PreBot.Web.Auth;

/// <summary>A login provider that is configured (has a client id and secret).</summary>
public sealed record ExternalProvider(string Name, string Scheme, string Label, string? Hint);

public sealed class ExternalProviders(IReadOnlyList<ExternalProvider> all)
{
    public IReadOnlyList<ExternalProvider> All { get; } = all;

    public ExternalProvider? Find(string name) =>
        All.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
}

/// <summary>
/// "Login with Google / Discord". Each provider is enabled only when its keys are configured
/// (Authentication:Google:ClientId/ClientSecret, Authentication:Discord:ClientId/ClientSecret).
/// The provider signs into a short-lived "External" cookie; <see cref="CompleteAsync"/> then turns
/// that into a PreBot account and the normal session cookie.
/// </summary>
public static class ExternalLogin
{
    public const string Scheme = "External";
    public const string DiscordGlobalNameClaim = "urn:discord:user:global_name";

    public static ExternalProviders AddProviders(AuthenticationBuilder auth, IConfiguration config)
    {
        var providers = new List<ExternalProvider>();

        var google = config.GetSection("Authentication:Google");
        if (IsConfigured(google))
        {
            auth.AddGoogle(GoogleDefaults.AuthenticationScheme, o =>
            {
                o.ClientId = google["ClientId"]!;
                o.ClientSecret = google["ClientSecret"]!;
                o.SignInScheme = Scheme;
                // Only a stable id and a display name: no e-mail address (data minimisation, KVKK).
                o.Scope.Clear();
                o.Scope.Add("openid");
                o.Scope.Add("profile");
                o.Events.OnRemoteFailure = Cancelled;
            });
            providers.Add(new("google", GoogleDefaults.AuthenticationScheme, "Google ile giriş", null));
        }

        var discord = config.GetSection("Authentication:Discord");
        if (IsConfigured(discord))
        {
            auth.AddDiscord(DiscordAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.ClientId = discord["ClientId"]!;
                o.ClientSecret = discord["ClientSecret"]!;
                o.SignInScheme = Scheme;
                o.Scope.Clear();
                o.Scope.Add("identify"); // id + username only, no e-mail, no server list
                o.ClaimActions.MapJsonKey(DiscordGlobalNameClaim, "global_name");
                o.Events.OnRemoteFailure = Cancelled;
            });
            providers.Add(new("discord", DiscordAuthenticationDefaults.AuthenticationScheme, "Discord ile giriş",
                "Türkiye'de Discord erişime kapalı; VPN gerekebilir."));
        }

        return new ExternalProviders(providers);
    }

    /// <summary>Turns a confirmed external identity into a PreBot account and signs the user in.</summary>
    public static async Task<IResult> CompleteAsync(
        HttpContext http, ClaimsPrincipal external, string provider, AccountService accounts, string? returnUrl)
    {
        var key = external.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(key))
        {
            return Results.Redirect("/giris?hata=dis");
        }

        var name = external.FindFirstValue(DiscordGlobalNameClaim)
            ?? external.FindFirstValue(ClaimTypes.GivenName)
            ?? external.FindFirstValue(ClaimTypes.Name)
            ?? "Oyuncu";
        var account = await accounts.GetOrCreateAsync(provider, key, AccountService.CleanName(name) is { Length: >= 2 } n ? n : "Oyuncu");

        await http.SignOutAsync(Scheme);
        await AuthEndpoints.SignInAsync(http, account.UserId, account.DisplayName, provider);
        return Results.LocalRedirect(AuthEndpoints.SafeReturnUrl(returnUrl));
    }

    private static bool IsConfigured(IConfigurationSection s) =>
        !string.IsNullOrWhiteSpace(s["ClientId"]) && !string.IsNullOrWhiteSpace(s["ClientSecret"]);

    /// <summary>User pressed "Cancel" on Google/Discord, or the provider failed: back to the login page.</summary>
    private static Task Cancelled(RemoteFailureContext context)
    {
        context.Response.Redirect("/giris?hata=iptal");
        context.HandleResponse();
        return Task.CompletedTask;
    }
}
