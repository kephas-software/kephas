// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataImportServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data import service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Data.IO.Tests.Import
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Model;
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultDataImportServiceTest : TestBase
    {
        [Test]
        public async Task ImportDataAsync()
        {
            var reader = this.CreateDataStreamReadService();
            var conversionService = Substitute.For<IDataConversionService>();
            var resolver = this.CreateProjectedTypeResolver();
            var ctxFactory = this.CreateInjectableFactoryMock<DataImportContext>(() => new DataImportContext(Substitute.For<IServiceProvider>()));

            var changedTargetEntities = new List<IEntityEntry>();

            var sourceDataContext = this.CreateSourceDataContext();
            var targetDataContext = this.CreateTargetDataContext(ei => changedTargetEntities.Add(ei));
            var dataSpace = this.CreateDataSpace<string, StringBuilder>(sourceDataContext, targetDataContext);

            var service = new DefaultDataImportService(ctxFactory, reader, conversionService, resolver);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                reader.ReadAsync(dataStream, Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult((object)"hello"));

                conversionService.ConvertAsync("hello", Arg.Any<StringBuilder>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<IDataConversionResult>(DataConversionResult.FromTarget(new StringBuilder("kitty"))));

                var result = service.ImportDataAsync(dataStream, ctx => ctx.DataSpace(dataSpace));
                await result.AsTask();

                Assert.AreEqual(OperationState.Completed, result.OperationState);
                Assert.IsFalse(result.Errors().Any());
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
            var ctxFactory = this.CreateInjectableFactoryMock<DataImportContext>(() => new DataImportContext(Substitute.For<IServiceProvider>()));

            var changedTargetEntities = new List<IEntityEntry>();

            var sourceDataContext = this.CreateSourceDataContext();
            var targetDataContext = this.CreateTargetDataContext(ei => changedTargetEntities.Add(ei));
            var dataSpace = this.CreateDataSpace<string, StringBuilder>(sourceDataContext, targetDataContext);

            var service = new DefaultDataImportService(ctxFactory, reader, conversionService, resolver);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                reader.ReadAsync(dataStream, Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult((object)"hello"));

                conversionService.ConvertAsync("hello", Arg.Any<string>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                    .Returns(ci => Task.FromResult<IDataConversionResult>(DataConversionResult.FromTarget((string)ci.Arg<IDataConversionContext>()["entity"] == "hello" ? new StringBuilder("mimi") : new StringBuilder("kitty"))));

                var result = service.ImportDataAsync(dataStream, ctx => ctx.DataSpace(dataSpace).DataConversionConfig = (e, ctx) => ctx["entity"] = e);
                await result.AsTask();

                Assert.AreEqual(OperationState.Completed, result.OperationState);
                Assert.IsFalse(result.Errors().Any());
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
            var ctxFactory = this.CreateInjectableFactoryMock<DataImportContext>(() => new DataImportContext(Substitute.For<IServiceProvider>()));

            var sourceDataContext = this.CreateSourceDataContext();
            var targetDataContext = this.CreateTargetDataContext();
            var dataSpace = this.CreateDataSpace<string, StringBuilder>(sourceDataContext, targetDataContext);

            var sb = new StringBuilder();
            var b1 = this.CreateBehaviorFactory(sb, "b1", Priority.Low);
            var b2 = this.CreateBehaviorFactory(sb, "b2", Priority.High);

            var service = new DefaultDataImportService(ctxFactory, reader, conversionService, resolver, new List<IExportFactory<IDataImportBehavior, AppServiceMetadata>> { b1, b2 });
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                reader.ReadAsync(dataStream, Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult((object)"hello"));

                conversionService.ConvertAsync("hello", Arg.Any<string>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<IDataConversionResult>(DataConversionResult.FromTarget(new StringBuilder("kitty"))));

                await service.ImportDataAsync(dataStream, ctx => ctx.DataSpace(dataSpace)).AsTask();
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
                Arg.Any<IEntityEntry>(),
                Arg.Any<IDataImportContext>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        sb?.Append($"convert({name}),");
                        return Task.FromResult(0);
                    });

            b.BeforePersistEntityAsync(
                Arg.Any<IEntityEntry>(),
                Arg.Any<IEntityEntry>(),
                Arg.Any<IDataImportContext>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        sb?.Append($"before-persist({name}),");
                        return Task.FromResult(0);
                    });

            b.AfterPersistEntityAsync(
                Arg.Any<IEntityEntry>(),
                Arg.Any<IEntityEntry>(),
                Arg.Any<IDataImportContext>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        sb?.Append($"after-persist({name}),");
                        return Task.FromResult(0);
                    });

            return new ExportFactory<IDataImportBehavior, AppServiceMetadata>(() => b, new AppServiceMetadata(processingPriority: processingPriority));
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
            dataSpace[typeof(TSource)].Returns(sourceDataContext);
            dataSpace[typeof(TTarget)].Returns(targetDataContext);

            return dataSpace;
        }

        private IDataContext CreateSourceDataContext()
        {
            var sourceDataContext = Substitute.For<IDataContext>();
            sourceDataContext.Attach(Arg.Any<object>())
                .Returns(ci => new EntityEntry(ci.Arg<object>()) { DataContext = sourceDataContext });

            return sourceDataContext;
        }

        private IDataContext CreateTargetDataContext(Action<IEntityEntry> attachEntityCallback = null)
        {
            var targetDataContext = Substitute.For<IDataContext>();
            var persistChangesCommand = Substitute.For<IPersistChangesCommand>();
            targetDataContext.CreateCommand(typeof(IPersistChangesCommand))
                .Returns(persistChangesCommand);

            targetDataContext.Attach(Arg.Any<object>())
                .Returns(
                    ci =>
                        {
                            var ei = new EntityEntry(ci.Arg<object>()) { DataContext = targetDataContext };
                            attachEntityCallback?.Invoke(ei);
                            return ei;
                        });

            return targetDataContext;
        }
    }
}