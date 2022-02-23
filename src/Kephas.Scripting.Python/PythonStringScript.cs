// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PythonStringScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting;

/// <summary>
/// Python script based on a string.
/// </summary>
public class PythonStringScript : StringScript
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PythonStringScript"/> class.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="name">The name.</param>
    public PythonStringScript(string sourceCode, string? name = null)
        : base(sourceCode, PythonLanguageService.Language, name)
    {
    }
}