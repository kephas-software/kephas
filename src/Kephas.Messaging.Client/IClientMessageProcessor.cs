﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientMessageProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for clients of message processors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Client
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging;
    using Kephas.Services;

    /// <summary>
    /// Application service for clients of message processors.
    /// </summary>
    /// <remarks>
    /// The client message processor is defined as a shared service.
    /// </remarks>
    [SharedAppServiceContract]
    public interface IClientMessageProcessor
    {
        /// <summary>
        /// Processes the specified message asynchronously by sending it to the server and waiting for a response.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        Task<IMessage> ProcessAsync(IMessage message, CancellationToken token = default);

        /// <summary>
        /// Processes the specified message asynchronously by sending it to the server without waiting for a response.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessOneWay(IMessage message);
    }
}