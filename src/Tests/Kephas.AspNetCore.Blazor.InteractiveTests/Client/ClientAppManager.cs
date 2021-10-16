﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientAppManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client
{
    using Kephas.Application;
    using Kephas.Injection;
    using Kephas.Services;
    using Kephas.Services.Behaviors;

    /// <summary>
    /// The client app manager.
    /// </summary>
    [OverridePriority(Priority.Highest)]
    public class ClientAppManager : DefaultAppManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAppManager"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="injector">The injector.</param>
        /// <param name="serviceBehaviorProvider">The service behavior provider.</param>
        public ClientAppManager(
            IAppRuntime appRuntime,
            IInjector injector,
            IServiceBehaviorProvider? serviceBehaviorProvider = null)
            : base(appRuntime, injector, serviceBehaviorProvider)
        {
        }
    }
}