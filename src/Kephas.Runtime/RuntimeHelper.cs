// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Application;

namespace Kephas;

/// <summary>
/// Helper class for the runtime.
/// </summary>
public static class RuntimeHelper
{
    /// <summary>
    /// Gets the application runtime.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <returns>
    /// The application runtime.
    /// </returns>
    public static IAppRuntime GetAppRuntime(this IAppServiceCollection appServices)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));

        return appServices.TryGetServiceInstance<IAppRuntime>()
               ?? throw new InvalidOperationException("The application runtime is not registered.");
    }
}
