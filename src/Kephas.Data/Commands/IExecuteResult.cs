// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExecuteResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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