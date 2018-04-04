// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullEncryptionServiceTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null encryption service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Cryptography
{
    using System.IO;
    using System.Threading.Tasks;

    using Kephas.Cryptography;

    using NUnit.Framework;

    [TestFixture]
    public class NullEncryptionServiceTest
    {
        [Test]
        [TestCase(new byte[] { 0, 1, 2 }, new byte[] { 2, 1, 0 })]
        [TestCase(new byte[0], new byte[0])]
        public async Task EncryptAsync(byte[] input, byte[] output)
        {
            var service = new NullEncryptionService();
            using (var inputStream = new MemoryStream(input))
            using (var outputStream = new MemoryStream())
            {
                await service.EncryptAsync(inputStream, outputStream);
                var result = outputStream.ToArray();
                Assert.AreEqual(result.Length, output.Length);
                for (var i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(result[i], output[i]);
                }
            }
        }

        [Test]
        public async Task EncryptAsync_1002_bytes()
        {
            var input = new byte[1002];
            for (var i = 0; i < input.Length; i++)
            {
                input[i] = i < 1000 ? (byte)0 : (byte)(i % 256);
            }

            var service = new NullEncryptionService();
            using (var inputStream = new MemoryStream(input))
            using (var outputStream = new MemoryStream())
            {
                await service.EncryptAsync(inputStream, outputStream);
                var result = outputStream.ToArray();
                Assert.AreEqual(result[1000], (byte)(1001 % 256));
                Assert.AreEqual(result[1001], (byte)(1000 % 256));
            }
        }

        [Test]
        [TestCase(new byte[] { 0, 1, 2 }, new byte[] { 2, 1, 0 })]
        [TestCase(new byte[0], new byte[0])]
        public async Task DecryptAsync(byte[] input, byte[] output)
        {
            var service = new NullEncryptionService();
            using (var inputStream = new MemoryStream(input))
            using (var outputStream = new MemoryStream())
            {
                await service.DecryptAsync(inputStream, outputStream);
                var result = outputStream.ToArray();
                Assert.AreEqual(result.Length, output.Length);
                for (var i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(result[i], output[i]);
                }
            }
        }

        [Test]
        public async Task DecryptAsync_1002_bytes()
        {
            var input = new byte[1002];
            for (var i = 0; i < input.Length; i++)
            {
                input[i] = i < 1000 ? (byte)0 : (byte)(i % 256);
            }

            var service = new NullEncryptionService();
            using (var inputStream = new MemoryStream(input))
            using (var outputStream = new MemoryStream())
            {
                await service.DecryptAsync(inputStream, outputStream);
                var result = outputStream.ToArray();
                Assert.AreEqual(result[1000], (byte)(1001 % 256));
                Assert.AreEqual(result[1001], (byte)(1000 % 256));
            }
        }
    }
}