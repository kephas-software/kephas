// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultEventHubTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default event hub test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Interaction
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Interaction;
    using Kephas.Operations;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultEventHubTest
    {
        [Test]
        public async Task Subscribe_matching_event()
        {
            var hub = new DefaultEventHub();
            var stringEvent = string.Empty;
            IContext? stringContext = null;
            Exception? exEvent = null;
            IContext? exContext = null;
            using (hub.Subscribe(
                e => e.GetType() == typeof(string),
                (e, ctx, token) =>
                {
                    stringEvent = (string)e;
                    stringContext = ctx;
                    return Task.CompletedTask;
                }))
            using (hub.Subscribe(
                e => e.GetType() == typeof(Exception),
                (e, ctx, token) =>
                {
                    exEvent = (Exception)e;
                    exContext = ctx;
                    return Task.CompletedTask;
                }))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.AreEqual("hello", stringEvent);
                Assert.AreSame(expectedContext, stringContext);
                Assert.IsNull(exEvent);
                Assert.IsNull(exContext);
                Assert.IsFalse(result.HasErrors());
#if NETCOREAPP2_1 || NETCOREAPP3_1
                Assert.IsNull(result.Value);
#endif
            }
        }

        [Test]
        public async Task Subscribe_matching_event_exception()
        {
            var hub = new DefaultEventHub();
            var stringEvent = string.Empty;
            IContext? stringContext = null;
            Exception? exEvent = null;
            IContext? exContext = null;
            using (hub.Subscribe(
                e => e.GetType() == typeof(string),
                (e, ctx, token) =>
                {
                    stringEvent = (string)e;
                    stringContext = ctx;
                    return Task.FromException(new InvalidOperationException());
                }))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.AreEqual("hello", stringEvent);
                Assert.AreSame(expectedContext, stringContext);
                Assert.IsNull(exEvent);
                Assert.IsNull(exContext);
                Assert.IsTrue(result.HasErrors());
                CollectionAssert.AllItemsAreInstancesOfType(result.Exceptions, typeof(InvalidOperationException));
                Assert.IsNull(result.Value);
            }
        }

        [Test]
        public async Task Subscribe_matching_event_with_return_value()
        {
            var hub = new DefaultEventHub();
            using (hub.Subscribe(
                e => e.GetType() == typeof(string),
                (e, ctx, token) => Task.FromResult(12)))
            using (hub.Subscribe(
                e => e.GetType() == typeof(Exception),
                (e, ctx, token) => Task.CompletedTask))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.IsFalse(result.HasErrors());
                Assert.AreEqual(12, result.Value);
            }
        }

        [Test]
        public async Task Subscribe_matching_event_with_return_value_multiple_last_wins()
        {
            var hub = new DefaultEventHub();
            using (hub.Subscribe(
                e => e.GetType() == typeof(string),
                (e, ctx, token) => Task.FromResult(12)))
            using (hub.Subscribe(
                e => e.GetType() == typeof(string),
                (e, ctx, token) => Task.FromResult(24)))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.IsFalse(result.HasErrors());
                Assert.AreEqual(24, result.Value);
            }
        }

        [Test]
        public async Task Subscribe_typed_matching_event()
        {
            var hub = new DefaultEventHub();
            var stringEvent = string.Empty;
            IContext? stringContext = null;
            Exception? exEvent = null;
            IContext? exContext = null;
            using (hub.Subscribe<string>(
                (e, ctx, token) =>
                {
                    stringEvent = e;
                    stringContext = ctx;
                    return Task.CompletedTask;
                }))
            using (hub.Subscribe<Exception>(
                (e, ctx, token) =>
                {
                    exEvent = e;
                    exContext = ctx;
                    return Task.CompletedTask;
                }))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.AreEqual("hello", stringEvent);
                Assert.AreSame(expectedContext, stringContext);
                Assert.IsNull(exEvent);
                Assert.IsNull(exContext);
                Assert.IsFalse(result.HasErrors());
#if NETCOREAPP2_1 || NETCOREAPP3_1
                Assert.IsNull(result.Value);
#endif
            }
        }

        [Test]
        public async Task Subscribe_typed_matching_event_with_return_value()
        {
            var hub = new DefaultEventHub();
            var stringEvent = string.Empty;
            IContext? stringContext = null;
            Exception? exEvent = null;
            IContext? exContext = null;
            using (hub.Subscribe<string>(
                (e, ctx, token) =>
                {
                    stringEvent = e;
                    stringContext = ctx;
                    return Task.FromResult(12);
                }))
            using (hub.Subscribe<Exception>(
                (e, ctx, token) =>
                {
                    exEvent = e;
                    exContext = ctx;
                    return Task.CompletedTask;
                }))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.AreEqual("hello", stringEvent);
                Assert.AreSame(expectedContext, stringContext);
                Assert.IsNull(exEvent);
                Assert.IsNull(exContext);
                Assert.IsFalse(result.HasErrors());
                Assert.AreEqual(12, result.Value);
            }
        }
    }
}
