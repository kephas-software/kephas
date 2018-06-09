// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A base implementtion for contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Security.Principal;

    using Kephas;
    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Logging;

    /// <summary>
    /// A base implementtion for contexts.
    /// </summary>
    public class Context : Expando, IContext
    {
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
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; private set; }

        /// <summary>
        /// Gets the dependency injection/composition context.
        /// </summary>
        /// <value>
        /// The composition context.
        /// </value>
        public ICompositionContext CompositionContext { get; private set; }

        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        /// <value>
        /// The authenticated user.
        /// </value>
        public IIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets the context logger.
        /// </summary>
        /// <value>
        /// The context logger.
        /// </value>
        public ILogger ContextLogger { get; set; }

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