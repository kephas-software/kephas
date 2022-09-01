// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityEntry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity entry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Resources;
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Provides extended information about the entity.
    /// </summary>
    public class EntityEntry : Expando, IEntityEntry
    {
        /// <summary>
        /// The changed properties.
        /// </summary>
        private readonly ISet<string> changedProperties = new HashSet<string>();

        /// <summary>
        /// The change state.
        /// </summary>
        private ChangeState changeState;

        /// <summary>
        /// The expando entity.
        /// </summary>
        private IDynamic? expandoEntity;

        /// <summary>
        /// The original entity.
        /// </summary>
        private IDynamic? originalEntity;

        /// <summary>
        /// Context for the data.
        /// </summary>
        private WeakReference<IDataContext>? dataContextRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEntry"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityEntry(object entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            this.Entity = entity;
            this.Id = Guid.NewGuid();

            // ReSharper disable once VirtualMemberCallInConstructor
            this.AttachPropertyChangeHandlers();
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public object Entity { get; }

        /// <summary>
        /// Gets a copy of the original entity, before any changes occurred.
        /// </summary>
        /// <value>
        /// The original entity.
        /// </value>
        public IDynamic OriginalEntity => this.originalEntity ??= this.CreateOriginalEntity();

        /// <summary>
        /// Gets the identifier of the entity.
        /// </summary>
        /// <value>
        /// The identifier of the entity.
        /// </value>
        public object? EntityId => this.TryGetEntityId();

        /// <summary>
        /// Gets or sets the entity owning data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        public IDataContext DataContext
        {
            get
            {
                var dataContext = this.TryGetDataContext();
                if (dataContext == null)
                {
                    throw new ObjectDisposedException("The entity entry is disposed.");
                }

                return dataContext;
            }

            set
            {
                if (this.dataContextRef != null)
                {
                    throw new InvalidOperationException(Strings.EntityEntry_DataContextAlreadySet_Exception);
                }

                this.dataContextRef = new WeakReference<IDataContext>(value);
            }
        }

        /// <summary>
        /// Gets or sets the change state of the entity before persisting to the data store.
        /// </summary>
        /// <remarks>
        /// This value is typically used in the post processing part of the persist behavior
        /// (<see cref="IOnPersistBehavior.AfterPersistAsync"/>) to perform specific tasks 
        /// depending on the value of <see cref="ChangeState"/> before  persisting to the data store.
        /// Outside this behavior this value is not reliable, as the behaviors may trigger multiple persist commands
        /// for an entity and this state is typically the value before the last persist command.
        /// </remarks>
        /// <value>
        /// The change state.
        /// </value>
        public ChangeState PrePersistChangeState { get; set; }

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
                    if (tracker.ChangeState == value)
                    {
                        return;
                    }

                    tracker.ChangeState = value;
                }
                else
                {
                    if (this.changeState == value)
                    {
                        return;
                    }

                    this.changeState = value;
                }

                // Ensure the original entity is created.
                // ReSharper disable once StyleCop.SA1309
                if (value != ChangeState.NotChanged)
                {
                    var _ = this.OriginalEntity;
                }
            }
        }

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public object Id { get; protected set; }

        /// <summary>
        /// Gets a wrapper expando object over the entity, to access dynamic values from it.
        /// </summary>
        protected IDynamic ExpandoEntity => this.expandoEntity ??= this.Entity.ToExpando();

        /// <summary>
        /// Gets the root of the entity graph.
        /// </summary>
        /// <returns>
        /// The graph root.
        /// </returns>
        public IAggregatable? GetGraphRoot()
        {
            var entityGraph = this.TryGetEntityGraph();
            return entityGraph?.GetGraphRoot();
        }

        /// <summary>
        /// Gets the flattened structural entity graph excluding the loose parts (only the internal structure).
        /// </summary>
        /// <returns>
        /// The flattened structural entity graph.
        /// </returns>
        public IEnumerable<object> GetStructuralEntityGraph()
        {
            var entityGraph = this.TryGetEntityGraph();
            if (entityGraph == null)
            {
                return new[] { this.Entity };
            }

            return entityGraph.GetStructuralEntityGraph();
        }

        /// <summary>
        /// Gets the flattened entity graph asynchronously.
        /// This may include also loose parts which are asynchronously loaded.
        /// If no loose parts must be loaded, then the result is the same with <see cref="GetStructuralEntityGraph"/>
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the flattened entity graph.
        /// </returns>
        public Task<IEnumerable<object>> GetFlattenedEntityGraphAsync(
            IGraphOperationContext operationContext,
            CancellationToken cancellationToken = default)
        {
            var entityGraph = this.TryGetEntityGraph();
            if (entityGraph == null)
            {
                return Task.FromResult<IEnumerable<object>>(new[] { this.Entity });
            }

            return entityGraph.GetFlattenedEntityGraphAsync(operationContext, cancellationToken);
        }

        /// <summary>
        /// Gets a value indicating whether the provided property changed.
        /// </summary>
        /// <param name="property">The property name.</param>
        /// <returns>
        /// True if the property changed, false if not.
        /// </returns>
        public virtual bool IsChanged(string property)
        {
            if (this.ChangeState != ChangeState.Changed)
            {
                return false;
            }

            var originalValue = this.OriginalEntity[property];
            return !object.Equals(this.ExpandoEntity[property], originalValue);
        }

        /// <summary>
        /// Accepts the changes and resets the change state to <see cref="Capabilities.ChangeState.NotChanged"/>.
        /// </summary>
        public virtual void AcceptChanges()
        {
            this.ResetChangeState();
        }

        /// <summary>
        /// Discards the changes and resets the change state to <see cref="Capabilities.ChangeState.NotChanged"/>.
        /// </summary>
        public virtual void DiscardChanges()
        {
            try
            {
                this.DetachPropertyChangeHandlers();

                // undo the values in the entities, only if some changes occurred.
                if (this.originalEntity != null)
                {
                    foreach (var changedProperty in this.changedProperties)
                    {
                        this.ExpandoEntity[changedProperty] = this.OriginalEntity[changedProperty];
                    }
                }

                this.ResetChangeState();
            }
            finally
            {
                this.AttachPropertyChangeHandlers();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.ChangeState} {this.Entity}";
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="EntityEntry"/> and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.DetachPropertyChangeHandlers();
        }

        /// <summary>
        /// Gets the entity identifier.
        /// </summary>
        /// <returns>
        /// The entity identifier.
        /// </returns>
        protected virtual object? TryGetEntityId()
        {
            // first of all get the ID from an Identifiable interface
            if (this.Entity is IIdentifiable identifiable)
            {
                return identifiable.Id;
            }

            // then try to access the ID dynamically.
            return this.ExpandoEntity[nameof(IIdentifiable.Id)];
        }

        /// <summary>
        /// Tries to get the data context that created this entry.
        /// </summary>
        /// <returns>The data context or <c>null</c>.</returns>
        protected virtual IDataContext? TryGetDataContext()
        {
            IDataContext? dataContext = null;
            this.dataContextRef?.TryGetTarget(out dataContext);
            return dataContext;
        }

        /// <summary>
        /// Gets the change state tracker.
        /// </summary>
        /// <returns>
        /// The change state tracker.
        /// </returns>
        protected virtual IChangeStateTrackable? TryGetChangeStateTracker()
        {
            return this.Entity as IChangeStateTrackable;
        }

        /// <summary>
        /// Gets the entity graph.
        /// </summary>
        /// <returns>
        /// The entity graph.
        /// </returns>
        protected virtual IAggregatable? TryGetEntityGraph()
        {
            return this.Entity as IAggregatable;
        }

        /// <summary>
        /// Gets the <see cref="INotifyPropertyChanging"/> capability.
        /// </summary>
        /// <returns>
        /// An object implementing INotifyPropertyChanging, or <c>null</c>.
        /// </returns>
        protected virtual INotifyPropertyChanging? TryGetNotifyPropertyChanging()
        {
            return this.Entity as INotifyPropertyChanging;
        }

        /// <summary>
        /// Gets the <see cref="INotifyPropertyChanged"/> capability.
        /// </summary>
        /// <returns>
        /// An object implementing INotifyPropertyChanged, or <c>null</c>.
        /// </returns>
        protected virtual INotifyPropertyChanged? TryGetNotifyPropertyChanged()
        {
            return this.Entity as INotifyPropertyChanged;
        }

        /// <summary>
        /// Creates the original entity as a stamp of the current entity.
        /// </summary>
        /// <returns>
        /// The new original entity.
        /// </returns>
        protected virtual IDynamic CreateOriginalEntity()
        {
            var typeInfo = this.Entity.GetTypeInfo();
            var originalValues = new Dictionary<string, object?>();
            foreach (var prop in typeInfo.Properties)
            {
                originalValues.Add(prop.Name, prop.GetValue(this.Entity));
            }

            var original = originalValues.ToExpando();
            return original;
        }

        /// <summary>
        /// Attaches the property change handlers.
        /// </summary>
        protected virtual void AttachPropertyChangeHandlers()
        {
            var propertyChanging = this.TryGetNotifyPropertyChanging();
            if (propertyChanging != null)
            {
                propertyChanging.PropertyChanging += this.EnsureOriginalEntity;
            }

            var propertyChanged = this.TryGetNotifyPropertyChanged();
            if (propertyChanged != null)
            {
                propertyChanged.PropertyChanged += this.EnsureTrackChanges;
            }
        }

        /// <summary>
        /// Detaches the property change handlers.
        /// </summary>
        protected virtual void DetachPropertyChangeHandlers()
        {
            var propertyChanging = this.TryGetNotifyPropertyChanging();
            if (propertyChanging != null)
            {
                propertyChanging.PropertyChanging -= this.EnsureOriginalEntity;
            }

            var propertyChanged = this.TryGetNotifyPropertyChanged();
            if (propertyChanged != null)
            {
                propertyChanged.PropertyChanged -= this.EnsureTrackChanges;
            }
        }

        /// <summary>
        /// Ensures that the original entity is created.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Property changing event information.</param>
        private void EnsureOriginalEntity(object sender, PropertyChangingEventArgs e)
        {
            // force the creation of the original entity, if not already done.
            var _ = this.OriginalEntity;
        }

        /// <summary>
        /// Ensures that the changes are tracked.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Property changed event information.</param>
        private void EnsureTrackChanges(object sender, PropertyChangedEventArgs e)
        {
            this.changedProperties.Add(e.PropertyName);
            if (this.ChangeState == ChangeState.NotChanged)
            {
                this.ChangeState = ChangeState.Changed;
            }
        }

        /// <summary>
        /// Resets the change state.
        /// </summary>
        private void ResetChangeState()
        {
            this.originalEntity = null;
            this.changedProperties.Clear();
            this.ChangeState = ChangeState.NotChanged;
        }
    }
}