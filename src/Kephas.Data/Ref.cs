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
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ref{T}"/> class.
        /// </summary>
        /// <param name="entity">The entity containing the reference.</param>
        /// <param name="refFieldName">Name of the reference identifier property.</param>
        public Ref(object entity, string refFieldName)
            : base(entity, refFieldName)
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
        /// Gets a value indicating whether the reference is empty/not set.
        /// </summary>
        /// <value>
        /// True if this reference is empty, false if not.
        /// </value>
        public virtual bool IsEmpty => Kephas.Data.Id.IsEmpty(this.Id);

        /// <summary>
        /// Gets or sets the identifier of the referenced entity.
        /// </summary>
        /// <value>
        /// The identifier of the referenced entity.
        /// </value>
        public virtual object Id
        {
            get => this.GetEntityPropertyValue(this.RefFieldName);
            set => this.SetEntityPropertyValue(this.RefFieldName, value);
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
            if (Data.Id.IsEmpty(this.Id))
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
    }
}