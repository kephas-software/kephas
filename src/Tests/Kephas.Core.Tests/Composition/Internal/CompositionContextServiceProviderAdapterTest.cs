// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContextServiceProviderAdapterTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the composition context service provider adapter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Composition.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactoryImporters;
    using Kephas.Composition.Internal;
    using Kephas.Testing.Core.Composition;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    [TestFixture]
    public class CompositionContextServiceProviderAdapterTest
    {
        [Test]
        public void GetService_success()
        {
            var context = Mock.Create<ICompositionContext>();
            context.Arrange(c => c.GetExport(typeof(string), Arg.AnyString)).Returns("hello");
            var adapter = new CompositionContextServiceProviderAdapter(context);

            var result = (string)adapter.GetService(typeof(string));
            Assert.AreEqual("hello", result);
        }

        [Test]
        public void GetService_ExportFactory_generic_1_success()
        {
            var context = Mock.Create<ICompositionContext>();
            context.Arrange(c => c.GetExport(typeof(IExportFactoryImporter<string>), Arg.AnyString)).Returns(this.CreateExportFactoryImporter("exported test"));
            var adapter = new CompositionContextServiceProviderAdapter(context);

            var result = (IExportFactory<string>)adapter.GetService(typeof(IExportFactory<string>));
            Assert.AreEqual("exported test", result.CreateExport().Value);
        }

        [Test]
        public void GetService_ExportFactory_generic_2_success()
        {
            var context = Mock.Create<ICompositionContext>();
            context.Arrange(c => c.GetExport(typeof(IExportFactoryImporter<string, string>), Arg.AnyString)).Returns(this.CreateExportFactoryImporter("exported test", "metadata"));
            var adapter = new CompositionContextServiceProviderAdapter(context);

            var result = (IExportFactory<string, string>)adapter.GetService(typeof(IExportFactory<string, string>));
            Assert.AreEqual("exported test", result.CreateExport().Value);
            Assert.AreEqual("metadata", result.CreateExport().Metadata);
        }

        [Test]
        public void GetService_ExportFactories_enumerable_generic_1_success()
        {
            var context = Mock.Create<ICompositionContext>();
            context.Arrange(c => c.GetExport(typeof(ICollectionExportFactoryImporter<string>), Arg.AnyString)).Returns(this.CreateExportFactoriesImporter("exported test"));
            var adapter = new CompositionContextServiceProviderAdapter(context);

            var result = (IEnumerable<IExportFactory<string>>)adapter.GetService(typeof(IEnumerable<IExportFactory<string>>));
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
        }

        [Test]
        public void GetService_ExportFactories_enumerable_generic_2_success()
        {
            var context = Mock.Create<ICompositionContext>();
            context.Arrange(c => c.GetExport(typeof(ICollectionExportFactoryImporter<string, string>), Arg.AnyString)).Returns(this.CreateExportFactoriesImporter("exported test", "metadata"));
            var adapter = new CompositionContextServiceProviderAdapter(context);

            var result = (IEnumerable<IExportFactory<string, string>>)adapter.GetService(typeof(IEnumerable<IExportFactory<string, string>>));
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
            Assert.AreEqual("metadata", result.First().CreateExport().Metadata);
        }

        [Test]
        public void GetService_ExportFactories_collection_generic_1_success()
        {
            var context = Mock.Create<ICompositionContext>();
            context.Arrange(c => c.GetExport(typeof(ICollectionExportFactoryImporter<string>), Arg.AnyString)).Returns(this.CreateExportFactoriesImporter("exported test"));
            var adapter = new CompositionContextServiceProviderAdapter(context);

            var result = (ICollection<IExportFactory<string>>)adapter.GetService(typeof(ICollection<IExportFactory<string>>));
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
        }

        [Test]
        public void GetService_ExportFactories_collection_generic_2_success()
        {
            var context = Mock.Create<ICompositionContext>();
            context.Arrange(c => c.GetExport(typeof(ICollectionExportFactoryImporter<string, string>), Arg.AnyString)).Returns(this.CreateExportFactoriesImporter("exported test", "metadata"));
            var adapter = new CompositionContextServiceProviderAdapter(context);

            var result = (ICollection<IExportFactory<string, string>>)adapter.GetService(typeof(ICollection<IExportFactory<string, string>>));
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
            Assert.AreEqual("metadata", result.First().CreateExport().Metadata);
        }

        [Test]
        public void GetService_ExportFactories_list_generic_1_success()
        {
            var context = Mock.Create<ICompositionContext>();
            context.Arrange(c => c.GetExport(typeof(ICollectionExportFactoryImporter<string>), Arg.AnyString)).Returns(this.CreateExportFactoriesImporter("exported test"));
            var adapter = new CompositionContextServiceProviderAdapter(context);

            var result = (IList<IExportFactory<string>>)adapter.GetService(typeof(IList<IExportFactory<string>>));
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);

            var result2 = (List<IExportFactory<string>>)adapter.GetService(typeof(List<IExportFactory<string>>));
            Assert.AreEqual(1, result2.Count());
            Assert.AreEqual("exported test", result2.First().CreateExport().Value);
        }

        [Test]
        public void GetService_ExportFactories_list_generic_2_success()
        {
            var context = Mock.Create<ICompositionContext>();
            context.Arrange(c => c.GetExport(typeof(ICollectionExportFactoryImporter<string, string>), Arg.AnyString)).Returns(this.CreateExportFactoriesImporter("exported test", "metadata"));
            var adapter = new CompositionContextServiceProviderAdapter(context);

            var result = (IList<IExportFactory<string, string>>)adapter.GetService(typeof(IList<IExportFactory<string, string>>));
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
            Assert.AreEqual("metadata", result.First().CreateExport().Metadata);

            var result2 = (List<IExportFactory<string, string>>)adapter.GetService(typeof(List<IExportFactory<string, string>>));
            Assert.AreEqual(1, result2.Count());
            Assert.AreEqual("exported test", result2.First().CreateExport().Value);
            Assert.AreEqual("metadata", result2.First().CreateExport().Metadata);
        }

        private IExportFactoryImporter<T> CreateExportFactoryImporter<T>(T value)
        {
            return new ExportFactoryImporter<T>(new TestExportFactory<T>(() => value));
        }

        private IExportFactoryImporter<T, TMetadata> CreateExportFactoryImporter<T, TMetadata>(T value, TMetadata metadata)
        {
            return new ExportFactoryImporter<T, TMetadata>(new TestExportFactory<T, TMetadata>(() => value, metadata));
        }

        private ICollectionExportFactoryImporter<T> CreateExportFactoriesImporter<T>(T value)
        {
            return new CollectionExportFactoryImporter<T>(this.GetEnumeration(value));
        }

        IEnumerable<IExportFactory<T>> GetEnumeration<T>(T value)
        {
            yield return new TestExportFactory<T>(() => value);
        }

        private ICollectionExportFactoryImporter<T, TMetadata> CreateExportFactoriesImporter<T, TMetadata>(T value, TMetadata metadata)
        {
            return new CollectionExportFactoryImporter<T, TMetadata>(this.GetEnumeration(value, metadata));
        }

        IEnumerable<IExportFactory<T, TMetadata>> GetEnumeration<T, TMetadata>(T value, TMetadata metadata)
        {
            yield return new TestExportFactory<T, TMetadata>(() => value, metadata);
        }
    }
}