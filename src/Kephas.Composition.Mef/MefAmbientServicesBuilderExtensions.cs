// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   MEF extensions for the ambient services builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

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
            
            var containerBuilder = new MefCompositionContainerBuilder(new CompositionContainerBuilderContext(ambientServicesBuilder.AmbientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            return ambientServicesBuilder.WithCompositionContainer(containerBuilder.CreateContainer());
        }

        /// <summary>
        /// Sets asynchronously the composition container to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <returns>A promise of the provided ambient services builder.</returns>
        public static async Task<AmbientServicesBuilder> WithMefCompositionContainerAsync(this AmbientServicesBuilder ambientServicesBuilder, Action<MefCompositionContainerBuilder> containerBuilderConfig = null)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            var containerBuilder = new MefCompositionContainerBuilder(new CompositionContainerBuilderContext(ambientServicesBuilder.AmbientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            var container = await containerBuilder.CreateContainerAsync().PreserveThreadContext();
            return ambientServicesBuilder.WithCompositionContainer(container);
        }
    }
}