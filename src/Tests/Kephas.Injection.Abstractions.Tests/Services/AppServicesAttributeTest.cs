// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Services
{
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

        private class NullAppServicesProvider
        {
        }

        private class AppServicesProvider : IAppServiceInfoProvider
        {
        }
    }
}