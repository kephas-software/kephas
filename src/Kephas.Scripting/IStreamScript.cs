// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStreamScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting;

using Kephas.IO;

/// <summary>
/// Script based on a stream.
/// </summary>
public interface IStreamScript : IScript
{
    /// <summary>
    /// Gets the source code stream.
    /// </summary>
    /// <returns>The source code stream.</returns>
    Stream GetSourceCodeStream();

    /// <summary>
    /// Gets the script source code.
    /// </summary>
    /// <returns>The source code.</returns>
    string IScript.GetSourceCode()
        => this.GetSourceCodeStream().ReadAllString();

    /// <summary>
    /// Gets the script source code asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>The source code.</returns>
    Task<string> IScript.GetSourceCodeAsync(CancellationToken cancellationToken)
        => this.GetSourceCodeStream().ReadAllStringAsync();
}