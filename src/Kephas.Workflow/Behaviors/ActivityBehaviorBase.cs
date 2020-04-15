// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activity behavior base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Kephas.Workflow.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;

    /// <summary>
    /// Base class for activity behaviors.
    /// </summary>
    /// <typeparam name="TActivity">Type of the activity.</typeparam>
    public abstract class ActivityBehaviorBase<TActivity> : Loggable, IActivityBehavior<TActivity>
        where TActivity : IActivity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityBehaviorBase{TActivity}"/>
        /// class.
        /// </summary>
        /// <param name="logManager">Optional. Manager for log.</param>
        public ActivityBehaviorBase(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Interception called after invoking the service to execute the activity.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public virtual Task AfterExecuteAsync(IActivityContext context, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interception called before invoking the service to execute the activity.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public virtual Task BeforeExecuteAsync(IActivityContext context, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
