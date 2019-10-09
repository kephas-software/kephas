// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSpaceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data space test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Composition.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class DataSpaceTest : DataTestBase
    {
        [Test]
        public void Composition_success()
        {
            var container = this.CreateContainerForData();
            var dataSpace = container.GetExport<IDataSpace>();

            Assert.IsInstanceOf<DataSpace>(dataSpace);
        }
    }
}