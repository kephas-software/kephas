namespace Kephas.Platform.Net45.Tests.Application
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Application;

    using NUnit.Framework;

    [TestFixture]
    public class Net45AppEnvironmentTest
    {
        [Test]
        public async Task GetAppAssembliesAsync_success()
        {
            var appEnv = new Net45AppEnvironment();
            var assemblies = await appEnv.GetAppAssembliesAsync();
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(3, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
        }
    }
}