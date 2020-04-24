// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuitMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the quit message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ComponentModel.DataAnnotations;
using Kephas.Messaging;

namespace Kephas.Commands.Messaging.Endpoints
{
    /// <summary>
    /// A quit message.
    /// </summary>
    [DisplayInfo(Description = "Quits the application.")]
    public class QuitMessage : IMessage
    {
    }
}
