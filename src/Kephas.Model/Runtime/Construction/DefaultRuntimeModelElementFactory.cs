// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeModelElementFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default runtime model information constructor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Model.Factory;
    using Kephas.Model.Runtime.Construction.Composition;
    using Kephas.Services;

    /// <summary>
    /// The default runtime model element factory.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRuntimeModelElementFactory : IRuntimeModelElementFactory
    {
        /// <summary>
        /// The element information factories.
        /// </summary>
        private readonly IDictionary<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata> modelElementConstructors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRuntimeModelElementFactory" /> class.
        /// </summary>
        /// <param name="modelElementConstructors">The element information export factories.</param>
        public DefaultRuntimeModelElementFactory(ICollection<IExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>> modelElementConstructors)
        {
            Contract.Requires(modelElementConstructors != null);

            this.modelElementConstructors = modelElementConstructors
                    .OrderBy(e => e.Metadata.ProcessingPriority)
                    .ToDictionary(e => e.CreateExport().Value, e => e.Metadata);
        }

        /// <summary>
        /// Tries to get the named element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A named element information or <c>null</c>.
        /// </returns>
        public INamedElement TryCreateModelElement(IModelConstructionContext constructionContext, object runtimeElement)
        {
            if (runtimeElement == null)
            {
                return null;
            }

            // TODO optimize querying the factories by selecting first
            // those ones matching the runtime type
            // keep some dictionary indexed by type for this purpose.
            return this.modelElementConstructors
                    .Select(factoryPair => factoryPair.Key.TryCreateModelElement(constructionContext, runtimeElement))
                    .FirstOrDefault(elementInfo => elementInfo != null);
        }
    }
}