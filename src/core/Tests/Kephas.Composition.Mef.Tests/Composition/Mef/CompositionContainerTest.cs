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

        [Export]
        public class ExportedClassImplicitImporter
        {
            [Import]
            public ExportedClass ExportedClass { get; set; }
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