// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTemplate.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using Kephas.Dynamic;

/// <summary>
/// Template constructed from a file.
/// </summary>
public class FileTemplate : Expando, ITemplate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileTemplate"/> class.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="kind">Optional. The template kind. If not specified, the file extension is considered.</param>
    /// <param name="name">Optional. The template name. If not specified, the file name without the extension is considered.</param>
    public FileTemplate(string path, string? kind = null, string? name = null)
    {
        this.Path = path ?? throw new ArgumentNullException(nameof(path));
        this.Kind = kind ?? System.IO.Path.GetExtension(path);
        this.Name = name ?? System.IO.Path.GetFileNameWithoutExtension(path);
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
    /// Gets the template path.
    /// </summary>
    /// <value>
    /// The template path.
    /// </value>
    public string Path { get; }

    /// <summary>
    /// Gets the template content.
    /// </summary>
    /// <returns>The template content.</returns>
    public string GetContent()
        => File.ReadAllText(this.Path);

    /// <summary>
    /// Gets the template content asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// The template content.
    /// </returns>
    public Task<string> GetContentAsync(CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(this.Path, cancellationToken);
}
