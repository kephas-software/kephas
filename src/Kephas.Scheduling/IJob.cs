// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJob.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJob interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Contract for automated pieces of work that can be performed at either a particular time, or on a recurring schedule.
    /// </summary>
    public interface IJob : IExpando, IInstance<IJobInfo>, IDisposable
    {
        /// <summary>
        /// Gets the arguments for the execution.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        IExpando Arguments { get; }

        /// <summary>
        /// Gets the job's execution state.
        /// </summary>
        /// <value>
        /// The execution state.
        /// </value>
        OperationState State { get; }

        /// <summary>
        /// Gets the execution result.
        /// </summary>
        /// <value>
        /// The execution result.
        /// </value>
        IOperationResult Result { get; }
    }
}