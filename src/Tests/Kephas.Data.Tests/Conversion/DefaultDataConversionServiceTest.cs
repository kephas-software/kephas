// --------------------------------------------------------------------------------------------------------------------
// <copyright file="defaultdataconversionservicetest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the defaultdataconversionservicetest class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Conversion
{
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Composition;
    using Kephas.Data.Conversion;
    using Kephas.Data.Conversion.Composition;
    using Kephas.Services;
    using Kephas.Testing.Core.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultDataConversionServiceTest
    {
        [Test]
        public async Task ConvertAsync_one_converter_success()
        {
            var converter = Substitute.For<IDataConverter>();
            converter.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult {Target = 5}));

            var service =
                new DefaultDataConversionService(Substitute.For<IAmbientServices>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter,
                        new DataConverterMetadata(typeof(int), typeof(int)))
                });

            var result = await service.ConvertAsync<int, int>(1, 2, new DataConversionContext(service), CancellationToken.None);
            Assert.AreEqual(5, result.Target);
        }

        [Test]
        public async Task ConvertAsync_two_converters_success_last_overrides_target()
        {
            var converter1 = Substitute.For<IDataConverter>();
            converter1.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 5 }));

            var converter2 = Substitute.For<IDataConverter>();
            converter2.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 6 }));

            var service =
                new DefaultDataConversionService(Substitute.For<IAmbientServices>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter1,
                        new DataConverterMetadata(typeof(int), typeof(int))),
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter2,
                        new DataConverterMetadata(typeof(int), typeof(int)))
                });

            var result = await service.ConvertAsync<int, int>(1, 2, new DataConversionContext(service), CancellationToken.None);
            Assert.AreEqual(6, result.Target);
        }

        [Test]
        public async Task ConvertAsync_two_converters_success_respects_processing_priority()
        {
            var converter1 = Substitute.For<IDataConverter>();
            converter1.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 5 }));

            var converter2 = Substitute.For<IDataConverter>();
            converter2.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 6 }));

            var service =
                new DefaultDataConversionService(Substitute.For<IAmbientServices>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter1,
                        new DataConverterMetadata(typeof(int), typeof(int), processingPriority:  (int)Priority.Low)),
                    new TestExportFactory<IDataConverter, DataConverterMetadata>(() => converter2,
                        new DataConverterMetadata(typeof(int), typeof(int), processingPriority: (int)Priority.High))
                });

            var result = await service.ConvertAsync<int, int>(1, 2, new DataConversionContext(service), CancellationToken.None);
            Assert.AreEqual(5, result.Target);
        }
    }
}