// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingAuthorizationScopeProvider.cs" company="Kephas Software SRL">
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

    using Kephas.Security.Authorization;
    using Kephas.Services;

    /// <summary>
    /// Default implementation of an authorization scope provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class MessagingAuthorizationScopeProvider : IAuthorizationScopeProvider
    {
        /// <summary>
        /// Gets the authorization scope asynchronously.
        /// </summary>
        /// <param name="context">The message processing context.</param>
        /// <param name="cancellationToken">Optional. the cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the authorization scope.
        /// </returns>
        public Task<(object scope, bool canResolve)> GetAuthorizationScopeAsync(IContext context, CancellationToken cancellationToken = default)
        {
            if (context is IMessageProcessingContext processingContext)
            {
                return Task.FromResult(((object)processingContext.Message, true));
            }

            return Task.FromResult<(object scope, bool canResolve)>((null, false));
        }
    }
}