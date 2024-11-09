// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSpaceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data space test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class DataSpaceTest : DataSpaceTestBase
    {
        protected override IServiceProvider BuildServiceProvider()
        {
            return this.CreateServicesBuilder().BuildWithDependencyInjection();
        }
    }
}