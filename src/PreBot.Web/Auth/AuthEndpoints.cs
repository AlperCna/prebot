using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PreBot.Core.Services;

namespace PreBot.Web.Auth;

public static class AuthEndpoints
{
    public const string DevProvider = "dev";

    /// <summary>Accounts created with the invite code (closed test with friends, no Google setup needed).</summary>
    public const string InviteProvider = "invite";

    /// <summary>True when Site:InviteCode is set: the login page then offers "name + invite code".</summary>
    public static bool InviteEnabled(IConfiguration config) => !string.IsNullOrWhiteSpace(config["Site:InviteCode"]);

    /// <summary>Sign-in/out endpoints. They run as plain HTTP requests, outside Blazor circuits, so cookies can be set.</summary>
    public static void MapAuthEndpoints(this WebApplication app)
    {
        // Local testing only: sign in as any name, so two browsers can play two different players.
        // Never mapped outside Development, because it lets anyone become anyone.
        if (app.Environment.IsDevelopment())
        {
            app.MapPost("/auth/dev-login", async (
                HttpContext http,
                AccountService accounts,
                [FromForm] string name,
                [FromForm] string? returnUrl) =>
            {
                var displayName = AccountService.CleanName(name);
                if (displayName.Length < 2)
                {
                    return Results.Redirect("/giris?hata=isim");
                }

                var account = await accounts.GetOrCreateAsync(DevProvider, displayName.ToLowerInvariant(), displayName);
                await SignInAsync(http, account.UserId, account.DisplayName, DevProvider);
                return Results.LocalRedirect(SafeReturnUrl(returnUrl));
            });
        }

        // Closed test: sign in with a display name and the shared invite code (Site:InviteCode). Meant for a
        // small group of friends — whoever knows the code can pick any name, so it is off unless configured.
        if (InviteEnabled(app.Configuration))
        {
            app.MapPost("/auth/invite-login", async (
                HttpContext http,
                AccountService accounts,
                IConfiguration config,
                [FromForm] string name,
                [FromForm] string code,
                [FromForm] string? kabul,
                [FromForm] string? returnUrl) =>
            {
                var back = Uri.EscapeDataString(SafeReturnUrl(returnUrl));
                if (kabul != "1")
                {
                    return Results.Redirect($"/giris?hata=kabul&returnUrl={back}");
                }

                var expected = System.Text.Encoding.UTF8.GetBytes(config["Site:InviteCode"]!.Trim());
                var given = System.Text.Encoding.UTF8.GetBytes((code ?? "").Trim());
                if (!System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(expected, given))
                {
                    return Results.Redirect($"/giris?hata=kod&returnUrl={back}");
                }

                var displayName = AccountService.CleanName(name);
                if (displayName.Length < 2)
                {
                    return Results.Redirect($"/giris?hata=isim&returnUrl={back}");
                }

                var account = await accounts.GetOrCreateAsync(InviteProvider, displayName.ToLowerInvariant(), displayName);
                await accounts.MarkTermsAcceptedAsync(account.UserId);
                await SignInAsync(http, account.UserId, account.DisplayName, InviteProvider);
                return Results.LocalRedirect(SafeReturnUrl(returnUrl));
            }).RequireRateLimiting("auth");
        }

        // Login with Google / Discord: send the user to the provider...
        app.MapGet("/auth/login/{provider}", (string provider, string? returnUrl, string? kabul, ExternalProviders providers) =>
        {
            var p = providers.Find(provider);
            if (p is null)
            {
                return Results.NotFound();
            }

            // The login page asks to accept the terms and privacy notice (and confirm 16+) first.
            if (kabul != "1")
            {
                return Results.Redirect($"/giris?hata=kabul&returnUrl={Uri.EscapeDataString(SafeReturnUrl(returnUrl))}");
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = $"/auth/callback?provider={p.Name}&returnUrl={Uri.EscapeDataString(SafeReturnUrl(returnUrl))}",
            };
            return Results.Challenge(properties, [p.Scheme]);
        }).RequireRateLimiting("auth");

        // ...and back here once they confirmed who they are.
        app.MapGet("/auth/callback", async (HttpContext http, AccountService accounts, ExternalProviders providers, string? provider, string? returnUrl) =>
        {
            var result = await http.AuthenticateAsync(ExternalLogin.Scheme);
            var p = provider is null ? null : providers.Find(provider);
            if (!result.Succeeded || result.Principal is null || p is null)
            {
                return Results.Redirect("/giris?hata=dis");
            }

            return await ExternalLogin.CompleteAsync(http, result.Principal, p.Name, accounts, returnUrl);
        }).RequireRateLimiting("auth");

        // Logging out is a POST with an antiforgery token, so another site cannot log people out.
        app.MapPost("/auth/logout", async (HttpContext http, Microsoft.AspNetCore.Antiforgery.IAntiforgery antiforgery) =>
        {
            // The handler reads no form fields, so the token is not checked automatically: check it here.
            if (!await antiforgery.IsRequestValidAsync(http))
            {
                return Results.BadRequest();
            }

            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.LocalRedirect("/");
        });

        // GET only clears a cookie whose account no longer exists (after "Verilerimi sil").
        app.MapGet("/auth/logout", async (HttpContext http, AccountService accounts) =>
        {
            var userId = http.User.GetUserId();
            if (userId is not null && await accounts.FindAsync(userId.Value) is not null)
            {
                return Results.LocalRedirect("/profil");
            }

            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.LocalRedirect("/");
        });
    }

    public static Task SignInAsync(HttpContext http, ulong userId, string displayName, string provider)
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, displayName),
                new Claim("provider", provider),
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);
        return http.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });
    }

    public static string SafeReturnUrl(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith('/') && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")
            ? returnUrl
            : "/havuz";

    public static ulong? GetUserId(this ClaimsPrincipal user) =>
        ulong.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
