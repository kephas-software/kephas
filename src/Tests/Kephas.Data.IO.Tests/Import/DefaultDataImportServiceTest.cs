// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataImportServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data import service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.Import
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Model.Services;
    using Kephas.Services;
    using Kephas.Services.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultDataImportServiceTest
    {
        [Test]
        public async Task ImportDataAsync()
        {
            var reader = this.CreateDataStreamReadService();
            var conversionService = Substitute.For<IDataConversionService>();
            var resolver = this.CreateProjectedTypeResolver();

            var changedTargetEntities = new List<IEntityInfo>();

            var sourceDataContext = this.CreateSourceDataContext();
            var targetDataContext = this.CreateTargetDataContext(ei => changedTargetEntities.Add(ei));
            var dataSpace = this.CreateDataSpace<string, StringBuilder>(sourceDataContext, targetDataContext);

            var service = new DefaultDataImportService(reader, conversionService, resolver);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                reader.ReadAsync(dataStream, Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult((object)"hello"));

                conversionService.ConvertAsync("hello", Arg.Any<StringBuilder>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<IDataConversionResult>(DataConversionResult.FromTarget(new StringBuilder("kitty"))));

                var context = new DataImportContext(dataSpace);
                var result = await service.ImportDataAsync(dataStream, context);

                Assert.AreEqual(DataIOOperationState.CompletedSuccessfully, result.OperationState);
                Assert.AreEqual(0, result.Exceptions.Count);
                Assert.AreEqual(1, result.Messages.Count);
            }

            Assert.AreEqual(1, changedTargetEntities.Count);
            Assert.AreEqual("kitty", changedTargetEntities[0].Entity.ToString());
            Assert.AreEqual(ChangeState.AddedOrChanged, changedTargetEntities[0].ChangeState);
        }

        [Test]
        public async Task ImportDataAsync_config()
        {
            var reader = this.CreateDataStreamReadService();
            var conversionService = Substitute.For<IDataConversionService>();
            var resolver = this.CreateProjectedTypeResolver();

            var changedTargetEntities = new List<IEntityInfo>();

            var sourceDataContext = this.CreateSourceDataContext();
            var targetDataContext = this.CreateTargetDataContext(ei => changedTargetEntities.Add(ei));
            var dataSpace = this.CreateDataSpace<string, StringBuilder>(sourceDataContext, targetDataContext);

            var service = new DefaultDataImportService(reader, conversionService, resolver);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                reader.ReadAsync(dataStream, Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult((object)"hello"));

                conversionService.ConvertAsync("hello", Arg.Any<string>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                    .Returns(ci => Task.FromResult<IDataConversionResult>(DataConversionResult.FromTarget((string)ci.Arg<IDataConversionContext>()["entity"] == "hello" ? new StringBuilder("mimi") : new StringBuilder("kitty"))));

                var context = new DataImportContext(dataSpace)
                                  {
                                      DataConversionContextConfig = (e, ctx) => ctx["entity"] = e
                                  };
                var result = await service.ImportDataAsync(dataStream, context);

                Assert.AreEqual(DataIOOperationState.CompletedSuccessfully, result.OperationState);
                Assert.AreEqual(0, result.Exceptions.Count);
                Assert.AreEqual(1, result.Messages.Count);
            }

            Assert.AreEqual(1, changedTargetEntities.Count);
            Assert.AreEqual("mimi", changedTargetEntities[0].Entity.ToString());
            Assert.AreEqual(ChangeState.AddedOrChanged, changedTargetEntities[0].ChangeState);
        }

        [Test]
        public async Task ImportDataAsync_behaviors()
        {
            var reader = this.CreateDataStreamReadService();
            var conversionService = Substitute.For<IDataConversionService>();
            var resolver = this.CreateProjectedTypeResolver();

            var sourceDataContext = this.CreateSourceDataContext();
            var targetDataContext = this.CreateTargetDataContext();
            var dataSpace = this.CreateDataSpace<string, StringBuilder>(sourceDataContext, targetDataContext);

            var sb = new StringBuilder();
            var b1 = this.CreateBehaviorFactory(sb, "b1", Priority.Low);
            var b2 = this.CreateBehaviorFactory(sb, "b2", Priority.High);

            var service = new DefaultDataImportService(reader, conversionService, resolver, new List<IExportFactory<IDataImportBehavior, AppServiceMetadata>> { b1, b2});
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                reader.ReadAsync(dataStream, Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult((object)"hello"));

                conversionService.ConvertAsync("hello", Arg.Any<string>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<IDataConversionResult>(DataConversionResult.FromTarget(new StringBuilder("kitty"))));

                var context = new DataImportContext(dataSpace);
                var result = await service.ImportDataAsync(dataStream, context);
            }

            var flowResult = sb.ToString();
            Assert.AreEqual("before-read(b2),before-read(b1),after-read(b1),after-read(b2),convert(b2),convert(b1),before-persist(b2),before-persist(b1),after-persist(b1),after-persist(b2),", flowResult);
        }

        private IExportFactory<IDataImportBehavior, AppServiceMetadata> CreateBehaviorFactory(StringBuilder sb = null, string name = "b", Priority processingPriority = Priority.Normal)
        {
            var b = Substitute.For<IDataImportBehavior>();

            b.BeforeReadDataSourceAsync(
                Arg.Any<DataStream>(),
                Arg.Any<IDataImportContext>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        sb?.Append($"before-read({name}),");
                        return Task.FromResult(0);
                    });

            b.AfterReadDataSourceAsync(
                Arg.Any<DataStream>(),
                Arg.Any<IDataImportContext>(),
                Arg.Any<IList<object>>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        sb?.Append($"after-read({name}),");
                        return Task.FromResult(0);
                    });

            b.BeforeConvertEntityAsync(
                Arg.Any<IEntityInfo>(),
                Arg.Any<IDataImportContext>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        sb?.Append($"convert({name}),");
                        return Task.FromResult(0);
                    });

            b.BeforePersistEntityAsync(
                Arg.Any<IEntityInfo>(),
                Arg.Any<IEntityInfo>(),
                Arg.Any<IDataImportContext>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        sb?.Append($"before-persist({name}),");
                        return Task.FromResult(0);
                    });


            b.AfterPersistEntityAsync(
                Arg.Any<IEntityInfo>(),
                Arg.Any<IEntityInfo>(),
                Arg.Any<IDataImportContext>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        sb?.Append($"after-persist({name}),");
                        return Task.FromResult(0);
                    });

            return new ExportFactory<IDataImportBehavior, AppServiceMetadata>(() => b, new AppServiceMetadata(processingPriority: (int)processingPriority));
        }

        private IProjectedTypeResolver CreateProjectedTypeResolver()
        {
            var resolver = Substitute.For<IProjectedTypeResolver>();
            resolver.ResolveProjectedType(Arg.Any<Type>(), Arg.Any<IDataImportContext>(), Arg.Any<bool>())
                .Returns(ci => ci.Arg<Type>());

            return resolver;
        }

        private IDataStreamReadService CreateDataStreamReadService()
        {
            var reader = Substitute.For<IDataStreamReadService>();
            return reader;
        }

        private IDataSpace CreateDataSpace<TSource, TTarget>(
            IDataContext sourceDataContext,
            IDataContext targetDataContext)
        {
            var dataSpace = Substitute.For<IDataSpace>();
            dataSpace[typeof(TSource), Arg.Any<IContext>()].Returns(sourceDataContext);
            dataSpace[typeof(TTarget), Arg.Any<IContext>()].Returns(targetDataContext);
            return dataSpace;
        }

        private IDataContext CreateSourceDataContext()
        {
            var sourceDataContext = Substitute.For<IDataContext>();
            sourceDataContext.AttachEntity(Arg.Any<object>())
                .Returns(ci => new EntityInfo(ci.Arg<object>()));

            return sourceDataContext;
        }

        private IDataContext CreateTargetDataContext(Action<IEntityInfo> attachEntityCallback = null)
        {
            var targetDataContext = Substitute.For<IDataContext>();
            var persistChangesCommand = Substitute.For<IPersistChangesCommand>();
            targetDataContext.CreateCommand(typeof(IPersistChangesCommand))
                .Returns(persistChangesCommand);

            targetDataContext.AttachEntity(Arg.Any<object>())
                .Returns(
                    ci =>
                        {
                            var ei = new EntityInfo(ci.Arg<object>());
                            attachEntityCallback?.Invoke(ei);
                            return ei;
                        });

            return targetDataContext;
        }
    }
}