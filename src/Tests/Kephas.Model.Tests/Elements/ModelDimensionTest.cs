// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="ModelDimension" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Elements
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Model.Elements;
    using Kephas.Model.Elements.Construction;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    /// <summary>
    /// Test class for <see cref="ModelDimension"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class ModelDimensionTest
    {
        [Test]
        public void Constructor_Success()
        {
            var modelSpace = Mock.Create<IModelSpace>();

            var elementInfo = Mock.Create<IModelDimensionInfo>();
            elementInfo.Arrange(e => e.Index).Returns(12);
            elementInfo.Arrange(e => e.Name).Returns("Hello");
            elementInfo.Arrange(e => e.IsAggregatable).Returns(true);

            var element = new ModelDimension(elementInfo, modelSpace);

            Assert.AreEqual("Hello", element.Name);
            Assert.AreEqual("^Hello", element.QualifiedName);
            Assert.AreEqual(12, element.Index);
            Assert.AreEqual(true, element.IsAggregatable);
        }
    }
}