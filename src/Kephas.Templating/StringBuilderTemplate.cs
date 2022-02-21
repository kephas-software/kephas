// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderTemplate.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using System.Text;

using Kephas.Dynamic;
using Kephas.Templating.Interpolation;

/// <summary>
/// Template constructed from a string builder.
/// </summary>
public class StringBuilderTemplate : Expando, ITemplate
{
    private readonly StringBuilder content;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringBuilderTemplate"/> class.
    /// </summary>
    /// <param name="content">The template content.</param>
    /// <param name="kind">Optional. The template kind. If not specified, <see cref="InterpolationTemplatingEngine.Interpolation"/> is used.</param>
    /// <param name="name">Optional. The template name.</param>
    public StringBuilderTemplate(StringBuilder content, string? kind = null, string? name = null)
    {
        this.content = content ?? throw new ArgumentNullException(nameof(content));
        this.Name = name ?? nameof(StringBuilderTemplate);
        this.Kind = kind ?? InterpolationTemplatingEngine.Interpolation;
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