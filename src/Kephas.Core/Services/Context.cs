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
    using System.Security;
    using System.Security.Principal;

    using Kephas;
    using Kephas.Composition;
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
        private IIdentity identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        public Context()
            : this((ICompositionContext)null, isThreadSafe: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="isThreadSafe">Optional. <c>true</c> if this object is thread safe when working
        ///                            with the internal dictionary, <c>false</c> otherwise. Default is
        ///                            <c>false</c>.</param>
        /// <param name="merge">Optional. True to merge the parent context into the new context.</param>
        public Context(IContext parentContext, bool isThreadSafe = false, bool merge = false)
            : this(parentContext?.CompositionContext, isThreadSafe)
        {
            if (merge)
            {
                if (parentContext != null)
                {
                    this.Merge(parentContext);
                }
            }
            else
            {
                this.Identity = parentContext?.Identity;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional). If not provided,
        /// <see cref="M:AmbientServices.Instance"/> will be considered.
        /// </param>
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
        /// <param name="compositionContext">The context for the composition (optional). If not provided,
        /// <see cref="M:AmbientServices.Instance.CompositionContainer"/> will be considered.
        /// </param>
        /// <param name="isThreadSafe">
        /// <c>true</c> if this object is thread safe when working
        /// with the internal dictionary, <c>false</c> otherwise. Default is <c>false</c>.
        /// </param>
        public Context(ICompositionContext compositionContext, bool isThreadSafe = false)
            : base(isThreadSafe)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.SetCompositionContext(compositionContext);
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <newValue>
        /// The ambient services.
        /// </newValue>
        public IAmbientServices AmbientServices { get; private set; }

        /// <summary>
        /// Gets the dependency injection/composition context.
        /// </summary>
        /// <newValue>
        /// The composition context.
        /// </newValue>
        public ICompositionContext CompositionContext { get; private set; }

        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        /// <newValue>
        /// The authenticated user.
        /// </newValue>
        public IIdentity Identity
        {
            get => this.identity;
            set
            {
                if (this.ValidateIdentity(this.identity, value))
                {
                    this.identity = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the context logger.
        /// </summary>
        /// <newValue>
        /// The context logger.
        /// </newValue>
        public ILogger ContextLogger { get; set; }

        /// <summary>
        /// Validates the identity before changing it.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// True if validation succeeds, false if it fails.
        /// </returns>
        protected virtual bool ValidateIdentity(IIdentity currentValue, IIdentity newValue)
        {
            if (currentValue != null && currentValue != newValue)
            {
                throw new SecurityException(Strings.Context_CannotChangeIdentity_Exception);
            }

            return true;
        }

        /// <summary>
        /// Sets composition context.
        /// </summary>
        /// <param name="compositionContext">
        /// The context for the composition (optional). If not provided,
        /// <see cref="M:AmbientServices.Instance.CompositionContainer"/> will be considered.
        /// </param>
        protected virtual void SetCompositionContext(ICompositionContext compositionContext)
        {
            this.AmbientServices = compositionContext?.GetExport<IAmbientServices>() ?? Kephas.AmbientServices.Instance;
            this.CompositionContext = compositionContext ?? this.AmbientServices.CompositionContainer;
        }

        /// <summary>
        /// Sets ambient services.
        /// </summary>
        /// <remarks>
        /// The composition context is also set as the one exposed by the ambient services.
        /// </remarks>
        /// <param name="ambientServices">
        /// The ambient services (optional). If not provided,
        /// <see cref="M:AmbientServices.Instance"/> will be considered.
        /// </param>
        protected virtual void SetAmbientServices(IAmbientServices ambientServices)
        {
            this.AmbientServices = ambientServices ?? Kephas.AmbientServices.Instance;
            this.CompositionContext = this.AmbientServices.CompositionContainer;
        }
    }
}