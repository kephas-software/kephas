// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILanguageService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILanguageService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// A shared application service contract responsible for interpreting/executing scripts
    /// for a specified language.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(LanguageAttribute) })]
    public interface ILanguageService
    {
        /// <summary>
        /// Executes the script asynchronously.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="scriptGlobals">Optional. The script globals.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="executionContext">Optional. The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        Task<object> ExecuteAsync(
            IScript script,
            IScriptGlobals scriptGlobals = null,
            IExpando args = null,
            IContext executionContext = null,
            CancellationToken cancellationToken = default);
    }
}