// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginDataTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin data test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Plugins
{
    using System;

    using Kephas.Application;
    using Kephas.Plugins;
    using NUnit.Framework;

    [TestFixture]
    public class PluginDataTest
    {
        [Test]
        public void PluginData_data_not_null()
        {
            var pluginData = new PluginData(new AppIdentity("Gigi.Belogea", "1.2.3"), PluginState.Disabled);
            Assert.IsNotNull(pluginData.Data);
            Assert.AreEqual(0, pluginData.Data.Count);
        }

        [Test]
        public void ToString_with_checksum()
        {
            var pluginData = new PluginData(new AppIdentity("Gigi.Belogea", "1.2.3"), PluginState.Disabled);
            Assert.AreEqual("Gigi.Belogea:1.2.3\nDisabled\n\n274170507", pluginData.ToString());
        }

        [Test]
        public void ToString_with_data_and_checksum()
        {
            var pluginData = new PluginData(new AppIdentity("Gigi.Belogea", "1.2.3"), PluginState.PendingInitialization)
                    .ChangeData("my", "data");
            Assert.AreEqual("Gigi.Belogea:1.2.3\nPendingInitialization\nmy:data\n1855999989", pluginData.ToString());
        }

        [Test]
        public void Parse_valid_checksum()
        {
            var pluginData = PluginData.Parse("Gigi.Belogea:1.2.3\nDisabled\n\n274170507");
            Assert.AreEqual(new AppIdentity("Gigi.Belogea", "1.2.3"), pluginData.Identity);
            Assert.AreEqual(PluginState.Disabled, pluginData.State);
        }

        [Test]
        public void Parse_valid_data_and_checksum()
        {
            var pluginData = PluginData.Parse("Gigi.Belogea:1.2.3\nPendingInitialization\nmy:data\n1855999989");
            Assert.AreEqual(new AppIdentity("Gigi.Belogea", "1.2.3"), pluginData.Identity);
            Assert.AreEqual(PluginState.PendingInitialization, pluginData.State);
            Assert.AreEqual(1, pluginData.Data.Count);
        }

        [Test]
        public void Parse_invalid_checksum()
        {
            Assert.Throws<InvalidPluginDataException>(() => PluginData.Parse("Gigi.Belogea:1.2.3\nEnabled\n\n274170507"));
        }

        [Test]
        public void Parse_invalid_app_identity_name()
        {
            Assert.Throws<ArgumentException>(() => PluginData.Parse("Gigi/Belogea:1.2.3\nEnabled\n\n274170507"));
        }

        [Test]
        public void Parse_invalid_app_identity_version()
        {
            Assert.Throws<ArgumentException>(() => PluginData.Parse("GigiBelogea:1.2.3{dev}\nEnabled\n\n274170507"));
        }
    }
}
