// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionInjector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The MEF injector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition
{
    using System;
    using System.Composition;
    using System.Composition.Hosting;

    using Kephas.Injection.SystemComposition.ExportProviders;

    /// <summary>
    /// The MEF injector.
    /// </summary>
    public class SystemCompositionInjector : SystemCompositionInjectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionInjector" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected internal SystemCompositionInjector(ContainerConfiguration configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // ReSharper disable once VirtualMemberCallInConstructor
            var compositionHost = this.CreateCompositionContext(configuration);
            this.Initialize(compositionHost);
        }

        /// <summary>
        /// Gets a value indicating whether this object is root.
        /// </summary>
        /// <value>
        /// True if this object is root, false if not.
        /// </value>
        protected override bool IsRoot => true;

        /// <summary>
        /// Creates the injector.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The new injector.
        /// </returns>
        protected virtual CompositionContext CreateCompositionContext(ContainerConfiguration configuration)
        {
            // TODO the injector should return the scoped injector, not the global one.
            configuration.WithProvider(new InjectorExportDescriptorProvider(this));
            var compositionHost = configuration.CreateContainer();
            return compositionHost;
        }
    }
}