// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExecuteResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IExecuteResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// Interface for execute result.
    /// </summary>
    public interface IExecuteResult : IDataCommandResult
    {
        /// <summary>
        /// Gets the execution result.
        /// </summary>
        /// <value>
        /// The execution result.
        /// </value>
        object Result { get; }
    }
}