// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides an indexer for getting and setting custom values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Dynamic;
    using System.Security.Principal;

    /// <summary>
    /// Defines a base contract for context-dependent operations.
    /// </summary>
    public interface IContext : IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        /// <value>
        /// The authenticated user.
        /// </value>
        IIdentity AuthenticatedUser { get; set; }
    }
}