// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The "ping" message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The "ping" message.
    /// </summary>
    [Display(Description = "Sends a 'ping' message to the server.")]
    public class PingMessage : IMessage
    {
    }
}