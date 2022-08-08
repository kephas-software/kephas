// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class AppServicesAttributeTest
    {
        [Test]
        public void Constructor_null_provider()
        {
            var attr = new AppServicesAttribute(typeof(NullAppServicesProvider));

            CollectionAssert.IsEmpty(attr.GetAppServiceContracts());
            CollectionAssert.IsEmpty(attr.GetAppServices());
        }

        [Test]
        public void Constructor_bad_provider()
        {
            Assert.Throws<ArgumentException>(() => new AppServicesAttribute(typeof(BadAppServicesProvider)));
        }

        [Test]
        public void Constructor_infos_provider()
        {
            var attr = new AppServicesAttribute(typeof(AppServicesProvider));

            var serviceInfos = attr.GetAppServiceContracts();
            CollectionAssert.AreEqual(new[] { typeof(AppServicesProvider.IService) }, serviceInfos.Select(i => i.ContractDeclarationType).ToArray());
            CollectionAssert.IsEmpty(attr.GetAppServices());
        }

        private class NullAppServicesProvider : IAppServiceInfosProvider
        {
        }

        private class BadAppServicesProvider
        {
        }

        private class AppServicesProvider : IAppServiceInfosProvider
        {
            /// <summary>
            /// Gets the contract declaration types.
            /// </summary>
            /// <param name="context">Optional. The context in which the service types are requested.</param>
            /// <returns>
            /// The contract declaration types.
            /// </returns>
            IEnumerable<ContractInfo>? IAppServiceInfosProvider.GetContractDeclarationTypes(IContext? context)
            {
                yield return new ContractInfo(typeof(IService), null);
            }

            [AppServiceContract]
            public interface IService
            {
            }
        }
    }
}