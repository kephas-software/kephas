// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesAutofacExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Composition.Hosting;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Autofac related ambient services builder extensions.
    /// </summary>
    public static class AmbientServicesAutofacExtensions
    {
        /// <summary>
        /// Builds the composition container with Autofac and adds it to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices BuildWithAutofac(this IAmbientServices ambientServices, Action<AutofacCompositionContainerBuilder> containerBuilderConfig = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var containerBuilder = new AutofacCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            return ambientServices.WithCompositionContainer(container);
        }
    }
}