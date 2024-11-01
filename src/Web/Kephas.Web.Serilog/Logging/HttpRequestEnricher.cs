// --
// <copyright file="HttpRequestEnricher.cs" company="Hays AG">
//   Copyright © 2017-2019 Hays AG. All rights reserved.
// </copyright>
// <summary>
//   Custom Http enricher for Serilog
// </summary>
// --

using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Kephas.Web.Logging;

/// <summary>
/// Custom Http enricher for Serilog.
/// </summary>
/// <seealso cref="ILogEventEnricher" />
public class HttpRequestEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
{
    /// <summary>
    /// Enrich the log event.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null || logEvent == null || propertyFactory == null)
        {
            return;
        }

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Uri", context.Request.Path.ToString(), true));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", context.User?.Claims?.FirstOrDefault(x => x.Type == "username")?.Value, true));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientHostIP", context.Connection?.RemoteIpAddress?.ToString(), true));
    }
}