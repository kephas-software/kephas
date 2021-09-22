// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DistributedMessagingCommandResolverTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Commands.Messaging
{
    using System;
    using System.Collections.Generic;

    using Kephas.Commands;
    using Kephas.Commands.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Tests.Orchestration;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DistributedMessagingCommandResolverTest : OrchestrationTestBase
    {
        [Test]
        public void Injection()
        {
            var container = this.CreateContainer();
            var resolver = container.Resolve<ICommandResolver>();

            Assert.IsInstanceOf<DistributedMessagingCommandResolver>(resolver);
        }

        [Test]
        public void Resolve()
        {
            var messageBroker = Substitute.For<IMessageBroker>();
            var resolver = new DistributedMessagingCommandResolver(new Lazy<IMessageBroker>(() => messageBroker), new List<Lazy<ICommandRegistry, AppServiceMetadata>>());
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
            var resolver = new DistributedMessagingCommandResolver(new Lazy<IMessageBroker>(() => messageBroker), new List<Lazy<ICommandRegistry, AppServiceMetadata>>());
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
            var resolver = new DistributedMessagingCommandResolver(new Lazy<IMessageBroker>(() => messageBroker), new List<Lazy<ICommandRegistry, AppServiceMetadata>>());
            var args = new Args { [RunAtOperationInfo.RunAtArg] = "webapp", [RunAtOperationInfo.OneWayArg] = string.Empty };
            var opInfo = resolver.ResolveCommand("help", args);
            Assert.IsInstanceOf<RunAtOperationInfo>(opInfo);
            Assert.AreEqual(new Endpoint(appInstanceId: "webapp"), ((RunAtOperationInfo)opInfo).Endpoint);
            Assert.IsTrue(((RunAtOperationInfo)opInfo).IsOneWay);
            Assert.IsNull(args[RunAtOperationInfo.RunAtArg]);
        }
    }
}