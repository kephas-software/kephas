// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobDetailId.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job detail identifier class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models.Identifiers
{
    using global::Quartz;

    internal class JobDetailId : BaseKeyId
    {
        public JobDetailId()
        {
        }

        public JobDetailId(JobKey jobKey, string instanceName)
        {
            InstanceName = instanceName;
            this.Name = jobKey.Name;
            this.Group = jobKey.Group;
        }
    }
}