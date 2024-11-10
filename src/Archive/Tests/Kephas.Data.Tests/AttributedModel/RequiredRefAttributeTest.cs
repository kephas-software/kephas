// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredRefAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the required reference attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.AttributedModel
{
    using System;

    using Kephas.Data.AttributedModel;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class RequiredRefAttributeTest
    {
        [Test]
        public void Validate_value()
        {
            var attr = new RequiredRefAttribute();
            Assert.IsFalse(attr.IsValid(0));
            Assert.IsTrue(attr.IsValid(-1));
            Assert.IsFalse(attr.IsValid(null));
            Assert.IsFalse(attr.IsValid(Guid.Empty));
        }

        [Test]
        public void Validate_ref()
        {
            var r = Substitute.For<IRef>();
            var attr = new RequiredRefAttribute();

            r.IsEmpty.Returns(true);
            Assert.IsFalse(attr.IsValid(r));

            r.IsEmpty.Returns(false);
            Assert.IsTrue(attr.IsValid(r));
        }
    }
}