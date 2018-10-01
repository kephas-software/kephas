// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAuthorizationScopeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default authorization scope provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Authorization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Default implementation of an authorization scope provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAuthorizationScopeProvider : IAuthorizationScopeProvider
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
        public Task<object> GetAuthorizationScopeAsync(
            IMessage message,
            IMessageProcessingContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object>(message);
        }
    }
}