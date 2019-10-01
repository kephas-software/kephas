namespace Kephas.Core.Tests.Application
{
    using System;

    using Kephas.Application;

    using NUnit.Framework;

    [TestFixture]
    public class NullAppManifestTest
    {
        [Test]
        public void DefaultAppManifest_success()
        {
            var appManifest = new NullAppManifest();

            Assert.AreEqual("null", appManifest.AppId);
            Assert.AreEqual(new Version("0.0.0.0"), appManifest.AppVersion);
        }
    }
}