// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringTemplate.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using Kephas.Dynamic;

/// <summary>
/// Template constructed from a string.
/// </summary>
public class StringTemplate : Expando, ITemplate
{
    private readonly string content;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringTemplate"/> class.
    /// </summary>
    /// <param name="name">The template name.</param>
    /// <param name="kind">The template kind.</param>
    /// <param name="content">The template content.</param>
    public StringTemplate(string name, string kind, string content)
    {
        this.content = content ?? throw new ArgumentNullException(nameof(content));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.Kind = kind ?? throw new ArgumentNullException(nameof(kind));
    }

    /// <summary>
    /// Gets the template name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the template kind.
    /// </summary>
    /// <remarks>
    /// Typically, the template kind is provided by the file extension: .razor, .t4, etc.
    /// </remarks>
    public string Kind { get; }

    /// <summary>
    /// Gets the template content asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>The template content.</returns>
    public Task<string> GetContentAsync(CancellationToken cancellationToken = default) => Task.FromResult(this.content);

}