// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceConstructorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service constructor test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System;
    using System.Linq;

    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Runtime;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class AppServiceConstructorTest : ConstructorTestBase
    {
        private (INamedElement element, IRuntimeTypeInfo runtimeElement) TryCreateTestModelElement<TContract>()
        {
            return TryCreateTestModelElement(typeof(TContract));
        }

        private (INamedElement element, IRuntimeTypeInfo runtimeElement) TryCreateTestModelElement(Type serviceType)
        {
            var constructor = new AppServiceTypeConstructor();
            var context = this.GetConstructionContext();
            var runtimeElement = context.RuntimeTypeRegistry.GetTypeInfo(serviceType);
            var modelElement = constructor.TryCreateModelElement(context, runtimeElement);

            return (modelElement, runtimeElement);
        }

        [Test]
        public void TryCreateModelElement_singleton_app_service()
        {
            var (modelElement, runtimeElement) = this.TryCreateTestModelElement<ISingletonTestAppService>();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(runtimeElement, modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_app_service()
        {
            var (modelElement, runtimeElement) = this.TryCreateTestModelElement<ITestAppService>();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(runtimeElement, modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_generic_exported_app_service()
        {
            var (modelElement, runtimeElement) = this.TryCreateTestModelElement(typeof(ITestGenericExportedService<>));

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(runtimeElement, modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_generic_exported_as_non_generic()
        {
            var (modelElement, runtimeElement) = this.TryCreateTestModelElement(typeof(ITestGenericService<>));

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(runtimeElement, modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_open_generic()
        {
            var (modelElement, runtimeElement) = this.TryCreateTestModelElement(typeof(ITestOpenGenericService<>));

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(runtimeElement, modelElement.Parts.First());
        }

        [SingletonAppServiceContract]
        public interface ISingletonTestAppService {}

        [AppServiceContract]
        public interface ITestAppService {}

        public interface ITestNonGenericService {}

        [AppServiceContract(ContractType = typeof(ITestNonGenericService))]
        public interface ITestGenericService<TParam> {}

        [AppServiceContract]
        public interface ITestGenericExportedService<TParam> {}

        [AppServiceContract(AsOpenGeneric = true)]
        public interface ITestOpenGenericService<TParam> {}
    }
}