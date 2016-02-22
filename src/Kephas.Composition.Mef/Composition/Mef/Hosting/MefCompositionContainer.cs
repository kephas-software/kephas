﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Diagnostics.Contracts;

    using Kephas.Composition;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Composition.Mef.Internals;

    /// <summary>
    /// The MEF composition container.
    /// </summary>
    public class MefCompositionContainer : MefCompositionContext
    {
        /// <summary>
        /// The scope provider.
        /// </summary>
        private MefScopeProvider scopeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefCompositionContainer" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        internal MefCompositionContainer(ContainerConfiguration configuration)
        {
            Contract.Requires(configuration != null);

            this.Initialize(this.CreateCompositionContext(configuration));
        }

        /// <summary>
        /// Creates a new scoped composition context.
        /// </summary>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        public override ICompositionContext CreateScopedContext()
        {
            this.scopeProvider = this.scopeProvider ?? this.GetExport<MefScopeProvider>();

            return new MefScopedCompositionContext(this.scopeProvider.CreateScopedContextExport());
        }

        /// <summary>
        /// Creates the composition context.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The new composition context.
        /// </returns>
        private CompositionContext CreateCompositionContext(ContainerConfiguration configuration)
        {
            configuration.WithPart<MefScopeProvider>();
            configuration.WithProvider(new FactoryExportDescriptorProvider<ICompositionContext>(() => this, isShared: true));
            return configuration.CreateContainer();
        }
    }
}