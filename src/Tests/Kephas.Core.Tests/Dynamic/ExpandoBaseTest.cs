// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the expando base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Dynamic
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class ExpandoBaseTest
    {
        [Test]
        public void Constructor_same_object_and_dictionary()
        {
            var dict = new Dictionary<string, object>();
            dynamic expando = new TestExpando(dict, dict);
            Assert.IsNull(expando.Count);
        }
    }

    public class TestExpando : ExpandoBase
    {
        public TestExpando(object inner, IDictionary<string, object> innerDictionary)
            : base(inner, innerDictionary)
        {
        }
    }
}