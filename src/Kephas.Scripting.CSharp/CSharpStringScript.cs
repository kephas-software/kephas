// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpStringScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting;

/// <summary>
/// C# script based on a string.
/// </summary>
/// <seealso cref="StringScript" />
public class CSharpStringScript : StringScript
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CSharpStringScript"/> class.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="name">The name.</param>
    public CSharpStringScript(string sourceCode, string? name = null)
        : base(CSharpLanguageService.Language, sourceCode, name)
    {
    }
}