// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessSchedulerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in process scheduler test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Runtime;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.JobStore;
    using Kephas.Scheduling.Runtime;
    using Kephas.Scheduling.Triggers;
    using Kephas.Testing.Composition;
    using Kephas.Workflow;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class InProcessSchedulerTest : CompositionTestBase
    {
        private readonly RuntimeTypeRegistry typeRegistry;

        public InProcessSchedulerTest()
        {
            this.typeRegistry = new RuntimeTypeRegistry();
        }

        [Test]
        public void Composition()
        {
            var container = this.CreateContainer(typeof(IScheduler).Assembly, typeof(IWorkflowProcessor).Assembly);
            var scheduler = container.GetExport<IScheduler>();
            Assert.IsInstanceOf<InProcessScheduler>(scheduler);
        }

        [Test]
        public async Task EnqueueAsync_extension()
        {
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new SchedulingContext(Substitute.For<IInjector>()));
            var scheduler = new InProcessScheduler(contextFactory, workflowProcessor, new InMemoryJobStore(), new StaticAppRuntime());

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IDynamic>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await scheduler.EnqueueAsync(
                jobInfo,
                trigger: new TimerTrigger
                {
                    Count = 2,
                    Interval = TimeSpan.FromMilliseconds(30),
                    IntervalKind = TimerIntervalKind.StartToStart,
                });

            await Task.Delay(150);

            try
            {
                Assert.AreEqual(2, execution);
            }
            finally
            {
                await scheduler.FinalizeAsync();
            }
        }

        [Test]
        public async Task EnqueueAsync()
        {
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new SchedulingContext(Substitute.For<IInjector>()));
            var appRuntime = new StaticAppRuntime(appId: "test", appInstanceId: "test-1");
            var scheduler = new InProcessScheduler(contextFactory, workflowProcessor, new InMemoryJobStore(), appRuntime);

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IDynamic>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await scheduler.EnqueueAsync(
                jobInfo,
                ctx => ctx
                    .Trigger(new TimerTrigger
                    {
                        Count = 2,
                        Interval = TimeSpan.FromMilliseconds(30),
                        IntervalKind = TimerIntervalKind.StartToStart,
                    }));

            await Task.Delay(150);
            try
            {
                Assert.AreEqual(2, execution);
            }
            finally
            {
                await scheduler.FinalizeAsync();
            }
        }

        [Test]
        public async Task CancelTriggerAsync_triggerId()
        {
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new SchedulingContext(Substitute.For<IInjector>()));
            var scheduler = new InProcessScheduler(contextFactory, workflowProcessor, new InMemoryJobStore(), new StaticAppRuntime());

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            var triggerId = 1;
            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IDynamic>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await scheduler.EnqueueAsync(
                jobInfo,
                ctx => ctx.Trigger(new TimerTrigger(triggerId)
                {
                    Count = null,
                    Interval = TimeSpan.FromMilliseconds(30),
                }),
                default);

            await Task.Delay(150);

            await scheduler.CancelTriggerAsync(triggerId);

            await Task.Delay(150);

            try
            {
                Assert.LessOrEqual(execution, 7);
                CollectionAssert.IsEmpty(jobInfo.Triggers);
            }
            finally
            {
                await scheduler.FinalizeAsync();
            }
        }

        [Test]
        public async Task CancelTriggerAsync_trigger()
        {
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new SchedulingContext(Substitute.For<IInjector>()));
            var scheduler = new InProcessScheduler(contextFactory, workflowProcessor, new InMemoryJobStore(), new StaticAppRuntime());

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            var triggerId = 1;
            var trigger = new TimerTrigger(triggerId) {Count = null, Interval = TimeSpan.FromMilliseconds(30)};
            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IDynamic>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await scheduler.EnqueueAsync(
                jobInfo,
                null,
                null,
                null,
                trigger);

            await Task.Delay(150);

            await scheduler.CancelTriggerAsync(trigger);

            await Task.Delay(150);

            try
            {
                Assert.LessOrEqual(execution, 7);
                CollectionAssert.IsEmpty(jobInfo.Triggers);
            }
            finally
            {
                await scheduler.FinalizeAsync();
            }
        }

        [Test]
        public async Task CancelScheduledJobAsync_jobInfo()
        {
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new SchedulingContext(Substitute.For<IInjector>()));
            var scheduler = new InProcessScheduler(contextFactory, workflowProcessor, new InMemoryJobStore(), new StaticAppRuntime());

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            var triggerId = 1;
            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IDynamic>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await scheduler.EnqueueAsync(
                jobInfo,
                null,
                null,
                null,
                new TimerTrigger(triggerId) { Count = null, Interval = TimeSpan.FromMilliseconds(30) });

            await Task.Delay(150);

            await scheduler.CancelScheduledJobAsync(jobInfo);

            await Task.Delay(150);

            try
            {
                Assert.LessOrEqual(execution, 7);
                CollectionAssert.IsEmpty(jobInfo.Triggers);
                CollectionAssert.DoesNotContain(scheduler.GetScheduledJobs(), jobInfo);
            }
            finally
            {
                await scheduler.FinalizeAsync();
            }
        }

        [Test]
        public async Task CancelScheduledJobAsync_jobInfoId()
        {
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new SchedulingContext(Substitute.For<IInjector>()));
            var scheduler = new InProcessScheduler(contextFactory, workflowProcessor, new InMemoryJobStore(), new StaticAppRuntime());

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            var triggerId = 1;
            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IDynamic>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await scheduler.EnqueueAsync(
                jobInfo,
                null,
                null,
                null,
                new TimerTrigger(triggerId) { Count = null, Interval = TimeSpan.FromMilliseconds(30) });

            await Task.Delay(150);

            await scheduler.CancelScheduledJobAsync(jobInfo.Id);

            await Task.Delay(150);

            try
            {
                Assert.LessOrEqual(execution, 7);
                CollectionAssert.IsEmpty(jobInfo.Triggers);
                CollectionAssert.DoesNotContain(scheduler.GetScheduledJobs(), jobInfo);
            }
            finally
            {
                await scheduler.FinalizeAsync();
            }
        }

        [Test]
        public async Task FinalizeAsync_disposes_all_triggers_and_scheduled_jobs()
        {
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new SchedulingContext(Substitute.For<IInjector>()));
            var scheduler = new InProcessScheduler(contextFactory, workflowProcessor, new InMemoryJobStore(), new StaticAppRuntime());

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IDynamic>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await scheduler.EnqueueAsync(
                jobInfo,
                ctx => ctx.Trigger(new TimerTrigger
                {
                    Count = null,
                    Interval = TimeSpan.FromMilliseconds(30),
                }));

            await Task.Delay(150);

            await scheduler.FinalizeAsync();

            await Task.Delay(150);

            Assert.LessOrEqual(execution, 7);

            var scheduledJobs = scheduler.GetScheduledJobs();
            CollectionAssert.IsEmpty(scheduledJobs);
        }

        [Test]
        public async Task FinalizeAsync_disposes_all_triggers()
        {
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new SchedulingContext(Substitute.For<IInjector>()));
            var scheduler = new InProcessScheduler(contextFactory, workflowProcessor, new InMemoryJobStore(), new StaticAppRuntime());

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IDynamic>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await scheduler.EnqueueAsync(
                jobInfo,
                null,
                null,
                null,
                new TimerTrigger { Count = null, Interval = TimeSpan.FromMilliseconds(30) });

            await Task.Delay(150);

            await scheduler.FinalizeAsync();

            await Task.Delay(150);

            Assert.LessOrEqual(execution, 7);
        }
    }
}
