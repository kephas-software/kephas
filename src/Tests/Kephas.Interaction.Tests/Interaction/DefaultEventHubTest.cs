// --------------------------------------------------------------------------------------------------------------------
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

    using Kephas.ExceptionHandling;
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
            var hub = this.CreateEventHub();
            var stringEvent = string.Empty;
            IContext? stringContext = null;
            Exception? exEvent = null;
            IContext? exContext = null;
            using (hub.Subscribe(
                e => e is string,
                async (e, ctx, token) =>
                {
                    stringEvent = (string)e;
                    stringContext = ctx;
                    await Task.Delay(33, token);
                }))
            using (hub.Subscribe(
                e => e is Exception,
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
                Assert.AreEqual(OperationState.Completed, result.OperationState);
                Assert.GreaterOrEqual(result.Elapsed.TotalMilliseconds, 33);
                CollectionAssert.AreEqual(new List<object?> { null }, result.Value);
            }
        }

        [Test]
        public async Task Subscribe_matching_event_exception()
        {
            var hub = this.CreateEventHub();
            var stringEvent = string.Empty;
            IContext? stringContext = null;
            Exception? exEvent = null;
            IContext? exContext = null;
            using (hub.Subscribe(
                e => e is string,
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
                CollectionAssert.AreEqual(new List<object?>(), result.Value);
            }
        }

        [Test]
        public async Task Subscribe_matching_event_with_return_value()
        {
            var hub = this.CreateEventHub();
            using (hub.Subscribe(
                e => e is string,
                (e, ctx, token) => Task.FromResult(12)))
            using (hub.Subscribe(
                e => e.GetType() == typeof(Exception),
                (e, ctx, token) => Task.CompletedTask))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.IsFalse(result.HasErrors());
                CollectionAssert.AreEqual(new List<object?> { 12 }, result.Value);
            }
        }

        [Test]
        public async Task Subscribe_matching_event_with_return_value_interrupt_success()
        {
            var hub = this.CreateEventHub();
            using (hub.Subscribe(
                e => e is string,
                (e, ctx, token) => Task.FromException(new InterruptSignal(12))))
            using (hub.Subscribe(
                e => e is string,
                (e, ctx, token) => Task.FromResult(24)))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.IsFalse(result.HasErrors());
                CollectionAssert.AreEqual(new List<object?> { 12 }, result.Value);
            }
        }

        [Test]
        public async Task Subscribe_matching_event_with_return_value_interrupt_fail_error()
        {
            var hub = this.CreateEventHub();
            using (hub.Subscribe(
                e => e is string,
                (e, ctx, token) => Task.FromException(new InterruptSignal(severity: SeverityLevel.Error))))
            using (hub.Subscribe(
                e => e is string,
                (e, ctx, token) => Task.FromResult(24)))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.IsTrue(result.HasErrors());
                CollectionAssert.AllItemsAreInstancesOfType(result.Exceptions, typeof(InterruptSignal));
                CollectionAssert.IsEmpty(result.Value);
            }
        }

        [Test]
        public async Task Subscribe_matching_event_with_return_value_interrupt_fail_exception()
        {
            var hub = this.CreateEventHub();
            using (hub.Subscribe(
                e => e is string,
                (e, ctx, token) => Task.FromException(new InterruptSignal(new InvalidOperationException()))))
            using (hub.Subscribe(
                e => e is string,
                (e, ctx, token) => Task.FromResult(24)))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.IsTrue(result.HasErrors());
                CollectionAssert.AllItemsAreInstancesOfType(result.Exceptions, typeof(InvalidOperationException));
                CollectionAssert.IsEmpty(result.Value);
            }
        }

        [Test]
        public async Task Subscribe_matching_event_with_multiple_return_values()
        {
            var hub = this.CreateEventHub();
            using (hub.Subscribe(
                e => e is string,
                (e, ctx, token) => Task.FromResult(12)))
            using (hub.Subscribe(
                e => e is string,
                (e, ctx, token) => Task.FromResult(24)))
            {
                var expectedContext = Substitute.For<IContext>();
                var result = await hub.PublishAsync("hello", expectedContext);

                Assert.IsFalse(result.HasErrors());
                CollectionAssert.AreEqual(new List<object?> { 12, 24 }, result.Value);
            }
        }

        [Test]
        public async Task Subscribe_typed_matching_event()
        {
            var hub = this.CreateEventHub();
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
                CollectionAssert.AreEqual(new List<object?> { null }, result.Value);
            }
        }

        [Test]
        public async Task Subscribe_typed_matching_event_with_return_value()
        {
            var hub = this.CreateEventHub();
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
                CollectionAssert.AreEqual(new List<object?> { 12 }, result.Value);
            }
        }

        private DefaultEventHub CreateEventHub(IInjectableFactory? injectableFactory = null)
        {
            return new DefaultEventHub(injectableFactory ?? Substitute.For<IInjectableFactory>());
        }
    }
}
