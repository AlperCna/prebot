using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PreBot.Core.Domain;
using PreBot.Core.Services;
using PreBot.Web.Auth;
using PreBot.Web.Live;

namespace PreBot.Web.Components.Shared;

/// <summary>
/// Base for signed-in pages: resolves the current user, and re-runs <see cref="LoadAsync"/>
/// whenever anything in the pool changes, so every open page stays live.
/// </summary>
public abstract class UserComponentBase : ComponentBase, IDisposable
{
    [CascadingParameter] private Task<AuthenticationState>? AuthState { get; set; }

    [Inject] protected AccountService Accounts { get; set; } = default!;
    [Inject] protected PreService Pre { get; set; } = default!;
    [Inject] protected PoolActions Actions { get; set; } = default!;
    [Inject] protected LiveUpdates Live { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    protected ulong UserId { get; private set; }

    protected UserAccount? Account { get; private set; }

    /// <summary>
    /// False while the page is only prerendered HTML (the live connection isn't up yet, so clicks would do
    /// nothing). Action buttons are disabled until then, so no click is ever silently lost.
    /// </summary>
    protected bool Ready => RendererInfo.IsInteractive;

    private Timer? _clock;

    protected override async Task OnInitializedAsync()
    {
        var state = AuthState is null ? null : await AuthState;
        var userId = state?.User.GetUserId();
        Account = userId is null ? null : await Accounts.FindAsync(userId.Value);
        if (Account is null || Account.BannedAtUtc is not null)
        {
            // Cookie points to an account that no longer exists (data deleted) or was banned: sign out.
            Navigation.NavigateTo("/auth/logout", forceLoad: true);
            return;
        }

        UserId = Account.UserId;
        await LoadAsync();
        Live.Changed += OnPoolChanged;

        // Re-render every 20 s so "x dk kaldı" texts stay correct between changes.
        _clock = new Timer(_ => InvokeAsync(StateHasChanged), null, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));
    }

    protected abstract Task LoadAsync();

    private void OnPoolChanged() => _ = InvokeAsync(async () =>
    {
        await LoadAsync();
        StateHasChanged();
    });

    public virtual void Dispose()
    {
        Live.Changed -= OnPoolChanged;
        _clock?.Dispose();
        GC.SuppressFinalize(this);
    }
}
