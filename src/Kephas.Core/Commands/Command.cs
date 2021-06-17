// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Command.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Represents a command to be executed.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        public Command(string commandLine)
        {
            Requires.NotNullOrEmpty(commandLine, nameof(commandLine));

            (this.Name, this.Args) = this.ParseCore(commandLine);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="args">The command arguments.</param>
        public Command(string name, IDynamic? args = null)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            this.Name = name;
            this.Args = args ?? new Args();
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the command arguments.
        /// </summary>
        public IDynamic Args { get; }

        /// <summary>
        /// Parses the command from the provided command line and returns it.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        /// <returns>The parsed command.</returns>
        public static Command Parse(string commandLine)
        {
            return new (commandLine);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.Name} {this.Args.AsArgs()}";
        }

        /// <summary>
        /// Parses the command line and returns the parts.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        /// <returns>The command name and arguments.</returns>
        protected virtual (string command, IDynamic args) ParseCore(string commandLine)
        {
            var commandIndex = commandLine.IndexOf(" ");
            var name = commandIndex <= 0 ? commandLine : commandLine.Substring(0, commandIndex);
            commandLine = commandIndex <= 0 ? string.Empty : commandLine.Substring(commandIndex).Trim();
            var commandArgs = new Args(commandLine);
            return (name, commandArgs);
        }
    }
}