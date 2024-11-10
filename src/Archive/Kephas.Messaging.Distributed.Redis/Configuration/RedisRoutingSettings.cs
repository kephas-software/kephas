namespace Kephas.Messaging.Redis.Configuration;

using Kephas.Configuration;

/// <summary>
/// Settings for Redis based routing.
/// </summary>
public class RedisRoutingSettings : ISettings
{
    /// <summary>
    /// Gets or sets the connection URI.
    /// </summary>
    public string? ConnectionUri { get; set; }

    /// <summary>
    /// Gets or sets the namespace.
    /// </summary>
    public string? Namespace { get; set; }
}