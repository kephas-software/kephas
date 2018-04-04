// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements
{
    using System.Collections.Generic;
    using System.Linq;

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
        }

        /// <summary>
        /// Gets the entity keys.
        /// </summary>
        /// <value>
        /// The entity keys.
        /// </value>
        public IEnumerable<IKey> Keys => this.Members.OfType<IKey>();
    }
}