// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppModulesHealthCheckTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using FluentAssertions;
using Kephas.Extensions.Diagnostics.HealthChecks.Configuration;
using Kephas.Extensions.Diagnostics.HealthChecks.Providers;
using Kephas.Runtime;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.Diagnostics.HealthChecks.Providers;

public class AppModulesHealthCheckTest
{
    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthyStatusWhenNoModules()
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        var options = Substitute.For<IOptions<HealthChecksSettings>>();
        options.Value.Returns(new HealthChecksSettings { ModulePatterns = new Dictionary<string, string>() });

        var healthCheck = new AppModulesHealthCheck(appRuntime, options);
        var context = new HealthCheckContext();
        var result = await healthCheck.CheckHealthAsync(context);

        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("No modules are available.");
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsExpectedResultsForModules()
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        var options = Substitute.For<IOptions<HealthChecksSettings>>();
        options.Value.Returns(new HealthChecksSettings
        {
            ModulePatterns = new Dictionary<string, string>
            {
                { "Module1", "Module1.*" },
                { "Module2", "Module2.*" }
            }
        });

        var assemblies = new[]
        {
            Assembly.GetExecutingAssembly(),
            typeof(AppModulesHealthCheck).Assembly
        };
        appRuntime.GetAppAssemblies().Returns(assemblies);

        var healthCheck = new AppModulesHealthCheck(appRuntime, options);
        var context = new HealthCheckContext();
        var result = await healthCheck.CheckHealthAsync(context);

        result.Status.Should().Be(HealthStatus.Healthy);
        result.Data.Should().ContainKey("Module1");
        result.Data.Should().ContainKey("Module2");
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsUnhealthyStatusWhenExceptionThrown()
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        var options = Substitute.For<IOptions<HealthChecksSettings>>();
        options.Value.Returns(new HealthChecksSettings
        {
            ModulePatterns = new Dictionary<string, string>
            {
                { "Module1", "Module1.*" }
            }
        });

        var healthCheck = Substitute.ForPartsOf<AppModulesHealthCheck>(appRuntime, options);
        healthCheck.When(x => x.CheckAppModulesHealth(Arg.Any<string>())).DoNotCallBase();
        healthCheck.CheckAppModulesHealth(Arg.Any<string>()).Returns(x => throw new Exception("Test exception"));

        var context = new HealthCheckContext();
        var result = await healthCheck.CheckHealthAsync(context);

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().StartWith("At least one application module has errors.");
        result.Exception!.Message.Should().Be("At least one application module has errors. (Test exception)");
    }
}