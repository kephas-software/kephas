// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullAuthorizationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null authorization service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A null authorization service.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullAuthorizationService : IAuthorizationService
    {
        /// <summary>
        /// Authorizes the provided context asynchronously.
        /// </summary>
        /// <param name="authContext">Context for the authorization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result indicating whether the operation succeeded or not.
        /// </returns>
        public Task<bool> AuthorizeAsync(IAuthorizationContext authContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }
}