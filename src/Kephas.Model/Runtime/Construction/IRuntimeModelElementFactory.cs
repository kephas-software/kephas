// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelElementFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for runtime model information providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Model.Construction;
    using Kephas.Services;

    /// <summary>
    /// Contract for creating a model element based on the runtime element.
    /// </summary>
    [SharedAppServiceContract]
    public interface IRuntimeModelElementFactory
    {
        /// <summary>
        /// Tries to get the named element information from the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A named element information or <c>null</c>.
        /// </returns>
        INamedElement TryCreateModelElement(IModelConstructionContext constructionContext, object runtimeElement);
    }
}