// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpScriptInterpreter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the C# script interpreter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.CSharp
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using Microsoft.CodeAnalysis.CSharp.Scripting;

    /// <summary>
    /// A C# script interpreter.
    /// </summary>
    [Language(Language)]
    public class CSharpScriptInterpreter : IScriptInterpreter
    {
        /// <summary>
        /// The language.
        /// </summary>
        public const string Language = "csharp";

        /// <summary>
        /// Executes the expression asynchronously.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="args">The arguments (optional).</param>
        /// <param name="executionContext">The execution context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        public async Task<object> ExecuteAsync(
            IScript script,
            IExpando args = null,
            IContext executionContext = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(script, nameof(script));
            Requires.NotNull(script.SourceCode, nameof(script.SourceCode));

            // TODO improve implementation

            if (script.SourceCode is string codeText)
            {
                var globals = new Globals { args = args ?? new Expando() };
                var result = await CSharpScript.RunAsync(codeText, globals: globals, cancellationToken: cancellationToken)
                    .PreserveThreadContext();
                return result.ReturnValue;
            }

            if (script.SourceCode is Stream codeStream)
            {
                var csscript = CSharpScript.Create(codeStream);
                var globals = new Globals { args = args ?? new Expando() };
                var state = await csscript.RunAsync(globals: globals, cancellationToken: cancellationToken).PreserveThreadContext();
                return state.ReturnValue;
            }

            // TODO localization
            throw new ScriptingException($"Source code type {script.GetType()} not supported. Please provide either a {typeof(string)} or a {typeof(Stream)}.");
        }

        /// <summary>
        /// The globals.
        /// </summary>
        public class Globals
        {
            /// <summary>
            /// Gets or sets the arguments.
            /// </summary>
            /// <value>
            /// The arguments.
            /// </value>
            public IExpando args { get; set; }
        }
    }
}