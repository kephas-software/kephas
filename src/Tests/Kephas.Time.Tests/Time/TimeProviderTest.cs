// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Time;
using Xunit;

namespace Kephas.Tests.Time
{
    public class TimeProviderTest
    {
        [Fact]
        public void GetUtcNow()
        {
            var timeProvider = new Kephas.Time.TimeProvider();

            var prevTime = DateTime.UtcNow;
            var time = timeProvider.GetUtcNow();
            var nextTime = DateTime.UtcNow;

            time.Should().NotBeBefore(prevTime);
            time.Should().NotBeAfter(nextTime);
        }

        [Fact]
        public void GetLocalNow()
        {
            var timeProvider = new Kephas.Time.TimeProvider();

            var prevTime = DateTime.Now;
            var time = timeProvider.GetLocalNow();
            var nextTime = DateTime.Now;

            time.Should().NotBeBefore(prevTime);
            time.Should().NotBeAfter(nextTime);
        }

        [Fact]
        public void GetUtcNowWithOffset()
        {
            var timeProvider = new Kephas.Time.TimeProvider();

            var prevTime = DateTimeOffset.UtcNow;;
            var time = timeProvider.GetUtcNowWithOffset();
            var nextTime = DateTimeOffset.UtcNow;

            time.Should().NotBeBefore(prevTime);
            time.Should().NotBeAfter(nextTime);
        }

        [Fact]
        public void GetLocalNowWithOffset()
        {
            var timeProvider = new Kephas.Time.TimeProvider();

            var prevTime = DateTimeOffset.Now;
            var time = timeProvider.GetLocalNowWithOffset();
            var nextTime = DateTimeOffset.Now;

            time.Should().NotBeBefore(prevTime);
            time.Should().NotBeAfter(nextTime);
        }
    }
}
