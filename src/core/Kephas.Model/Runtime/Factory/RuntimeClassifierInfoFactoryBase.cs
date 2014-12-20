// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeClassifierInfoFactoryBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base runtime provider for classifier information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System.Reflection;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Base runtime provider for classifier information.
    /// </summary>
    /// <typeparam name="TElementInfo">The type of the element information.</typeparam>
    /// <typeparam name="TRuntimeInfo">The type of the runtime information.</typeparam>
    public abstract class RuntimeClassifierInfoFactoryBase<TElementInfo, TRuntimeInfo> : RuntimeModelElementInfoFactoryBase<TElementInfo, TRuntimeInfo>
        where TElementInfo : IClassifierInfo
        where TRuntimeInfo : TypeInfo
    {
    }
}