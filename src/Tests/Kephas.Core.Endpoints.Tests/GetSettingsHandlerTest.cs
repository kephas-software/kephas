// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsHandlerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get settings handler test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class GetSettingsHandlerTest
    {
        [Test]
        public async Task ProcessAsync()
        {
            var settings = new CoreSettings();
            var config = Substitute.For<IConfiguration<CoreSettings>>();
            config.Settings.Returns(settings);
            var container = Substitute.For<ICompositionContext>();
            container.GetExport(typeof(IConfiguration<CoreSettings>))
                .Returns(config);
            var typeResolver = new DefaultTypeResolver(() => new List<Assembly> { typeof(CoreSettings).Assembly });
            var handler = new GetSettingsHandler(container, typeResolver);
            var result = await handler.ProcessAsync(
                new GetSettingsMessage { SettingsType = "core" },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.AreSame(settings, result.Settings);
        }
    }
}
