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

    using Kephas.Model.Factory;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Factory;
    using Kephas.Reflection;

    /// <summary>
    /// Base class for runtime model info providers.
    /// </summary>
    public abstract class RuntimeModelInfoProviderBase : IModelInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelInfoProviderBase"/> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">  The runtime model information factory. </param>
        protected RuntimeModelInfoProviderBase(IRuntimeModelElementFactory runtimeModelElementFactory)
        {
            Contract.Requires(runtimeModelElementFactory != null);

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
        public abstract Task<IEnumerable<IElementInfo>> GetElementInfosAsync(IModelConstructionContext constructionContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}