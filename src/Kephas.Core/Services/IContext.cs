// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines a base contract for context-dependent operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Security.Principal;

    using Kephas.Dynamic;

    /// <summary>
    /// Defines a base contract for context-dependent operations.
    /// </summary>
    public interface IContext : IExpando, IAmbientServicesAware
    {
        /// <summary>
        /// Gets or sets the authenticated identity.
        /// </summary>
        /// <value>
        /// The authenticated identity.
        /// </value>
        IIdentity Identity { get; set; }
    }
}