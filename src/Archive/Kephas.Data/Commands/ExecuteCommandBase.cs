// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteCommandBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the execute command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Logging;

    /// <summary>
    /// Base class for command execution commands.
    /// </summary>
    public abstract class ExecuteCommandBase : DataCommandBase<IExecuteContext, IExecuteResult>, IExecuteCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteCommandBase"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        protected ExecuteCommandBase(ILogManager logManager)
            : base(logManager)
        {
        }
    }
}