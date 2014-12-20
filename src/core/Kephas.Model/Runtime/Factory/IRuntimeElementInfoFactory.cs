// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeElementInfoFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using Kephas.Model.Elements.Construction;
    using Kephas.Services;

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    public interface IRuntimeElementInfoFactory
    {
        /// <summary>
        /// Tries to create an element information structure based on the provided runtime element information.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c> if the runtime element information is not supported.
        /// </returns>
        INamedElementInfo TryGetElementInfo(object runtimeElement);
    }

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    /// <typeparam name="TElementInfo">The type of the element information.</typeparam>
    /// <typeparam name="TRuntimeInfo">The type of the runtime information.</typeparam>
    [SharedAppServiceContract(AllowMultiple = true, ContractType = typeof(IRuntimeElementInfoFactory),
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
    public interface IRuntimeElementInfoFactory<TElementInfo, TRuntimeInfo> : IRuntimeElementInfoFactory
        where TElementInfo : INamedElementInfo
        where TRuntimeInfo : class
    {
    }
}