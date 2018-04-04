// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionConstructorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="ModelDimensionConstructor" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Dimensions.Scope;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="ModelDimensionConstructor"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class ModelDimensionConstructorTest : ConstructorTestBase
    {
        [Test]
        public void TryCreateModelElement_Success()
        {
            var constructor = new ModelDimensionConstructor();
            var context = this.GetConstructionContext();
            var element = constructor.TryCreateModelElement(context, typeof(IHelloDimension).AsRuntimeTypeInfo());

            Assert.AreEqual("Hello", element.Name);
            Assert.AreEqual("^Hello", element.QualifiedFullName);
            Assert.IsInstanceOf<IModelDimension>(element);

            var dimension = (IModelDimension)element;
            Assert.AreEqual(12, dimension.Index);
        }

        [Test]
        public void TryCreateModelElement_Success_no_ending()
        {
            var constructor = new ModelDimensionConstructor();
            var context = this.GetConstructionContext();
            var dimElement = constructor.TryCreateModelElement(context, typeof(IDim2).AsRuntimeTypeInfo());

            Assert.AreEqual("Dim2", dimElement.Name);
        }

        [Test]
        public void TryCreateModelElement_Success_aggregatable()
        {
            var constructor = new ModelDimensionConstructor();
            var context = this.GetConstructionContext();
            var dimElement = (IModelDimension)constructor.TryCreateModelElement(context, typeof(IDim2).AsRuntimeTypeInfo());

            Assert.AreEqual(true, dimElement.IsAggregatable);
        }

        [ModelDimension(12, DefaultDimensionElement = typeof(IGlobalScopeDimensionElement))]
        private interface IHelloDimension { }

        [ModelDimension(10)]
        public interface IDim1Dimension { }

        [AggregatableModelDimension(24)]
        public interface IDim2 { }
    }
}