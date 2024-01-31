// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service contract attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Tests.Services
{
    using Kephas.Services;
    using Xunit;

    /// <summary>
    /// An application service contract attribute test.
    /// </summary>
        public class AppServiceContractAttributeTest
    {
        [Fact]
        public void Constructor()
        {
            var attr = new AppServiceContractAttribute();
            Assert.Equal(AppServiceLifetime.Transient,  attr.Lifetime);
        }
    }
}
