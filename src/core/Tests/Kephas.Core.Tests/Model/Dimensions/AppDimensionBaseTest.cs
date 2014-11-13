// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDimensionBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppDimensionBaseTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Model.Dimensions
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Model.Dimensions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AppDimensionBaseTest
    {
        [TestMethod]
        public void Name_DimensionSuffix()
        {
            var dim = new TestDimension();
            Assert.AreEqual("Test", dim.Name);
        }

        [TestMethod]
        public void Name_DimensionWithoutSuffix()
        {
            var dim = new TestDimensionWithoutSuffix();
            Assert.AreEqual("TestDimensionWithoutSuffix", dim.Name);
        }

        private class TestDimension : AppDimensionBase
        {
            public override bool IsAggregatable
            {
                get
                {
                    return true;
                }
            }
        }

        private class TestDimensionWithoutSuffix : AppDimensionBase
        {
            public override bool IsAggregatable
            {
                get
                {
                    return false;
                }
            }
        }
    }
}