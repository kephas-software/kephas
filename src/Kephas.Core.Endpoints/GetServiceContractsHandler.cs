// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServiceContractsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging;
    using Kephas.Services.Composition;

    /// <summary>
    /// Message handler for <see cref="GetServiceContractsMessage"/>.
    /// </summary>
    public class GetServiceContractsHandler : MessageHandlerBase<GetServiceContractsMessage, GetServiceContractsResponseMessage>
    {
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetServiceContractsHandler"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public GetServiceContractsHandler(IAmbientServices ambientServices)
        {
            this.ambientServices = ambientServices;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override Task<GetServiceContractsResponseMessage> ProcessAsync(GetServiceContractsMessage message, IMessagingContext context, CancellationToken token)
        {
            var appServiceInfos = this.ambientServices
                .GetAppServiceInfos()
                .Select(i => i.appServiceInfo);

            if (!string.IsNullOrEmpty(message.ContractType))
            {
                appServiceInfos = appServiceInfos.Where(i => i.ContractType.Name.Contains(message.ContractType));
            }

            if (message.AllowMultiple != null)
            {
                appServiceInfos = appServiceInfos.Where(i => i.AllowMultiple == message.AllowMultiple.Value);
            }

            if (message.AsOpenGeneric != null)
            {
                appServiceInfos = appServiceInfos.Where(i => i.AsOpenGeneric == message.AsOpenGeneric);
            }

            return Task.FromResult(new GetServiceContractsResponseMessage
            {
                ServiceInfos = appServiceInfos.ToArray(),
            });
        }
    }
}