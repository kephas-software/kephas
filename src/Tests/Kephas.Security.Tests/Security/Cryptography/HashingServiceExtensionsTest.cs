// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingServiceExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the hashing service extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Cryptography
{
    using System;
    using System.Text;

    using Kephas.Cryptography;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class HashingServiceExtensionsTest
    {
        [Test]
        public void Hash_string()
        {
            var valueString = "123";
            Action<IHashingContext> optionsConfig = ctx => { };
            var hashBytes = Encoding.UTF8.GetBytes("8910");

            IHashingService hashingService = new TestHashingService((value, options) =>
            {
                var actualStringValue = Encoding.UTF8.GetString(value);
                return actualStringValue == valueString ? hashBytes : null;
            });

            var hash = hashingService.Hash(valueString, optionsConfig);
            var hashString = Encoding.UTF8.GetString(hash);
            Assert.AreEqual("8910", hashString);
        }

        [Test]
        public void Hash_string_salt()
        {
            var valueString = "123";
            var salt = new byte[] { 1, 2, 3 };
            var hashBytes = Encoding.UTF8.GetBytes("8910");

            IHashingService hashingService = new TestHashingService((value, options) =>
            {
                var ctx = Substitute.For<IHashingContext>();
                options?.Invoke(ctx);
                ctx.Received(1).Salt = salt;
                var actualValue = Encoding.UTF8.GetString(value);
                return actualValue == valueString ? hashBytes : Array.Empty<byte>();
            });

            var hash = hashingService.Hash(valueString, salt);
            var hashString = Encoding.UTF8.GetString(hash);
            Assert.AreEqual("8910", hashString);
        }

        private class TestHashingService : IHashingService
        {
            private readonly Func<byte[], Action<IHashingContext>?, byte[]> callback;

            public TestHashingService(Func<byte[], Action<IHashingContext>?, byte[]> callback)
            {
                this.callback = callback;
            }

            public byte[] Hash(byte[] value, Action<IHashingContext>? optionsConfig = null)
                => this.callback(value, optionsConfig);
        }
    }
}