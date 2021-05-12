// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemPermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// Defines the system permission reserved by the internal system identity.
    /// </summary>
    [PermissionInfo(TokenName, Scoping.Global)]
    [RequiresPermission(typeof(SystemPermission))]
    public sealed class SystemPermission
    {
        /// <summary>
        /// The token name of the System permission.
        /// </summary>
        public const string TokenName = "system";

        private SystemPermission()
        {
        }
    }
}