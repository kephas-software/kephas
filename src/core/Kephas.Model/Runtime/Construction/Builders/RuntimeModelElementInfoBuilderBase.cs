// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementInfoBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract builder for runtime model element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Builders
{
    using System.Reflection;

    /// <summary>
    /// Base abstract builder for runtime model element information.
    /// </summary>
    /// <typeparam name="TModelElementInfo">Type of the model element information.</typeparam>
    /// <typeparam name="TRuntimeElement">Type of the runtime element.</typeparam>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    public abstract class RuntimeModelElementInfoBuilderBase<TModelElementInfo, TRuntimeElement, TBuilder> : RuntimeNamedElementInfoBuilderBase<TModelElementInfo, TRuntimeElement, TBuilder>
        where TModelElementInfo : RuntimeModelElementInfo<TRuntimeElement>
        where TRuntimeElement : MemberInfo
        where TBuilder : RuntimeModelElementInfoBuilderBase<TModelElementInfo, TRuntimeElement, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RuntimeModelElementInfoBuilderBase{TModelElementInfo,TRuntimeElement,TBuilder}"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        protected RuntimeModelElementInfoBuilderBase(TRuntimeElement runtimeElement)
            : base(runtimeElement)
        {
        }
    }
}