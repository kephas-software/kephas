// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessStartResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Diagnostics;

using System.Diagnostics;
using System.Text;
using Kephas.Operations;

/// <summary>
/// A process start result interface.
/// </summary>
public interface IProcessStartResult : IOperationResult<Process>, IDisposable
{
    /// <summary>
    /// Gets the process.
    /// </summary>
    /// <value>
    /// The process.
    /// </value>
    Process Process { get; }

    /// <summary>
    /// Gets the start exception.
    /// </summary>
    /// <value>
    /// The start exception.
    /// </value>
    Exception? StartException { get; }

    /// <summary>
    /// Gets information describing the error.
    /// </summary>
    /// <value>
    /// Information describing the error.
    /// </value>
    StringBuilder ErrorData { get; }

    /// <summary>
    /// Gets or sets information describing the output.
    /// </summary>
    /// <value>
    /// Information describing the output.
    /// </value>
    StringBuilder OutputData { get; set; }
}