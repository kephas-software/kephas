namespace Kephas.Platform.Net46.Tests.Configuration
{
    using Kephas.Configuration;
    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class AppSettingsConfigurationManagerTest
    {
        [Test]
        public void Indexer_setter()
        {
            var service = new AppSettingsConfigurationManager();
            var value = new object();
            service["hello"] = value;

            Assert.AreSame(value, service["hello"]);
        }

        [Test]
        public void GetSettings_with_pattern()
        {
            var service = new AppSettingsConfigurationManager();
            var settings = service.GetSettings<Expando>("My*");
            Assert.AreEqual("myValue", settings["MySetting"]);
            Assert.AreEqual("myOtherValue", settings["MyOtherSetting"]);
        }

        [Test]
        public void GetSettings_typed_with_pattern()
        {
            var service = new AppSettingsConfigurationManager();
            var settings = service.GetSettings<MySettings>("My*");
            Assert.AreEqual("myValue", settings.MySetting);
            Assert.AreEqual("myOtherValue", settings.MyOtherSetting);
        }

        public class MySettings
        {
            public string MySetting { get; set; }

            public string MyOtherSetting { get; set; }
        }
    }
}