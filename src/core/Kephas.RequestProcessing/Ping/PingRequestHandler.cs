// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingRequestHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Request handler for the <see cref="PingRequest" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Ping
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.RequestProcessing.Server;

    /// <summary>
    /// Request handler for the <see cref="PingRequest"/>.
    /// </summary>
    public class PingRequestHandler : RequestHandlerBase<PingRequest, PingBackResponse>
    {
        /// <summary>
        /// Processes the provided request asynchronously and returns a response promise.
        /// </summary>
        /// <param name="request">The request to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override Task<PingBackResponse> ProcessAsync(PingRequest request, IProcessingContext context, CancellationToken token)
        {
            return Task.FromResult(new PingBackResponse { ServerTime = DateTimeOffset.Now });
        }
    }
}