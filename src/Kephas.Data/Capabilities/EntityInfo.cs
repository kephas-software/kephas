// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the entity information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Provides extended information about the entity.
    /// </summary>
    public class EntityInfo : Expando, IEntityInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInfo"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="changeState">The entity's change state.</param>
        public EntityInfo(object entity, ChangeState changeState = ChangeState.NotChanged)
        {
            Requires.NotNull(entity, nameof(entity));

            this.Entity = entity;
            this.ChangeState = changeState;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public object Entity { get; }

        /// <summary>
        /// Gets or sets the change state of the entity.
        /// </summary>
        /// <value>
        /// The change state.
        /// </value>
        public ChangeState ChangeState { get; set; }
    }
}