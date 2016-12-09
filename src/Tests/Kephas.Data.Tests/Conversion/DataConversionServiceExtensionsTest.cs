// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionServiceExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data conversion service extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Conversion
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Conversion;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using NUnit.Framework;

    [TestFixture]
    public class DataConversionServiceExtensionsTest
    {
        [Test]
        public async Task ConvertAsync_success()
        {
            var conversionService = Substitute.For<IDataConversionService>();
            conversionService.ConvertAsync<string, string>(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(DataConversionResult.FromTarget("hello"));

            var result = await DataConversionServiceExtensions.ConvertAsync(conversionService, "sisi", "says", new DataConversionContext(conversionService), CancellationToken.None);
            Assert.AreEqual("hello", result.Target);
        }

        [Test]
        public async Task ConvertAsync_exception()
        {
            var conversionService = Substitute.For<IDataConversionService>();
            conversionService.ConvertAsync<string, string>(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Throws<InvalidOperationException>();

            Assert.Throws<InvalidOperationException>(() => DataConversionServiceExtensions.ConvertAsync(conversionService, "sisi", "says", new DataConversionContext(conversionService), CancellationToken.None));
        }
    }
}