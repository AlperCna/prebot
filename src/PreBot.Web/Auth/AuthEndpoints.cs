using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PreBot.Core.Services;

namespace PreBot.Web.Auth;

public static class AuthEndpoints
{
    public const string DevProvider = "dev";

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

        app.MapGet("/auth/logout", async (HttpContext http) =>
        {
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
