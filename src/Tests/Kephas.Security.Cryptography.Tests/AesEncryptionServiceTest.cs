// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AesEncryptionServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aes encryption service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Core.Tests.Cryptography
{
    using System;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Kephas.Cryptography;
    using Kephas.Testing;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AesEncryptionServiceTest : TestBase
    {
        [Test]
        [TestCase("some text to encrypt", "/gPFNSFpnBLe1jOr7rqEpUPlJ1iM9ZhBCt9hE9DgYlk=")]
        public async Task EncryptAsync(string value, string key)
        {
            var ctxFactory = this.CreateInjectableFactoryMock(() => new EncryptionContext(Substitute.For<IServiceProvider>()));
            var service = new AesEncryptionService(ctxFactory);
            var encrypted = await service.EncryptAsync(value, ctx => ctx.Key(this.GetKey(key)));
            var decrypted = await service.DecryptAsync(encrypted, ctx => ctx.Key(this.GetKey(key)));

            Assert.AreEqual(value, decrypted);
        }

        [Test]
        [TestCase("some text to encrypt", "/gPFNSFpnBLe1jOr7rqEpUPlJ1iM9ZhBCt9hE9DgYlk=")]
        public async Task EncryptAsync_twice_different_values(string value, string key)
        {
            var ctxFactory = this.CreateInjectableFactoryMock(() => new EncryptionContext(Substitute.For<IServiceProvider>()));
            var service = new AesEncryptionService(ctxFactory);
            var encrypted1 = await service.EncryptAsync(value, ctx => ctx.Key(this.GetKey(key)));
            var encrypted2 = await service.EncryptAsync(value, ctx => ctx.Key(this.GetKey(key)));

            Assert.AreNotEqual(encrypted1, encrypted2);
        }

        [Test]
        [TestCase("some text to encrypt", "/gPFNSFpnBLe1jOr7rqEpUPlJ1iM9ZhBCt9hE9DgYlk=")]
        public void Encrypt(string value, string key)
        {
            var ctxFactory = this.CreateInjectableFactoryMock(() => new EncryptionContext(Substitute.For<IServiceProvider>()));
            var service = new AesEncryptionService(ctxFactory);
            var encrypted = service.Encrypt(value, ctx => ctx.Key(this.GetKey(key)));
            var decrypted = service.Decrypt(encrypted, ctx => ctx.Key(this.GetKey(key)));

            Assert.AreEqual(value, decrypted);
        }

        [Test]
        [TestCase("some text to encrypt", "/gPFNSFpnBLe1jOr7rqEpUPlJ1iM9ZhBCt9hE9DgYlk=")]
        public void Encrypt_twice_different_values(string value, string key)
        {
            var ctxFactory = this.CreateInjectableFactoryMock(() => new EncryptionContext(Substitute.For<IServiceProvider>()));
            var service = new AesEncryptionService(ctxFactory);
            var encrypted1 = service.Encrypt(value, ctx => ctx.Key(this.GetKey(key)));
            var encrypted2 = service.Encrypt(value, ctx => ctx.Key(this.GetKey(key)));

            Assert.AreNotEqual(encrypted1, encrypted2);
        }

        [Test]
        [TestCase(256, 32)]
        [TestCase(192, 24)]
        public void GenerateKey(int keySize, int expectedLength)
        {
            var ctxFactory = this.CreateInjectableFactoryMock(() => new EncryptionContext(Substitute.For<IServiceProvider>()));
            var service = new AesEncryptionService(ctxFactory);
            var key = service.GenerateKey(ctx => ctx.KeySize(keySize));

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