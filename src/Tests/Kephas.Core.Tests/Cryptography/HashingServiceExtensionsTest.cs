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
            var hashingService = Substitute.For<IHashingService>();
            var valueString = "123";
            Action<IHashingContext> optionsConfig = ctx => { };
            var hashBytes = Encoding.UTF8.GetBytes("8910");
            hashingService.Hash(Arg.Any<byte[]>(), Arg.Any<Action<IHashingContext>>()).Returns(ci =>
                {
                    var value = Encoding.UTF8.GetString(ci.ArgAt<byte[]>(0));
                    return value == valueString ? hashBytes : null;
                });

            var hash = HashingServiceExtensions.Hash(hashingService, valueString, optionsConfig);
            var hashString = Encoding.UTF8.GetString(hash);
            Assert.AreEqual("8910", hashString);
        }

        [Test]
        public void Hash_string_salt()
        {
            var hashingService = Substitute.For<IHashingService>();
            var valueString = "123";
            var salt = new byte[] { 1, 2, 3 };
            var hashBytes = Encoding.UTF8.GetBytes("8910");
            hashingService.Hash(Arg.Any<byte[]>(), Arg.Any<Action<IHashingContext>>()).Returns(ci =>
            {
                var ctx = Substitute.For<IHashingContext>();
                ci.Arg<Action<IHashingContext>>()(ctx);
                ctx.Received(1).Salt = salt;
                var value = Encoding.UTF8.GetString(ci.ArgAt<byte[]>(0));
                return value == valueString ? hashBytes : null;
            });

            var hash = HashingServiceExtensions.Hash(hashingService, valueString, salt);
            var hashString = Encoding.UTF8.GetString(hash);
            Assert.AreEqual("8910", hashString);
        }
    }
}