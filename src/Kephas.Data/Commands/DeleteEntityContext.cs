﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteEntityContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the delete entity context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A delete entity context.
    /// </summary>
    public class DeleteEntityContext : DataOperationContext, IDeleteEntityContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteEntityContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entity">The entity to delete.</param>
        public DeleteEntityContext(IDataContext dataContext, object entity)
            : base(dataContext)
        {
            Requires.NotNull(entity, nameof(entity));

            this.Entities = new[] { entity };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteEntityContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entities">The entities to delete.</param>
        public DeleteEntityContext(IDataContext dataContext, IEnumerable<object> entities)
            : base(dataContext)
        {
            Requires.NotNull(entities, nameof(entities));

            this.Entities = entities;
        }

        /// <summary>
        /// Gets the entity to delete.
        /// </summary>
        /// <value>
        /// The entity to delete.
        /// </value>
        public IEnumerable<object> Entities { get; }
    }
}