// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCommandShellTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default command shell test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultCommandShellTest : ConsoleTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            var shell = container.GetExport<ICommandShell>();

            Assert.IsInstanceOf<DefaultCommandShell>(shell);
        }

        [Test]
        public async Task StartAsync_token_cancellation_stops_processing()
        {
            var processor = Substitute.For<ICommandProcessor>();
            var console = Substitute.For<IConsole>();

            var shell = new DefaultCommandShell(console, processor, Substitute.For<ISerializationService>(), Substitute.For<IContextFactory>());
            using (var source = new CancellationTokenSource())
            {
                var startTask = shell.StartAsync(source.Token);
                Assert.ThrowsAsync(Is.AssignableTo(typeof(OperationCanceledException)), () => Task.WhenAll(startTask, Task.Run(() => source.Cancel()), Task.Delay(100)));
                Assert.AreEqual(TaskStatus.Canceled, startTask.Status);
            }
        }

        [Test]
        public async Task StartAsync_process_commands()
        {
            var processor = Substitute.For<ICommandProcessor>();
            var console = Substitute.For<IConsole>();
            var output = new List<string>();
            var input = new[] { "hi", "there" };
            var inputEnumerator = input.GetEnumerator();
            console.When(c => c.WriteLine(Arg.Any<string>()))
                .Do(ci => output.Add(ci.Arg<string>() + Environment.NewLine));
            console.When(c => c.Write(Arg.Any<string>()))
                .Do(ci => output.Add(ci.Arg<string>()));
            console.ReadLine()
                .Returns(ci => inputEnumerator.MoveNext() ? inputEnumerator.Current : throw new OperationCanceledException());

            var serializer = Substitute.For<ISerializer>();
            var serialization = this.CreateSerializationServiceMock();
            serialization.GetSerializer(Arg.Any<ISerializationContext>()).Returns(serializer);
            serializer.SerializeAsync(Arg.Any<object>(), Arg.Any<TextWriter>(), Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(ci =>
                {
                    ci.Arg<TextWriter>().Write(ci.Arg<object>().ToString());
                    return TaskHelper.CompletedTask;
                });

            processor.ProcessAsync(Arg.Any<string>(), Arg.Any<IExpando>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult<object>(ci.Arg<string>() + " processed"));

            var contextFactory = this.CreateContextFactory(() => new SerializationContext(Substitute.For<ICompositionContext>(), serialization));

            var shell = new DefaultCommandShell(console, processor, serialization, contextFactory);
            Assert.ThrowsAsync<OperationCanceledException>(() => shell.StartAsync(default));

            Assert.AreEqual(7, output.Count);
            Assert.AreEqual("Type 'quit' and hit <ENTER> to terminate the application." + Environment.NewLine, output[0]);
            Assert.AreEqual("Type 'help' and hit <ENTER> to see a list of available commands." + Environment.NewLine, output[1]);
            Assert.AreEqual("> ", output[2]);
            Assert.AreEqual("hi processed" + Environment.NewLine, output[3]);
            Assert.AreEqual("> ", output[4]);
            Assert.AreEqual("there processed" + Environment.NewLine, output[5]);
            Assert.AreEqual("> ", output[6]);
        }
    }
}
