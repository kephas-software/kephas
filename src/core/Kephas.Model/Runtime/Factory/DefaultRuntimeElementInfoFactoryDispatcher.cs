// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeElementInfoFactoryDispatcher.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default runtime model information factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// The default runtime model information factory.
    /// </summary>
    public class DefaultRuntimeElementInfoFactoryDispatcher : IRuntimeElementInfoFactoryDispatcher
    {
        /// <summary>
        /// The element information factories.
        /// </summary>
        private readonly IDictionary<IRuntimeElementInfoFactory, RuntimeElementInfoFactoryMetadata> elementInfoFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRuntimeElementInfoFactoryDispatcher" /> class.
        /// </summary>
        /// <param name="elementInfoExportFactories">The element information export factories.</param>
        protected DefaultRuntimeElementInfoFactoryDispatcher(ICollection<IExportFactory<IRuntimeElementInfoFactory, RuntimeElementInfoFactoryMetadata>> elementInfoExportFactories)
        {
            Contract.Requires(elementInfoExportFactories != null);

            this.elementInfoFactories = elementInfoExportFactories
                    .OrderBy(e => e.Metadata.ProcessingPriority)
                    .ToDictionary(e => e.CreateExport().Value, e => e.Metadata);
        }

        /// <summary>
        /// Tries to get the named element information.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>A named element information or <c>null</c>.</returns>
        public INamedElementInfo TryGetModelElementInfo(object runtimeElement)
        {
            if (runtimeElement == null)
            {
                return null;
            }

            // TODO optimize querying the factories by selecting first
            // those ones matching the runtime type
            // keep some dictionary indexed by type for this purpose.
            return this.elementInfoFactories
                    .Select(factoryPair => factoryPair.Key.TryGetElementInfo(this, runtimeElement))
                    .FirstOrDefault(elementInfo => elementInfo != null);
        }
    }
}