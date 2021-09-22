// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginManagerBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugn manager base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Plugins
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Reflection;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Plugins;
    using Kephas.Plugins.Application;
    using Kephas.Plugins.Reflection;
    using Kephas.Plugins.Transactions;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class PluginManagerBaseTest : PluginsTestBase
    {
        [Test]
        public void GetInstalledPlugins_empty()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(ctx);

            var plugins = pluginManager.GetInstalledPlugins();

            CollectionAssert.IsEmpty(plugins);
        }

        [Test]
        public async Task InstallPluginAsync_p1()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(ctx, (p, pctx) => pctx.Transaction.AddCommand(new TestUndoCommand("h:i", "param|1", "param\n2")));

            var result = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3"));

            Assert.IsNotNull(result);
            Assert.AreEqual(PluginState.Enabled, result.Value.State);
            Assert.AreEqual("p1:1.2.3\nEnabled\nEmbedded\n-inst-undo-1:test|h:i|param&pipe;1|param\\n2\n96402984", result.Value.GetPluginData().ToString());
        }

        [Test]
        public async Task InstallPluginAsync_p1_cannot_install()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(
                ctx,
                canInstall: (d, c) => false);

            Assert.ThrowsAsync<PluginOperationException>(() => pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3")));
        }

        [Test]
        public async Task InstallPluginAsync_p1_cannot_initialize()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(
                ctx,
                canInitialize: (d, c) => false);

            var result = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3"));

            Assert.IsNotNull(result);
            Assert.AreEqual(PluginState.PendingInitialization, result.Value.State);
            Assert.AreEqual("p1:1.2.3\nPendingInitialization\nEmbedded\n\n1878947376", result.Value.GetPluginData().ToString());
        }

        [Test]
        public async Task InstallPluginAsync_p1_cannot_enable()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(
                ctx,
                canEnable: (d, c) => false);

            var result = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3"));

            Assert.IsNotNull(result);
            Assert.AreEqual(PluginState.Disabled, result.Value.State);
            Assert.AreEqual("p1:1.2.3\nDisabled\nEmbedded\n\n1878949554", result.Value.GetPluginData().ToString());
        }

        [Test]
        public async Task UninstallPluginAsync_p1()
        {
            using var ctx = new PluginsTestContext();
            var callbackCalls = 0;
            var pluginManager = this.CreatePluginManager(ctx, (p, pctx) => pctx.Transaction.AddCommand(new TestUndoCommand("h:i", "param|1", "param\n2")));

            var instResult = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3"));
            var uninstResult = await pluginManager.UninstallPluginAsync(new AppIdentity("p1"), pctx => pctx["callback"] = (Action)(() => callbackCalls++));

            Assert.IsNotNull(uninstResult);
            Assert.AreEqual(PluginState.None, uninstResult.Value.State);
            Assert.AreEqual("p1:1.2.3\nNone\nEmbedded\n\n1878946417", uninstResult.Value.GetPluginData().ToString());
            Assert.AreEqual(1, callbackCalls);
        }

        [Test]
        public async Task UninstallPluginAsync_p1_not_installed()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(
                ctx,
                canUninstall: (d, c) => false);

            Assert.ThrowsAsync<PluginOperationException>(() => pluginManager.UninstallPluginAsync(new AppIdentity("p1")));
        }

        [Test]
        public async Task UninstallPluginAsync_p1_cannot_uninstall()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(
                ctx,
                canUninstall: (d, c) => false);

            var instResult = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3"));
            var uninstResult = await pluginManager.UninstallPluginAsync(new AppIdentity("p1"));

            Assert.IsNotNull(uninstResult);
            Assert.AreEqual(PluginState.PendingUninstallation, uninstResult.Value.State);
            Assert.AreEqual("p1:1.2.3\nPendingUninstallation\nEmbedded\n\n1878943028", uninstResult.Value.GetPluginData().ToString());
        }

        [Test]
        public async Task UninstallPluginAsync_p1_cannot_uninitialize()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(
                ctx,
                canUninitialize: (d, c) => false);

            var instResult = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3"));
            Assert.ThrowsAsync<PluginOperationException>(() => pluginManager.UninstallPluginAsync(new AppIdentity("p1")));
        }

        [Test]
        public async Task UninstallPluginAsync_p1_cannot_disable()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(
                ctx,
                canDisable: (d, c) => false);

            var instResult = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3"));
            Assert.ThrowsAsync<PluginOperationException>(() => pluginManager.UninstallPluginAsync(new AppIdentity("p1", "1.2.3")));
        }

        [Test]
        public async Task UpdatePluginAsync_p1()
        {
            using var ctx = new PluginsTestContext();
            var pluginManager = this.CreatePluginManager(ctx);

            var result = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3"));

            result = await pluginManager.UpdatePluginAsync(new AppIdentity("p1", "2.0.0"));

            Assert.IsNotNull(result);
            Assert.AreEqual(PluginState.Enabled, result.Value.State);
            Assert.AreEqual("p1:2.0.0\nEnabled\nEmbedded\n\n970928651", result.Value.GetPluginData().ToString());
        }

        [Test]
        public async Task UpdatePluginAsync_p1_two_step_uninstall()
        {
            using var ctx = new PluginsTestContext();
            var canUninstall = false;
            var pluginManager = this.CreatePluginManager(ctx, canUninstall: (p, ctx) => canUninstall);

            var pluginIdentity = new AppIdentity("p1", "1.2.3");
            var result = await pluginManager.InstallPluginAsync(pluginIdentity);

            Assert.That(async () => await pluginManager.UpdatePluginAsync(new AppIdentity("p1", "2.0.0")), Throws.InstanceOf<PluginOperationException>());

            Assert.AreEqual(PluginState.PendingUninstallation, pluginManager.GetPluginState(pluginIdentity));

            canUninstall = true;
            result = await pluginManager.UpdatePluginAsync(new AppIdentity("p1", "2.0.0"));

            Assert.IsNotNull(result);
            Assert.AreEqual(PluginState.Enabled, result.Value.State);
            Assert.AreEqual("p1:2.0.0\nEnabled\nEmbedded\n\n970928651", result.Value.GetPluginData().ToString());
        }

        public class TestUndoCommand : UndoCommandBase
        {
            public TestUndoCommand(params string[] args)
                : base("test", args)
            {
            }

            public override void Execute(IPluginContext context)
            {
                var callback = context["callback"] as Action;
                callback?.Invoke();
            }
        }
    }
}
