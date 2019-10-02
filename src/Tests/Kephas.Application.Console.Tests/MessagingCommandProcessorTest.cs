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
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas.Application.Console.Endpoints;
    using Kephas.Dynamic;
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
            Assert.Contains("Help", helpResponse.Command as ICollection);
            Assert.Contains("Ping", helpResponse.Command as ICollection);
            Assert.Contains("Quit", helpResponse.Command as ICollection);
            Assert.AreEqual("Please provide one command name to see the information about that command. Example: help command=help.", helpResponse.Description);
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
    }
}
