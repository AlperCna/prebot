namespace PreBot.Web;

/// <summary>
/// Who runs this PreBot instance. Shown in the privacy notice (KVKK md. 10: identity of the data
/// controller) and on the contact page (Law 5651: provider contact details). Set in configuration:
/// Site:OperatorName, Site:ContactEmail, Site:HostingLocation.
/// </summary>
public sealed record SiteInfo(string? OperatorName, string? ContactEmail, string? HostingLocation)
{
    public const string NotSet = "(yayına almadan önce doldurulacak)";

    public string Operator => string.IsNullOrWhiteSpace(OperatorName) ? NotSet : OperatorName;
    public string Email => string.IsNullOrWhiteSpace(ContactEmail) ? NotSet : ContactEmail;
    public string Hosting => string.IsNullOrWhiteSpace(HostingLocation) ? NotSet : HostingLocation;
    public bool HasEmail => !string.IsNullOrWhiteSpace(ContactEmail);

    public static SiteInfo From(IConfiguration config) =>
        new(config["Site:OperatorName"], config["Site:ContactEmail"], config["Site:HostingLocation"]);
}
