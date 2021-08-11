// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleAppShutdownAwaiterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the console application shutdown awaiter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console.Tests.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Interaction;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class ConsoleAppShutdownAwaiterTest : ConsoleTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            var awaiter = container.GetExport<IAppMainLoop>();

            Assert.IsInstanceOf<ConsoleAppMainLoop>(awaiter);
        }

        [Test]
        public async Task WaitForShutdownSignalAsync_signal_arrived()
        {
            var shell = Substitute.For<ICommandShell>();
            var eventHub = Substitute.For<IEventHub>();
            Func<object, IContext, CancellationToken, Task> callback = null;
            eventHub.Subscribe(Arg.Any<Type>(), Arg.Any<Func<object, IContext, CancellationToken, Task>>())
                .Returns(ci =>
                {
                    callback = ci.Arg<Func<object, IContext, CancellationToken, Task>>();
                    return Substitute.For<IEventSubscription>();
                });

            shell.StartAsync(Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.Delay(100));

            var awaiter = new ConsoleAppMainLoop(shell, eventHub);
            var awaiterTask = awaiter.Main(default);
            await Task.WhenAll(
                awaiterTask,
                callback(new ShutdownSignal("hello"), Substitute.For<IContext>(), default));

            Assert.AreEqual(AppShutdownInstruction.Shutdown, awaiterTask.Result.instruction);
            Assert.AreEqual(OperationState.Canceled, awaiterTask.Result.result.OperationState);
        }

        [Test]
        public async Task WaitForShutdownSignalAsync_quit_from_shell()
        {
            var shell = Substitute.For<ICommandShell>();
            shell.StartAsync(Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.Delay(10));
            var eventHub = Substitute.For<IEventHub>();
            Func<object, IContext, CancellationToken, Task> callback = null;
            eventHub.Subscribe(Arg.Any<Func<object, bool>>(), Arg.Any<Func<object, IContext, CancellationToken, Task>>())
                .Returns(ci =>
                {
                    callback = ci.Arg<Func<object, IContext, CancellationToken, Task>>();
                    return Substitute.For<IEventSubscription>();
                });
            var awaiter = new ConsoleAppMainLoop(shell, eventHub);
            var awaiterTask = awaiter.Main(default);
            await awaiterTask;

            Assert.AreEqual(AppShutdownInstruction.Shutdown, awaiterTask.Result.instruction);
            Assert.AreEqual(OperationState.Completed, awaiterTask.Result.result.OperationState);
        }
    }
}
