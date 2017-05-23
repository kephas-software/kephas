// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaturalKeyAttributeTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the natural key attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Tests.AttributedModel
{
    using Kephas.Data.Model.AttributedModel;

    using NUnit.Framework;

    [TestFixture]
    public class NaturalKeyAttributeTest
    {
        [Test]
        public void NaturalKeyAttribute()
        {
            var attr = new NaturalKeyAttribute("key-name", new[] { "prop-1" });
            Assert.AreEqual(KeyKind.Natural, attr.Kind);
        }
    }
}