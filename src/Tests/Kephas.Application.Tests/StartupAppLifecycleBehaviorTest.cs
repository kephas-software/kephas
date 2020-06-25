// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupAppLifecycleBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application.Configuration;
    using Kephas.Application.Interaction;
    using Kephas.Configuration;
    using Kephas.Interaction;
    using Kephas.Runtime;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class StartupAppLifecycleBehaviorTest
    {
        [Test]
        public async Task BeforeAppInitializeAsync_string_command()
        {
            var eventHub = this.CreateEventHubMock();

            var settings = new SystemSettings();
            var config = Substitute.For<IConfiguration<SystemSettings>>();
            config.Settings.Returns(settings);

            var behavior = new StartupAppLifecycleBehavior(eventHub, config, new RuntimeTypeRegistry());
            await behavior.BeforeAppInitializeAsync(Substitute.For<IAppContext>(), default);

            await eventHub.PublishAsync(new ScheduleStartupCommandSignal("hello"), Substitute.For<IContext>(), default);
            Assert.AreEqual(1, settings.SetupCommands.Length);
            Assert.AreEqual("hello", settings.SetupCommands[0]);

            config.Received(1).UpdateSettingsAsync(null, Arg.Any<IContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task BeforeAppInitializeAsync_message_command()
        {
            var eventHub = this.CreateEventHubMock();

            var settings = new SystemSettings();
            var config = Substitute.For<IConfiguration<SystemSettings>>();
            config.Settings.Returns(settings);

            var behavior = new StartupAppLifecycleBehavior(eventHub, config, new RuntimeTypeRegistry());
            await behavior.BeforeAppInitializeAsync(Substitute.For<IAppContext>(), default);

            await eventHub.PublishAsync(new ScheduleStartupCommandSignal(new HelloMessage { To = "gigi", Likes = 3 }), Substitute.For<IContext>(), default);
            Assert.AreEqual(1, settings.SetupCommands.Length);
            Assert.AreEqual("Hello To=\"gigi\" Likes=3", settings.SetupCommands[0]);

            config.Received(1).UpdateSettingsAsync(null, Arg.Any<IContext>(), Arg.Any<CancellationToken>());
        }

        private IEventHub CreateEventHubMock()
        {
            var eventHub = Substitute.For<IEventHub>();
            Func<object, IContext?, CancellationToken, Task>? handler = null;
            eventHub.Subscribe(
                    typeof(ScheduleStartupCommandSignal),
                    Arg.Any<Func<object, IContext?, CancellationToken, Task>>())
                .Returns(ci =>
                {
                    handler = ci.Arg<Func<object, IContext?, CancellationToken, Task>>();
                    return Substitute.For<IEventSubscription>();
                });
            eventHub.PublishAsync(Arg.Any<object>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => handler?.Invoke(ci.Arg<object>(), ci.Arg<IContext>(), ci.Arg<CancellationToken>()) ??
                               Task.CompletedTask);
            return eventHub;
        }

        private class HelloMessage
        {
            public string From { get; set; } = "me";
            public string To { get; set; } = "you";
            public int Likes { get; set; }
        }
    }
}