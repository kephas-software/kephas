// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionFactoryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppDimensionBaseTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Factory
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements;
    using Kephas.Model.Factory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class ModelDimensionFactoryTest
    {
        [TestMethod]
        public void TryCreateElement_ReturnType()
        {
            var mockModelSpaceProvider = Mock.Create<IModelSpaceProvider>();
            var factory = new ModelDimensionFactory(mockModelSpaceProvider);
            var element = factory.TryCreateElement(typeof(ITestDimension).GetTypeInfo());
            Assert.IsInstanceOfType(element, typeof(ModelDimension));
        }

        [TestMethod]
        public void TryCreateElement_DimensionSuffix()
        {
            var mockModelSpaceProvider = Mock.Create<IModelSpaceProvider>();
            var factory = new ModelDimensionFactory(mockModelSpaceProvider);
            var element = factory.TryCreateElement(typeof(ITestDimension).GetTypeInfo());
            Assert.AreEqual("Test", element.Name);
        }

        [TestMethod]
        public void TryCreateElement_DimensionWithoutSuffix()
        {
            var mockModelSpaceProvider = Mock.Create<IModelSpaceProvider>();
            var factory = new ModelDimensionFactory(mockModelSpaceProvider);
            var element = factory.TryCreateElement(typeof(ITestDimensionWithoutSuffix).GetTypeInfo());
            Assert.AreEqual("TestDimensionWithoutSuffix", element.Name);
        }

        [TestMethod]
        public void TryCreateElement_Index()
        {
            var mockModelSpaceProvider = Mock.Create<IModelSpaceProvider>();
            var factory = new ModelDimensionFactory(mockModelSpaceProvider);
            var element = (IModelDimension)factory.TryCreateElement(typeof(ITestDimensionWithoutSuffix).GetTypeInfo());
            Assert.AreEqual(1, element.Index);
        }

        [AggregatableModelDimension(0)]
        private interface ITestDimension
        {
        }

        [ModelDimension(1)]
        private interface ITestDimensionWithoutSuffix
        {
        }
    }
}