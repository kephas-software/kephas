// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptGlobals.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the script globals class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using Kephas.Dynamic;

    /// <summary>
    /// The script globals.
    /// </summary>
    public class ScriptGlobals : Expando, IScriptGlobals
    {
        /// <summary>
        /// Gets or sets the global arguments.
        /// </summary>
        /// <value>
        /// The global arguments.
        /// </value>
        public IDynamic Args { get; set; } = new Expando();
    }
}