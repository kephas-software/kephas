// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCommandResolverTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Tests
{
    using System;
    using System.Collections.Generic;

    using Kephas.Commands.Messaging.Reflection;
    using Kephas.Messaging.Distributed;
    using Kephas.Services.Composition;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessagingCommandResolverTest : CommandsTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            var resolver = container.GetExport<ICommandResolver>();

            Assert.IsInstanceOf<MessagingCommandResolver>(resolver);
        }

        [Test]
        public void Resolve()
        {
            var messageBroker = Substitute.For<IMessageBroker>();
            var resolver = new MessagingCommandResolver(new Lazy<IMessageBroker>(() => messageBroker), new List<Lazy<ICommandRegistry, AppServiceMetadata>>());
            var args = new Args { [RunAtOperationInfo.RunAtArg] = "webapp" };
            var opInfo = resolver.ResolveCommand("help", args);
            Assert.IsInstanceOf<RunAtOperationInfo>(opInfo);
            Assert.AreEqual(new Endpoint(appInstanceId: "webapp"), ((RunAtOperationInfo)opInfo).Endpoint);
            Assert.IsFalse(((RunAtOperationInfo)opInfo).IsOneWay);
            Assert.IsNull(args[RunAtOperationInfo.RunAtArg]);
        }

        [Test]
        public void Resolve_oneway_true()
        {
            var messageBroker = Substitute.For<IMessageBroker>();
            var resolver = new MessagingCommandResolver(new Lazy<IMessageBroker>(() => messageBroker), new List<Lazy<ICommandRegistry, AppServiceMetadata>>());
            var args = new Args { [RunAtOperationInfo.RunAtArg] = "webapp", [RunAtOperationInfo.OneWayArg] = "true" };
            var opInfo = resolver.ResolveCommand("help", args);
            Assert.IsInstanceOf<RunAtOperationInfo>(opInfo);
            Assert.AreEqual(new Endpoint(appInstanceId: "webapp"), ((RunAtOperationInfo)opInfo).Endpoint);
            Assert.IsTrue(((RunAtOperationInfo)opInfo).IsOneWay);
            Assert.IsNull(args[RunAtOperationInfo.RunAtArg]);
        }

        [Test]
        public void Resolve_oneway_empty()
        {
            var messageBroker = Substitute.For<IMessageBroker>();
            var resolver = new MessagingCommandResolver(new Lazy<IMessageBroker>(() => messageBroker), new List<Lazy<ICommandRegistry, AppServiceMetadata>>());
            var args = new Args { [RunAtOperationInfo.RunAtArg] = "webapp", [RunAtOperationInfo.OneWayArg] = string.Empty };
            var opInfo = resolver.ResolveCommand("help", args);
            Assert.IsInstanceOf<RunAtOperationInfo>(opInfo);
            Assert.AreEqual(new Endpoint(appInstanceId: "webapp"), ((RunAtOperationInfo)opInfo).Endpoint);
            Assert.IsTrue(((RunAtOperationInfo)opInfo).IsOneWay);
            Assert.IsNull(args[RunAtOperationInfo.RunAtArg]);
        }
    }
}