// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRazorProjectEngineContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Services;
using Microsoft.AspNetCore.Razor.Language;

/// <summary>
/// Context for creating a <see cref="RazorProjectEngine"/>.
/// </summary>
public interface IRazorProjectEngineContext : IContext
{
    /// <summary>
    /// Gets the template.
    /// </summary>
    /// <value>
    /// The template.
    /// </value>
    public ITemplate? Template { get; }

    /// <summary>
    /// Gets the processing context.
    /// </summary>
    /// <value>
    /// The processing context.
    /// </value>
    public ITemplateProcessingContext? ProcessingContext { get; }

    /// <summary>
    /// Gets the root namespace.
    /// </summary>
    /// <value>
    /// The root namespace.
    /// </value>
    string? RootNamespace { get; }

    /// <summary>
    /// Gets the type of the model.
    /// </summary>
    /// <value>
    /// The type of the model.
    /// </value>
    public Type? ModelType { get; }

    /// <summary>
    /// Gets the action to configure the builder.
    /// </summary>
    /// <value>
    /// The configure action.
    /// </value>
    Action<RazorProjectEngineBuilder>? Configure { get; }
}