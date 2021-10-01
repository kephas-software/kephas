// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicObjectExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="DynamicObjectExtensions" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="DynamicObjectExtensions"/>
    /// </summary>
    [TestFixture]
    public class DynamicObjectExtensionsTest
    {
        [Test]
        public void ToDynamic_dynamic()
        {
            var obj = new ExpandoObject();
            var dyn = obj.ToDynamicObject();
            Assert.AreSame(obj, dyn);
        }

        [Test]
        public void ToDynamic_non_dynamic_method()
        {
            var obj = new List<string>();
            var dyn = obj.ToDynamicObject();
            Assert.AreNotSame(obj, dyn);

            dyn.Add("John");
            Assert.AreEqual(1, obj.Count);
            Assert.IsTrue(obj.Contains("John"));
        }

        [Test]
        public void ToDynamic_non_dynamic_property()
        {
            var obj = new List<string> { "one", "two" };
            var dyn = obj.ToDynamicObject();
            Assert.AreNotSame(obj, dyn);

            Assert.AreEqual(2, dyn.Count);
        }

        [Test]
        public void ToDynamic_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => ((object)null).ToDynamicObject());
        }
    }
}