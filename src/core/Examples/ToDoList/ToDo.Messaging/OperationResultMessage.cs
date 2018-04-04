// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   An operation result message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ToDo.Messaging
{
    using Kephas.Messaging;

    /// <summary>
    /// An operation result message.
    /// </summary>
    public class OperationResultMessage : IMessage
    {
         /// <summary>
         /// Gets or sets the result.
         /// </summary>
         /// <value>
         /// The result.
         /// </value>
         string Result { get; set; }
    }
}