// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationScopeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthorizationScopeProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Authorization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract for providing the authorization scope for a message.
    /// </summary>
    [SharedAppServiceContract]
    public interface IAuthorizationScopeProvider
    {
        /// <summary>
        /// Gets the authorization scope asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The message processing context.</param>
        /// <param name="cancellationToken">Optional. the cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the authorization scope.
        /// </returns>
        Task<object> GetAuthorizationScopeAsync(
            IMessage message,
            IMessageProcessingContext context,
            CancellationToken cancellationToken = default);
    }
}