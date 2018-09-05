// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageIdMatching.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message ID matching class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Composition
{
    /// <summary>
    /// Values that represent how the message ID matches the handler,
    /// additionally to the message type.
    /// </summary>
    public enum MessageIdMatching
    {
        /// <summary>
        /// The handler can handle messages with the indicated ID.
        /// </summary>
        Id,

        /// <summary>
        /// The handler can handle all messages, independent of the ID.
        /// </summary>
        All,
    }
}