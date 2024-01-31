// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSourceGeneratorIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using FluentAssertions;
using Kephas.Analyzers.TestAssembly;
using Kephas.Services;
using Xunit;

namespace Kephas.Analyzers.Tests.Services
{
        public class AppServicesSourceGeneratorIntegrationTest
    {
        [Fact]
        public void Execute_generated_AppServicesAttribute()
        {
            var testAssembly = typeof(NoService).Assembly;

            var appServicesAttrs = testAssembly.GetCustomAttributes<Attribute>().OfType<IAppServiceInfoProvider>().ToList();
            Assert.Equal(1, appServicesAttrs.Count);

            var serviceProviderType = appServicesAttrs[0].GetType().GenericTypeArguments[0];
            Assert.Equal("Kephas.Services.Generated.AppServices_Kephas_Analyzers_TestAssembly", serviceProviderType.FullName);
        }

        [Fact]
        public void Execute_generated_AppServiceTypeProvider_class()
        {
            var testAssembly = typeof(NoService).Assembly;

            var appServicesAttr = testAssembly.GetCustomAttributes<Attribute>().OfType<IAppServiceInfoProvider>().Single();

            var serviceProviderType = appServicesAttr.GetType().GenericTypeArguments[0];
            var provider = Activator.CreateInstance(serviceProviderType)!;
            var typesProvider = (IAppServiceInfoProvider)provider;
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

            Assert.True(
                expectedServices.All(
                    service => services.Any(
                        s => s.ServiceType == service.serviceType && s.ContractDeclarationType == service.contractDeclarationType)));

            var contractsProvider = (IAppServiceInfoProvider)provider;
            contractsProvider.GetContractDeclarationTypes()
                .Should()
                .BeEquivalentTo(
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
                    });
        }
    }
}
