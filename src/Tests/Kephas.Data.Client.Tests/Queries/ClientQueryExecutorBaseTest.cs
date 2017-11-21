// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQueryExecutorBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the client query executor base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class ClientQueryExecutorBaseTest
    {
        [Test]
        public async Task ExecuteQueryAsync()
        {
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

            var executor = new TestClientQueryExecutor(queryConverter, conversionService, typeResolver);
            var results = (await executor.ExecuteQueryAsync(clientQuery)).Cast<TestClientEntity>().ToList();

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual("C-1", results[0].Name);
            Assert.AreEqual("C-2", results[1].Name);
            Assert.AreEqual("C-3", results[2].Name);
        }

        [Test]
        public async Task ExecuteQueryAsync_context_config()
        {
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

            var executionContext = new ClientQueryExecutionContext
                                       {
                                           ClientQueryConversionContextConfig = ctx => ctx["gigi"] = "belogea",
                                           DataConversionContextConfig = (entity, ctx) => ctx["entity"] = entity,
                                       };
            var executor = new TestClientQueryExecutor(queryConverter, conversionService, typeResolver);

            var results = (await executor.ExecuteQueryAsync(clientQuery, executionContext)).Cast<TestClientEntity>().ToList();

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

        public class TestClientQueryExecutor : ClientQueryExecutorBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestClientQueryExecutor"/> class.
            /// </summary>
            /// <param name="clientQueryConverter">The client query converter.</param>
            /// <param name="conversionService">The conversion service.</param>
            /// <param name="typeResolver">The type resolver.</param>
            public TestClientQueryExecutor(
                IClientQueryConverter clientQueryConverter,
                IDataConversionService conversionService,
                ITypeResolver typeResolver)
                : base(clientQueryConverter, conversionService, typeResolver)
            {
            }

            /// <summary>
            /// Resolves the entity type based on its client counterpart.
            /// </summary>
            /// <param name="clientEntityType">Type of the client entity.</param>
            /// <returns>
            /// A type representing the entity type.
            /// </returns>
            protected override Type ResolveEntityType(Type clientEntityType)
            {
                return clientEntityType == typeof(TestClientEntity) ? typeof(TestEntity) : clientEntityType;
            }

            /// <summary>
            /// Creates the client data context.
            /// </summary>
            /// <returns>
            /// The new client data context.
            /// </returns>
            protected override IDataContext CreateClientDataContext()
            {
                return Substitute.For<IDataContext>();
            }

            /// <summary>
            /// Creates the entity data context.
            /// </summary>
            /// <returns>
            /// The new data context.
            /// </returns>
            protected override IDataContext CreateDataContext()
            {
                return Substitute.For<IDataContext>();
            }
        }
    }
}