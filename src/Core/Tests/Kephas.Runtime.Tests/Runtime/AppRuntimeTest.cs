// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Runtime;
using Kephas.Runtime.Configuration;
using Xunit;

namespace Kephas.Tests.Runtime;

public class AppRuntimeTest
{
    [Fact]
    public void GetAppAssemblies_default_pattern()
    {
        var names = new[]
        {
            "FluentAssertions",
            "Kephas.Runtime",
            "Castle.Core",
            "Newtonsoft.Json",
            "NSubstitute",
        };
        var appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = GetType().Assembly});
        var appAssemblies = appRuntime.GetAppAssemblies(); 
        appAssemblies.Select(a => a.GetName().Name)
            .Should().BeEquivalentTo(names);
    }

    [Fact]
    public void GetAppAssemblies_custom_pattern()
    {
        var names = new[]
        {
            "Kephas.Runtime.Tests",
            "Kephas.Runtime",
        };
        var appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = GetType().Assembly, AppAssemblyNamePattern = "^Kephas\\."});
        var appAssemblies = appRuntime.GetAppAssemblies();
        appAssemblies.Select(a => a.GetName().Name)
            .Should().BeEquivalentTo(names);
    }

    [Fact]
    public void GetAppName_from_assembly()
    {
        IAppRuntime appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = typeof(IAppRuntime).Assembly});
        appRuntime.GetAppName().Should().Be("Kephas.Runtime");
    }

    [Fact]
    public void GetAppName_from_settings()
    {
        IAppRuntime appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = typeof(IAppRuntime).Assembly, AppName = "app"});
        appRuntime.GetAppName().Should().Be("app");
    }

    [Fact]
    public void GetAppVersion_from_assembly()
    {
        IAppRuntime appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = typeof(IAppRuntime).Assembly});
        appRuntime.GetAppVersion().Should().StartWith("1.0.0+");
    }

    [Fact]
    public void GetAppVersion_from_settings()
    {
        IAppRuntime appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = typeof(IAppRuntime).Assembly, AppVersion = "1.2.3.4"});
        appRuntime.GetAppVersion().Should().Be("1.2.3.4");
    }

    [Fact]
    public void GetEnvironment_from_settings()
    {
        IAppRuntime appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = typeof(IAppRuntime).Assembly, Environment = "env"});
        appRuntime.GetEnvironment().Should().Be("env");
    }

    [Fact]
    public void GetEnvironment_unknown()
    {
        IAppRuntime appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = typeof(IAppRuntime).Assembly});
        appRuntime.GetEnvironment().Should().Be(IAppRuntime.DefaultEnvironment);
    }
}