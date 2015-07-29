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
    using System.Security.Principal;

    using Kephas.Dynamic;

    /// <summary>
    /// A base implementtion for contexts.
    /// </summary>
    public abstract class ContextBase : Expando, IContext
    {
        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        /// <value>
        /// The authenticated user.
        /// </value>
        public IIdentity AuthenticatedIdentity { get; set; }
    }
}