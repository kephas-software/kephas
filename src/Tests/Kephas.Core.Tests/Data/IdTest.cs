// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        public void IsEmpty_default()
        {
            lock (SyncObject)
            {
                Assert.IsTrue(Id.IsEmpty(null));
                Assert.IsTrue(Id.IsEmpty(0));
                Assert.IsTrue(Id.IsEmpty(0L));
                Assert.IsTrue(Id.IsEmpty(string.Empty));
                Assert.IsTrue(Id.IsEmpty(Guid.Empty));
            }
        }

        [Test]
        public void IsEmpty_custom()
        {
            lock (SyncObject)
            {
                var originalIsEmpty = Id.IsEmpty;
                Id.IsEmpty = obj => obj == null || (obj is int && (int)obj <= 0);

                Assert.IsTrue(Id.IsEmpty(0));
                Assert.IsTrue(Id.IsEmpty(-1));

                Assert.IsFalse(Id.IsEmpty(1));

                Id.IsEmpty = originalIsEmpty;
            }
        }

        [Test]
        public void IsEmpty_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => Id.IsEmpty = null);
        }

        [Test]
        public void IsTemporary_default()
        {
            lock (SyncObject)
            {
                Assert.IsFalse(Id.IsTemporary(null));
                Assert.IsFalse(Id.IsTemporary(0));
                Assert.IsFalse(Id.IsTemporary(0L));
                Assert.IsFalse(Id.IsTemporary(string.Empty));
                Assert.IsFalse(Id.IsTemporary(Guid.Empty));

                Assert.IsTrue(Id.IsTemporary(-1));
                Assert.IsTrue(Id.IsTemporary(-5L));
            }
        }
    }
}