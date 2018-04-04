// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataCommandResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataCommandResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using Kephas.Dynamic;

    /// <summary>
    /// Contract for data command results.
    /// </summary>
    public interface IDataCommandResult : IExpando
    {
        /// <summary>
        /// Gets the result message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        Exception Exception { get; }
    }
}