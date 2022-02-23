// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuaStringScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Lua;

/// <summary>
/// Lua script based on a string.
/// </summary>
/// <seealso cref="StringScript" />
public class LuaStringScript : StringScript
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LuaStringScript"/> class.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="name">The name.</param>
    public LuaStringScript(string sourceCode, string? name = null)
        : base(LuaLanguageService.Language, sourceCode, name)
    {
    }
}