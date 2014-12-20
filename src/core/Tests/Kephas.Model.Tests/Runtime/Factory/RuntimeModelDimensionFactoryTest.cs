// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelDimensionFactoryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="RuntimeModelDimensionInfoFactory" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Factory
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Factory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="RuntimeModelDimensionInfoFactory"/>.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeModelDimensionFactoryTest
    {
        [TestMethod]
        public void TryCreateElement_ReturnType()
        {
            var factory = new RuntimeModelDimensionInfoFactory();
            var elementInfo = factory.TryGetElementInfo(typeof(ITestDimension).GetTypeInfo());
            Assert.IsNotNull(elementInfo);
            Assert.IsInstanceOfType(elementInfo, typeof(RuntimeModelDimensionInfo));
        }

        [TestMethod]
        public void TryCreateElement_DimensionSuffix()
        {
            var factory = new RuntimeModelDimensionInfoFactory();
            var elementInfo = factory.TryGetElementInfo(typeof(ITestDimension).GetTypeInfo());
            Assert.AreEqual("Test", elementInfo.Name);
        }

        [TestMethod]
        public void TryCreateElement_DimensionWithoutSuffix()
        {
            var factory = new RuntimeModelDimensionInfoFactory();
            var elementInfo = factory.TryGetElementInfo(typeof(ITestDimensionWithoutSuffix).GetTypeInfo());
            Assert.AreEqual("TestDimensionWithoutSuffix", elementInfo.Name);
        }

        [TestMethod]
        public void TryCreateElement_Index()
        {
            var factory = new RuntimeModelDimensionInfoFactory();
            var elementInfo = (IModelDimensionInfo)factory.TryGetElementInfo(typeof(ITestDimensionWithoutSuffix).GetTypeInfo());
            Assert.AreEqual(1, elementInfo.Index);
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