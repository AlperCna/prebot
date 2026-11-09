namespace PreBot.Core.Domain;

/// <summary>Lane/role a player offers or a listing asks for. <see cref="Fill"/> matches any role.</summary>
public enum Role
{
    Top,
    Jungle,
    Mid,
    Adc,
    Support,
    Fill,
}

/// <summary>
/// Official Riot tiers only (no custom skill rating — Riot policy forbids MMR/ELO alternatives).
/// Master, Grandmaster and Challenger are grouped because Flex party rules treat them the same way.
/// </summary>
public enum Rank
{
    Iron = 0,
    Bronze = 1,
    Silver = 2,
    Gold = 3,
    Platinum = 4,
    Emerald = 5,
    Diamond = 6,
    MasterPlus = 7,
}

public enum MetricKind
{
    ListingOpened,
    WaveSent,
    SeatClaimed,
    ListingFilled,
    SeatReleased,
    ListingCancelled,
    ListingExpired,

    /// <summary>The host found someone through the custom game lobby (outside PreBot's seat claiming).</summary>
    FilledViaLobby,

    /// <summary>Notifications handed to players in one go, per source (Value = how many, Source = where from).</summary>
    NotificationsSent,

    /// <summary>A player opened a listing notification or tapped one of its buttons (first time only).</summary>
    NotificationOpened,

    /// <summary>A player answered "Maçtayım, bitince haber ver".</summary>
    InGameReported,

    /// <summary>A host opened a joiner's op.gg profile from the listing page.</summary>
    ProfileOpened,

    /// <summary>Push services accepted this many device deliveries for one wave (Value).</summary>
    PushAccepted,
}

/// <summary>Why a player heard about a listing. Measured, so the trial shows which way of finding players works.</summary>
public enum NotificationSource
{
    /// <summary>They said "Şu an müsaitim" and were in the pool.</summary>
    Pool,

    /// <summary>Their "Bana haber ver" subscription.</summary>
    Alert,

    /// <summary>They joined the pool while the listing was already open.</summary>
    Newcomer,

    /// <summary>They said "Maçtayım" earlier and their match is over now.</summary>
    AfterMatch,
}

public enum ReportReason
{
    /// <summary>Joined and did not show up.</summary>
    NoShow,

    /// <summary>Insults, harassment, toxicity.</summary>
    Toxic,

    /// <summary>Fake rank or Riot ID.</summary>
    FakeInfo,

    /// <summary>Boosting, account selling, betting, spam.</summary>
    Forbidden,

    Other,
}

public enum SeatReleaseReason
{
    /// <summary>The player gave the seat back themselves.</summary>
    Voluntary,

    /// <summary>The host reported that the player did not show up.</summary>
    NoShow,

    /// <summary>The player deleted their data.</summary>
    AccountDeleted,

    /// <summary>The operator closed the player's account.</summary>
    Banned,

    /// <summary>
    /// The host chose someone else ("Uygun değil"), e.g. after looking at their profile. Not a no-show: it is
    /// not shown to later hosts and not counted in the no-show rate.
    /// </summary>
    HostDeclined,
}

public enum ListingStatus
{
    Open,
    Filled,
    Expired,
    Cancelled,
}

public enum ClaimResult
{
    Claimed,
    ClaimedAndFilled,
    AlreadyJoined,
    Full,
    NotOpen,
    IsHost,
    NotFound,

    /// <summary>Too many joins in a short time (see <c>ActionLimiter</c>).</summary>
    RateLimited,

    /// <summary>The player already holds a seat in another open listing.</summary>
    HoldsOtherSeat,

    /// <summary>No Riot ID saved: the host could not invite them.</summary>
    NeedsRiotId,
}

public enum LeaveResult
{
    Left,
    NotJoined,
    NotFound,
}

public enum HostActionResult
{
    Done,
    NotHost,
    NothingToDo,
    NotFound,

    /// <summary>"Gelmedi" too soon after the player joined: give them time to accept the invite first.</summary>
    TooEarly,

    /// <summary>The host already declined the maximum number of players for this listing.</summary>
    LimitReached,
}
