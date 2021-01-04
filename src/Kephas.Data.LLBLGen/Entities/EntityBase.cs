// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Entities
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using Kephas.Data.Capabilities;
    using Kephas.Model;
    using Kephas.Reflection;
    using Kephas.Runtime;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// Base class for LLBLGen entities.
    /// </summary>
    public abstract class EntityBase : EntityBase2, IIdentifiable, IChangeStateTrackable, INotifyPropertyChanging, IInstance, IEntityEntryAware, IEntityBase
    {
        /// <summary>
        /// Information describing the type.
        /// </summary>
        private ITypeInfo typeInfo;

        /// <summary>
        /// Information describing the runtime type.
        /// </summary>
        private IRuntimeTypeInfo runtimeTypeInfo;

        /// <summary>
        /// Information describing the weak entity.
        /// </summary>
        private WeakReference<IEntityEntry> weakEntityInfo;

        /// <summary>
        /// The change state.
        /// </summary>
        private ChangeState changeState = ChangeState.NotChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase"/> class.
        /// </summary>
        protected EntityBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected EntityBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Occurs when a property is about to change.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Gets or sets the change state of the entity.
        /// </summary>
        /// <value>
        /// The change state.
        /// </value>
        [XmlIgnore]
        [IgnoreDataMember]
        public ChangeState ChangeState
        {
            get => this.IsNew || this.changeState == ChangeState.Added
                       ? ChangeState.Added
                       : this.MarkedForDeletion || this.changeState == ChangeState.Deleted
                           ? ChangeState.Deleted
                           : this.IsDirty // || this.changeState == ChangeState.Changed // sometimes, the change state is set to dirty, but then reset (load from DB),
                                          // so the changeState and IsDirty are desynchronized. Also, when
                               ? ChangeState.Changed
                               : ChangeState.NotChanged;

            set
            {
                switch (value)
                {
                    case ChangeState.Added:
                        this.changeState = ChangeState.Added;
                        this.IsNew = true;
                        this.IsDirty = false;
                        this.MarkedForDeletion = false;
                        break;
                    case ChangeState.Deleted:
                        this.changeState = ChangeState.Deleted;
                        this.IsNew = false;
                        this.IsDirty = false;
                        this.MarkedForDeletion = true;
                        break;
                    case ChangeState.Changed:
                        this.changeState = ChangeState.Changed;
                        this.IsNew = false;
                        this.IsDirty = true;
                        this.MarkedForDeletion = false;
                        break;
                    case ChangeState.NotChanged:
                        this.changeState = ChangeState.NotChanged;
                        this.IsNew = false;
                        this.IsDirty = false;
                        this.MarkedForDeletion = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the identifier for the entity.
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        object IIdentifiable.Id => this[nameof(IIdentifiable.Id)];

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlIgnore]
        [IgnoreDataMember]
        long IEntityBase.Id
        {
            get => (long?)this[nameof(IIdentifiable.Id)] ?? default;
            set => this[nameof(IIdentifiable.Id)] = value;
        }

        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="T:System.Object" /> identified by the key.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public object? this[string key]
        {
            get
            {
                var field = this.Fields?[key];
                if (field != null)
                {
                    return field.CurrentValue;
                }

                return this.GetRuntimeTypeInfo().GetValue(this, key);
            }

            // always set the value through reflection so that the change state is properly set,
            // not directly in the field's current value.
            set => this.GetRuntimeTypeInfo().SetValue(this, key, value);
        }

        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public ITypeInfo GetTypeInfo()
        {
            return this.typeInfo ??= this.ComputeTypeInfo();
        }

        /// <summary>
        /// Gets the associated entity information.
        /// </summary>
        /// <returns>
        /// The associated entity information.
        /// </returns>
        public IEntityEntry? GetEntityEntry()
        {
            IEntityEntry? entityEntry = null;
            this.weakEntityInfo?.TryGetTarget(out entityEntry);
            return entityEntry;
        }

        /// <summary>
        /// Sets the associated entity information.
        /// </summary>
        /// <param name="entityEntry">Information describing the entity.</param>
        public void SetEntityEntry(IEntityEntry entityEntry)
        {
            this.weakEntityInfo = new WeakReference<IEntityEntry>(entityEntry);
        }

        /// <summary>
        /// Gets the runtime type information.
        /// </summary>
        /// <returns>
        /// The runtime type information.
        /// </returns>
        protected IRuntimeTypeInfo GetRuntimeTypeInfo()
        {
            return this.runtimeTypeInfo ??= this.GetType().AsRuntimeTypeInfo();
        }

        /// <summary>
        /// Calculates the type information.
        /// </summary>
        /// <returns>
        /// The calculated type information.
        /// </returns>
        protected virtual ITypeInfo ComputeTypeInfo()
        {
            return this.GetType().GetAbstractType().AsRuntimeTypeInfo();
        }

        /// <summary>
        /// Executes the set value action.
        /// </summary>
        /// <param name="fieldIndex">Zero-based index of the field.</param>
        /// <param name="valueToSet">Set the value to belongs to.</param>
        /// <param name="cancel">The cancel.</param>
        protected override void OnSetValue(int fieldIndex, object valueToSet, out bool cancel)
        {
            var propertyName = this.Fields[fieldIndex].Name;
            this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

            base.OnSetValue(fieldIndex, valueToSet, out cancel);
        }

        /// <summary>Marks the fields as dirty.</summary>
        protected override void MarkFieldsAsDirty()
        {
            base.MarkFieldsAsDirty();
            if (this.changeState == ChangeState.NotChanged)
            {
                this.changeState = ChangeState.Changed;
            }
        }

        /// <summary>
        /// Method which will check if the caller is allowed to fill this entity object with entity data from the database or not.
        /// </summary>
        /// <returns>true if the caller is allowed to load the data into this instance, otherwise false.</returns>
        protected override bool OnCanLoadEntity()
        {
            return !this.MarkedForDeletion && !this.IsDirty && base.OnCanLoadEntity();
        }
    }
}