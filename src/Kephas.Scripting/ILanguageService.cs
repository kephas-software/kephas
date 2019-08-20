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
        /// Executes the expression asynchronously.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="scriptGlobals">The script globals (optional).</param>
        /// <param name="args">The arguments (optional).</param>
        /// <param name="executionContext">The execution context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
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