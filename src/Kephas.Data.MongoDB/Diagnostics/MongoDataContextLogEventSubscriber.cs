// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDataContextLogEventSubscriber.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo data context log event subscriber class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Diagnostics
{
    using System;
    using System.Reflection;
    using System.Text;

    using Kephas.Logging;

    using global::MongoDB.Bson;
    using global::MongoDB.Driver.Core.Events;

    using Kephas.Composition;

    /// <summary>
    /// An <see cref="IEventSubscriber"/> for <see cref="MongoDataContext"/>.
    /// </summary>
    internal class MongoDataContextLogEventSubscriber : IEventSubscriber
    {
        /// <summary>
        /// The logger prefix.
        /// </summary>
        private const string LoggerPrefix = "MongoDB Driver: ";

        /// <summary>
        /// Limit for a duration to be marked as warning.
        /// </summary>
        private static readonly TimeSpan WarningLimitDuration = TimeSpan.FromMilliseconds(300);

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The event subscriber.
        /// </summary>
        private readonly IEventSubscriber subscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDataContextLogEventSubscriber" /> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        public MongoDataContextLogEventSubscriber(ICompositionContext compositionContext)
        {
            this.logger = compositionContext.GetExport<ILogManager>().GetLogger<MongoDataContextLogEventSubscriber>();
            this.subscriber = new ReflectionEventSubscriber(this, nameof(this.Handle), BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Attempts to get event handler from the given data.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
        {
            return this.subscriber.TryGetEventHandler(out handler);
        }

        private void Handle(CommandFailedEvent driverEvent)
        {
            this.logger.Error(
              "{0}|{1}|ConnectionId:{2}|RequestId:{3}|Elapsed:{4:c}|{5}",
              LoggerPrefix,
              driverEvent.GetType().Name,
              driverEvent.ConnectionId,
              driverEvent.RequestId,
              driverEvent.Duration,
              this.GetExceptionContent(driverEvent.Failure));
        }

        private void Handle(CommandStartedEvent driverEvent)
        {
        }

        private void Handle(CommandSucceededEvent driverEvent)
        {
            if (driverEvent.Duration > WarningLimitDuration)
            {
                this.logger.Warn(
                  "{0}|{1}|ConnectionId:{2}|RequestId:{3}|Elapsed:{4:c}|Elapsed (network):{5}|Elapsed (deserialization):{6}|Cmd:{7}|Cursor:{8}",
                  LoggerPrefix,
                  driverEvent.GetType().Name,
                  driverEvent.ConnectionId,
                  driverEvent.RequestId,
                  driverEvent.Duration,
                  string.Empty,
                  string.Empty,
                  driverEvent.CommandName,
                  this.GetCursorInfo(driverEvent.Reply));
            }
        }

        /// <summary>
        /// Gets cursor information.
        /// </summary>
        /// <param name="driverEventReply">The driver event reply.</param>
        /// <returns>
        /// The cursor information.
        /// </returns>
        private string GetCursorInfo(BsonDocument driverEventReply)
        {
            var info = new StringBuilder();
            var cursor = default(BsonElement);
            if (driverEventReply?.TryGetElement("cursor", out cursor) ?? false)
            {
                if (cursor.Value.IsBsonDocument)
                {
                    var ns = default(BsonElement);
                    if (cursor.Value.AsBsonDocument.TryGetElement("ns", out ns))
                    {
                        info.Append(ns.Value);
                    }
                }
            }

            return info.ToString();
        }

        private void Handle(ConnectionPoolClosedEvent driverEvent)
        {
        }

        private void Handle(ConnectionPoolOpenedEvent driverEvent)
        {
        }

        private void Handle(ConnectionPoolAddedConnectionEvent driverEvent)
        {
        }

        private void Handle(ConnectionPoolRemovedConnectionEvent driverEvent)
        {
        }

        private void Handle(ConnectionPoolCheckingOutConnectionEvent driverEvent)
        {
        }

        private void Handle(ConnectionPoolCheckedOutConnectionEvent driverEvent)
        {
        }

        private void Handle(ConnectionPoolCheckedInConnectionEvent driverEvent)
        {
        }

        private void Handle(ConnectionClosedEvent driverEvent)
        {
        }

        private void Handle(ConnectionOpenedEvent driverEvent)
        {
        }

        private void Handle(ConnectionReceivedMessageEvent driverEvent)
        {
            if (driverEvent.Duration > WarningLimitDuration)
            {
                this.logger.Warn(
                  "{0}|{1}|ConnectionId:{2}|RequestId:{3}|Elapsed:{4:c}|Elapsed (network):{5:c}|Elapsed (deserialization):{6:c}",
                  LoggerPrefix,
                  driverEvent.GetType().Name,
                  driverEvent.ConnectionId,
                  driverEvent.ResponseTo,
                  driverEvent.Duration,
                  driverEvent.NetworkDuration,
                  driverEvent.DeserializationDuration);
            }
        }

        private void Handle(ConnectionSendingMessagesFailedEvent driverEvent)
        {
            this.logger.Error("{0}|{1}|ConnectionId:{2}|{3}", LoggerPrefix, driverEvent.GetType().Name, driverEvent.ConnectionId, this.GetExceptionContent(driverEvent.Exception));
        }

        private void Handle(ConnectionSentMessagesEvent driverEvent)
        {
        }

        /// <summary>
        /// Gets the content of the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The exception content as string.</returns>
        private string GetExceptionContent(Exception ex)
        {
            if (ex == null)
            {
                return null;
            }

            var escapedMessage = ex.Message.Replace("'", @"""");
            var content = $"{{$type:'{ex.GetType().FullName}',message:'{escapedMessage}'}}";

            return content;
        }
    }
}