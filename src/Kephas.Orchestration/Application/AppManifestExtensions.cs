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
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The application information.
        /// </returns>
        public static IRuntimeAppInfo GetAppInfo(this IAppRuntime appRuntime)
        {
            return new RuntimeAppInfo
                       {
                           AppId = appRuntime.GetAppId(),
                           AppInstanceId = appRuntime.GetAppInstanceId(),
                           ProcessId = Process.GetCurrentProcess().Id,
                           Features = appRuntime.GetFeatures().Select(f => f.Name).ToArray(),
                           HostName = appRuntime.GetHostName(),
                           HostAddress = appRuntime.GetHostAddress().ToString(),
                       };
        }
    }
}