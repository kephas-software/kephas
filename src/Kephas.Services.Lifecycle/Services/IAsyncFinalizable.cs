// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncFinalizable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAsyncFinalizable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the <see cref="FinalizeAsync"/> method for service asynchronous finalization.
    /// </summary>
    public interface IAsyncFinalizable
    {
        /// <summary>
        /// Finalizes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task FinalizeAsync(IContext? context = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Provides the <see cref="FinalizeAsync"/> method for service asynchronous finalization.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    public interface IAsyncFinalizable<in TContext> : IAsyncFinalizable
        where TContext : class, IContext
    {
        /// <summary>
        /// Finalizes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task FinalizeAsync(TContext? context = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finalizes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task IAsyncFinalizable.FinalizeAsync(IContext? context, CancellationToken cancellationToken)
        {
            var typedContext = context as TContext;
            if (typedContext == null && context != null)
            {
                throw new ArgumentException($"Expecting a context of type {typeof(TContext)}, instead received {context}.", nameof(context));
            }

            return this.FinalizeAsync(typedContext, cancellationToken);
        }
    }
}