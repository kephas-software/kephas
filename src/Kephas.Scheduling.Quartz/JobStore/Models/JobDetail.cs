// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobDetail.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job detail class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models
{
    using System;

    using global::Quartz;
    using global::Quartz.Impl;

    using Kephas.Data;
    using Kephas.Scheduling.Quartz.JobStore.Model;
    using Kephas.Scheduling.Quartz.JobStore.Models.Identifiers;

    public class JobDetail : GroupedQuartzEntityBase, Model.IJobDetail
    {
        public JobDetail()
        {
        }

        public JobDetail(global::Quartz.IJobDetail jobDetail, string instanceName)
        {
            this.InstanceName = instanceName;
            this.Name = jobDetail.Key.Name;
            this.Group = jobDetail.Key.Group;
            this.Description = jobDetail.Description;
            this.JobType = jobDetail.JobType;
            this.JobDataMap = jobDetail.JobDataMap;
            this.Durable = jobDetail.Durable;
            this.PersistJobDataAfterExecution = jobDetail.PersistJobDataAfterExecution;
            this.ConcurrentExecutionDisallowed = jobDetail.ConcurrentExecutionDisallowed;
            this.RequestsRecovery = jobDetail.RequestsRecovery;
        }

        //TODO [BsonId]
        public JobDetailId Id { get; set; }

        public string Description { get; set; }

        public Type JobType { get; set; }

        public JobDataMap JobDataMap { get; set; }

        public bool Durable { get; set; }

        public bool PersistJobDataAfterExecution { get; set; }

        public bool ConcurrentExecutionDisallowed { get; set; }

        public bool RequestsRecovery { get; set; }
    }
}