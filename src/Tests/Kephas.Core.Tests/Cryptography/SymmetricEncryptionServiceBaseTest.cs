// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymmetricEncryptionServiceBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the symmetric encryption service base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Cryptography
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Cryptography;
    using Kephas.Injection;
    using Kephas.Services;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class SymmetricEncryptionServiceBaseTest : TestBase
    {
        [Test]
        public void GenerateKey_ensure_disposed()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new EncryptionContext(Substitute.For<IInjector>()));
            var alg = Substitute.For<SymmetricAlgorithm>();
            var service = new TestEncryptionService(ctxFactory, ctx => alg);

            var key = service.GenerateKey();
            alg.Received(1).Dispose();
        }

        [Test]
        public void Encrypt_ensure_disposed()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new EncryptionContext(Substitute.For<IInjector>()));
            var alg = Substitute.For<SymmetricAlgorithm>();
            var service = new TestEncryptionService(ctxFactory, ctx => alg);

            var encrypted = service.Encrypt("gigi");
            alg.Received(1).Dispose();
        }

        [Test]
        public async Task EncryptAsync_ensure_disposed()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new EncryptionContext(Substitute.For<IInjector>()));
            var alg = Substitute.For<SymmetricAlgorithm>();
            var service = new TestEncryptionService(ctxFactory, ctx => alg);

            var encrypted = await service.EncryptAsync("gigi");
            alg.Received(1).Dispose();
        }

        [Test]
        public void Decrypt_ensure_disposed()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new EncryptionContext(Substitute.For<IInjector>()));
            var alg = Substitute.For<SymmetricAlgorithm>();
            var service = new TestEncryptionService(ctxFactory, ctx => alg);

            var decrypted = service.Decrypt("EDiOnvVKLypF2p0Fw+gb97AVttPLWpBOXvG9gpk61f76");
            alg.Received(1).Dispose();
        }

        [Test]
        public async Task DecryptAsync_ensure_disposed()
        {
            var ctxFactory = this.CreateContextFactoryMock(() => new EncryptionContext(Substitute.For<IInjector>()));
            var alg = Substitute.For<SymmetricAlgorithm>();
            var service = new TestEncryptionService(ctxFactory, ctx => alg);

            var decrypted = await service.DecryptAsync("EDiOnvVKLypF2p0Fw+gb97AVttPLWpBOXvG9gpk61f76");
            alg.Received(1).Dispose();
        }

        public class TestEncryptionService : SymmetricEncryptionServiceBase<SymmetricAlgorithm>
        {
            private readonly Func<IEncryptionContext, SymmetricAlgorithm> algorithmCtor;

            public TestEncryptionService(IContextFactory contextFactory, Func<IEncryptionContext, SymmetricAlgorithm> algorithmCtor)
                : base(() => contextFactory.CreateContext<EncryptionContext>(), algorithmCtor)
            {
                this.algorithmCtor = algorithmCtor;
            }

            protected override SymmetricAlgorithm CreateSymmetricAlgorithm(IEncryptionContext encryptionContext)
            {
                return this.algorithmCtor(encryptionContext);
            }

            protected override void Decrypt(Stream input, Stream output, SymmetricAlgorithm algorithm, IEncryptionContext encryptionContext)
            {
                var bytes = Encoding.UTF8.GetBytes("decrypted");
                output.Write(bytes, 0, bytes.Length);
            }

            protected override Task DecryptAsync(
                Stream input,
                Stream output,
                SymmetricAlgorithm algorithm,
                IEncryptionContext encryptionContext,
                CancellationToken cancellationToken)
            {
                var bytes = Encoding.UTF8.GetBytes("decryptedAsync");
                return output.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
            }

            protected override void Encrypt(Stream input, Stream output, SymmetricAlgorithm algorithm, IEncryptionContext encryptionContext)
            {
                var bytes = Encoding.UTF8.GetBytes("encrypted");
                output.Write(bytes, 0, bytes.Length);
            }

            protected override Task EncryptAsync(
                Stream input,
                Stream output,
                SymmetricAlgorithm algorithm,
                IEncryptionContext encryptionContext,
                CancellationToken cancellationToken)
            {
                var bytes = Encoding.UTF8.GetBytes("encryptedAsync");
                return output.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
            }
        }
    }
}