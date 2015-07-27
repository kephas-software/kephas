// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefCompositionContainerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="MefCompositionContainer" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Kephas.Composition.Mef.Hosting;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="MefCompositionContainer"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class MefCompositionContainerTest : CompositionTestBase
    {
        public MefCompositionContainer CreateContainer(params Type[] types)
        {
            return this.WithEmptyConfiguration().WithParts(types).CreateCompositionContainer();
        }

        public MefCompositionContainer CreateExportProvidersContainer(params Type[] types)
        {
            var config = this.WithEmptyConfiguration();
            return this.WithExportProviders(config).WithParts(types).CreateCompositionContainer();
        }

        [Test]
        public void GetExport_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExport(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryGetExport_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.TryGetExport(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryGetExport_failure()
        {
            var container = this.CreateContainer();
            var exported = container.TryGetExport(typeof(ExportedClass));

            Assert.IsNull(exported);
        }

        [Test]
        public void GetExport_generic_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExport<ExportedClass>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryGetExport_generic_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.TryGetExport<ExportedClass>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryGetExport_generic_failure()
        {
            var container = this.CreateContainer();
            var exported = container.TryGetExport<ExportedClass>();

            Assert.IsNull(exported);
        }

        [Test]
        public void GetExport_with_exportfactory_success()
        {
            var container = this.CreateExportProvidersContainer(typeof(ExportedClass), typeof(ExportedClassImplicitFactoryImporter));
            var importer = container.GetExport<ExportedClassImplicitFactoryImporter>();

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.ExportedClassFactory);
            var exportedService = importer.ExportedClassFactory.CreateExport().Value;
            Assert.IsInstanceOf<ExportedClass>(exportedService);
        }

        [Test]
        public void GetExport_with_importmany_exportfactory_success()
        {
            var container = this.CreateExportProvidersContainer(typeof(ExportedClass), typeof(ExportedClassImplicitFactoryImporter));
            var importer = container.GetExport<ExportedClassImplicitFactoryImporter>();

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.ExportedClassFactory);
            var exportedService = importer.ExportedClassFactory.CreateExport().Value;
            Assert.IsInstanceOf<ExportedClass>(exportedService);
        }

        [Test]
        public void GetExport_with_exportfactory_metadata_success()
        {
            var container = this.CreateExportProvidersContainer(typeof(ExportedClass), typeof(ExportedClassImplicitFactoryMetadataImporter));
            var importer = container.GetExport<ExportedClassImplicitFactoryMetadataImporter>();

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.ExportedClassMetadataFactory);
            var exportedService = importer.ExportedClassMetadataFactory.CreateExport().Value;
            var metadata = importer.ExportedClassMetadataFactory.Metadata;
            Assert.IsInstanceOf<ExportedClass>(exportedService);
            Assert.IsNotNull(metadata);
        }

        [Test]
        [ExpectedException(typeof(CompositionFailedException))]
        public void GetExport_failure()
        {
            var container = this.CreateContainer();
            var exported = container.GetExport(typeof(ExportedClass));
        }

        [Test]
        public void GetExports_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExports(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(1, exportedList.Count);
            Assert.IsInstanceOf<ExportedClass>(exportedList[0]);
        }

        [Test]
        public void GetExports_empty()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExports(typeof(IEnumerable<object>));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(0, exportedList.Count);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Dispose()
        {
            var container = this.CreateContainer();
            container.Dispose();
            var export = container.TryGetExport<IList<string>>();
        }

        [Test]
        public void Dispose_multiple()
        {
            var container = this.CreateContainer();
            container.Dispose();
            container.Dispose();
        }

        private class ContainerServicesImporter
        {
            [Import]
            public ICompositionContext CompositionContainer { get; set; }
        }

        [Export]
        public class ExportedClass
        {
        }

        [Export(typeof(ExportedClass))]
        public class ExportedClass2 : ExportedClass
        {
        }

        [Export]
        public class ExportedClassImplicitImporter
        {
            [Import]
            public ExportedClass ExportedClass { get; set; }
        }

        [Export]
        public class ExportedClassImplicitFactoryImporter
        {
            [Import]
            public IExportFactory<ExportedClass> ExportedClassFactory { get; set; }
        }

        [Export]
        public class ExportedClassImplicitManyFactoryImporter
        {
            public ICollection<IExportFactory<ExportedClass>> ExportedClassFactoryCollection { get; set; }
            public IList<IExportFactory<ExportedClass>> ExportedClassFactoryList { get; set; }
            public IEnumerable<IExportFactory<ExportedClass>> ExportedClassFactoryEnumerable { get; set; }
            public IExportFactory<ExportedClass>[] ExportedClassFactoryArray { get; set; }
        }

        [Export]
        public class ExportedClassImplicitFactoryMetadataImporter
        {
            [Import]
            public IExportFactory<ExportedClass, TestMetadata> ExportedClassMetadataFactory { get; set; }
        }

        public class TestMetadata
        {
        }

        private class ConsumerOfExportedClassImplicitImporter
        {
            [Import]
            public ExportedClassImplicitImporter ImplicitImporter { get; set; }
        }

        private class ConsumerOfExportedClass
        {
            [Import]
            public ExportedClass ExportedClass { get; set; }
        }
    }
}