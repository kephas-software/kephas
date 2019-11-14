// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBrokeredMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageEnvelope interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Generic;

    using Kephas.Data;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Messages;
    using Kephas.Services;

    /// <summary>
    /// Contract interface for brokered messages.
    /// </summary>
    public interface IBrokeredMessage : IIdentifiable, IMessageEnvelope, IIndexable
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        new string Id { get; }

        /// <summary>
        /// Gets or sets the sender of the message.
        /// </summary>
        /// <value>
        /// The message sender.
        /// </value>
        IEndpoint Sender { get; set; }

        /// <summary>
        /// Gets or sets the message to send.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        IMessage Content { get; set; }

        /// <summary>
        /// Gets or sets the recipients.
        /// </summary>
        /// <value>
        /// The recipients.
        /// </value>
        IEnumerable<IEndpoint> Recipients { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this message is one way.
        /// </summary>
        /// <value>
        /// True if this message is one way, false if not.
        /// </value>
        bool IsOneWay { get; set; }

        /// <summary>
        /// Gets or sets the timeout when waiting for responses.
        /// </summary>
        /// <remarks>
        /// A value of <c>null</c> means indefinitely waiting, but
        /// it is strongly discouraged to wait indefinitely for a response.
        /// The value <see cref="BrokeredMessage.DefaultTimeout"/> is used by default.
        /// </remarks>
        /// <value>
        /// The response timeout.
        /// </value>
        TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the message to reply to.
        /// </summary>
        /// <value>
        /// The identifier of the reply to message.
        /// </value>
        string ReplyToMessageId { get; set; }

        /// <summary>
        /// Gets or sets the bearer token.
        /// </summary>
        /// <value>
        /// The bearer token.
        /// </value>
        string BearerToken { get; set; }

        /// <summary>
        /// Gets or sets the custom properties of the brokered message.
        /// </summary>
        /// <value>
        /// The custom properties.
        /// </value>
        IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        Priority Priority { get; set; }

        /// <summary>
        /// Gets or sets the trace.
        /// </summary>
        /// <value>
        /// The trace.
        /// </value>
        string Trace { get; set; }

        /// <summary>
        /// Makes a deep copy of this object, optionally replacing the existing recipients with the provided ones.
        /// </summary>
        /// <param name="recipients">Optional. The recipients.</param>
        /// <returns>
        /// A copy of this object.
        /// </returns>
        IBrokeredMessage Clone(IEnumerable<IEndpoint> recipients = null);
    }

    /// <summary>
    /// Extension methods for <see cref="IBrokeredMessage"/>.
    /// </summary>
    public static class BrokeredMessageExtensions
    {
        /// <summary>
        /// Adds an input trace from the provided router.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="router">The router.</param>
        /// <returns>
        /// This <paramref name="brokeredMessage"/>.
        /// </returns>
        public static IBrokeredMessage TraceInputRoute(this IBrokeredMessage brokeredMessage, IMessageRouter router)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));

            brokeredMessage.Trace = $"in:{router?.GetType().Name}:{DateTime.UtcNow:s}{Environment.NewLine}{brokeredMessage.Trace}";

            return brokeredMessage;
        }

        /// <summary>
        /// Adds an output trace from the provided router.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="router">The router.</param>
        /// <returns>
        /// This <paramref name="brokeredMessage"/>.
        /// </returns>
        public static IBrokeredMessage TraceOutputRoute(this IBrokeredMessage brokeredMessage, IMessageRouter router)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));

            brokeredMessage.Trace = $"out:{router?.GetType().Name}:{DateTime.UtcNow:s}{Environment.NewLine}{brokeredMessage.Trace}";

            return brokeredMessage;
        }

        /// <summary>
        /// Adds a reply trace.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="requestTrace">The request trace.</param>
        /// <returns>
        /// This <paramref name="brokeredMessage"/>.
        /// </returns>
        public static IBrokeredMessage TraceReply(this IBrokeredMessage brokeredMessage, string requestTrace)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));

            brokeredMessage.Trace = $"reply:{DateTime.UtcNow:s}{Environment.NewLine}{requestTrace}";

            return brokeredMessage;
        }
    }
}