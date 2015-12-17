// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A base implementtion for contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Diagnostics.Contracts;
    using System.Security.Principal;

    using Kephas;
    using Kephas.Dynamic;

    /// <summary>
    /// A base implementtion for contexts.
    /// </summary>
    public abstract class ContextBase : Expando, IContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBase"/> class.
        /// </summary>
        protected ContextBase()
            : this(Kephas.AmbientServices.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBase"/> class.
        /// </summary>
        /// <param name="ambientServices">
        /// The ambient services.
        /// </param>
        protected ContextBase(IAmbientServices ambientServices)
        {
            Contract.Requires(ambientServices != null);

            this.AmbientServices = ambientServices;
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        /// <value>
        /// The authenticated user.
        /// </value>
        public IIdentity Identity { get; set; }
    }
}