// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataWithSystemCompositionInjectionSpaceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data space test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Injection.SystemComposition
{
    using NUnit.Framework;

    [TestFixture]
    public class DataWithSystemCompositionInjectionSpaceTest : DataWithSystemCompositionInjectionTestBase
    {
        [Test]
        public void Injection_success()
        {
            var container = CreateInjector();
            var dataSpace = container.Resolve<IDataSpace>();

            Assert.IsInstanceOf<DataSpace>(dataSpace);
        }
    }
}