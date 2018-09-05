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
    /// <summary>
    /// Base class for command execution commands.
    /// </summary>
    public abstract class ExecuteCommandBase : DataCommandBase<IExecuteContext, IExecuteResult>, IExecuteCommand
    {
    }
}