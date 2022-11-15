// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientAppManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client
{
    using Kephas.Application;
    using Kephas.Services;
    using Kephas.Services.Behaviors;

    /// <summary>
    /// The client app manager.
    /// </summary>
    /// <remarks>
    /// Made the class internal and add it to the service collection in <see cref="ClientApp{TApp}.ConfigureHost"/>,
    /// otherwise it will be used on server, too, overriding the server app manager.
    /// </remarks>
    [OverridePriority(Priority.Highest)]
    internal class ClientAppManager : DefaultAppManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAppManager"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="serviceProvider">The injector.</param>
        public ClientAppManager(
            IAppRuntime appRuntime,
            IServiceProvider serviceProvider)
            : base(appRuntime, serviceProvider)
        {
        }
    }
}