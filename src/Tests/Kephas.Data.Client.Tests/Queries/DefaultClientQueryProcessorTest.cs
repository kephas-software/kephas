// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQueryExecutorBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client query executor base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.ExportFactories;

namespace Kephas.Data.Client.Tests.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Data.Client.Queries;
    using Kephas.Data.Client.Queries.Conversion;
    using Kephas.Data.Conversion;
    using Kephas.Model;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultClientQueryProcessorTest : ClientDataTestBase
    {
        [Test]
        public void Injection()
        {
            var container = this.CreateContainer();
            var processor = container.Resolve<IClientQueryProcessor>();

            Assert.IsInstanceOf<DefaultClientQueryProcessor>(processor);
        }

        [Test]
        public async Task ExecuteQueryAsync()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new ClientQueryExecutionContext(Substitute.For<IInjector>()));
            var entities = new List<TestEntity> { new TestEntity { Name = "1" }, new TestEntity { Name = "2" }, new TestEntity { Name = "3" }, };
            var query = entities.AsQueryable();

            var clientQuery = new ClientQuery();
            var queryConverter = Substitute.For<IClientQueryConverter>();
            queryConverter.ConvertQuery(clientQuery, Arg.Any<IClientQueryConversionContext>()).Returns(query);

            var typeResolver = Substitute.For<ITypeResolver>();
            typeResolver.ResolveType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(TestClientEntity));

            var conversionService = Substitute.For<IDataConversionService>();
            conversionService.ConvertAsync(
                Arg.Any<TestEntity>(),
                Arg.Any<TestClientEntity>(),
                Arg.Any<IDataConversionContext>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        var result = Substitute.For<IDataConversionResult>();
                        result.Target = new TestClientEntity { Name = "C-" + ci.Arg<TestEntity>().Name };
                        return Task.FromResult(result);
                    });

            var executor = new TestDefaultClientQueryExecutor(ctxFactory, queryConverter, conversionService, typeResolver);
            var results = (await executor.ExecuteQueryAsync(clientQuery)).Cast<TestClientEntity>().ToList();

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual("C-1", results[0].Name);
            Assert.AreEqual("C-2", results[1].Name);
            Assert.AreEqual("C-3", results[2].Name);
        }

        [Test]
        public async Task ExecuteQueryAsync_skip_conversion_for_same_type()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new ClientQueryExecutionContext(Substitute.For<IInjector>()));
            var entities = new List<TestEntity> { new TestEntity { Name = "1" }, new TestEntity { Name = "2" }, new TestEntity { Name = "3" }, };
            var query = entities.AsQueryable();

            var clientQuery = new ClientQuery();
            var queryConverter = Substitute.For<IClientQueryConverter>();
            queryConverter.ConvertQuery(clientQuery, Arg.Any<IClientQueryConversionContext>()).Returns(query);

            var typeResolver = Substitute.For<ITypeResolver>();
            typeResolver.ResolveType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(TestEntity));

            var conversionService = Substitute.For<IDataConversionService>();
            conversionService.ConvertAsync(
                Arg.Any<TestEntity>(),
                Arg.Any<TestEntity>(),
                Arg.Any<IDataConversionContext>(),
                Arg.Any<CancellationToken>()).Returns<Task<IDataConversionResult>>(
                ci =>
                    {
                        throw new InvalidOperationException("Conversion should be skipped");
                    });

            var executor = new TestDefaultClientQueryExecutor(
                ctxFactory,
                queryConverter,
                conversionService,
                typeResolver,
                clientDataContextCreator: () => throw new InvalidOperationException("Client data context creation should be skipped"));
            var results = (await executor.ExecuteQueryAsync(clientQuery)).Cast<TestEntity>().ToList();

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual("1", results[0].Name);
            Assert.AreEqual("2", results[1].Name);
            Assert.AreEqual("3", results[2].Name);
        }

        [Test]
        public async Task ExecuteQueryAsync_context_config()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new ClientQueryExecutionContext(Substitute.For<IInjector>()));
            var entities = new List<TestEntity> { new TestEntity { Name = "1" }, new TestEntity { Name = "2" }, new TestEntity { Name = "3" }, };
            var query = entities.AsQueryable();

            var clientQuery = new ClientQuery();
            var queryConverter = Substitute.For<IClientQueryConverter>();
            queryConverter.ConvertQuery(clientQuery, Arg.Any<IClientQueryConversionContext>())
                .Returns(ci => (string)ci.Arg<IClientQueryConversionContext>()["gigi"] == "belogea" ? query : new TestEntity[0].AsQueryable());

            var typeResolver = Substitute.For<ITypeResolver>();
            typeResolver.ResolveType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(TestClientEntity));

            var conversionService = Substitute.For<IDataConversionService>();
            conversionService.ConvertAsync(
                Arg.Any<TestEntity>(),
                Arg.Any<TestClientEntity>(),
                Arg.Any<IDataConversionContext>(),
                Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        var result = Substitute.For<IDataConversionResult>();
                        result.Target = new TestClientEntity { Name = "C-" + ((TestEntity)ci.Arg<IDataConversionContext>()["entity"])?.Name };
                        return Task.FromResult(result);
                    });

            var executor = new TestDefaultClientQueryExecutor(ctxFactory, queryConverter, conversionService, typeResolver);

            var results = (await executor.ExecuteQueryAsync(
                clientQuery,
                exctx =>
                    exctx.SetQueryConversionConfig(ctx => ctx.Set("gigi", "belogea"))
                         .SetDataConversionConfig((entity, ctx) => ctx.Set("entity", entity))))
                .Cast<TestClientEntity>().ToList();

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual("C-1", results[0].Name);
            Assert.AreEqual("C-2", results[1].Name);
            Assert.AreEqual("C-3", results[2].Name);
        }

        public class TestClientEntity
        {
            public string Name { get; set; }
        }

        public class TestEntity
        {
            public string Name { get; set; }
        }

        public class TestDefaultClientQueryExecutor : DefaultClientQueryProcessor
        {
            public TestDefaultClientQueryExecutor(
                IContextFactory contextFactory,
                IClientQueryConverter clientQueryConverter,
                IDataConversionService conversionService,
                ITypeResolver typeResolver,
                IProjectedTypeResolver projectedTypeResolver = null,
                Func<IDataContext> dataContextCreator = null,
                Func<IDataContext> clientDataContextCreator = null)
                : base(
                    contextFactory,
                    clientQueryConverter,
                    conversionService,
                    typeResolver,
                    projectedTypeResolver ?? GetProjectedTypeResolver(),
                    GetDataSpaceFactory(dataContextCreator, clientDataContextCreator),
                    new RuntimeTypeRegistry())
            {
            }

            private static IExportFactory<IDataSpace> GetDataSpaceFactory(
                Func<IDataContext> dataContextCreator,
                Func<IDataContext> clientDataContextCreator)
            {
                var dataSpace = Substitute.For<IDataSpace>();
                dataSpace[typeof(TestClientEntity)].Returns(ci => clientDataContextCreator?.Invoke() ?? Substitute.For<IDataContext>());
                dataSpace[typeof(TestEntity)].Returns(ci => dataContextCreator?.Invoke() ?? Substitute.For<IDataContext>());
                var factory = new ExportFactory<IDataSpace>(() => dataSpace);
                return factory;
            }

            private static IProjectedTypeResolver GetProjectedTypeResolver()
            {
                var resolver = Substitute.For<IProjectedTypeResolver>();
                resolver
                    .ResolveProjectedType(Arg.Any<Type>(), Arg.Any<IContext>(), Arg.Any<bool>())
                    .Returns(ci => ci.Arg<Type>() == typeof(TestClientEntity) ? typeof(TestEntity) : ci.Arg<Type>());
                return resolver;
            }
        }
    }
}