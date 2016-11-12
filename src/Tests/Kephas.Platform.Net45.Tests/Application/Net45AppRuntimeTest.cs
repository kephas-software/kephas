using Kephas.Reflection;

namespace Kephas.Platform.Net45.Tests.Application
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Application;

    using NUnit.Framework;

    [TestFixture]
    public class Net45AppRuntimeTest
    {
        [Test]
        public async Task GetAppAssembliesAsync_success()
        {
            var appEnv = new Net45AppRuntime();
            var assemblies = await appEnv.GetAppAssembliesAsync(n => !n.IsSystemAssembly() && !n.FullName.StartsWith("JetBrains"));
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(3, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
        }
    }
}