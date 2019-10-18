// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncLanguageService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISyncLanguageService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Interface for synchronous language service.
    /// </summary>
    public interface ISyncLanguageService
    {
        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="scriptGlobals">Optional. The script globals.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="executionContext">Optional. The execution context.</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        object Execute(
            IScript script,
            IScriptGlobals scriptGlobals = null,
            IExpando args = null,
            IContext executionContext = null);
    }
}
