// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimaryKeyAttributeTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the primary key attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Tests.AttributedModel
{
    using Kephas.Data.Model.AttributedModel;

    using NUnit.Framework;

    [TestFixture]
    public class PrimaryKeyAttributeTest
    {
        [Test]
        public void PrimaryKeyAttribute()
        {
            var attr = new PrimaryKeyAttribute("key-attr", new[] { "prop-1" });
            Assert.AreEqual(KeyKind.Primary, attr.Kind);
        }
    }
}