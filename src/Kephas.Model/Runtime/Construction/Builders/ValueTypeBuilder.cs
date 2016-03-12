// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTypeBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Builder for runtime value type information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Builders
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Reflection;

    using ValueType = Kephas.Model.Elements.ValueType;

    /// <summary>
    /// Builder for runtime value type information.
    /// </summary>
    public class ValueTypeBuilder : ClassifierBuilderBase<ValueType, IValueType, ValueTypeBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeBuilder"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        public ValueTypeBuilder(IModelConstructionContext constructionContext, Type runtimeElement)
            : this(constructionContext, runtimeElement.AsDynamicTypeInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeBuilder"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        public ValueTypeBuilder(IModelConstructionContext constructionContext, IDynamicTypeInfo runtimeElement)
            : base(constructionContext, runtimeElement)
        {
        }

        /// <summary>
        /// Marks the value type as being primitive.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public ValueTypeBuilder AsPrimitive()
        {
            this.Element.IsPrimitive = true;

            return this;
        }

        /// <summary>
        /// Marks the value type as being complex.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public ValueTypeBuilder AsComplex()
        {
            this.Element.IsPrimitive = false;

            return this;
        }

        /// <summary>
        /// Creates the element information out of the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new instance of <see typeparamref="TNamedElement"/>.
        /// </returns>
        protected override IRuntimeModelElementConstructor CreateElementConstructor(
            IModelConstructionContext constructionContext,
            IDynamicTypeInfo runtimeElement)
        {
            return new ValueTypeConstructor(forceValueType: true);
        }
    }
}