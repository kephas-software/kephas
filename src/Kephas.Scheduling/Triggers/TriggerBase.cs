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
    using Kephas.Reflection;
    using Kephas.Scheduling.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A trigger base.
    /// </summary>
    public abstract class TriggerBase : Expando, ITrigger, IInitializable, IDisposable
    {
        /// <summary>
        /// Occurs when the trigger is fired.
        /// </summary>
        public event EventHandler Fire;

        /// <summary>
        /// Occurs when the trigger reached its end of life.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets or sets a value indicating whether the trigger is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the trigger ID.
        /// </summary>
        public object Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Gets type information.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public virtual ITriggerInfo GetTypeInfo() => (ITriggerInfo)this.GetRuntimeTypeInfo();

        /// <summary>
        /// Gets type information.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        ITypeInfo IInstance.GetTypeInfo() => this.GetTypeInfo();

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
            this.Dispose(true);
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
        protected virtual void OnFire()
        {
            this.Fire?.Invoke(this, EventArgs.Empty);
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