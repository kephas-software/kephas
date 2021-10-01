// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A base implementation for contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Services
{
    using System;
    using System.Security;
    using System.Security.Principal;

    using Kephas;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Resources;

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
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="isThreadSafe">
        /// <c>true</c> if this object is thread safe when working
        /// with the internal dictionary, <c>false</c> otherwise. Default is <c>false</c>.
        /// </param>
        public Context(IAmbientServices ambientServices, bool isThreadSafe = false)
            : base(isThreadSafe)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.SetAmbientServices(ambientServices);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="isThreadSafe">
        /// <c>true</c> if this object is thread safe when working
        /// with the internal dictionary, <c>false</c> otherwise. Default is <c>false</c>.
        /// </param>
        public Context(IInjector injector, bool isThreadSafe = false)
            : base(isThreadSafe)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.SetInjector(injector);
        }

        /// <summary>
        /// Occurs when the identity changes.
        /// </summary>
        public event EventHandler? IdentityChanged;

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <newValue>
        /// The ambient services.
        /// </newValue>
        public IAmbientServices AmbientServices { get; private set; }

        /// <summary>
        /// Gets the dependency injection/injector.
        /// </summary>
        /// <newValue>
        /// The injector.
        /// </newValue>
        public IInjector Injector { get; private set; }

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
                throw new SecurityException(Strings.Context_CannotChangeIdentity_Exception);
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
        /// <param name="injector">The injector.</param>
        protected virtual void SetInjector(IInjector injector)
        {
            Requires.NotNull(injector, nameof(injector));

            this.AmbientServices = injector.Resolve<IAmbientServices>();
            this.Injector = injector;
        }

        /// <summary>
        /// Sets the ambient services.
        /// </summary>
        /// <remarks>
        /// The injector is also set as the one exposed by the ambient services.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        protected virtual void SetAmbientServices(IAmbientServices ambientServices)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            this.AmbientServices = ambientServices;
            this.Injector = ambientServices.Injector;
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

        private static IInjector GetParentInjector(IContext parentContext)
        {
            Requires.NotNull(parentContext, nameof(parentContext));
            Requires.NotNull(parentContext.Injector, nameof(parentContext.Injector));

            return parentContext.Injector;
        }
    }
}