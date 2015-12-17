// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelperTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="ReflectionHelper" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="ReflectionHelper"/>.
    /// </summary>
    [TestFixture]
    public class ReflectionHelperTest
    {
        [Test]
        public void GetNonGenericName_non_generic()
        {
            var name = ReflectionHelper.GetNonGenericFullName(typeof(string));
            Assert.AreEqual("System.String", name);
        }

        [Test]
        public void GetNonGenericName_generic()
        {
            var name = ReflectionHelper.GetNonGenericFullName(typeof(IEnumerable<>));
            Assert.AreEqual("System.Collections.Generic.IEnumerable", name);
        }
    }
}