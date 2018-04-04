// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunqConfigurator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the funq configurator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Hosting.Configurators
{
    using Kephas.Services;
    using Kephas.Web.ServiceStack.Composition;

    /// <summary>
    /// Configurator for the Funq IoC container.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class FunqConfigurator : IHostConfigurator
    {
        /// <summary>
        /// The container adapter.
        /// </summary>
        private readonly IComposableContainerAdapter containerAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunqConfigurator"/> class.
        /// </summary>
        /// <param name="containerAdapter">The container adapter.</param>
        public FunqConfigurator(IComposableContainerAdapter containerAdapter)
        {
            this.containerAdapter = containerAdapter;
        }

        /// <summary>
        /// Configure application host.
        /// </summary>
        /// <param name="configContext">Context for the configuration.</param>
        public void Configure(IHostConfigurationContext configContext)
        {
            configContext.Host.Container.Adapter = this.containerAdapter;
        }
    }
}