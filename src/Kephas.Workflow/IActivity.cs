// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IActivity interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;

    /// <summary>
    /// An activity is an executable instance which receives a target, arguments, and an executing context upon execution.
    /// </summary>
    public interface IActivity : IExpando, IInstance
    {
        /// <summary>
        /// Gets the target against which the activity is executed.
        /// </summary>
        /// <remarks>
        /// The target is typically the activity's container instance. 
        /// For example, a user entity may contain a ChangePassword activity,
        /// in which case the target is the user.
        /// </remarks>
        /// <value>
        /// The target.
        /// </value>
        object Target { get; }

        /// <summary>
        /// Gets the arguments for the execution.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        IExpando Arguments { get; }

        /// <summary>
        /// Gets the execution context.
        /// </summary>
        /// <value>
        /// The execution context.
        /// </value>
        IActivityContext Context { get; }

        /// <summary>
        /// Gets the activity state flags.
        /// </summary>
        /// <value>
        /// The activity state flags.
        /// </value>
        ActivityState State { get; }

        /// <summary>
        /// Executes the activity asynchronously.
        /// </summary>
        /// <param name="target">The target against which the activity is executed.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the execute.
        /// </returns>
        Task<IExpando> ExecuteAsync(
            object target,
            IExpando arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default);
    }
}