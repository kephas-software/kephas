// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultPermissionService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.Permissions
{
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Default implementation of <see cref="IPermissionService"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultPermissionService : PermissionServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPermissionService"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public DefaultPermissionService(
            IRuntimeTypeRegistry typeRegistry,
            ILogManager? logManager = null)
            : base(typeRegistry, logManager)
        {
        }
    }
}