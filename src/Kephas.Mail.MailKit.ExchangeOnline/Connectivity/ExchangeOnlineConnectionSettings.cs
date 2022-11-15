namespace Kephas.Mail.Connectivity;

using Kephas.Configuration;

/// <summary>
/// Connection settings for Exchange Online.
/// </summary>
public class ExchangeOnlineConnectionSettings : ISettings
{
    private const string ScopeEmail = "email";
    private const string ScopeOpenId = "openid";
    private const string ScopeOfflineAccess = "offline_access";
    private const string ScopeImap = "https://outlook.office.com/IMAP.AccessAsUser.All";

    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the client ID.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the scopes.
    /// </summary>
    public IList<string>? Scopes { get; set; } =
        new List<string> { ScopeEmail, ScopeOpenId, ScopeOfflineAccess, ScopeImap };

    /// <summary>
    /// Gets or sets the number of seconds before token expiration when the refresh should be issued.
    /// </summary>
    public int RefreshTokenSecondsBeforeExpiration { get; set; } = 600;
}