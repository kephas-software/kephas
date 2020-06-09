// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSourceHandlerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data source handler test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition.ExportFactories;
    using Kephas.Data.DataSources;
    using Kephas.Messaging;
    using Kephas.Model;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DataSourceHandlerTest : TestBase
    {
        [Test]
        public async Task HandleAsync_data_source_context_properly_set()
        {
            var dataSpace = Substitute.For<IDataSpace>();
            var dataContext = Substitute.For<IDataContext>();
            dataSpace[typeof(int)].Returns(dataContext);
            dataSpace[typeof(int).AsRuntimeTypeInfo()].Returns(dataContext);

            var dataSpaceFactory = new ExportFactory<IDataSpace>(() => dataSpace);
            var dataSourceService = Substitute.For<IDataSourceService>();

            var typeResolver = Substitute.For<ITypeResolver>();
            typeResolver.ResolveType(Arg.Any<string>(), Arg.Any<bool>())
                .Returns(typeof(string));

            var projectedTypeResolver = Substitute.For<IProjectedTypeResolver>();
            projectedTypeResolver.ResolveProjectedType(typeof(string), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(int));

            var handler = new DataSourceHandler(dataSpaceFactory, dataSourceService, typeResolver, projectedTypeResolver);

            dataSourceService.GetDataSourceAsync(Arg.Any<IPropertyInfo>(), Arg.Any<IDataSourceContext>(), Arg.Any<CancellationToken>())
                .Returns(
                    ci =>
                        {
                            Assert.AreEqual("Length", ci.Arg<IPropertyInfo>().Name);
                            Assert.AreSame(dataContext, ci.Arg<IDataSourceContext>().DataContext);
                            Assert.AreSame(typeof(string), ci.Arg<IDataSourceContext>().EntityType.AsType());
                            Assert.AreSame(typeof(int), ci.Arg<IDataSourceContext>().ProjectedEntityType.AsType());
                            Assert.AreEqual("SomeRef", ci.Arg<IDataSourceContext>()["RefName"]);

                            return Task.FromResult((IEnumerable<object>)new object[] { "hello", "world" });
                        });

            var options = new { RefName = "SomeRef" };
            var message = new DataSourceMessage
                              {
                                  EntityType = "any",
                                  Property = "length",
                                  Options = options,
                              };

            var context = Substitute.For<IMessagingContext>();
            var result = await handler.ProcessAsync(message, context, default);
            var list = result.DataSource;
            Assert.AreEqual(2, list.Count);
        }
    }
}