// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for providers of element infos.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Factory
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Model.Elements.Construction;
    using Kephas.Services;

    /// <summary>
    /// Contract for providers of element infos.
    /// </summary>
    [ContractClass(typeof(ModelInfoProviderContractClass))]
    [SharedAppServiceContract]
    public interface IModelInfoProvider
    {
        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        Task<IEnumerable<INamedElementInfo>> GetElementInfosAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Contract class for <see cref="IModelInfoProvider"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelInfoProvider))]
    internal abstract class ModelInfoProviderContractClass : IModelInfoProvider
    {
        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        public Task<IEnumerable<INamedElementInfo>> GetElementInfosAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Ensures(Contract.Result<IEnumerable<INamedElementInfo>>() != null);
            return Contract.Result<Task<IEnumerable<INamedElementInfo>>>();
        }
    }
}