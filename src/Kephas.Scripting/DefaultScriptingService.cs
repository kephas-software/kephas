// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultScriptingService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default scripting service class.
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
    /// A default scripting service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultScriptingService : IScriptingService
    {
        /// <summary>
        /// The interpreter factories.
        /// </summary>
        private readonly IDictionary<string, IExportFactory<IScriptInterpreter, ScriptInterpreterMetadata>> interpreterFactories =
            new Dictionary<string, IExportFactory<IScriptInterpreter, ScriptInterpreterMetadata>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The interpreter factories.
        /// </summary>
        private readonly IDictionary<string, IList<IExportFactory<IScriptInterpreterBehavior, ScriptInterpreterBehaviorMetadata>>> interpreterBehaviorFactories =
            new Dictionary<string, IList<IExportFactory<IScriptInterpreterBehavior, ScriptInterpreterBehaviorMetadata>>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultScriptingService"/> class.
        /// </summary>
        /// <param name="compositionContext">The ambient services.</param>
        /// <param name="interpreterFactories">The interpreter factories.</param>
        /// <param name="interpreterBehaviorFactories">The interpreter behavior factories.</param>
        public DefaultScriptingService(
            ICompositionContext compositionContext,
            ICollection<IExportFactory<IScriptInterpreter, ScriptInterpreterMetadata>> interpreterFactories,
            ICollection<IExportFactory<IScriptInterpreterBehavior, ScriptInterpreterBehaviorMetadata>> interpreterBehaviorFactories)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(interpreterFactories, nameof(interpreterFactories));
            Requires.NotNull(interpreterBehaviorFactories, nameof(interpreterBehaviorFactories));

            this.CompositionContext = compositionContext;

            interpreterFactories
                .OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ForEach(f =>
                    {
                        f.Metadata.Language.ForEach(
                            l =>
                                {
                                    if (!this.interpreterFactories.ContainsKey(l))
                                    {
                                        this.interpreterFactories.Add(l, f);
                                    }
                                });
                    });

            interpreterBehaviorFactories
                .OrderBy(f => f.Metadata.Language)
                .ThenBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ForEach(f =>
                    {
                        var list = this.interpreterBehaviorFactories.TryGetValue(f.Metadata.Language);
                        if (list == null)
                        {
                            list = new List<IExportFactory<IScriptInterpreterBehavior, ScriptInterpreterBehaviorMetadata>>();
                            this.interpreterBehaviorFactories.Add(f.Metadata.Language, list);
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
        public ILogger<DefaultScriptingService> Logger { get; set; }

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

            var interpreterFactory = this.interpreterFactories.TryGetValue(script.Language);
            if (interpreterFactory == null)
            {
                // TODO localize
                throw new ScriptingException($"The script language '{script.Language}' does not have an associated interpreter.");
            }

            var scriptingContext = this.CreateScriptingContext(script, args, executionContext);
            var behaviors = this.interpreterBehaviorFactories.TryGetValue(script.Language)?.Select(f => f.CreateExportedValue()).ToList() ?? new List<IScriptInterpreterBehavior>();

            foreach (var behavior in behaviors)
            {
                await behavior.BeforeExecuteAsync(scriptingContext, cancellationToken).PreserveThreadContext();
            }

            try
            {
                var result = await interpreterFactory.CreateExportedValue()
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