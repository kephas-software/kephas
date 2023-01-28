// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kephas.Collections.Concurrent;
using Kephas.Data.Formatting;
using Kephas.Dynamic;

/// <summary>
/// Encapsulates the result of an operation.
/// </summary>
public record OperationResult : IExpandoMixin, IOperationResult, IDataFormattable
{
    private ConcurrentDictionary<string, object?>? dynamicData;
    private object? value;
    private OperationState operationState;
    private float percentCompleted;
    private DateTimeOffset? startedAt = DateTimeOffset.Now;
    private DateTimeOffset? endedAt;
    private TimeSpan? elapsed;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class.
    /// </summary>
    public OperationResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class.
    /// </summary>
    /// <param name="value">The return value.</param>
    public OperationResult(object? value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the return value.
    /// </summary>
    /// <value>
    /// The return value.
    /// </value>
    public virtual object? Value
    {
        get => this.value;
        set => this.SetProperty(ref this.value, value);
    }

    /// <summary>
    /// Gets or sets the state of the operation.
    /// </summary>
    /// <value>
    /// The state of the operation.
    /// </value>
    public OperationState OperationState
    {
        get => this.operationState;
        set => this.SetProperty(ref this.operationState, value);
    }

    /// <summary>
    /// Gets or sets the percent completed.
    /// </summary>
    /// <value>
    /// The percent completed.
    /// </value>
    public virtual float PercentCompleted
    {
        get => this.percentCompleted;
        set => this.SetProperty(ref this.percentCompleted, value);
    }

    /// <summary>
    /// Gets or sets the time when the job started.
    /// </summary>
    public DateTimeOffset? StartedAt
    {
        get => this.startedAt;
        set => this.SetProperty(ref this.startedAt, value);
    }

    /// <summary>
    /// Gets or sets the time when the job ended.
    /// </summary>
    public DateTimeOffset? EndedAt
    {
        get => this.endedAt;
        set => this.SetProperty(ref this.endedAt, value);
    }

    /// <summary>
    /// Gets or sets the elapsed time.
    /// </summary>
    /// <value>
    /// The elapsed time.
    /// </value>
    public virtual TimeSpan Elapsed
    {
        get => this.elapsed ??
               (this.startedAt.HasValue ? DateTimeOffset.Now - this.startedAt.Value : TimeSpan.Zero);
        set => this.SetProperty(ref this.elapsed, value);
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
    /// <param name="context">Optional. The formatting context.</param>
    /// <returns>A serialization friendly object representing this object.</returns>
    public virtual object ToData(object? context = null)
    {
        return new Expando
        {
            [nameof(this.OperationState)] = this.OperationState,
            [nameof(this.Elapsed)] = this.Elapsed,
            [nameof(this.PercentCompleted)] = this.PercentCompleted,
            [nameof(this.Messages)] =
                this.Messages.Count == 0 ? null : this.Messages.Select(m => m.ToData(context)).ToArray(),
            [nameof(this.Value)] = this.OperationState == OperationState.Completed
                ? this.Value.ToData(context)
                : null,
        };
    }

    /// <summary>
    /// Gets the inner dictionary.
    /// </summary>
    IDictionary<string, object?> IExpandoMixin.InnerDictionary => this.dynamicData ??= new();

    /// <summary>
    /// Called when a property changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        var handler = this.PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the property.
    /// </summary>
    /// <typeparam name="T">The field type.</typeparam>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        this.OnPropertyChanged(name);
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
    public new TValue Value
    {
        get => (TValue)base.Value!;
        set => base.Value = value;
    }
}