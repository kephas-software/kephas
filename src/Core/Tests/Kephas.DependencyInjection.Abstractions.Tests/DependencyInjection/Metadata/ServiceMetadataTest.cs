// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadataTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AppServiceMetadata" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection.Metadata;
using Xunit;

namespace Kephas.Tests.DependencyInjection.Metadata;

/// <summary>
/// Test class for <see cref="ServiceMetadata"/>.
/// </summary>
public class ServiceMetadataTest
{
    [Fact]
    public void Constructor_OverrideBaseImplementation()
    {
        var metadata = new Dictionary<string, object?> { { "OverrideBaseImplementation", true } };
        var appServiceMetadata = new ServiceMetadata(metadata);
        appServiceMetadata.OverrideBaseImplementation.Should().BeTrue();
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Highest)]
    public void Constructor_Priority(Priority priority)
    {
        var metadata = new Dictionary<string, object?> { { "Priority", priority } };
        var appServiceMetadata = new ServiceMetadata(metadata);
        appServiceMetadata.Priority.Should().Be(priority);
    }

    [Theory]
    [InlineData("my key")]
    public void Constructor_ServiceKey(string serviceKey)
    {
        var metadata = new Dictionary<string, object?> { { "ServiceKey", serviceKey } };
        var appServiceMetadata = new ServiceMetadata(metadata);
        appServiceMetadata.ServiceKey.Should().Be(serviceKey);
    }

    [Theory]
    [InlineData(typeof(string))]
    public void Constructor_ImplementationType(Type implementationType)
    {
        var metadata = new Dictionary<string, object?> { { "ImplementationType", implementationType } };
        var appServiceMetadata = new ServiceMetadata(metadata);
        appServiceMetadata.ImplementationType.Should().Be(implementationType);
    }
}