// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthChecksIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.Extensions.Diagnostics.HealthChecks;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Kephas.Extensions.Tests.Diagnostics.HealthChecks;

public class HealthChecksIntegrationTest : IntegrationTestBase<HealthChecksIntegrationTest>
{
    [Fact]
    public void COllectHealthChecks()
    {
        var healthChecks = ServiceProvider.GetRequiredService<ILazyEnumerable<IHealthCheck, HealthCheckMetadata>>();
        healthChecks.Select(hc => hc.Metadata).Should().Satisfy(
            e => e.ServiceKey == "AppModules" && e.FailureStatus == HealthStatus.Degraded,
            e => e.ServiceKey == "Memory" && e.FailureStatus == HealthStatus.Unhealthy,
            e => e.ServiceKey == "Process" && e.FailureStatus == HealthStatus.Unhealthy
        );

    }
}