// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Testing.Model;
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
            var simpleServiceClassifier = modelSpace.Classifiers.Single(c => c.Name == "SimpleService");

            Assert.IsInstanceOf<AppServiceType>(simpleServiceClassifier);
            Assert.AreEqual(1, simpleServiceClassifier.Parts.Count());
        }

        [Test]
        public async Task InitializeAsync_simple_app_service_methods()
        {
            var container = this.CreateContainerForModel(typeof(ISimpleService), typeof(IDisposableService));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var simpleServiceClassifier = modelSpace.Classifiers.Single(c => c.Name == "SimpleService");
            var disposableServiceClassifier = modelSpace.Classifiers.Single(c => c.Name == "DisposableService");

            Assert.AreEqual(2, simpleServiceClassifier.Methods.Count());

            var doSomethingMethod =
                simpleServiceClassifier.Methods.Single(m => m.Name == nameof(ISimpleService.DoSomething));

            Assert.IsInstanceOf<Method>(doSomethingMethod);
            Assert.AreEqual(1, doSomethingMethod.Parameters.Count());
            var param1 = doSomethingMethod.Parameters.Single();
            Assert.AreSame(disposableServiceClassifier, param1.ValueType);
        }

        [Test]
        public async Task InitializeAsync_app_service_with_contract_type()
        {
            var container = this.CreateContainerForModel(typeof(IGenericDisposableService<>), typeof(IDisposable));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var disposableClassifier = modelSpace.Classifiers.Single(c => c.Name == "DisposableService");

            Assert.IsInstanceOf<AppServiceType>(disposableClassifier);
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

            Assert.IsInstanceOf<AppServiceType>(disposableClassifier);
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

            Assert.IsInstanceOf<AppServiceType>(disposableClassifier);
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

            Assert.IsInstanceOf<AppServiceType>(disposableClassifier);
            Assert.AreEqual(1, disposableClassifier.Parts.Count());
        }
    }
}