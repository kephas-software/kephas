// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections.Concurrent;

namespace Kephas.Operations;

/// <summary>
/// Encapsulates the result of an operation.
/// </summary>
public record OperationResult() : IOperationResult
{
    private TimeSpan? elapsed;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class.
    /// </summary>
    /// <param name="value">The return value.</param>
    public OperationResult(object? value)
        : this()
    {
        Value = value;
    }

    /// <summary>
    /// Gets or sets the return value.
    /// </summary>
    /// <value>
    /// The return value.
    /// </value>
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets the state of the operation.
    /// </summary>
    /// <value>
    /// The state of the operation.
    /// </value>
    public OperationStatus OperationStatus { get; set; }

    /// <summary>
    /// Gets or sets the percent completed.
    /// </summary>
    /// <value>
    /// The percent completed.
    /// </value>
    public float? PercentCompleted { get; set; }

    /// <summary>
    /// Gets or sets the time when the operation started.
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the time when the operation ended.
    /// </summary>
    public DateTimeOffset? EndedAt { get; set; }

    /// <summary>
    /// Gets or sets the elapsed time.
    /// </summary>
    /// <value>
    /// The elapsed time.
    /// </value>
    public virtual TimeSpan? Elapsed
    {
        get => elapsed ??
               (StartedAt.HasValue ? DateTimeOffset.Now - StartedAt.Value : TimeSpan.Zero);
        set => elapsed = value;
    }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public ICollection<IOperationMessage> Messages { get; set; } = new ConcurrentCollection<IOperationMessage>();
}

/// <summary>
/// Encapsulates the result of an operation.
/// </summary>
/// <typeparam name="TValue">Type of the value.</typeparam>
public record OperationResult<TValue> : OperationResult, IOperationResult<TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult{TValue}"/> class.
    /// </summary>
    public OperationResult()
        : this((TValue)default!)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult{TValue}"/> class.
    /// </summary>
    /// <param name="value">The return value.</param>
    public OperationResult(TValue value)
        : base(value!)
    {
    }

    /// <summary>
    /// Gets or sets the return value.
    /// </summary>
    /// <value>
    /// The return value.
    /// </value>
    public new virtual TValue Value
    {
        get => (TValue)base.Value!;
        set => base.Value = value;
    }
}
