// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestEnricher.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientIP", context.Connection?.RemoteIpAddress?.ToString(), true));
    }
}