// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaturalKeyAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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