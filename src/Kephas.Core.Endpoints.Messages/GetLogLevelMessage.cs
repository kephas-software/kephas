// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLogLevelMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get log level message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// A get log level message.
    /// </summary>
    [DisplayInfo(Description = "Gets the application minimum log level.")]
    public class GetLogLevelMessage : IMessage
    {
    }
}