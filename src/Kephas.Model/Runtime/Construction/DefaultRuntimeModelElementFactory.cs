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

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Configuration;
    using Kephas.Model.Runtime.Configuration.Composition;
    using Kephas.Model.Runtime.Construction.Composition;
    using Kephas.Reflection;
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
        /// The model element configurators.
        /// </summary>
        private readonly IDictionary<IDynamicTypeInfo, List<IRuntimeModelElementConfigurator>> modelElementConfigurators;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRuntimeModelElementFactory" /> class.
        /// </summary>
        /// <param name="modelElementConstructors">The element information export factories.</param>
        /// <param name="modelElementConfigurators">The model element configurators.</param>
        public DefaultRuntimeModelElementFactory(
            ICollection<IExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>> modelElementConstructors,
            ICollection<IExportFactory<IRuntimeModelElementConfigurator, RuntimeModelElementConfiguratorMetadata>> modelElementConfigurators)
        {
            Contract.Requires(modelElementConstructors != null);

            this.modelElementConstructors = modelElementConstructors
                    .OrderBy(e => e.Metadata.ProcessingPriority)
                    .ToDictionary(e => e.CreateExport().Value, e => e.Metadata);

            this.modelElementConfigurators =
                (from cfg in modelElementConfigurators
                 group cfg by cfg.Metadata.RuntimeElementType
                 into cfgGroup select cfgGroup).ToDictionary(
                     g => g.Key.AsDynamicTypeInfo(),
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
        public INamedElement TryCreateModelElement(IModelConstructionContext constructionContext, object runtimeElement)
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
            var runtimeDynamicType = runtimeElement as IDynamicTypeInfo;
            if (runtimeDynamicType != null)
            {
                var configurators = this.modelElementConfigurators.TryGetValue(runtimeDynamicType);
                configurators?.ForEach(cfg => cfg.With(element).Configure());
            }

            return element;
        }
    }
}