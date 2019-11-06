// --------------------------------------------------------------------------------------------------------------------
// <copyright file="defaultdataconversionservicetest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the defaultdataconversionservicetest class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Conversion
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.Conversion.Composition;
    using Kephas.Data.Conversion.TargetResolvers;
    using Kephas.Reflection;
    using Kephas.Services;

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
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 5L }));

            var service =
                new DefaultDataConversionService(Substitute.For<ICompositionContext>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter,
                        new DataConverterMetadata(typeof(int), typeof(long)))
                },
                this.GetDefaultTargetResolverFactories());

            var result = await service.ConvertAsync<int, long>(1, 2L, new DataConversionContext(Substitute.For<IDataSpace>(), service), CancellationToken.None);
            Assert.AreEqual(5L, result.Target);
        }

        [Test]
        public async Task ConvertAsync_two_converters_success_last_overrides_target()
        {
            var converter1 = Substitute.For<IDataConverter>();
            converter1.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 5L }));

            var converter2 = Substitute.For<IDataConverter>();
            converter2.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 6L }));

            var service =
                new DefaultDataConversionService(Substitute.For<ICompositionContext>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter1,
                        new DataConverterMetadata(typeof(int), typeof(long))),
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter2,
                        new DataConverterMetadata(typeof(int), typeof(long)))
                },
                    this.GetDefaultTargetResolverFactories());

            var result = await service.ConvertAsync<int, long>(1, 2L, new DataConversionContext(Substitute.For<IDataSpace>(), service), CancellationToken.None);
            Assert.AreEqual(6L, result.Target);
        }

        [Test]
        public async Task ConvertAsync_two_converters_success_respects_processing_priority()
        {
            var converter1 = Substitute.For<IDataConverter>();
            converter1.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 5L }));

            var converter2 = Substitute.For<IDataConverter>();
            converter2.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 6L }));

            var service =
                new DefaultDataConversionService(Substitute.For<ICompositionContext>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter1,
                        new DataConverterMetadata(typeof(int), typeof(long), processingPriority:  (int)Priority.Low)),
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter2,
                        new DataConverterMetadata(typeof(int), typeof(long), processingPriority: (int)Priority.High))
                },
                    this.GetDefaultTargetResolverFactories());

            var result = await service.ConvertAsync<int, long>(1, 2L, new DataConversionContext(Substitute.For<IDataSpace>(), service), CancellationToken.None);
            Assert.AreEqual(5L, result.Target);
        }

        [Test]
        public async Task ConvertAsync_object_target_null_but_rootTargetType_provided()
        {
            var converter = Substitute.For<IDataConverter>();
            converter.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = new StringBuilder("hello") }));

            var resolver = Substitute.For<IDataConversionTargetResolver>();
            resolver.TryResolveTargetEntityAsync(
                Arg.Any<IDataContext>(),
                typeof(StringBuilder).GetTypeInfo(),
                Arg.Any<object>(),
                Arg.Any<IEntityEntry>(),
                Arg.Any<CancellationToken>()).Returns(new StringBuilder());

            var service = new DefaultDataConversionService(
                Substitute.For<ICompositionContext>(),
                new IExportFactory<IDataConverter, DataConverterMetadata>[]
                    {
                        new ExportFactory<IDataConverter, DataConverterMetadata>(
                            () => converter,
                            new DataConverterMetadata(typeof(string), typeof(StringBuilder)))
                    },
                new IExportFactory<IDataConversionTargetResolver, DataConversionTargetResolverMetadata>[]
                    {
                        new ExportFactory<IDataConversionTargetResolver, DataConversionTargetResolverMetadata>(
                            () => resolver,
                            new DataConversionTargetResolverMetadata(typeof(string), typeof(StringBuilder))), 
                    });

            var targetDataContext = Substitute.For<IDataContext>();
            var dataSpace = this.CreateDataSpace<string, StringBuilder>(null, targetDataContext);
            var result = await service.ConvertAsync<object, object>("sisi", null, new DataConversionContext(dataSpace, service, rootTargetType: typeof(StringBuilder)), CancellationToken.None);
            Assert.AreEqual("hello", result.Target.ToString());
        }

        [Test]
        public async Task ConvertAsync_object_target_not_null_rootTargetType_not_provided()
        {
            var converter = Substitute.For<IDataConverter>();
            converter.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = new StringBuilder("hello") }));

            var service =
                new DefaultDataConversionService(Substitute.For<ICompositionContext>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter,
                        new DataConverterMetadata(typeof(string), typeof(StringBuilder)))
                },
                    this.GetDefaultTargetResolverFactories());

            var result = await service.ConvertAsync<object, object>("sisi", new StringBuilder("queen"), new DataConversionContext(Substitute.For<IDataSpace>(), service), CancellationToken.None);
            Assert.AreEqual("hello", result.Target.ToString());
        }

        [Test]
        public async Task ConvertAsync_object_target_not_null_rootTargetType_override()
        {
            var converter = Substitute.For<IDataConverter>();
            converter.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = "hello" }));

            var service =
                new DefaultDataConversionService(
                    Substitute.For<ICompositionContext>(),
                    new IExportFactory<IDataConverter, DataConverterMetadata>[]
                    {
                        new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter,
                            new DataConverterMetadata(typeof(string), typeof(int)))
                    },
                    this.GetDefaultTargetResolverFactories());

            var result = await service.ConvertAsync<object, object>("sisi", 12, new DataConversionContext(Substitute.For<IDataSpace>(), service, rootTargetType: typeof(string)), CancellationToken.None);
            Assert.AreEqual("hello", result.Target);
        }

        [Test]
        public async Task ConvertAsync_object_target_null_rootTargetType_interface_override()
        {
            var converter = Substitute.For<IDataConverter>();
            converter.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                    {
                        var target = ci.ArgAt<IEntity>(1);
                        target.Name = "hello";
                        return Task.FromResult((IDataConversionResult)DataConversionResult.FromTarget(target));
                    });

            var service =
                new DefaultDataConversionService(
                    Substitute.For<ICompositionContext>(),
                    new IExportFactory<IDataConverter, DataConverterMetadata>[]
                    {
                        new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter,
                            new DataConverterMetadata(typeof(IEntity), typeof(IEntity)))
                    },
                    this.GetDefaultTargetResolverFactories());

            var createCommand = Substitute.For<ICreateEntityCommand>();
            var targetDataContext = Substitute.For<IDataContext>();
            targetDataContext.CreateCommand(typeof(ICreateEntityCommand)).Returns(createCommand);
            var dataSpace = this.CreateDataSpace<ClientEntity, Entity>(null, targetDataContext);

            createCommand.ExecuteAsync(Arg.Any<ICreateEntityContext>(), Arg.Any<CancellationToken>())
                .Returns(
                    ci =>
                        {
                            var createContext = ci.Arg<ICreateEntityContext>();
                            var target = createContext.EntityType.AsRuntimeTypeInfo().CreateInstance();
                            return Task.FromResult<ICreateEntityResult>(new CreateEntityResult(target, null, null, null));
                        });

            var result = await service.ConvertAsync<IEntity, IEntity>(new ClientEntity { Name = "sisi" }, null, new DataConversionContext(dataSpace, service, rootTargetType: typeof(Entity)), CancellationToken.None);
            Assert.IsInstanceOf<Entity>(result.Target);
            var entity = result.Target as Entity;
            Assert.AreEqual("hello", entity.Name);
        }

        [Test]
        public void ConvertAsync_object_exception()
        {
            var service = new DefaultDataConversionService(
                Substitute.For<ICompositionContext>(),
                new IExportFactory<IDataConverter, DataConverterMetadata>[0],
                this.GetDefaultTargetResolverFactories());

            Assert.Throws<DataConversionException>(() => service.ConvertAsync<object, object>("sisi", null, new DataConversionContext(Substitute.For<IDataSpace>(), service), CancellationToken.None));
        }

        private ICollection<IExportFactory<IDataConversionTargetResolver, DataConversionTargetResolverMetadata>> GetDefaultTargetResolverFactories()
        {
            return new List<IExportFactory<IDataConversionTargetResolver, DataConversionTargetResolverMetadata>>
                       {
                           new ExportFactory<IDataConversionTargetResolver, DataConversionTargetResolverMetadata>(
                               () => new IdDataConversionTargetResolver(),
                               new DataConversionTargetResolverMetadata(typeof(object), typeof(object), (int)Priority.High))
                       };
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

        public interface IEntity
        {
            string Name { get; set; }
        }

        public class Entity : IEntity
        {
            public string Name { get; set; }
        }

        public class ClientEntity : IEntity
        {
            public string Name { get; set; }
        }
    }
}