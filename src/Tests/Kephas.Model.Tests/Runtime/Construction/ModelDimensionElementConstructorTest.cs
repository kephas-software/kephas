// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionElementConstructorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="ModelDimensionElementConstructor" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Tests.Runtime.Construction.TestDim;
    using Kephas.Reflection;

    using NUnit.Framework;

    using Telerik.JustMock;

    /// <summary>
    /// Tests for <see cref="ModelDimensionElementConstructor"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class ModelDimensionElementConstructorTest : ConstructorTestBase
    {
        [Test]
        public void TryCreateModelElement_success()
        {
            var constructor = new ModelDimensionElementConstructor();
            var context = this.GetConstructionContext();
            var dimElement = constructor.TryCreateModelElement(context, typeof(IFirstTestDimDimensionElement).AsRuntimeTypeInfo());

            Assert.IsNotNull(dimElement);
            Assert.IsInstanceOf<ModelDimensionElement>(dimElement);

            var dimensionElement = (ModelDimensionElement)dimElement;
            Assert.AreEqual("First", dimElement.Name);
            Assert.AreEqual(":First", dimElement.QualifiedFullName);
            Assert.AreEqual("TestDim", dimensionElement.DimensionName);
        }

        [Test]
        public void TryCreateModelElement_Success_no_ending()
        {
            var constructor = new ModelDimensionElementConstructor();
            var context = this.GetConstructionContext();
            var dimElement = constructor.TryCreateModelElement(context, typeof(ISecondTestDim).AsRuntimeTypeInfo());

            Assert.AreEqual("Second", dimElement.Name);
        }

        [Test]
        public void TryCreateModelElement_Success_no_dimension_name_in_element_name()
        {
            var constructor = new ModelDimensionElementConstructor();
            var context = this.GetConstructionContext();
            var dimElement = constructor.TryCreateModelElement(context, typeof(IThirdDimensionElement).AsRuntimeTypeInfo());

            Assert.AreEqual("Third", dimElement.Name);
        }

        [Test]
        public void TryCreateModelElement_Success_no_dimension_name_in_namespace()
        {
            var constructor = new ModelDimensionElementConstructor();
            var context = this.GetConstructionContext();
            var dimElement = constructor.TryCreateModelElement(context, typeof(INoDimTestDimDimensionElement).AsRuntimeTypeInfo());

            Assert.AreEqual("NoDimTestDim", dimElement.Name);
        }
    }

    [ModelDimensionElement]
    public interface INoDimTestDimDimensionElement { }

    namespace TestDim
    {
        [ModelDimensionElement]
        public interface IFirstTestDimDimensionElement { }

        [ModelDimensionElement]
        public interface ISecondTestDim { }

        [ModelDimensionElement]
        public interface IThirdDimensionElement { }
    }
}
