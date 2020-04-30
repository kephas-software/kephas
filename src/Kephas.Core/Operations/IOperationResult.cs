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
        object? ReturnValue { get; set; }

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

        /// <summary>
        /// Gets the operation result awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        OperationResultAwaiter GetAwaiter();

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
        new TValue ReturnValue { get; set; }

        /// <summary>
        /// Gets the operation result awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        new OperationResultAwaiter<TValue> GetAwaiter();

        /// <summary>
        /// Converts this object to a task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        new Task<TValue> AsTask();
    }
}