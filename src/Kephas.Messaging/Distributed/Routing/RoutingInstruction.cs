// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingInstruction.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the routing instruction class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    /// <summary>
    /// Values that represent routing instructions.
    /// </summary>
    public enum RoutingInstruction
    {
        /// <summary>
        /// No action to take.
        /// </summary>
        None,

        /// <summary>
        /// The broker should dispatch the reply.
        /// </summary>
        Reply,
    }
}
