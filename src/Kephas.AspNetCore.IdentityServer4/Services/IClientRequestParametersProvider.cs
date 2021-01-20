// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientRequestParametersProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Services
{
    using System.Collections.Generic;

    using Kephas.Services;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Generates OAUTH/OpenID parameter values for configured clients.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IClientRequestParametersProvider
    {
        /// <summary>
        /// Gets parameter values for the client with client id<paramref name="clientId"/>.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <param name="clientId">The client id for the client.</param>
        /// <returns>A <see cref="IDictionary{TKey, TValue}"/> containing the client parameters and their values.</returns>
        IDictionary<string, string> GetClientParameters(HttpContext context, string clientId);
    }
}