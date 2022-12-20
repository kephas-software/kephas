// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelperTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="ReflectionHelper" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;

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

        [Test]
        public void GetPropertyName()
        {
            var propName = ReflectionHelper.GetPropertyName<ITypeInfo>(t => t.Name);
            Assert.AreEqual(nameof(ITypeInfo.Name), propName);
        }
    }
}