// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTypeConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the value type constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
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
        /// Determines whether a model element can be created for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// <c>true</c>true if a model element can be created, <c>false</c> if not.
        /// </returns>
        protected override bool CanCreateModelElement(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            if (this.ForceValueType)
            {
                return true;
            }

            var underlyingType = runtimeElement.TypeInfo;
            return underlyingType.IsValueType || base.CanCreateModelElement(constructionContext, runtimeElement);
        }

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
            return new ValueType(constructionContext, this.TryComputeName(constructionContext, runtimeElement));
        }
    }
}