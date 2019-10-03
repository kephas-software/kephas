// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the help message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// A help message.
    /// </summary>
    [TypeDisplay(Description = "Displays the available commands.")]
    public class HelpMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        [Display(Name = "The command for which the help content should be provided.")]
        public string Command { get; set; }
    }

    /// <summary>
    /// A help response message.
    /// </summary>
    public class HelpResponseMessage : IResponse
    {
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public object Command { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public string[] Parameters { get; set; }
    }
}