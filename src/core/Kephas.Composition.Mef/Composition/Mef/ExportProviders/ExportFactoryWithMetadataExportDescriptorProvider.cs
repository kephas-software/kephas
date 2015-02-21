// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryWithMetadataExportDescriptorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The export factory with metadata export descriptor provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ExportProviders
{
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;
    using System.Reflection;

    /// <summary>
    /// The export factory with metadata export descriptor provider.
    /// </summary>
    internal class ExportFactoryWithMetadataExportDescriptorProvider : ExportDescriptorProvider
    {
        /// <summary>
        /// The inner provider.
        /// </summary>
        private readonly ExportDescriptorProvider innerProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryWithMetadataExportDescriptorProvider"/> class.
        /// </summary>
        /// <param name="innerProvider">The inner provider.</param>
        public ExportFactoryWithMetadataExportDescriptorProvider(ExportDescriptorProvider innerProvider)
        {
            this.innerProvider = innerProvider;
        }

        /// <summary>
        /// Gets the export descriptors.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <param name="definitionAccessor">The definition accessor.</param>
        /// <returns>An enumeration of export promises.</returns>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            if (!contract.ContractType.GetTypeInfo().IsGenericType
                || contract.ContractType.GetGenericTypeDefinition() != typeof(IExportFactory<,>))
            {
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            // TODO get the inner provider by creating a dummy host and getting from its providers
            // TODO the one with this type name.
            // TODO rewrite the composition contract to satisfy the inner ExportFactoryWithMetadataExportDescriptorProvider
            var innerPromise = this.innerProvider.GetExportDescriptors(contract, definitionAccessor);

            // TODO The promise that is provided by the inner should be wrapped to convert the export factories
            // to the interfaces.
            var outerPromise = innerPromise;
            return outerPromise;
        }
    }
}

