// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacAmbientServicesBuilderExtensions.cs" company="Kephas Software SRL">
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
    public static class AutofacAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <returns>The provided ambient services builder.</returns>
        public static AmbientServicesBuilder WithAutofacCompositionContainer(this AmbientServicesBuilder ambientServicesBuilder, Action<AutofacCompositionContainerBuilder> containerBuilderConfig = null)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            var containerBuilder = new AutofacCompositionContainerBuilder(new CompositionRegistrationContext(ambientServicesBuilder.AmbientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            return ambientServicesBuilder.WithCompositionContainer(container);
        }
    }
}