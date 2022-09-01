﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.ExceptionHandling;
    using Kephas.Resources;

    /// <summary>
    /// Extensions for <see cref="IOperationResult"/>.
    /// </summary>
    public static class OperationResultExtensions
    {
        /// <summary>
        /// Sets the return value to the provided one.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="value">The return value.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult Value<TResult>([DisallowNull] this TResult result, object? value)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            result.Value = value;

            return result;
        }

        /// <summary>
        /// Sets the return value to the provided one.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <typeparam name="TValue">Type of the return value.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="value">The return value.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult Value<TResult, TValue>([DisallowNull] this TResult result, TValue value)
            where TResult : class, IOperationResult<TValue>
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            result.Value = value;

            return result;
        }

        /// <summary>
        /// Marks the operation as completed setting the elapsed time and, optionally, the state.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="elapsed">Optional. The elapsed time.</param>
        /// <param name="operationState">Optional. State of the operation.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult Complete<TResult>([DisallowNull] this TResult result, TimeSpan? elapsed = null, OperationState? operationState = null)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            result.Elapsed = result.ComputeElapsed(elapsed);
            result.PercentCompleted = 1;
            result.OperationState = operationState ?? Kephas.Operations.OperationState.Completed;

            return result;
        }

        /// <summary>
        /// Marks the operation as completed setting the elapsed time and, optionally, the state.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception generating the failure.</param>
        /// <param name="elapsed">The elapsed time.</param>
        /// <param name="operationState">Optional. State of the operation.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult Fail<TResult>([DisallowNull] this TResult result, Exception exception, TimeSpan? elapsed = null, OperationState? operationState = null)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));
            exception = exception ?? throw new ArgumentNullException(nameof(exception));

            result.Elapsed = result.ComputeElapsed(elapsed);
            result.OperationState = operationState ?? GetOperationState(exception);
            result.MergeException(exception);

            return result;
        }

        /// <summary>
        /// Sets the operation state to the provided one.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="state">The operation state.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult OperationState<TResult>([DisallowNull] this TResult result, OperationState state)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            result.OperationState = state;

            return result;
        }

        /// <summary>
        /// Merges the exception.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult MergeException<TResult>([DisallowNull] this TResult result, Exception ex)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));
            ex = ex ?? throw new ArgumentNullException(nameof(ex));

            result.Messages.Add(new OperationMessage(ex));

            return result;
        }

        /// <summary>
        /// Merges the exception.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult MergeMessage<TResult>([DisallowNull] this TResult result, string message)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));
            message = message ?? throw new ArgumentNullException(nameof(message));

            result.Messages.Add(new OperationMessage(message));

            return result;
        }

        /// <summary>
        /// Merges the exception.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult MergeMessage<TResult>([DisallowNull] this TResult result, IOperationMessage message)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));
            message = message ?? throw new ArgumentNullException(nameof(message));

            result.Messages.Add(message);

            return result;
        }

        /// <summary>
        /// Merges all messages and exceptions.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="resultToMerge">The result to merge.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult MergeMessages<TResult>([DisallowNull] this TResult result, IOperationResult? resultToMerge)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            if (resultToMerge == null)
            {
                return result;
            }

            result.Messages.AddRange(resultToMerge.Messages);

            return result;
        }

        /// <summary>
        /// Merges all messages and exceptions.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="asyncResult">The task of which result will be merged.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult MergeMessages<TResult>([DisallowNull] this TResult result, Task<IOperationResult> asyncResult)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));
            asyncResult = asyncResult ?? throw new ArgumentNullException(nameof(asyncResult));

            if (!asyncResult.IsCompleted && !asyncResult.IsCanceled && !asyncResult.IsFaulted)
            {
                throw new InvalidOperationException(OperationsStrings.OperationResult_Merge_TaskNotCompleteException);
            }

            return asyncResult.Exception == null
                    ? MergeMessages(result, asyncResult.Result)
                    : MergeException(result, asyncResult.Exception);
        }

        /// <summary>
        /// Merges all messages, exceptions, the elapsed time, and the return value.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="resultToMerge">The result to merge.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        [return: NotNull]
        public static TResult MergeAll<TResult>([DisallowNull] this TResult result, IOperationResult? resultToMerge)
            where TResult : class, IOperationResult
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            if (resultToMerge == null)
            {
                return result;
            }

            result.Messages.AddRange(resultToMerge.Messages);
            result.Value = resultToMerge.Value;
            result.Elapsed += resultToMerge.Elapsed;

            return result;
        }

        /// <summary>
        /// Indicates whether the result has errors.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// <c>true</c> if the result contains errors, <c>false</c> otherwise.
        /// </returns>
        public static bool HasErrors(this IOperationResult result) => result.Messages.Any(m => m.IsError());

        /// <summary>
        /// Indicates whether the result has warnings.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// <c>true</c> if the result contains warnings, <c>false</c> otherwise.
        /// </returns>
        public static bool HasWarnings(this IOperationResult result) => result.Messages.Any(m => m.IsWarning());

        /// <summary>
        /// Gets an enumeration of errors.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// An enumeration of errors.
        /// </returns>
        public static IEnumerable<IOperationMessage> Errors(this IOperationResult result)
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            return result.Messages.Where(m => m.IsError());
        }

        /// <summary>
        /// Gets an enumeration of warnings.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// An enumeration of warnings.
        /// </returns>
        public static IEnumerable<IOperationMessage> Warnings(this IOperationResult result)
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            return result.Messages.Where(m => m.IsWarning());
        }

        /// <summary>
        /// Gets an enumeration of information messages.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// An enumeration of information messages.
        /// </returns>
        public static IEnumerable<IOperationMessage> Infos(this IOperationResult result)
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            return result.Messages.Where(m => !m.IsWarning() && !m.IsError());
        }

        /// <summary>
        /// Tries to get an exception from the result, if the result has errors.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>An exception or <c>null</c>.</returns>
        public static Exception? TryGetException(this IOperationResult result)
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            var errors = result.Messages.Where(m => m.IsError()).ToList();

            return errors.Count switch
            {
                0 => null,
                1 => ToException(errors[0]),
                _ => new AggregateException(errors.Select(ToException))
            };
        }

        /// <summary>
        /// Throws an exception if the result has errors.
        /// </summary>
        /// <param name="result">The operation result.</param>
        public static void ThrowIfHasErrors(this IOperationResult result)
        {
            var exception = TryGetException(result);

            if (exception is not null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Converts the provided value to a new operation result and completes it as successful.
        /// </summary>
        /// <param name="value">The result value.</param>
        /// <param name="elapsed">Optional. The elapsed time. If not provided will be set to zero.</param>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>The completed operation result.</returns>
        public static IOperationResult<TValue> ToOperationResult<TValue>(this TValue value, TimeSpan? elapsed = null)
        {
            return new OperationResult<TValue>(value)
                .Complete(elapsed ?? TimeSpan.Zero, Operations.OperationState.Completed);
        }

        /// <summary>
        /// Converts the provided exception to a new operation result and completes it
        /// either as failed, canceled, timed out, or aborted, depending on the exception type.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="elapsed">Optional. The elapsed time. If not provided will be set to zero.</param>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>The failed operation result.</returns>
        public static IOperationResult<TValue> ToOperationResult<TValue>(this Exception exception, TimeSpan? elapsed = null)
        {
            exception = exception ?? throw new ArgumentNullException(nameof(exception));

            var state = GetOperationState(exception);
            return new OperationResult<TValue>()
                .MergeException(exception)
                .Complete(elapsed ?? TimeSpan.Zero, state);
        }

        /// <summary>
        /// Converts the provided exception to a new operation result and completes it as failed.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="elapsed">Optional. The elapsed time. If not provided will be set to zero.</param>
        /// <returns>The failed operation result.</returns>
        public static IOperationResult ToOperationResult(Exception exception, TimeSpan? elapsed = null)
        {
            return ToOperationResult<object>(exception, elapsed);
        }

        /// <summary>
        /// Flattens the operation result by aggregating the elapsed time
        /// and messages into the nested one and returning it.
        /// </summary>
        /// <param name="aggregatedResult">The aggregated result.</param>
        /// <param name="throwOnNull">Optional. If set to true throws and exception if the nested result is <c>null</c>.</param>
        /// <typeparam name="TOperationResult">The operation result ype.</typeparam>
        /// <returns>The nested result with aggregated elapsed time and messages.</returns>
        public static TOperationResult? Flatten<TOperationResult>(
            this IOperationResult<TOperationResult> aggregatedResult,
            bool throwOnNull = true)
            where TOperationResult : class, IOperationResult
        {
            aggregatedResult = aggregatedResult ?? throw new ArgumentNullException(nameof(aggregatedResult));

            var nestedResult = aggregatedResult.Value;
            if (nestedResult == null)
            {
                return throwOnNull
                    ? throw new ArgumentNullException(nameof(aggregatedResult), "The nested result is not set.")
                    : nestedResult;
            }

            return nestedResult
                .MergeMessages(aggregatedResult)
                .Complete(
                    nestedResult.Elapsed == TimeSpan.Zero
                        ? aggregatedResult.Elapsed
                        : aggregatedResult.Elapsed == TimeSpan.Zero
                            ? nestedResult.Elapsed
                            : aggregatedResult.Elapsed,
                    nestedResult.OperationState == Operations.OperationState.NotStarted
                        ? aggregatedResult.OperationState
                        : nestedResult.OperationState);
        }

        private static OperationState GetOperationState(Exception exception)
        {
            var state = exception switch
            {
                OperationCanceledException _ => Operations.OperationState.Canceled,
                TimeoutException _ => Operations.OperationState.TimedOut,
                OperationAbortedException _ => Operations.OperationState.Aborted,
                _ => Operations.OperationState.Failed
            };
            return state;
        }

        private static TimeSpan ComputeElapsed<TOperationResult>(this TOperationResult result, TimeSpan? elapsed)
            where TOperationResult : class, IOperationResult
        {
            if (elapsed != null)
            {
                return elapsed.Value;
            }

            var endedAt = result.EndedAt ?? DateTimeOffset.Now;
            return endedAt - result.StartedAt ?? TimeSpan.Zero;
        }

        private static Exception ToException(this IOperationMessage m)
            => m.Exception is not null ? m.Exception! : new OperationException(m.Message);
    }
}