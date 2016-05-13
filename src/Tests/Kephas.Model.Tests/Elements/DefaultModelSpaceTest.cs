namespace Kephas.Model.Tests.Elements
{
    using System.Linq;

    using Kephas.Model.Construction;
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

            var dim1 = new ModelDimension(context, "D1");
            dim1.IsAggregatable = false;
            dim1.Index = 0;

            var dim1e1 = new ModelDimensionElement(context, "E1");
            dim1e1.DimensionName = "D1";

            var dim1e2 = new ModelDimensionElement(context, "E2");
            dim1e2.DimensionName = "D1";
            
            var dim2 = new ModelDimension(context, "D2");
            dim2.IsAggregatable = true;
            dim2.Index = 1;

            var dim2e1 = new ModelDimensionElement(context, "F1");
            dim2e1.DimensionName = "D2";

            var dim2e2 = new ModelDimensionElement(context, "F2");
            dim2e2.DimensionName = "D2";

            context.ElementInfos = new IElementInfo[] { dim1, dim2, dim1e1, dim1e2, dim2e1, dim2e2 };

            var dimensions = modelSpace.ComputeDimensions(context);
            Assert.AreEqual(2, dimensions.Length);
            Assert.AreEqual(2, dimensions[0].Elements.Count());
            Assert.AreEqual(2, dimensions[1].Elements.Count());
        }

        [Test]
        public void ComputeProjections_2_dims()
        {
            var context = new ModelConstructionContext();
            var modelSpace = new DefaultModelSpace(context);
            context.ModelSpace = modelSpace;

            var dim1 = new ModelDimension(context, "D1");
            dim1.IsAggregatable = false;
            dim1.Index = 0;

            var dim1e1 = new ModelDimensionElement(context, "E1");
            dim1e1.DimensionName = "D1";

            var dim1e2 = new ModelDimensionElement(context, "E2");
            dim1e2.DimensionName = "D1";

            var dim2 = new ModelDimension(context, "D2");
            dim2.IsAggregatable = true;
            dim2.Index = 1;

            var dim2e1 = new ModelDimensionElement(context, "F1");
            dim2e1.DimensionName = "D2";

            var dim2e2 = new ModelDimensionElement(context, "F2");
            dim2e2.DimensionName = "D2";

            context.ElementInfos = new IElementInfo[] { dim1, dim2, dim1e1, dim1e2, dim2e1, dim2e2 };

            var dimensions = modelSpace.ComputeDimensions(context);
            var projections = modelSpace.ComputeProjections(context, dimensions);
            Assert.AreEqual(6, projections.Count);
        }
    }
}
