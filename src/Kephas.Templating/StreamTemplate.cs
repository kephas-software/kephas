// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamTemplate.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using Kephas.Dynamic;
using Kephas.IO;
using Kephas.Templating.Interpolation;

/// <summary>
/// Template constructed from a stream.
/// </summary>
public class StreamTemplate : Expando, ITemplate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamTemplate"/> class.
    /// </summary>
    /// <param name="content">The template content.</param>
    /// <param name="kind">Optional. The template kind. If not specified, <see cref="InterpolationTemplatingEngine.Interpolation"/> is used.</param>
    /// <param name="name">Optional. The template name.</param>
    public StreamTemplate(Stream content, string? kind = null, string? name = null)
    {
        this.Stream = content ?? throw new ArgumentNullException(nameof(content));
        this.Name = name ?? nameof(StreamTemplate);
        this.Kind = kind ?? InterpolationTemplatingEngine.Interpolation;
    }

    /// <summary>
    /// Gets the stream.
    /// </summary>
    /// <value>
    /// The stream.
    /// </value>
    public Stream Stream { get; }

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
    public Task<string> GetContentAsync(CancellationToken cancellationToken = default) => this.Stream.ReadAllStringAsync();
}