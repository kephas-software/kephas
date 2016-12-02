using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using System.Threading.Tasks;
using Kephas.Data.Conversion;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Kephas.Data.Tests.Conversion
{
    [TestFixture]
    public class DataConversionServiceExtensionsTest
    {
        [Test]
        public async Task ConvertAsync_success()
        {
            var conversionService = Mock.Create<IDataConversionService>();
            conversionService
                .Arrange(c => c.ConvertAsync<string, string>(Arg.AnyString, Arg.AnyString, Arg.IsAny<IDataConversionContext>(), Arg.IsAny<CancellationToken>()))
                .TaskResult(DataConversionResult.FromTarget("hello"));

            var result = await DataConversionServiceExtensions.ConvertAsync(conversionService, "sisi", "says", new DataConversionContext(conversionService), CancellationToken.None);
            Assert.AreEqual("hello", result.Target);
        }

        [Test]
        public async Task ConvertAsync_exception()
        {
            var conversionService = Mock.Create<IDataConversionService>();
            conversionService
                .Arrange(c => c.ConvertAsync<string, string>(Arg.AnyString, Arg.AnyString, Arg.IsAny<IDataConversionContext>(), Arg.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();

            Assert.Throws<InvalidOperationException>(() => DataConversionServiceExtensions.ConvertAsync(conversionService, "sisi", "says", new DataConversionContext(conversionService), CancellationToken.None));
        }
    }
}