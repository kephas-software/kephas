// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="ObjectExtensions" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Extensions
{
    using System.Collections.Generic;
    using System.Dynamic;

    using Kephas.Extensions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="ObjectExtensions"/>
    /// </summary>
    [TestFixture]
    public class ObjectExtensionsTest
    {
        [Test]
        public void ToDynamic_dynamic()
        {
            var obj = new ExpandoObject();
            var dyn = obj.ToDynamic();
            Assert.AreSame(obj, dyn);
        }

        [Test]
        public void ToDynamic_non_dynamic()
        {
            var obj = new List<string>();
            var dyn = obj.ToDynamic();
            Assert.AreNotSame(obj, dyn);

            dyn.Add("John");
            Assert.AreEqual(1, obj.Count);
            Assert.IsTrue(obj.Contains("John"));
        }

        [Test]
        public void ToDynamic_null()
        {
            var dyn = ((object)null).ToDynamic();
            Assert.IsNull(dyn);
        }
    }
}