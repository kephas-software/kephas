namespace Kephas.Core.Tests.Application
{
    using System;
    using System.Reflection;

    using Kephas.Application;

    using NUnit.Framework;

    [TestFixture]
    public class AppManifestBaseTest
    {
        [Test]
        public void AppManifestBase_Empty()
        {
            Assert.Throws<InvalidOperationException>(() => new EmptyAppManifest());
        }

        [Test]
        public void AppManifestBase_AppAssembly_NoAppManifestAttributeException()
        {
            Assert.Throws<InvalidOperationException>(() => new AssemblyAppManifest(typeof(AppManifestBaseTest).Assembly));
        }

        [Test]
        public void AppManifestBase_AppAssembly_success()
        {
            var appManifest = new AssemblyAppManifest(typeof(AppManifestBase).Assembly);
            Assert.AreEqual("kephas", appManifest.AppId);
            Assert.AreEqual(new Version("3.10.0"), appManifest.AppVersion);
        }

        [Test]
        public void AppManifestBase_AppIdAndVersion_success()
        {
            var appManifest = new AppIdAndVersionManifest("hello", new Version("2.3.4"));
            Assert.AreEqual("hello", appManifest.AppId);
            Assert.AreEqual(new Version("2.3.4"), appManifest.AppVersion);
        }

        [Test]
        public void AppManifestBase_AppId_success()
        {
            var appManifest = new AppIdAndVersionManifest("hello");
            Assert.AreEqual("hello", appManifest.AppId);

            var appVersion = typeof(AppManifestBaseTest).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            Assert.AreEqual(new Version(appVersion), appManifest.AppVersion);
        }

        private class EmptyAppManifest : AppManifestBase { }

        private class AssemblyAppManifest : AppManifestBase
        {
            public AssemblyAppManifest(Assembly appAssembly)
                : base(appAssembly)
            {
            }
        }

        private class AppIdAndVersionManifest : AppManifestBase
        {
            public AppIdAndVersionManifest(string appId, Version appVersion = null)
                : base(appId, appVersion)
            {
            }
        }
    }
}