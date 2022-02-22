// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemplate.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using Kephas.Dynamic;

/// <summary>
/// Abstraction for describing a template.
/// </summary>
public interface ITemplate : IExpando
{
    /// <summary>
    /// Gets the template name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the template kind.
    /// </summary>
    /// <remarks>
    /// Typically, the template kind is provided by the file extension: .razor, .t4, etc.
    /// </remarks>
    string Kind { get; }

    /// <summary>
    /// Gets the template content.
    /// </summary>
    /// <returns>The template content.</returns>
    string GetContent();

    /// <summary>
    /// Gets the template content asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>The template content.</returns>
    Task<string> GetContentAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(this.GetContent());
}