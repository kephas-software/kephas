// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncDataCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISyncDataCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// Contract for synchronous data commands.
    /// </summary>
    public interface ISyncDataCommand
    {
        /// <summary>
        /// Executes the data command.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IDataCommandResult"/>.
        /// </returns>
        IDataCommandResult Execute(IDataOperationContext operationContext);
    }

    /// <summary>
    /// Generic contract for synchronous data commands.
    /// </summary>
    public interface ISyncDataCommand<in TOperationContext, out TResult> : ISyncDataCommand
        where TOperationContext : IDataOperationContext
        where TResult : IDataCommandResult
    {
        /// <summary>
        /// Executes the data command.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IDataCommandResult"/>.
        /// </returns>
        TResult Execute(TOperationContext operationContext);
    }
}