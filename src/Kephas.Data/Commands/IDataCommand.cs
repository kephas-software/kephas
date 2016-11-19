// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for data commands.
    /// </summary>
    public interface IDataCommand
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        Task<IDataCommandResult> ExecuteAsync(IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Contract for data commands, with typed operationContext and result.
    /// </summary>
    /// <typeparam name="TContext">Type of the operationContext.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public interface IDataCommand<in TContext, TResult> : IDataCommand
        where TContext : IDataOperationContext
        where TResult : IDataCommandResult
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="context">The operationContext.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        Task<TResult> ExecuteAsync(TContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}