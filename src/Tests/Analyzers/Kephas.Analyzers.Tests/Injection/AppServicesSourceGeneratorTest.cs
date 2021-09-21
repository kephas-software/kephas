// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Analyzers.Tests.Injection
{
    using System.Linq;
    using System.Reflection;

    using Kephas.Analyzers.TestAssembly;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class AppServicesSourceGeneratorTest
    {
        [Test]
        public void Execute_generated_contracts_and_services()
        {
            var testAssembly = typeof(NoService).Assembly;

            var appServicesAttrs = testAssembly.GetCustomAttributes<AppServicesAttribute>().ToList();
            Assert.AreEqual(1, appServicesAttrs.Count);

            var appServicesAttr = appServicesAttrs[0];
            CollectionAssert.AreEquivalent(
                new[] { typeof(ISingletonServiceContract), typeof(ServiceAndContract), typeof(ServiceBase) },
                appServicesAttr.ContractDeclarationTypes);

            Assert.AreEqual(1, appServicesAttr.ServiceProviderTypes.Length);
            Assert.IsTrue(typeof(IAppServiceTypesProvider).IsAssignableFrom(appServicesAttr.ServiceProviderTypes[0]));
        }
    }
}
