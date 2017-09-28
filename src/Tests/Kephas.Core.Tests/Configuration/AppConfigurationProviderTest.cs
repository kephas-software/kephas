// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfigurationProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application configuration provider test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Configuration
{
    using Kephas.Configuration;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AppConfigurationProviderTest
    {
        [Test]
        public void GetSettings()
        {
            var appConfiguration = Substitute.For<IAppConfiguration>();
            var settings = new object();
            appConfiguration.GetSettings(":string:*;string*", typeof(string)).Returns(settings);
            var provider = new AppConfigurationProvider(appConfiguration);
            var actual = provider.GetSettings(typeof(string));
            Assert.AreSame(settings, actual);
        }
    }
}