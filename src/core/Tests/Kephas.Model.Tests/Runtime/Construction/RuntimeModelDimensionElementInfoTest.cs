// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelDimensionElementInfoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="RuntimeModelDimensionElementInfo" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Model.Runtime.Construction;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    using TestModel.TestDim;

    /// <summary>
    /// Test class for <see cref="RuntimeModelDimensionElementInfo"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeModelDimensionElementInfoTest
    {
        [Test]
        public void Constructor_Success_DimensionElement_ending()
        {
            var info = new RuntimeModelDimensionElementInfo(typeof(IFirstTestDimDimensionElement).GetTypeInfo());

            Assert.AreEqual("First", info.Name);
            Assert.AreEqual("TestDim", info.DimensionName);

            var modelDimension = Mock.Create<IModelDimension>();
            modelDimension.Arrange(d => d.Name).Returns("TestDim");

            Assert.IsTrue(info.IsMemberOf(modelDimension));
            Assert.IsFalse(info.IsMemberOf(Mock.Create<IModelDimension>()));
            Assert.IsFalse(info.IsMemberOf(Mock.Create<IModelElement>()));
        }

        [Test]
        public void Constructor_Success_no_ending()
        {
            var info = new RuntimeModelDimensionElementInfo(typeof(ISecondTestDim).GetTypeInfo());

            Assert.AreEqual("Second", info.Name);
        }
    }
}

namespace TestModel.TestDim
{
    using Kephas.Model.AttributedModel;

    [ModelDimensionElement]
    public interface IFirstTestDimDimensionElement { }

    [ModelDimensionElement]
    public interface ISecondTestDim { }
}