﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionServiceProviderFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition service provider factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using System;

    using Kephas.Injection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A composition service provider factory.
    /// </summary>
    public class InjectionServiceProviderFactory : IServiceProviderFactory<IAmbientServices>
    {
        private readonly IAmbientServices ambientServices;
        private readonly Action<IAmbientServices> containerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="containerBuilder">The container builder.</param>
        public InjectionServiceProviderFactory(IAmbientServices ambientServices, Action<IAmbientServices> containerBuilder)
        {
            this.ambientServices = ambientServices;
            this.containerBuilder = containerBuilder;
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
            services.UseAmbientServices(this.ambientServices);
            this.containerBuilder(this.ambientServices);

            return this.ambientServices;
        }

        /// <summary>
        /// Creates an <see cref="T:System.IServiceProvider" /> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <returns>
        /// An <see cref="T:System.IServiceProvider" />.
        /// </returns>
        public IServiceProvider CreateServiceProvider(IAmbientServices containerBuilder)
        {
            return containerBuilder.Injector.ToServiceProvider();
        }
    }
}
