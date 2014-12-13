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

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// Test class for <see cref="ModelDimension"/>.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class ModelDimensionTest
    {
        [TestMethod]
        public void Constructor_Success()
        {
            var modelSpace = Mock.Create<IModelSpace>();
            var element = new ModelDimension(modelSpace, "Hello", 12, true);
            Assert.AreEqual("Hello", element.Name);
            Assert.AreEqual("^Hello", element.QualifiedName);
            Assert.AreEqual(12, element.Index);
            Assert.AreEqual(true, element.IsAggregatable);
        }
    }
}