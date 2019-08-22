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
    using Kephas.Reflection;
    using Kephas.Services;

    using NUnit.Framework;

    [TestFixture]
    public class AppServiceConstructorTest : ConstructorTestBase
    {
        private INamedElement TryCreateTestModelElement<TService>()
        {
            return TryCreateTestModelElement(typeof(TService));
        }

        private INamedElement TryCreateTestModelElement(Type serviceType)
        {
            var constructor = new AppServiceTypeConstructor();
            var context = this.GetConstructionContext();
            var runtimeElement = serviceType.AsRuntimeTypeInfo();
            var modelElement = constructor.TryCreateModelElement(context, runtimeElement);

            return modelElement;
        }

        [Test]
        public void TryCreateModelElement_singleton_app_service()
        {
            var modelElement = this.TryCreateTestModelElement<ISingletonTestAppService>();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ISingletonTestAppService).AsRuntimeTypeInfo(), modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_app_service()
        {
            var modelElement = this.TryCreateTestModelElement<ITestAppService>();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ITestAppService).AsRuntimeTypeInfo(), modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_generic_exported_app_service()
        {
            var modelElement = this.TryCreateTestModelElement(typeof(ITestGenericExportedService<>));

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ITestGenericExportedService<>).AsRuntimeTypeInfo(), modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_generic_exported_as_non_generic()
        {
            var modelElement = this.TryCreateTestModelElement(typeof(ITestGenericService<>));

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ITestGenericService<>).AsRuntimeTypeInfo(), modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_open_generic()
        {
            var modelElement = this.TryCreateTestModelElement(typeof(ITestOpenGenericService<>));

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppServiceType>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ITestOpenGenericService<>).AsRuntimeTypeInfo(), modelElement.Parts.First());
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