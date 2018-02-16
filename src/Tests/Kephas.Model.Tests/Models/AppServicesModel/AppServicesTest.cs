// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application services test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.AppServicesModel
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Model.Elements;

    using NUnit.Framework;

    [TestFixture]
    public class AppServicesTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_simple_app_service()
        {
            var container = this.CreateContainerForModel(typeof(ISimpleService));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var disposableClassifier = modelSpace.Classifiers.Single(c => c.Name == "SimpleService");

            Assert.IsInstanceOf<AppService>(disposableClassifier);
            Assert.AreEqual(1, disposableClassifier.Parts.Count());
        }

        [Test]
        public async Task InitializeAsync_app_service_with_contract_type()
        {
            var container = this.CreateContainerForModel(typeof(IGenericDisposableService<>), typeof(IDisposable));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var disposableClassifier = modelSpace.Classifiers.Single(c => c.Name == "DisposableService");

            Assert.IsInstanceOf<AppService>(disposableClassifier);
            Assert.AreEqual(1, disposableClassifier.Parts.Count());
        }

        [Test]
        public async Task InitializeAsync_app_service_with_contract_type_multiple_bases()
        {
            var container = this.CreateContainerForModel(typeof(IGenericDisposableService<>), typeof(IDisposable), typeof(ICloneable));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var disposableClassifier = modelSpace.Classifiers.Single(c => c.Name == "DisposableService");

            Assert.IsInstanceOf<AppService>(disposableClassifier);
            Assert.AreEqual(1, disposableClassifier.Parts.Count());
        }

        [Test]
        public async Task InitializeAsync_open_generic_app_service()
        {
            var container = this.CreateContainerForModel(typeof(IOpenGenericService<>));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var disposableClassifier = modelSpace.Classifiers.Single(c => c.Name == "OpenGenericService`1");

            Assert.IsInstanceOf<AppService>(disposableClassifier);
            Assert.AreEqual(1, disposableClassifier.Parts.Count());
        }

        [Test]
        public async Task InitializeAsync_closed_generic_app_service()
        {
            var container = this.CreateContainerForModel(typeof(IClosedGenericService<>));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var disposableClassifier = modelSpace.Classifiers.Single(c => c.Name == "ClosedGenericService`1");

            Assert.IsInstanceOf<AppService>(disposableClassifier);
            Assert.AreEqual(1, disposableClassifier.Parts.Count());
        }
    }
}