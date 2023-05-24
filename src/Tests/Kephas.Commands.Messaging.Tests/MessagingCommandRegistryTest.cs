﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCommandProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging command processor test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Kephas.Commands.Endpoints;
    using Kephas.Dynamic;
    using Kephas.Security.Authorization;
    using NUnit.Framework;

    [TestFixture]
    public class MessagingCommandRegistryTest : CommandsTestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IAuthorizationService).Assembly,     // Kephas.Security
            };
        }

        [Test]
        public void Injection()
        {
            var container = this.BuildServiceProvider();
            var registries = container.ResolveMany<ICommandRegistry>();

            var msgRegistry = registries.OfType<MessagingCommandRegistry>().Single();
            var commandTypes = msgRegistry.GetCommandTypes().ToList();

            Assert.AreEqual(3, commandTypes.Count);
            Assert.IsTrue(commandTypes.Any(c => c.Name == "Help"));
            Assert.IsTrue(commandTypes.Any(c => c.Name == "Quit"));
            Assert.IsTrue(commandTypes.Any(c => c.Name == "Ping"));
        }

        [Test]
        public async Task ProcessAsync_injection_help()
        {
            var container = this.BuildServiceProvider();
            var processor = container.Resolve<ICommandProcessor>();

            var response = await processor.ProcessAsync("help");

            Assert.IsInstanceOf<HelpResponse>(response);

            var helpResponse = (HelpResponse)response;
            var commands = helpResponse.Command as IEnumerable<KeyValuePair<string, string>>;
            Assert.IsTrue(commands.Any(c => c.Key == "Ping"));
            Assert.IsTrue(commands.Any(c => c.Key == "Quit"));
            Assert.AreEqual("Please provide one command name to see the information about that command. Example: 'help <command>'.", helpResponse.Description);
        }

        [Test]
        public async Task ProcessAsync_injection_help_indexed_params()
        {
            var container = this.BuildServiceProvider();
            var processor = container.Resolve<ICommandProcessor>();

            var response = await processor.ProcessAsync("help", new Expando { ["help"] = true });

            Assert.IsInstanceOf<HelpResponse>(response);

            var helpResponse = (HelpResponse)response;
            Assert.AreEqual("Help", helpResponse.Command);
            Assert.AreEqual("Displays the available commands. Use 'help <command>' to display information about the requested command.", helpResponse.Description);
        }
    }
}
