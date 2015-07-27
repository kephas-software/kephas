// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   MEF extensions for the ambient services builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using Kephas.Composition.Mef.Hosting;

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
        public static AmbientServicesBuilder WithMefCompositionContainer(this AmbientServicesBuilder ambientServicesBuilder, Action<MefCompositionContainerBuilder> containerBuilderConfig)
        {
            Contract.Requires(ambientServicesBuilder != null);
            Contract.Requires(containerBuilderConfig != null);

            var containerBuilder = new MefCompositionContainerBuilder(
                ambientServicesBuilder.AmbientServices.LogManager,
                ambientServicesBuilder.AmbientServices.ConfigurationManager,
                ambientServicesBuilder.AmbientServices.PlatformManager);

            containerBuilderConfig(containerBuilder);

            return ambientServicesBuilder.WithCompositionContainer(containerBuilder.CreateContainer());
        }

        /// <summary>
        /// Sets asynchronously the composition container to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <returns>The provided ambient services builder.</returns>
        public static async Task<AmbientServicesBuilder> WithMefCompositionContainerAsync(this AmbientServicesBuilder ambientServicesBuilder, Action<MefCompositionContainerBuilder> containerBuilderConfig = null)
        {
            Contract.Requires(ambientServicesBuilder != null);

            var containerBuilder = new MefCompositionContainerBuilder(
                ambientServicesBuilder.AmbientServices.LogManager,
                ambientServicesBuilder.AmbientServices.ConfigurationManager,
                ambientServicesBuilder.AmbientServices.PlatformManager);

            if (containerBuilderConfig != null)
            {
                containerBuilderConfig(containerBuilder);
            }

            return ambientServicesBuilder.WithCompositionContainer(await containerBuilder.CreateContainerAsync());
        }
    }
}