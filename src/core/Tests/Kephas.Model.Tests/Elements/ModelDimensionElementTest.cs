// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionElementTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="ModelDimensionElement" />.
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
    /// Test class for <see cref="ModelDimensionElement"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class ModelDimensionElementTest
    {
        [Test]
        public void Constructor_Success()
        {
            var modelSpace = Mock.Create<IModelSpace>();

            var elementInfo = Mock.Create<IModelDimensionElementInfo>();
            elementInfo.Arrange(e => e.Name).Returns("One");
            elementInfo.Arrange(e => e.DimensionName).Returns("Layer");

            var element = new ModelDimensionElement(elementInfo, modelSpace);

            Assert.AreEqual("One", element.Name);
            Assert.AreEqual(":One", element.QualifiedName);
        }
    }
}