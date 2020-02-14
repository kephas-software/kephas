// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionServiceProviderFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition service provider factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting
{
    using System;

    using Kephas;
    using Kephas.Composition;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A composition service provider factory.
    /// </summary>
    public class CompositionServiceProviderFactory : IServiceProviderFactory<IAmbientServices>
    {
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public CompositionServiceProviderFactory(IAmbientServices ambientServices)
        {
            this.ambientServices = ambientServices;
        }

        /// <summary>
        /// Creates a container builder from an
        /// <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>
        /// A container builder that can be used to create an <see cref="T:System.IServiceProvider" />.
        /// </returns>
        public IAmbientServices CreateBuilder(IServiceCollection services)
        {
            return this.ambientServices;
        }

        /// <summary>
        /// Creates an <see cref="T:System.IServiceProvider" /> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <returns>
        /// An <see cref="T:System.IServiceProvider" />
        /// </returns>
        public IServiceProvider CreateServiceProvider(IAmbientServices containerBuilder)
        {
            return containerBuilder.CompositionContainer.ToServiceProvider();
        }
    }
}
