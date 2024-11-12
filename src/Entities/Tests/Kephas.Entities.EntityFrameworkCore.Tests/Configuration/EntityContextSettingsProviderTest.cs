// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Entities.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Kephas.Entities.Tests.Configuration;

public class EntityContextSettingsProviderTest
{
    [Fact]
    public void GetEntityContextSettings_ShouldReturnSettings_WhenEntityContextNameIsValid()
    {
        // Arrange
        var entitiesSettings = new EntitiesSettings
        {
            EntityContexts = new List<EntityContextSettings>
            {
                new() { Name = "TestContext" }
            }
        };
        var options = Substitute.For<IOptions<EntitiesSettings>>();
        options.Value.Returns(entitiesSettings);

        var provider = new EntityContextSettingsProvider(options);

        // Act
        var result = provider.GetEntityContextSettings("TestContext");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("TestContext");
    }

    [Fact]
    public void GetEntityContextSettings_ShouldReturnNewSettings_WhenEntityContextNameIsInvalid()
    {
        // Arrange
        var entitiesSettings = new EntitiesSettings();
        var options = Substitute.For<IOptions<EntitiesSettings>>();
        options.Value.Returns(entitiesSettings);

        var provider = new EntityContextSettingsProvider(options);

        // Act
        var result = provider.GetEntityContextSettings("InvalidContext");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("InvalidContext");
    }

    [Fact]
    public void GetEntityContextSettings_ShouldSetConnection_WhenConnectionIsNull()
    {
        // Arrange
        var entitiesSettings = new EntitiesSettings
        {
            EntityContexts = new List<EntityContextSettings>
            {
                new() { Name = "TestContext" }
            },
            Connections = new List<ConnectionSettings>
            {
                new() { Name = "TestContext" }
            }
        };
        var options = Substitute.For<IOptions<EntitiesSettings>>();
        options.Value.Returns(entitiesSettings);

        var provider = new EntityContextSettingsProvider(options);

        // Act
        var result = provider.GetEntityContextSettings("TestContext");

        // Assert
        result.Connection.Should().NotBeNull();
        result.Connection.Name.Should().Be("TestContext");
    }
}