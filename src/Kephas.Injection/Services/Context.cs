// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A base implementation for contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Security;
    using System.Security.Principal;

    using Kephas;
    using Kephas.Dynamic;
    using Kephas.Injection;
    using Kephas.Logging;
    using Kephas.Serialization;

    /// <summary>
    /// A base implementation for contexts.
    /// </summary>
    public class Context : Expando, IContext
    {
        /// <summary>
        /// The identity.
        /// </summary>
        private IIdentity? identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="isThreadSafe">Optional. <c>true</c> if this object is thread safe when working
        ///                            with the internal dictionary, <c>false</c> otherwise. Default is
        ///                            <c>false</c>.</param>
        /// <param name="merge">Optional. True to merge the parent context into the new context.</param>
        public Context(IContext parentContext, bool isThreadSafe = false, bool merge = false)
            : this(GetParentInjector(parentContext), isThreadSafe)
        {
            if (merge)
            {
                this.Merge(parentContext);
            }
            else
            {
                this.Identity = parentContext.Identity;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="isThreadSafe">
        /// <c>true</c> if this object is thread safe when working
        /// with the internal dictionary, <c>false</c> otherwise. Default is <c>false</c>.
        /// </param>
        public Context(IServiceProvider serviceProvider, bool isThreadSafe = false)
            : base(isThreadSafe)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.SetInjector(serviceProvider);
        }

        /// <summary>
        /// Occurs when the identity changes.
        /// </summary>
        public event EventHandler? IdentityChanged;

        /// <summary>
        /// Gets the dependency injection/injector.
        /// </summary>
        /// <newValue>
        /// The injector.
        /// </newValue>
        [ExcludeFromSerialization]
        public virtual IServiceProvider ServiceProvider { get; private set; } = null!;

        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        /// <newValue>
        /// The authenticated user.
        /// </newValue>
        public IIdentity? Identity
        {
            get => this.identity;
            set
            {
                if (this.ValidateIdentity(this.identity, value))
                {
                    this.identity = value;
                    this.OnIdentityChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the context logger.
        /// </summary>
        /// <value>
        /// The context logger.
        /// </value>
        [ExcludeFromSerialization]
        public ILogger? Logger { get; set; }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Services.Context and optionally releases
        /// the managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Validates the identity before changing it.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// True if validation succeeds, false if it fails.
        /// </returns>
        protected virtual bool ValidateIdentity(IIdentity? currentValue, IIdentity? newValue)
        {
            if (currentValue != null && currentValue != newValue)
            {
                // TODO throw new SecurityException(Strings.Context_CannotChangeIdentity_Exception);
                throw new SecurityException("Cannot change the identity in the context once it has been set.");
            }

            return true;
        }

        /// <summary>
        /// Issues the <see cref="IdentityChanged"/> event.
        /// </summary>
        protected virtual void OnIdentityChanged()
        {
            this.IdentityChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the injector.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        protected virtual void SetInjector(IServiceProvider serviceProvider)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Services.Context and optionally releases
        /// the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.DisposeResources();
        }

        private static IServiceProvider GetParentInjector(IContext parentContext)
        {
            parentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));
            if (parentContext.ServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(parentContext.ServiceProvider));
            }

            return parentContext.ServiceProvider;
        }
    }
}