// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="CompositionContainer" />.
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
    using Kephas.Services;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="CompositionContainer"/>.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionContainerTest : CompositionTestBase
    {
        public CompositionContainer CreateContainer(params Type[] types)
        {
            return this.WithEmptyConfiguration().WithParts(types).CreateCompositionContainer();
        }

        public CompositionContainer CreateExportProvidersContainer(params Type[] types)
        {
            var config = this.WithEmptyConfiguration();
            return this.WithExportProviders(config).WithParts(types).CreateCompositionContainer();
        }

        [TestMethod]
        public void Container_exports_composition_services()
        {
            var container = this.CreateContainer();
            var importer = new ContainerServicesImporter();
            container.SatisfyImports(importer);

            Assert.IsNotNull(importer.CompositionContainer);
        }

        [TestMethod]
        public void SatisfyImports_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var importer = new ConsumerOfExportedClass();
            container.SatisfyImports(importer);

            Assert.IsNotNull(importer.ExportedClass);
        }

        [TestMethod]
        public void SatisfyImports_implicits_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass), typeof(ExportedClassImplicitImporter));
            var importer = new ConsumerOfExportedClassImplicitImporter();
            container.SatisfyImports(importer);

            Assert.IsNotNull(importer.ImplicitImporter);
            Assert.IsNotNull(importer.ImplicitImporter.ExportedClass);
        }

        [TestMethod]
        public void GetExport_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExport(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            Assert.IsInstanceOfType(exported, typeof(ExportedClass));
        }

        [TestMethod]
        public void GetExport_with_exportfactory_success()
        {
            var container = this.CreateExportProvidersContainer(typeof(ExportedClass), typeof(ExportedClassImplicitFactoryImporter));
            var importer = container.GetExport<ExportedClassImplicitFactoryImporter>();

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.ExportedClassFactory);
            var exportedService = importer.ExportedClassFactory.CreateExport().Value;
            Assert.IsInstanceOfType(exportedService, typeof(ExportedClass));
        }

        [TestMethod]
        public void GetExport_with_importmany_exportfactory_success()
        {
            var container = this.CreateExportProvidersContainer(typeof(ExportedClass), typeof(ExportedClassImplicitFactoryImporter));
            var importer = container.GetExport<ExportedClassImplicitFactoryImporter>();

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.ExportedClassFactory);
            var exportedService = importer.ExportedClassFactory.CreateExport().Value;
            Assert.IsInstanceOfType(exportedService, typeof(ExportedClass));
        }

        [TestMethod]
        public void GetExport_with_exportfactory_metadata_success()
        {
            var container = this.CreateExportProvidersContainer(typeof(ExportedClass), typeof(ExportedClassImplicitFactoryMetadataImporter));
            var importer = container.GetExport<ExportedClassImplicitFactoryMetadataImporter>();

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.ExportedClassMetadataFactory);
            var exportedService = importer.ExportedClassMetadataFactory.CreateExport().Value;
            var metadata = importer.ExportedClassMetadataFactory.Metadata;
            Assert.IsInstanceOfType(exportedService, typeof(ExportedClass));
            Assert.IsNotNull(metadata);
        }

        [TestMethod]
        [ExpectedException(typeof(CompositionFailedException))]
        public void GetExport_failure()
        {
            var container = this.CreateContainer();
            var exported = container.GetExport(typeof(ExportedClass));
        }

        [TestMethod]
        public void GetExports_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExports(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(1, exportedList.Count);
            Assert.IsInstanceOfType(exportedList[0], typeof(ExportedClass));
        }

        [TestMethod]
        public void GetExports_empty()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExports(typeof(IEnumerable<object>));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(0, exportedList.Count);
        }

        private class ContainerServicesImporter
        {
            [Import]
            public ICompositionContainer CompositionContainer { get; set; }
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