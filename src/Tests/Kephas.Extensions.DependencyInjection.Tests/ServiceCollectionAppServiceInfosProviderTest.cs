// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionAppServiceInfosProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Services;
    using Microsoft.Extensions.DependencyInjection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceCollectionAppServiceInfosProviderTest
    {
        [Test]
        public void GetAppServiceInfos_empty_context()
        {
            var provider = new ServiceCollectionAppServiceInfosProvider();
            var serviceInfos = provider.GetAppServiceContracts();

            CollectionAssert.IsEmpty(serviceInfos);
        }

        [Test]
        public void GetAppServiceInfos_non_generic_service()
        {
            var provider = new ServiceCollectionAppServiceInfosProvider();
            var context = this.GetContext(services =>
            {
                services.AddSingleton<string>("hello");
            });
            var serviceInfos = provider.GetAppServiceContracts().ToList();

            Assert.AreEqual(1, serviceInfos.Count);
            Assert.AreEqual(typeof(string), serviceInfos[0].ContractDeclarationType);
            if (serviceInfos[0] is not
                {
                    AppServiceInfo:
                    {
                        InstancingStrategy: "hello",
                        AllowMultiple: true,
                        Lifetime: AppServiceLifetime.Singleton,
                        AsOpenGeneric: false,
                        Metadata: null
                    }
                })
            {
                Assert.Fail($"The {serviceInfos[0].ContractDeclarationType} service is not properly registered");
            }
        }

        [Test]
        public void GetAppServiceInfos_open_generic_service()
        {
            var provider = new ServiceCollectionAppServiceInfosProvider();
            var context = this.GetContext(services =>
            {
                services.AddSingleton(typeof(ICollection<>), typeof(List<>));
            });
            var serviceInfos = provider.GetAppServiceContracts().ToList();

            Assert.AreEqual(1, serviceInfos.Count);
            Assert.AreEqual(typeof(ICollection<>), serviceInfos[0].ContractDeclarationType);
            Assert.AreEqual(typeof(List<>), serviceInfos[0].AppServiceInfo.InstancingStrategy);
            if (serviceInfos[0] is not
                {
                    AppServiceInfo:
                    {
                        AllowMultiple: true,
                        Lifetime: AppServiceLifetime.Singleton,
                        AsOpenGeneric: true,
                        Metadata: null
                    }
                })
            {
                Assert.Fail($"The {serviceInfos[0].ContractDeclarationType} service is not properly registered");
            }
        }

        private IContext GetContext(Action<IServiceCollection> configure)
        {
            var context = Substitute.For<IContext>();
            var ambientServices = new AmbientServices();
            context.AmbientServices.Returns(ambientServices);
            var serviceCollection = new ServiceCollection();
            ambientServices.Add<IServiceCollection>(serviceCollection);
            configure(serviceCollection);
            return context;
        }
    }
}
