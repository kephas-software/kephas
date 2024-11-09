// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompiledRazorPage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

/// <summary>
/// Interface for compiled Razor page.
/// </summary>
public interface ICompiledRazorPage
{
    /// <summary>
    /// Gets the associated template.
    /// </summary>
    ITemplate Template { get; }

    /// <summary>
    /// Renders the template asynchronously using the provided model.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="model">The model.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous result.</returns>
    Task RenderAsync<T>(T model, TextWriter writer, CancellationToken cancellationToken = default);
}