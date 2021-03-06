// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransaction.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITransaction interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Transactions
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Interface for transaction.
    /// </summary>
    public interface ITransaction
    {
        /// <summary>
        /// Adds an undo command to the transaction log.
        /// </summary>
        /// <param name="undoCommand">The undo command.</param>
        /// <returns>
        /// This transaction.
        /// </returns>
        ITransaction AddCommand(UndoCommandBase undoCommand);

        /// <summary>
        /// Undoes the operations in the given context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the <see cref="IOperationResult"/>.
        /// </returns>
        Task<IOperationResult> RollbackAsync(IPluginContext context, CancellationToken cancellationToken = default);

#if NETSTANDARD2_0
#else
        /// <summary>
        /// Undoes the operations in the given context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An <see cref="IOperationResult"/>.
        /// </returns>
        IOperationResult Rollback(IPluginContext context)
        {
            return this.RollbackAsync(context).GetResultNonLocking();
        }
#endif
    }

#if NETSTANDARD2_0
    /// <summary>
    /// Interface for sync operations transaction.
    /// </summary>
    public interface ISyncTransaction
    {
        /// <summary>
        /// Undoes the operations in the given context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An <see cref="IOperationResult"/>.
        /// </returns>
        IOperationResult Rollback(IPluginContext context);
    }
#endif

    /// <summary>
    /// A transaction extensions.
    /// </summary>
    public static class TransactionExtensions
    {
#if NETSTANDARD2_0
        /// <summary>
        /// Undoes the operations in the given context.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An <see cref="IOperationResult"/>.
        /// </returns>
        public static IOperationResult Rollback(this ITransaction transaction, IPluginContext context)
        {
            if (transaction is ISyncTransaction syncTransaction)
            {
                return syncTransaction.Rollback(context);
            }

            return transaction.RollbackAsync(context).GetResultNonLocking();
        }
#endif

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <typeparam name="T">The transaction type.</typeparam>
        /// <param name="transaction">The transaction.</param>
        /// <param name="sourceFile">Source file.</param>
        /// <param name="targetFile">Target file.</param>
        /// <returns>
        /// An <see cref="IOperationResult"/>.
        /// </returns>
        public static T MoveFile<T>(this T transaction, string sourceFile, string targetFile)
            where T : class, ITransaction
        {
            Requires.NotNull(transaction, nameof(transaction));

            File.Move(sourceFile, targetFile);

            transaction.AddCommand(new MoveFileUndoCommand(sourceFile, targetFile));

            return transaction;
        }
    }
}
