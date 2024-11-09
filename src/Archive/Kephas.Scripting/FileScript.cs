// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting;

using Kephas.Dynamic;
using System.IO;

/// <summary>
/// Script based on a file.
/// </summary>
/// <seealso cref="Expando" />
/// <seealso cref="IScript" />
public class FileScript : Expando, IScript
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileScript"/> class.
    /// </summary>
    /// <param name="language">The script language.</param>
    /// <param name="sourceCodeFilePath">The source code file path.</param>
    /// <param name="name">Optional. The script name.</param>
    public FileScript(string sourceCodeFilePath, string? language = null, string? name = null)
    {
        this.Language = language ?? Path.GetExtension(sourceCodeFilePath);
        this.Name = name ?? Path.GetFileNameWithoutExtension(sourceCodeFilePath);
        this.FilePath = sourceCodeFilePath ?? throw new ArgumentNullException(nameof(sourceCodeFilePath));
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
    /// Gets the script file path.
    /// </summary>
    /// <value>
    /// The script file path.
    /// </value>
    public string FilePath { get; }

    /// <summary>
    /// Gets the source code from the file.
    /// </summary>
    /// <returns>The source code from the file.</returns>
    public string GetSourceCode()
        => File.ReadAllText(this.FilePath);

    /// <summary>
    /// Gets the script source code asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>The source code.</returns>
    public Task<string> GetSourceCodeAsync(CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(this.FilePath, cancellationToken);
}