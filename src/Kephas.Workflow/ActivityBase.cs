// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activity base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Kephas.Collections.Concurrent;
    using Kephas.Data.Formatting;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Serialization;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Base implementation of <see cref="IActivity"/>.
    /// </summary>
    public abstract class ActivityBase : IActivity, IExpandoMixin, IDataFormattable
    {
        private ConcurrentDictionary<string, object?>? dynamicData;
        private object? value;
        private OperationState operationState;
        private float percentCompleted;
        private DateTimeOffset? startedAt;
        private DateTimeOffset? endedAt;
        private TimeSpan? elapsed;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

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
            get => this.elapsed ?? (this.startedAt.HasValue ? DateTimeOffset.Now - this.startedAt.Value : TimeSpan.Zero);
            set => this.SetProperty(ref this.elapsed, value);
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public ICollection<IOperationMessage> Messages { get; } = new ConcurrentCollection<IOperationMessage>();

        /// <summary>
        /// Gets or sets the target against which the activity is executed.
        /// </summary>
        /// <remarks>
        /// The target is typically the activity's container instance.
        /// For example, a user entity may contain a ChangePassword activity,
        /// in which case the target is the user.
        /// </remarks>
        /// <value>
        /// The target.
        /// </value>
        public object? Target { get; set; }

        /// <summary>
        /// Gets or sets the arguments for the execution.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public IDynamic? Arguments { get; set; }

        /// <summary>
        /// Gets or sets the execution context.
        /// </summary>
        /// <value>
        /// The execution context.
        /// </value>
        [ExcludeFromSerialization]
        public IActivityContext? Context { get; set; }

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public object Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the return value.
        /// </summary>
        /// <value>
        /// The return value.
        /// </value>
        public object? Value
        {
            get => this.value;
            set => this.SetProperty(ref this.value, value);
        }

        /// <summary>
        /// Gets the inner dictionary.
        /// </summary>
        IDictionary<string, object?> IExpandoMixin.InnerDictionary => this.dynamicData ??= new();

        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public virtual IActivityInfo GetTypeInfo() => (IActivityInfo)this.GetTypeInfoCore();

        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        ITypeInfo IInstance.GetTypeInfo() => this.GetTypeInfoCore();

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
                [nameof(this.Messages)] = this.Messages.Count == 0 ? null : this.Messages.Select(m => m.ToData(context)).ToArray(),
                [nameof(this.Value)] = this.OperationState == OperationState.Completed
                    ? this.Value.ToData(context)
                    : null,
            };
        }

        /// <summary>
        /// Converts this object to a task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual Task<object?> AsTask() => Task.FromResult(this.Value);

        /// <summary>
        /// Gets the type information (overridable implementation).
        /// </summary>
        /// <returns>The type information.</returns>
        protected virtual ITypeInfo GetTypeInfoCore() => this.GetRuntimeTypeInfo()!;

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
}