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
    using System.Linq;

    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Dimensions.App;
    using Kephas.Model.Dimensions.AppFamily;
    using Kephas.Model.Dimensions.Aspect;
    using Kephas.Model.Dimensions.Module;
    using Kephas.Model.Dimensions.Scope;
    using Kephas.Model.Elements;
    using Kephas.Model.Elements.Annotations;
    using Kephas.Model.Runtime.AttributedModel;
    using Kephas.Model.Runtime.Construction.Annotations;

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
                            .Dim<IKephasAppFamilyDimensionElement>()
                            .Dim<IKernelAppDimensionElement>()
                            .Dim<ICoreModuleDimensionElement>()
                            .Dim<IGlobalScopeDimensionElement>()
                            .Dim<IDefaultAspectDimensionElement>());
        }

        /// <summary>
        /// Marks the classifier as mixin.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public TBuilder AsMixin()
        {
            if (!this.Element.Members.OfType<MixinAnnotation>().Any())
            {
                var annotation = MixinAnnotationConstructor.Instance.TryCreateModelElement(this.ConstructionContext, new MixinAttribute());
                if (annotation != null)
                {
                    (this.Element as IWritableNamedElement)?.AddMember(annotation);
                }
            }

            return (TBuilder)this;
        }
    }
}