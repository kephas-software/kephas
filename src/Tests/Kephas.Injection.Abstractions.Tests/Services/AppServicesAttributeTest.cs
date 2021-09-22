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

            CollectionAssert.IsEmpty(attr.GetAppServiceInfos());
            CollectionAssert.IsEmpty(attr.GetAppServiceTypes());
        }

        [Test]
        public void Constructor_infos_provider()
        {
            var attr = new AppServicesAttribute(typeof(AppServicesProvider));

            var serviceInfos = attr.GetAppServiceInfos();
            CollectionAssert.AreEqual(new[] { typeof(AppServicesProvider.IService) }, serviceInfos.Select(i => i.contractDeclarationType));
            CollectionAssert.IsEmpty(attr.GetAppServiceTypes());
        }

        private class NullAppServicesProvider
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
            public IEnumerable<Type>? GetContractDeclarationTypes(dynamic? context = null)
            {
                yield return typeof(IService);
            }

            [AppServiceContract]
            public interface IService
            {
            }
        }
    }
}