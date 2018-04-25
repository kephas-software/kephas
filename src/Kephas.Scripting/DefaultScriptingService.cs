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
        private readonly IDictionary<string, IExportFactory<IInterpreter, InterpreterMetadata>> interpreterFactories =
            new Dictionary<string, IExportFactory<IInterpreter, InterpreterMetadata>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The interpreter factories.
        /// </summary>
        private readonly IDictionary<string, IList<IExportFactory<IInterpreterBehavior, InterpreterBehaviorMetadata>>> interpreterBehaviorFactories =
            new Dictionary<string, IList<IExportFactory<IInterpreterBehavior, InterpreterBehaviorMetadata>>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultScriptingService"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="interpreterFactories">The interpreter factories.</param>
        /// <param name="interpreterBehaviorFactories">The interpreter behavior factories.</param>
        public DefaultScriptingService(
            IAmbientServices ambientServices,
            ICollection<IExportFactory<IInterpreter, InterpreterMetadata>> interpreterFactories,
            ICollection<IExportFactory<IInterpreterBehavior, InterpreterBehaviorMetadata>> interpreterBehaviorFactories)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(interpreterFactories, nameof(interpreterFactories));
            Requires.NotNull(interpreterBehaviorFactories, nameof(interpreterBehaviorFactories));

            this.AmbientServices = ambientServices;

            interpreterFactories
                .OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ForEach(f =>
                    {
                        if (!this.interpreterFactories.ContainsKey(f.Metadata.Language))
                        {
                            this.interpreterFactories.Add(f.Metadata.Language, f);
                        }
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
                            list = new List<IExportFactory<IInterpreterBehavior, InterpreterBehaviorMetadata>>();
                            this.interpreterBehaviorFactories.Add(f.Metadata.Language, list);
                        }

                        list.Add(f);
                    });
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

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

            var scriptingContext = new ScriptingContext(this.AmbientServices)
                                       {
                                           Identity = executionContext?.Identity,
                                           Script = script,
                                           Args = args,
                                           ExecutionContext = executionContext
                                       };
            var behaviors = this.interpreterBehaviorFactories.TryGetValue(script.Language)?.Select(f => f.CreateExportedValue()).ToList() ?? new List<IInterpreterBehavior>();

            foreach (var behavior in behaviors)
            {
                await behavior.BeforeExecuteAsync(scriptingContext, cancellationToken).PreserveThreadContext();
            }

            try
            {
                var result = await interpreterFactory.CreateExportedValue()
                                 .ExecuteAsync(script, args, executionContext, cancellationToken)
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
    }
}