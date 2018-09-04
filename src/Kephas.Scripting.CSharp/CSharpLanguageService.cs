// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpLanguageService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the C# language service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.CSharp
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using Microsoft.CodeAnalysis.CSharp.Scripting;

    /// <summary>
    /// A C# language service.
    /// </summary>
    [Language(Language, LanguageAlt)]
    public class CSharpLanguageService : ILanguageService
    {
        /// <summary>
        /// The language identifier.
        /// </summary>
        public const string Language = "C#";

        /// <summary>
        /// The alternate language identifier.
        /// </summary>
        public const string LanguageAlt = "csharp";

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
        public async Task<object> ExecuteAsync(
            IScript script,
            IScriptGlobals scriptGlobals = null,
            IExpando args = null,
            IContext executionContext = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(script, nameof(script));
            Requires.NotNull(script.SourceCode, nameof(script.SourceCode));

            if (script.SourceCode is string codeText)
            {
                var result = await CSharpScript.RunAsync(codeText, globals: scriptGlobals, cancellationToken: cancellationToken)
                    .PreserveThreadContext();
                return result.ReturnValue;
            }

            if (script.SourceCode is Stream codeStream)
            {
                var csscript = CSharpScript.Create(codeStream);
                var state = await csscript.RunAsync(globals: scriptGlobals, cancellationToken: cancellationToken).PreserveThreadContext();
                return state.ReturnValue;
            }

            // TODO localization
            throw new ScriptingException($"Source code type {script.GetType()} not supported. Please provide either a {typeof(string)} or a {typeof(Stream)}.");
        }
    }
}