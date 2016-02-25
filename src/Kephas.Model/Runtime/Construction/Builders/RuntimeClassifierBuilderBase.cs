// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeClassifierBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract builder for runtime classifier information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Builders
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Model.Dimensions.AppLayer;
    using Kephas.Model.Dimensions.Aspect;
    using Kephas.Model.Dimensions.Model;
    using Kephas.Model.Dimensions.Module;
    using Kephas.Model.Dimensions.Scope;
    using Kephas.Model.Runtime.Factory;
    using Kephas.Reflection;

    /// <summary>
    /// Base abstract builder for runtime classifier information.
    /// </summary>
    /// <typeparam name="TClassifierInfo">Type of the classifier information.</typeparam>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    public abstract class RuntimeClassifierBuilderBase<TClassifierInfo, TBuilder> : RuntimeModelElementBuilderBase<TClassifierInfo, ITypeInfo, TBuilder>
        where TClassifierInfo : RuntimeClassifierInfo
        where TBuilder : RuntimeClassifierBuilderBase<TClassifierInfo, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RuntimeClassifierBuilderBase{TClassifierInfo,TBuilder}"/> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        protected RuntimeClassifierBuilderBase(IRuntimeModelElementFactory runtimeModelElementFactory, TypeInfo runtimeElement)
            : base(runtimeModelElementFactory, runtimeElement)
        {
            this.Element.Projection = new List<object>();
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
            ((List<object>)this.Element.Projection).Add(typeof(TModelDimensionElement));

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