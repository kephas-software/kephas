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
    public abstract class RuntimeModelInfoProviderBase<TProvider> : IModelInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelInfoProviderBase{TProvider}"/> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">  The runtime model information factory. </param>
        protected RuntimeModelInfoProviderBase(IRuntimeModelElementFactory runtimeModelElementFactory)
        {
            Contract.Requires(runtimeModelElementFactory != null);

            this.RuntimeModelElementFactory = runtimeModelElementFactory;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<TProvider> Logger { get; set; }

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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<IElementInfo> elementInfos = null;
            await Profiler.WithInfoStopwatchAsync(
                async () => elementInfos = await this.GetElementInfosCoreAsync(constructionContext, cancellationToken).WithServerThreadingContext(),
                this.Logger).WithServerThreadingContext();

            return elementInfos;
        }

        /// <summary>
        /// Gets the element infos used for building the model space (core implementation).
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        protected abstract Task<IEnumerable<IElementInfo>> GetElementInfosCoreAsync(IModelConstructionContext constructionContext, CancellationToken cancellationToken);
    }
}