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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

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
        Task<object?> ExecuteAsync(
            IScript script,
            IScriptGlobals? scriptGlobals = null,
            IDynamic? args = null,
            IContext? executionContext = null,
            CancellationToken cancellationToken = default);

#if NETSTANDARD2_0
#else
        /// <summary>
        /// Executes the script synchronously.
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
            IScriptGlobals? scriptGlobals = null,
            IExpando? args = null,
            IContext? executionContext = null)
        {
            return this.ExecuteAsync(script, scriptGlobals, args, executionContext).GetResultNonLocking();
        }
#endif
    }

#if NETSTANDARD2_0
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
        object? Execute(
            IScript script,
            IScriptGlobals? scriptGlobals = null,
            IDynamic? args = null,
            IContext? executionContext = null);
    }

    /// <summary>
    /// Extension methods for <see cref="ILanguageService"/>.
    /// </summary>
    public static class LanguageServiceExtensions
    {
        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="scriptGlobals">Optional. The script globals.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="executionContext">Optional. The execution context.</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        public static object Execute(
            this ILanguageService languageService,
            IScript script,
            IScriptGlobals? scriptGlobals = null,
            IExpando? args = null,
            IContext? executionContext = null)
        {
            Requires.NotNull(languageService, nameof(languageService));

            if (languageService is ISyncLanguageService syncLanguageService)
            {
                return syncLanguageService.Execute(script, scriptGlobals, args, executionContext);
            }

            return languageService.ExecuteAsync(script, scriptGlobals, args, executionContext).GetResultNonLocking();
        }
    }
#endif
}