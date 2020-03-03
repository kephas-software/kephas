// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCommandProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging command processor test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Application.Console.Endpoints;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessagingCommandProcessorTest : ConsoleTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            var processor = container.GetExport<ICommandProcessor>();

            Assert.IsNotNull(processor);
        }

        [Test]
        public async Task ProcessAsync_help()
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
        public async Task ProcessAsync_help_indexed_params()
        {
            var container = this.CreateContainer();
            var processor = container.GetExport<ICommandProcessor>();

            var response = await processor.ProcessAsync("help", new Expando { ["help"] = string.Empty });

            Assert.IsInstanceOf<HelpResponseMessage>(response);

            var helpResponse = (HelpResponseMessage)response;
            Assert.AreEqual("Help", helpResponse.Command);
            Assert.AreEqual("Displays the available commands.", helpResponse.Description);
        }

        [Test]
        public async Task ProcessAsync_help_nullable_param()
        {
            var registry = Substitute.For<ICommandRegistry>();
            registry.GetCommandTypes(Arg.Any<string>()).Returns(
                ci => new List<ITypeInfo>()
                    {
                        typeof(HelpMessage).AsRuntimeTypeInfo(),
                        typeof(NullableParamMessage).AsRuntimeTypeInfo(),
                    }.Where(t => string.IsNullOrEmpty(ci.Arg<string>()) || t.Name.StartsWith(ci.Arg<string>().Substring(0, 4), StringComparison.InvariantCultureIgnoreCase)));
            registry.ResolveCommandType("help").Returns(typeof(HelpMessage).AsRuntimeTypeInfo());
            registry.ResolveCommandType("nullable-param").Returns(typeof(NullableParamMessage).AsRuntimeTypeInfo());

            var container = this.CreateContainer(config: b => b.WithFactory<ICommandRegistry>(() => registry));
            var processor = container.GetExport<ICommandProcessor>();

            var response = await processor.ProcessAsync("help", new Expando { ["nullable-param"] = string.Empty });

            Assert.IsInstanceOf<HelpResponseMessage>(response);

            var helpResponse = (HelpResponseMessage)response;
            var command = helpResponse.Command as string;
            Assert.AreEqual("NullableParam", command);
            Assert.AreEqual(1, helpResponse.Parameters.Length);
            Assert.AreEqual("StartTime (System.DateTime?): ", helpResponse.Parameters[0]);
        }

        [Test]
        public async Task ProcessAsync_nullable_param()
        {
            var registry = Substitute.For<ICommandRegistry>();
            registry.GetCommandTypes(Arg.Any<string>()).Returns(
                ci => new List<ITypeInfo>()
                    {
                        typeof(NullableParamMessage).AsRuntimeTypeInfo(),
                    }.Where(t => string.IsNullOrEmpty(ci.Arg<string>()) || t.Name.StartsWith(ci.Arg<string>().Substring(0, 4), StringComparison.InvariantCultureIgnoreCase)));
            registry.ResolveCommandType("nullable-param").Returns(typeof(NullableParamMessage).AsRuntimeTypeInfo());

            var container = this.CreateContainer(config: b => b.WithFactory<ICommandRegistry>(() => registry));
            var processor = container.GetExport<ICommandProcessor>();
            var handlerRegistry = container.GetExport<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<NullableParamMessage>((msg, ctx) => new ResponseMessage { Message = $"Start time: {msg.StartTime:s}" });

            var response = await processor.ProcessAsync("nullable-param", new Expando { ["StartTime"] = "2020-02-23" });

            Assert.IsInstanceOf<ResponseMessage>(response);

            var typedResponse = (ResponseMessage)response;
            Assert.AreEqual("Start time: 2020-02-23T00:00:00", typedResponse.Message);
        }

        [Test]
        public async Task ProcessAsync_enum_param()
        {
            var registry = Substitute.For<ICommandRegistry>();
            registry.GetCommandTypes(Arg.Any<string>()).Returns(
                ci => new List<ITypeInfo>()
                    {
                        typeof(EnumMessage).AsRuntimeTypeInfo(),
                    }.Where(t => string.IsNullOrEmpty(ci.Arg<string>()) || t.Name.StartsWith(ci.Arg<string>().Substring(0, 4), StringComparison.InvariantCultureIgnoreCase)));
            registry.ResolveCommandType("enum").Returns(typeof(EnumMessage).AsRuntimeTypeInfo());

            var container = this.CreateContainer(config: b => b.WithFactory<ICommandRegistry>(() => registry));
            var processor = container.GetExport<ICommandProcessor>();
            var handlerRegistry = container.GetExport<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<EnumMessage>((msg, ctx) => new ResponseMessage { Message = $"Log level: {msg.LogLevel}" });

            var response = await processor.ProcessAsync("enum", new Expando { ["LogLevel"] = "Warning" });

            Assert.IsInstanceOf<ResponseMessage>(response);

            var typedResponse = (ResponseMessage)response;
            Assert.AreEqual("LogLevel: Warning", typedResponse.Message);
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
