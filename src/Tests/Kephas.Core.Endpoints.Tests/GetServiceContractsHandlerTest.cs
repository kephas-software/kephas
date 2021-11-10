// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServiceContractsHandlerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Services;
    using Kephas.Testing.Injection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class GetServiceContractsHandlerTest : InjectionTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(ILogger<>).Assembly,         // Kephas.Logging
            };
        }

        [Test]
        public async Task ProcessAsync_all()
        {
            var container = this.CreateInjector();
            var handler = new GetServiceContractsHandler(container.Resolve<IAmbientServices>());

            var result = await handler.ProcessAsync(
                new GetServiceContractsMessage(),
                Substitute.For<IMessagingContext>(),
                default);

            var svcInfo = result.ServiceInfos.Single(si => si.ContractType == typeof(ILogManager));
            Assert.AreEqual(AppServiceLifetime.Singleton, svcInfo.Lifetime);
            Assert.IsFalse(svcInfo.AsOpenGeneric);

            svcInfo = result.ServiceInfos.Single(si => si.ContractType == typeof(ILogger<>));
            Assert.AreEqual(AppServiceLifetime.Singleton, svcInfo.Lifetime);
            Assert.IsTrue(svcInfo.AsOpenGeneric);
        }

        [Test]
        public async Task ProcessAsync_filter_asopengeneric()
        {
            var container = this.CreateInjector();
            var handler = new GetServiceContractsHandler(container.Resolve<IAmbientServices>());

            var result = await handler.ProcessAsync(
                new GetServiceContractsMessage
                {
                    AsOpenGeneric = true,
                },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.IsTrue(result.ServiceInfos.All(si => si.AsOpenGeneric));
        }
    }
}