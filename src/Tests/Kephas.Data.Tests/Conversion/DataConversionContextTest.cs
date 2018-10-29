// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data conversion context test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Conversion
{
    using Kephas.Data.Conversion;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataConversionContextTest
    {
        [Test]
        public void Constructor()
        {
            var conversionSvc = Substitute.For<IDataConversionService>();
            var dataSpace = Substitute.For<IDataSpace>();
            var context = new DataConversionContext(conversionSvc, dataSpace);

            Assert.AreSame(conversionSvc, context.DataConversionService);
            Assert.AreSame(dataSpace, context.DataSpace);
            Assert.AreSame(dataSpace.Identity, context.Identity);
            Assert.IsTrue(context.ThrowOnError);
        }
    }
}