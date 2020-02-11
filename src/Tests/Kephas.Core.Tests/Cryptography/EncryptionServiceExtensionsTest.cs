// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionServiceExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the encryption service extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Cryptography
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Cryptography;

    using NSubstitute;
    using NSubstitute.Core;

    using NUnit.Framework;

    [TestFixture]
    public class EncryptionServiceExtensionsTest
    {
        [Test]
        [TestCase("password", "ZHJvd3NzYXA=")]
        [TestCase("123", "MzIx")]
        public async Task EncryptAsync(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.EncryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            var encrypted = await EncryptionServiceExtensions.EncryptAsync(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }

#if NETCOREAPP3_1
#else
        [Test]
        [TestCase("password", "ZHJvd3NzYXA=")]
        [TestCase("123", "MzIx")]
        public void Encrypt_no_sync_support(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.EncryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Encrypt(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }
#endif

#if NETCOREAPP3_1
        [Test]
        [TestCase("password", "ZHJvd3NzYXA=")]
        [TestCase("123", "MzIx")]
        public void Encrypt_with_sync_support(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.WhenForAnyArgs(s => s.Encrypt(null, null, null))
                .Do(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Encrypt(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }
#else
        [Test]
        [TestCase("password", "ZHJvd3NzYXA=")]
        [TestCase("123", "MzIx")]
        public void Encrypt_with_sync_support(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService, ISyncEncryptionService>();
            var syncEncryptionService = (ISyncEncryptionService)encryptionService;
            syncEncryptionService.WhenForAnyArgs(s => s.Encrypt(null, null, null))
                .Do(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Encrypt(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }
#endif

        [Test]
        [TestCase("ZHJvd3NzYXA=", "password")]
        [TestCase("MzIx", "123")]
        public async Task DecryptAsync(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.DecryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            var encrypted = await EncryptionServiceExtensions.DecryptAsync(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }

#if NETCOREAPP3_1
#else
        [Test]
        [TestCase("ZHJvd3NzYXA=", "password")]
        [TestCase("MzIx", "123")]
        public void Decrypt_no_sync_support(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.DecryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Decrypt(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }
#endif

#if NETCOREAPP3_1
        [Test]
        [TestCase("ZHJvd3NzYXA=", "password")]
        [TestCase("MzIx", "123")]
        public void Decrypt_with_sync_support(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.WhenForAnyArgs(s => s.Decrypt(null, null, null))
                .Do(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Decrypt(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }
#else
        [Test]
        [TestCase("ZHJvd3NzYXA=", "password")]
        [TestCase("MzIx", "123")]
        public void Decrypt_with_sync_support(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService, ISyncEncryptionService>();
            var syncEncryptionService = (ISyncEncryptionService)encryptionService;
            syncEncryptionService.WhenForAnyArgs(s => s.Decrypt(null, null, null))
                .Do(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Decrypt(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }
#endif

        [Test]
        [TestCase("password")]
        [TestCase("123")]
        public async Task EncryptAsync_DecryptAsync_are_inverse(string input)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.EncryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);
            encryptionService.DecryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            var encrypted = await EncryptionServiceExtensions.EncryptAsync(encryptionService, input);
            var decrypted = await EncryptionServiceExtensions.DecryptAsync(encryptionService, encrypted);
            Assert.AreEqual(input, decrypted);
        }

#if NETCOREAPP3_1
        [Test]
        [TestCase("password")]
        [TestCase("123")]
        public void Encrypt_Decrypt_are_inverse_no_sync_support(string input)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.When(s => s.Encrypt(Arg.Any<Stream>(), Arg.Any<Stream>(), Arg.Any<Action<IEncryptionContext>>()))
                .Do(this.ReverseBytes);
            encryptionService.When(s => s.Decrypt(Arg.Any<Stream>(), Arg.Any<Stream>(), Arg.Any<Action<IEncryptionContext>>()))
                .Do(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Encrypt(encryptionService, input);
            var decrypted = EncryptionServiceExtensions.Decrypt(encryptionService, encrypted);
            Assert.AreEqual(input, decrypted);
        }
#else
        [Test]
        [TestCase("password")]
        [TestCase("123")]
        public void Encrypt_Decrypt_are_inverse_no_sync_support(string input)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.EncryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);
            encryptionService.DecryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Encrypt(encryptionService, input);
            var decrypted = EncryptionServiceExtensions.Decrypt(encryptionService, encrypted);
            Assert.AreEqual(input, decrypted);
        }
#endif

#if NETCOREAPP3_1
        [Test]
        [TestCase("password")]
        [TestCase("123")]
        public void Encrypt_Decrypt_are_inverse_with_sync_support(string input)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.WhenForAnyArgs(s => s.Encrypt(null, null, null))
                .Do(this.ReverseBytes);
            encryptionService.WhenForAnyArgs(s => s.Decrypt(null, null, null))
                .Do(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Encrypt(encryptionService, input);
            var decrypted = EncryptionServiceExtensions.Decrypt(encryptionService, encrypted);
            Assert.AreEqual(input, decrypted);
        }
#else
        [Test]
        [TestCase("password")]
        [TestCase("123")]
        public void Encrypt_Decrypt_are_inverse_with_sync_support(string input)
        {
            var encryptionService = Substitute.For<IEncryptionService, ISyncEncryptionService>();
            var syncEncryptionService = (ISyncEncryptionService)encryptionService;
            syncEncryptionService.WhenForAnyArgs(s => s.Encrypt(null, null, null))
                .Do(this.ReverseBytes);
            syncEncryptionService.WhenForAnyArgs(s => s.Decrypt(null, null, null))
                .Do(this.ReverseBytes);

            var encrypted = EncryptionServiceExtensions.Encrypt(encryptionService, input);
            var decrypted = EncryptionServiceExtensions.Decrypt(encryptionService, encrypted);
            Assert.AreEqual(input, decrypted);
        }
#endif

        public void ReverseBytes(CallInfo ci)
        {
            var inputStream = ci.ArgAt<Stream>(0);
            var outputStream = ci.ArgAt<Stream>(1);
            var inputArray = ((MemoryStream)inputStream).ToArray();
            var outputArray = inputArray.Reverse().ToArray();
            outputStream.Write(outputArray, 0, outputArray.Length);
        }
    }
}