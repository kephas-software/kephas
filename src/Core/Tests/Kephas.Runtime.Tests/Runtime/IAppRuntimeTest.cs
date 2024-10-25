using System.Net;
using System.Reflection;
using FluentAssertions;
using Kephas.Runtime;
using Xunit;

namespace Kephas.Tests.Runtime;

public class IAppRuntimeTest
{
    [Fact]
    public void GetMachineName()
    {
        IAppRuntime appRuntime = new TestAppRuntime(typeof(IAppRuntime).Assembly);
        appRuntime.GetMachineName().Should().Be(Dns.GetHostName());
    }

    [Fact]
    public void GetAppName_from_assembly()
    {
        IAppRuntime appRuntime = new TestAppRuntime(typeof(IAppRuntime).Assembly);
        appRuntime.GetAppName().Should().Be("Kephas.Runtime");
    }

    public class TestAppRuntime : IAppRuntime
    {
        private readonly Assembly _assembly;

        public TestAppRuntime(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public Assembly? GetEntryAssembly() => _assembly;
        
        public IEnumerable<Assembly> GetAppAssemblies()
        {
            yield return _assembly;
        }
    }
}
