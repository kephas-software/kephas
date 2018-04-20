// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// MEF extensions for the ambient services builder.
    /// </summary>
    public static class MefAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <returns>The provided ambient services builder.</returns>
        public static AmbientServicesBuilder WithMefCompositionContainer(this AmbientServicesBuilder ambientServicesBuilder, Action<MefCompositionContainerBuilder> containerBuilderConfig = null)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            var containerBuilder = new MefCompositionContainerBuilder(new CompositionRegistrationContext(ambientServicesBuilder.AmbientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            return ambientServicesBuilder.WithCompositionContainer(container);
        }
    }
}