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
    using Kephas.Dynamic;
    using Kephas.Model.Elements;
    using Kephas.Model.Factory;

    /// <summary>
    /// A value type constructor.
    /// </summary>
    public class ValueTypeConstructor : ClassifierConstructorBase<ValueType, IValueType>
    {
        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override ValueType TryCreateModelElementCore(IModelConstructionContext constructionContext, IDynamicTypeInfo runtimeElement)
        {
            return new ValueType(constructionContext, this.TryComputeName(constructionContext, runtimeElement));
        }
    }
}