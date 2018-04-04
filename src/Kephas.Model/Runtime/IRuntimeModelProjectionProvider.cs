// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelProjectionProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeModelProjectionProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Runtime;

    /// <summary>
    /// Provides the model projection for the <see cref="IRuntimeTypeInfo"/>.
    /// </summary>
    public interface IRuntimeModelProjectionProvider
    {
        /// <summary>
        /// Gets the model projection for the provided <see cref="IRuntimeTypeInfo"/>.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// The model projection.
        /// </returns>
        IModelProjection GetModelProjection(IModelConstructionContext constructionContext, IRuntimeTypeInfo typeInfo);
    }
}
