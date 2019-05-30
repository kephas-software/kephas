// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageBuilderMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the brokered message builder metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// A brokered message builder metadata.
    /// </summary>
    public class BrokeredMessageBuilderMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageBuilderMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public BrokeredMessageBuilderMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.MessageType = (Type)metadata.TryGetValue(nameof(this.MessageType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageBuilderMetadata"/> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        public BrokeredMessageBuilderMetadata(Type messageType, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
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