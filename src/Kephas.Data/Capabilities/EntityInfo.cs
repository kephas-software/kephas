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
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Provides extended information about the entity.
    /// </summary>
    public class EntityInfo : Expando, IEntityInfo
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
        private IExpando expandoEntity;

        /// <summary>
        /// The original entity.
        /// </summary>
        private IExpando originalEntity;

        /// <summary>
        /// Context for the data.
        /// </summary>
        private WeakReference<IDataContext> dataContextRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInfo"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityInfo(object entity)
        {
            Requires.NotNull(entity, nameof(entity));

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
        /// Gets a copy of the original entity, before any changes occured.
        /// </summary>
        /// <value>
        /// The original entity.
        /// </value>
        public IExpando OriginalEntity => this.originalEntity ?? (this.originalEntity = this.CreateOriginalEntity());

        /// <summary>
        /// Gets the identifier of the entity.
        /// </summary>
        /// <value>
        /// The identifier of the entity.
        /// </value>
        public object EntityId => this.TryGetEntityId() ?? this.Id;

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
                IDataContext dataContext = null;
                this.dataContextRef?.TryGetTarget(out dataContext);
                return dataContext;
            }
            set
            {
                if (this.dataContextRef != null)
                {
                    throw new InvalidOperationException(Strings.EntityInfo_DataContextAlreadySet_Exception);
                }

                this.dataContextRef = new WeakReference<IDataContext>(value);
            }
        }

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
        protected IExpando ExpandoEntity => this.expandoEntity ?? (this.expandoEntity = this.Entity as IExpando ?? new Expando(this.Entity));

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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var entityGraph = this.TryGetEntityGraph();
            if (entityGraph == null)
            {
                return Task.FromResult<IEnumerable<object>>(new[] { this.Entity });
            }

            return entityGraph.GetFlattenedEntityGraphAsync(operationContext, cancellationToken);
        }

        /// <summary>
        /// Accepts the changes and resets the change state to <see cref="Capabilities.ChangeState.NotChanged"/>.
        /// </summary>
        public void AcceptChanges()
        {
            this.ResetChangeState();
        }

        /// <summary>
        /// Discards the changes and resets the change state to <see cref="Capabilities.ChangeState.NotChanged"/>.
        /// </summary>
        public void DiscardChanges()
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
        /// Releases the unmanaged resources used by the <see cref="EntityInfo"/> and
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
        protected virtual object TryGetEntityId()
        {
            // first of all get the ID from an Identifiable interface
            var identifiable = this.Entity as IIdentifiable;
            if (identifiable != null)
            {
                return identifiable.Id;
            }

            // then try to access the ID dynamically.
            return this.ExpandoEntity[nameof(IIdentifiable.Id)];
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

        /// <summary>
        /// Gets the <see cref="INotifyPropertyChanging"/> capability.
        /// </summary>
        /// <returns>
        /// An object implementing INotifyPropertyChanging, or <c>null</c>.
        /// </returns>
        protected virtual INotifyPropertyChanging TryGetNotifyPropertyChanging()
        {
            return this.Entity as INotifyPropertyChanging;
        }

        /// <summary>
        /// Gets the <see cref="INotifyPropertyChanged"/> capability.
        /// </summary>
        /// <returns>
        /// An object implementing INotifyPropertyChanged, or <c>null</c>.
        /// </returns>
        protected virtual INotifyPropertyChanged TryGetNotifyPropertyChanged()
        {
            return this.Entity as INotifyPropertyChanged;
        }

        /// <summary>
        /// Creates the original entity as a stamp of the current entity.
        /// </summary>
        /// <returns>
        /// The new original entity.
        /// </returns>
        protected virtual IExpando CreateOriginalEntity()
        {
            var typeInfo = this.Entity.GetTypeInfo();
            var originalValues = new Dictionary<string, object>();
            foreach (var prop in typeInfo.Properties)
            {
                originalValues.Add(prop.Name, prop.GetValue(this.Entity));
            }

            var original = new Expando(originalValues);
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