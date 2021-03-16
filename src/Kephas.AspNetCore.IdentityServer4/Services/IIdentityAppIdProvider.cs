// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentityAppIdProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Services
{
    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// Service for getting the application identity for the identity infrastructure.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IIdentityAppIdProvider
    {
        /// <summary>
        /// Gets the application identity for the identity infrastructure.
        /// </summary>
        /// <returns>The <see cref="AppIdentity"/>.</returns>
        AppIdentity GetIdentityAppId();
    }
}
