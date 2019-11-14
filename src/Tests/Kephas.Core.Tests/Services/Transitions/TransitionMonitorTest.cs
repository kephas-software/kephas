// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransitionMonitorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="TransitionMonitor" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services.Transitions
{
    using System;

    using Kephas.Services.Transitions;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="TransitionMonitor"/>
    /// </summary>
    [TestFixture]
    public class TransitionMonitorTest
    {
        [Test]
        public void InitialState()
        {
            var monitor = new TransitionMonitor("init", "svc");
            Assert.IsTrue(monitor.IsNotStarted);
            Assert.IsFalse(monitor.IsInProgress);
            Assert.IsFalse(monitor.IsCompleted);
            Assert.IsFalse(monitor.IsCompletedSuccessfully);
            Assert.IsFalse(monitor.IsFaulted);
        }

        [Test]
        public void Start_successful()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            Assert.IsFalse(monitor.IsNotStarted);
            Assert.IsTrue(monitor.IsInProgress);
            Assert.IsFalse(monitor.IsCompleted);
            Assert.IsFalse(monitor.IsCompletedSuccessfully);
            Assert.IsFalse(monitor.IsFaulted);
        }

        [Test]
        public void Start_twice_failure()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            Assert.Throws<ServiceTransitionException>(() => monitor.Start());
        }

        [Test]
        public void Start_after_complete_failure()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Complete();
            Assert.Throws<ServiceTransitionException>(() => monitor.Start());
        }

        [Test]
        public void Start_after_fault_failure()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Fault(new Exception());
            Assert.Throws<ServiceTransitionException>(() => monitor.Start());
        }

        [Test]
        public void Complete_successful()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Complete();
            Assert.IsFalse(monitor.IsNotStarted);
            Assert.IsFalse(monitor.IsInProgress);
            Assert.IsTrue(monitor.IsCompleted);
            Assert.IsTrue(monitor.IsCompletedSuccessfully);
            Assert.IsFalse(monitor.IsFaulted);
        }

        [Test]
        public void Complete_not_started_failure()
        {
            var monitor = new TransitionMonitor("init", "svc");
            Assert.Throws<ServiceTransitionException>(() => monitor.Complete());
        }

        [Test]
        public void Complete_twice_success()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Complete();
            monitor.Complete();
            Assert.IsFalse(monitor.IsNotStarted);
            Assert.IsFalse(monitor.IsInProgress);
            Assert.IsTrue(monitor.IsCompleted);
            Assert.IsTrue(monitor.IsCompletedSuccessfully);
            Assert.IsFalse(monitor.IsFaulted);
        }

        [Test]
        public void Complete_on_faulted_failure()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Fault(new Exception());
            Assert.Throws<ServiceTransitionException>(() => monitor.Complete());
        }

        [Test]
        public void Fault_successful()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Fault(new Exception());
            Assert.IsFalse(monitor.IsNotStarted);
            Assert.IsFalse(monitor.IsInProgress);
            Assert.IsTrue(monitor.IsCompleted);
            Assert.IsFalse(monitor.IsCompletedSuccessfully);
            Assert.IsTrue(monitor.IsFaulted);
        }

        [Test]
        public void Fault_not_started_failure()
        {
            var monitor = new TransitionMonitor("init", "svc");
            Assert.Throws<ServiceTransitionException>(() => monitor.Fault(new Exception()));
        }

        [Test]
        public void Fault_must_provide_exception()
        {
            var monitor = new TransitionMonitor("init", "svc");
            Assert.That(() => monitor.Fault(null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void Fault_twice_success()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Fault(new Exception());
            monitor.Fault(new Exception());
            Assert.IsFalse(monitor.IsNotStarted);
            Assert.IsFalse(monitor.IsInProgress);
            Assert.IsTrue(monitor.IsCompleted);
            Assert.IsFalse(monitor.IsCompletedSuccessfully);
            Assert.IsTrue(monitor.IsFaulted);
        }

        [Test]
        public void Reset_from_initial_success()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Reset();
            Assert.IsTrue(monitor.IsNotStarted);
            Assert.IsFalse(monitor.IsInProgress);
            Assert.IsFalse(monitor.IsCompleted);
            Assert.IsFalse(monitor.IsCompletedSuccessfully);
            Assert.IsFalse(monitor.IsFaulted);
        }

        [Test]
        public void Reset_from_in_progress_success()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Reset();
            Assert.IsTrue(monitor.IsNotStarted);
            Assert.IsFalse(monitor.IsInProgress);
            Assert.IsFalse(monitor.IsCompleted);
            Assert.IsFalse(monitor.IsCompletedSuccessfully);
            Assert.IsFalse(monitor.IsFaulted);
        }

        [Test]
        public void Reset_from_completed_success()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Complete();
            monitor.Reset();
            Assert.IsTrue(monitor.IsNotStarted);
            Assert.IsFalse(monitor.IsInProgress);
            Assert.IsFalse(monitor.IsCompleted);
            Assert.IsFalse(monitor.IsCompletedSuccessfully);
            Assert.IsFalse(monitor.IsFaulted);
        }

        [Test]
        public void Reset_from_faulted_success()
        {
            var monitor = new TransitionMonitor("init", "svc");
            monitor.Start();
            monitor.Fault(new Exception());
            monitor.Reset();
            Assert.IsTrue(monitor.IsNotStarted);
            Assert.IsFalse(monitor.IsInProgress);
            Assert.IsFalse(monitor.IsCompleted);
            Assert.IsFalse(monitor.IsCompletedSuccessfully);
            Assert.IsFalse(monitor.IsFaulted);
        }
    }
}