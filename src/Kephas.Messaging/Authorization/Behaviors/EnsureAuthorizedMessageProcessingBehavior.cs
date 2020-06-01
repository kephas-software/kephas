// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureAuthorizedMessageProcessingBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the authorize message processing behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Authorization.Behaviors
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Messaging.Behaviors;
    using Kephas.Messaging.Behaviors.AttributedModel;
    using Kephas.Messaging.Composition;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A message processing behavior ensuring that only authorized calls execute the request.
    /// </summary>
    [MessagingBehavior(MessageTypeMatching.TypeOrHierarchy)]
    [ProcessingPriority(Priority.Highest + 10)]
    public class EnsureAuthorizedMessageProcessingBehavior : MessagingBehaviorBase<IMessage>
    {
        /// <summary>
        /// The authorization service.
        /// </summary>
        private readonly IAuthorizationService authorizationService;

        /// <summary>
        /// The authorization scope service.
        /// </summary>
        private readonly IAuthorizationScopeService authorizationScopeService;

        /// <summary>
        /// The permissions map.
        /// </summary>
        private readonly ConcurrentDictionary<Type, IReadOnlyList<Type>>
            permissionsMap = new ConcurrentDictionary<Type, IReadOnlyList<Type>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureAuthorizedMessageProcessingBehavior"/>
        /// class.
        /// </summary>
        /// <param name="authorizationService">The authorization service.</param>
        /// <param name="authorizationScopeService">The authorization scope service.</param>
        public EnsureAuthorizedMessageProcessingBehavior(
            IAuthorizationService authorizationService,
            IAuthorizationScopeService authorizationScopeService)
        {
            Requires.NotNull(authorizationService, nameof(authorizationService));
            Requires.NotNull(authorizationScopeService, nameof(authorizationScopeService));

            this.authorizationService = authorizationService;
            this.authorizationScopeService = authorizationScopeService;
        }

        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        public override async Task BeforeProcessAsync(IMessage message, IMessagingContext context, CancellationToken token)
        {
            var messageType = message.GetType();

            var permissions = this.GetRequiredPermissions(messageType);
            if ((permissions?.Count ?? 0) > 0)
            {
                var authScope = await this.authorizationScopeService.GetAuthorizationScopeAsync(context, cancellationToken: token).PreserveThreadContext();
                await this.authorizationService.AuthorizeAsync(context, permissions, authScope, cancellationToken: token).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Gets the required permissions in this collection.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        /// The required permissions.
        /// </returns>
        private IReadOnlyList<Type> GetRequiredPermissions(Type messageType)
        {
            var perms = this.permissionsMap.GetOrAdd(messageType, this.ComputePermissions);
            return perms;
        }

        /// <summary>
        /// Calculates the permissions.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        /// The calculated permissions.
        /// </returns>
        private IReadOnlyList<Type> ComputePermissions(Type messageType)
        {
            var permAttrs = messageType.GetTypeInfo().GetCustomAttributes<RequiresPermissionAttribute>();
            var permissions = new HashSet<Type>();
            foreach (var permAttr in permAttrs)
            {
                permissions.AddRange(permAttr.PermissionTypes);
            }

            return permissions.ToList().AsReadOnly();
        }
    }
}