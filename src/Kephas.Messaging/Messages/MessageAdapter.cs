// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    using Kephas.Messaging.Resources;
    using System;

    /// <summary>
    /// A message adapter.
    /// </summary>
    public class MessageAdapter : IMessageAdapter
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public object Message { get; set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the message is not set.</exception>
        /// <returns>
        /// The message.
        /// </returns>
        object IMessageAdapter.GetMessage()
        {
            if (this.Message == null)
            {
                throw new InvalidOperationException(Strings.MessageAdapter_MessageNotSet_Exception);
            }

            return this.Message;
        }
    }
}
