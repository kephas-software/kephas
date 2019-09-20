// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesDependencyInjectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dependency injection ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Composition.Hosting;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Extensions.DependencyInjection.Hosting;

    /// <summary>
    /// Microsoft.Extensions.DependencyInjection related ambient services extensions.
    /// </summary>
    public static class AmbientServicesDependencyInjectionExtensions
    {
        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <returns>The provided ambient services builder.</returns>
        public static IAmbientServices WithDependencyInjectionCompositionContainer(this IAmbientServices ambientServices, Action<DependencyInjectionCompositionContainerBuilder> containerBuilderConfig = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var containerBuilder = new DependencyInjectionCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            return ambientServices.WithCompositionContainer(container);
        }
    }
}