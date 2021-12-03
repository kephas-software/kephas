// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelInfoProviderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for runtime model info providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for runtime model info providers.
    /// </summary>
    /// <typeparam name="TProvider">The concrete provider type.</typeparam>
    public abstract class RuntimeModelInfoProviderBase<TProvider> : Loggable, IModelInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelInfoProviderBase{TProvider}"/> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">  The runtime model information factory. </param>
        protected RuntimeModelInfoProviderBase(IRuntimeModelElementFactory runtimeModelElementFactory)
        {
            runtimeModelElementFactory = runtimeModelElementFactory ?? throw new System.ArgumentNullException(nameof(runtimeModelElementFactory));

            this.RuntimeModelElementFactory = runtimeModelElementFactory;
        }

        /// <summary>
        /// Gets the runtime model information factory. 
        /// </summary>
        /// <value> The runtime model information factory. </value>
        public IRuntimeModelElementFactory RuntimeModelElementFactory { get; }

        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        public async Task<IEnumerable<IElementInfo>> GetElementInfosAsync(
            IModelConstructionContext constructionContext,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<IElementInfo> elementInfos = null;
            await Profiler.WithDebugStopwatchAsync(
                async () => elementInfos = await this.GetElementInfosCoreAsync(constructionContext, cancellationToken).PreserveThreadContext(),
                this.Logger).PreserveThreadContext();

            return elementInfos;
        }

        /// <summary>
        /// Tries to get an <see cref="IElementInfo"/> of the model space based on the provided native element information.
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
        public virtual IElementInfo? TryGetModelElementInfo(IElementInfo nativeElementInfo, IModelConstructionContext constructionContext)
        {
            return null;
        }

        /// <summary>
        /// Gets the element infos used for building the model space (core implementation).
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        protected abstract Task<IEnumerable<IElementInfo>> GetElementInfosCoreAsync(
            IModelConstructionContext constructionContext,
            CancellationToken cancellationToken);
    }
}