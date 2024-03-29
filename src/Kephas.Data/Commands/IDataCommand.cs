﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Contract for data commands.
    /// </summary>
    public interface IDataCommand : IOperation
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IOperationResult"/>.
        /// </returns>
        Task<IOperationResult> ExecuteAsync(IDataOperationContext operationContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the data command.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IOperationResult"/>.
        /// </returns>
        IOperationResult Execute(IDataOperationContext operationContext)
        {
            return this.ExecuteAsync(operationContext).GetResultNonLocking();
        }
    }

    /// <summary>
    /// Contract for data commands, with typed operationContext and result.
    /// </summary>
    /// <typeparam name="TOperationContext">Type of the operation context.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public interface IDataCommand<in TOperationContext, TResult> : IDataCommand
        where TOperationContext : IDataOperationContext
        where TResult : IOperationResult
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IOperationResult"/>.
        /// </returns>
        Task<TResult> ExecuteAsync(TOperationContext operationContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the data command.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IOperationResult"/>.
        /// </returns>
        TResult Execute(TOperationContext operationContext)
        {
            return this.ExecuteAsync(operationContext).GetResultNonLocking();
        }
    }
}