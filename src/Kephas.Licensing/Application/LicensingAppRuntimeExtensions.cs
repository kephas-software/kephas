// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicensingAppRuntimeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using Kephas.Operations;
using Kephas.Services;

/// <summary>
/// Extension method involving licensing.
/// </summary>
public static class LicensingAppRuntimeExtensions
{
    private const string CheckLicenseToken = $"__{nameof(CheckLicenseToken)}";

    /// <summary>
    /// Checks the license of the provided <see cref="AppIdentity"/>.
    /// </summary>
    /// <param name="appRuntime">The application runtime.</param>
    /// <param name="appIdentity">The application identity.</param>
    /// <param name="context">The context.</param>
    /// <returns>An <see cref="IOperationResult{TValue}"/> yielding the check result.</returns>
    public static IOperationResult<bool> CheckLicense(this IAppRuntime appRuntime, AppIdentity appIdentity, IContext? context)
    {
        appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));

        if (appRuntime[CheckLicenseToken] is Func<AppIdentity, IContext?, IOperationResult<bool>> checkLicense)
        {
            return checkLicense(appIdentity, context);
        }

        return new OperationResult<bool>(true).Complete();
    }

    /// <summary>
    /// Provides the callback function to check for the license.
    /// </summary>
    /// <param name="appRuntime">The application runtime.</param>
    /// <param name="checkLicense">The callback function.</param>
    /// <typeparam name="T">The type implementing <see cref="IAppRuntime"/>.</typeparam>
    /// <returns>The provided application runtime.</returns>
    public static T OnCheckLicense<T>(this T appRuntime, Func<AppIdentity, IContext?, IOperationResult<bool>> checkLicense)
        where T : IAppRuntime
    {
        appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));

        appRuntime[CheckLicenseToken] = checkLicense ?? throw new ArgumentNullException(nameof(checkLicense));
        return appRuntime;
    }
}