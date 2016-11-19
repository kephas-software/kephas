namespace Kephas.Data.Tests.Conversion
{
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Composition;
    using Kephas.Data.Conversion;
    using Kephas.Data.Conversion.Composition;
    using Kephas.Services;
    using Kephas.Testing.Core.Composition;
    using NUnit.Framework;
    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    [TestFixture]
    public class DefaultDataConversionServiceTest
    {
        [Test]
        public async Task ConvertAsync_one_converter_success()
        {
            var converter = Mock.Create<IDataConverter>();
            converter.Arrange(c => c.ConvertAsync(Arg.AnyObject, Arg.AnyObject, Arg.IsAny<IDataConversionContext>(), Arg.IsAny<CancellationToken>()))
                .TaskResult(new DataConversionResult {Target = 5});

            var service =
                new DefaultDataConversionService(new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter,
                        new DataConverterMetadata(typeof(int), typeof(int)))
                });

            var result = await service.ConvertAsync<int, int>(1, 2, new DataConversionContext(), CancellationToken.None);
            Assert.AreEqual(5, result.Target);
        }

        [Test]
        public async Task ConvertAsync_two_converters_success_last_overrides_target()
        {
            var converter1 = Mock.Create<IDataConverter>();
            converter1.Arrange(c => c.ConvertAsync(Arg.AnyObject, Arg.AnyObject, Arg.IsAny<IDataConversionContext>(), Arg.IsAny<CancellationToken>()))
                .TaskResult(new DataConversionResult { Target = 5 });

            var converter2 = Mock.Create<IDataConverter>();
            converter2.Arrange(c => c.ConvertAsync(Arg.AnyObject, Arg.AnyObject, Arg.IsAny<IDataConversionContext>(), Arg.IsAny<CancellationToken>()))
                .TaskResult(new DataConversionResult { Target = 6 });

            var service =
                new DefaultDataConversionService(new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter1,
                        new DataConverterMetadata(typeof(int), typeof(int))),
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter2,
                        new DataConverterMetadata(typeof(int), typeof(int)))
                });

            var result = await service.ConvertAsync<int, int>(1, 2, new DataConversionContext(), CancellationToken.None);
            Assert.AreEqual(6, result.Target);
        }

        [Test]
        public async Task ConvertAsync_two_converters_success_respects_processing_priority()
        {
            var converter1 = Mock.Create<IDataConverter>();
            converter1.Arrange(c => c.ConvertAsync(Arg.AnyObject, Arg.AnyObject, Arg.IsAny<IDataConversionContext>(), Arg.IsAny<CancellationToken>()))
                .TaskResult(new DataConversionResult { Target = 5 });

            var converter2 = Mock.Create<IDataConverter>();
            converter2.Arrange(c => c.ConvertAsync(Arg.AnyObject, Arg.AnyObject, Arg.IsAny<IDataConversionContext>(), Arg.IsAny<CancellationToken>()))
                .TaskResult(new DataConversionResult { Target = 6 });

            var service =
                new DefaultDataConversionService(new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter1,
                        new DataConverterMetadata(typeof(int), typeof(int), processingPriority:  (int)Priority.Low)),
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter2,
                        new DataConverterMetadata(typeof(int), typeof(int), processingPriority: (int)Priority.High))
                });

            var result = await service.ConvertAsync<int, int>(1, 2, new DataConversionContext(), CancellationToken.None);
            Assert.AreEqual(5, result.Target);
        }
    }
}