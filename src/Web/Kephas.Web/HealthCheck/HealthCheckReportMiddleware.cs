// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthCheckReportMiddleware.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Kephas.Extensions.Diagnostics.HealthChecks;
using Kephas.Extensions.Diagnostics.HealthChecks.Configuration;
using Kephas.Serialization.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Kephas.Web.HealthCheck;

/// <summary>
/// Middleware for writing health check reports.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HealthCheckReportMiddleware
{
    /// <summary>
    /// Registers the health check report in the application middleware.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="path">The path for the health report. If not provided, the value provided by the <see cref="HealthChecksSettings.ReportUrlPath"/> will be used.</param>
    /// <typeparam name="T">The application builder type.</typeparam>
    /// <returns>The <paramref name="app"/>.</returns>
    public static T UseHealthChecksReport<T>(this T app, string? path = null)
        where T : IApplicationBuilder
    {
        app = app ?? throw new ArgumentNullException(nameof(app));

        var services = app.ApplicationServices;
        var healthChecks = services.GetServices<Lazy<IHealthCheck, HealthCheckMetadata>>();
        var hcOptions = services.GetRequiredService<IOptions<HealthChecksSettings>>();

        if (hcOptions.Value.EnableReport)
        {
            UseHealthChecksReportForPath(app, path ?? hcOptions.Value.ReportUrlPath, healthChecks);
        }
            
        return app;
    }

    private static void UseHealthChecksReportForPath<T>(T app, string hcApiPath, IEnumerable<Lazy<IHealthCheck, HealthCheckMetadata>> healthChecks)
        where T : IApplicationBuilder
    {
        app.UseHealthChecks(
            hcApiPath,
            new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = HealthCheckResponseAsync,
            });

        foreach (var name in healthChecks.Select(hc => hc.Metadata.ServiceKey))
        {
            app.UseHealthChecks(
                $"{hcApiPath}/{name}",
                new HealthCheckOptions
                {
                    Predicate = check => string.Equals(check.Name, name, StringComparison.InvariantCultureIgnoreCase),
                    ResponseWriter = HealthCheckResponseAsync,
                });
        }
    }

    private static async Task HealthCheckResponseAsync(HttpContext httpContext, HealthReport healthReportResult)
    {
        httpContext.Response.ContentType = "application/json";

        var jsonService = httpContext.RequestServices.GetRequiredService<IJsonService>();

        var jsonResponse = jsonService.Serialize(
            healthReportResult, 
            options =>
            {
                options.WriteIndented = true;
            });
        await httpContext.Response.WriteAsync(jsonResponse);
    }
}