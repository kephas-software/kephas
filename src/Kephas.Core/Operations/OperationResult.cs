// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Kephas.Dynamic;

    /// <summary>
    /// Encapsulates the result of an operation.
    /// </summary>
    public class OperationResult : Expando, IOperationResult
    {
        /// <summary>
        /// The operation state.
        /// </summary>
        private OperationState operationState;

        /// <summary>
        /// The percent completed.
        /// </summary>
        private double percentCompleted;

        /// <summary>
        /// The elapsed value.
        /// </summary>
        private TimeSpan elapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        public OperationResult()
        {
            this.Exceptions = new ConcurrentBag<Exception>();
            this.Messages = new ConcurrentBag<IOperationMessage>();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        public double PercentCompleted
        {
            get => this.percentCompleted;
            set => this.SetProperty(ref this.percentCompleted, value);
        }

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        /// <value>
        /// The elapsed time.
        /// </value>
        public TimeSpan Elapsed
        {
            get => this.elapsed;
            set => this.SetProperty(ref this.elapsed, value);
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public IProducerConsumerCollection<IOperationMessage> Messages { get; set; }

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        /// <value>
        /// The exceptions.
        /// </value>
        public IProducerConsumerCollection<Exception> Exceptions { get; }

        /// <summary>
        /// Called when [property changed].
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