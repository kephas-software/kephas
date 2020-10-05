// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenRef.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the llbl generate reference class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Data.Capabilities;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An entity reference specialized for the LLBLGen infrastructure.
    /// </summary>
    /// <typeparam name="T">The referenced entity type.</typeparam>
    public class LLBLGenRef<T> : Ref<T>
        where T : class, IIdentifiable
    {
        /// <summary>
        /// Name of the reference.
        /// </summary>
        private readonly string refName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenRef{T}" /> class.
        /// </summary>
        /// <param name="entityEntryAware">The entity information provider.</param>
        /// <param name="refIdName">Name of the reference identifier.</param>
        public LLBLGenRef(IEntityEntryAware entityEntryAware, string refIdName)
            : base(entityEntryAware, refIdName)
        {
            this.refName = refIdName.Substring(0, refIdName.Length - 2);
        }

        /// <summary>Gets or sets the identifier of the referenced entity.</summary>
        /// <value>The identifier of the referenced entity.</value>
        public override object? Id
        {
            get
            {
                var value = base.Id;
                if (Kephas.Data.Id.IsEmpty(value))
                {
                    var entity = this.Entity;
                    if (entity != null)
                    {
                        return entity.Id;
                    }
                }

                return value;
            }

            set
            {
                // if the entity is already set, do not change anything if the new ID is the same as the entity's
                // otherwise, in case of new referenced entities, the child collections may get empty.
                var newId = (long?)value;
                var entity = this.Entity;
                if (entity != null && newId != null && Equals(entity.Id, newId.Value))
                {
                    return;
                }

                base.Id = value;
            }
        }

        /// <summary>
        /// Gets or sets the identifier of the referenced entity.
        /// </summary>
        /// <value>
        /// The identifier of the referenced entity.
        /// </value>
        public override T? Entity
        {
            get => (T?)this.GetEntityPropertyValue(this.refName);
            set => this.SetEntityPropertyValue(this.refName, value);
        }

        /// <summary>Gets the referenced entity asynchronously.</summary>
        /// <param name="throwIfNotFound">If true and the referenced entity is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>A task promising the referenced entity.</returns>
        public override async Task<T?> GetAsync(bool throwIfNotFound = true, CancellationToken cancellationToken = default)
        {
            var id = this.Id;
            var entity = this.Entity;
            if (Kephas.Data.Id.IsEmpty(id))
            {
                return entity;
            }

            if (entity != null && entity.Id.Equals(id))
            {
                return entity;
            }

            // TODO CHECK when upgrading to Kephas 6.0.0 if the semantics regarding the temporary IDs is preserved when removing this code/method
            // one thing could be: if the id is not found, entity is set to NULL - is this really necessary/OK?
            if (Kephas.Data.Id.IsTemporary(id))
            {
                var containerEntityEntry = this.GetContainerEntityEntry();
                var dataContext = this.GetDataContext(containerEntityEntry);
                entity = (T)await dataContext.FindAsync(
                                    typeof(T),
                                    id,
                                    throwIfNotFound,
                                    cancellationToken: cancellationToken).PreserveThreadContext();
                this.Entity = entity;
                return entity;
            }

            return await base.GetAsync(throwIfNotFound, cancellationToken).PreserveThreadContext();
        }
    }
}