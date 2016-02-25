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
    using System.Reflection;

    using Kephas.Model.Runtime.Factory;

    /// <summary>
    /// Builder for runtime value type information.
    /// </summary>
    public class ValueTypeBuilder : RuntimeClassifierBuilderBase<RuntimeValueTypeInfo, ValueTypeBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeBuilder"/> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        public ValueTypeBuilder(IRuntimeModelElementFactory runtimeModelElementFactory, Type runtimeElement)
            : this(runtimeModelElementFactory, runtimeElement.GetTypeInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeBuilder"/> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        public ValueTypeBuilder(IRuntimeModelElementFactory runtimeModelElementFactory, TypeInfo runtimeElement)
            : base(runtimeModelElementFactory, runtimeElement)
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
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new instance of <see cref="RuntimeValueTypeInfo"/>.
        /// </returns>
        protected override RuntimeValueTypeInfo CreateElementInfo(TypeInfo runtimeElement)
        {
            return new RuntimeValueTypeInfo(runtimeElement);
        }
    }
}