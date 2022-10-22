// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service contract attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Services
{
    using Kephas.Services;
    using NUnit.Framework;

    /// <summary>
    /// An application service contract attribute test.
    /// </summary>
    [TestFixture]
    public class AppServiceContractAttributeTest
    {
        [Test]
        public void Constructor()
        {
            var attr = new AppServiceContractAttribute();
            Assert.AreEqual(AppServiceLifetime.Transient,  attr.Lifetime);
        }
    }
}