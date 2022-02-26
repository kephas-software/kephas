namespace Kephas.Messaging.Redis.Configuration;

using Kephas.Configuration;

public class RedisRoutingSettings : ISettings
{
    public string ConnectionUri { get; set; }

    public string Namespace { get; set; }
}