// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AesEncryptionServiceTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aes encryption service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Cryptography
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    using Kephas.Cryptography;

    using NUnit.Framework;

    [TestFixture]
    public class AesEncryptionServiceTest
    {
        [Test]
        [TestCase("some text to encrypt", "/gPFNSFpnBLe1jOr7rqEpUPlJ1iM9ZhBCt9hE9DgYlk=")]
        public async Task EncryptAsync(string value, string key)
        {
            var service = new AesEncryptionService();
            var context = new EncryptionContext { Key = this.GetKey(key) };
            var encrypted = await service.EncryptAsync(value, context);
            var decrypted = await service.DecryptAsync(encrypted, context);

            Assert.AreEqual(value, decrypted);
        }

        [Test]
        [TestCase("some text to encrypt", "/gPFNSFpnBLe1jOr7rqEpUPlJ1iM9ZhBCt9hE9DgYlk=")]
        public async Task EncryptAsync_twice_different_values(string value, string key)
        {
            var service = new AesEncryptionService();
            var context = new EncryptionContext { Key = this.GetKey(key) };
            var encrypted1 = await service.EncryptAsync(value, context);
            var encrypted2 = await service.EncryptAsync(value, context);

            Assert.AreNotEqual(encrypted1, encrypted2);
        }

        [Test]
        [TestCase("some text to encrypt", "/gPFNSFpnBLe1jOr7rqEpUPlJ1iM9ZhBCt9hE9DgYlk=")]
        public void Encrypt(string value, string key)
        {
            var service = new AesEncryptionService();
            var context = new EncryptionContext { Key = this.GetKey(key) };
            var encrypted = service.Encrypt(value, context);
            var decrypted = service.Decrypt(encrypted, context);

            Assert.AreEqual(value, decrypted);
        }

        [Test]
        [TestCase("some text to encrypt", "/gPFNSFpnBLe1jOr7rqEpUPlJ1iM9ZhBCt9hE9DgYlk=")]
        public void Encrypt_twice_different_values(string value, string key)
        {
            var service = new AesEncryptionService();
            var context = new EncryptionContext { Key = this.GetKey(key) };
            var encrypted1 = service.Encrypt(value, context);
            var encrypted2 = service.Encrypt(value, context);

            Assert.AreNotEqual(encrypted1, encrypted2);
        }

        [Test]
        [TestCase(256, 32)]
        [TestCase(192, 24)]
        public void GenerateKey(int keySize, int expectedLength)
        {
            var service = new AesEncryptionService();
            var context = new EncryptionContext { KeySize = keySize };
            var key = service.GenerateKey(context);

            Assert.AreEqual(expectedLength, key.Length);
        }

        private string GenerateKey()
        {
            var a = new AesManaged();
            a.GenerateKey();
            return Convert.ToBase64String(a.Key);
        }

        private byte[] GetKey(string key)
        {
            return Convert.FromBase64String(key);
        }
    }
}