// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityEntry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEntityEntry interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    using System;

    using Kephas.Data.Behaviors;
    using Kephas.Dynamic;

    /// <summary>
    /// Provides extended entity entry like the <see cref="ChangeState"/>.
    /// </summary>
    public interface IEntityEntry : IExpandoBase, IChangeStateTrackableEntityEntry, IIdentifiable, IAggregatable, IDisposable
    {
        /// <summary>
        /// Gets a copy of the original entity, before any changes occurred.
        /// </summary>
        /// <value>
        /// The original entity.
        /// </value>
        IExpandoBase OriginalEntity { get; }

        /// <summary>
        /// Gets the identifier of the entity.
        /// </summary>
        /// <value>
        /// The identifier of the entity.
        /// </value>
        object? EntityId { get; }

        /// <summary>
        /// Gets or sets the entity owning data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        IDataContext DataContext { get; set; }

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
        ChangeState PrePersistChangeState { get; set; }

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
    /// Extension methods for <see cref="IEntityEntry"/>.
    /// </summary>
    public static class EntityEntryExtensions
    {
        /// <summary>
        /// Tries to attach the entity entry to the given entity.
        /// </summary>
        /// <param name="entityEntry">The entityEntry to act on.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// <c>true</c> if the attach succeeded, otherwise <c>false</c>.
        /// </returns>
        public static bool TryAttachToEntity(this IEntityEntry entityEntry, object entity)
        {
            // TODO see issue https://github.com/kephas-software/kephas/issues/36
            if (entityEntry == null || entity == null)
            {
                return false;
            }

            if (entity is IEntityEntryAware entityEntryAware)
            {
                entityEntryAware.SetEntityEntry(entityEntry);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to get the attached entity entry from the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The attached <see cref="IEntityEntry"/>, or <c>null</c>.
        /// </returns>
        public static IEntityEntry? TryGetAttachedEntityEntry(this object entity)
        {
            // TODO see issue https://github.com/kephas-software/kephas/issues/36
            if (entity is IEntityEntryAware entityEntryAware)
            {
                return entityEntryAware.GetEntityEntry();
            }

            return null;
        }
    }
}