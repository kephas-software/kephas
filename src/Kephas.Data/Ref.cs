// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ref.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the reference base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    using IdType = Kephas.Data.Id;

    /// <summary>
    /// Implementation of an entity reference.
    /// </summary>
    /// <typeparam name="T">The referenced entity type.</typeparam>
    public abstract class Ref<T> : IRef<T>
        where T : class
    {
        /// <summary>
        /// The entity information provider.
        /// </summary>
        private readonly WeakReference<IEntityInfoAware> entityRef;

        /// <summary>
        /// Name of the reference identifier.
        /// </summary>
        private readonly string refIdName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ref{T}"/> class.
        /// </summary>
        /// <param name="entityInfoAware">The entity information provider.</param>
        /// <param name="refIdName">Name of the reference identifier.</param>
        protected Ref(IEntityInfoAware entityInfoAware, string refIdName)
        {
            Requires.NotNull(entityInfoAware, nameof(entityInfoAware));
            Requires.NotNullOrEmpty(refIdName, nameof(refIdName));

            this.entityRef = new WeakReference<IEntityInfoAware>(entityInfoAware);
            this.refIdName = refIdName;
            this.EntityType = typeof(T);
        }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object IIdentifiable.Id => this.Id;

        /// <summary>
        /// Gets the type of the referenced entity.
        /// </summary>
        /// <value>
        /// The type of the referenced entity.
        /// </value>
        public Type EntityType { get; }

        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="throwIfNotFound">If true and the referenced entity is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        public async Task<T> GetAsync(bool throwIfNotFound = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IdType.IsUnset(this.Id))
            {
                return null;
            }

            var entityInfo = this.GetEntityInfo();
            var dataContext = this.GetDataContext(entityInfo);

            var findContext = new FindContext<T>(dataContext, this.Id, throwIfNotFound);
            var refEntity = await dataContext.FindAsync<T>(findContext, cancellationToken).PreserveThreadContext();
            return refEntity;
        }

        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="throwIfNotFound">If true and the referenced entity is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        async Task<object> IRef.GetAsync(bool throwIfNotFound, CancellationToken cancellationToken)
        {
            return await this.GetAsync(throwIfNotFound, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Gets or sets the identifier of the referenced entity.
        /// </summary>
        /// <value>
        /// The identifier of the referenced entity.
        /// </value>
        public object Id
        {
            get => this.GetEntityInfo().Entity.GetPropertyValue(this.refIdName);
            set => this.GetEntityInfo().Entity.SetPropertyValue(this.refIdName, value);
        }

        /// <summary>
        /// Gets entity information.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when a supplied object has been disposed.</exception>
        /// <returns>
        /// The entity information.
        /// </returns>
        protected virtual IEntityInfo GetEntityInfo()
        {
            IEntityInfoAware entity;
            if (!this.entityRef.TryGetTarget(out entity))
            {
                // TODO localization
                throw new ObjectDisposedException(this.GetType().Name, "The entity has been disposed.");
            }

            return entity.GetEntityInfo();
        }

        /// <summary>
        /// Gets the data context for entity retrieval.
        /// </summary>
        /// <param name="entityInfo">Information describing the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        protected virtual IDataContext GetDataContext(IEntityInfo entityInfo)
        {
            var dataContext = entityInfo?.DataContext;
            if (dataContext == null)
            {
                // TODO localization
                throw new InvalidOperationException("Cannot retrieve a data context object.");
            }

            return dataContext;
        }
    }
}