// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingServiceBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the hashing service base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Cryptography
{
    using System.Security.Cryptography;
    using System.Text;

    using Kephas.Composition;
    using Kephas.Cryptography;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HashingServiceBaseTest : TestBase
    {
        [Test]
        public void Hash_with_config()
        {
            var service = new TestHashingService(this.CreateContextFactory(() => new HashingContext(Substitute.For<IInjector>())));

            var hash = service.Hash(new byte[] { 1, 2, 3 }, ctx => ctx["hash"] = Encoding.UTF8.GetBytes("hashed!"));
            var actual = Encoding.UTF8.GetString(service.LastHashingContext?["hash"] as byte[]);

            Assert.AreEqual("hashed!", actual);
        }

        public class TestHashingService : HashingServiceBase
        {
            public TestHashingService(IContextFactory contextFactory)
                : base(contextFactory)
            {
            }

            public IHashingContext? LastHashingContext { get; private set; }

            protected override HashAlgorithm CreateHashAlgorithm(IHashingContext? context = null)
            {
                this.LastHashingContext = context;
                return Substitute.For<HashAlgorithm>();
            }
        }
    }
}
