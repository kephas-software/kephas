// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Factory class for runtime property information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Factory class for runtime property information.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class PropertyConstructor : ModelElementConstructorBase<Property, IProperty, IRuntimePropertyInfo>
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
        protected override Property TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimePropertyInfo runtimeElement)
        {
            var property = new Property(constructionContext, this.TryComputeNameCore(runtimeElement))
                               {
                                   CanRead = runtimeElement.CanRead,
                                   CanWrite = runtimeElement.CanWrite
                               };
            return property;
        }
    }
}