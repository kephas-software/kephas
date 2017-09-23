// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefCompositionContainer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The MEF composition container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Hosting
{
    using System.Composition;
    using System.Composition.Hosting;

    using Kephas.Composition;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// The MEF composition container.
    /// </summary>
    public class MefCompositionContainer : MefCompositionContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefCompositionContainer" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected internal MefCompositionContainer(ContainerConfiguration configuration)
        {
            Requires.NotNull(configuration, nameof(configuration));

            this.Initialize(this.CreateCompositionContext(configuration));
        }

        /// <summary>
        /// Creates the composition context.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The new composition context.
        /// </returns>
        protected virtual CompositionContext CreateCompositionContext(ContainerConfiguration configuration)
        {
            configuration.WithProvider(new FactoryExportDescriptorProvider<ICompositionContext>(() => this, isShared: true));
            return configuration.CreateContainer();
        }
    }
}