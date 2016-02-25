// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelDimensionInfoFactoryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="ModelDimensionConstructor" />.
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

    using NUnit.Framework;

    using Telerik.JustMock;

    /// <summary>
    /// Tests for <see cref="ModelDimensionConstructor"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeModelDimensionInfoFactoryTest
    {
        [Test]
        public void TryCreateElement_ReturnType()
        {
            var factory = new ModelDimensionConstructor();
            var elementInfo = factory.TryCreateModelElement(Mock.Create<IRuntimeModelElementFactory>(), typeof(ITestDimension).GetTypeInfo());
            Assert.IsNotNull(elementInfo);
            Assert.IsInstanceOf<RuntimeModelDimensionInfo>(elementInfo);
        }

        [Test]
        public void TryCreateElement_DimensionSuffix()
        {
            var factory = new ModelDimensionConstructor();
            var elementInfo = factory.TryCreateModelElement(Mock.Create<IRuntimeModelElementFactory>(), typeof(ITestDimension).GetTypeInfo());
            Assert.AreEqual("Test", elementInfo.Name);
        }

        [Test]
        public void TryCreateElement_DimensionWithoutSuffix()
        {
            var factory = new ModelDimensionConstructor();
            var elementInfo = factory.TryCreateModelElement(Mock.Create<IRuntimeModelElementFactory>(), typeof(ITestDimensionWithoutSuffix).GetTypeInfo());
            Assert.AreEqual("TestDimensionWithoutSuffix", elementInfo.Name);
        }

        [Test]
        public void TryCreateElement_Index()
        {
            var factory = new ModelDimensionConstructor();
            var elementInfo = (IModelDimensionInfo)factory.TryCreateModelElement(Mock.Create<IRuntimeModelElementFactory>(), typeof(ITestDimensionWithoutSuffix).GetTypeInfo());
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