// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationScopeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthorizationScopeProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract for providing the authorization scope for a message.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true)]
    public interface IAuthorizationScopeProvider
    {
        /// <summary>
        /// Gets the authorization scope asynchronously.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. the cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the authorization scope.
        /// </returns>
        Task<(object scope, bool canResolve)> GetAuthorizationScopeAsync(IContext context, CancellationToken cancellationToken = default);
    }
}