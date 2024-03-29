﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingModelRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging model registry test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model.Tests.Runtime.ModelRegistries
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Model.Elements;
    using Kephas.Messaging.Model.Runtime.ModelRegistries;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    using IMessage = Kephas.Messaging.IMessage;

    [TestFixture]
    public class MessagingModelRegistryTest : TestBase
    {
        [Test]
        public async Task GetRuntimeElementsAsync()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssemblies()
                .Returns(new[] { this.GetType().Assembly });

            var typeLoader = Substitute.For<ITypeLoader>();
            typeLoader.GetExportedTypes(Arg.Any<Assembly>()).Returns(new[] { typeof(IMessage), typeof(IMessageType), typeof(MessageType), typeof(string), typeof(PingMessage) });

            var contextFactory = this.CreateInjectableFactoryMock(() =>
                new ModelRegistryConventions(Substitute.For<IServiceProvider>()));

            var registry = new MessagingModelRegistry(contextFactory, appRuntime, typeLoader);
            var result = (await registry.GetRuntimeElementsAsync()).ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreSame(typeof(PingMessage), result[0]);
        }

        [Test]
        public async Task GetRuntimeElementsAsync_ExcludeFromModel()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssemblies()
                .Returns(new[] { this.GetType().Assembly });

            var typeLoader = Substitute.For<ITypeLoader>();
            typeLoader.GetExportedTypes(Arg.Any<Assembly>()).Returns(new[] { typeof(IMessage), typeof(IMessageType), typeof(MessageType), typeof(string), typeof(ExcludedMessage) });

            var contextFactory = this.CreateInjectableFactoryMock(() =>
                new ModelRegistryConventions(Substitute.For<IServiceProvider>()));

            var registry = new MessagingModelRegistry(contextFactory, appRuntime, typeLoader);
            var result = (await registry.GetRuntimeElementsAsync()).ToList();
            Assert.AreEqual(0, result.Count);
        }

        [ExcludeFromModel]
        public class ExcludedMessage : IMessage
        {
        }
    }
}