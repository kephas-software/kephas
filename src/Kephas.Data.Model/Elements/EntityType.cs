// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity type class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Data.Model.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements;

    /// <summary>
    /// Classifier for entities, either persisted or used by the client tier.
    /// </summary>
    public class EntityType : ClassifierBase<IEntityType>, IEntityType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityType" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public EntityType(IModelConstructionContext constructionContext, string name)
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