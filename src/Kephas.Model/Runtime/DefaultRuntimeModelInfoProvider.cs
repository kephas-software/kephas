// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeModelInfoProvider.cs" company="Quartz Software SRL">
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

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Model.Construction;
    using Kephas.Model.Resources;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Model provider based on the .NET runtime and the type system.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRuntimeModelInfoProvider : RuntimeModelInfoProviderBase<DefaultRuntimeModelInfoProvider>
    {
        /// <summary>
        /// The model registrars.
        /// </summary>
        private readonly ICollection<IRuntimeModelRegistry> modelRegistries;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRuntimeModelInfoProvider" /> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">The runtime model info factory.</param>
        /// <param name="modelRegistries">The model registries.</param>
        public DefaultRuntimeModelInfoProvider(
            IRuntimeModelElementFactory runtimeModelElementFactory,
            ICollection<IRuntimeModelRegistry> modelRegistries)
            : base(runtimeModelElementFactory)
        {
            Contract.Requires(runtimeModelElementFactory != null);
            Contract.Requires(modelRegistries != null);

            this.modelRegistries = modelRegistries;
        }

        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        protected override async Task<IEnumerable<IElementInfo>> GetElementInfosCoreAsync(IModelConstructionContext constructionContext, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var runtimeElements = await this.GetRuntimeElementInfos(cancellationToken).WithServerThreadingContext();

            constructionContext.RuntimeModelElementFactory = this.RuntimeModelElementFactory;
            var elementInfos = new List<INamedElement>();

            foreach (var runtimeElement in runtimeElements)
            {
                if (runtimeElement == null)
                {
                    continue;
                }

                var elementInfo = this.RuntimeModelElementFactory.TryCreateModelElement(constructionContext, runtimeElement);
                if (elementInfo == null)
                {
                    this.Logger.Warn(Strings.CannotProvideElementInfoForRuntimeElement, runtimeElement.ToString());
                    continue;
                }

                elementInfos.Add(elementInfo);
            }

            cancellationToken.ThrowIfCancellationRequested();

            return elementInfos;
        }

        /// <summary>
        /// Gets the runtime element infos.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The runtime element infos.
        /// </returns>
        private async Task<HashSet<IDynamicTypeInfo>> GetRuntimeElementInfos(CancellationToken cancellationToken)
        {
            var runtimeElements = new HashSet<IDynamicTypeInfo>();
            foreach (var modelRegistry in this.modelRegistries)
            {
                var registryElements =
                    await modelRegistry.GetRuntimeElementsAsync(cancellationToken).WithServerThreadingContext();
                runtimeElements.AddRange(registryElements.Select(this.ToDynamicTypeInfo));

                cancellationToken.ThrowIfCancellationRequested();
            }

            return runtimeElements;
        }

        /// <summary>
        /// Normalizes the runtime element by returning the associated <see cref="IDynamicTypeInfo"/> instead.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The normalized runtime type.</returns>
        private IDynamicTypeInfo ToDynamicTypeInfo(object runtimeElement)
        {
            var runtimeType = runtimeElement as Type;
            if (runtimeType != null)
            {
                return runtimeType.AsDynamicTypeInfo();
            }

            var runtimeTypeInfo = runtimeElement as TypeInfo;
            if (runtimeTypeInfo != null)
            {
                return runtimeTypeInfo.AsDynamicTypeInfo();
            }

            return runtimeElement as IDynamicTypeInfo;
        }
    }
}