// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplyReceivedEventArgs.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ReplyReceivedEventArgs interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Additional information for reply received events.
    /// </summary>
    public class ReplyReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public IBrokeredMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public IContext Context { get; set; }
    }
}
