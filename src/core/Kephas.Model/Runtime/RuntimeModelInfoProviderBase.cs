// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelInfoProviderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for runtime model info providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Runtime.Factory;

    /// <summary>
    /// Base class for runtime model info providers.
    /// </summary>
    public abstract class RuntimeModelInfoProviderBase : IRuntimeModelInfoProvider
    {
        /// <summary>
        /// The element information factories.
        /// </summary>
        private readonly IDictionary<IRuntimeElementInfoFactory, RuntimeElementInfoFactoryMetadata> elementInfoFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelInfoProviderBase" /> class.
        /// </summary>
        /// <param name="elementInfoExportFactories">The element information export factories.</param>
        protected RuntimeModelInfoProviderBase(ICollection<IExportFactory<IRuntimeElementInfoFactory, RuntimeElementInfoFactoryMetadata>> elementInfoExportFactories)
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
            // TODO optimize querying the factories by selecting first
            // those ones matching the runtime type
            // keep some dictionary indexed by type for this purpose.
            return this.elementInfoFactories
                    .Select(factoryPair => factoryPair.Key.TryGetElementInfo(this, runtimeElement))
                    .FirstOrDefault(elementInfo => elementInfo != null);
        }

        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        public abstract Task<IEnumerable<INamedElementInfo>> GetElementInfosAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}