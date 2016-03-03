// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract builder for runtime classifier information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Builders
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Dimensions.AppLayer;
    using Kephas.Model.Dimensions.Aspect;
    using Kephas.Model.Dimensions.Model;
    using Kephas.Model.Dimensions.Module;
    using Kephas.Model.Dimensions.Scope;
    using Kephas.Model.Elements;

    /// <summary>
    /// Base abstract builder for runtime classifier information.
    /// </summary>
    /// <typeparam name="TModel">Type of the model.</typeparam>
    /// <typeparam name="TModelContract">Type of the model contract.</typeparam>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    public abstract class ClassifierBuilderBase<TModel, TModelContract, TBuilder> : ModelElementBuilderBase<TModel, TModelContract, IDynamicTypeInfo, TBuilder>
        where TModel : ClassifierBase<TModelContract>
        where TModelContract : IClassifier
        where TBuilder : ClassifierBuilderBase<TModel, TModelContract, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierBuilderBase{TModel, TModelContract, TBuilder}"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        protected ClassifierBuilderBase(IModelConstructionContext constructionContext, IDynamicTypeInfo runtimeElement)
            : base(constructionContext, runtimeElement)
        {
        }

        /// <summary>
        /// Places the element information in the specified projection.
        /// </summary>
        /// <param name="projectionBuilder">The projection builder.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public TBuilder InProjection(Action<RuntimeProjectionBuilder> projectionBuilder)
        {
            if (projectionBuilder != null)
            {
                var projBuilder = new RuntimeProjectionBuilder();
                projectionBuilder(projBuilder);
                this.Element.SetRuntimeProjection(projBuilder.Projection);
            }

            return (TBuilder)this;
        }

        /// <summary>
        /// Places the element information in the core projection.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public TBuilder InCoreProjection()
        {
            return this.InProjection(b => b
                            .Dim<IKephasAppLayerDimensionElement>()
                            .Dim<IPrimitivesModelDimensionElement>()
                            .Dim<ICoreModuleDimensionElement>()
                            .Dim<IGlobalScopeDimensionElement>()
                            .Dim<IMainAspectDimensionElement>());
        }
    }
}