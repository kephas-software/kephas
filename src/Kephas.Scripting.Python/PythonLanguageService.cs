// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PythonLanguageService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the python language service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Python
{
    using System.CodeDom;
    using System.Dynamic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using IronPython.Hosting;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;

    using Microsoft.Scripting;
    using Microsoft.Scripting.Hosting;

    /// <summary>
    /// A Python language service.
    /// </summary>
    [Language(Language, LanguageAlt)]
    public class PythonLanguageService : ILanguageService
    {
        /// <summary>
        /// The language identifier.
        /// </summary>
        public const string Language = "Python";

        /// <summary>
        /// The alternate language identifier.
        /// </summary>
        public const string LanguageAlt = "py";

        /// <summary>
        /// Name of the return value variable.
        /// </summary>
        private const string ReturnValueVariableName = "returnValue";

        private ScriptEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="PythonLanguageService"/> class.
        /// </summary>
        public PythonLanguageService()
        {
            this.engine = IronPython.Hosting.Python.CreateEngine();
        }

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
        public Task<object> ExecuteAsync(IScript script, IScriptGlobals scriptGlobals = null, IExpando args = null, IContext executionContext = null, CancellationToken cancellationToken = default)
        {
            // http://putridparrot.com/blog/hosting-ironpython-in-a-c-application/

            var scope = this.engine.CreateScope();

            scope.ImportModule("clr");
            this.engine.Execute("import clr", scope);

            args = args ?? new Expando();
            scriptGlobals = scriptGlobals ?? new ScriptGlobals { Args = args };

            foreach (var kv in scriptGlobals.ToDictionary(k => k.ToCamelCase(), v => v))
            {
                scope.SetVariable(kv.Key, kv.Value);
            }

            var source = script.SourceCode is string codeText
                ? this.engine.CreateScriptSourceFromString(codeText, SourceCodeKind.AutoDetect)
                : script.SourceCode is Stream codeStream
                    ? this.engine.CreateScriptSource(new BasicStreamContentProvider(codeStream), $"dynamicCode.py")
                    // TODO localization
                    : throw new ScriptingException($"Source code type {script.GetType()} not supported. Please provide either a {typeof(string)} or a {typeof(Stream)}.");

            var result = source.Execute(scope);
            var returnValue = this.GetReturnValue(result, scope);
            return Task.FromResult((object)returnValue);
        }

        private object GetReturnValue(dynamic result, ScriptScope scope)
        {
            return scope.TryGetVariable(ReturnValueVariableName, out var value)
                ? value
                : result;
        }

        private class BasicStreamContentProvider : StreamContentProvider
        {
            private readonly Stream stream;

            public BasicStreamContentProvider(Stream stream)
            {
                this.stream = stream;
            }

            public override Stream GetStream() => this.stream;
        }
    }
}
