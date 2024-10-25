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
            "Kephas.Collections",
            "Kephas.Extensions",
            "Kephas.Runtime.AssemblyLoader",
            "xunit.core",
            "xunit.abstractions",
            "xunit.assert",
            "Castle.Core",
            "Newtonsoft.Json",
            "NSubstitute",
            "xunit.execution.dotnet",
            "xunit.runner.reporters.netcoreapp10",
            "xunit.runner.utility.netcoreapp10",
            "xunit.runner.visualstudio.testadapter"
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
            "Kephas.Collections",
            "Kephas.Extensions",
            "Kephas.Runtime.AssemblyLoader",
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
        Assert.Equal("Kephas.Runtime", appRuntime.GetAppName());
    }

    [Fact]
    public void GetAppName_from_settings()
    {
        IAppRuntime appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = typeof(IAppRuntime).Assembly, AppName = "app"});
        Assert.Equal("app", appRuntime.GetAppName());
    }

    [Fact]
    public void GetEnvironment_from_settings()
    {
        IAppRuntime appRuntime = new AppRuntime(new AppRuntimeSettings {EntryAssembly = typeof(IAppRuntime).Assembly, Environment = "env"});
        Assert.Equal("env", appRuntime.GetEnvironment());
    }
}