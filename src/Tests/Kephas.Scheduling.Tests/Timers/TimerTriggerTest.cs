// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimerTriggerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the timer trigger test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Tests.Timers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Scheduling.Triggers;
    using NUnit.Framework;

    [TestFixture]
    public class TimerTriggerTest
    {
        [Test]
        public async Task Fire_respects_Count_StartToStart()
        {
            var trigger = new TimerTrigger
            {
                Interval = TimeSpan.FromMilliseconds(30),
                Count = 2,
                IntervalKind = TimerIntervalKind.StartToStart,
            };

            var fired = 0;
            var disposed = 0;
            var fires = new List<long>();
            trigger.Fire += (s, e) =>
            {
                fired++;
                fires.Add(DateTimeOffset.Now.Ticks);
                Thread.Sleep(30);
            };
            trigger.Disposed += (s, e) => disposed++;

            trigger.Initialize();

            await Task.Delay(200);

            Assert.AreEqual(2, fired);
            Assert.AreEqual(1, disposed);
            this.AssertInterval(fires, 30);
        }

        [Test]
        public async Task Fire_respects_Count_EndToStart()
        {
            var trigger = new TimerTrigger
            {
                Interval = TimeSpan.FromMilliseconds(30),
                Count = 2,
                IntervalKind = TimerIntervalKind.EndToStart,
            };

            var fired = 0;
            var disposed = 0;
            var fires = new List<long>();
            trigger.Fire += (s, e) =>
            {
                fired++;
                fires.Add(DateTimeOffset.Now.Ticks);
                var opResult = new OperationResult(Task.Delay(30));
                if (e.CompleteCallback != null)
                {
                    opResult.AsTask().ContinueWith(t => e.CompleteCallback.Invoke(opResult));
                }
            };
            trigger.Disposed += (s, e) => disposed++;

            trigger.Initialize();

            await Task.Delay(200);

            Assert.AreEqual(2, fired);
            Assert.AreEqual(1, disposed);
            this.AssertInterval(fires, 60);
        }

        [Test]
        public void ToString_Infinite_3_seconds_interval()
        {
            var trigger = new TimerTrigger(144)
            {
                Interval = TimeSpan.FromSeconds(3),
                Count = null,
                IntervalKind = TimerIntervalKind.EndToStart,
            };

            var triggerString = trigger.ToString();
            Assert.AreEqual("TimerTrigger:144 -00:00:03", triggerString);
        }

        private void AssertInterval(IList<long> fires, long milliseconds)
        {
            var delta = 10L; // milliseconds
            var prevtick = fires[0];
            for (int i = 1; i < fires.Count; i++)
            {
                var diffMillis = (fires[i] - prevtick) / TimeSpan.TicksPerMillisecond;
                var actualDelta = diffMillis - milliseconds;
                if (actualDelta > delta || actualDelta < -delta)
                {
                    Assert.Warn($"Expected an interval of {milliseconds} but got {diffMillis}.");
                }

                prevtick = fires[i];
            }
        }
    }
}
