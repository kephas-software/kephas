namespace Kephas.Model.Tests.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements;
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultModelSpaceTest
    {
        [Test]
        public void ComputeDimensions_2_dims()
        {
            var context = new ModelConstructionContext();
            var modelSpace = new DefaultModelSpace(context);
            context.ModelSpace = modelSpace;

            var elementInfos = new List<IElementInfo>();
            elementInfos.AddRange(this.CreateModelDimension(context, "D1", isAggregatable: false, index: 0, dimensionElements: new [] { "E1", "E2" }));
            elementInfos.AddRange(this.CreateModelDimension(context, "D2", isAggregatable: true, index: 1, dimensionElements: new[] { "F1", "F2" }));

            context.ElementInfos = elementInfos;

            var dimensions = modelSpace.ComputeDimensions(context);
            Assert.AreEqual(2, dimensions.Length);
            Assert.AreEqual(2, dimensions[0].Elements.Count());
            Assert.AreEqual(2, dimensions[1].Elements.Count());

            Assert.IsTrue(dimensions.Cast<IWritableNamedElement>().All(d => d.ConstructionState.IsCompletedSuccessfully));
            Assert.IsTrue(dimensions.SelectMany(d => d.Elements).Cast<IWritableNamedElement>().All(d => d.ConstructionState.IsCompletedSuccessfully));
        }

        [Test]
        public void ComputeProjections_2_dims()
        {
            var context = new ModelConstructionContext();
            var modelSpace = new DefaultModelSpace(context);
            context.ModelSpace = modelSpace;

            var elementInfos = new List<IElementInfo>();
            elementInfos.AddRange(this.CreateModelDimension(context, "D1", isAggregatable: false, index: 0, dimensionElements: new[] { "E1", "E2" }));
            elementInfos.AddRange(this.CreateModelDimension(context, "D2", isAggregatable: true, index: 1, dimensionElements: new[] { "F1", "F2" }));

            context.ElementInfos = elementInfos;

            var dimensions = modelSpace.ComputeDimensions(context);
            var projections = modelSpace.ComputeProjections(context, dimensions);

            Assert.AreEqual(6, projections.Count);
            Assert.IsTrue(projections.Cast<IWritableNamedElement>().All(d => d.ConstructionState.IsCompletedSuccessfully));
        }

        /// <summary>
        /// Enumerates create model dimension in this collection.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dimensionName">Name of the dimension.</param>
        /// <param name="isAggregatable">true if this object is aggregatable.</param>
        /// <param name="index">Zero-based index of the dimension.</param>
        /// <param name="dimensionElements">The dimension elements.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process create model dimension in this
        /// collection.
        /// </returns>
        private IList<IElementInfo> CreateModelDimension(IModelConstructionContext context, string dimensionName, bool isAggregatable, int index, IEnumerable<string> dimensionElements)
        {
            var elementInfos = new List<IElementInfo>();
            var dimension = new ModelDimension(context, dimensionName)
            {
                IsAggregatable = isAggregatable,
                Index = index
            };
            elementInfos.Add(dimension);

            foreach (var elementName in dimensionElements)
            {
                var element = new ModelDimensionElement(context, elementName) { DimensionName = dimensionName };
                elementInfos.Add(element);
            }

            return elementInfos;
        }
    }
}
