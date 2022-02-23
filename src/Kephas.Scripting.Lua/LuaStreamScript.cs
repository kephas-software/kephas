// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuaStreamScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Lua;

/// <summary>
/// Lua script based on a stream.
/// </summary>
public class LuaStreamScript : StreamScript
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LuaStreamScript"/> class.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="name">The name.</param>
    public LuaStreamScript(Stream sourceCode, string? name = null)
        : base(sourceCode, LuaLanguageService.Language, name)
    {
    }
}