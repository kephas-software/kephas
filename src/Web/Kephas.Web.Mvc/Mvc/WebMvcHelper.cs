// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebMvcHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kephas.Web.Mvc;

/// <summary>
/// Extension methods for MVC.
/// </summary>
public static class WebMvcHelper
{
    /// <summary>
    /// The cache profile duration.
    /// </summary>
    public static readonly int CacheProfileDuration = 60;

    /// <summary>
    /// Add the controllers in authenticated mode. 
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="mvcBuilderConfig">Additional configuration for the MVC builder.</param>
    /// <param name="mvcOptionsConfig">Additional configuration for the MVC options.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection ConfigureControllers(
        this IServiceCollection services,
        Action<IMvcBuilder>? mvcBuilderConfig = null,
        Action<MvcOptions>? mvcOptionsConfig = null)
    {
        services.TryAddSingleton<IApplicationPartsProvider, ApplicationPartsProvider>();
        var mvcBuilder = services.AddControllers(
                options =>
                {
                    if (options.Filters.Any(item => item is IAllowAnonymousFilter))
                    {
                        return;
                    }

                    options.CacheProfiles.Add("Default", new CacheProfile { Duration = CacheProfileDuration });
                    mvcOptionsConfig?.Invoke(options);
                })
            .ConfigureApplicationParts();
        mvcBuilderConfig?.Invoke(mvcBuilder);

        return services;
    }

    /// <summary>
    /// Adds the application parts from the application assemblies.
    /// </summary>
    /// <param name="mvcBuilder">The <see cref="IMvcBuilder"/> instance.</param>
    /// <returns>The provided builder.</returns>
    public static IMvcBuilder ConfigureApplicationParts(this IMvcBuilder mvcBuilder)
    {
        ArgumentNullException.ThrowIfNull(mvcBuilder);

        var partsProvider = GetApplicationPartsProvider(mvcBuilder);
        mvcBuilder.PartManager.ApplicationParts.AddRange(partsProvider.GetApplicationParts());

        return mvcBuilder;
    }

    private static IApplicationPartsProvider GetApplicationPartsProvider(IMvcBuilder mvcBuilder)
    {
        var container = mvcBuilder.Services.BuildServiceProvider();
        return container.GetRequiredService<IApplicationPartsProvider>();
    }
}
