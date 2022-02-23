// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PythonLanguageService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the python language service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using IronPython.Hosting;
    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Configuration.Interaction;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.IO;
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
    public class PythonLanguageService : Loggable, ILanguageService, IInitializable, IDisposable
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
        private readonly ILocationsManager locationsManager;
        private readonly IEventSubscription? subscription;
        private readonly object engineSync = new ();

        private ScriptEngine? engine;
        private ICollection<string> globalModules = Array.Empty<string>();
        private bool isInitialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PythonLanguageService"/> class.
        /// </summary>
        /// <param name="pythonConfiguration">The Python engine configuration.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <param name="locationsManager">Optional. The locations manager.</param>
        /// <param name="eventHub">Optional. The event hub used for configuration change notifications.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public PythonLanguageService(
            IConfiguration<PythonSettings>? pythonConfiguration = null,
            IAppRuntime? appRuntime = null,
            ILocationsManager? locationsManager = null,
            IEventHub? eventHub = null,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.pythonConfiguration = pythonConfiguration;
            this.appRuntime = appRuntime;
            this.locationsManager = locationsManager ?? new FolderLocationsManager();
            this.subscription = eventHub?.Subscribe<ConfigurationChangedSignal>((s, ctx) =>
            {
                if (s.SettingsType != typeof(PythonSettings).FullName) return;
                this.isInitialized = false;
            });
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

            var scope = this.CreateScriptScope(scriptGlobals, this.pythonConfiguration?.GetSettings(executionContext));
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

            var scope = this.CreateScriptScope(scriptGlobals, this.pythonConfiguration?.GetSettings(executionContext));
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

            lock (this.engineSync)
            {
                if (this.isInitialized)
                {
                    return;
                }

                this.engine = IronPython.Hosting.Python.CreateEngine();
                var settings = this.pythonConfiguration?.GetSettings(context);
                this.SetSearchPaths(settings);
                this.SetGlobalModules(settings);

                this.isInitialized = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.isInitialized = false;
                this.subscription?.Dispose();
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
                var fullSearchPaths = this.locationsManager.GetLocations(searchPaths, basePath, "pythonSearchPaths");
                this.engine!.SetSearchPaths(fullSearchPaths.ToList());
            }
        }

        private void SetGlobalModules(PythonSettings? settings)
        {
            var globalScope = this.engine!.CreateScope();
            this.engine.Runtime.Globals = globalScope;

            globalScope.ImportModule("clr");
            this.engine.Execute("import clr", globalScope);

            this.globalModules = this.GetGlobalModules().ToList();
            this.Logger.Info("Python global modules: {globalModules}.", globalModules);

            if (this.ShouldPreloadGlobalModules(settings))
            {
                this.PreloadGlobalModules(globalScope);
            }
        }

        private void PreloadGlobalModules(ScriptScope scope)
        {
            foreach (var globalModule in this.globalModules)
            {
                try
                {
                    scope.ImportModule(globalModule);
                    this.engine!.Execute($"import {globalModule}", scope);
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Could not import global module '{module}' from '{modules}'.", globalModule, this.globalModules);
                }
            }
        }

        private IEnumerable<string> GetGlobalModules()
        {
            var searchPaths = this.engine!.GetSearchPaths();
            foreach (var searchPath in searchPaths)
            {
                foreach (var moduleFile in Directory.EnumerateFiles(searchPath, "*.py"))
                {
                    yield return Path.GetFileNameWithoutExtension(moduleFile);
                }
            }
        }

        private ScriptSource GetScriptSource(IScript script)
        {
            var source = script switch
            {
                IStreamScript streamScript => this.engine!.CreateScriptSource(
                    new BasicStreamContentProvider(streamScript.GetSourceCodeStream()), $"dynamicCode.py"),
                _ => this.engine!.CreateScriptSourceFromString(script.GetSourceCode(), SourceCodeKind.AutoDetect),
            };
            return source;
        }

        private async Task<ScriptSource> GetScriptSourceAsync(IScript script, CancellationToken cancellationToken)
        {
            var source = script switch
            {
                IStreamScript streamScript => this.engine!.CreateScriptSource(
                    new BasicStreamContentProvider(streamScript.GetSourceCodeStream()), $"dynamicCode.py"),
                _ => this.engine!.CreateScriptSourceFromString(
                    await script.GetSourceCodeAsync(cancellationToken).PreserveThreadContext(),
                    SourceCodeKind.AutoDetect),
            };
            return source;
        }

        private bool ShouldPreloadGlobalModules(PythonSettings? settings)
            => settings?.PreloadGlobalModules ?? false;

        private ScriptScope CreateScriptScope(IScriptGlobals scriptGlobals, PythonSettings? settings)
        {
            var scope = this.engine!.CreateScope();

            foreach (var (key, value) in scriptGlobals.ToDictionary(k => k.ToCamelCase(), v => v))
            {
                scope.SetVariable(key, value);
            }

            if (this.ShouldPreloadGlobalModules(settings))
            {
                this.PreloadGlobalModules(scope);
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
