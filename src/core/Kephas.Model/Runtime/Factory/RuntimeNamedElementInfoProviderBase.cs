// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeNamedElementInfoProviderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of an element information provider based on the .NET runtime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System.Reflection;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Base implementation of a named element information provider based on the .NET runtime.
    /// </summary>
    /// <typeparam name="TElement">The type of the element being created.</typeparam>
    /// <typeparam name="TElementInfo">The type of the element information.</typeparam>
    public abstract class RuntimeNamedElementInfoProviderBase<TElement, TElementInfo> : IRuntimeElementInfoProvider<TElement, TElementInfo>
        where TElement : INamedElement
        where TElementInfo : INamedElementInfo
    {
        /// <summary>
        /// Tries to create an element information structure based on the provided native element.
        /// </summary>
        /// <param name="runtimeElementInfo">The runtime element information.</param>
        /// <returns>
        /// A new element information based on the provided native element, or <c>null</c> if the runtime element is not supported.
        /// </returns>
        public INamedElementInfo TryGetElementInfo(MemberInfo runtimeElementInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}