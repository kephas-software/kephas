﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Model provider based on the .NET runtime and the type system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Model.Construction;
    using Kephas.Model.Resources;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;
    using Kephas.Runtime;
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
            Requires.NotNull(runtimeModelElementFactory, nameof(runtimeModelElementFactory));
            Requires.NotNull(modelRegistries, nameof(modelRegistries));

            this.modelRegistries = modelRegistries;
        }

        /// <summary>
        /// Tries to get an <see cref="IElementInfo"/> based on the provided native element information.
        /// </summary>
        /// <param name="nativeElementInfo">The native element information.</param>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// The constructed generic type or <c>null</c> if the provider cannot handle the provided type information.
        /// </returns>
        /// <remarks>
        /// A return value of <c>null</c> indicates only that the provided <paramref name="nativeElementInfo"/> cannot be handled.
        /// For any other errors an exception should be thrown.
        /// </remarks>
        public override IElementInfo TryGetElementInfo(IElementInfo nativeElementInfo, IModelConstructionContext constructionContext)
        {
            var runtimeTypeInfo = nativeElementInfo as IRuntimeTypeInfo;
            if (runtimeTypeInfo == null)
            {
                return null;
            }

            constructionContext.RuntimeModelElementFactory = this.RuntimeModelElementFactory;
            var constructedTypeInfo = this.RuntimeModelElementFactory.TryCreateModelElement(constructionContext, runtimeTypeInfo);
            return constructedTypeInfo;
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

            var runtimeElements = await this.GetRuntimeElementInfosAsync(cancellationToken).PreserveThreadContext();

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
        private async Task<HashSet<IRuntimeTypeInfo>> GetRuntimeElementInfosAsync(CancellationToken cancellationToken)
        {
            var runtimeElements = new HashSet<IRuntimeTypeInfo>();
            foreach (var modelRegistry in this.modelRegistries)
            {
                var registryElements = await modelRegistry.GetRuntimeElementsAsync(cancellationToken).PreserveThreadContext();
                runtimeElements.AddRange(registryElements.Select(this.ToRuntimeTypeInfo));

                cancellationToken.ThrowIfCancellationRequested();
            }

            return runtimeElements;
        }

        /// <summary>
        /// Normalizes the runtime element by returning the associated <see cref="IRuntimeTypeInfo"/> instead.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The normalized runtime type.</returns>
        private IRuntimeTypeInfo ToRuntimeTypeInfo(object runtimeElement)
        {
            var runtimeTypeInfo = runtimeElement as TypeInfo;
            if (runtimeTypeInfo != null)
            {
                return runtimeTypeInfo.AsRuntimeTypeInfo();
            }

            var runtimeType = runtimeElement as Type;
            if (runtimeType != null)
            {
                return runtimeType.AsRuntimeTypeInfo();
            }

            return runtimeElement as IRuntimeTypeInfo;
        }
    }
}