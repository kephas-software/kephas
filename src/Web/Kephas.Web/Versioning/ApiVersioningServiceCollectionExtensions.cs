// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiVersioningServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET8_0_OR_GREATER
using Asp.Versioning;
#else
using Microsoft.AspNetCore.Mvc.Versioning;
#endif
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;


namespace Kephas.Web.Versioning;

/// <summary>
/// Provides extension methods to the <see cref="IServiceCollection"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ApiVersioningServiceCollectionExtensions
{
    /// <summary>
    /// Configures the API versioning.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
    {
        // Configure API version.
#if NET8_0_OR_GREATER
#else
        services.AddVersionedApiExplorer(
            options =>
            {
                options.GroupNameFormat = "VVV";
                options.SubstituteApiVersionInUrl = true;
            });
#endif

        services
            .AddApiVersioning(
                options =>
                {
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ReportApiVersions = true;
                })
#if NET8_0_OR_GREATER
            .AddApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "VVV";
                    options.SubstituteApiVersionInUrl = true;
                })
#else
#endif
            ;

        return services;
    }
}