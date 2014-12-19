// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementInfoProviderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base runtime provider for model element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Base runtime provider for model element information.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TElementInfo">The type of the element information.</typeparam>
    public abstract class RuntimeModelElementInfoProviderBase<TElement, TElementInfo> : RuntimeNamedElementInfoProviderBase<TElement, TElementInfo>
        where TElement : IModelElement
        where TElementInfo : IModelElementInfo
    {
         
    }
}