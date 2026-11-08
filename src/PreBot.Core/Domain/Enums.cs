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
