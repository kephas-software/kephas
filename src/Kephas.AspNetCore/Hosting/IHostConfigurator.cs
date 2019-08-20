// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IHostConfigurator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Hosting
{
    using Kephas.AspNetCore.Application;
    using Kephas.Services;

    /// <summary>
    /// Interface for host configurator.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IHostConfigurator
    {
        /// <summary>
        /// Configures the host using the given application context.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        void Configure(IAspNetAppContext appContext);
    }
}