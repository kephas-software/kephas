// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MisfireHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the misfire handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Quartz.Impl.AdoJobStore;

    using Kephas.Logging;
    using Kephas.Threading.Tasks;

    internal class MisfireHandler : global::Kephas.Scheduling.Quartz.Threading.QuartzThread
    {
        private readonly ILogger Log;

        private readonly SchedulingJobStore jobStore;
        private bool shutdown;
        private int numFails;

        public MisfireHandler(SchedulingJobStore jobStore)
        {
            this.jobStore = jobStore;
            this.Name = $"QuartzScheduler_{jobStore.InstanceName}-{jobStore.InstanceId}_MisfireHandler";
            this.IsBackground = true;

            this.Log = jobStore.LogManager?.GetLogger<MisfireHandler>();
        }

        public void Shutdown()
        {
            this.shutdown = true;
            this.Interrupt();
        }

        public override void Run()
        {
            while (!this.shutdown)
            {
                var now = DateTime.UtcNow;
                // TODO refactor to use tasks
                var recoverResult = this.Manage(default).GetResultNonLocking();
                if (recoverResult.ProcessedMisfiredTriggerCount > 0)
                {
                    this.jobStore.SignalSchedulingChangeImmediately(recoverResult.EarliestNewTime);
                }

                if (!this.shutdown)
                {
                    var timeToSleep = TimeSpan.FromMilliseconds(50);
                    if (!recoverResult.HasMoreMisfiredTriggers)
                    {
                        timeToSleep = this.jobStore.MisfireThreshold - (DateTime.UtcNow - now);
                        if (timeToSleep <= TimeSpan.Zero)
                        {
                            timeToSleep = TimeSpan.FromMilliseconds(50);
                        }

                        if (this.numFails > 0)
                        {
                            timeToSleep = this.jobStore.DbRetryInterval > timeToSleep
                                ? this.jobStore.DbRetryInterval
                                : timeToSleep;
                        }
                    }

                    try
                    {
                        Thread.Sleep(timeToSleep);
                    }
                    catch (ThreadInterruptedException)
                    {
                    }
                }
            }
        }

        private async Task<RecoverMisfiredJobsResult> Manage(CancellationToken cancellationToken)
        {
            try
            {
                this.Log.Debug("Scanning for misfires...");
                var result = await this.jobStore.DoRecoverMisfires(cancellationToken).PreserveThreadContext();
                this.numFails = 0;
                return result;
            }
            catch (Exception ex)
            {
                if (this.numFails % this.jobStore.RetryableActionErrorLogThreshold == 0)
                {
                    this.Log.Error(ex, "Error handling misfires.");
                }

                this.numFails++;
            }

            return RecoverMisfiredJobsResult.NoOp;
        }
    }
}