// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostBuilderLoggingExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using FluentAssertions;
using Kephas.Extensions.Hosting;
using Kephas.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog.Events;
using Xunit;

namespace Kephas.Extensions.Tests.Hosting;

public class HostBuilderLoggingExtensionsTest
{
    [Fact]
    public void UseLogging_no_further_configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var hostBuilder = GetHostBuilder(services);

        // Act
        hostBuilder.UseLogging();

        // Assert
        services.Should().Contain(d => d.ImplementationType == typeof(SerilogLevelSwitch) && d.Lifetime == ServiceLifetime.Singleton);

        var sp = BuildServiceProvider(services);
        sp.GetService<ILogger<string>>();
    }
    
    [Fact]
    public void UseLogging_additional_configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var hostBuilder = GetHostBuilder(services);

        // Act
        var called = false;
        hostBuilder.UseLogging(_ => called = true);
        
        // Assert
        services.Should().Contain(d => d.ImplementationType == typeof(SerilogLevelSwitch) && d.Lifetime == ServiceLifetime.Singleton);

        var sp = BuildServiceProvider(services);
        sp.GetService<ILogger<string>>();
        called.Should().BeTrue();
    }
    
    /// <summary>
    /// <see cref="HostBuilderLoggingExtensions.UseLogging{TBuilder}(TBuilder)"/>  is being checked.
    /// </summary>
    [Fact]
    public void UseLogging_when_custom_exception_contains_circular_reference_then_custom_properties_are_not_logged()
    {
        // Arrange
        var outputBuilder = new StringBuilder();
        var services = UseLogging_SetupServicesForSerilog(outputBuilder);
        var sp = BuildServiceProvider(services);
        var logger = sp.GetRequiredService<ILogger<string>>();

        // Act
        logger.LogError(new TestCircularReferenceException(), "test");

        // Assert
        var output = outputBuilder.ToString(); 
        output.Should().StartWith("Error test SourceContext,ThreadId,ProcessId,MachineName,ExceptionDetail");
        output.Should().EndWith("[(\"Type\": \"Kephas.Extensions.Tests.Hosting.HostBuilderLoggingExtensionsTest+TestCircularReferenceException\"), (\"HResult\": -2146233088), (\"Message\": \"Exception of type 'Kephas.Extensions.Tests.Hosting.HostBuilderLoggingExtensionsTest+TestCircularReferenceException' was thrown.\"), (\"Source\": null)]");
    }
    
    
    /// <summary>
    /// <see cref="HostBuilderLoggingExtensions.UseLogging{TBuilder}(TBuilder)"/>  is being checked.
    /// </summary>
    [Fact]
    public void UseLogging_when_custom_exception_contains_indirect_circular_reference_then_custom_properties_are_not_logged()
    {
        // Arrange
        var outputBuilder = new StringBuilder();
        var services = UseLogging_SetupServicesForSerilog(outputBuilder);
        var sp = BuildServiceProvider(services);
        var logger = sp.GetRequiredService<ILogger<string>>();

        // Act
        logger.LogError(new TestIndirectCircularReferenceException(), "test");

        // Assert
        var output = outputBuilder.ToString(); 
        output.Should().StartWith("Error test SourceContext,ThreadId,ProcessId,MachineName,ExceptionDetail");
        output.Should().EndWith("[(\"Type\": \"Kephas.Extensions.Tests.Hosting.HostBuilderLoggingExtensionsTest+TestIndirectCircularReferenceException\"), (\"HResult\": -2146233088), (\"Message\": \"Exception of type 'Kephas.Extensions.Tests.Hosting.HostBuilderLoggingExtensionsTest+TestIndirectCircularReferenceException' was thrown.\"), (\"Source\": null)]");
    }

    /// <summary>
    /// <see cref="HostBuilderLoggingExtensions.UseLogging{TBuilder}(TBuilder)"/>  is being checked.
    /// </summary>
    [Fact]
    public void UseLogging_when_standard_exception_then_default_destructurer_is_used()
    {
        // Arrange
        var outputBuilder = new StringBuilder();
        var services = UseLogging_SetupServicesForSerilog(outputBuilder);
        var sp = BuildServiceProvider(services);
        var logger = sp.GetRequiredService<ILogger<string>>();

        // Act
        logger.LogError(new AggregateException(new InvalidOperationException("error")), "test");

        // Assert
        var output = outputBuilder.ToString(); 
        output.Should().StartWith("Error test SourceContext,ThreadId,ProcessId,MachineName,ExceptionDetail");
        output.Should().EndWith("[(\"Type\": \"System.AggregateException\"), (\"HResult\": -2146233088), (\"Message\": \"One or more errors occurred. (error)\"), (\"Source\": null), (\"InnerException\": [(\"Type\": \"System.InvalidOperationException\"), (\"HResult\": -2146233079), (\"Message\": \"error\"), (\"Source\": null)]), (\"InnerExceptions\": [[(\"Type\": \"System.InvalidOperationException\"), (\"HResult\": -2146233079), (\"Message\": \"error\"), (\"Source\": null)]])]");
    }

    private static ServiceCollection UseLogging_SetupServicesForSerilog(StringBuilder outputBuilder)
    {
        var services = new ServiceCollection();
        var hostBuilder = GetHostBuilder(services);

        hostBuilder.UseLogging(options =>
        {
            var postConfigure = options.PostConfigure;
            options.PostConfigure = (sp, config) =>
            {
                postConfigure?.Invoke(sp, config);
                config.WriteTo.Logger(new TestLogger(outputBuilder), LogEventLevel.Verbose);
            };
        });
        return services;
    }

#if NET6_0
    private static IHostBuilder GetHostBuilder(ServiceCollection services)
    {
        var hostBuilder = Substitute.For<IHostBuilder>();
        hostBuilder.ConfigureServices(Arg.Any<Action<HostBuilderContext, IServiceCollection>>())
            .Returns(ci =>
            {
                ci.Arg<Action<HostBuilderContext, IServiceCollection>>()(null!, services);
                return hostBuilder;
            });
        return hostBuilder;
    }
#else
    private static IHostApplicationBuilder GetHostBuilder(IServiceCollection services)
    {
        var hostBuilder = Substitute.For<IHostApplicationBuilder>();
        hostBuilder.Services.Returns(services);
        return hostBuilder;
    }
#endif

    private static IServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        var cb = new ConfigurationBuilder();
        cb.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Serilog:MinimumLevel", "Information" }
        });

        services.AddSingleton<IConfiguration>(cb.Build());
        services.AddLogging();
        var sp = services.BuildServiceProvider();
        return sp;
    }

    private class TestCircularReferenceException : Exception
    {
        public Exception CircularReference => this;
    }

    private class TestIndirectCircularReferenceException : Exception
    {
        public TestIndirectCircularReferenceException()
        {
            Context = new TestContext();
            Context.Data = new TestData { Context = Context };
        }

        public TestContext Context { get; }
        
        public Exception CircularReference => this;
    }

    public record TestContext
    {
        public TestData? Data { get; set; }
    }

    public record TestData
    {
        public TestContext? Context { get; set; }
    };
    
    private class TestLogger(StringBuilder sb) : global::Serilog.ILogger
    {
        public void Write(LogEvent logEvent)
        {
            logEvent.Properties.TryGetValue("ExceptionDetail", out var exceptionDetail);
            sb.AppendLine($"{logEvent.Level} {logEvent.MessageTemplate} {string.Join(",", logEvent.Properties.Select(p => p.Key))}");
            using var writer = new StringWriter(sb);
            exceptionDetail?.Render(writer);
        }
    }
}