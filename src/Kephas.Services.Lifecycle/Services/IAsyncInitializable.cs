// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncInitializable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides the <see cref="InitializeServiceAsync" /> method for asynchronous service initialization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the <see cref="InitializeAsync"/> method for asynchronous service initialization.
    /// </summary>
    public interface IAsyncInitializable
    {
        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task InitializeAsync(IContext? context = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Provides the <see cref="InitializeAsync"/> method for asynchronous service initialization.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    public interface IAsyncInitializable<in TContext> : IAsyncInitializable
        where TContext : class, IContext
    {
        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task InitializeAsync(TContext? context = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task IAsyncInitializable.InitializeAsync(IContext? context, CancellationToken cancellationToken)
        {
            var typedContext = context as TContext;
            if (typedContext == null && context != null)
            {
                throw new ArgumentException($"Expecting a context of type {typeof(TContext)}, instead received {context}.", nameof(context));
            }

            return this.InitializeAsync(typedContext, cancellationToken);
        }
    }
}