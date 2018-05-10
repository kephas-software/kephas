// --------------------------------------------------------------------------------------------------------------------
// <copyright file="defaultdataconversionservicetest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the defaultdataconversionservicetest class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Conversion
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
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
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = 5 }));

            var service =
                new DefaultDataConversionService(Substitute.For<ICompositionContext>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter,
                        new DataConverterMetadata(typeof(int), typeof(int)))
                },
                this.GetDefaultTargetResolverFactories());

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
                new DefaultDataConversionService(Substitute.For<ICompositionContext>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter1,
                        new DataConverterMetadata(typeof(int), typeof(int))),
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter2,
                        new DataConverterMetadata(typeof(int), typeof(int)))
                },
                    this.GetDefaultTargetResolverFactories());

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
                new DefaultDataConversionService(Substitute.For<ICompositionContext>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter1,
                        new DataConverterMetadata(typeof(int), typeof(int), processingPriority:  (int)Priority.Low)),
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter2,
                        new DataConverterMetadata(typeof(int), typeof(int), processingPriority: (int)Priority.High))
                },
                    this.GetDefaultTargetResolverFactories());

            var result = await service.ConvertAsync<int, int>(1, 2, new DataConversionContext(service), CancellationToken.None);
            Assert.AreEqual(5, result.Target);
        }

        [Test]
        public async Task ConvertAsync_object_target_null_but_rootTargetType_provided()
        {
            var converter = Substitute.For<IDataConverter>();
            converter.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = "hello" }));

            var service =
                new DefaultDataConversionService(Substitute.For<ICompositionContext>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter,
                        new DataConverterMetadata(typeof(string), typeof(string)))
                },
                    this.GetDefaultTargetResolverFactories());

            var result = await service.ConvertAsync<object, object>("sisi", null, new DataConversionContext(service, rootTargetType: typeof(string)), CancellationToken.None);
            Assert.AreEqual("hello", result.Target);
        }

        [Test]
        public async Task ConvertAsync_object_target_not_null_rootTargetType_not_provided()
        {
            var converter = Substitute.For<IDataConverter>();
            converter.ConvertAsync(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<IDataConversionContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IDataConversionResult)new DataConversionResult { Target = "hello" }));

            var service =
                new DefaultDataConversionService(Substitute.For<ICompositionContext>(), new IExportFactory<IDataConverter, DataConverterMetadata>[]
                {
                    new ExportFactory<IDataConverter, DataConverterMetadata>(() => converter,
                        new DataConverterMetadata(typeof(string), typeof(string)))
                },
                    this.GetDefaultTargetResolverFactories());

            var result = await service.ConvertAsync<object, object>("sisi", "queen", new DataConversionContext(service), CancellationToken.None);
            Assert.AreEqual("hello", result.Target);
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

            var result = await service.ConvertAsync<object, object>("sisi", 12, new DataConversionContext(service, rootTargetType: typeof(string)), CancellationToken.None);
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

            createCommand.ExecuteAsync(Arg.Any<ICreateEntityContext>(), Arg.Any<CancellationToken>())
                .Returns(
                    ci =>
                        {
                            var createContext = ci.Arg<ICreateEntityContext>();
                            var target = createContext.EntityType.AsRuntimeTypeInfo().CreateInstance();
                            return Task.FromResult<ICreateEntityResult>(new CreateEntityResult(target, null, null, null));
                        });

            var result = await service.ConvertAsync<IEntity, IEntity>(new Entity { Name = "sisi" }, null, new DataConversionContext(service, rootTargetType: typeof(Entity), targetDataContext: targetDataContext), CancellationToken.None);
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


            Assert.Throws<DataConversionException>(() => service.ConvertAsync<object, object>("sisi", null, new DataConversionContext(service), CancellationToken.None));
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

        public interface IEntity
        {
            string Name { get; set; }
        }

        public class Entity : IEntity
        {
            public string Name { get; set; }
        }
    }
}