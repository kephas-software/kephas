// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpStreamScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting;

/// <summary>
/// C# script based on a stream.
/// </summary>
/// <seealso cref="StreamScript" />
public class CSharpStreamScript : StreamScript
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CSharpStreamScript"/> class.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="name">The name.</param>
    public CSharpStreamScript(Stream sourceCode, string? name = null)
        : base(sourceCode, CSharpLanguageService.Language, name)
    {
    }
}