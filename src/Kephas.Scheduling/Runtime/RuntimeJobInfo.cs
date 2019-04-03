// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeJobInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime job information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Scheduling.Reflection;
    using Kephas.Workflow;
    using Kephas.Workflow.Runtime;

    /// <summary>
    /// Information about the runtime job.
    /// </summary>
    public class RuntimeJobInfo : RuntimeActivityInfo, IJobInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeJobInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        protected internal RuntimeJobInfo(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets the job triggers.
        /// </summary>
        /// <value>
        /// The job triggers.
        /// </value>
        public IEnumerable<ITrigger> Triggers { get; }

        /// <summary>
        /// Executes the job asynchronously.
        /// </summary>
        /// <param name="job">The job to execute.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the output.
        /// </returns>
        public async Task<object> ExecuteAsync(
            IJob job,
            IExpando arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}