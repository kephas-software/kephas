// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using FluentAssertions;
using Xunit;

namespace Kephas.Tests.Time;

public class TimeProviderTest
{
    [Fact]
    public void GetUtcNow()
    {
        var timeProvider = new Kephas.Time.TimeProvider();

        var prevTime = DateTimeOffset.UtcNow;;
        var time = timeProvider.GetUtcNow();
        var nextTime = DateTimeOffset.UtcNow;

        time.Should().NotBeBefore(prevTime);
        time.Should().NotBeAfter(nextTime);
    }

    [Fact]
    public void GetLocalNow()
    {
        var timeProvider = new Kephas.Time.TimeProvider();

        var prevTime = DateTimeOffset.Now;
        var time = timeProvider.GetLocalNow();
        var nextTime = DateTimeOffset.Now;

        time.Should().NotBeBefore(prevTime);
        time.Should().NotBeAfter(nextTime);
    }

    [Fact]
    public void GetTimestamp()
    {
        var timeProvider = new Kephas.Time.TimeProvider();

        var prevTime = Stopwatch.GetTimestamp();
        var time = timeProvider.GetTimestamp();
        var nextTime = Stopwatch.GetTimestamp();

        time.Should().BeGreaterOrEqualTo(prevTime);
        time.Should().BeLessThanOrEqualTo(nextTime);
    }
}
