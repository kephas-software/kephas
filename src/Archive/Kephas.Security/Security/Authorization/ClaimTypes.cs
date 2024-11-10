// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClaimTypes.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    /// <summary>
    /// Enumerates the claim types used in ClaimsIdentity.
    /// </summary>
    public static class ClaimTypes
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public const string Guid = "urn:kephas:claims:guid";

        /// <summary>
        /// Granted permissions.
        /// </summary>
        public const string GrantedPermissions = "urn:kephas:claims:grantedpermissions";
    }
}