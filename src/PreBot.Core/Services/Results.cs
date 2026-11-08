using PreBot.Core.Domain;

namespace PreBot.Core.Services;

public sealed record CreateListingResult(Listing? Listing, int? ExistingOpenListingId, int PotentialMatches);

public sealed record WaveResult(int ListingId, int Wave, IReadOnlyList<ulong> UserIds);

public sealed record ClaimOutcome(ClaimResult Result, ulong HostUserId = 0);

public sealed record SweepResult(int ExpiredAvailabilities, IReadOnlyList<int> ExpiredListingIds);

public sealed record SlotView(ulong UserId, string? RiotId);

/// <param name="LeftUserIds">Players who held a seat and gave it back (left or no-show). They cannot rejoin this listing.</param>
public sealed record ListingView(
    Listing Listing,
    string? HostRiotId,
    IReadOnlyList<SlotView> ActiveSlots,
    IReadOnlySet<ulong> LeftUserIds);

public sealed record PoolSummary(int Total, IReadOnlyDictionary<Role, int> ByRole);
