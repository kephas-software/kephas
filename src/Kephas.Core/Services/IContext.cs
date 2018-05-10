// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines a base contract for context-dependent operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Security.Principal;

    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Logging;

    /// <summary>
    /// Defines a base contract for context-dependent operations.
    /// </summary>
    public interface IContext : IExpando, IAmbientServicesAware, ICompositionContextAware
    {
        /// <summary>
        /// Gets or sets the authenticated identity.
        /// </summary>
        /// <value>
        /// The authenticated identity.
        /// </value>
        IIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets the context logger.
        /// </summary>
        /// <value>
        /// The context logger.
        /// </value>
        ILogger ContextLogger { get; set; }
    }
}