// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelDimensionInfoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="RuntimeModelDimensionInfo" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Model.Runtime.Construction;

    using NUnit.Framework;

    using TestModel.TestDim;

    /// <summary>
    /// Test class for <see cref="RuntimeModelDimensionInfo"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeModelDimensionInfoTest
    {
        [Test]
        public void Constructor_Success_Dimension_ending()
        {
            var info = new RuntimeModelDimensionInfo(typeof(IDim1Dimension).GetTypeInfo());

            Assert.AreEqual("Dim1", info.Name);
            Assert.AreEqual(12, info.Index);
            Assert.AreEqual(false, info.IsAggregatable);
        }

        [Test]
        public void Constructor_Success_no_ending()
        {
            var info = new RuntimeModelDimensionInfo(typeof(IDim2).GetTypeInfo());

            Assert.AreEqual("Dim2", info.Name);
            Assert.AreEqual(24, info.Index);
            Assert.AreEqual(true, info.IsAggregatable);
        }
    }
}

namespace TestModel.TestDim
{
    using Kephas.Model.AttributedModel;

    [ModelDimension(12)]
    public interface IDim1Dimension { }

    [AggregatableModelDimension(24)]
    public interface IDim2 { }
}