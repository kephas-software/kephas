// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Collections.Generic;

    using Kephas;
    using Kephas.Data.Capabilities;
    using Kephas.Dynamic;
    using Kephas.Model;
    using Kephas.Reflection;

    /// <summary>
    /// An entity base.
    /// </summary>
    public abstract class EntityBase : Expando, IEntity, IChangeStateTrackable, IEntityEntryAware
    {
        /// <summary>
        /// Information describing the type.
        /// </summary>
        private ITypeInfo typeInfo;

        /// <summary>
        /// Information describing the weak entity.
        /// </summary>
        private WeakReference<IEntityEntry> weakEntityEntry;

        /// <summary>
        /// The values.
        /// </summary>
        private IDictionary<string, object> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase"/> class.
        /// </summary>
        protected EntityBase()
            : this(new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase"/> class.
        /// </summary>
        /// <param name="innerDictionary">The inner dictionary.</param>
        private EntityBase(IDictionary<string, object> innerDictionary)
            : base(innerDictionary)
        {
            this.values = innerDictionary;
        }

        /// <summary>Gets the identifier for this instance.</summary>
        /// <value>The identifier.</value>
        object IIdentifiable.Id => this[nameof(IIdentifiable.Id)];

        /// <summary>Gets or sets the change state of the entity.</summary>
        /// <value>The change state.</value>
        ChangeState IChangeStateTrackable.ChangeState { get; set; }

        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public ITypeInfo GetTypeInfo()
        {
            return this.typeInfo ?? (this.typeInfo = this.ComputeTypeInfo());
        }

        /// <summary>
        /// Gets the associated entity entry.
        /// </summary>
        /// <returns>
        /// The associated entity entry.
        /// </returns>
        public IEntityEntry GetEntityEntry()
        {
            IEntityEntry entityEntry = null;
            this.weakEntityEntry?.TryGetTarget(out entityEntry);
            return entityEntry;
        }

        /// <summary>
        /// Sets the associated entity entry.
        /// </summary>
        /// <param name="entityEntry">Information describing the entity.</param>
        public void SetEntityEntry(IEntityEntry entityEntry)
        {
            this.weakEntityEntry = new WeakReference<IEntityEntry>(entityEntry);
        }

        /// <summary>
        /// Calculates the type information.
        /// </summary>
        /// <returns>
        /// The calculated type information.
        /// </returns>
        protected virtual ITypeInfo ComputeTypeInfo()
        {
            return this.GetAbstractTypeInfo();
        }

        /// <summary>Attempts to set the value with the given key.</summary>
        /// <remarks>
        /// First of all, it is tried to set the property value to the inner object, if one is set.
        /// The next try is to set the property value to the expando object itself.
        /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is used to set the value with the provided key.
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>
        /// <c>true</c> if the value could be set, <c>false</c> otherwise.
        /// </returns>
        protected override bool TrySetValue(string key, object value)
        {
            if (this.values.TryGetValue(key, out var currentValue))
            {
                if (Equals(currentValue, value))
                {
                    return true;
                }
            }

            this.values[key] = value;
            var trackable = this as IChangeStateTrackable;
            if (trackable.ChangeState == ChangeState.NotChanged)
            {
                trackable.ChangeState = ChangeState.Changed;
            }

            return true;
        }
    }
}