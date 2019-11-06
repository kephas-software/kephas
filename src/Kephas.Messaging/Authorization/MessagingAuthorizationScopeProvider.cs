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
    /// Default implementation of an authorization scope provider for messages.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class MessagingAuthorizationScopeProvider : IAuthorizationScopeProvider
    {
        /// <summary>
        /// Gets the authorization scope asynchronously.
        /// </summary>
        /// <param name="context">The scope context.</param>
        /// <param name="cancellationToken">Optional. the cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the authorization scope.
        /// </returns>
        public Task<(object scope, bool canResolve)> GetAuthorizationScopeAsync(IAuthorizationScopeContext context, CancellationToken cancellationToken = default)
        {
            if (context?.CallingContext is IMessagingContext messagingContext)
            {
                return Task.FromResult((messagingContext.Message.GetContent(), true));
            }

            return Task.FromResult<(object scope, bool canResolve)>((null, false));
        }
    }
}