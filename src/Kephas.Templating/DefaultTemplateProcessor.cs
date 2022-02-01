// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTemplateProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default template processor.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultTemplateProcessor : Loggable, ITemplateProcessor
    {
        /// <summary>
        /// The language service factories.
        /// </summary>
        private readonly IDictionary<string, Lazy<ITemplatingEngine, TemplatingEngineMetadata>> engineFactories =
            new Dictionary<string, Lazy<ITemplatingEngine, TemplatingEngineMetadata>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The scripting behavior factories.
        /// </summary>
        private readonly IDictionary<string, IList<Lazy<ITemplateProcessingBehavior, TemplateProcessingBehaviorMetadata>>> behaviorFactories =
            new Dictionary<string, IList<Lazy<ITemplateProcessingBehavior, TemplateProcessingBehaviorMetadata>>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTemplateProcessor"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="engineFactories">The templating engine factories.</param>
        /// <param name="behaviorFactories">The template processing behavior factories.</param>
        public DefaultTemplateProcessor(
            IContextFactory contextFactory,
            ICollection<Lazy<ITemplatingEngine, TemplatingEngineMetadata>> engineFactories,
            ICollection<Lazy<ITemplateProcessingBehavior, TemplateProcessingBehaviorMetadata>> behaviorFactories)
            : base(contextFactory)
        {
            contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            engineFactories = engineFactories ?? throw new ArgumentNullException(nameof(engineFactories));
            behaviorFactories = behaviorFactories ?? throw new ArgumentNullException(nameof(behaviorFactories));

            this.ContextFactory = contextFactory;

            engineFactories
                .Order()
                .ForEach(f =>
                    {
                        f.Metadata.TemplateKind.ForEach(
                            l =>
                                {
                                    if (!this.engineFactories.ContainsKey(l))
                                    {
                                        this.engineFactories.Add(l, f);
                                    }
                                });
                    });

            engineFactories
                .Where(f => f.Metadata.TemplateKind != null)
                .SelectMany(f => f.Metadata.TemplateKind)
                .Distinct()
                .ForEach(l => this.behaviorFactories.Add(l, new List<Lazy<ITemplateProcessingBehavior, TemplateProcessingBehaviorMetadata>>()));

            behaviorFactories
                .Order()
                .ForEach(f =>
                    {
                        if (f.Metadata.TemplateKind == null || f.Metadata.TemplateKind.Length == 0)
                        {
                            this.behaviorFactories.Values.ForEach(list => list.Add(f));
                        }
                        else
                        {
                            f.Metadata.TemplateKind.ForEach(l => this.behaviorFactories.TryGetValue(l)?.Add(f));
                        }
                    });
        }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        /// <value>
        /// The context factory.
        /// </value>
        public IContextFactory ContextFactory { get; }

        /// <summary>
        /// Processes the provided template asynchronously returning the processed output.
        /// </summary>
        /// <typeparam name="T">The type of the bound model.</typeparam>
        /// <param name="template">The template to be interpreted/executed.</param>
        /// <param name="model">Optional. The template model.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        public async Task<object?> ProcessAsync<T>(
            ITemplate template,
            T? model = default,
            Action<ITemplateProcessingContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            template = template ?? throw new ArgumentNullException(nameof(template));

            var engineFactory = this.engineFactories.TryGetValue(template.Kind);
            if (engineFactory == null)
            {
                throw new TemplatingException(Strings.DefaultTemplateProcessor_ProcessAsync_MissingEngine.FormatWith(template.Kind, template.Name), template);
            }

            var processingContext = this.CreateTemplateProcessingContext(template, model, optionsConfig);
            var behaviors = this.behaviorFactories.TryGetValue(template.Kind)?.Select(f => f.Value).ToList() ?? new List<ITemplateProcessingBehavior>();

            foreach (var behavior in behaviors)
            {
                await behavior.BeforeProcessAsync(processingContext, cancellationToken).PreserveThreadContext();
            }

            try
            {
                var result = await engineFactory.Value
                    .ProcessAsync(template, model, processingContext, cancellationToken)
                    .PreserveThreadContext();
                processingContext.Result = result;
            }
            catch (Exception ex)
            {
                processingContext.Exception = ex;
            }

            behaviors.Reverse();

            foreach (var behavior in behaviors)
            {
                await behavior.AfterProcessAsync(processingContext, cancellationToken).PreserveThreadContext();
            }

            if (processingContext.Exception != null || (processingContext.Result?.HasErrors() ?? false))
            {
                var innerException = processingContext.Exception
                                     ?? new AggregateException(processingContext.Result!.Exceptions);
                throw new TemplatingException(Strings.DefaultTemplateProcessor_ProcessAsync_Exception.FormatWith(template.Name), innerException, template, processingContext.Result);
            }

            return processingContext.Result?.Value;
        }

        /// <summary>
        /// Creates the scripting context.
        /// </summary>
        /// <param name="template">The template to be processed.</param>
        /// <param name="model">The model.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new <see cref="TemplateProcessingContext"/>.
        /// </returns>
        protected virtual ITemplateProcessingContext CreateTemplateProcessingContext(
            ITemplate template,
            object? model,
            Action<ITemplateProcessingContext>? optionsConfig = null)
        {
            var processingContext = this.ContextFactory.CreateContext<TemplateProcessingContext>();
            processingContext.Template = template;
            processingContext.Model = model;

            optionsConfig?.Invoke(processingContext);
            return processingContext;
        }
    }
}