// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Messaging.Runtime;
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// A messaging application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class MessagingAppLifecycleBehavior : Loggable, IAppLifecycleBehavior
    {
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingAppLifecycleBehavior"/>
        /// class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        public MessagingAppLifecycleBehavior(IRuntimeTypeRegistry typeRegistry)
        {
            this.typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> BeforeAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            this.typeRegistry.RegisterFactory(new MessagingTypeInfoFactory());
            return Task.FromResult<IOperationResult>(true.ToOperationResult());
        }
    }
}