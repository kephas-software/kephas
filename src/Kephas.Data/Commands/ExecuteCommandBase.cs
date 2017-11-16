// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteCommandBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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