// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the entity class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;

    /// <summary>
    /// Classifier for entities, either persisted or used by the client tier.
    /// </summary>
    public class Entity : ClassifierBase<IEntity>, IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public Entity(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }
    }
}