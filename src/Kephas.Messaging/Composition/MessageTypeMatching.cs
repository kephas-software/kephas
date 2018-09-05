// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageTypeMatching.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message type handling class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Composition
{
    /// <summary>
    /// Values that represent how the message type matches the handler.
    /// </summary>
    public enum MessageTypeMatching
    {
        /// <summary>
        /// The handler can handle messages of the indicated type.
        /// </summary>
        Type,

        /// <summary>
        /// The handler can handle messages of the indicated type or any derived type.
        /// </summary>
        TypeOrHierarchy,
    }
}