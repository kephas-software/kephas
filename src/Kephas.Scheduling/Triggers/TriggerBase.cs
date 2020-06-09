// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the trigger base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Triggers
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Scheduling.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A trigger base.
    /// </summary>
    public abstract class TriggerBase : Expando, ITrigger, IInitializable
    {
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerBase"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        protected TriggerBase(IRuntimeTypeRegistry? typeRegistry = null)
        {
            this.TypeRegistry = typeRegistry;
        }

        /// <summary>
        /// Occurs when the trigger is fired.
        /// </summary>
        public event EventHandler<FireEventArgs>? Fire;

        /// <summary>
        /// Occurs when the trigger reached its end of life.
        /// </summary>
        public event EventHandler? Disposed;

        /// <summary>
        /// Gets or sets a value indicating whether the trigger is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the trigger ID.
        /// </summary>
        public object Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        protected IRuntimeTypeRegistry? TypeRegistry { get; }

        /// <summary>
        /// Gets type information.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public virtual ITriggerInfo GetTypeInfo() => (ITriggerInfo)this.GetRuntimeTypeInfo(this.TypeRegistry)!;

        /// <summary>
        /// Gets type information.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        ITypeInfo IInstance.GetTypeInfo() => this.GetTypeInfo();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.GetTypeInfo().Name} ({this.Id})";
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        public virtual void Initialize(IContext? context = null)
        {
            this.IsEnabled = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.Dispose(true);

            this.isDisposed = true;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="TriggerBase"/> and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.IsEnabled = false;

            this.OnDisposed();
        }

        /// <summary>
        /// Triggers the <see cref="Fire"/> event.
        /// </summary>
        /// <param name="completeCallback">Optional. The callback upon job completion.</param>
        protected virtual void OnFire(Action<IOperationResult>? completeCallback = null)
        {
            if (this.IsEnabled)
            {
                this.Fire?.Invoke(this, new FireEventArgs(completeCallback));
            }
        }

        /// <summary>
        /// Triggers the <see cref="Disposed"/> event.
        /// </summary>
        protected virtual void OnDisposed()
        {
            this.Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}