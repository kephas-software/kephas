// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BreakLoopSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.Workflow.Interaction;

using Kephas.ExceptionHandling;
using Kephas.Interaction;

/// <summary>
/// Signal indicating that the current loop should be interrupted.
/// </summary>
public class BreakLoopSignal : Exception, ISignal
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BreakLoopSignal"/> class.
    /// </summary>
    public BreakLoopSignal()
        : this($"Signal {nameof(BreakLoopSignal)}.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BreakLoopSignal"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public BreakLoopSignal(string message)
        : base(message)
    {
        this.Severity = LogLevel.Information;
    }

    /// <summary>
    /// Gets the severity.
    /// </summary>
    /// <value>
    /// The severity.
    /// </value>
    public LogLevel Severity { get; }
}
