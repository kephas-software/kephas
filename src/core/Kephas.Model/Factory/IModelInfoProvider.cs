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
        /// <returns>An enumeration of element information.</returns>
        IEnumerable<INamedElementInfo> GetElementInfos();
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
        /// <returns>
        /// An enumeration of element information.
        /// </returns>
        public IEnumerable<INamedElementInfo> GetElementInfos()
        {
            Contract.Ensures(Contract.Result<IEnumerable<INamedElementInfo>>() != null);
            return Contract.Result<IEnumerable<INamedElementInfo>>();
        }
    }
}