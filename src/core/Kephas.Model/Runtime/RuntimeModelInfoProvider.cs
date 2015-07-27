// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Model provider based on the .NET runtime and the type system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Extensions;
    using Kephas.Logging;
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Factory;
    using Kephas.Model.Resources;
    using Kephas.Model.Runtime.Factory;

    /// <summary>
    /// Model provider based on the .NET runtime and the type system.
    /// </summary>
    public class RuntimeModelInfoProvider : IModelInfoProvider
    {
        /// <summary>
        /// The model registrars.
        /// </summary>
        private readonly ICollection<IRuntimeModelRegistry> modelRegistries;

        /// <summary>
        /// The element information factories.
        /// </summary>
        private readonly IDictionary<IRuntimeElementInfoFactory, RuntimeElementInfoFactoryMetadata> elementInfoFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelInfoProvider" /> class.
        /// </summary>
        /// <param name="modelRegistries">The model registries.</param>
        /// <param name="elementInfoExportFactories">The element information export factories.</param>
        public RuntimeModelInfoProvider(
            ICollection<IRuntimeModelRegistry> modelRegistries,
            ICollection<IExportFactory<IRuntimeElementInfoFactory, RuntimeElementInfoFactoryMetadata>> elementInfoExportFactories)
        {
            Contract.Requires(modelRegistries != null);
            Contract.Requires(elementInfoExportFactories != null);

            this.modelRegistries = modelRegistries;
            this.elementInfoFactories = elementInfoExportFactories.ToDictionary(e => e.CreateExport().Value, e => e.Metadata);
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<RuntimeModelInfoProvider> Logger { get; set; }

        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        public async Task<IEnumerable<INamedElementInfo>> GetElementInfosAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var runtimeElements = new HashSet<object>();
            foreach (var modelRegistry in this.modelRegistries)
            {
                var registryElements = await modelRegistry.GetRuntimeElementsAsync(cancellationToken).ConfigureAwait(false);
                runtimeElements.AddRange(registryElements.Select(this.NormalizeRuntimeElement));

                cancellationToken.ThrowIfCancellationRequested();
            }

            var elementInfos = new List<INamedElementInfo>();

            foreach (var runtimeElement in runtimeElements)
            {
                if (runtimeElement == null)
                {
                    continue;
                }

                var elementInfo = this.TryGetModelElementInfo(runtimeElement);
                if (elementInfo == null)
                {
                    this.Logger.WarnFormat(Strings.CannotProvideElementInfoForRuntimeElement, runtimeElement.ToString());
                    continue;
                }

                elementInfos.Add(elementInfo);
            }

            cancellationToken.ThrowIfCancellationRequested();

            return elementInfos;
        }

        /// <summary>
        /// Normalizes the runtime element by returing the associated <see cref="TypeInfo"/> instead.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The normalized runtime type.</returns>
        private object NormalizeRuntimeElement(object runtimeElement)
        {
            var runtimeType = runtimeElement as Type;
            return runtimeType != null ? runtimeType.GetTypeInfo() : runtimeElement;
        }

        /// <summary>
        /// Tries to get the named element information.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>A named element information or <c>null</c>.</returns>
        private INamedElementInfo TryGetModelElementInfo(object runtimeElement)
        {
            return this.elementInfoFactories
                    .Select(factoryPair => factoryPair.Key.TryGetElementInfo(runtimeElement))
                    .FirstOrDefault(elementInfo => elementInfo != null);
        }
    }
}