// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the key attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Tests.AttributedModel
{
    using System;

    using Kephas.Data.Model.AttributedModel;

    using NUnit.Framework;

    [TestFixture]
    public class KeyAttributeTest
    {
        [Test]
        public void KeyAttribute()
        {
            var attr = new KeyAttribute("key-name", new[] { "prop-1" });

            Assert.AreEqual(KeyKind.Default, attr.Kind);
            Assert.AreEqual("key-name", attr.Name);
        }

        [Test]
        public void KeyAttribute_invalid_properties()
        {
            Assert.Throws<ArgumentNullException>(() => new KeyAttribute("key-name", null));
            Assert.Throws<ArgumentException>(() => new KeyAttribute("key-name", new string[0]));
        }
    }
}