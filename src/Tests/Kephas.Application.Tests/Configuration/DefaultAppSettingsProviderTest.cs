﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppSettingsProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests.Configuration
{
    using Kephas.Application.Configuration;
    using Kephas.Configuration;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultAppSettingsProviderTest : ApplicationTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            var provider = container.GetExport<IAppSettingsProvider>();

            Assert.IsInstanceOf<DefaultAppSettingsProvider>(provider);
        }

        [Test]
        public void GetAppSettings_success()
        {
            var appSettings = new AppSettings();
            var config = Substitute.For<IConfiguration<AppSettings>>();
            config.GetSettings(Arg.Any<IContext?>()).Returns(appSettings);
            var provider = new DefaultAppSettingsProvider(Substitute.For<IAppRuntime>(), config);

            Assert.AreSame(appSettings, provider.GetAppSettings());
        }
    }
}