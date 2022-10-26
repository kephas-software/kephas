// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    /// <summary>
    /// Default implementation of <see cref="AppBase{TAmbientServices}"/>.
    /// </summary>
    public class App : AppBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="serviceProviderBuilder">Optional. The service provider serviceProviderBuilder.</param>
        /// <param name="ambientServices">Optional. The ambient services. If not provided then
        ///     a new instance of <see cref="Kephas.AmbientServices"/> will be created and used.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public App(
            Func<IAmbientServices, IServiceProvider>? serviceProviderBuilder = null,
            IAmbientServices? ambientServices = null,
            IAppArgs? appArgs = null)
            : base(ambientServices, appArgs: appArgs)
        {
        }
    }
}