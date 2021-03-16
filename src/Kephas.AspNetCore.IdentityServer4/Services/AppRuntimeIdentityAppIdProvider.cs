// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeIdentityAppIdProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Services
{
    using System;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// Service implementing the <see cref="IIdentityAppIdProvider"/>
    /// providing the <see cref="AppIdentity"/> from the <see cref="IAppRuntime"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class AppRuntimeIdentityAppIdProvider : IIdentityAppIdProvider
    {
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppRuntimeIdentityAppIdProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        public AppRuntimeIdentityAppIdProvider(IAppRuntime appRuntime)
        {
            this.appRuntime = appRuntime;
        }

        /// <summary>
        /// Gets the application identity for the identity infrastructure.
        /// </summary>
        /// <returns>The <see cref="AppIdentity"/>.</returns>
        public AppIdentity GetIdentityAppId()
        {
            return this.appRuntime.GetAppIdentity() ?? new AppIdentity(Assembly.GetEntryAssembly().GetName().Name);
        }
    }
}
