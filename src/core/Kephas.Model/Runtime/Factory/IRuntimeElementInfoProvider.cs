// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System.Reflection;

    using Kephas.Model.Elements.Construction;
    using Kephas.Services;

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    public interface IRuntimeElementInfoProvider
    {
        /// <summary>
        /// Tries to create an element information structure based on the provided native element.
        /// </summary>
        /// <param name="runtimeElementInfo">The runtime element information.</param>
        /// <returns>A new element information based on the provided native element, or <c>null</c> if the runtime element is not supported.</returns>
        INamedElementInfo TryGetElementInfo(MemberInfo runtimeElementInfo);
    }

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    /// <typeparam name="TElement">The type of the element being created.</typeparam>
    /// <typeparam name="TElementInfo">The type of the element information.</typeparam>
    [SharedAppServiceContract(AllowMultiple = true, ContractType = typeof(IRuntimeElementInfoProvider),
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
    public interface IRuntimeElementInfoProvider<TElement, TElementInfo> : IRuntimeElementInfoProvider
        where TElementInfo : INamedElementInfo
    {
    }
}