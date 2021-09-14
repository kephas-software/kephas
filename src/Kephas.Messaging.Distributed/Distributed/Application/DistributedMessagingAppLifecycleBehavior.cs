// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A distributed messaging application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class DistributedMessagingAppLifecycleBehavior : Loggable, IAppLifecycleBehavior
    {
        private readonly IConfiguration<DistributedMessagingSettings> messagingConfig;
        private readonly IMessageBroker messageBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedMessagingAppLifecycleBehavior"/>
        /// class.
        /// </summary>
        /// <param name="messagingConfig">The messaging configuration.</param>
        /// <param name="messageBroker">The message broker.</param>
        public DistributedMessagingAppLifecycleBehavior(IConfiguration<DistributedMessagingSettings> messagingConfig, IMessageBroker messageBroker)
        {
            Requires.NotNull(messagingConfig, nameof(messagingConfig));
            Requires.NotNull(messageBroker, nameof(messageBroker));

            this.messagingConfig = messagingConfig;
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
        public async Task<IOperationResult> BeforeAppInitializeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            this.InitializeConfig(appContext);

            await ServiceHelper.InitializeAsync(this.messageBroker, appContext, cancellationToken).PreserveThreadContext();
            return true.ToOperationResult();
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task<IOperationResult> AfterAppFinalizeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            await ServiceHelper.FinalizeAsync(this.messageBroker, appContext, cancellationToken).PreserveThreadContext();
            return true.ToOperationResult();
        }

        private void InitializeConfig(IContext? appContext)
        {
            try
            {
                var settings = this.messagingConfig.GetSettings(appContext);

                BrokeredMessage.DefaultTimeout = settings.DefaultTimeout ?? BrokeredMessage.DefaultTimeout;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while trying to set messaging default values.");
            }
        }
    }
}