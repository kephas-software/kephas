// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderTemplate.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using System.Text;

/// <summary>
/// Template constructed from a string builder.
/// </summary>
public class StringBuilderTemplate : ITemplate
{
    private readonly StringBuilder content;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringBuilderTemplate"/> class.
    /// </summary>
    /// <param name="name">The template name.</param>
    /// <param name="kind">The template kind.</param>
    /// <param name="content">The template content.</param>
    public StringBuilderTemplate(string name, string kind, StringBuilder content)
    {
        this.content = content;
        this.Name = name;
        this.Kind = kind;
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
    public Task<string> GetContentAsync(CancellationToken cancellationToken = default) => Task.FromResult(this.content.ToString());

}