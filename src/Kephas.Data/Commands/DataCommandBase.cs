// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Activation;
    using Kephas.Data.Caching;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base implementation of a data command.
    /// </summary>
    /// <typeparam name="TOperationContext">Type of the operationContext.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public abstract class DataCommandBase<TOperationContext, TResult> : Loggable, IDataCommand<TOperationContext, TResult>
        where TOperationContext : IDataOperationContext
        where TResult : IOperationResult
    {
        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommandBase{TOperationContext,TResult}"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        protected DataCommandBase(ILogManager? logManager)
            : base(logManager)
        {
        }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IOperationResult"/>.
        /// </returns>
        public abstract Task<TResult> ExecuteAsync(TOperationContext operationContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IOperationResult"/>.
        /// </returns>
        async Task<IOperationResult> IDataCommand.ExecuteAsync(IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            if (!(operationContext is TOperationContext typedOperationContext))
            {
                // TODO localization
                throw new DataException($"{typeof(TOperationContext)} context expected, instead provided an {operationContext?.GetType()}.");
            }

            this.EnsureInitialized(operationContext);
            var result = await this.ExecuteAsync(typedOperationContext, cancellationToken).PreserveThreadContext();
            return result;
        }

        /// <summary>
        /// Executes the asynchronous operation.
        /// </summary>
        /// <exception cref="DataException">Thrown when a Data error condition occurs.</exception>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token .</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        async Task<object?> IOperation.ExecuteAsync(IContext? context, CancellationToken cancellationToken)
        {
            if (context is not TOperationContext typedOperationContext)
            {
                // TODO localization
                throw new DataException($"{typeof(TOperationContext)} context expected, instead provided an {context?.GetType()}.");
            }

            this.EnsureInitialized(context);
            var result = await this.ExecuteAsync(typedOperationContext, cancellationToken).PreserveThreadContext();
            return result;
        }

        /// <summary>
        /// Executes the operation.
        /// </summary>
        /// <exception cref="DataException">Thrown when a Data error condition occurs.</exception>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        object? IOperation.Execute(IContext? context)
        {
            if (context is not TOperationContext typedOperationContext)
            {
                // TODO localization
                throw new DataException($"{typeof(TOperationContext)} context expected, instead provided an {context?.GetType()}.");
            }

            this.EnsureInitialized(context);
            var result = this.Execute(typedOperationContext);
            return result;
        }

        /// <summary>
        /// Executes the data command.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IOperationResult"/>.
        /// </returns>
        public virtual TResult Execute(TOperationContext operationContext)
        {
            return this.ExecuteAsync(operationContext).GetResultNonLocking();
        }

        /// <summary>
        /// Tries to get the data context's local cache.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <returns>
        /// An IDataContextCache.
        /// </returns>
        protected virtual IDataContextCache? TryGetLocalCache(IDataContext dataContext)
        {
            return (dataContext as DataContextBase)?.LocalCache;
        }

        /// <summary>
        /// Tries to get the data context's entity activator.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <returns>
        /// An IActivator.
        /// </returns>
        protected virtual IActivator? TryGetEntityActivator(IDataContext dataContext)
        {
            return (dataContext as DataContextBase)?.EntityActivator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureInitialized(IContext context)
        {
            if (!this.isInitialized)
            {
                this.Logger = this.GetLogger(context);
                this.isInitialized = true;
            }
        }
    }
}