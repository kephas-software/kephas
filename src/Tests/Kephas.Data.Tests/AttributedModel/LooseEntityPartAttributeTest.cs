// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LooseEntityPartAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the loose entity part attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.AttributedModel
{
    using Kephas.Data.AttributedModel;

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