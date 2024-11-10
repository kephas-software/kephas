// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemplateProcessingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using System;

using Kephas.Operations;
using Kephas.Services;

/// <summary>
/// Interface for scripting context.
/// </summary>
public interface ITemplateProcessingContext : IContext
{
    /// <summary>
    /// Gets or sets the template to process.
    /// </summary>
    /// <value>
    /// The template to process.
    /// </value>
    ITemplate? Template { get; set; }

    /// <summary>
    /// Gets or sets the bound model.
    /// </summary>
    /// <value>
    /// The bound model.
    /// </value>
    object? Model { get; set; }

    /// <summary>
    /// Gets or sets the TextWriter used to write the output.
    /// </summary>
    TextWriter? TextWriter { get; set; }

    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    /// <value>
    /// The result.
    /// </value>
    IOperationResult? Result { get; set; }

    /// <summary>
    /// Gets or sets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    Exception? Exception { get; set; }
}