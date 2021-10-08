// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivityBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IActivityBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Base contract for controlling the execution of activities.
    /// </summary>
    public interface IActivityBehavior
    {
        /// <summary>
        /// Interception called before invoking the service to execute the activity.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        Task BeforeExecuteAsync(IActivityContext context, CancellationToken token);

        /// <summary>
        /// Interception called after invoking the service to execute the activity.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        /// <remarks>
        /// The context will contain the response returned by the service.
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        Task AfterExecuteAsync(IActivityContext context, CancellationToken token);
    }

    /// <summary>
    /// Singleton application service contract for controlling the execution of activities.
    /// </summary>
    /// <typeparam name="TActivity">Type of the activity.</typeparam>
    [SingletonAppServiceContract(AllowMultiple = true, ContractType = typeof(IActivityBehavior))]
    public interface IActivityBehavior<TActivity> : IActivityBehavior
        where TActivity : IActivity
    {
    }
}