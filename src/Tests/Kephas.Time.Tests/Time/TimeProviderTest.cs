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
        public void GetUtcDateTime()
        {
            var timeProvider = new TimeProvider();

            var prevTime = DateTime.UtcNow;
            var time = timeProvider.GetUtcDateTime();
            var nextTime = DateTime.UtcNow;

            time.Should().NotBeBefore(prevTime);
            time.Should().NotBeAfter(nextTime);
        }

        [Fact]
        public void GetLocalDateTime()
        {
            var timeProvider = new TimeProvider();

            var prevTime = DateTime.Now;
            var time = timeProvider.GetLocalDateTime();
            var nextTime = DateTime.Now;

            time.Should().NotBeBefore(prevTime);
            time.Should().NotBeAfter(nextTime);
        }

        [Fact]
        public void GetUtcDateTimeOffset()
        {
            var timeProvider = new TimeProvider();

            var prevTime = DateTimeOffset.UtcNow;;
            var time = timeProvider.GetUtcDateTimeOffset();
            var nextTime = DateTimeOffset.UtcNow;

            time.Should().NotBeBefore(prevTime);
            time.Should().NotBeAfter(nextTime);
        }

        [Fact]
        public void GetLocalDateTimeOffset()
        {
            var timeProvider = new TimeProvider();

            var prevTime = DateTimeOffset.Now;
            var time = timeProvider.GetLocalDateTimeOffset();
            var nextTime = DateTimeOffset.Now;

            time.Should().NotBeBefore(prevTime);
            time.Should().NotBeAfter(nextTime);
        }
    }
}
