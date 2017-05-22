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
        public void IsUnset_default()
        {
            lock (SyncObject)
            {
                Assert.IsTrue(Id.IsUnset(null));
                Assert.IsTrue(Id.IsUnset(0));
                Assert.IsTrue(Id.IsUnset(0L));
                Assert.IsTrue(Id.IsUnset(string.Empty));
                Assert.IsTrue(Id.IsUnset(Guid.Empty));
            }
        }

        [Test]
        public void IsUnset_custom()
        {
            lock (SyncObject)
            {
                var originalIsUnset = Id.IsUnset;
                Id.IsUnset = obj => obj == null || (obj is int && (int)obj <= 0);

                Assert.IsTrue(Id.IsUnset(0));
                Assert.IsTrue(Id.IsUnset(-1));

                Assert.IsFalse(Id.IsUnset(1));

                Id.IsUnset = originalIsUnset;
            }
        }

        [Test]
        public void IsUnset_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => Id.IsUnset = null);
        }
    }
}