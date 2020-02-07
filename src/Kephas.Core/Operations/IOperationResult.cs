// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOperationResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.ExceptionHandling;
    using Kephas.Resources;

    /// <summary>
    /// Contract for operation results.
    /// </summary>
    public interface IOperationResult : IExpando, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the return value.
        /// </summary>
        /// <value>
        /// The return value.
        /// </value>
        object ReturnValue { get; set; }

        /// <summary>
        /// Gets or sets the state of the operation.
        /// </summary>
        /// <value>
        /// The state of the operation.
        /// </value>
        OperationState OperationState { get; set; }

        /// <summary>
        /// Gets or sets the percent completed.
        /// </summary>
        /// <value>
        /// The percent completed.
        /// </value>
        float PercentCompleted { get; set; }

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        /// <value>
        /// The elapsed time.
        /// </value>
        TimeSpan Elapsed { get; set; }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        ICollection<IOperationMessage> Messages { get; }

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        /// <value>
        /// The exceptions.
        /// </value>
        ICollection<Exception> Exceptions { get; }
    }

    /// <summary>
    /// Interface for typed operation result.
    /// </summary>
    /// <typeparam name="TValue">Type of the return value.</typeparam>
    public interface IOperationResult<TValue> : IOperationResult
    {
        /// <summary>
        /// Gets or sets the return value.
        /// </summary>
        /// <value>
        /// The return value.
        /// </value>
        new TValue ReturnValue { get; set; }
    }

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
        /// <param name="returnValue">The return value.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        public static TResult ReturnValue<TResult>(this TResult result, object returnValue)
            where TResult : class, IOperationResult
        {
            Requires.NotNull(result, nameof(result));

            result.ReturnValue = returnValue;

            return result;
        }

        /// <summary>
        /// Sets the return value to the provided one.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <typeparam name="TValue">Type of the return value.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="returnValue">The return value.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        public static TResult ReturnValue<TResult, TValue>(this TResult result, TValue returnValue)
            where TResult : class, IOperationResult<TValue>
        {
            Requires.NotNull(result, nameof(result));

            result.ReturnValue = returnValue;

            return result;
        }

        /// <summary>
        /// Marks the operation as completed setting the elapsed time and, optionally, the state.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="elapsed">The elapsed time.</param>
        /// <param name="operationState">Optional. State of the operation.</param>
        /// <returns>
        /// The provided result.
        /// </returns>
        public static TResult Complete<TResult>(this TResult result, TimeSpan elapsed, OperationState? operationState = null)
            where TResult : class, IOperationResult
        {
            Requires.NotNull(result, nameof(result));

            result.Elapsed = elapsed;
            result.OperationState = operationState ?? Kephas.Operations.OperationState.Completed;

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
        public static TResult OperationState<TResult>(this TResult result, OperationState state)
            where TResult : class, IOperationResult
        {
            Requires.NotNull(result, nameof(result));

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
        public static TResult MergeException<TResult>(this TResult result, Exception ex)
            where TResult : class, IOperationResult
        {
            Requires.NotNull(result, nameof(result));
            Requires.NotNull(ex, nameof(ex));

            result.Exceptions.Add(ex);

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
        public static TResult MergeMessage<TResult>(this TResult result, string message)
            where TResult : class, IOperationResult
        {
            Requires.NotNull(result, nameof(result));
            Requires.NotNull(message, nameof(message));

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
        public static TResult MergeMessage<TResult>(this TResult result, IOperationMessage message)
            where TResult : class, IOperationResult
        {
            Requires.NotNull(result, nameof(result));
            Requires.NotNull(message, nameof(message));

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
        public static TResult MergeMessages<TResult>(this TResult result, IOperationResult resultToMerge)
            where TResult : class, IOperationResult
        {
            Requires.NotNull(result, nameof(result));

            if (resultToMerge == null)
            {
                return result;
            }

            result.Messages.AddRange(resultToMerge.Messages);
            result.Exceptions.AddRange(resultToMerge.Exceptions);

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
        public static TResult MergeMessages<TResult>(this TResult result, Task<IOperationResult> asyncResult)
            where TResult : class, IOperationResult
        {
            Requires.NotNull(result, nameof(result));
            Requires.NotNull(asyncResult, nameof(asyncResult));

            if (!asyncResult.IsCompleted && !asyncResult.IsCanceled && !asyncResult.IsFaulted)
            {
                throw new InvalidOperationException(Strings.OperationResult_Merge_TaskNotCompleteException);
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
        public static TResult MergeAll<TResult>(this TResult result, IOperationResult resultToMerge)
            where TResult : class, IOperationResult
        {
            Requires.NotNull(result, nameof(result));

            if (resultToMerge == null)
            {
                return result;
            }

            result.Messages.AddRange(resultToMerge.Messages);
            result.Exceptions.AddRange(resultToMerge.Exceptions);
            result.ReturnValue = resultToMerge.ReturnValue;
            result.Elapsed += resultToMerge.Elapsed;

            return result;
        }

        /// <summary>
        /// Indicates whether the result has errors.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// A TResult.
        /// </returns>
        public static bool HasErrors(this IOperationResult result)
        {
            Requires.NotNull(result, nameof(result));

            return result.Exceptions.Any(
                e => (e is ISeverityQualifiedNotification qex
                      && (qex.Severity == SeverityLevel.Error || qex.Severity == SeverityLevel.Fatal))
                     || !(e is ISeverityQualifiedNotification));
        }

        /// <summary>
        /// Indicates whether the result has warnings.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// A TResult.
        /// </returns>
        public static bool HasWarnings(this IOperationResult result)
        {
            Requires.NotNull(result, nameof(result));

            return result.Exceptions.Any(
                e => e is ISeverityQualifiedNotification qex && qex.Severity == SeverityLevel.Warning);
        }

        /// <summary>
        /// Marks the result as completed and computes the operation state.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// A TResult.
        /// </returns>
        public static IEnumerable<Exception> Warnings(this IOperationResult result)
        {
            Requires.NotNull(result, nameof(result));

            return result.Exceptions.Where(
                e => e is ISeverityQualifiedNotification qex && qex.Severity == SeverityLevel.Warning);
        }
    }
}