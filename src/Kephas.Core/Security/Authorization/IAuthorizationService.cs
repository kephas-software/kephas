// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthorizationService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract for handling authorization.
    /// </summary>
    [SharedAppServiceContract]
    public interface IAuthorizationService
    {
        /// <summary>
        /// Authorizes the provided context asynchronously.
        /// </summary>
        /// <param name="authContext">Context for the authorization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result indicating whether the operation succeeded or not.
        /// </returns>
        Task<bool> AuthorizeAsync(
            IAuthorizationContext authContext,
            CancellationToken cancellationToken = default);
    }
}