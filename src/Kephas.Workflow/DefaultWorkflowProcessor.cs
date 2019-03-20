// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultWorkflowProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default workflow processor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Kephas.Workflow.Behaviors;
    using Kephas.Workflow.Behaviors.Composition;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// The default implementation of the <see cref="IWorkflowProcessor"/> service contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultWorkflowProcessor : IWorkflowProcessor
    {
        /// <summary>
        /// The behavior factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IActivityBehavior, ActivityBehaviorMetadata>> behaviorFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWorkflowProcessor"/> class.
        /// </summary>
        /// <param name="behaviorFactories">The behavior factories.</param>
        public DefaultWorkflowProcessor(ICollection<IExportFactory<IActivityBehavior, ActivityBehaviorMetadata>> behaviorFactories)
        {
            this.behaviorFactories = behaviorFactories;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultWorkflowProcessor> Logger { get; set; }

        /// <summary>
        /// Executes the activity asynchronously, enabling the activity execution behaviors.
        /// </summary>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="target">The activity target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        public async Task<object> ExecuteAsync(
            IActivity activity,
            object target,
            IExpando arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(activity, nameof(activity));

            var logger = context.ContextLogger.Merge(this.Logger);

            cancellationToken.ThrowIfCancellationRequested();

            // resolve the activity type which will execute the activity
            var activityInfo = this.GetActivityInfo(activity, context);

            cancellationToken.ThrowIfCancellationRequested();

            // resolve the arguments
            var executionArgs = await this.GetExecutionArgumentsAsync(activityInfo, arguments, context, cancellationToken)
                               .PreserveThreadContext();

            cancellationToken.ThrowIfCancellationRequested();

            // get the behaviors for execution
            var (behaviors, reversedBehaviors) = this.GetOrderedBehaviors(activityInfo, context);

            //...
            return null;
        }

        /// <summary>
        /// Gets the <see cref="IActivityInfo"/> for the activity to execute.
        /// </summary>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="activityContext">Context for the activity.</param>
        /// <returns>
        /// The activity information.
        /// </returns>
        protected virtual IActivityInfo GetActivityInfo(IActivity activity, IActivityContext activityContext)
        {
            var activityInfo = activity.GetTypeInfo() as IActivityInfo;
            return activityInfo;
        }

        /// <summary>
        /// Gets the activity arguments asynchronously.
        /// </summary>
        /// <param name="activityInfo">Information describing the activity.</param>
        /// <param name="input">The input.</param>
        /// <param name="activityContext">Context for the activity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the activity arguments.
        /// </returns>
        protected virtual Task<IExpando> GetExecutionArgumentsAsync(
            IActivityInfo activityInfo,
            IExpando input,
            IActivityContext activityContext,
            CancellationToken cancellationToken)
        {
            // TODO gather the parameters from the activity info and add the default values
            // to the input, if not already specified.
            return Task.FromResult(input);
        }

        /// <summary>
        /// Gets the behaviors for execution.
        /// </summary>
        /// <param name="activityInfo">Information describing the activity.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the behaviors in this collection.
        /// </returns>
        protected virtual (IEnumerable<IActivityBehavior> behaviors, IEnumerable<IActivityBehavior> reversedBehaviors) GetOrderedBehaviors(
            IActivityInfo activityInfo,
            IActivityContext context)
        {
            // TODO fix the check of the activity type
            var behaviors = this.behaviorFactories
                .Where(f => f.Metadata.ActivityType == null || activityInfo == f.Metadata.ActivityType.AsRuntimeTypeInfo())
                .OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .Select(f => f.CreateExportedValue())
                .ToList();
            return (behaviors, ((IEnumerable<IActivityBehavior>)behaviors).Reverse().ToList());
        }
    }
}