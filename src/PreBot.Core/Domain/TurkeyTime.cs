namespace PreBot.Core.Domain;

/// <summary>Turkey local time (UTC+3 all year since 2016) for hour windows and daily limits.</summary>
public static class TurkeyTime
{
    private static readonly TimeZoneInfo Zone = Find();

    public static DateTime ToLocal(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone);

    private static TimeZoneInfo Find()
    {
        foreach (var id in new[] { "Europe/Istanbul", "Turkey Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                // try the next id
            }
        }

        return TimeZoneInfo.CreateCustomTimeZone("TRT", TimeSpan.FromHours(3), "Türkiye", "Türkiye");
    }
}
