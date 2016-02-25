// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Factory class for runtime property information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Dynamic;
    using Kephas.Model.Elements;
    using Kephas.Model.Factory;

    /// <summary>
    /// Factory class for runtime property information.
    /// </summary>
    public class PropertyConstructor : ModelElementConstructorBase<Property, IProperty, IDynamicPropertyInfo>
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
        protected override Property TryCreateModelElementCore(IModelConstructionContext constructionContext, IDynamicPropertyInfo runtimeElement)
        {
            var property = new Property(constructionContext, this.ComputeName(runtimeElement));
            return property;
        }
    }
}