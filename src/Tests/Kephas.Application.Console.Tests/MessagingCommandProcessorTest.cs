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
    using System.Threading.Tasks;

    using Kephas.Application.Console.Endpoints;

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
        }
    }
}
