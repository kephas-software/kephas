// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelDimensionElementInfoFactoryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="RuntimeModelDimensionElementInfoFactory" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Factory
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Factory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="RuntimeModelDimensionElementInfoFactory"/>.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeModelDimensionElementInfoFactoryTest
    {
        [TestMethod]
        public void TryCreateElement_ReturnType()
        {
            var factory = new RuntimeModelDimensionElementInfoFactory();
            var elementInfo = factory.TryGetElementInfo(typeof(IFirstTestDimDimensionElement).GetTypeInfo());
            Assert.IsNotNull(elementInfo);
            Assert.IsInstanceOfType(elementInfo, typeof(RuntimeModelDimensionElementInfo));
        }

        [ModelDimensionElement]
        public interface IFirstTestDimDimensionElement { }
    }
}
