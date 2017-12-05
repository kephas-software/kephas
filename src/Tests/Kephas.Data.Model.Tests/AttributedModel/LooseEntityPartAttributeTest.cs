// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LooseEntityPartAttributeTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the loose entity part attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Tests.AttributedModel
{
    using Kephas.Data.Model.AttributedModel;

    using NUnit.Framework;

    [TestFixture]
    public class LooseEntityPartAttributeTest
    {
        [Test]
        public void Constructor()
        {
            var attr = new LooseEntityPartAttribute();
            Assert.AreEqual(EntityPartKind.Loose, attr.Kind);
        }
    }
}