// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using Kephas.Collections.Concurrent;
using Kephas.Serialization;

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
        this.Value = value;
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
    public OperationState OperationState { get; set; }

    /// <summary>
    /// Gets or sets the percent completed.
    /// </summary>
    /// <value>
    /// The percent completed.
    /// </value>
    public virtual float PercentCompleted { get; set; }

    /// <summary>
    /// Gets or sets the time when the operation started.
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; } = DateTimeOffset.Now;

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
    public virtual TimeSpan Elapsed
    {
        get => this.elapsed ??
               (this.StartedAt.HasValue ? DateTimeOffset.Now - this.StartedAt.Value : TimeSpan.Zero);
        set => this.elapsed = value;
    }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public ICollection<IOperationMessage> Messages { get; set; } = new ConcurrentCollection<IOperationMessage>();

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString()
    {
        return $"{this.OperationState} ({this.PercentCompleted:P1})";
    }

    /// <summary>
    /// Converts this object to a serialization friendly representation.
    /// </summary>
    /// <returns>A serialization friendly object representing this object.</returns>
    public virtual object? GetObjectData()
    {
        return new Dictionary<string, object?>
        {
            [nameof(this.OperationState)] = this.OperationState,
            [nameof(this.Elapsed)] = this.Elapsed,
            [nameof(this.PercentCompleted)] = this.PercentCompleted,
            [nameof(this.Messages)] =
                this.Messages.Count == 0 ? null : this.Messages.Select(m => m.GetObjectData()).ToArray(),
            [nameof(this.Value)] = this.OperationState == OperationState.Completed
                ? this.Value.GetObjectData()
                : null,
        };
    }
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
