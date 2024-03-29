﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultScriptProcessor.cs" company="Kephas Software SRL">
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

    using Kephas;
    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Scripting.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default script processor.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultScriptProcessor : Loggable, IScriptProcessor
    {
        /// <summary>
        /// The language service factories.
        /// </summary>
        private readonly IDictionary<string, Lazy<ILanguageService, LanguageServiceMetadata>> languageServiceFactories =
            new Dictionary<string, Lazy<ILanguageService, LanguageServiceMetadata>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The scripting behavior factories.
        /// </summary>
        private readonly IDictionary<string, IList<Lazy<IScriptingBehavior, ScriptingBehaviorMetadata>>> scriptingBehaviorFactories =
            new Dictionary<string, IList<Lazy<IScriptingBehavior, ScriptingBehaviorMetadata>>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultScriptProcessor"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="languageServiceFactories">The language service factories.</param>
        /// <param name="scriptingBehaviorFactories">The scripting behavior factories.</param>
        public DefaultScriptProcessor(
            IInjectableFactory injectableFactory,
            ICollection<Lazy<ILanguageService, LanguageServiceMetadata>> languageServiceFactories,
            ICollection<Lazy<IScriptingBehavior, ScriptingBehaviorMetadata>> scriptingBehaviorFactories)
            : base(injectableFactory)
        {
            injectableFactory = injectableFactory ?? throw new ArgumentNullException(nameof(injectableFactory));
            languageServiceFactories = languageServiceFactories ?? throw new ArgumentNullException(nameof(languageServiceFactories));
            scriptingBehaviorFactories = scriptingBehaviorFactories ?? throw new ArgumentNullException(nameof(scriptingBehaviorFactories));

            this.InjectableFactory = injectableFactory;

            languageServiceFactories
                .Order()
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

            languageServiceFactories
                .Where(f => f.Metadata.Language != null)
                .SelectMany(f => f.Metadata.Language)
                .Distinct()
                .ForEach(l => this.scriptingBehaviorFactories.Add(l, new List<Lazy<IScriptingBehavior, ScriptingBehaviorMetadata>>()));

            scriptingBehaviorFactories
                .Order()
                .ForEach(f =>
                    {
                        if (f.Metadata.Language == null || f.Metadata.Language.Length == 0)
                        {
                            this.scriptingBehaviorFactories.Values.ForEach(list => list.Add(f));
                        }
                        else
                        {
                            f.Metadata.Language.ForEach(l => this.scriptingBehaviorFactories.TryGetValue(l)?.Add(f));
                        }
                    });
        }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        /// <value>
        /// The context factory.
        /// </value>
        public IInjectableFactory InjectableFactory { get; }

        /// <summary>
        /// Executes the expression asynchronously.
        /// </summary>
        /// <exception cref="ScriptingException">Thrown when a Scripting error condition occurs.</exception>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        public async Task<object?> ExecuteAsync(
            IScript script,
            object? args = null,
            Action<IScriptingContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            script = script ?? throw new ArgumentNullException(nameof(script));

            var languageServiceFactory = this.languageServiceFactories.TryGetValue(script.Language);
            if (languageServiceFactory == null)
            {
                throw new ScriptingException(Strings.DefaultScriptProcessor_ExecuteAsync_MissingLanguageService.FormatWith(script.Language));
            }

            var scriptArgs = args?.ToDynamic() ?? new Expando();
            var scriptingContext = this.CreateScriptingContext(script, scriptArgs, optionsConfig);
            var behaviors = this.scriptingBehaviorFactories.TryGetValue(script.Language)?.Select(f => f.Value).ToList() ?? new List<IScriptingBehavior>();

            foreach (var behavior in behaviors)
            {
                await behavior.BeforeExecuteAsync(scriptingContext, cancellationToken).PreserveThreadContext();
            }

            try
            {
                var result = await languageServiceFactory.Value
                                 .ExecuteAsync(script, scriptingContext.ScriptGlobals, scriptArgs, scriptingContext.ExecutionContext, cancellationToken)
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
                throw new ScriptingException(Strings.DefaultScriptProcessor_ExecuteAsync_Exception, scriptingContext.Exception);
            }

            return scriptingContext.Result;
        }

        /// <summary>
        /// Creates the scripting context.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new scripting context.
        /// </returns>
        protected virtual IScriptingContext CreateScriptingContext(
            IScript script,
            IDynamic args,
            Action<IScriptingContext>? optionsConfig = null)
        {
            var scriptingContext = this.InjectableFactory.Create<ScriptingContext>();
            scriptingContext.Script = script;
            scriptingContext.Args = args;
            var scriptGlobals = new ScriptGlobals();
            scriptingContext.ScriptGlobals = scriptGlobals;

            optionsConfig?.Invoke(scriptingContext);

            // do the deconstruction after the configuration is invoked, to allow changing the Args
            // and the DeconstructArgs flags.
            scriptGlobals.SetArgs(scriptingContext.Args, scriptingContext.DeconstructArgs);

            return scriptingContext;
        }
    }
}