// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOperation interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Defines the contract of an executable operation.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// Executes the operation in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// An object.
        /// </returns>
#if NETSTANDARD2_1
        object? Execute(IContext? context = null)
        {
            return this.ExecuteAsync(context).GetResultNonLocking();
        }
#else
        object? Execute(IContext? context = null);
#endif

#if NETSTANDARD2_1
        /// <summary>
        /// Executes the operation asynchronously in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An object.
        /// </returns>
        async Task<object?> ExecuteAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            return this.Execute(context);
        }
#endif
    }

#if NETSTANDARD2_1
#else
    /// <summary>
    /// Defines the contract of an executable asynchronous operation.
    /// </summary>
    public interface IAsyncOperation
    {
        /// <summary>
        /// Executes the operation asynchronously in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An object.
        /// </returns>
        Task<object?> ExecuteAsync(IContext? context = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Extension methods for <see cref="IOperation"/>.
    /// </summary>
    public static class OperationExtensions
    {
        /// <summary>
        /// Executes the operation asynchronously in the given context.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An object.
        /// </returns>
        public static async Task<object?> ExecuteAsync(
            this IOperation operation,
            IContext? context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(operation, nameof(operation));

            if (operation is IAsyncOperation asyncOperation)
            {
                return await asyncOperation.ExecuteAsync(context, cancellationToken).PreserveThreadContext();
            }

            await Task.Yield();

            return operation.Execute(context);
        }
    }
#endif
}