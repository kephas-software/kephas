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
    using System.Threading.Tasks;

    using Kephas.Dynamic;

    /// <summary>
    /// Contract for operation results.
    /// </summary>
    public interface IOperationResult : IDynamic, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the return value.
        /// </summary>
        /// <value>
        /// The return value.
        /// </value>
        object? Value { get; set; }

        /// <summary>
        /// Gets or sets the state of the operation.
        /// </summary>
        /// <value>
        /// The state of the operation.
        /// </value>
        OperationState OperationState { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the operation started.
        /// </summary>
        DateTimeOffset? StartedAt { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the operation ended.
        /// </summary>
        DateTimeOffset? EndedAt { get; set; }

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

        /// <summary>
        /// Converts this object to a task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task AsTask();
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
        new TValue Value { get; set; }

        /// <summary>
        /// Converts this object to a task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        new Task<TValue> AsTask();

        /// <summary>
        /// Converts this object to a task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task IOperationResult.AsTask() => this.AsTask();
    }
}