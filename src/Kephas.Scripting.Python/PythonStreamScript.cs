// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PythonStreamScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Python;

/// <summary>
/// Python script based on a stream.
/// </summary>
public class PythonStreamScript : StreamScript
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PythonStreamScript"/> class.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="name">The name.</param>
    public PythonStreamScript(Stream sourceCode, string? name = null) 
        : base(PythonLanguageService.Language, sourceCode, name)
    {
    }
}