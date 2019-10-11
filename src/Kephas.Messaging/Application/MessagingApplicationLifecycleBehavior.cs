// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingApplicationLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Interaction;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Runtime;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A messaging application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class MessagingApplicationLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IMessageBroker messageBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingApplicationLifecycleBehavior"/>
        /// class.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        public MessagingApplicationLifecycleBehavior(IMessageBroker messageBroker)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));

            this.messageBroker = messageBroker;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task BeforeAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            RuntimeTypeInfo.RegisterFactory(new MessagingTypeInfoFactory());

            return ServiceHelper.InitializeAsync(this.messageBroker, appContext, cancellationToken);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task AfterAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task BeforeAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task AfterAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return ServiceHelper.FinalizeAsync(this.messageBroker, appContext, cancellationToken);
        }
    }
}