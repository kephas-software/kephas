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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Provides extended information about the entity.
    /// </summary>
    public class EntityInfo : Expando, IEntityInfo
    {
        /// <summary>
        /// The change state.
        /// </summary>
        private ChangeState changeState;

        /// <summary>
        /// The dynamic entity.
        /// </summary>
        private IIndexable dynamicEntity;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInfo"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityInfo(object entity)
        {
            Requires.NotNull(entity, nameof(entity));

            this.Entity = entity;
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
        /// Gets the identifier of the entity.
        /// </summary>
        /// <value>
        /// The identifier of the entity.
        /// </value>
        public Id EntityId => this.TryGetEntityId() ?? this.Id;

        /// <summary>
        /// Gets or sets the change state of the entity.
        /// </summary>
        /// <value>
        /// The change state.
        /// </value>
        public ChangeState ChangeState
        {
            get
            {
                var tracker = this.TryGetChangeStateTracker();
                return tracker?.ChangeState ?? this.changeState;
            }

            set
            {
                var tracker = this.TryGetChangeStateTracker();
                if (tracker != null)
                {
                    tracker.ChangeState = value;
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

        /// <summary>
        /// Gets a wrapper expando object over the entity, to access dynamic values from it.
        /// </summary>
        protected IIndexable ExpandoEntity => this.dynamicEntity ?? (this.dynamicEntity = this.Entity as IIndexable ?? new Expando(this.Entity));

        /// <summary>
        /// Gets the root of the entity graph.
        /// </summary>
        /// <returns>
        /// The graph root.
        /// </returns>
        public IAggregatable GetGraphRoot()
        {
            var entityGraph = this.TryGetEntityGraph();
            return entityGraph?.GetGraphRoot();
        }

        /// <summary>
        /// Gets the flattened graph asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The flattened graph asynchronous.
        /// </returns>
        public Task<IEnumerable<object>> GetFlattenedGraphAsync(
            IGraphOperationContext operationContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var entityGraph = this.TryGetEntityGraph();
            if (entityGraph == null)
            {
                return Task.FromResult<IEnumerable<object>>(new[] { this.Entity });
            }

            return entityGraph.GetFlattenedGraphAsync(operationContext, cancellationToken);
        }

        /// <summary>
        /// Gets the entity identifier.
        /// </summary>
        /// <returns>
        /// The entity identifier.
        /// </returns>
        protected virtual Id TryGetEntityId()
        {
            // first of all get the ID from an Identifiable interface
            var identifiable = this.Entity as IIdentifiable;
            if (identifiable != null)
            {
                return identifiable.Id;
            }

            // then try to access the ID dynamically.
            var id = this.ExpandoEntity[nameof(IIdentifiable.Id)];
            if (id != null)
            {
                return id as Id ?? new Id(id);
            }

            return null;
        }

        /// <summary>
        /// Gets the change state tracker.
        /// </summary>
        /// <returns>
        /// The change state tracker.
        /// </returns>
        protected virtual IChangeStateTrackable TryGetChangeStateTracker()
        {
            return this.Entity as IChangeStateTrackable;
        }

        /// <summary>
        /// Gets the entity graph.
        /// </summary>
        /// <returns>
        /// The entity graph.
        /// </returns>
        protected virtual IAggregatable TryGetEntityGraph()
        {
            return this.Entity as IAggregatable;
        }
    }
}