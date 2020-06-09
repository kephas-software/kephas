// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Model
{
    using Kephas.Activation;
    using Kephas.Model;
    using Kephas.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class TypeExtensionsTest
    {
        private readonly RuntimeTypeRegistry typeRegistry;

        public TypeExtensionsTest()
        {
            this.typeRegistry = new RuntimeTypeRegistry();
        }

        [Test]
        public void GetAbstractType_none()
        {
            var expected = typeof(string);
            var actual = typeof(string).GetAbstractType();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void GetAbstractType_single()
        {
            var expected = typeof(string);
            var actual = typeof(TestImplementationType).GetAbstractType();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void GetAbstractType_multiple()
        {
            var expected = typeof(string);
            var actual = typeof(TestImplementationMultipleType).GetAbstractType();

            Assert.AreSame(expected, actual);
        }

        [ImplementationFor(typeof(string))]
        public class TestImplementationType {}

        [ImplementationFor(typeof(string), typeof(int))]
        public class TestImplementationMultipleType {}
    }
}