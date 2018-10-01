// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeMessageProcessingBehavior.cs" company="Kephas Software SRL">
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
    [MessageProcessingBehavior(MessageTypeMatching.TypeOrHierarchy)]
    [ProcessingPriority(Priority.Highest)]
    public class EnsureAuthorizedMessageProcessingBehavior : MessageProcessingBehaviorBase<IMessage>
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
        private readonly ConcurrentDictionary<Type, (IReadOnlyList<string> names, IReadOnlyList<Type> types)>
            permissionsMap = new ConcurrentDictionary<Type, (IReadOnlyList<string> names, IReadOnlyList<Type> types)>();

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
        public override async Task BeforeProcessAsync(IMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            var messageType = message.GetType();

            var (names, types) = this.GetRequiredPermissions(messageType);
            if ((names != null && names.Count > 0) || (types != null && types.Count > 0))
            {
                var authScope = await this.authorizationScopeService.GetAuthorizationScopeAsync(context, token).PreserveThreadContext();
                var authContext = new AuthorizationContext(context, names, types, authScope);
                await this.authorizationService.AuthorizeAsync(authContext, token).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Gets the required permissions in this collection.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        /// The required permissions.
        /// </returns>
        private (IReadOnlyList<string> names, IReadOnlyList<Type> types) GetRequiredPermissions(Type messageType)
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
        private (IReadOnlyList<string> names, IReadOnlyList<Type> types) ComputePermissions(Type messageType)
        {
            var permAttrs = messageType.GetTypeInfo().GetCustomAttributes<RequiresPermissionAttribute>();
            var namesSet = new HashSet<string>();
            var typesSet = new HashSet<Type>();
            foreach (var permAttr in permAttrs)
            {
                namesSet.AddRange(permAttr.Permissions);
                typesSet.AddRange(permAttr.PermissionTypes);
            }

            return (namesSet.ToList().AsReadOnly(), typesSet.ToList().AsReadOnly());
        }
    }
}