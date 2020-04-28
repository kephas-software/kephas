// --------------------------------------------------------------------------------------------------------------------
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Commands.Messaging.Endpoints;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessagingCommandRegistryTest : CommandsTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            var registries = container.GetExports<ICommandRegistry>();

            var msgRegistry = registries.OfType<MessagingCommandRegistry>().Single();
            var commandTypes = msgRegistry.GetCommandTypes().ToList();

            Assert.AreEqual(2, commandTypes.Count);
            Assert.IsTrue(commandTypes.Any(c => c.Name == "Help"));
            Assert.IsTrue(commandTypes.Any(c => c.Name == "Quit"));
        }

        [Test]
        public async Task ProcessAsync_composition_help()
        {
            var container = this.CreateContainer();
            var processor = container.GetExport<ICommandProcessor>();

            var response = await processor.ProcessAsync("help");

            Assert.IsInstanceOf<HelpResponseMessage>(response);

            var helpResponse = (HelpResponseMessage)response;
            var commands = helpResponse.Command as IEnumerable<KeyValuePair<string, string>>;
            Assert.IsTrue(commands.Any(c => c.Key == "Ping"));
            Assert.IsTrue(commands.Any(c => c.Key == "Quit"));
            Assert.AreEqual("Please provide one command name to see the information about that command. Example: 'help <command>'.", helpResponse.Description);
        }

        [Test]
        public async Task ProcessAsync_composition_help_indexed_params()
        {
            var container = this.CreateContainer();
            var processor = container.GetExport<ICommandProcessor>();

            var response = await processor.ProcessAsync("help", new Expando { ["help"] = true });

            Assert.IsInstanceOf<HelpResponseMessage>(response);

            var helpResponse = (HelpResponseMessage)response;
            Assert.AreEqual("Help", helpResponse.Command);
            Assert.AreEqual("Displays the available commands. Use 'help <command>' to display information about the requested command.", helpResponse.Description);
        }

        [Test]
        public async Task ProcessAsync_composition_partial_command_name()
        {
            var container = this.CreateContainer(
                parts: new[] { typeof(NullableParamMessage) });
            var processor = container.GetExport<ICommandProcessor>();
            var handlerRegistry = container.GetExport<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<NullableParamMessage>((msg, ctx) => new ResponseMessage { Message = $"Start time: {msg.StartTime:s}" });

            var response = await processor.ProcessAsync("null", new Expando { ["StartTime"] = "2020-02-23" });

            Assert.IsInstanceOf<ResponseMessage>(response);

            var typedResponse = (ResponseMessage)response;
            Assert.AreEqual("Start time: 2020-02-23T00:00:00", typedResponse.Message);
        }

        [Test]
        public async Task ProcessAsync_composition_enum_param()
        {
            var container = this.CreateContainer(
                parts: new[] { typeof(EnumMessage) });
            var processor = container.GetExport<ICommandProcessor>();
            var handlerRegistry = container.GetExport<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<EnumMessage>((msg, ctx) => new ResponseMessage { Message = $"Log level: {msg.LogLevel}" });

            var response = await processor.ProcessAsync("enum", new Expando { ["LogLevel"] = "warning" });

            Assert.IsInstanceOf<ResponseMessage>(response);

            var typedResponse = (ResponseMessage)response;
            Assert.AreEqual("Log level: Warning", typedResponse.Message);
        }

        public class NullableParamMessage : IMessage
        {
            public DateTime? StartTime { get; set; }
        }

        public class EnumMessage : IMessage
        {
            public LogLevel? LogLevel { get; set; }
        }
    }
}
