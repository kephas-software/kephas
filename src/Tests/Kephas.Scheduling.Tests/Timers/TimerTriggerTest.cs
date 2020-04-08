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
    using System.Threading.Tasks;

    using Kephas.Scheduling.Triggers;
    using NUnit.Framework;

    [TestFixture]
    public class TimerTriggerTest
    {
        [Test]
        public async Task Fire()
        {
            var trigger = new TimerTrigger
                {
                    Interval = TimeSpan.FromMilliseconds(100),
                    Count = 2,
                };

            var fired = 0;
            var disposed = 0;
            trigger.Fire += (s, e) => fired++;
            trigger.Disposed += (s, e) => disposed++;

            trigger.Initialize();

            await Task.Delay(400);

            Assert.AreEqual(2, fired);
            Assert.AreEqual(1, disposed);
        }
    }
}
