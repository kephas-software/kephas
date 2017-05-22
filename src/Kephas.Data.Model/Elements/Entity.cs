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
    using System.Collections.Generic;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements;

    /// <summary>
    /// Classifier for entities, either persisted or used by the client tier.
    /// </summary>
    public class Entity : ClassifierBase<IEntity>, IEntity
    {
        /// <summary>
        /// The empty keys array.
        /// </summary>
        public static readonly IKey[] EmptyKeys = new IKey[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public Entity(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
            this.Keys = EmptyKeys;
        }

        /// <summary>
        /// Gets the entity keys.
        /// </summary>
        /// <value>
        /// The entity keys.
        /// </value>
        public IEnumerable<IKey> Keys { get; }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            base.OnCompleteConstruction(constructionContext);
        }
    }
}