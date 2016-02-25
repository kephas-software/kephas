// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionConstructorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Model.Factory;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;

    using NUnit.Framework;

    using Telerik.JustMock;

    /// <summary>
    /// Test class for <see cref="ModelDimensionConstructor"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class ModelDimensionConstructorTest
    {
        [Test]
        public void TryCreateModelElement_Success()
        {
            var modelSpace = Mock.Create<IModelSpace>();

            var context = new ModelConstructionContext { ModelSpace = modelSpace };
            var constructor = new ModelDimensionConstructor();
            var element = constructor.TryCreateModelElement(context, typeof(IHelloDimension).AsDynamicTypeInfo());

            Assert.AreEqual("Hello", element.Name);
            Assert.AreEqual("^Hello", element.QualifiedName);
            Assert.IsInstanceOf<IModelDimension>(element);

            var dimension = (IModelDimension)element;
            Assert.AreEqual(12, dimension.Index);
            Assert.AreEqual(true, dimension.IsAggregatable);
        }

        [AggregatableModelDimension(12, DefaultDimensionElement = typeof(IGlobalScopeDimensionElement))]
        private interface IHelloDimension { }
    }
}