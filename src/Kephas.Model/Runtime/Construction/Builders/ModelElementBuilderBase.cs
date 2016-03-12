// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelElementBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract builder for runtime model element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Builders
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Reflection;

    /// <summary>
    /// Base abstract builder for model elements.
    /// </summary>
    /// <typeparam name="TModel">Type of the model.</typeparam>
    /// <typeparam name="TModelContract">Type of the model contract.</typeparam>
    /// <typeparam name="TRuntime">Type of the runtime element.</typeparam>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    public abstract class ModelElementBuilderBase<TModel, TModelContract, TRuntime, TBuilder> : NamedElementBuilderBase<TModel, TModelContract, TRuntime, TBuilder>
        where TModel : ModelElementBase<TModelContract>
        where TModelContract : IModelElement
        where TRuntime : class, IElementInfo
        where TBuilder : ModelElementBuilderBase<TModel, TModelContract, TRuntime, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelElementBuilderBase{TModel, TModelContract, TRuntime, TBuilder}"/>
        /// class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        protected ModelElementBuilderBase(IModelConstructionContext constructionContext, TRuntime runtimeElement)
            : base(constructionContext, runtimeElement)
        {
        }
    }
}