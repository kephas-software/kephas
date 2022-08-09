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

            var serviceProviderType = appServicesAttrs[0].ProviderType;
            Assert.AreEqual("Kephas.Injection.Generated.AppServices_Kephas_Analyzers_TestAssembly", serviceProviderType.FullName);
        }

        [Test]
        public void Execute_generated_AppServiceTypeProvider_class()
        {
            var testAssembly = typeof(NoService).Assembly;

            var appServicesAttr = testAssembly.GetCustomAttributes<AppServicesAttribute>().Single();

            var provider = Activator.CreateInstance(appServicesAttr.ProviderType)!;
            var typesProvider = (IAppServiceInfosProvider)provider;
            var services = typesProvider.GetAppServices();

            var expectedServices = new (Type serviceType, Type contractDeclarationType)[]
            {
                (typeof(StringService), typeof(IGenericContract<string>)),
                (typeof(IntService), typeof(IGenericContract<int>)),
                (typeof(NonGenericService), typeof(IGenericContractDeclaration<,>)),
                (typeof(OpenGenericService<>), typeof(IOpenGenericContract<>)),
                (typeof(ServiceAndContract), typeof(ServiceAndContract)),
                (typeof(DerivedService), typeof(ServiceBase)),
                (typeof(SingletonService), typeof(ISingletonServiceContract)),
                (typeof(FileScopedNsSingletonService), typeof(IFileScopedNsSingletonServiceContract)),
            };

            Assert.IsTrue(
                expectedServices.All(
                    service => services.Any(
                        s => s.ServiceType == service.serviceType && s.ContractDeclarationType == service.contractDeclarationType)));

            var contractsProvider = (IAppServiceInfosProvider)provider;
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new ContractDeclarationInfo(typeof(ISingletonServiceContract), null),
                    new ContractDeclarationInfo(typeof(IFileScopedNsSingletonServiceContract), null),
                    new ContractDeclarationInfo(typeof(ServiceAndContract), null),
                    new ContractDeclarationInfo(typeof(ServiceBase), null),
                    new ContractDeclarationInfo(typeof(IOpenGenericContract<>), null),
                    new ContractDeclarationInfo(typeof(IGenericContract<>), null),
                    new ContractDeclarationInfo(typeof(IGenericContractDeclaration<,>), null),
                    new ContractDeclarationInfo(typeof(IMetaService), typeof(MetaServiceMetadata)),
                },
                contractsProvider.GetContractDeclarationTypes());
        }
    }
}
