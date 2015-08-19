// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSimpleTypesModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A model information provider for data simple types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Runtime
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction.Builders;
    using Kephas.Model.Runtime.Factory;

    /// <summary>
    /// A model information provider for data simple types.
    /// </summary>
    public class DataSimpleTypesModelInfoProvider : RuntimeModelInfoProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSimpleTypesModelInfoProvider"/> class.
        /// </summary>
        /// <param name="runtimeElementInfoFactoryDispatcher">  The runtime model information factory. </param>
        public DataSimpleTypesModelInfoProvider(IRuntimeElementInfoFactoryDispatcher runtimeElementInfoFactoryDispatcher)
            : base(runtimeElementInfoFactoryDispatcher)
        {
            Contract.Requires(runtimeElementInfoFactoryDispatcher != null);
        }

        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        public override Task<IEnumerable<INamedElementInfo>> GetElementInfosAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var elementInfos = new List<INamedElementInfo>
                       {
                           new ValueTypeInfoBuilder(this.RuntimeElementInfoFactoryDispatcher, typeof(Id)).AsPrimitive().InCoreProjection().ElementInfo,
                       };

            return Task.FromResult((IEnumerable<INamedElementInfo>)elementInfos);
        }
    }
}