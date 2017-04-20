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
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Provides extended information about the entity.
    /// </summary>
    public class EntityInfo : Expando, IEntityInfo
    {
        /// <summary>
        /// The change state tracker.
        /// </summary>
        private readonly IChangeStateTrackable changeStateTracker;

        /// <summary>
        /// The change state.
        /// </summary>
        private ChangeState changeState;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInfo"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="changeState">The entity's change state.</param>
        public EntityInfo(object entity, ChangeState changeState = ChangeState.NotChanged)
        {
            Requires.NotNull(entity, nameof(entity));

            this.Entity = entity;
            this.changeState = changeState;
            this.Id = new Id(Guid.NewGuid());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInfo"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="changeStateTracker">The entity's change state tracker.</param>
        public EntityInfo(object entity, IChangeStateTrackable changeStateTracker)
        {
            Requires.NotNull(entity, nameof(entity));
            Requires.NotNull(changeStateTracker, nameof(changeStateTracker));

            this.Entity = entity;
            this.changeStateTracker = changeStateTracker;
            this.Id = new Id(Guid.NewGuid());
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
        public ChangeState ChangeState
        {
            get => this.changeStateTracker?.ChangeState ?? this.changeState;
            set
            {
                if (this.changeStateTracker != null)
                {
                    this.changeStateTracker.ChangeState = value;
                }
                else
                {
                    this.changeState = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Id Id { get; protected set; }
    }
}