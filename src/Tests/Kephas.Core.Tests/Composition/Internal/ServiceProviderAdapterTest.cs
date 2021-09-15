// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderAdapterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition context service provider adapter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.ExportFactories;
using Kephas.Injection.ExportFactoryImporters;
using Kephas.Injection.Internal;

namespace Kephas.Core.Tests.Composition.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class ServiceProviderAdapterTest
    {
        [Test]
        public void GetService_success()
        {
            var context = Substitute.For<IInjector>();
            context.TryGetExport(typeof(string), Arg.Any<string>()).Returns("hello");
            var adapter = new ServiceProviderAdapter(context);

            var result = (string?)adapter.GetService(typeof(string));
            Assert.AreEqual("hello", result);
        }

        [Test]
        public void GetService_not_found()
        {
            var context = Substitute.For<IInjector>();
            context.TryGetExport(typeof(string), Arg.Any<string>()).Returns(null);
            var adapter = new ServiceProviderAdapter(context);

            var result = (string?)adapter.GetService(typeof(string));
            Assert.IsNull(result);
        }

        [Test]
        public void GetService_ExportFactory_generic_1_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExport(typeof(IExportFactoryImporter<string>), Arg.Any<string>()).Returns(this.CreateExportFactoryImporter("exported test"));
            var adapter = new ServiceProviderAdapter(context);

            var result = (IExportFactory<string>?)adapter.GetService(typeof(IExportFactory<string>))!;
            Assert.AreEqual("exported test", result.CreateExport().Value);
        }

        [Test]
        public void GetService_ExportFactory_generic_2_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExport(typeof(IExportFactoryImporter<string, string>), Arg.Any<string>()).Returns(this.CreateExportFactoryImporter("exported test", "metadata"));
            var adapter = new ServiceProviderAdapter(context);

            var result = (IExportFactory<string, string>?)adapter.GetService(typeof(IExportFactory<string, string>))!;
            Assert.AreEqual("exported test", result.CreateExport().Value);
            Assert.AreEqual("metadata", result.CreateExport().Metadata);
        }

        [Test]
        public void GetService_ExportFactories_enumerable_generic_1_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExport(typeof(ICollectionExportFactoryImporter<string>), Arg.Any<string>()).Returns(this.CreateExportFactoriesImporter("exported test"));
            var adapter = new ServiceProviderAdapter(context);

            var result = (IEnumerable<IExportFactory<string>>?)adapter.GetService(typeof(IEnumerable<IExportFactory<string>>))!;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
        }

        [Test]
        public void GetService_ExportFactories_enumerable_generic_2_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExport(typeof(ICollectionExportFactoryImporter<string, string>), Arg.Any<string>()).Returns(this.CreateExportFactoriesImporter("exported test", "metadata"));
            var adapter = new ServiceProviderAdapter(context);

            var result = (IEnumerable<IExportFactory<string, string>>?)adapter.GetService(typeof(IEnumerable<IExportFactory<string, string>>))!;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
            Assert.AreEqual("metadata", result.First().CreateExport().Metadata);
        }

        [Test]
        public void GetService_ExportFactories_collection_generic_1_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExport(typeof(ICollectionExportFactoryImporter<string>), Arg.Any<string>()).Returns(this.CreateExportFactoriesImporter("exported test"));
            var adapter = new ServiceProviderAdapter(context);

            var result = (ICollection<IExportFactory<string>>?)adapter.GetService(typeof(ICollection<IExportFactory<string>>))!;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
        }

        [Test]
        public void GetService_ExportFactories_collection_generic_2_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExport(typeof(ICollectionExportFactoryImporter<string, string>), Arg.Any<string>()).Returns(this.CreateExportFactoriesImporter("exported test", "metadata"));
            var adapter = new ServiceProviderAdapter(context);

            var result = (ICollection<IExportFactory<string, string>>?)adapter.GetService(typeof(ICollection<IExportFactory<string, string>>))!;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
            Assert.AreEqual("metadata", result.First().CreateExport().Metadata);
        }

        [Test]
        public void GetService_ExportFactories_list_generic_1_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExport(typeof(ICollectionExportFactoryImporter<string>), Arg.Any<string>()).Returns(this.CreateExportFactoriesImporter("exported test"));
            var adapter = new ServiceProviderAdapter(context);

            var result = (IList<IExportFactory<string>>?)adapter.GetService(typeof(IList<IExportFactory<string>>))!;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);

            var result2 = (List<IExportFactory<string>>?)adapter.GetService(typeof(List<IExportFactory<string>>))!;
            Assert.AreEqual(1, result2.Count());
            Assert.AreEqual("exported test", result2.First().CreateExport().Value);
        }

        [Test]
        public void GetService_ExportFactories_list_generic_2_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExport(typeof(ICollectionExportFactoryImporter<string, string>), Arg.Any<string>()).Returns(this.CreateExportFactoriesImporter("exported test", "metadata"));
            var adapter = new ServiceProviderAdapter(context);

            var result = (IList<IExportFactory<string, string>>?)adapter.GetService(typeof(IList<IExportFactory<string, string>>))!;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
            Assert.AreEqual("metadata", result.First().CreateExport().Metadata);

            var result2 = (List<IExportFactory<string, string>>?)adapter.GetService(typeof(List<IExportFactory<string, string>>))!;
            Assert.AreEqual(1, result2.Count());
            Assert.AreEqual("exported test", result2.First().CreateExport().Value);
            Assert.AreEqual("metadata", result2.First().CreateExport().Metadata);
        }

        [Test]
        public void GetService_Enumerable_string_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExports(typeof(string)).Returns(new[] { "hello", "world" });
            var adapter = new ServiceProviderAdapter(context);

            var result = (IEnumerable<string>?)adapter.GetService(typeof(IEnumerable<string>))!;
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("hello", result.First());
            Assert.AreEqual("world", result.Skip(1).First());
        }

        [Test]
        public void GetService_Collection_string_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExports(typeof(string)).Returns(new[] { "hello", "world" });
            var adapter = new ServiceProviderAdapter(context);

            var result = (ICollection<string>?)adapter.GetService(typeof(ICollection<string>))!;
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("hello", result.First());
            Assert.AreEqual("world", result.Skip(1).First());
        }

        [Test]
        public void GetService_IList_string_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExports(typeof(string)).Returns(new[] { "hello", "world" });
            var adapter = new ServiceProviderAdapter(context);

            var result = (IList<string>?)adapter.GetService(typeof(IList<string>))!;
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("hello", result.First());
            Assert.AreEqual("world", result.Skip(1).First());
        }

        [Test]
        public void GetService_List_string_success()
        {
            var context = Substitute.For<IInjector>();
            context.GetExports(typeof(string)).Returns(new[] { "hello", "world" });
            var adapter = new ServiceProviderAdapter(context);

            var result = (List<string>?)adapter.GetService(typeof(List<string>))!;
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("hello", result.First());
            Assert.AreEqual("world", result.Skip(1).First());
        }

        private IExportFactoryImporter<T> CreateExportFactoryImporter<T>(T value)
        {
            return new ExportFactoryImporter<T>(new ExportFactory<T>(() => value));
        }

        private IExportFactoryImporter<T, TMetadata> CreateExportFactoryImporter<T, TMetadata>(T value, TMetadata metadata)
        {
            return new ExportFactoryImporter<T, TMetadata>(new ExportFactory<T, TMetadata>(() => value, metadata));
        }

        private ICollectionExportFactoryImporter<T> CreateExportFactoriesImporter<T>(T value)
        {
            return new CollectionExportFactoryImporter<T>(this.GetEnumeration(value));
        }

        IEnumerable<IExportFactory<T>> GetEnumeration<T>(T value)
        {
            yield return new ExportFactory<T>(() => value);
        }

        private ICollectionExportFactoryImporter<T, TMetadata> CreateExportFactoriesImporter<T, TMetadata>(T value, TMetadata metadata)
        {
            return new CollectionExportFactoryImporter<T, TMetadata>(this.GetEnumeration(value, metadata));
        }

        IEnumerable<IExportFactory<T, TMetadata>> GetEnumeration<T, TMetadata>(T value, TMetadata metadata)
        {
            yield return new ExportFactory<T, TMetadata>(() => value, metadata);
        }
    }
}