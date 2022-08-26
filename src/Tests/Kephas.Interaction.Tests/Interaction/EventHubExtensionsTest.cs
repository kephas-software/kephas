﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultEventHubTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default event hub test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Kephas.Interaction;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class EventHubExtensionsTest
    {
        [Test]
        public void Subscribe_wrong_args()
        {
            var calls = 0;
            Assert.Throws<ArgumentNullException>(() => EventHubExtensions.Subscribe<string>(null, async (e, c, t) => calls++));

            Assert.Throws<ArgumentNullException>(() => EventHubExtensions.Subscribe<string>(Substitute.For<IEventHub>(), null));
        }

        [Test]
        public async Task Subscribe_func()
        {
            var eventHub = this.CreateEventHub();
            eventHub.Subscribe<string, string>((e, ctx) => e);

            var result = await eventHub.PublishAsync("hello", null);
            CollectionAssert.AreEqual(new List<object?> { "hello" }, result.Value);
        }

        [Test]
        public async Task Subscribe_async_func()
        {
            var eventHub = this.CreateEventHub();
            eventHub.Subscribe<string, string>((e, ctx, ct) => Task.FromResult(e));

            var result = await eventHub.PublishAsync("hello", null);
            CollectionAssert.AreEqual(new List<object?> { "hello" }, result.Value);
        }

        [Test]
        public async Task Subscribe_action()
        {
            var calls = 0;
            var eventHub = this.CreateEventHub();
            eventHub.Subscribe<string>((e, ctx) => calls++);

            var result = await eventHub.PublishAsync("hello", null);
            CollectionAssert.AreEqual(new List<object?> { null }, result.Value);
            Assert.AreEqual(1, calls);
        }

        [Test]
        public async Task Subscribe_async_action()
        {
            var calls = 0;
            var eventHub = this.CreateEventHub();
            eventHub.Subscribe<string>((e, ctx, ct) =>
            {
                calls++;
                return Task.CompletedTask;
            });

            var result = await eventHub.PublishAsync("hello", null);
            CollectionAssert.AreEqual(new List<object?> { null }, result.Value);
            Assert.AreEqual(1, calls);
        }

        private DefaultEventHub CreateEventHub(IInjectableFactory? contextFactory = null)
        {
            return new DefaultEventHub(Substitute.For<IInjectableFactory>());
        }
    }
}