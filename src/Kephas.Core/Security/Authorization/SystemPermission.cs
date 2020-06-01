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
    [PermissionInfo(Name, Scoping.Global)]
    [RequiresPermission(typeof(SystemPermission))]
    public sealed class SystemPermission : IPermission
    {
        /// <summary>
        /// The name of the AppAdmin permission.
        /// </summary>
        public const string Name = "system";

        private SystemPermission()
        {
        }
    }
}