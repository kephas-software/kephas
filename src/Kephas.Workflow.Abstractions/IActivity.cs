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
    using Kephas.Data;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// An activity is an executable instance which receives a target, arguments, and an executing context upon execution.
    /// </summary>
    /// <remarks>
    /// An activity instance may be executed only once.
    /// To execute an activity multiple times, create for each execution an instance and execute it.
    /// During the execution, it may be canceled or paused,
    /// and also debuggers may be attached to provide development support.
    /// The state provide the flags during the execution.
    /// </remarks>
    public interface IActivity : IOperationResult, IInstance<IActivityInfo>, IIdentifiable
    {
        /// <summary>
        /// Gets or sets the target against which the activity is executed.
        /// </summary>
        /// <remarks>
        /// The target is typically the activity's container instance.
        /// For example, a user entity may contain a ChangePassword activity,
        /// in which case the target is the user.
        /// </remarks>
        /// <value>
        /// The target.
        /// </value>
        object? Target { get; set; }

        /// <summary>
        /// Gets or sets the arguments for the execution.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        IDynamic? Arguments { get; set; }

        /// <summary>
        /// Gets or sets the execution context.
        /// </summary>
        /// <value>
        /// The execution context.
        /// </value>
        IActivityContext? Context { get; set; }
    }
}