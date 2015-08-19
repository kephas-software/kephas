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

    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Factory;
    using Kephas.Model.Runtime.Factory;

    /// <summary>
    /// Base class for runtime model info providers.
    /// </summary>
    public abstract class RuntimeModelInfoProviderBase : IModelInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Kephas.Model.Runtime.RuntimeModelInfoProviderBase"/> class.
        /// </summary>
        /// <param name="runtimeElementInfoFactoryDispatcher">  The runtime model information factory. </param>
        protected RuntimeModelInfoProviderBase(IRuntimeElementInfoFactoryDispatcher runtimeElementInfoFactoryDispatcher)
        {
            Contract.Requires(runtimeElementInfoFactoryDispatcher != null);

            this.RuntimeElementInfoFactoryDispatcher = runtimeElementInfoFactoryDispatcher;
        }

        /// <summary>
        /// Gets the runtime model information factory. 
        /// </summary>
        /// <value> The runtime model information factory. </value>
        public IRuntimeElementInfoFactoryDispatcher RuntimeElementInfoFactoryDispatcher { get; }

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