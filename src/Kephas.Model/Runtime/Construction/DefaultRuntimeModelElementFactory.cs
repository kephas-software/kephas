// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeModelElementFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The default runtime model information constructor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Model.Runtime.Construction
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Configuration;
    using Kephas.Model.Runtime.Configuration.Composition;
    using Kephas.Model.Runtime.Construction.Composition;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// The default runtime model element factory.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRuntimeModelElementFactory : IRuntimeModelElementFactory
    {
        private readonly IDictionary<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata> modelElementConstructors;
        private readonly IDictionary<IRuntimeTypeInfo, List<IRuntimeModelElementConfigurator>> modelElementConfigurators;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRuntimeModelElementFactory" /> class.
        /// </summary>
        /// <param name="modelElementConstructors">The element information export factories.</param>
        /// <param name="modelElementConfigurators">The model element configurators.</param>
        /// <param name="typeRegistry">The type registry.</param>
        public DefaultRuntimeModelElementFactory(
            ICollection<IExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>> modelElementConstructors,
            ICollection<IExportFactory<IRuntimeModelElementConfigurator, RuntimeModelElementConfiguratorMetadata>> modelElementConfigurators,
            IRuntimeTypeRegistry typeRegistry)
        {
            Requires.NotNull(modelElementConstructors, nameof(modelElementConstructors));

            this.modelElementConstructors = modelElementConstructors
                    .Order()
                    .ToDictionary(e => e.CreateExport().Value, e => e.Metadata);

            this.modelElementConfigurators =
                (from cfg in modelElementConfigurators
                 group cfg by cfg.Metadata.RuntimeElementType
                 into cfgGroup
                select cfgGroup).ToDictionary(
                     g => typeRegistry.GetTypeInfo(g.Key),
                     g => g.OrderBy(e => e.Metadata.ProcessingPriority).Select(e => e.CreateExport().Value).ToList());
        }

        /// <summary>
        /// Tries to get the named element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A named element information or <c>null</c>.
        /// </returns>
        public INamedElement? TryCreateModelElement(IModelConstructionContext constructionContext, object runtimeElement)
        {
            if (runtimeElement == null)
            {
                return null;
            }

            // TODO optimize querying the factories by selecting first
            // those ones matching the runtime type
            // keep some dictionary indexed by type for this purpose.
            var element = this.modelElementConstructors
                    .Select(factoryPair => factoryPair.Key.TryCreateModelElement(constructionContext, runtimeElement))
                    .FirstOrDefault(elementInfo => elementInfo != null);

            // apply the configurators in the indicated order.
            if (runtimeElement is IRuntimeTypeInfo runtimeTypeInfo)
            {
                var configurators = this.modelElementConfigurators.TryGetValue(runtimeTypeInfo);
                configurators?.ForEach(cfg => cfg.Configure(constructionContext, element));
            }

            return element;
        }
    }
}