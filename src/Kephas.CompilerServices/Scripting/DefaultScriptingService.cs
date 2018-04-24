// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultScriptingService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default scripting service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.CompilerServices.Scripting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.CompilerServices.Scripting.Composition;
    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Services;

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
        /// Initializes a new instance of the <see cref="DefaultScriptingService"/> class.
        /// </summary>
        /// <param name="interpreterFactories">The interpreter factories.</param>
        public DefaultScriptingService(
            ICollection<IExportFactory<IInterpreter, InterpreterMetadata>> interpreterFactories)
        {
            Requires.NotNull(interpreterFactories, nameof(interpreterFactories));

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
        }

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
        public Task<object> ExecuteAsync(
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

            return interpreterFactory.CreateExportedValue().ExecuteAsync(script, args, executionContext, cancellationToken);
        }
    }
}