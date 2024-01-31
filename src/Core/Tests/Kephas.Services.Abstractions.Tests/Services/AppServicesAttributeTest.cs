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
    using Xunit;

        public class AppServicesAttributeTest
    {
        [Fact]
        public void Constructor_null_provider()
        {
            var attr = new AppServicesAttribute<NullAppServicesProvider>();

            Assert.Empty(attr.GetAppServiceContracts());
            Assert.Empty(attr.GetAppServices());
        }

        [Fact]
        public void Constructor_infos_provider()
        {
            var attr = new AppServicesAttribute<AppServicesProvider>();

            var serviceInfos = attr.GetAppServiceContracts();
            Assert.Equal(new[] { typeof(AppServicesProvider.IService) }, serviceInfos.Select(i => i.ContractDeclarationType).ToArray());
            Assert.Empty(attr.GetAppServices());
        }

        private class NullAppServicesProvider : IAppServiceInfoProvider
        {
        }

        private class AppServicesProvider : IAppServiceInfoProvider
        {
            /// <summary>
            /// Gets the contract declaration types.
            /// </summary>
            /// <returns>
            /// The contract declaration types.
            /// </returns>
            IEnumerable<ContractDeclarationInfo>? IAppServiceInfoProvider.GetContractDeclarationTypes()
            {
                yield return new ContractDeclarationInfo(typeof(IService), null);
            }

            [AppServiceContract]
            public interface IService
            {
            }
        }
    }
}
