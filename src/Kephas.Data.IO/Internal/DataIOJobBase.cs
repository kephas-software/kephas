// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOJobBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Internal
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;

    /// <summary>
    /// Base class for data I/O jobs.
    /// </summary>
    internal abstract class DataIOJobBase<TContext>
        where TContext : IDataIOContext
    {
        private TaskCompletionSource<IOperationResult> taskCompletionSource;
        private Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOJobBase{TContext}"/> class.
        /// </summary>
        /// <param name="context">The operation context.</param>
        protected DataIOJobBase(TContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// The operation context.
        /// </summary>
        protected TContext Context { get; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        protected IAsyncOperationResult Result { get; private set; }

        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The operation result.</returns>
        public IAsyncOperationResult ExecuteAsync(CancellationToken cancellationToken)
        {
            this.stopwatch = Stopwatch.StartNew();
            this.taskCompletionSource = new TaskCompletionSource<IOperationResult>();
            this.Result = new AsyncOperationResult(this.taskCompletionSource.Task)
            {
                OperationState = OperationState.InProgress,
            };
            this.Context.EnsureResult(() => this.Result);

            try
            {
                var task = this.ExecuteCoreAsync(cancellationToken);
                task.ContinueWith(t =>
                {
                    this.stopwatch.Stop();

                    if (t.IsFaulted)
                    {
                        var exception = t.Exception.InnerExceptions.Count == 1
                            ? t.Exception.InnerException
                            : t.Exception;

                        this.Result.Fail(exception, this.stopwatch.Elapsed);
                        this.taskCompletionSource.TrySetException(exception);
                    }
                    else if (t.IsCanceled)
                    {
                        this.Result.Complete(this.stopwatch.Elapsed, OperationState.Canceled);
                        this.taskCompletionSource.TrySetCanceled();
                    }
                    else
                    {
                        this.Result.Complete(this.stopwatch.Elapsed);
                        this.taskCompletionSource.TrySetResult(t.Result);
                    }

                    this.Context?.Dispose();
                });
            }
            catch (Exception ex)
            {
                this.stopwatch.Stop();
                this.Result.Fail(ex, this.stopwatch.Elapsed);
                this.taskCompletionSource.TrySetException(ex);
                this.Context?.Dispose();
            }

            return this.Result;
        }

        /// <summary>
        /// Executes the I/O job (core implementation).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An operation result.
        /// </returns>
        protected abstract Task<IOperationResult> ExecuteCoreAsync(CancellationToken cancellationToken);
    }
}