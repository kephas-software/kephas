// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract builder for runtime classifier information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction.Builders
{
    using System.Collections.Generic;

    using Kephas.Model.Dimensions.AppLayer;
    using Kephas.Model.Dimensions.Aspect;
    using Kephas.Model.Dimensions.Model;
    using Kephas.Model.Dimensions.Module;
    using Kephas.Model.Dimensions.Scope;
    using Kephas.Reflection;

    /// <summary>
    /// Base abstract builder for runtime classifier information.
    /// </summary>
    /// <typeparam name="TClassifier">Type of the classifier information.</typeparam>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    public abstract class ClassifierBuilderBase<TClassifier, TBuilder> : ModelElementBuilderBase<TClassifier, ITypeInfo, TBuilder>
        where TClassifier : ClassifierBase<TClassifier, ITypeInfo>
        where TBuilder : ClassifierBuilderBase<TClassifier, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierBuilderBase{TClassifier,TBuilder}"/>
        /// class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        protected ClassifierBuilderBase(IModelSpace modelSpace, ITypeInfo runtimeElement)
            : base(modelSpace, runtimeElement)
        {
            // TODO
            // this.NamedElement.Projection = new List<object>();
        }

        /// <summary>
        /// Places the element information in the specified projection.
        /// </summary>
        /// <typeparam name="TModelDimensionElement">Type of the model dimension element.</typeparam>
        /// <returns>
        /// This builder.
        /// </returns>
        public TBuilder InProjection<TModelDimensionElement>()
        {
            ((List<object>)this.NamedElement.Projection).Add(typeof(TModelDimensionElement));

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
            return
                this.InProjection<IKephasAppLayerDimensionElement>()
                    .InProjection<IPrimitivesModelDimensionElement>()
                    .InProjection<ICoreModuleDimensionElement>()
                    .InProjection<IGlobalScopeDimensionElement>()
                    .InProjection<IMainAspectDimensionElement>();
        }
    }
}