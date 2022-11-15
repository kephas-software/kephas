// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityServerJwtConstants.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Authentication
{
    /// <summary>
    /// Constants for a default API authentication handler.
    /// </summary>
    public class IdentityServerJwtConstants
    {
        /// <summary>
        /// Scheme used for the default API policy authentication scheme.
        /// </summary>
        public const string IdentityServerJwtScheme = "IdentityServerJwt";

        /// <summary>
        /// Scheme used for the underlying default API JwtBearer authentication scheme.
        /// </summary>
        public const string IdentityServerJwtBearerScheme = "IdentityServerJwtBearer";
    }
}
