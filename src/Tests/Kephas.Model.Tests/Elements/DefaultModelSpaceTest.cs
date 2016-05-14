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

            var dimDictionary = dimensions.ToDictionary(d => d.Name, d => d);
            var elemDictionary = dimensions.SelectMany(d => d.Elements).ToDictionary(e => e.Name, e => e);
            Assert.AreSame(elemDictionary["E1"].DeclaringDimension, dimDictionary["D1"]);
            Assert.AreSame(elemDictionary["E2"].DeclaringDimension, dimDictionary["D1"]);
            Assert.AreSame(elemDictionary["F1"].DeclaringDimension, dimDictionary["D2"]);
            Assert.AreSame(elemDictionary["F2"].DeclaringDimension, dimDictionary["D2"]);
            Assert.AreEqual("^D1:E1", elemDictionary["E1"].FullName);
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

            var projDictionary = projections.ToDictionary(p => p.Name, p => p);
            Assert.IsFalse(projDictionary[":E1:F1"].IsAggregated);
            Assert.IsFalse(projDictionary[":E1:F2"].IsAggregated);
            Assert.IsFalse(projDictionary[":E2:F1"].IsAggregated);
            Assert.IsFalse(projDictionary[":E2:F2"].IsAggregated);
            Assert.IsTrue(projDictionary[":E1"].IsAggregated);
            Assert.IsTrue(projDictionary[":E2"].IsAggregated);
            Assert.AreSame(projDictionary[":E1:F1"].AggregatedProjection, projDictionary[":E1"]);
            Assert.AreSame(projDictionary[":E1:F2"].AggregatedProjection, projDictionary[":E1"]);
            Assert.AreSame(projDictionary[":E2:F1"].AggregatedProjection, projDictionary[":E2"]);
            Assert.AreSame(projDictionary[":E2:F2"].AggregatedProjection, projDictionary[":E2"]);
            Assert.AreEqual(":E2:F2", projDictionary[":E2:F2"].FullName);
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
