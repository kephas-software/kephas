// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBusMessageBroker.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service bus message broker class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Azure.ServiceBus
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Messaging.Distributed;
    using Kephas.Security;
    using Kephas.Services;

    /// <summary>
    /// A service bus message broker.
    /// </summary>
    public class ServiceBusMessageBroker : MessageBrokerBase, IAsyncInitializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusMessageBroker"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="securityService">The security service.</param>
        public ServiceBusMessageBroker(IAppManifest appManifest, ISecurityService securityService)
            : base(appManifest, securityService)
        {
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        protected override async Task SendAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public async Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}