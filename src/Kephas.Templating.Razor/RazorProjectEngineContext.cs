// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorProjectEngineContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Injection;
using Kephas.Services;
using Microsoft.AspNetCore.Razor.Language;

/// <summary>
/// The implementation of the <see cref="IRazorProjectEngineContext"/>.
/// </summary>
/// <seealso cref="Context" />
/// <seealso cref="IRazorProjectEngineContext" />
public class RazorProjectEngineContext : Context, IRazorProjectEngineContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RazorProjectEngineContext"/> class.
    /// </summary>
    /// <param name="injector">The injector.</param>
    public RazorProjectEngineContext(IInjector injector)
        : base(injector)
    {
    }

    /// <summary>
    /// Gets or sets the template.
    /// </summary>
    /// <value>
    /// The template.
    /// </value>
    public ITemplate? Template { get; set; }

    /// <summary>
    /// Gets or sets the processing context.
    /// </summary>
    /// <value>
    /// The processing context.
    /// </value>
    public ITemplateProcessingContext? ProcessingContext { get; set; }

    /// <summary>
    /// Gets or sets the root namespace.
    /// </summary>
    /// <value>
    /// The root namespace.
    /// </value>
    public string? RootNamespace { get; set; }

    /// <summary>
    /// Gets or sets the type of the model.
    /// </summary>
    /// <value>
    /// The type of the model.
    /// </value>
    public Type? ModelType { get; set; }

    /// <summary>
    /// Gets or sets the action to configure the builder.
    /// </summary>
    /// <value>
    /// The configure action.
    /// </value>
    public Action<RazorProjectEngineBuilder>? Configure { get; set; }
}