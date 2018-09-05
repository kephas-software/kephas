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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Base class for references.
    /// </summary>
    public abstract class RefBase
    {
        /// <summary>
        /// The entity information provider.
        /// </summary>
        private readonly WeakReference<object> entityRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefBase"/> class.
        /// </summary>
        /// <param name="entity">The entity containing the reference.</param>
        /// <param name="refFieldName">The name of the reference field.</param>
        protected RefBase(object entity, string refFieldName)
        {
            Requires.NotNull(entity, nameof(entity));
            Requires.NotNull(refFieldName, nameof(refFieldName));

            this.entityRef = new WeakReference<object>(entity);
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
        protected virtual object GetEntityPropertyValue(string propertyName)
        {
            var entity = this.GetEntity();
            return entity is IIndexable expandoEntity
                       ? expandoEntity[propertyName]
                       : entity.GetPropertyValue(propertyName);
        }

        /// <summary>
        /// Sets the value of the indicated entity property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetEntityPropertyValue(string propertyName, object value)
        {
            var entity = this.GetEntity();
            if (entity is IIndexable expandoEntity)
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
        protected virtual object GetEntity()
        {
            var entity = this.GetEntityInfo()?.Entity;
            if (entity == null)
            {
                throw new DataException(string.Format(Strings.RefBase_GetEntity_Null_Exception, this.RefFieldName));
            }

            return entity;
        }

        /// <summary>
        /// Gets entity information.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the entity has been disposed.</exception>
        /// <returns>
        /// The entity information.
        /// </returns>
        protected virtual IEntityInfo GetEntityInfo()
        {
            if (!this.entityRef.TryGetTarget(out var entity))
            {
                throw new ObjectDisposedException(this.GetType().Name, string.Format(Strings.RefBase_GetEntityInfo_Disposed_Exception, this.RefFieldName));
            }

            var entityInfo = entity.TryGetAttachedEntityInfo();
            if (entityInfo == null)
            {
                throw new DataException(string.Format(Strings.RefBase_GetEntityInfo_Null_Exception, this.RefFieldName));
            }

            return entityInfo;
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
                throw new ArgumentNullException($"{nameof(entityInfo)}.{nameof(entityInfo.DataContext)}", string.Format(Strings.RefBase_GetDataContext_NullDataContext_Exception, this.RefFieldName));
            }

            return dataContext;
        }
    }
}