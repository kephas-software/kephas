// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ref.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

    using Kephas.Data.Commands;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Implementation of an entity reference.
    /// </summary>
    /// <typeparam name="T">The referenced entity type.</typeparam>
    public class Ref<T> : RefBase, IRef<T>
        where T : class, IIdentifiable
    {
        /// <summary>
        /// The referenced entity.
        /// </summary>
        private T entity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ref{T}"/> class.
        /// </summary>
        /// <param name="containerEntity">The entity containing the reference.</param>
        /// <param name="refFieldName">Name of the reference identifier property.</param>
        public Ref(object containerEntity, string refFieldName)
            : base(containerEntity, refFieldName)
        {
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
        /// Gets or sets the referenced entity.
        /// </summary>
        /// <remarks>
        /// The entity is not ensured to be set prior to calling <see cref="IRef{T}.GetAsync"/>
        /// due to performance reasons, but the actual behavior is left to the implementor.
        /// </remarks>
        /// <value>
        /// The referenced entity.
        /// </value>
        public virtual T Entity
        {
            get => this.entity;
            set => this.entity = value;
        }

        /// <summary>
        /// Gets or sets the referenced entity.
        /// </summary>
        /// <remarks>
        /// The entity is not ensured to be set prior to calling <see cref="IRef.GetAsync"/>
        /// due to performance reasons, but the actual behavior is left to the implementor.
        /// </remarks>
        /// <value>
        /// The referenced entity.
        /// </value>
        object IRef.Entity
        {
            get => this.Entity;
            set => this.Entity = (T)value;
        }

        /// <summary>
        /// Gets a value indicating whether the reference is empty/not set.
        /// </summary>
        /// <value>
        /// True if this reference is empty, false if not.
        /// </value>
        public virtual bool IsEmpty => this.Entity == null && Kephas.Data.Id.IsEmpty(this.Id);

        /// <summary>
        /// Gets or sets the identifier of the referenced entity.
        /// </summary>
        /// <value>
        /// The identifier of the referenced entity.
        /// </value>
        public virtual object Id
        {
            get
            {
                var value = this.GetEntityPropertyValue(this.RefFieldName);
                // if the ID value is not set, but the entity,
                // try getting the ID from the referenced entity.
                if (Kephas.Data.Id.IsEmpty(value))
                {
                    var refEntity = this.Entity;
                    if (refEntity != null)
                    {
                        return refEntity.Id;
                    }
                }

                return value;
            }

            set
            {
                this.SetEntityPropertyValue(this.RefFieldName, value);

                // if the referenced entity ID is not the same with
                // the provided one, clean the reference to enable a clean get.
                if (this.entity != null && !Equals(this.entity.Id, value))
                {
                    this.entity = null;
                }
            }
        }

        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="throwIfNotFound">If true and the referenced entity is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        public virtual async Task<T> GetAsync(bool throwIfNotFound = true, CancellationToken cancellationToken = default)
        {
            var id = this.Id;
            var refEntity = this.Entity;
            if (Kephas.Data.Id.IsEmpty(id))
            {
                return refEntity;
            }

            if (refEntity != null && Equals(refEntity.Id, id))
            {
                return refEntity;
            }

            var containerEntityInfo = this.GetContainerEntityInfo();
            var dataContext = this.GetDataContext(containerEntityInfo);

            var findContext = new FindContext<T>(dataContext, id, throwIfNotFound);
            refEntity = await dataContext.FindAsync<T>(findContext, cancellationToken).PreserveThreadContext();
            if (refEntity != null)
            {
                this.Entity = refEntity;
            }

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
    }
}