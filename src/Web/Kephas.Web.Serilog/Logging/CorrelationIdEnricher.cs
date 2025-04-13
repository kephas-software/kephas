using Kephas.Context;
using Serilog.Core;
using Serilog.Events;

namespace Kephas.Web.Logging;

/// <summary>
/// Correlation ID enricher for Serilog.
/// </summary>
/// <seealso cref="ILogEventEnricher" />
public class CorrelationIdEnricher(IContextAccessor contextAccessor) : ILogEventEnricher
{
    /// <summary>
    /// Enrich the log event.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", contextAccessor.GetCorrelationId().ToString(), false));
    }
}