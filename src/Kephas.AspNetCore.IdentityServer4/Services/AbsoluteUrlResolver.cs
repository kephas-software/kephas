// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsoluteUrlResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Services
{
    using System;

    using Kephas.Services;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Default implementation of the <see cref="IAbsoluteUrlResolver"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class AbsoluteUrlResolver : IAbsoluteUrlResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbsoluteUrlResolver"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public AbsoluteUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            // We need the context accessor here in order to produce an absolute url from a potentially relative url.
            this.ContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the context accessor.
        /// </summary>
        protected IHttpContextAccessor ContextAccessor { get; }

        /// <summary>
        /// Gets the absolute URL for the provided context and path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="context">Optional. The context. Call this method with the <paramref name="context"/> parameter when implementing a service that has an HttpContext instance available.</param>
        /// <returns>The absolute URL.</returns>
        public virtual string? GetAbsoluteUrl(string path, HttpContext? context = null)
        {
            var (process, result) = this.ShouldProcessPath(path);
            if (!process)
            {
                return result;
            }

            context ??= this.ContextAccessor.HttpContext;
            if (context?.Request == null)
            {
                throw new InvalidOperationException("The request is not currently available. This service can only be used within the context of an existing HTTP request.");
            }

            var request = context.Request;
            return $"{request.Scheme}://{request.Host.ToUriComponent()}{request.PathBase.ToUriComponent()}{path}";
        }

        private (bool, string?) ShouldProcessPath(string? path)
        {
            if (path == null || !Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
            {
                return (false, null);
            }

            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                return (false, path);
            }

            return (true, path);
        }
    }
}
