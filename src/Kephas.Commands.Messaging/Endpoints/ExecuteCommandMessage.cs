// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteCommandMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Endpoints
{
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// Message for executing a command.
    /// </summary>
    /// <remarks>
    /// Do not implement <see cref="IMessage"/> so that it remains invisible to the registry.
    /// </remarks>
    public class ExecuteCommandMessage
    {
        /// <summary>
        /// Gets or sets the command to be executed.
        /// </summary>
        public string? Command { get; set; }

        /// <summary>
        /// Gets or sets the command arguments.
        /// </summary>
        public IArgs? Args { get; set; }
    }

    /// <summary>
    /// The response of a command execution.
    /// </summary>
    public class ExecuteCommandResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the execution return value.
        /// </summary>
        public object? ReturnValue { get; set; }
    }
}