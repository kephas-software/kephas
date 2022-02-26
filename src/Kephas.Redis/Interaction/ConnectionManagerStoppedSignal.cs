namespace Kephas.Redis.Interaction;

using Kephas.ExceptionHandling;
using Kephas.Interaction;

/// <summary>
/// The Redis connection manager stopped signal. Issued after the manager completes finalization/disposal.
/// </summary>
public class ConnectionManagerStoppedSignal : SignalBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionManagerStoppedSignal"/> class.
    /// </summary>
    /// <param name="message">Optional. The message.</param>
    /// <param name="severity">Optional. The severity.</param>
    public ConnectionManagerStoppedSignal(string? message = null, SeverityLevel severity = SeverityLevel.Info)
        : base(message ?? "The Redis connection manager is stopped.", severity)
    {
    }
}