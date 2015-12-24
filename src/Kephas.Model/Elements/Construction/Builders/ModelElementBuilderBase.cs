// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelElementBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract builder for runtime model element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction.Builders
{
    using Kephas.Reflection;

    /// <summary>
    /// Base abstract builder for runtime model element information.
    /// </summary>
    /// <typeparam name="TModelElement">Type of the model element information.</typeparam>
    /// <typeparam name="TElementInfo">Type of the runtime element.</typeparam>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    public abstract class ModelElementBuilderBase<TModelElement, TElementInfo, TBuilder> : NamedElementBuilderBase<TModelElement, TElementInfo, TBuilder>
        where TModelElement : ModelElementBase<TModelElement, TElementInfo>
        where TElementInfo : class, IElementInfo
        where TBuilder : ModelElementBuilderBase<TModelElement, TElementInfo, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelElementBuilderBase{TModelElement,TElementInfo,TBuilder}"/> class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        protected ModelElementBuilderBase(IModelSpace modelSpace, TElementInfo runtimeElement)
            : base(modelSpace, runtimeElement)
        {
        }
    }
}