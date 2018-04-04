// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data I/O result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Kephas.Dynamic;

    /// <summary>
    /// The default implementation of the <see cref="IDataIOResult"/>.
    /// </summary>
    public class DataIOResult : Expando, IDataIOResult
    {
        /// <summary>
        /// The operation state.
        /// </summary>
        private DataIOOperationState operationState;

        /// <summary>
        /// The percent completed.
        /// </summary>
        private double percentCompleted;

        /// <summary>
        /// The elapsed value.
        /// </summary>
        private TimeSpan elapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOResult"/> class.
        /// </summary>
        public DataIOResult()
        {
            this.Exceptions = new ConcurrentBag<DataIOException>();
            this.Messages = new ConcurrentBag<IDataIOMessage>();
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
        public DataIOOperationState OperationState
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
        public IProducerConsumerCollection<IDataIOMessage> Messages { get; set; }

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        /// <value>
        /// The exceptions.
        /// </value>
        public IProducerConsumerCollection<DataIOException> Exceptions { get; }

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