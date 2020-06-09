// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemorySchedulerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory scheduler test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Tests.InMemory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Runtime;
    using Kephas.Scheduling.InMemory;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Runtime;
    using Kephas.Scheduling.Triggers;
    using Kephas.Testing;
    using Kephas.Workflow;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class InMemorySchedulerTest : TestBase
    {
        private RuntimeTypeRegistry typeRegistry;

        public InMemorySchedulerTest()
        {
            this.typeRegistry = new RuntimeTypeRegistry();
        }

        [Test]
        public async Task InitializeAsync_Enqueue()
        {
            var eventHub = new DefaultEventHub();
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new ActivityContext(Substitute.For<ICompositionContext>(), workflowProcessor));
            var scheduler = new InMemoryScheduler(eventHub, contextFactory, workflowProcessor);

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IExpando>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await eventHub.PublishAsync(
                new EnqueueEvent
                {
                    JobInfo = jobInfo,
                    Options = ctx => ctx.Trigger(new TimerTrigger { Count = 2, Interval = TimeSpan.FromMilliseconds(30), IntervalKind = TimerIntervalKind.StartToStart }),
                },
                null);

            await Task.Delay(150);
            Assert.AreEqual(2, execution);

            await scheduler.FinalizeAsync();
        }

        [Test]
        public async Task InitializeAsync_Enqueue_with_client()
        {
            var eventHub = new DefaultEventHub();
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new ActivityContext(Substitute.For<ICompositionContext>(), workflowProcessor));
            var scheduler = new InMemoryScheduler(eventHub, contextFactory, workflowProcessor);
            var schedulerClient = new InMemorySchedulerClient(eventHub, contextFactory);

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IExpando>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await schedulerClient.EnqueueAsync(
                jobInfo,
                null,
                null,
                ctx => ctx.Trigger(new TimerTrigger { Count = 2, Interval = TimeSpan.FromMilliseconds(30) }));

            await Task.Delay(150);
            Assert.AreEqual(2, execution);

            await scheduler.FinalizeAsync();
        }

        [Test]
        public async Task InitializeAsync_CancelTrigger()
        {
            var eventHub = new DefaultEventHub();
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new ActivityContext(Substitute.For<ICompositionContext>(), workflowProcessor));
            var scheduler = new InMemoryScheduler(eventHub, contextFactory, workflowProcessor);

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            var triggerId = 1;
            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IExpando>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await eventHub.PublishAsync(
                new EnqueueEvent
                {
                    JobInfo = jobInfo,
                    Options = ctx => ctx.Trigger(new TimerTrigger(triggerId) { Count = null, Interval = TimeSpan.FromMilliseconds(30) }),
                },
                null);

            await Task.Delay(150);

            await eventHub.PublishAsync(
                new CancelTriggerEvent
                {
                    TriggerId = triggerId,
                },
                null);

            await Task.Delay(150);

            Assert.LessOrEqual(execution, 7);

            await scheduler.FinalizeAsync();
        }

        [Test]
        public async Task InitializeAsync_CancelTrigger_with_client()
        {
            var eventHub = new DefaultEventHub();
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new ActivityContext(Substitute.For<ICompositionContext>(), workflowProcessor));
            var scheduler = new InMemoryScheduler(eventHub, contextFactory, workflowProcessor);
            var schedulerClient = new InMemorySchedulerClient(eventHub, contextFactory);

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            var triggerId = 1;
            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IExpando>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await schedulerClient.EnqueueAsync(
                jobInfo,
                null,
                null,
                ctx => ctx.Trigger(new TimerTrigger(triggerId) { Count = null, Interval = TimeSpan.FromMilliseconds(30) }));

            await Task.Delay(150);

            await schedulerClient.CancelTriggerAsync(triggerId);

            await Task.Delay(150);

            Assert.LessOrEqual(execution, 7);

            await scheduler.FinalizeAsync();
        }

        [Test]
        public async Task FinalizeAsync_disposes_all_triggers()
        {
            var eventHub = new DefaultEventHub();
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new ActivityContext(Substitute.For<ICompositionContext>(), workflowProcessor));
            var scheduler = new InMemoryScheduler(eventHub, contextFactory, workflowProcessor);

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IExpando>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await eventHub.PublishAsync(
                new EnqueueEvent
                {
                    JobInfo = jobInfo,
                    Options = ctx => ctx.Trigger(new TimerTrigger { Count = null, Interval = TimeSpan.FromMilliseconds(30) }),
                },
                null);

            await Task.Delay(150);

            await scheduler.FinalizeAsync();

            await Task.Delay(150);

            Assert.LessOrEqual(execution, 7);
        }

        [Test]
        public async Task FinalizeAsync_disposes_all_triggers_with_client()
        {
            var eventHub = new DefaultEventHub();
            var workflowProcessor = Substitute.For<IWorkflowProcessor>();
            var contextFactory = this.CreateContextFactoryMock(() => new ActivityContext(Substitute.For<ICompositionContext>(), workflowProcessor));
            var scheduler = new InMemoryScheduler(eventHub, contextFactory, workflowProcessor);
            var schedulerClient = new InMemorySchedulerClient(eventHub, contextFactory);

            await scheduler.InitializeAsync();

            var execution = 0;
            var jobInfo = new RuntimeFuncJobInfo(this.typeRegistry, () => execution++);

            workflowProcessor.ExecuteAsync(Arg.Any<IJob>(), Arg.Any<object>(), Arg.Any<IExpando>(), Arg.Any<Action<IActivityContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => jobInfo.ExecuteAsync(ci.Arg<IJob>(), null, null, contextFactory.CreateContext<ActivityContext>(), ci.Arg<CancellationToken>()));
            await schedulerClient.EnqueueAsync(
                jobInfo,
                null,
                null,
                ctx => ctx.Trigger(new TimerTrigger { Count = null, Interval = TimeSpan.FromMilliseconds(30) }));

            await Task.Delay(150);

            await scheduler.FinalizeAsync();

            await Task.Delay(150);

            Assert.LessOrEqual(execution, 7);
        }
    }
}
