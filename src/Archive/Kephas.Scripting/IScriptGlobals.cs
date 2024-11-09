// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScriptGlobals.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScriptGlobals interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using Kephas.Dynamic;

    /// <summary>
    /// Interface for scripting context.
    /// </summary>
    public interface IScriptGlobals : IExpando
    {
        /// <summary>
        /// Gets the global arguments.
        /// </summary>
        /// <value>
        /// The global arguments.
        /// </value>
        IDynamic? Args { get; }
    }
}