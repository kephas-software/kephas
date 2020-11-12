// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppSettingsProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests.Configuration
{
    using Kephas.Application.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultAppSettingsProviderTest : ApplicationTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            var provider = container.GetExport<IAppSettingsProvider>();

            Assert.IsInstanceOf<DefaultAppSettingsProvider>(provider);
        }
    }
}