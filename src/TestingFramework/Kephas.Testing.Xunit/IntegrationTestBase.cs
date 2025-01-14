// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.Extensions;
using Kephas.Runtime;
using Kephas.Runtime.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Testing;

/// <summary>
/// Base class for integration tests.
/// </summary>
public abstract class IntegrationTestBase<TTestClass> : IAsyncLifetime
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestBase{TTestClass}"/> class.
    /// Adds the following services:
    /// <list type="bullet">
    ///     <item>
    ///         <description>Dependency injection with the provided <paramref name="configuration"/>, or <see cref="Test.GetIntegrationConfig"/> if none provided, and using the <typeparamref name="TTestClass"/>'s assembly as an entry assembly.</description>
    ///     </item>
    ///     <item>
    ///         <description>Serilog integration logging.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="assemblyNamePattern">The assembly name pattern.</param>
    /// <param name="configuration">The configuration instance. If not provided, the default integration configuration will be used.</param>
    /// <param name="logBuilder">Optional. The log builder used to verify the log output.</param>
    protected IntegrationTestBase(string assemblyNamePattern, IConfiguration? configuration = null, StringBuilder? logBuilder = null)
    {
        LogBuilder = logBuilder ?? new StringBuilder();
        Configuration = configuration ?? Test.GetIntegrationConfig();

        var appRuntimeSettings = new AppRuntimeSettings
        {
            // the testing infrastructure does not have an entry assembly, that is why we need to set it explicitly.
            EntryAssembly = typeof(TTestClass).Assembly,
            AppAssemblyNamePattern = assemblyNamePattern,
        };

        Services
            .AddApplicationServices(Configuration, new AppRuntime(appRuntimeSettings))
            .AddIntegrationLogging(Configuration)
            .AddSingleton(Configuration);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestBase{TTestClass}"/> class.
    /// Adds the following services:
    /// <list type="bullet">
    ///     <item>
    ///         <description>Dependency injection with the provided <paramref name="configuration"/>, or <see cref="Test.GetIntegrationConfig"/> if none provided, and using the <typeparamref name="TTestClass"/>'s assembly as an entry assembly.</description>
    ///     </item>
    ///     <item>
    ///         <description>Serilog integration logging.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="includeTestAssembly">If <c>true</c>, includes in the DI container also services from the test assembly, otherwise not.</param>
    /// <param name="configuration">The configuration instance. If not provided, the default integration configuration will be used.</param>
    protected IntegrationTestBase(bool includeTestAssembly = false, IConfiguration? configuration = null)
        : this(
            includeTestAssembly
                ? AppRuntimeSettings.TestsIncludedAppAssemblyNamePattern
                : AppRuntimeSettings.DefaultAppAssemblyNamePattern,
            configuration)
    {
    }

    /// <summary>
    /// Gets the log builder.
    /// </summary>
    protected StringBuilder LogBuilder { get; }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    protected IConfiguration Configuration { get; }
    
    /// <summary>
    /// Gets the service collection. Add all the required services in the constructor.
    /// </summary>
    protected IServiceCollection Services { get; } = new ServiceCollection();

    /// <summary>
    /// Gets the root service provider. It is initialized in <see cref="InitializeAsync"/>
    /// and is disposed in <see cref="DisposeAsync"/>.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    /// The default service scope.
    /// </summary>
    protected IServiceScope DefaultScope { get; private set; } = null!;
    
    /// <summary>
    /// Called immediately after the class has been created, before it is used.
    /// </summary>
    /// <remarks>
    /// Override this method to add test data after the default initialization is completed.
    /// </remarks>
    public virtual Task InitializeAsync()
    {
        ServiceProvider = Services.BuildServiceProvider();
        DefaultScope = ServiceProvider.CreateAsyncScope();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when an object is no longer needed. Called just before <see cref="M:System.IDisposable.Dispose" />
    /// if the class also implements that.
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        switch (DefaultScope)
        {
            case IAsyncDisposable asyncScope:
                await asyncScope.DisposeAsync();
                break;
            case IDisposable scope:
                scope.Dispose();
                break;
        }

        switch (ServiceProvider)
        {
            case IAsyncDisposable asyncProvider:
                await asyncProvider.DisposeAsync();
                break;
            case IDisposable provider:
                provider.Dispose();
                break;
        }
    }
}