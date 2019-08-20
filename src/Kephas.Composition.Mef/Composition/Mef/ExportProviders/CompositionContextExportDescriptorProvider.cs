// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContextExportDescriptorProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition context export descriptor provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ExportProviders
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;

    using Kephas.Composition.Mef.Hosting;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A composition context export descriptor provider.
    /// </summary>
    public class CompositionContextExportDescriptorProvider : ExportDescriptorProvider, IExportProvider
    {
        /// <summary>
        /// The composition container.
        /// </summary>
        private readonly ICompositionContext compositionContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContextExportDescriptorProvider"/>
        /// class.
        /// </summary>
        /// <param name="compositionContainer">The composition container.</param>
        public CompositionContextExportDescriptorProvider(ICompositionContext compositionContainer)
        {
            Requires.NotNull(compositionContainer, nameof(compositionContainer));
            this.compositionContainer = compositionContainer;
        }

        /// <summary>
        /// Promise export descriptors for composition context.
        /// </summary>
        /// <param name="contract">The export key required by another component.</param><param name="descriptorAccessor">Accesses the other export descriptors present in the composition.</param>
        /// <returns>
        /// Promises for new export descriptors.
        /// </returns>
        /// <remarks>
        /// A provider will only be queried once for each unique export key.
        ///             The descriptor accessor can only be queried immediately if the descriptor being promised is an adapter, such as
        ///             <see cref="T:System.Lazy`1"/>; otherwise, dependencies should only be queried within execution of the function provided
        ///             to the <see cref="T:System.Composition.Hosting.Core.ExportDescriptorPromise"/>. The actual descriptors provided should not close over or reference any
        ///             aspect of the dependency/promise structure, as this should be able to be GC'ed.
        /// </remarks>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            if (contract.ContractType != typeof(ICompositionContext))
            {
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            return new[]
                {
                    new ExportDescriptorPromise(
                        contract,
                        contract.ContractType.Name,
                        true,
                        ExportDescriptorProvider.NoDependencies,
                        dependencies => ExportDescriptor.Create(
                            (c, o) =>
                                {
                                    var instance = MefCompositionContextBase.TryGetCompositionContext(c, createNewIfMissing: false) ?? this.compositionContainer;
                                    return instance;
                                },
                        ExportDescriptorProvider.NoMetadata))
               };
        }
    }
}