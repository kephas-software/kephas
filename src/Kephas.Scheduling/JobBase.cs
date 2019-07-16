// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using System;

    using Kephas.Scheduling.Reflection;
    using Kephas.Workflow;

    /// <summary>
    /// Base implementation of a <see cref="IJob"/>.
    /// </summary>
    public abstract class JobBase : ActivityBase, IJob
    {
        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public new IJobInfo GetTypeInfo() => (IJobInfo)base.GetTypeInfo();

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Scheduling.JobBase and optionally
        /// releases the managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Scheduling.JobBase and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; false to
        /// release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}