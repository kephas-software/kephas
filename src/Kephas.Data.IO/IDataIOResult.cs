// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataIOResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataIOResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Data.IO.Resources;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
  /// Contract for data exchange result.
  /// </summary>
  public interface IDataIOResult : INotifyPropertyChanged
  {
    /// <summary>
    /// Gets or sets the state of the operation.
    /// </summary>
    /// <value>
    /// The state of the operation.
    /// </value>
    DataIOOperationState OperationState { get; set; }

    /// <summary>
    /// Gets or sets the percent completed.
    /// </summary>
    /// <value>
    /// The percent completed.
    /// </value>
    double PercentCompleted { get; set; }

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
    IProducerConsumerCollection<IDataIOMessage> Messages { get; }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <value>
    /// The exceptions.
    /// </value>
    IProducerConsumerCollection<DataIOException> Exceptions { get; }
  }

  /// <summary>
  /// Extensions for <see cref="IDataIOResult"/>.
  /// </summary>
  public static class DataExchangeResultExtensions
  {
    /// <summary>
    /// Merges the exception.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="ex">The exception.</param>
    /// <returns>The provided result.</returns>
    public static IDataIOResult MergeException(this IDataIOResult result, Exception ex)
    {
      Requires.NotNull(result, nameof(result));
      Requires.NotNull(ex, nameof(ex));

      result.Exceptions.TryAdd(DataIOException.FromException(ex));

      return result;
    }

    /// <summary>
    /// Merges the exception.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="message">The message.</param>
    /// <returns>
    /// The provided result.
    /// </returns>
    public static IDataIOResult MergeMessage(this IDataIOResult result, string message)
    {
      Requires.NotNull(result, nameof(result));
      Requires.NotNull(message, nameof(message));

      result.Messages.TryAdd(new DataIOMessage(message));

      return result;
    }

    /// <summary>
    /// Merges the exception.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="message">The message.</param>
    /// <returns>
    /// The provided result.
    /// </returns>
    public static IDataIOResult MergeMessage(this IDataIOResult result, IDataIOMessage message)
    {
      Requires.NotNull(result, nameof(result));
      Requires.NotNull(message, nameof(message));

      result.Messages.TryAdd(message);

      return result;
    }

    /// <summary>
    /// Merges the exception.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="resultToMerge">The result to merge.</param>
    /// <returns>
    /// The provided result.
    /// </returns>
    public static IDataIOResult MergeResult(this IDataIOResult result, IDataIOResult resultToMerge)
    {
      Requires.NotNull(result, nameof(result));
      Requires.NotNull(resultToMerge, nameof(resultToMerge));

      result.Messages.AddRange(resultToMerge.Messages);
      result.Exceptions.AddRange(resultToMerge.Exceptions);

      return result;
    }

    /// <summary>
    /// Merges the exception.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="task">The task of which result will be merged.</param>
    /// <returns>
    /// The provided result.
    /// </returns>
    public static IDataIOResult MergeResult(this IDataIOResult result, Task<IDataIOResult> task)
    {
      Requires.NotNull(result, nameof(result));
      Requires.NotNull(task, nameof(task));

      if (!task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
      {
        throw new InvalidOperationException(Strings.DataIOResult_Merge_TaskNotCompleteException);
      }

      return task.Exception == null
              ? MergeResult(result, task.Result)
              : MergeException(result, task.Exception);
    }
  }
}