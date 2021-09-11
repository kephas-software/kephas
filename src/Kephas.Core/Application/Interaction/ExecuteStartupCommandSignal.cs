// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteStartupCommandSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using Kephas.Interaction;
    using Kephas.Services;

    /// <summary>
    /// Signal for executing a startup command.
    /// </summary>
    public class ExecuteStartupCommandSignal : SignalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteStartupCommandSignal"/> class.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <param name="executionContext">The execution context.</param>
        public ExecuteStartupCommandSignal(object command, IContext executionContext)
            : base("Execute command.")
        {
            this.Command = command;
            this.ExecutionContext = executionContext;
        }

        /// <summary>
        /// Gets the command object.
        /// </summary>
        public object Command { get; }

        /// <summary>
        /// Gets the execution context.
        /// </summary>
        public IContext ExecutionContext { get; }
    }
}