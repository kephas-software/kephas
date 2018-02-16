// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceConstructorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
            var constructor = new AppServiceConstructor();
            var context = this.GetConstructionContext();
            var runtimeElement = serviceType.AsRuntimeTypeInfo();
            var modelElement = constructor.TryCreateModelElement(context, runtimeElement);

            return modelElement;
        }

        [Test]
        public void TryCreateModelElement_shared_app_service()
        {
            var modelElement = this.TryCreateTestModelElement<ISharedTestAppService>();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppService>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ISharedTestAppService).AsRuntimeTypeInfo(), modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_app_service()
        {
            var modelElement = this.TryCreateTestModelElement<ITestAppService>();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppService>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ITestAppService).AsRuntimeTypeInfo(), modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_generic_exported_app_service()
        {
            var modelElement = this.TryCreateTestModelElement(typeof(ITestGenericExportedService<>));

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppService>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ITestGenericExportedService<>).AsRuntimeTypeInfo(), modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_generic_exported_as_non_generic()
        {
            var modelElement = this.TryCreateTestModelElement(typeof(ITestGenericService<>));

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppService>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ITestGenericService<>).AsRuntimeTypeInfo(), modelElement.Parts.First());
        }

        [Test]
        public void TryCreateModelElement_open_generic()
        {
            var modelElement = this.TryCreateTestModelElement(typeof(ITestOpenGenericService<>));

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<AppService>(modelElement);
            Assert.AreEqual(1, modelElement.Parts.Count());
            Assert.AreSame(typeof(ITestOpenGenericService<>).AsRuntimeTypeInfo(), modelElement.Parts.First());
        }

        [SharedAppServiceContract]
        public interface ISharedTestAppService {}


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