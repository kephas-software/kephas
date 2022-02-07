// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignatureTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the signature test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class SignatureTest
    {
        [Test]
        public void Equals_this()
        {
            var s = new Signature();
            Assert.IsTrue(s.Equals(s));
        }

        [Test]
        public void Equals_empty()
        {
            var s = new Signature();
            var s2 = new Signature();
            Assert.IsTrue(s.Equals(s2));
        }

        [Test]
        public void Equals_same_types()
        {
            var s = new Signature(typeof(string), typeof(int));
            var s2 = new Signature(typeof(string), typeof(int));
            Assert.IsTrue(s.Equals(s2));
        }

        [Test]
        public void Equals_different_types()
        {
            var s = new Signature(typeof(string), typeof(long));
            var s2 = new Signature(typeof(string), typeof(int));
            Assert.IsFalse(s.Equals(s2));
        }

        [Test]
        public void ToString_int_string()
        {
            var s = new Signature(typeof(int), typeof(string));
            Assert.AreEqual("(Int32, String)", s.ToString());
        }

        [Test]
        public void ToString_Signature_nullablelong_null()
        {
            var s = new Signature(typeof(Signature), typeof(long?), null);
            Assert.AreEqual("(Signature, Int64?, )", s.ToString());
        }

        [Test]
        public void GetEnumerator_type()
        {
            var s = new Signature(typeof(Signature), typeof(bool), typeof(int?));
            var e = s.GetEnumerator();
            e.MoveNext();
            Assert.AreSame(typeof(Signature), e.Current);
            e.MoveNext();
            Assert.AreSame(typeof(bool), e.Current);
            e.MoveNext();
            Assert.AreSame(typeof(int?), e.Current);
            Assert.IsFalse(e.MoveNext());
        }
    }
}
