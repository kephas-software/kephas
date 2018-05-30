// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataExportServiceTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data export service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.Export
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Client.Queries;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Export;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultDataExportServiceTest
    {
        [Test]
        public async Task ExportDataAsync_query()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryExecutor>();

            var entities = new List<object> { "hello" };
            queryExecutor.ExecuteQueryAsync(Arg.Any<ClientQuery>(), Arg.Any<IClientQueryExecutionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IList<object>>(entities));

            writer.WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var service = new DefaultDataExportService(Substitute.For<ICompositionContext>(), writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                var context = new DataExportContext(Substitute.For<ClientQuery>(), dataStream);
                await service.ExportDataAsync(context);
            }

            writer.Received(1)
                .WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ExportDataAsync_query_no_data()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryExecutor>();

            queryExecutor.ExecuteQueryAsync(Arg.Any<ClientQuery>(), Arg.Any<IClientQueryExecutionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IList<object>>(new object[0]));

            var service = new DefaultDataExportService(Substitute.For<ICompositionContext>(), writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                var context = new DataExportContext(Substitute.For<ClientQuery>(), dataStream) { ThrowOnNotFound = true };
                Assert.ThrowsAsync<NotFoundDataException>(() => service.ExportDataAsync(context));
            }
        }

        [Test]
        public async Task ExportDataAsync_data()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryExecutor>();

            var entities = new List<object> { "hello" };

            writer.WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var service = new DefaultDataExportService(Substitute.For<ICompositionContext>(), writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                var context = new DataExportContext(entities, dataStream);
                await service.ExportDataAsync(context);
            }

            writer.Received(1)
                .WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ExportDataAsync_no_data_no_throw()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryExecutor>();

            var entities = new List<object>();

            writer.WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var service = new DefaultDataExportService(Substitute.For<ICompositionContext>(), writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                var context = new DataExportContext(entities, dataStream);
                await service.ExportDataAsync(context);
            }

            writer.Received(1)
                .WriteAsync(entities, Arg.Any<DataStream>(), Arg.Any<IDataIOContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ExportDataAsync_data_no_data()
        {
            var writer = Substitute.For<IDataStreamWriteService>();
            var queryExecutor = Substitute.For<IClientQueryExecutor>();

            var service = new DefaultDataExportService(Substitute.For<ICompositionContext>(), writer, queryExecutor);
            using (var dataStream = new DataStream(new MemoryStream(), ownsStream: true))
            {
                var context = new DataExportContext(new object[0], dataStream) { ThrowOnNotFound = true };
                Assert.ThrowsAsync<NotFoundDataException>(() => service.ExportDataAsync(context));
            }
        }
    }
}