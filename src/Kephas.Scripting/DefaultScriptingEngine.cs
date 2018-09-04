// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultScriptingEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default scripting engine class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Scripting.Composition;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default scripting engine.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultScriptingEngine : IScriptingEngine
    {
        /// <summary>
        /// The language service factories.
        /// </summary>
        private readonly IDictionary<string, IExportFactory<ILanguageService, LanguageServiceMetadata>> languageServiceFactories =
            new Dictionary<string, IExportFactory<ILanguageService, LanguageServiceMetadata>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The scripting behavior factories.
        /// </summary>
        private readonly IDictionary<string, IList<IExportFactory<IScriptingBehavior, ScriptingBehaviorMetadata>>> scriptingBehaviorFactories =
            new Dictionary<string, IList<IExportFactory<IScriptingBehavior, ScriptingBehaviorMetadata>>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultScriptingEngine"/> class.
        /// </summary>
        /// <param name="compositionContext">The ambient services.</param>
        /// <param name="languageServiceFactories">The language service factories.</param>
        /// <param name="scriptingBehaviorFactories">The scripting behavior factories.</param>
        public DefaultScriptingEngine(
            ICompositionContext compositionContext,
            ICollection<IExportFactory<ILanguageService, LanguageServiceMetadata>> languageServiceFactories,
            ICollection<IExportFactory<IScriptingBehavior, ScriptingBehaviorMetadata>> scriptingBehaviorFactories)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(languageServiceFactories, nameof(languageServiceFactories));
            Requires.NotNull(scriptingBehaviorFactories, nameof(scriptingBehaviorFactories));

            this.CompositionContext = compositionContext;

            languageServiceFactories
                .OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ForEach(f =>
                    {
                        f.Metadata.Language.ForEach(
                            l =>
                                {
                                    if (!this.languageServiceFactories.ContainsKey(l))
                                    {
                                        this.languageServiceFactories.Add(l, f);
                                    }
                                });
                    });

            scriptingBehaviorFactories
                .OrderBy(f => f.Metadata.Language)
                .ThenBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ForEach(f =>
                    {
                        var list = this.scriptingBehaviorFactories.TryGetValue(f.Metadata.Language);
                        if (list == null)
                        {
                            list = new List<IExportFactory<IScriptingBehavior, ScriptingBehaviorMetadata>>();
                            this.scriptingBehaviorFactories.Add(f.Metadata.Language, list);
                        }

                        list.Add(f);
                    });
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultScriptingEngine> Logger { get; set; }

        /// <summary>
        /// Gets a context for the dependency injection/composition.
        /// </summary>
        /// <value>
        /// The composition context.
        /// </value>
        public ICompositionContext CompositionContext { get; }

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

            var languageServiceFactory = this.languageServiceFactories.TryGetValue(script.Language);
            if (languageServiceFactory == null)
            {
                // TODO localize
                throw new ScriptingException($"The script language '{script.Language}' does not have an associated language service.");
            }

            var scriptingContext = this.CreateScriptingContext(script, args, executionContext);
            var behaviors = this.scriptingBehaviorFactories.TryGetValue(script.Language)?.Select(f => f.CreateExportedValue()).ToList() ?? new List<IScriptingBehavior>();

            foreach (var behavior in behaviors)
            {
                await behavior.BeforeExecuteAsync(scriptingContext, cancellationToken).PreserveThreadContext();
            }

            try
            {
                var result = await languageServiceFactory.CreateExportedValue()
                                 .ExecuteAsync(script, scriptingContext.ScriptGlobals, args, executionContext, cancellationToken)
                                 .PreserveThreadContext();
                scriptingContext.Result = result;
            }
            catch (Exception ex)
            {
                scriptingContext.Exception = ex;
            }

            behaviors.Reverse();

            foreach (var behavior in behaviors)
            {
                await behavior.AfterExecuteAsync(scriptingContext, cancellationToken).PreserveThreadContext();
            }

            if (scriptingContext.Exception != null)
            {
                // TODO localization
                throw new ScriptingException($"An error occurred while executing script.", scriptingContext.Exception);
            }

            return scriptingContext.Result;
        }

        /// <summary>
        /// Creates the scripting context.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="args">The arguments (optional).</param>
        /// <param name="executionContext">The execution context (optional).</param>
        /// <returns>
        /// The new scripting context.
        /// </returns>
        protected virtual IScriptingContext CreateScriptingContext(IScript script, IExpando args, IContext executionContext)
        {
            var scriptingContext = new ScriptingContext(this.CompositionContext)
                                       {
                                           Identity = executionContext?.Identity,
                                           Script = script,
                                           Args = args,
                                           ExecutionContext = executionContext,
                                           ScriptGlobals = new ScriptGlobals { Args = args }
                                       };
            return scriptingContext;
        }
    }
}