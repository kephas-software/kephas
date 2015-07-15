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
    using System.Dynamic;
    using System.Security.Principal;

    using Kephas.Dynamic;

    /// <summary>
    /// A base implementtion for contexts.
    /// </summary>
    public abstract class ContextBase : Expando, IContext
    {
        /// <summary>
        /// The custom values.
        /// </summary>
        private readonly dynamic data = new ExpandoObject();

        /// <summary>
        /// Gets the custom values.
        /// </summary>
        /// <value>
        /// The custom values.
        /// </value>
        public dynamic Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        /// <value>
        /// The authenticated user.
        /// </value>
        public IIdentity AuthenticatedIdentity { get; set; }

    }
}