// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfigurationProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application configuration provider test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Configuration.Providers
{
    using Kephas.Application.Configuration;
    using Kephas.Configuration.Providers;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AppConfigurationProviderTest
    {
        [Test]
        public void GetSettings()
        {
            var appConfiguration = Substitute.For<IAppConfiguration>();
            var settings = new TestSettings();
            appConfiguration["test"].Returns(settings);
            var provider = new AppConfigurationProvider(appConfiguration);
            var actual = provider.GetSettings(typeof(TestSettings));
            Assert.AreSame(settings, actual);
        }

        public class TestSettings { }
    }
}