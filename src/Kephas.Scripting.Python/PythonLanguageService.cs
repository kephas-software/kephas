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
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using IronPython.Hosting;
    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Microsoft.Scripting;
    using Microsoft.Scripting.Hosting;

    /// <summary>
    /// A Python language service.
    /// </summary>
    [Language(Language, LanguageAlt)]
    public class PythonLanguageService : Loggable, ILanguageService, IInitializable
    {
        /// <summary>
        /// The language identifier.
        /// </summary>
        public const string Language = "Python";

        /// <summary>
        /// The alternate language identifier.
        /// </summary>
        public const string LanguageAlt = "py";

        private const string ReturnValueVariableName = "returnValue";

        private readonly IConfiguration<PythonSettings>? pythonConfiguration;
        private readonly IAppRuntime? appRuntime;
        private readonly ScriptEngine engine;
        private bool isInitialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PythonLanguageService"/> class.
        /// </summary>
        /// <param name="pythonConfiguration">The Python engine configuration.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public PythonLanguageService(
            IConfiguration<PythonSettings>? pythonConfiguration = null,
            IAppRuntime? appRuntime = null,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.pythonConfiguration = pythonConfiguration;
            this.appRuntime = appRuntime;
            this.engine = IronPython.Hosting.Python.CreateEngine();
        }

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
        public object? Execute(
            IScript script,
            IScriptGlobals? scriptGlobals = null,
            IDynamic? args = null,
            IContext? executionContext = null)
        {
            // http://putridparrot.com/blog/hosting-ironpython-in-a-c-application/

            this.Initialize();

            args ??= new Expando();
            scriptGlobals ??= new ScriptGlobals(args);

            var scope = this.CreateGlobalScope(scriptGlobals);
            var source = this.GetScriptSource(script);

            var result = source.Execute(scope);
            var returnValue = this.GetReturnValue(result, scope);
            return returnValue;
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
        public async Task<object?> ExecuteAsync(
            IScript script,
            IScriptGlobals? scriptGlobals = null,
            IDynamic? args = null,
            IContext? executionContext = null,
            CancellationToken cancellationToken = default)
        {
            // http://putridparrot.com/blog/hosting-ironpython-in-a-c-application/

            this.Initialize();

            args ??= new Expando();
            scriptGlobals ??= new ScriptGlobals(args);

            var scope = this.CreateGlobalScope(scriptGlobals);
            var source = await this.GetScriptSourceAsync(script, cancellationToken).PreserveThreadContext();

            await Task.Yield();

            var result = source.Execute(scope);
            var returnValue = this.GetReturnValue(result, scope);
            return returnValue;
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        public void Initialize(IContext? context = null)
        {
            if (this.isInitialized)
            {
                return;
            }

            lock (this.engine)
            {
                if (this.isInitialized)
                {
                    return;
                }

                var settings = this.pythonConfiguration?.GetSettings(context);
                this.SetSearchPaths(settings);
                this.SetGlobalModules(settings);

                this.isInitialized = true;
            }
        }

        private void SetSearchPaths(PythonSettings? settings)
        {
            var basePath = this.appRuntime?.GetAppLocation();
            var searchPaths = basePath != null
                ? settings?.SearchPaths?.Select(p => Path.Combine(basePath, p)).ToArray()
                : settings?.SearchPaths;
            if (searchPaths != null)
            {
                this.engine.SetSearchPaths(searchPaths);
            }
        }

        private void SetGlobalModules(PythonSettings? settings)
        {
            var globalScope = this.engine.CreateScope();
            this.engine.Runtime.Globals = globalScope;

            globalScope.ImportModule("clr");
            this.engine.Execute("import clr", globalScope);

            var globalModules = settings?.GlobalModules;
            if (globalModules == null)
            {
                return;
            }

            foreach (var globalModule in globalModules)
            {
                try
                {
                    globalScope.ImportModule(globalModule);
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Could not import global module '{module}' from '{modules}'.", globalModule, globalModules);
                }
            }
        }

        private ScriptSource GetScriptSource(IScript script)
        {
            var source = script switch
            {
                IStreamScript streamScript => this.engine.CreateScriptSource(
                    new BasicStreamContentProvider(streamScript.GetSourceCodeStream()), $"dynamicCode.py"),
                _ => this.engine.CreateScriptSourceFromString(script.GetSourceCode(), SourceCodeKind.AutoDetect),
            };
            return source;
        }

        private async Task<ScriptSource> GetScriptSourceAsync(IScript script, CancellationToken cancellationToken)
        {
            var source = script switch
            {
                IStreamScript streamScript => this.engine.CreateScriptSource(
                    new BasicStreamContentProvider(streamScript.GetSourceCodeStream()), $"dynamicCode.py"),
                _ => this.engine.CreateScriptSourceFromString(
                    await script.GetSourceCodeAsync(cancellationToken).PreserveThreadContext(),
                    SourceCodeKind.AutoDetect),
            };
            return source;
        }

        private ScriptScope CreateGlobalScope(IScriptGlobals scriptGlobals)
        {
            var scope = this.engine.CreateScope();

            foreach (var (key, value) in scriptGlobals.ToDictionary(k => k.ToCamelCase(), v => v))
            {
                scope.SetVariable(key, value);
            }

            return scope;
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
