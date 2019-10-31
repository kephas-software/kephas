// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for contexts when processing messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Contract for contexts when processing messages.
    /// </summary>
    public interface IMessagingContext : IContext
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

    /// <summary>
    /// Extension methods for <see cref="IMessagingContext"/>.
    /// </summary>
    public static class MessagingContextExtensions
    {
        /// <summary>
        /// Sets the message handler.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The messaging context.</param>
        /// <param name="handler">The message handler.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Handler<TContext>(
            this TContext context,
            IMessageHandler handler)
            where TContext : class, IMessagingContext
        {
            Requires.NotNull(context, nameof(context));

            context.Handler = handler;

            return context;
        }

        /// <summary>
        /// Sets the processing message.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The messaging context.</param>
        /// <param name="message">The processing message.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Message<TContext>(
            this TContext context,
            IMessage message)
            where TContext : class, IMessagingContext
        {
            Requires.NotNull(context, nameof(context));

            context.Message = message;

            return context;
        }

        /// <summary>
        /// Sets the response message.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The messaging context.</param>
        /// <param name="response">The response message.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Response<TContext>(
            this TContext context,
            IMessage response)
            where TContext : class, IMessagingContext
        {
            Requires.NotNull(context, nameof(context));

            context.Response = response;

            return context;
        }

        /// <summary>
        /// Sets the response exception.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The messaging context.</param>
        /// <param name="exception">The response exception.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Exception<TContext>(
            this TContext context,
            Exception exception)
            where TContext : class, IMessagingContext
        {
            Requires.NotNull(context, nameof(context));

            context.Exception = exception;

            return context;
        }
    }
}