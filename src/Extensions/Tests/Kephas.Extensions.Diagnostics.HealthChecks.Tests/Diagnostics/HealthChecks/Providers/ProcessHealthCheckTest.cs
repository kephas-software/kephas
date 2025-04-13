// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessHealthCheckTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;
using FluentAssertions;
using Kephas.Extensions.Diagnostics.HealthChecks.Providers;
using Kephas.Runtime;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.Diagnostics.HealthChecks.Providers;

public class ProcessHealthCheckTest
{
    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthyStatus()
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        appRuntime.GetAppName().Returns("TestApp");
        appRuntime.GetAppVersion().Returns("1.0.0");
        appRuntime.GetAppInstanceId().Returns("Instance1");
        appRuntime.GetEnvironment().Returns("Development");
        appRuntime.GetMachineName().Returns("TestMachine");

        var processInfo = new ProcessHealthCheck.ProcessInfo(
            "TestProcess",
            1234,
            10,
            5,
            1024 * 1024 * 50, // 50 MB
            1024 * 1024 * 100, // 100 MB
            new TimeSpan(0, 1, 0) // 1 minute
        );
        var healthCheck = Substitute.ForPartsOf<ProcessHealthCheck>(appRuntime);
        healthCheck.When(x => x.GetCurrentProcessInfo()).DoNotCallBase();
        healthCheck.GetCurrentProcessInfo().Returns(processInfo);

        var context = new HealthCheckContext();
        var result = await healthCheck.CheckHealthAsync(context);

        result.Status.Should().Be(HealthStatus.Healthy);
        result.Data["AppName"].Should().Be("TestApp");
        result.Data["AppVersion"].Should().Be("1.0.0");
        result.Data["AppInstanceId"].Should().Be("Instance1");
        result.Data["Environment"].Should().Be("Development");
        result.Data["MachineName"].Should().Be("TestMachine");
        result.Data["ProcessName"].Should().Be("TestProcess");
        result.Data["ProcessId"].Should().Be(1234);
        result.Data["HandleCount"].Should().Be(10);
        result.Data["ThreadCount"].Should().Be(5);
        result.Data["PrivateBytesMB"].Should().Be(50);
        result.Data["PeakWorkingSetMB"].Should().Be(100);
        result.Data["TotalProcessorTime"].Should().Be("0:01:00");
        result.Data["OSDescription"].Should().Be(RuntimeInformation.OSDescription);
        result.Data["OSArchitecture"].Should().Be(RuntimeInformation.OSArchitecture);
        result.Data["ProcessArchitecture"].Should().Be(RuntimeInformation.ProcessArchitecture);
        result.Data["FrameworkDescription"].Should().Be(RuntimeInformation.FrameworkDescription);
        result.Data["RuntimeIdentifier"].Should().Be(RuntimeInformation.RuntimeIdentifier);
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsUnhealthyStatusWhenExceptionThrown()
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        var healthCheck = Substitute.ForPartsOf<ProcessHealthCheck>(appRuntime);
        healthCheck.When(x => x.GetCurrentProcessInfo()).DoNotCallBase();
        healthCheck.GetCurrentProcessInfo().Returns(x => throw new Exception("Test exception"));

        var context = new HealthCheckContext();
        var result = await healthCheck.CheckHealthAsync(context);

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be("Process information could not be retrieved.");
        result.Exception!.Message.Should().Be("Test exception");
    }
}