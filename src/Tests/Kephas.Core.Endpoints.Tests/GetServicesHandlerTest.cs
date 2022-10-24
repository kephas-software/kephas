// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServicesHandlerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Endpoints;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Testing;
    using Kephas.Testing.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class GetServicesHandlerTest : EndpointsTestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(ILogger<>).Assembly,         // Kephas.Logging
                typeof(ITypeResolver).Assembly,     // Kephas.Reflection
            };
        }

        [Test]
        public async Task ProcessAsync_missing_contracttype()
        {
            var container = this.BuildServiceProvider();
            var handler = new GetServicesHandler(container.Resolve<ITypeResolver>(), container);

            Assert.ThrowsAsync<ArgumentException>(() => handler.ProcessAsync(
                new GetServicesMessage(),
                Substitute.For<IMessagingContext>(),
                default));
        }

        [Test]
        public async Task ProcessAsync_single()
        {
            var container = this.BuildServiceProvider();
            var handler = new GetServicesHandler(container.Resolve<ITypeResolver>(), container);

            var result = await handler.ProcessAsync(
                new GetServicesMessage { ContractType = typeof(IMessageProcessor).Name },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.AreEqual(1, result.Services.Length);
            Assert.AreEqual(typeof(DefaultMessageProcessor), result.Services[0].ServiceType);
        }

        [Test]
        public async Task ProcessAsync_multiple()
        {
            var container = this.BuildServiceProvider(
                assemblies: new[] { typeof(IMessageProcessor).Assembly, typeof(GetServicesHandler).Assembly, typeof(PingMessageHandler).Assembly },
                parts: new[] { typeof(TestGetServicesHandler) });
            var handler = new GetServicesHandler(container.Resolve<ITypeResolver>(), container);

            var result = await handler.ProcessAsync(
                new GetServicesMessage { ContractType = nameof(IMessageHandler) },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.Greater(result.Services.Length, 1);
            Assert.IsTrue(result.Services.Any(s => s.ServiceType == typeof(PingMessageHandler)));
            Assert.IsTrue(result.Services.Any(s => s.ServiceType == typeof(TestGetServicesHandler)));
        }


        [Test]
        public async Task ProcessAsync_multiple_with_override()
        {
            var container = this.BuildServiceProvider(
                assemblies: new Assembly[] { typeof(IMessageProcessor).Assembly, typeof(GetServicesMessage).Assembly },
                parts: new Type[] { typeof(TestGetServicesHandler) });
            var handler = new GetServicesHandler(container.Resolve<ITypeResolver>(), container);

            var result = await handler.ProcessAsync(
                new GetServicesMessage { ContractType = nameof(IMessageHandler), Ordered = true },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.Greater(result.Services.Length, 1);
            Assert.IsTrue(result.Services.Any(s => s.ServiceType == typeof(PingMessageHandler)));
            Assert.IsFalse(result.Services.Any(s => s.ServiceType == typeof(GetServicesHandler)));
            Assert.IsTrue(result.Services.Any(s => s.ServiceType == typeof(TestGetServicesHandler)));
        }

        [Override]
        public class TestGetServicesHandler : GetServicesHandler
        {
            public TestGetServicesHandler(ITypeResolver typeResolver, IServiceProvider serviceProvider)
                : base(typeResolver, serviceProvider)
            {
            }
        }
    }
}