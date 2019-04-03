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
    using Kephas.Scheduling.Reflection;
    using Kephas.Workflow;

    /// <summary>
    /// Base implementation of a <see cref="IJob"/>.
    /// </summary>
    public class JobBase : ActivityBase, IJob
    {
        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public new IJobInfo GetTypeInfo() => (IJobInfo)base.GetTypeInfo();

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}