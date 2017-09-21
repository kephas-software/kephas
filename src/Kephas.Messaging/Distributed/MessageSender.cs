// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageSender.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the message sender class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    /// <summary>
    /// A message sender.
    /// </summary>
    public class MessageSender : IMessageSender
    {
        /// <summary>
        /// Gets or sets the identifier of the sender.
        /// </summary>
        /// <value>
        /// The identifier of the sender.
        /// </value>
        public object Id { get; set; }
    }
}