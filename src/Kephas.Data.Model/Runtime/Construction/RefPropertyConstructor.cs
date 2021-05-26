// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefPropertyConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Runtime.Construction
{
    using Kephas.Data.Model.Elements;
    using Kephas.Data.Reflection;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Constructor for reference properties.
    /// </summary>
    [ProcessingPriority(Priority.BelowNormal)]
    public class RefPropertyConstructor : PropertyConstructor
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
        protected override Property? TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimePropertyInfo runtimeElement)
        {
            if (runtimeElement is not IRefPropertyInfo)
            {
                return null;
            }

            var property = new RefProperty(constructionContext, this.TryComputeNameCore(runtimeElement, constructionContext)!)
            {
                CanRead = runtimeElement.CanRead,
                CanWrite = runtimeElement.CanWrite,
            };
            return property;
        }
    }
}