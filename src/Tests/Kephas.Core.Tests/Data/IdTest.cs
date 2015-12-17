// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="Id" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Data
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Data;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="Id"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class IdTest
    {
        private static readonly object SyncObject = new object();

        [Test]
        public void Id_success()
        {
            var id = new Id(2);
            Assert.AreEqual(2, id.Value);

            id = new Id(15L);
            Assert.AreEqual(15L, id.Value);

            var guid = Guid.NewGuid();
            id = new Id(guid);
            Assert.AreEqual(guid, id.Value);

            id = new Id("hallo");
            Assert.AreEqual("hallo", id.Value);

            var someObject = new object();
            id = new Id(someObject);
            Assert.AreEqual(someObject, id.Value);
        }

        [Test]
        public void Id_unset()
        {
            var id = new Id(0);
            Assert.AreEqual(null, id.Value);

            id = new Id(0L);
            Assert.AreEqual(null, id.Value);

            id = new Id(Guid.Empty);
            Assert.AreEqual(null, id.Value);

            id = new Id(string.Empty);
            Assert.AreEqual(null, id.Value);

            id = new Id(null);
            Assert.AreEqual(null, id.Value);

            id = new Id(Undefined.Value);
            Assert.AreEqual(null, id.Value);
        }

        [Test]
        public void ToString_id()
        {
            var id = new Id("some string");
            var actual = id.ToString();
            Assert.AreEqual("id(some string)", actual);
        }

        [Test]
        public void Equals_int_Success()
        {
            var id1 = new Id(2);
            var id2 = new Id(2);

            Assert.AreEqual(id1, id2);
        }

        [Test]
        public void Equals_different_types_Failure()
        {
            var id1 = new Id(2);
            var id2 = new Id("2");

            Assert.AreNotEqual(id1, id2);
        }

        [Test]
        public void IsUnset_default()
        {
            lock (SyncObject)
            {
                Assert.IsTrue(new Id().IsUnset);
                Assert.IsTrue(new Id(null).IsUnset);
                Assert.IsTrue(new Id(Undefined.Value).IsUnset);
                Assert.IsTrue(new Id(0).IsUnset);
                Assert.IsTrue(new Id(0L).IsUnset);
                Assert.IsTrue(new Id(string.Empty).IsUnset);
                Assert.IsTrue(new Id(Guid.Empty).IsUnset);
            }
        }

        [Test]
        public void IsUnsetValue_custom()
        {
            lock (SyncObject)
            {
                var originalIsUnsetValue = Id.IsUnsetValue;
                Id.IsUnsetValue = obj => obj == null || (obj is int && (int)obj == 0);

                var zero = new Id(0);
                Assert.IsTrue(zero.IsUnset);

                var one = new Id(1);
                Assert.IsFalse(one.IsUnset);

                Id.IsUnsetValue = originalIsUnsetValue;
            }
        }

        [Test]
        public void IsUnsetValue_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => Id.IsUnsetValue = null);
        }
    }
}