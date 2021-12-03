// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefBase.cs" company="Kephas Software SRL">
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

    using Kephas.Data.Capabilities;
    using Kephas.Data.Resources;
    using Kephas.Dynamic;

    /// <summary>
    /// Base class for references.
    /// </summary>
    public abstract class RefBase
    {
        /// <summary>
        /// The container entity entry provider.
        /// </summary>
        private readonly WeakReference<object> containerEntityRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefBase"/> class.
        /// </summary>
        /// <param name="containerEntity">The entity containing the reference.</param>
        /// <param name="refFieldName">The name of the reference field.</param>
        protected RefBase(object containerEntity, string refFieldName)
        {
            containerEntity = containerEntity ?? throw new System.ArgumentNullException(nameof(containerEntity));
            refFieldName = refFieldName ?? throw new System.ArgumentNullException(nameof(refFieldName));

            this.containerEntityRef = new WeakReference<object>(containerEntity);
            this.RefFieldName = refFieldName;
        }

        /// <summary>
        /// Gets the name of the reference field.
        /// </summary>
        /// <value>
        /// The name of the reference field.
        /// </value>
        protected string RefFieldName { get; }

        /// <summary>
        /// Gets the value of the indicated entity property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The value of the entity property.
        /// </returns>
        protected virtual object? GetEntityPropertyValue(string propertyName)
        {
            var entity = this.GetContainerEntity();
            return entity is IDynamic expandoEntity
                       ? expandoEntity[propertyName]
                       : entity.GetPropertyValue(propertyName);
        }

        /// <summary>
        /// Sets the value of the indicated entity property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetEntityPropertyValue(string propertyName, object? value)
        {
            var entity = this.GetContainerEntity();
            if (entity is IDynamic expandoEntity)
            {
                expandoEntity[propertyName] = value;
            }
            else
            {
                entity.SetPropertyValue(propertyName, value);
            }
        }

        /// <summary>
        /// Gets the entity containing the reference.
        /// </summary>
        /// <returns>
        /// The entity containing the reference.
        /// </returns>
        protected virtual object GetContainerEntity()
        {
            var entity = this.GetContainerEntityEntry()?.Entity;
            if (entity == null)
            {
                throw new DataException(string.Format(Strings.RefBase_GetEntity_Null_Exception, this.RefFieldName));
            }

            return entity;
        }

        /// <summary>
        /// Gets the container entity entry.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the entity has been disposed.</exception>
        /// <returns>
        /// The container entity entry.
        /// </returns>
        protected virtual IEntityEntry GetContainerEntityEntry()
        {
            if (!this.containerEntityRef.TryGetTarget(out var containerEntity))
            {
                throw new ObjectDisposedException(this.GetType().Name, string.Format(Strings.RefBase_GetEntityEntry_Disposed_Exception, this.RefFieldName));
            }

            var entityEntry = containerEntity.TryGetAttachedEntityEntry();
            if (entityEntry == null)
            {
                throw new DataException(string.Format(Strings.RefBase_GetEntityEntry_Null_Exception, this.RefFieldName));
            }

            return entityEntry;
        }

        /// <summary>
        /// Gets the data context for entity retrieval.
        /// </summary>
        /// <param name="entityEntry">Information describing the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        protected virtual IDataContext GetDataContext(IEntityEntry entityEntry)
        {
            var dataContext = entityEntry?.DataContext;
            if (dataContext == null)
            {
                throw new ArgumentNullException($"{nameof(entityEntry)}.{nameof(entityEntry.DataContext)}", string.Format(Strings.RefBase_GetDataContext_NullDataContext_Exception, this.RefFieldName));
            }

            return dataContext;
        }
    }
}