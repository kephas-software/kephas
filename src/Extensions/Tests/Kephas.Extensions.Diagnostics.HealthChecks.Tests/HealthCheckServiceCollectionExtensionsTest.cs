// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthChecksServiceCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Diagnostics.HealthChecks.Configuration;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Xunit;

namespace Kephas.Extensions.Tests;

public class HealthChecksServiceCollectionExtensionsTest : IntegrationTestBase<HealthChecksServiceCollectionExtensionsTest>
{
    [Fact]
    public void MemoryCheckSettings_ReadFromConfiguration()
    {
        var options = ServiceProvider.GetRequiredService<IOptions<MemoryCheckSettings>>();
        options.Value.ThresholdExceededStatus.Should().Be(HealthStatus.Unhealthy);
        options.Value.Threshold.Should().Be(2048);
    }
}