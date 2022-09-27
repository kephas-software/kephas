// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataExportServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data export service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.IO.Tests.Export
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Data.Client.Queries;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Export;
    using Kephas.Services;
    using Kephas.Testing;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultDataExportServiceTest : TestBase
    {
        [Test]
        public async Task ExportDataAsync_query()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryProcessor>();
            var ctxFactory = this.CreateInjectableFactoryMock<DataExportContext>(() => new DataExportContext(Substitute.For<IServiceProvider>()));

            var entities = new List<object> { "hello" };
            queryExecutor.ExecuteQueryAsync(Arg.Any<ClientQuery>(), Arg.Any<Action<IClientQueryExecutionContext>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IList<object>>(entities));

            writer.WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var service = new DefaultDataExportService(ctxFactory, writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                await service.ExportDataAsync(ctx => ctx.Query(Substitute.For<ClientQuery>()).Output(dataStream)).AsTask();
            }

            writer.Received(1)
                .WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ExportDataAsync_query_no_data()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryProcessor>();
            var ctxFactory = this.CreateInjectableFactoryMock<DataExportContext>(() => new DataExportContext(Substitute.For<IServiceProvider>()));

            queryExecutor.ExecuteQueryAsync(Arg.Any<ClientQuery>(), Arg.Any<Action<IClientQueryExecutionContext>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IList<object>>(new object[0]));

            var service = new DefaultDataExportService(ctxFactory, writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                Assert.ThrowsAsync<NotFoundDataException>(async () =>
                    await service.ExportDataAsync(
                        ctx => ctx.Query(Substitute.For<ClientQuery>()).Output(dataStream).ThrowOnNotFound(true)).AsTask());
            }
        }

        [Test]
        public async Task ExportDataAsync_data()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryProcessor>();
            var ctxFactory = this.CreateInjectableFactoryMock<DataExportContext>(() => new DataExportContext(Substitute.For<IServiceProvider>()));

            var entities = new List<object> { "hello" };

            writer.WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var service = new DefaultDataExportService(ctxFactory, writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                await service.ExportDataAsync(ctx => ctx.Data(entities).Output(dataStream)).AsTask();
            }

            writer.Received(1)
                .WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ExportDataAsync_no_data_no_throw()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryProcessor>();
            var ctxFactory = this.CreateInjectableFactoryMock<DataExportContext>(() => new DataExportContext(Substitute.For<IServiceProvider>()));

            var entities = new List<object>();

            writer.WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var service = new DefaultDataExportService(ctxFactory, writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                await service.ExportDataAsync(ctx => ctx.Data(entities).Output(dataStream)).AsTask();
            }

            writer.Received(1)
                .WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ExportDataAsync_data_no_data()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryProcessor>();
            var ctxFactory = this.CreateInjectableFactoryMock<DataExportContext>(() => new DataExportContext(Substitute.For<IServiceProvider>()));

            var service = new DefaultDataExportService(ctxFactory, writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                Assert.ThrowsAsync<NotFoundDataException>(async () => await service.ExportDataAsync(ctx => ctx.Data(new object[0]).Output(dataStream).ThrowOnNotFound(true)).AsTask());
            }
        }
    }
}