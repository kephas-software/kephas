// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAbsoluteUrlResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Services
{
    using Kephas.Services;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Service contract resolving absolute URLs.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IAbsoluteUrlResolver
    {
        /// <summary>
        /// Gets the absolute URL for the provided context and path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="context">Optional. The context. Call this method with the <paramref name="context"/> parameter when implementing a service that has an HttpContext instance available.</param>
        /// <returns>The absolute URL.</returns>
        string? GetAbsoluteUrl(string path, HttpContext? context = null);
    }
}