// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringTemplate.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using Kephas.Dynamic;
using Kephas.Templating.Interpolation;

/// <summary>
/// Template constructed from a string.
/// </summary>
public class StringTemplate : Expando, ITemplate
{
    private readonly string content;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringTemplate"/> class.
    /// </summary>
    /// <param name="content">The template content.</param>
    /// <param name="kind">Optional. The template kind. If not specified, <see cref="InterpolationTemplatingEngine.Interpolation"/> is used.</param>
    /// <param name="name">Optional. The template name.</param>
    public StringTemplate(string content, string? kind = null, string? name = null)
    {
        this.content = content ?? throw new ArgumentNullException(nameof(content));
        this.Name = name ?? $"{nameof(StringTemplate)}_{Guid.NewGuid():N}";
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
    /// Gets the template content.
    /// </summary>
    /// <returns>The template content.</returns>
    public string GetContent() => this.content;
}