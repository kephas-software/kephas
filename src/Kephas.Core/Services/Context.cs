// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A base implementtion for contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Security.Principal;

    using Kephas;
    using Kephas.Dynamic;

    /// <summary>
    /// A base implementtion for contexts.
    /// </summary>
    public class Context : Expando, IContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional). If not provided, <see cref="M:AmbientServices.Instance"/> will be considered.</param>
        public Context(IAmbientServices ambientServices = null)
        {
            this.AmbientServices = ambientServices ?? Kephas.AmbientServices.Instance;
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