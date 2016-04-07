// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelProjectionProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IRuntimeModelProjectionProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using Kephas.Dynamic;
    using Kephas.Model.Construction;

    /// <summary>
    /// Provides the model projection for the <see cref="IDynamicTypeInfo"/>.
    /// </summary>
    public interface IRuntimeModelProjectionProvider
    {
        /// <summary>
        /// Gets the model projection for the provided <see cref="IDynamicTypeInfo"/>.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// The model projection.
        /// </returns>
        IModelProjection GetModelProjection(IModelConstructionContext constructionContext, IDynamicTypeInfo typeInfo);
    }
}
