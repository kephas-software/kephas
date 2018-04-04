// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicAppConfigurationTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic application configuration test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application.Configuration
{
    using Kephas.Application.Configuration;
    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class DynamicAppConfigurationTest
    {
        [Test]
        public void Indexer()
        {
            var appConfiguration = new DynamicAppConfiguration();
            var section = appConfiguration["dummy"];
            Assert.IsInstanceOf<Expando>(section);

            var sameSection = appConfiguration["dummy"];
            Assert.AreSame(section, sameSection);
        }
    }
}