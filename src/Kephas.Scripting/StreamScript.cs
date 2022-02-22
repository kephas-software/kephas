// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting;

using Kephas.Dynamic;

/// <summary>
/// Script based on a stream.
/// </summary>
public class StreamScript : Expando, IStreamScript
{
    private readonly Stream sourceCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamScript"/> class.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="name">Optional. The template name.</param>
    /// <param name="language">The script language.</param>
    public StreamScript(string language, Stream sourceCode, string? name = null)
    {
        this.sourceCode = sourceCode ?? throw new ArgumentNullException(nameof(sourceCode));
        this.Name = name ?? $"{nameof(StreamScript)}_{Guid.NewGuid():N}";
        this.Language = language ?? throw new ArgumentNullException(nameof(language));
    }

    /// <summary>
    /// Gets the script name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the script language.
    /// </summary>
    /// <value>
    /// The script language.
    /// </value>
    public string Language { get; }

    /// <summary>
    /// Gets the source code stream.
    /// </summary>
    /// <returns>The source code stream.</returns>
    public Stream GetSourceCodeStream() => this.sourceCode;
}