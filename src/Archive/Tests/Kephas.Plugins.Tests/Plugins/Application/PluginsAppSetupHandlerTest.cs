// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsAppSetupHandlerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Plugins.Application
{
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Plugins;
    using Kephas.Plugins.Application;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class PluginsAppSetupHandlerTest : PluginsTestBase
    {
        [Test]
        public async Task SetupAsync_do_nothing()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(ctx);
            var handler = new PluginsAppSetupHandler(pluginManager);

            var pluginIdentity = new AppIdentity("p1", "1.2.3");
            var installResult = await pluginManager.InstallPluginAsync(pluginIdentity);

            var result = await handler.SetupAsync(Substitute.For<IContext>());
            CollectionAssert.IsEmpty(result.Messages);
        }

        [Test]
        public async Task SetupAsync_two_step_uninstall()
        {
            using var ctx = new PluginsTestContext();
            var canUninstall = false;
            var pluginManager = this.CreatePluginManager(ctx, canUninstall: (p, ctx) => canUninstall);
            var handler = new PluginsAppSetupHandler(pluginManager);

            var pluginIdentity = new AppIdentity("p1", "1.2.3");
            var installResult = await pluginManager.InstallPluginAsync(pluginIdentity);

            Assert.That(async () => await pluginManager.UpdatePluginAsync(new AppIdentity("p1", "2.0.0")), Throws.InstanceOf<PluginOperationException>());

            canUninstall = true;
            var result = await handler.SetupAsync(Substitute.For<IContext>());
            Assert.IsTrue(result.Messages.Count > 8, "Should update the plugin.");
        }
    }
}