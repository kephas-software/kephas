﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeExtensions.cs" company="Kephas Software SRL">
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
    using Kephas.Application.Reflection;

    /// <summary>
    /// Extension methods for <see cref="IAppRuntime"/>.
    /// </summary>
    public static class AppRuntimeExtensions
    {
        /// <summary>
        /// Gets the application information for the provided runtime.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The application information.
        /// </returns>
        public static IRuntimeAppInfo GetRuntimeAppInfo(this IAppRuntime appRuntime)
        {
            return new RuntimeAppInfo
                       {
                           AppId = appRuntime.GetAppId(),
                           AppInstanceId = appRuntime.GetAppInstanceId(),
                           IsRoot = appRuntime.IsRoot(),
                           ProcessId = Process.GetCurrentProcess().Id,
                           Features = appRuntime.GetFeatures().Select(f => f.Name).ToArray(),
                           HostName = appRuntime.GetHostName(),
                           HostAddress = appRuntime.GetHostAddress().ToString(),
                       };
        }

        /// <summary>
        /// Gets the application information for the provided <see cref="IAppInfo"/>.
        /// </summary>
        /// <param name="appInfo">The application runtime.</param>
        /// <returns>
        /// The application information.
        /// </returns>
        public static IRuntimeAppInfo GetRuntimeAppInfo(this IAppInfo appInfo)
        {
            return new RuntimeAppInfo
            {
                AppId = appInfo.Identity.Id,
                AppInstanceId = appInfo[AppRuntimeBase.AppInstanceIdKey] as string,
                IsRoot = false,
            };
        }
    }
}