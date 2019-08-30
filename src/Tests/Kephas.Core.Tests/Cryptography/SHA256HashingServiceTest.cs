// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullHashingServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null hashing service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Cryptography
{
    using System;
    using System.Text;

    using Kephas.Composition;
    using Kephas.Cryptography;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class SHA256HashingServiceTest
    {
        [Test]
        [TestCase("other", "2SmKENGwc1g33EvYXaxkGw887yekfl1TpU8vP1svz/o=")]
        public void Hash(string value, string hash)
        {
            var hashingService = new SHA256HashingService();
            var valueBytes = Encoding.UTF8.GetBytes(value);
            var hashBytes = hashingService.Hash(valueBytes);
            var hashString = Convert.ToBase64String(hashBytes);
            Assert.AreEqual(hash, hashString);
        }

        [Test]
        [TestCase("other", "salt", "pBbAk+s59J5HOdOZfsuULq8hEgjkBKChn/vhh++mCwE=")]
        public void Hash_with_salt(string value, string salt, string hash)
        {
            var hashingService = new SHA256HashingService();
            var valueBytes = Encoding.UTF8.GetBytes(value);
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            var hashBytes = hashingService.Hash(valueBytes, new HashingContext(Substitute.For<ICompositionContext>()) { Salt = saltBytes });
            var hashString = Convert.ToBase64String(hashBytes);
            Assert.AreEqual(hash, hashString);
        }
    }
}