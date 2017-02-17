// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Composition metadata for <see cref="IMessageHandler" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Server.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// Composition metadata for <see cref="IMessageHandler"/>.
    /// </summary>
    public class MessageHandlerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MessageHandlerMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.MessageType = (Type)metadata.TryGetValue(nameof(this.MessageType), null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerMetadata" /> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="overridePriority">The override priority.</param>
        public MessageHandlerMetadata(Type messageType, int overridePriority = 0)
            : base(0, overridePriority)
        {
            this.MessageType = messageType;
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public Type MessageType { get; }
    }
}