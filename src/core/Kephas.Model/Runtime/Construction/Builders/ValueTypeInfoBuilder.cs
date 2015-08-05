// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTypeInfoBuilder.cs" company="Quartz Software SRL">
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

    /// <summary>
    /// Builder for runtime value type information.
    /// </summary>
    public class ValueTypeInfoBuilder : RuntimeClassifierInfoBuilderBase<RuntimeValueTypeInfo, ValueTypeInfoBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeInfoBuilder"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        public ValueTypeInfoBuilder(Type runtimeElement)
            : this(runtimeElement.GetTypeInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeInfoBuilder"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        public ValueTypeInfoBuilder(TypeInfo runtimeElement)
            : base(runtimeElement)
        {
        }

        /// <summary>
        /// Marks the value type as being primitive.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public ValueTypeInfoBuilder AsPrimitive()
        {
            this.ElementInfo.IsPrimitive = true;

            return this;
        }

        /// <summary>
        /// Marks the value type as being complex.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public ValueTypeInfoBuilder AsComplex()
        {
            this.ElementInfo.IsPrimitive = false;

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