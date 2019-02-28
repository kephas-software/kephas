// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiredTriggerId.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the fired trigger identifier class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models.Identifiers
{
    internal class FiredTriggerId : BaseId
    {
        public FiredTriggerId()
        {
        }

        public FiredTriggerId(string firedInstanceId, string instanceName)
        {
            InstanceName = instanceName;
            this.FiredInstanceId = firedInstanceId;
        }

        public string FiredInstanceId { get; set; }
    }
}