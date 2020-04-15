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
    using System;
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
    public class DefaultWorkflowProcessor : Loggable, IWorkflowProcessor
    {
        private readonly IContextFactory contextFactory;
        private readonly ICollection<IExportFactory<IActivityBehavior, ActivityBehaviorMetadata>> behaviorFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWorkflowProcessor"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="behaviorFactories">The behavior factories.</param>
        public DefaultWorkflowProcessor(
            IContextFactory contextFactory,
            ICollection<IExportFactory<IActivityBehavior, ActivityBehaviorMetadata>> behaviorFactories)
            : base(contextFactory)
        {
            this.contextFactory = contextFactory;
            this.behaviorFactories = behaviorFactories;
        }

        /// <summary>
        /// Executes the activity asynchronously, enabling the activity execution behaviors.
        /// </summary>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="target">The activity target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the execution result.
        /// </returns>
        public async Task<object?> ExecuteAsync(
            IActivity activity,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(activity, nameof(activity));

            using var context = this.CreateActivityContext(optionsConfig);

            var logger = context.Logger.Merge(this.Logger);

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

            cancellationToken.ThrowIfCancellationRequested();

            //... TODO improve
            await this.ApplyBeforeExecuteBehaviorsAsync(behaviors, context, cancellationToken).PreserveThreadContext();

            //... TODO improve
            try
            {
                var result = await this.ExecuteActivityAsync(activityInfo, activity, target, executionArgs, context, cancellationToken)
                    .PreserveThreadContext();
                context.Result = result;
            }
            catch (Exception ex)
            {
                context.Exception = ex;
            }

            //... TODO improve
            await this.ApplyAfterExecuteBehaviorsAsync(reversedBehaviors, context, cancellationToken).PreserveThreadContext();

            if (context.Exception != null)
            {
                throw context.Exception;
            }

            return context.Result;
        }

        /// <summary>
        /// Executes the activity asynchronously.
        /// </summary>
        /// <param name="activityInfo">Information describing the activity.</param>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="target">The activity target.</param>
        /// <param name="executionArgs">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the execute activity.
        /// </returns>
        protected virtual async Task<object?> ExecuteActivityAsync(IActivityInfo activityInfo, IActivity activity, object? target, IExpando? executionArgs, IActivityContext context, CancellationToken cancellationToken)
        {
            var timeout = context.Timeout;
            if (timeout.HasValue && timeout.Value > TimeSpan.Zero)
            {
                // TODO if the task times out should be canceled. Also, the delay task should be canceled.
                var executeTask = activityInfo.ExecuteAsync(activity, target, executionArgs, context, cancellationToken);
                var completedTask = await Task.WhenAny(executeTask, Task.Delay(timeout.Value)).PreserveThreadContext();
                if (completedTask == executeTask)
                {
                    return executeTask.Result;
                }

                throw new TimeoutException();
            }

            return await activityInfo.ExecuteAsync(activity, target, executionArgs, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Creates an activity context for the current processing.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new activity context.
        /// </returns>
        protected virtual IActivityContext CreateActivityContext(Action<IActivityContext>? optionsConfig = null)
        {
            var context = this.contextFactory.CreateContext<ActivityContext>();
            optionsConfig?.Invoke(context);
            return context;
        }

        /// <summary>
        /// Applies the behaviors invoking the <see cref="IActivityBehavior.BeforeExecuteAsync"/> asynchronously.
        /// </summary>
        /// <param name="behaviors">The behaviors.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task ApplyBeforeExecuteBehaviorsAsync(IEnumerable<IActivityBehavior> behaviors, IActivityContext context, CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.BeforeExecuteAsync(context, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Applies the behaviors invoking the <see cref="IActivityBehavior.AfterExecuteAsync"/> asynchronously.
        /// </summary>
        /// <param name="behaviors">The behaviors.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task ApplyAfterExecuteBehaviorsAsync(IEnumerable<IActivityBehavior> behaviors, IActivityContext context, CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.AfterExecuteAsync(context, cancellationToken).PreserveThreadContext();
            }
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
            var activityInfo = activity.GetTypeInfo();
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
                .Order()
                .GetServices()
                .ToList();
            return (behaviors, ((IEnumerable<IActivityBehavior>)behaviors).Reverse().ToList());
        }
    }
}