// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServicesHandlerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Core.Endpoints.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Kephas.Data;
    using Kephas.Messaging;
    using Kephas.Messaging.Endpoints;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Testing.Composition;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class GetServicesHandlerTest : CompositionTestBase
    {
        [Test]
        public async Task ProcessAsync_missing_contracttype()
        {
            var container = this.CreateContainer();
            var handler = new GetServicesHandler(container.GetExport<ITypeResolver>(), container);

            Assert.ThrowsAsync<ArgumentException>(() => handler.ProcessAsync(
                new GetServicesMessage(),
                Substitute.For<IMessagingContext>(),
                default));
        }

        [Test]
        public async Task ProcessAsync_single()
        {
            var container = this.CreateContainer();
            var handler = new GetServicesHandler(container.GetExport<ITypeResolver>(), container);

            var result = await handler.ProcessAsync(
                new GetServicesMessage { ContractType = typeof(IIdGenerator).Name },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.AreEqual(1, result.Services.Length);
            Assert.AreEqual(typeof(DefaultIdGenerator), result.Services[0].ServiceInstanceType);
        }

        [Test]
        public async Task ProcessAsync_multiple()
        {
            var container = this.CreateContainer(
                assemblies: new Assembly[] { typeof(IMessageProcessor).Assembly, typeof(GetServicesMessage).Assembly },
                parts: new Type[] { typeof(TestGetServicesHandler) });
            var handler = new GetServicesHandler(container.GetExport<ITypeResolver>(), container);

            var result = await handler.ProcessAsync(
                new GetServicesMessage { ContractType = typeof(IMessageHandler).Name },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.Greater(result.Services.Length, 1);
            Assert.IsTrue(result.Services.Any(s => s.ServiceInstanceType == typeof(PingMessageHandler)));
            Assert.IsTrue(result.Services.Any(s => s.ServiceInstanceType == typeof(TestGetServicesHandler)));
        }


        [Test]
        public async Task ProcessAsync_multiple_with_override()
        {
            var container = this.CreateContainer(
                assemblies: new Assembly[] { typeof(IMessageProcessor).Assembly, typeof(GetServicesMessage).Assembly },
                parts: new Type[] { typeof(TestGetServicesHandler) });
            var handler = new GetServicesHandler(container.GetExport<ITypeResolver>(), container);

            var result = await handler.ProcessAsync(
                new GetServicesMessage { ContractType = typeof(IMessageHandler).Name, Ordered = true },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.Greater(result.Services.Length, 1);
            Assert.IsTrue(result.Services.Any(s => s.ServiceInstanceType == typeof(PingMessageHandler)));
            Assert.IsFalse(result.Services.Any(s => s.ServiceInstanceType == typeof(GetServicesHandler)));
            Assert.IsTrue(result.Services.Any(s => s.ServiceInstanceType == typeof(TestGetServicesHandler)));
        }

        [Override]
        public class TestGetServicesHandler : GetServicesHandler
        {
            public TestGetServicesHandler(ITypeResolver typeResolver, IInjector injector)
                : base(typeResolver, injector)
            {
            }
        }
    }
}