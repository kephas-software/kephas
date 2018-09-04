// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppManifestExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application manifest extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System.Diagnostics;
    using System.Linq;

    using Kephas.Application;

    /// <summary>
    /// Extension methods for <see cref="IAppManifest"/>.
    /// </summary>
    public static class AppManifestExtensions
    {
        /// <summary>
        /// An IAppManifest extension method that gets application information.
        /// </summary>
        /// <param name="appManifest">The appManifest to act on.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The application information.
        /// </returns>
        public static IAppInfo GetAppInfo(this IAppManifest appManifest, IAppRuntime appRuntime)
        {
            return new AppInfo
                       {
                           AppId = appManifest.AppId,
                           AppInstanceId = appManifest.AppInstanceId,
                           ProcessId = Process.GetCurrentProcess().Id,
                           Features = appManifest.Features.Select(f => f.Name).ToArray(),
                           HostName = appRuntime.GetHostName(),
                           HostAddress = appRuntime.GetHostAddress().ToString(),
                       };
        }
    }
}