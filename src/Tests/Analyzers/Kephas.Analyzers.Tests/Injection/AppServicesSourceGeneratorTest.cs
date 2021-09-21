// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers.Tests.Injection
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Analyzers.TestAssembly;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class AppServicesSourceGeneratorTest
    {
        [Test]
        public void Execute_generated_AppServicesAttribute()
        {
            var testAssembly = typeof(NoService).Assembly;

            var appServicesAttrs = testAssembly.GetCustomAttributes<AppServicesAttribute>().ToList();
            Assert.AreEqual(1, appServicesAttrs.Count);

            var appServicesAttr = appServicesAttrs[0];
            CollectionAssert.AreEquivalent(
                new[]
                {
                    typeof(ISingletonServiceContract),
                    typeof(ServiceAndContract),
                    typeof(ServiceBase),
                    typeof(IOpenGenericContract<>),
                    typeof(IGenericContract<>),
                    typeof(IGenericContractDeclaration<>),
                },
                appServicesAttr.ContractDeclarationTypes);

            Assert.AreEqual(1, appServicesAttr.ServiceProviderTypes.Length);
            Assert.IsTrue(typeof(IAppServiceTypesProvider).IsAssignableFrom(appServicesAttr.ServiceProviderTypes[0]));
        }

        [Test]
        public void Execute_generated_AppServiceTypeProvider_class()
        {
            var testAssembly = typeof(NoService).Assembly;

            var appServicesAttr = testAssembly.GetCustomAttributes<AppServicesAttribute>().Single();
            var serviceProviderType = appServicesAttr.ServiceProviderTypes.Single();

            var serviceProvider = (IAppServiceTypesProvider)Activator.CreateInstance(serviceProviderType)!;
            var services = serviceProvider.GetAppServiceTypes();

            var expectedServices = new (Type serviceType, Type contractDeclarationType)[]
            {
                (typeof(StringService), typeof(IGenericContract<>)),
                (typeof(IntService), typeof(IGenericContract<>)),
                (typeof(GenericService<>), typeof(IGenericContractDeclaration<>)),
                (typeof(OpenGenericService<>), typeof(IOpenGenericContract<>)),
                (typeof(ServiceAndContract), typeof(ServiceAndContract)),
                (typeof(DerivedService), typeof(ServiceBase)),
                (typeof(SingletonService), typeof(ISingletonServiceContract)),
            };

            Assert.IsTrue(
                expectedServices.All(
                    service => services.Any(
                        s => s.serviceType == service.serviceType && s.contractDeclarationType == service.contractDeclarationType)));
        }
    }
}
