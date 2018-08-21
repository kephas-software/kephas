// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEntityInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    using System;

    using Kephas.Dynamic;

    /// <summary>
    /// Provides extended entity information like the <see cref="ChangeState"/>.
    /// </summary>
    public interface IEntityInfo : IExpando, IChangeStateTrackable, IIdentifiable, IAggregatable, IDisposable
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        object Entity { get; }

        /// <summary>
        /// Gets a copy of the original entity, before any changes occured.
        /// </summary>
        /// <value>
        /// The original entity.
        /// </value>
        IExpando OriginalEntity { get; }

        /// <summary>
        /// Gets the identifier of the entity.
        /// </summary>
        /// <value>
        /// The identifier of the entity.
        /// </value>
        object EntityId { get; }

        /// <summary>
        /// Gets or sets the entity owning data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        IDataContext DataContext { get; set; }

        /// <summary>
        /// Gets a value indicating whether the provided property changed.
        /// </summary>
        /// <param name="property">The property name.</param>
        /// <returns>
        /// True if the property changed, false if not.
        /// </returns>
        bool IsChanged(string property);

        /// <summary>
        /// Accepts the changes and resets the change state to <see cref="ChangeState.NotChanged"/>.
        /// </summary>
        void AcceptChanges();

        /// <summary>
        /// Discards the changes and resets the change state to <see cref="ChangeState.NotChanged"/>.
        /// </summary>
        void DiscardChanges();
    }

    /// <summary>
    /// An entity information extensions.
    /// </summary>
    public static class EntityInfoExtensions
    {
        /// <summary>
        /// Tries to attach the entity information to the given entity.
        /// </summary>
        /// <param name="entityInfo">The entityInfo to act on.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// <c>true</c> if the attach succeeded, otherwise <c>false</c>.
        /// </returns>
        public static bool TryAttachToEntity(this IEntityInfo entityInfo, object entity)
        {
            // TODO see issue https://github.com/quartz-software/kephas/issues/36
            if (entityInfo == null || entity == null)
            {
                return false;
            }

            if (entity is IEntityInfoAware entityInfoAware)
            {
                entityInfoAware.SetEntityInfo(entityInfo);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to get the attached entity information from the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The attached <see cref="IEntityInfo"/>, or <c>null</c>.
        /// </returns>
        public static IEntityInfo TryGetAttachedEntityInfo(this object entity)
        {
            // TODO see issue https://github.com/quartz-software/kephas/issues/36
            if (entity is IEntityInfoAware entityInfoAware)
            {
                return entityInfoAware.GetEntityInfo();
            }

            return null;
        }
    }
}