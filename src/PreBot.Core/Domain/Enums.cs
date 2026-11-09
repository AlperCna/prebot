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
}

public enum SeatReleaseReason
{
    /// <summary>The player gave the seat back themselves.</summary>
    Voluntary,

    /// <summary>The host reported that the player did not show up.</summary>
    NoShow,

    /// <summary>The player deleted their data.</summary>
    AccountDeleted,
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
}
