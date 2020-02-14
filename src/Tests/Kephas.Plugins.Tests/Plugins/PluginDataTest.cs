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
            var pluginData = new PluginData(new AppIdentity("Gigi.Belogea", "1.2.3"), PluginState.Disabled, PluginKind.Standalone);
            Assert.AreEqual("Gigi.Belogea:1.2.3\nDisabled\nStandalone\n\n129701855", pluginData.ToString());
        }

        [Test]
        public void ToString_with_data_and_checksum()
        {
            var pluginData = new PluginData(new AppIdentity("Gigi.Belogea", "1.2.3"), PluginState.PendingInitialization)
                    .ChangeData("my", "data");
            Assert.AreEqual("Gigi.Belogea:1.2.3\nPendingInitialization\nEmbedded\nmy:data\n-1975769539", pluginData.ToString());
        }

        [Test]
        public void Parse_valid_checksum()
        {
            var pluginData = PluginData.Parse("Gigi.Belogea:1.2.3\nDisabled\nStandalone\n\n129701855");
            Assert.AreEqual(new AppIdentity("Gigi.Belogea", "1.2.3"), pluginData.Identity);
            Assert.AreEqual(PluginState.Disabled, pluginData.State);
            Assert.AreEqual(PluginKind.Standalone, pluginData.Kind);
        }

        [Test]
        public void Parse_missing_kind()
        {
            var ex = Assert.Throws<InvalidPluginDataException>(() => PluginData.Parse("Gigi.Belogea:1.2.3\nPendingInitialization\nmy:data\n1855999989"));
            Assert.AreEqual($"The plugin data for Gigi.Belogea:1.2.3 is corrupt, probably was manually changed (3).", ex.Message);
        }

        [Test]
        public void Parse_valid_data_and_checksum()
        {
            var pluginData = PluginData.Parse("Gigi.Belogea:1.2.3\nPendingInitialization\nEmbedded\nmy:data\n-1975769539");
            Assert.AreEqual(new AppIdentity("Gigi.Belogea", "1.2.3"), pluginData.Identity);
            Assert.AreEqual(PluginState.PendingInitialization, pluginData.State);
            Assert.AreEqual(PluginKind.Embedded, pluginData.Kind);
            Assert.AreEqual(1, pluginData.Data.Count);
        }

        [Test]
        public void Parse_invalid_checksum()
        {
            var ex = Assert.Throws<InvalidPluginDataException>(() => PluginData.Parse("Gigi.Belogea:1.2.3\nEnabled\nStandalone\n\n274170507"));
            Assert.AreEqual($"The plugin data for Gigi.Belogea:1.2.3 is corrupt, probably was manually changed (100).", ex.Message);
        }

        [Test]
        public void Parse_invalid_app_identity_name()
        {
            var ex = Assert.Throws<ArgumentException>(() => PluginData.Parse("Gigi/Belogea:1.2.3\nEnabled\n\n274170507"));
            Assert.AreEqual("id", ex.ParamName);

#if NETCOREAPP3_1
            var paramNameString = $" (Parameter 'id')";
#else
            var paramNameString = $"{Environment.NewLine}Parameter name: id";
#endif
            Assert.AreEqual($"The app ID 'Gigi/Belogea' may not contain '/'. Not allowed characters: ':;,|/\\<>?'\"*@#$^`[]{{}}'.{paramNameString}", ex.Message);
        }

        [Test]
        public void Parse_invalid_app_identity_version()
        {
            var ex = Assert.Throws<ArgumentException>(() => PluginData.Parse("GigiBelogea:1.2.3{dev}\nEnabled\n\n274170507"));
            Assert.AreEqual("version", ex.ParamName);

#if NETCOREAPP3_1
            var paramNameString = $" (Parameter 'version')";
#else
            var paramNameString = $"{Environment.NewLine}Parameter name: version";
#endif
            Assert.AreEqual($"The app version '1.2.3{{dev}}' may not contain '{{'. Not allowed characters: ':;,|/\\<>?'\"*@#$^`[]{{}}'.{paramNameString}", ex.Message);
        }

        [Test]
        public void ChangeIdentity_different_casing()
        {
            var pluginData = new PluginData(new AppIdentity("Gigi.Belogea", "1.2.3-DEV"), PluginState.Enabled);
            pluginData.ChangeIdentity(new AppIdentity("gigi.belogea", "1.2.3-dev"));
            Assert.AreEqual(new AppIdentity("gigi.belogea", "1.2.3-dev"), pluginData.Identity);
        }

        [Test]
        public void ChangeIdentity_different_values()
        {
            var pluginData = new PluginData(new AppIdentity("Gigi.Belogea", "1.2.3-DEV"), PluginState.Enabled);
            Assert.Throws<InvalidPluginDataException>(() => pluginData.ChangeIdentity(new AppIdentity("gigi.belogea", "1.2.3")));
        }

        [Test]
        public void ChangeIdentity_version_null()
        {
            var pluginData = new PluginData(new AppIdentity("Gigi.Belogea"), PluginState.Enabled);
            pluginData.ChangeIdentity(new AppIdentity("gigi.belogea", "1.2.3-dev"));
            Assert.AreEqual(new AppIdentity("gigi.belogea", "1.2.3-dev"), pluginData.Identity);
        }
    }
}
