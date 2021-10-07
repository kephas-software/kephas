﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Kephas.Injection;
using Kephas.Injection.ExportFactoryImporters;
using NSubstitute;
using NUnit.Framework;

namespace Kephas.Core.Tests.Injection
{
    [TestFixture]
    public class InjectorExtensionsTest
    {
        [Test]
        public void GetExportFactory_generic_1_success()
        {
            var context = Substitute.For<IInjector>();
            context.Resolve(typeof(IExportFactoryImporter<string>)).Returns(this.CreateExportFactoryImporter("exported test"));

            var result = context.GetExportFactory<string>();
            Assert.AreEqual("exported test", result.CreateExport().Value);
        }

        [Test]
        public void GetExportFactory_success()
        {
            var context = Substitute.For<IInjector>();
            context.Resolve(typeof(IExportFactoryImporter<string>)).Returns(this.CreateExportFactoryImporter("exported test"));

            var result = (IExportFactory<string>)context.GetExportFactory(typeof(string));
            Assert.AreEqual("exported test", result.CreateExport().Value);
        }

        [Test]
        public void GetExportFactory_generic_2_success()
        {
            var context = Substitute.For<IInjector>();
            context.Resolve(typeof(IExportFactoryImporter<string, string>)).Returns(this.CreateExportFactoryImporter("exported test", "metadata"));

            var result = context.GetExportFactory<string, string>();
            Assert.AreEqual("exported test", result.CreateExport().Value);
            Assert.AreEqual("metadata", result.CreateExport().Metadata);
        }

        [Test]
        public void GetExportFactory_metadata_success()
        {
            var context = Substitute.For<IInjector>();
            context.Resolve(typeof(IExportFactoryImporter<string, string>)).Returns(this.CreateExportFactoryImporter("exported test", "metadata"));

            var result = (IExportFactory<string, string>)context.GetExportFactory(typeof(string), typeof(string));
            Assert.AreEqual("exported test", result.CreateExport().Value);
            Assert.AreEqual("metadata", result.CreateExport().Metadata);
        }

        [Test]
        public void GetExportFactories_generic_1_success()
        {
            var context = Substitute.For<IInjector>();
            context.Resolve(typeof(ICollectionExportFactoryImporter<string>)).Returns(this.CreateExportFactoriesImporter("exported test"));

            var result = context.GetExportFactories<string>();
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
        }

        [Test]
        public void GetExportFactories_success()
        {
            var context = Substitute.For<IInjector>();
            context.Resolve(typeof(ICollectionExportFactoryImporter<string>)).Returns(this.CreateExportFactoriesImporter("exported test"));

            var result = (IEnumerable<IExportFactory<string>>)context.GetExportFactories(typeof(string));
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
        }

        [Test]
        public void GetExportFactories_generic_2_success()
        {
            var context = Substitute.For<IInjector>();
            context.Resolve(typeof(ICollectionExportFactoryImporter<string, string>)).Returns(this.CreateExportFactoriesImporter("exported test", "metadata"));

            var result = context.GetExportFactories<string, string>();
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
            Assert.AreEqual("metadata", result.First().CreateExport().Metadata);
        }

        [Test]
        public void GetExportFactories_metadata_success()
        {
            var context = Substitute.For<IInjector>();
            context.Resolve(typeof(ICollectionExportFactoryImporter<string, string>)).Returns(this.CreateExportFactoriesImporter("exported test", "metadata"));

            var result = (IEnumerable<IExportFactory<string, string>>)context.GetExportFactories(typeof(string), typeof(string));
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("exported test", result.First().CreateExport().Value);
            Assert.AreEqual("metadata", result.First().CreateExport().Metadata);
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
            return new CollectionExportFactoryImporter<T>(new[] { new ExportFactory<T>(() => value) });
        }

        private ICollectionExportFactoryImporter<T, TMetadata> CreateExportFactoriesImporter<T, TMetadata>(T value, TMetadata metadata)
        {
            return new CollectionExportFactoryImporter<T, TMetadata>(new[] { new ExportFactory<T, TMetadata>(() => value, metadata) });
        }
    }
}