// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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