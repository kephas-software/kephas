// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostBuilderLoggingExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Logging;
using Kephas.Threading;
using Kephas.Web.Hosting;
using Kephas.Web.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog.Core;
using Xunit;

namespace Kephas.Web.Tests.Hosting;

public class WebHostBuilderLoggingExtensionsTest
{
    [Fact]
    public void UseWebLogging_no_further_configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var hostBuilder = GetHostBuilder(services);

        // Act
        hostBuilder.UseWebLogging();
        
        // Assert
        services.Should().ContainSingle(s =>
            s.ServiceType == typeof(CorrelationIdEnricher) && s.ImplementationType == typeof(CorrelationIdEnricher) && s.Lifetime == ServiceLifetime.Transient);
        services.Should().ContainSingle(s =>
            s.ServiceType == typeof(HttpRequestEnricher) && s.ImplementationType == typeof(HttpRequestEnricher) && s.Lifetime == ServiceLifetime.Transient);
        services.Should().ContainSingle(s =>
            s.ServiceType == typeof(ILogLevelSwitch) && s.ImplementationType == typeof(SerilogLevelSwitch) && s.Lifetime == ServiceLifetime.Singleton);

        var sp = BuildServiceProvider(services);
        sp.GetService<ILogger<string>>();
    }
    
    [Fact]
    public void UseWebLogging_additional_configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var hostBuilder = GetHostBuilder(services);

        // Act
        var called = false;
        hostBuilder.UseWebLogging(_ => called = true);
        
        // Assert
        services.Should().ContainSingle(s =>
            s.ServiceType == typeof(CorrelationIdEnricher) && s.ImplementationType == typeof(CorrelationIdEnricher) && s.Lifetime == ServiceLifetime.Transient);
        services.Should().ContainSingle(s =>
            s.ServiceType == typeof(HttpRequestEnricher) && s.ImplementationType == typeof(HttpRequestEnricher) && s.Lifetime == ServiceLifetime.Transient);
        services.Should().ContainSingle(s =>
            s.ServiceType == typeof(ILogLevelSwitch) && s.ImplementationType == typeof(SerilogLevelSwitch) && s.Lifetime == ServiceLifetime.Singleton);

        var sp = BuildServiceProvider(services);
        sp.GetService<ILogger<string>>();
        called.Should().BeTrue();
    }

    private static IHostApplicationBuilder GetHostBuilder(IServiceCollection services)
    {
        var hostBuilder = Substitute.For<IHostApplicationBuilder>();
        hostBuilder.Services.Returns(services);
        return hostBuilder;
    }

    private static IServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        var cb = new ConfigurationBuilder();
        cb.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Serilog:MinimumLevel", "Information" }
        });

        services.AddSingleton(Substitute.For<IAsyncLocalContext>());
        services.AddSingleton(Substitute.For<IHttpContextAccessor>());
        services.AddSingleton<IConfiguration>(cb.Build());
        services.AddLogging();
        var sp = services.BuildServiceProvider();
        return sp;
    }
}