// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandNotFoundException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Exception signalling an unknown command.
    /// </summary>
    public class CommandNotFoundException : KeyNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandNotFoundException(string command)
        {
            this.Command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="message">The exception message.</param>
        public CommandNotFoundException(string command, string message)
            : base(message)
        {
            this.Command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public CommandNotFoundException(string command, string message, Exception inner)
            : base(message, inner)
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets the unknown command.
        /// </summary>
        public string Command { get; }
    }
}