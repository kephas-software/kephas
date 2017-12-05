// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
}