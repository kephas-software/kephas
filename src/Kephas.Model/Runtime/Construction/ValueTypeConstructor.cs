// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTypeConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the value type constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.AttributedModel;
    using Kephas.Runtime;

    /// <summary>
    /// A value type constructor.
    /// </summary>
    public class ValueTypeConstructor : ClassifierConstructorBase<ValueType, IValueType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeConstructor"/> class.
        /// </summary>
        public ValueTypeConstructor()
            : this(forceValueType: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeConstructor"/> class.
        /// </summary>
        /// <param name="forceValueType"><c>true</c> if a value type should be forced, <c>false</c> if not.</param>
        internal ValueTypeConstructor(bool forceValueType)
        {
            this.ForceValueType = forceValueType;
        }

        /// <summary>
        /// Gets a value indicating whether the value type should be forced.
        /// </summary>
        /// <value>
        /// <c>true</c> if a value type should be forced, <c>false</c> if not.
        /// </value>
        public bool ForceValueType { get; }

        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override ValueType TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            var underlyingType = runtimeElement.TypeInfo;
            if (underlyingType.GetCustomAttribute<ValueTypeAttribute>() == null && !underlyingType.IsValueType && !this.ForceValueType)
            {
                return null;
            }

            return new ValueType(constructionContext, this.TryComputeName(constructionContext, runtimeElement));
        }
    }
}