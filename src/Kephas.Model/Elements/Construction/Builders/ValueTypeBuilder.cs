// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTypeBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Builder for runtime value type information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction.Builders
{
    using System;

    using Kephas.Reflection;

    using ValueType = Kephas.Model.Elements.ValueType;

    /// <summary>
    /// Builder for runtime value type information.
    /// </summary>
    public class ValueTypeBuilder : ClassifierBuilderBase<ValueType, ValueTypeBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeBuilder"/> class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        public ValueTypeBuilder(IModelSpace modelSpace, Type runtimeElement)
            : this(modelSpace, runtimeElement.AsDynamicTypeInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeBuilder"/> class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        public ValueTypeBuilder(IModelSpace modelSpace, ITypeInfo runtimeElement)
            : base(modelSpace, runtimeElement)
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
            this.NamedElement.IsPrimitive = true;

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
            this.NamedElement.IsPrimitive = false;

            return this;
        }

        /// <summary>
        /// Creates the element information out of the provided runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <param name="modelSpace">The model space.</param>
        /// <returns>
        /// A new instance of <see cref="ValueType"/>.
        /// </returns>
        protected override ValueType CreateNamedElement(ITypeInfo runtimeElement, IModelSpace modelSpace)
        {
            return new ValueType(runtimeElement, modelSpace);
        }
    }
}