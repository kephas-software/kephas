// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessingContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for contexts when processing messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Contract for contexts when processing messages.
    /// </summary>
    public interface IMessageProcessingContext : IContext
    {
        /// <summary>
        /// Gets the message processor.
        /// </summary>
        /// <value>
        /// The message processor.
        /// </value>
        IMessageProcessor MessageProcessor { get; }

        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        IMessageHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        IMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        IMessage Response { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        Exception Exception { get; set; }
    }
}